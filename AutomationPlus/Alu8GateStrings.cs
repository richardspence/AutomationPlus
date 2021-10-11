using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPlus
{
    class Alu8GateStrings
    {
        public static LocString NAME = (LocString)STRINGS.UI.FormatAsLink("8-Bit Arithmetic Logical Unit", nameof(AluGate));
        public static LocString DESC = (LocString)@"
Operator Codes:   
add = 0x1 (0001)
subtract = 0x2 (0010),
multiply = 0x4 (0100),
modulus = 0x5 (0101),
exp = 0x6 (0110),
divide = 0x8 (1000),
logicalBitRight = 0xD (1101),
logicalBitLeft = 0xE, (1110)";
        public static LocString EFFECT = (LocString)("Performs 8-bit arithmetic operations on the input parameters");
        public static LocString OUTPUT_NAME = (LocString)"XXXOUTPUT";
        public static LocString OUTPUT_ACTIVE = (LocString)("XXXSends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " while receiving " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active) + ". After receiving " + UI.FormatAsAutomationState("Red", UI.AutomationState.Standby) + ", will continue sending " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active) + " until the timer has expired");
        public static LocString OUTPUT_INACTIVE = (LocString)("XXXOtherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ".");

        public static string LOGIC_PORT = "LOGIC_PORT";
        public static string INPUT_PORT1 = "INPUT_PORT1";
        public static string INPUT_PORT2 = "INPUT_PORT1";
        public static string INPUT_PORT_ACTIVE = "INPUT_PORT_ACTIVE";
        public static string INPUT_PORT_INACTIVE = "INPUT_PORT_INACTIVE";
        public static string LOGIC_PORT_OUTPUT = "LOGIC_PORT_OUTPUT";
        public static string OUTPUT_PORT_ACTIVE = "OUTPUT_PORT_ACTIVE";
        public static string OUTPUT_PORT_INACTIVE = "OUTPUT_PORT_INACTIVE";

        public static string OP_PORT_DESCRIPTION { get; internal set; }
        public static string OP_CODE_ACTIVE { get; internal set; }
        public static string OP_CODE_INACTIVE { get; internal set; }
    }
}
