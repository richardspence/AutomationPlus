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
        private KBatchedAnimController kbac;
      
        public static readonly HashedString INPUT_PORT_ID = new HashedString("DisplayAdaptorInput");
        public static readonly HashedString OUTPUT_PORT_ID1 = new HashedString("DisplayAdaptorOutput1");
        public static readonly HashedString OUTPUT_PORT_ID2 = new HashedString("DisplayAdaptorOutput2");
        public static readonly HashedString OUTPUT_PORT_ID3 = new HashedString("DisplayAdaptorOutput3");
        public static readonly HashedString OUTPUT_PORT_ID4 = new HashedString("DisplayAdaptorOutput4");
        public static readonly HashedString OUTPUT_PORT_ID5 = new HashedString("DisplayAdaptorOutput5");

        private static readonly EventSystem.IntraObjectHandler<DisplayAdaptor> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<DisplayAdaptor>((component, data) => component.OnLogicValueChanged(data));
        private LogicPorts ports;

        private int currentValue = -1;

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
            this.kbac = this.GetComponent<KBatchedAnimController>();
            this.Subscribe<DisplayAdaptor>(-905833192, (IntraObjectHandler<DisplayAdaptor>)((comp, data) => comp.OnCopySettings(data)));
            RefreshAnimations();

        }

        private void RefreshAnimations()
        {
            var nw1 = Game.Instance.logicCircuitManager.GetNetworkForCell(this.ports.GetPortCell(DisplayAdaptor.INPUT_PORT_ID));
            var op1 = Game.Instance.logicCircuitManager.GetNetworkForCell(this.ports.GetPortCell(DisplayAdaptor.OUTPUT_PORT_ID1));
            var op2 = Game.Instance.logicCircuitManager.GetNetworkForCell(this.ports.GetPortCell(DisplayAdaptor.OUTPUT_PORT_ID2));
            var op3 = Game.Instance.logicCircuitManager.GetNetworkForCell(this.ports.GetPortCell(DisplayAdaptor.OUTPUT_PORT_ID3));
            var op4 = Game.Instance.logicCircuitManager.GetNetworkForCell(this.ports.GetPortCell(DisplayAdaptor.OUTPUT_PORT_ID4));
            var op5 = Game.Instance.logicCircuitManager.GetNetworkForCell(this.ports.GetPortCell(DisplayAdaptor.OUTPUT_PORT_ID5));
            if(nw1 != null
                && op1 != null
                && op2 != null
                && op3 != null
                && op4 != null
                && op5 != null
                )
            {
                this.kbac.Play("on_0");
                var info = DisplayInfo.GetDisplayInfo(IsHexMode, currentValue);
                // input = 4   20
                // 1 = input C  0 
                // 2 = 5   4 
                // 3 = 1   8
                // 4 = 2  12
                // 5 = 3  16
                ShowSymbols(nw1, currentValue, 20);
                ShowSymbols(op1, info.RibbonOutputs[0], 0);
                ShowSymbols(op2, info.RibbonOutputs[1], 4);
                ShowSymbols(op3, info.RibbonOutputs[2], 8);
                ShowSymbols(op4, info.RibbonOutputs[3], 12);
                ShowSymbols(op5, info.RibbonOutputs[4], 16);
            }
            else
            {
                this.kbac.Play("off");
                return;
            }
        }

        private void ShowSymbols(LogicCircuitNetwork nw, int value, int endIndex)
        {
            for (int startIndex = endIndex+3; startIndex >= endIndex; startIndex--)
            {
                var bit = startIndex - endIndex;
                ShowSymbolConditionally(nw, () => LogicCircuitNetwork.IsBitActive(bit, value), $"light{startIndex}_bloom_green", $"light{startIndex}_bloom_red");
            }
        }

        private void ShowSymbolConditionally(
          object nw,
        Func<bool> active,
        KAnimHashedString ifTrue,
        KAnimHashedString ifFalse)
        {
            var connected = nw != null;
            kbac.SetSymbolVisiblity(ifTrue, connected && active());
            kbac.SetSymbolVisiblity(ifFalse, connected && !active());
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
            return this.ports?.GetInputValue(DisplayAdaptor.INPUT_PORT_ID) ?? 0;
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
            RefreshAnimations();
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
                value = IsHexMode? 15: 12;
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
