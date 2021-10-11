using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPlus
{
    public class DELAYGATE
    {
        public static LocString NAME = (LocString)STRINGS.UI.FormatAsLink("Signal Delay", nameof(DELAYGATE));
        public static LocString DESC = (LocString)"This gate will propagate the input signal at a lter time based on the selected delay.";
        public static LocString EFFECT = (LocString)("Delays the input signal based on the selected delay time");
        public static LocString OUTPUT_NAME = (LocString)"XXXOUTPUT";
        public static LocString OUTPUT_ACTIVE = (LocString)("XXXSends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " while receiving " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active) + ". After receiving " + UI.FormatAsAutomationState("Red", UI.AutomationState.Standby) + ", will continue sending " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active) + " until the timer has expired");
        public static LocString OUTPUT_INACTIVE = (LocString)("XXXOtherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ".");
    }
}
