using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPlus
{
    class AluGateStrings
    {
        public static LocString NAME = (LocString)STRINGS.UI.FormatAsLink("Arithmetic Logical Unit", nameof(AluGate));
        public static LocString DESC = (LocString)"Performs 4-bit arithmetic operations on the input parameters";
        public static LocString EFFECT = (LocString)("Outputs a " + STRINGS.UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " on random bits ");
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
