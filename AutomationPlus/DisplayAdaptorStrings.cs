using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPlus
{
    class DisplayAdaptorStrings
    {
        public static LocString NAME = (LocString)STRINGS.UI.FormatAsLink("Display Adaptor", nameof(DisplayAdaptor));
        public static LocString DESC = (LocString)"Converts a 4-bit signal into a display signal that can be rendered using pixel packs";
        public static LocString EFFECT = (LocString)(@"Converts the ribbon input into a display character that can be displayed using 5 stacked pixel packs

Display codes:
0x0 (0000) through 0x9 (1001) display the numeral on the pixel packs
0xA (1010) Screen fully on
0xB (1011) Screen fully off
0xC (1100) renders a '\'
0xD, 0xE, 0xF -> Reserved

");


        public static string LOGIC_PORT = "LOGIC_PORT";
        public static string INPUT_PORT_ACTIVE = "See description for codes to send";
        public static string INPUT_PORT_INACTIVE = "See description for codes to send";
        public static string LOGIC_PORT_OUTPUT = "RIBBON OUTPUT";
        public static string OUTPUT_PORT_ACTIVE = $"{UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active)}: The specific pixels should be active";
        public static string OUTPUT_PORT_INACTIVE = $"{UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby)}: The specific pixels should be inactive";
    }
}
