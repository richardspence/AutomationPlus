using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                return - ((value ^ minValue) + 1);
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
        //private static readonly EventSystem.IntraObjectHandler<AluGate> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<AluGate>((component, data) => component.OnLogicValueChanged(data));
        protected LogicPorts ports;

        public AluGate() : base()
        {
          
        }

        protected int maxValue = 0xf;
        protected int bits = 4;

        public bool isTwosComplement = true;

        public string SideScreenTitleKey => "Foo";


        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            //this.Subscribe<PnrgGate>(-905833192, PnrgGate.OnCopySettingsDelegate);
        }

        public bool isOpCodeConnected()
        {
            return ports.IsPortConnected(AluGate.OP_PORT_ID);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Action<AluGate, object> p = (c, d) => c.OnLogicValueChanged(d);
            this.Subscribe<AluGate>(-801688580, p);
            this.ports = this.GetComponent<LogicPorts>();
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

        public void OnLogicValueChanged(object data)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, EventArgs.Empty);
            }

            LogicValueChanged logicValueChanged = (LogicValueChanged)data;
            if (logicValueChanged.portID == AluGate.OUTPUT_PORT_ID)
                return;
            RecalcValues();
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

    }

   
}
