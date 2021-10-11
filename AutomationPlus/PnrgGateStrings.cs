using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPlus
{
    public class PnrgGateStrings
    {
        public static LocString NAME = (LocString)STRINGS.UI.FormatAsLink("Random Number Generator", nameof(PnrgGate));
        public static LocString DESC = (LocString)$"On reset, will randomize the bits on the random that are set as {STRINGS.UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active)}.";
        public static LocString EFFECT = (LocString)("Outputs a " + STRINGS.UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " on random bits ");
        public static LocString OUTPUT_NAME = (LocString)"XXXOUTPUT";
        public static LocString OUTPUT_ACTIVE = (LocString)("XXXSends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " while receiving " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active) + ". After receiving " + UI.FormatAsAutomationState("Red", UI.AutomationState.Standby) + ", will continue sending " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active) + " until the timer has expired");
        public static LocString OUTPUT_INACTIVE = (LocString)("XXXOtherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ".");

        public static string LOGIC_PORT = $"{STRINGS.UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active)} To reset the random number";
        public static string INPUT_PORT_ACTIVE = "Regnerate a random number";
        public static string INPUT_PORT_INACTIVE = "";
        public static string LOGIC_PORT_OUTPUT = "The repeated automation signal";
        public static string OUTPUT_PORT_ACTIVE = "The random number";
        public static string OUTPUT_PORT_INACTIVE = "OUTPUT_PORT_INACTIVE";
    }
}
