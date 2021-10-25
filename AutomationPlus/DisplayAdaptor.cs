using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static EventSystem;

namespace AutomationPlus
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class DisplayAdaptor : KMonoBehaviour
    {
        [MyCmpAdd]
        private CopyBuildingSettings copyBuildingSettings;
        public static readonly HashedString INPUT_PORT_ID = new HashedString("DisplayAdaptorInput");
        public static readonly HashedString OUTPUT_PORT_ID1 = new HashedString("DisplayAdaptorOutput1");
        public static readonly HashedString OUTPUT_PORT_ID2 = new HashedString("DisplayAdaptorOutput2");
        public static readonly HashedString OUTPUT_PORT_ID3 = new HashedString("DisplayAdaptorOutput3");
        public static readonly HashedString OUTPUT_PORT_ID4 = new HashedString("DisplayAdaptorOutput4");
        public static readonly HashedString OUTPUT_PORT_ID5 = new HashedString("DisplayAdaptorOutput5");

        private static readonly EventSystem.IntraObjectHandler<DisplayAdaptor> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<DisplayAdaptor>((component, data) => component.OnLogicValueChanged(data));
        private LogicPorts ports;

        [Serialize]
        private int currentValue = 0;

        [Serialize]
        private bool _isHexMode = true;

        public bool IsHexMode
        {
            get { return _isHexMode; }
            set
            {
                _isHexMode = value;
                RefreshOutput();
            }
        }


        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            //this.Subscribe<PnrgGate>(-905833192, PnrgGate.OnCopySettingsDelegate);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            this.Subscribe(-801688580, DisplayAdaptor.OnLogicValueChangedDelegate);
            this.ports = this.GetComponent<LogicPorts>();
            this.Subscribe<DisplayAdaptor>(-905833192, (IntraObjectHandler<DisplayAdaptor>)((comp, data) => comp.OnCopySettings(data)));

        
        }

        private void OnCopySettings(object data)
        {
            DisplayAdaptor component = ((GameObject)data).GetComponent<DisplayAdaptor>();
            if (component == null)
                return;
            this.IsHexMode = component.IsHexMode;
        }

        public int GetInputValue()
        {
            return currentValue;
        }

        public string GetInputDisplayValue()
        {
            var formatString = $"Input: {{0:H:{4}}} {{0:B:{4}}} Display: {{1}}";
            var formatter = new BinaryFormatter(false);

            return String.Format(formatter, formatString, GetInputValue(), DisplayInfo.GetDisplayInfo(IsHexMode, currentValue).DisplayValue);
        }

        public void OnLogicValueChanged(object data)
        {
            LogicValueChanged logicValueChanged = (LogicValueChanged)data;
            if (logicValueChanged.portID != DisplayAdaptor.INPUT_PORT_ID)
                return;
            if (currentValue != GetInputValue())
            {
                currentValue = GetInputValue();
                RefreshOutput();
            }
        }

        private void RefreshOutput()
        {
            var key = currentValue;
            var info = DisplayInfo.GetDisplayInfo(IsHexMode, key);
            this.GetComponent<LogicPorts>().SendSignal(DisplayAdaptor.OUTPUT_PORT_ID1, info.RibbonOutputs[0]);
            this.GetComponent<LogicPorts>().SendSignal(DisplayAdaptor.OUTPUT_PORT_ID2, info.RibbonOutputs[1]);
            this.GetComponent<LogicPorts>().SendSignal(DisplayAdaptor.OUTPUT_PORT_ID3, info.RibbonOutputs[2]);
            this.GetComponent<LogicPorts>().SendSignal(DisplayAdaptor.OUTPUT_PORT_ID4, info.RibbonOutputs[3]);
            this.GetComponent<LogicPorts>().SendSignal(DisplayAdaptor.OUTPUT_PORT_ID5, info.RibbonOutputs[4]);
        }
    }

    public class DisplayInfo
    {
        public string DisplayValue;
        public int[] RibbonOutputs;

        public static Dictionary<int, DisplayInfo> GetDisplayMap(bool IsHexMode)
        {
            if (IsHexMode)
            {
                return DisplayInfo.HexMap;
            }
            else
            {
                return DisplayInfo.DisplayMap;
            }
        }

        public static DisplayInfo GetDisplayInfo(bool IsHexMode, int value)
        {
            var displayMap = GetDisplayMap(IsHexMode);
            if (!displayMap.ContainsKey(value))
            {
                value = 12;
            }

            return displayMap[value];
        }

        private static Dictionary<int, DisplayInfo> DisplayMap = new Dictionary<int, DisplayInfo>()
        {
            { 0, new DisplayInfo{ DisplayValue = "0", RibbonOutputs = new []{15,9,9,9,15 } } },
            { 1, new DisplayInfo{ DisplayValue = "1", RibbonOutputs = new []{8,8,8,8,8 } } },
            { 2, new DisplayInfo{ DisplayValue = "2", RibbonOutputs = new []{15,8,15,1,15 } } },
            { 3, new DisplayInfo{ DisplayValue = "3", RibbonOutputs = new []{15,8,14,8,15} } },
            { 4, new DisplayInfo{ DisplayValue = "4", RibbonOutputs = new []{5,5,15,4,4} } },
            { 5, new DisplayInfo{ DisplayValue = "5", RibbonOutputs = new []{15,1,15,8,15} } },
            { 6, new DisplayInfo{ DisplayValue = "6", RibbonOutputs =new []{1,1,15,9,15} } },
            { 7, new DisplayInfo{ DisplayValue = "7", RibbonOutputs = new []{15,8, 4,4,4} } },
            { 8, new DisplayInfo{ DisplayValue = "8", RibbonOutputs = new []{15,9, 15,9,15} } },
            { 9, new DisplayInfo{ DisplayValue = "9", RibbonOutputs = new []{15,9, 15,8,8} } },
            { 10, new DisplayInfo{ DisplayValue = "Fill", RibbonOutputs = new []{15,15,15,15,15} } },
            { 11, new DisplayInfo{ DisplayValue = "Clear", RibbonOutputs = new []{0,0,0,0,0} } },
            { 12, new DisplayInfo{ DisplayValue = "Unkown", RibbonOutputs = new []{1,2,0,4,8} } },
        };

        private static Dictionary<int, DisplayInfo> HexMap = new Dictionary<int, DisplayInfo>()
        {
            { 0, new DisplayInfo{ DisplayValue = "0x0", RibbonOutputs = new []{15,9,9,9,15 } } },
            { 1, new DisplayInfo{ DisplayValue = "0x1", RibbonOutputs = new []{8,8,8,8,8 } } },
            { 2, new DisplayInfo{ DisplayValue = "0x2", RibbonOutputs = new []{15,8,15,1,15 } } },
            { 3, new DisplayInfo{ DisplayValue = "0x3", RibbonOutputs = new []{15,8,14,8,15} } },
            { 4, new DisplayInfo{ DisplayValue = "0x4", RibbonOutputs = new []{5,5,15,4,4} } },
            { 5, new DisplayInfo{ DisplayValue = "0x5", RibbonOutputs = new []{15,1,15,8,15} } },
            { 6, new DisplayInfo{ DisplayValue = "0x6", RibbonOutputs =new []{1,1,15,9,15} } },
            { 7, new DisplayInfo{ DisplayValue = "0x7", RibbonOutputs = new []{15,8, 4,4,4} } },
            { 8, new DisplayInfo{ DisplayValue = "0x8", RibbonOutputs = new []{15,9, 15,9,15} } },
            { 9, new DisplayInfo{ DisplayValue = "0x9", RibbonOutputs = new []{15,9, 15,8,8} } },
            { 10, new DisplayInfo{ DisplayValue = "0xA", RibbonOutputs = new []{15,9,15,9,9} } },
            { 11, new DisplayInfo{ DisplayValue = "0xB", RibbonOutputs = new []{15,9,7,9,15} } },
            { 12, new DisplayInfo{ DisplayValue = "0xC", RibbonOutputs = new []{15,1,1,1,15 } } },
            { 13, new DisplayInfo{ DisplayValue = "0xD", RibbonOutputs = new []{7,9,9,9, 7 } } },
            { 14, new DisplayInfo{ DisplayValue = "0xE", RibbonOutputs = new []{15,1,7,1,15} } },
            { 15, new DisplayInfo{ DisplayValue = "0xF", RibbonOutputs = new []{15,1,7,1,1 } } },
        };
    }
}
