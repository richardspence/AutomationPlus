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
        public static LocString DESC = (LocString)$"On reset, will randomize the output bits that are sent a {STRINGS.UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active)}.";
        public static LocString EFFECT = (LocString)($"Outputs a " + STRINGS.UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + $" on random bits (a random binary number). On every reset a new number is generated causing a different set of bits to be { UI.FormatAsAutomationState("Green", UI.AutomationState.Standby)} and { UI.FormatAsAutomationState("Red", UI.AutomationState.Standby)}");
        public static LocString OUTPUT_NAME = (LocString)"XXXOUTPUT";
        public static LocString OUTPUT_ACTIVE = (LocString)("XXXSends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " while receiving " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active) + ". After receiving " + UI.FormatAsAutomationState("Red", UI.AutomationState.Standby) + ", will continue sending " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active) + " until the timer has expired");
        public static LocString OUTPUT_INACTIVE = (LocString)("XXXOtherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ".");

        public static string LOGIC_PORT = $"Reset Port (R) ";
        public static string INPUT_PORT_ACTIVE = $"{STRINGS.UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active)}: Reset the random number";
        public static string INPUT_PORT_INACTIVE = (UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": No effect");
        public static string LOGIC_PORT_OUTPUT = "Random Output";
        public static string OUTPUT_PORT_ACTIVE = $"Outputs a {UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active)} randomly on each bit";
        public static string OUTPUT_PORT_INACTIVE = $"Outputs a {UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby)} randomly on each bit";
    }
}
