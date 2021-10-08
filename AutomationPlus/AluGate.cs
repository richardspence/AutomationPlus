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

    class AluGate : KMonoBehaviour
    {
        [MyCmpAdd]
        private CopyBuildingSettings copyBuildingSettings;
        public static readonly HashedString INPUT_PORT_ID1 = new HashedString("AluGateInput1");
        public static readonly HashedString INPUT_PORT_ID2 = new HashedString("AluGateInput2");
        public static readonly HashedString OUTPUT_PORT_ID = new HashedString("AluGateOutput");
        public static readonly HashedString OP_PORT_ID = new HashedString("AluGateOpCode");
        [Serialize]
        private int currentValue = 0;
        [Serialize]
        public AluGateOperators opCode = AluGateOperators.none;
        //private static readonly EventSystem.IntraObjectHandler<AluGate> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<AluGate>((component, data) => component.OnLogicValueChanged(data));
        private LogicPorts ports;

        public AluGate() : base()
        {
            if (isTwosComplement)
            {
                minValue = 1 << (signBit - 1);
            }
        }

        private int maxValue = 0xf;
        private int signBit = 4;
        private int minValue =0;

        public bool isTwosComplement = true;
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

        public int GetInputValue1()
        {
            return this.ports?.GetInputValue(AluGate.INPUT_PORT_ID1) ?? 0;
        }

        public int GetInputValue2()
        {
            return this.ports?.GetInputValue(AluGate.INPUT_PORT_ID2) ?? 0;
        }

        private bool isNegativeValue(int value)
        {
            if (isTwosComplement)
            {
                var isNegative = (value & minValue) == minValue;
                return isNegative;
            }
            return false;
        }

        private int getTwosComplement(int value)
        {
            if (isNegativeValue(value))
            {
                return (value ^ minValue) + 1;
            }
            else
            {
                return value;
            }
        }

        public AluGateOperators GetOpCode()
        {

            var input = this.ports?.GetInputValue(AluGate.OP_PORT_ID);
            if(input == null || !this.isOpCodeConnected())
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

        public void OnLogicValueChanged(object data)
        {
            LogicValueChanged logicValueChanged = (LogicValueChanged)data;
            Debug.Log($"AluOLV: {logicValueChanged.portID} {logicValueChanged.newValue} {this.GetOpCode()}, {this.GetInputValue1()}, {this.GetInputValue2()}");
            if (logicValueChanged.portID == AluGate.OUTPUT_PORT_ID)
                return;
            var lhs = this.GetInputValue1();
            var rhs = this.GetInputValue2();
            var currentValue = 0;
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
                    currentValue = (int)Math.Pow(lhs,  rhs);
                    break;
                case AluGateOperators.divide:
                    if(rhs != 0) {
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
            this.GetComponent<LogicPorts>().SendSignal(AluGate.OUTPUT_PORT_ID, currentValue);
        }
    }
}
