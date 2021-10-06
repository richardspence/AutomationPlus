using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPlus
{
    [Flags]
    enum AluGateOperators
    {
        none = 0x0,
        add = 0x1,
        subtract = 0x2,
        multiply = 0x4,
        modulus = 0x5,
        exp = 0x6,
        divide = 0x8,
        //arithmeticBitRight = 0x9,
        //arithmeticBitLeft = 0x10,
        logicalBitRight = 0x13,
        logicalBitLeft = 0x14,
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
        //private static readonly EventSystem.IntraObjectHandler<AluGate> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<AluGate>((component, data) => component.OnLogicValueChanged(data));
        private LogicPorts ports;


        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            //this.Subscribe<PnrgGate>(-905833192, PnrgGate.OnCopySettingsDelegate);
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
            LogicPorts component = this.GetComponent<LogicPorts>();
            return component?.GetInputValue(AluGate.INPUT_PORT_ID1) ?? 0;
        }

        public int GetInputValue2()
        {
            LogicPorts component = this.GetComponent<LogicPorts>();
            return component?.GetInputValue(AluGate.INPUT_PORT_ID2) ?? 0;
        }


        public AluGateOperators GetOpCode()
        {
            LogicPorts component = this.GetComponent<LogicPorts>();
            var input = component?.GetInputValue(AluGate.OP_PORT_ID) ?? 0x0;
            return (AluGateOperators)input;
        }

        public void OnLogicValueChanged(object data)
        {
            LogicValueChanged logicValueChanged = (LogicValueChanged)data;
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
                    currentValue = lhs - rhs;
                    break;
                case AluGateOperators.multiply:
                    currentValue = lhs * rhs;
                    break;
                case AluGateOperators.modulus:
                    currentValue = lhs % rhs;
                    break;
                case AluGateOperators.exp:
                    currentValue = (int)Math.Pow(lhs,  rhs);
                    break;
                case AluGateOperators.divide:
                    currentValue = lhs / rhs;
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
            currentValue = currentValue & 15;
            this.GetComponent<LogicPorts>().SendSignal(AluGate.OUTPUT_PORT_ID, currentValue);
        }
    }
}
