using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPlus
{
    class Alu8Gate : AluGate
    {
        public static readonly HashedString INPUT_PORT_ID1B = new HashedString("AluGateInput1");
        public static readonly HashedString INPUT_PORT_ID2B = new HashedString("AluGateInput2");
        public static readonly HashedString OUTPUT_PORT_IDB = new HashedString("AluGateOutput");

        public Alu8Gate() : base()
        {
            this.bits = 8;
            this.maxValue = 0xff;
        }

        public override int GetInputValue1()
        {
            var val1 = base.GetInputValue1();
            var val2 = this.ports?.GetInputValue(Alu8Gate.INPUT_PORT_ID1B) ?? 0;

            return (val1 << 3) | val2;

        }

        public override int GetInputValue2()
        {
            var val1 = base.GetInputValue2();
            var val2 = this.ports?.GetInputValue(Alu8Gate.INPUT_PORT_ID2B) ?? 0;

            return (val1 << 3) | val2;

        }

        protected virtual void UpdateValue()
        {
            var val1 = this.currentValue >> 3;
            var val2 = this.currentValue | 0xf;
            this.GetComponent<LogicPorts>().SendSignal(AluGate.OUTPUT_PORT_ID, val1);
            this.GetComponent<LogicPorts>().SendSignal(Alu8Gate.OUTPUT_PORT_IDB, val2);
        }
    }
}
