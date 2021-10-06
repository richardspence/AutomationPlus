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
        public static LocString NAME = (LocString)STRINGS.UI.FormatAsLink("DELAY Gate Gate", nameof(DELAYGATE));
        public static LocString DESC = (LocString)"XXXThis gate continues outputting a Green Signal for a short time after the gate stops receiving a Green Signal.";
        public static LocString EFFECT = (LocString)("XXXOutputs a " + STRINGS.UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if the Input is receiving a " + STRINGS.UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ".\n\nContinues sending a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " for an amount of buffer time after the Input receives a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ".");
        public static LocString OUTPUT_NAME = (LocString)"XXXOUTPUT";
        public static LocString OUTPUT_ACTIVE = (LocString)("XXXSends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " while receiving " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active) + ". After receiving " + UI.FormatAsAutomationState("Red", UI.AutomationState.Standby) + ", will continue sending " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active) + " until the timer has expired");
        public static LocString OUTPUT_INACTIVE = (LocString)("XXXOtherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ".");
    }
}
