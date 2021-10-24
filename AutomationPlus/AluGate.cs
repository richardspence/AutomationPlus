using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static EventSystem;

namespace AutomationPlus
{
    enum AluGateOperators
    {
        none = 0x0,
        add = 0x1,
        subtract = 0x2,
        multiply = 0x4,
        modulus = 0x5,
        exp = 0x6,
        divide = 0x8,
        logicalBitRight = 0xD,
        logicalBitLeft = 0xE,
    }

    class BinaryFormatter : IFormatProvider, ICustomFormatter
    {
        private bool isTwosComplement;
        public BinaryFormatter(bool isTwosComplement)
        {
            this.isTwosComplement = isTwosComplement;
        }
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return this;
            else
                return null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (!(arg is int))
            {
                if (!string.IsNullOrEmpty(format))
                {
                    return string.Format(format, arg);
                }
                else
                {
                    return arg.ToString();
                }
            }
            // Check whether this is an appropriate callback
            if (!this.Equals(formatProvider))
                return null;
            var components = (format ?? "D").Split(':');
            var primary = components[0];
            var bits = 4;
            if (components.Length > 1)
            {
                bits = Convert.ToInt32(components[1]);
            }

            string numericString = arg.ToString();
            int value = (int)arg;
            var normalValue = value;
            if (isTwosComplement)
            {
                normalValue = BinaryUtils.GetValue(value, bits);
            }
            if (primary == "H")
            {
                return "0x" + Convert.ToString(value, 16);
            }
            if (primary == "D")
            {
                return normalValue.ToString();
            }
            else if (primary == "B")
            {
                var binaryValue = "";
                for (int i = 1; i <= bits; i++)
                {
                    var bit = (1 << (bits - i));
                    var hasBit = (value & bit) == bit;
                    if (hasBit)
                    {
                        binaryValue += "1";
                    }
                    else
                    {
                        binaryValue += "0";
                    }

                }
                return binaryValue;
            }
            else if (primary == "F")
            {
                return $"{this.Format("D", arg, formatProvider)} ({this.Format($"B:{bits}", arg, formatProvider)})";
            }
            return numericString;
        }
    }

    class BinaryUtils
    {
        private static Dictionary<int, int> _minValues = new Dictionary<int, int>()
        {
            {4, 1 << 3 },
            {8, 1 << 7 }
        };

        public static bool IsNegative(int value, int bits)
        {
            var minValue = _minValues[bits];
            var isNegative = (value & minValue) == minValue;
            return isNegative;
        }

        public static int GetTwosComplement(int value, int bits)
        {
            var minValue = _minValues[bits];
            if (IsNegative(value, bits))
            {
                return (value ^ minValue) + 1;
            }
            else
            {
                return value;
            }
        }

        public static int GetValue(int value, int bits)
        {
            var minValue = _minValues[bits];
            if (IsNegative(value, bits))
            {
                return -((value ^ minValue) + 1);
            }
            else
            {
                return value;
            }
        }
    }

    struct AluValues
    {
        public string inputValue1;
        public string inputValue2;
        public string outputValue;
    }

    class AluGate : KMonoBehaviour
    {
        [MyCmpAdd]
        private CopyBuildingSettings copyBuildingSettings;
        public static readonly HashedString INPUT_PORT_ID1 = new HashedString("AluGateInput1");
        public static readonly HashedString INPUT_PORT_ID2 = new HashedString("AluGateInput2");
        public static readonly HashedString OUTPUT_PORT_ID = new HashedString("AluGateOutput");
        public static readonly HashedString OP_PORT_ID = new HashedString("AluGateOpCode");

        public event EventHandler ValueChanged;
        private KBatchedAnimController kbac;

        [Serialize]
        protected int currentValue = 0;
        [Serialize]
        private AluGateOperators _opCode = AluGateOperators.none;
        public AluGateOperators opCode
        {
            get
            {
                return _opCode;
            }
            set
            {
                _opCode = value;
                this.RecalcValues();
            }
        }

        private Color activeTintColor = new Color(137 / 255f, 252 / 255f, 76 / 255f);
        private Color inactiveTintColor = Color.red;
        private Color disabledColor = new Color(79 / 255f, 93 / 255f, 71 / 255f); //#4F5D47

        protected LogicPorts ports;

        public AluGate() : base()
        {

        }

        protected int maxValue = 0xf;
        protected int bits = 4;
        [Serialize]
        private bool _isTwosComplement;

        public bool isTwosComplement
        {
            get { return _isTwosComplement; }
            set
            {
                _isTwosComplement = value;
                RefreshAnimations();
            }
        }




        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.Subscribe<AluGate>(-905833192, (IntraObjectHandler<AluGate>)((comp, data) => comp.OnCopySettings(data)));
        }

        private void OnCopySettings(object data)
        {
            AluGate component = ((GameObject)data).GetComponent<AluGate>();
            if (!((UnityEngine.Object)component != (UnityEngine.Object)null))
                return;
            this.isTwosComplement = component.isTwosComplement;
            this.opCode = component.opCode;
        }

        public bool isOpCodeConnected()
        {
            return ports.IsPortConnected(AluGate.OP_PORT_ID);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Action<AluGate, object> p = (c, d) => c.OnLogicValueChanged(d);
            this.kbac = this.GetComponent<KBatchedAnimController>();
            this.kbac.Play((HashedString)"off");
            this.Subscribe<AluGate>(-801688580, p);
            this.ports = this.GetComponent<LogicPorts>();
            RefreshAnimations();
        }

        public virtual int GetInputValue1()
        {
            return this.ports?.GetInputValue(AluGate.INPUT_PORT_ID1) ?? 0;
        }

        public virtual int GetInputValue2()
        {
            return this.ports?.GetInputValue(AluGate.INPUT_PORT_ID2) ?? 0;
        }

        public AluValues getValues()
        {
            var formatString = $"{{0:F:{this.bits}}}";
            var formatter = new BinaryFormatter(isTwosComplement);
            return new AluValues
            {
                inputValue1 = String.Format(formatter, formatString, GetInputValue1()),
                inputValue2 = String.Format(formatter, formatString, GetInputValue2()),
                outputValue = String.Format(formatter, formatString, currentValue),
            };
        }


        private int getTwosComplement(int value)
        {
            return BinaryUtils.GetTwosComplement(value, bits);
        }

        public AluGateOperators GetOpCode()
        {

            var input = this.ports?.GetInputValue(AluGate.OP_PORT_ID);
            if (input == null || !this.isOpCodeConnected())
            {
                return this.opCode;
            }
            return (AluGateOperators)input;
        }

        public int checkOverflow(int value)
        {
            while (value > maxValue)
            {
                value -= (maxValue + 1);
            }
            while (value < 0)
            {
                currentValue += maxValue;
            }
            return value;
        }

        protected virtual void UpdateValue()
        {
            this.GetComponent<LogicPorts>().SendSignal(AluGate.OUTPUT_PORT_ID, currentValue);
        }

        protected virtual bool ShouldRecalcValue(LogicValueChanged logicValueChanged) {
            return !(logicValueChanged.portID == AluGate.OUTPUT_PORT_ID);
        }

        public void OnLogicValueChanged(object data)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, EventArgs.Empty);
            }

            LogicValueChanged logicValueChanged = (LogicValueChanged)data;
            if (!ShouldRecalcValue(logicValueChanged))
            {
                RefreshAnimations();
                return;
            }
            RecalcValues();
            RefreshAnimations();
        }


        private void RefreshAnimations()
        {
            var val1 = GetInputValue1();
            var val2 = GetInputValue2();
            var nw1 = Game.Instance.logicCircuitManager.GetNetworkForCell(this.ports.GetPortCell(AluGate.INPUT_PORT_ID1));
            var nw2 = Game.Instance.logicCircuitManager.GetNetworkForCell(this.ports.GetPortCell(AluGate.INPUT_PORT_ID2));
            var nwOp = Game.Instance.logicCircuitManager.GetNetworkForCell(this.ports.GetPortCell(AluGate.OP_PORT_ID));
            var nwOut = Game.Instance.logicCircuitManager.GetNetworkForCell(this.ports.GetPortCell(AluGate.OUTPUT_PORT_ID));

            if (true)
            {
                this.TintSymbolConditionally(nwOp, () => nwOp.OutputValue > 0, this.kbac, "light5_bloom");

                this.TintSymbolConditionally(nwOut, () => LogicCircuitNetwork.IsBitActive(3, currentValue), this.kbac, "light1_bloom");
                this.TintSymbolConditionally(nwOut, () => LogicCircuitNetwork.IsBitActive(2, currentValue), this.kbac, "light2_bloom");
                this.TintSymbolConditionally(nwOut, () => LogicCircuitNetwork.IsBitActive(1, currentValue), this.kbac, "light3_bloom");
                this.TintSymbolConditionally(nwOut, () => LogicCircuitNetwork.IsBitActive(0, currentValue), this.kbac, "light4_bloom");

                this.TintSymbolConditionally(nw2, () => LogicCircuitNetwork.IsBitActive(3, val2), this.kbac, "light6_bloom");
                this.TintSymbolConditionally(nw2, () => LogicCircuitNetwork.IsBitActive(2, val2), this.kbac, "light7_bloom");
                this.TintSymbolConditionally(nw2, () => LogicCircuitNetwork.IsBitActive(1, val2), this.kbac, "light8_bloom");
                this.TintSymbolConditionally(nw2, () => LogicCircuitNetwork.IsBitActive(0, val2), this.kbac, "light9_bloom");

                this.TintSymbolConditionally(nw1, () => LogicCircuitNetwork.IsBitActive(3, val1), this.kbac, "light10_bloom");
                this.TintSymbolConditionally(nw1, () => LogicCircuitNetwork.IsBitActive(2, val1), this.kbac, "light11_bloom");
                this.TintSymbolConditionally(nw1, () => LogicCircuitNetwork.IsBitActive(1, val1), this.kbac, "light12_bloom");
                this.TintSymbolConditionally(nw1, () => LogicCircuitNetwork.IsBitActive(0, val1), this.kbac, "light13_bloom");
            }
            else
            {
                if (nw1 != null && nw2 != null && nwOut != null)
                {
                    this.kbac.Play("on_0");
                    ShowSymbolConditionally(nwOp?.OutputValue > 0, $"light_bloom_green_{4}", $"light_bloom_red_{4}");

                    ShowSymbolConditionally(LogicCircuitNetwork.IsBitActive(3, currentValue), $"light_bloom_green_{0}", $"light_bloom_red_{0}");
                    ShowSymbolConditionally(LogicCircuitNetwork.IsBitActive(2, currentValue), $"light_bloom_green_{1}", $"light_bloom_red_{1}");
                    ShowSymbolConditionally(LogicCircuitNetwork.IsBitActive(1, currentValue), $"light_bloom_green_{2}", $"light_bloom_red_{2}");
                    ShowSymbolConditionally(LogicCircuitNetwork.IsBitActive(0, currentValue), $"light_bloom_green_{3}", $"light_bloom_red_{3}");

                    ShowSymbolConditionally(LogicCircuitNetwork.IsBitActive(3, val2), $"light_bloom_green_{5}", $"light_bloom_red_{5}");
                    ShowSymbolConditionally(LogicCircuitNetwork.IsBitActive(2, val2), $"light_bloom_green_{6}", $"light_bloom_red_{6}");
                    ShowSymbolConditionally(LogicCircuitNetwork.IsBitActive(1, val2), $"light_bloom_green_{7}", $"light_bloom_red_{7}");
                    ShowSymbolConditionally(LogicCircuitNetwork.IsBitActive(0, val2), $"light_bloom_green_{8}", $"light_bloom_red_{8}");

                    ShowSymbolConditionally(LogicCircuitNetwork.IsBitActive(3, val1), $"light_bloom_green_{9}", $"light_bloom_red_{9}");
                    ShowSymbolConditionally(LogicCircuitNetwork.IsBitActive(2, val1), $"light_bloom_green_{10}", $"light_bloom_red_{10}");
                    ShowSymbolConditionally(LogicCircuitNetwork.IsBitActive(1, val1), $"light_bloom_green_{11}", $"light_bloom_red_{11}");
                    ShowSymbolConditionally(LogicCircuitNetwork.IsBitActive(0, val1), $"light_bloom_green_{12}", $"light_bloom_red_{12}");

                }
                else
                {
                    this.kbac.Play("off");
                }
            }


            if (nw1 != null && nw2 != null && nwOut != null)
            {
                LogicCircuitManager.ToggleNoWireConnected(false, this.gameObject);
            }
        }

        private void RecalcValues()
        {
            var lhs = this.GetInputValue1();
            var rhs = this.GetInputValue2();
            currentValue = 0;
            switch (this.GetOpCode())
            {
                case AluGateOperators.add:
                    currentValue = lhs + rhs;
                    break;
                case AluGateOperators.subtract:
                    if (isTwosComplement)
                    {
                        var twosComplement = getTwosComplement(rhs);
                        currentValue = lhs + twosComplement;
                    }
                    else
                    {
                        currentValue = lhs - rhs;
                    }
                    break;
                case AluGateOperators.multiply:
                    currentValue = lhs * rhs;
                    break;
                case AluGateOperators.modulus:
                    if (rhs != 0)
                    {
                        currentValue = lhs % rhs;
                    }
                    else
                    {
                        currentValue = 0;
                    }
                    break;
                case AluGateOperators.exp:
                    currentValue = (int)Math.Pow(lhs, rhs);
                    break;
                case AluGateOperators.divide:
                    if (rhs != 0)
                    {
                        currentValue = lhs / rhs;
                    }
                    else
                    {
                        currentValue = 0;
                    }
                    break;
                case AluGateOperators.logicalBitRight:
                    currentValue = lhs >> rhs;
                    break;
                case AluGateOperators.logicalBitLeft:
                    currentValue = lhs << rhs;
                    break;
                case AluGateOperators.none:
                default:
                    break;
            }
            // reduce the values
            currentValue = checkOverflow(currentValue);
            this.UpdateValue();
        }



        private void ShowSymbolConditionally(
          bool active,
          KAnimHashedString ifTrue,
          KAnimHashedString ifFalse)
        {
            kbac.SetSymbolVisiblity(ifTrue, active);
            kbac.SetSymbolVisiblity(ifFalse, !active);
        }

        private void TintSymbolConditionally(
          object tintAnything,
          Func<bool> condition,
          KBatchedAnimController kbac,
          KAnimHashedString symbol
         )
        {
            if (tintAnything != null)
                kbac.SetSymbolTint(symbol, condition() ? activeTintColor : inactiveTintColor);
            else
                kbac.SetSymbolTint(symbol, disabledColor);
        }

        private void SetBloomSymbolShowing(
          bool showing,
          KBatchedAnimController kbac,
          KAnimHashedString symbol,
          KAnimHashedString bloomSymbol)
        {
            kbac.SetSymbolVisiblity(bloomSymbol, showing);
            kbac.SetSymbolVisiblity(symbol, !showing);
        }
    }


}
