using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private int currentValue =-1;

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
            
        }

        public void OnLogicValueChanged(object data)
        {
            LogicValueChanged logicValueChanged = (LogicValueChanged)data;
            if (logicValueChanged.portID != DisplayAdaptor.INPUT_PORT_ID)
                return;
            var key = logicValueChanged.newValue;
            if (!DisplayInfo.Infos.ContainsKey(key))
            {
                key = 12;
            }
            if(key == currentValue)
            {
                return;
            }
            currentValue = key;
            var info = DisplayInfo.Infos[key];
            this.GetComponent<LogicPorts>().SendSignal(DisplayAdaptor.OUTPUT_PORT_ID1, info.display[0]);
            this.GetComponent<LogicPorts>().SendSignal(DisplayAdaptor.OUTPUT_PORT_ID2, info.display[1]);
            this.GetComponent<LogicPorts>().SendSignal(DisplayAdaptor.OUTPUT_PORT_ID3, info.display[2]);
            this.GetComponent<LogicPorts>().SendSignal(DisplayAdaptor.OUTPUT_PORT_ID4, info.display[3]);
            this.GetComponent<LogicPorts>().SendSignal(DisplayAdaptor.OUTPUT_PORT_ID5, info.display[4]);
        }
    }

    public class DisplayInfo
    {
        public int value;
        public int[] display;

        public static Dictionary<int, DisplayInfo> Infos = new Dictionary<int, DisplayInfo>()
        {
            { 0, new DisplayInfo{ value = 1, display = new []{15,9,9,9,15 } } },
            { 1, new DisplayInfo{ value = 1, display = new []{8,8,8,8,8 } } },
            { 2, new DisplayInfo{ value = 2, display = new []{15,8,15,1,15 } } },
            { 3, new DisplayInfo{ value = 3, display = new []{15,8,14,8,15} } },
            { 4, new DisplayInfo{ value = 4, display = new []{9,9,15,8,8} } },
            { 5, new DisplayInfo{ value = 5, display = new []{15,1,15,8,15} } },
            { 6, new DisplayInfo{ value = 6, display =new []{1,1,15,9,15} } },
            { 7, new DisplayInfo{ value = 7, display = new []{15,8, 4,4,4} } },
            { 8, new DisplayInfo{ value = 8, display = new []{15,9, 15,9,15} } },
            { 9, new DisplayInfo{ value = 9, display = new []{15,9, 15,8,8} } },
            { 10, new DisplayInfo{ value = 10, display = new []{15,15,15,15,15} } },
            { 11, new DisplayInfo{ value = 10, display = new []{0,0,0,0,0} } },
            { 12, new DisplayInfo{ value = 10, display = new []{1,2,0,4,8} } },

            
            //{ 0, new DisplayInfo{ value = 1, display = new []{15,9,9,9,15 } } },
            //{ 1, new DisplayInfo{ value = 1, display = new []{1,1,1,1,1 } } },
            //{ 2, new DisplayInfo{ value = 2, display = new []{15,1,15,8,15 } } },
            //{ 3, new DisplayInfo{ value = 3, display = new []{15,1,14,1,15} } },
            //{ 4, new DisplayInfo{ value = 4, display = new []{9,9,15,1,1} } },
            //{ 5, new DisplayInfo{ value = 5, display = new []{15,8,15,1,15} } },
            //{ 6, new DisplayInfo{ value = 6, display =new []{8,8,15,9,15} } },
            //{ 7, new DisplayInfo{ value = 7, display = new []{15,1, 1,2,2} } },
            //{ 8, new DisplayInfo{ value = 8, display = new []{15,9, 15,9,15} } },
            //{ 9, new DisplayInfo{ value = 9, display = new []{15,9, 15,1,1} } },
            //{ 10, new DisplayInfo{ value = 10, display = new []{15,15,15,15,15} } },
            //{ 11, new DisplayInfo{ value = 10, display = new []{0,0,0,0,0} } },
            //{ 12, new DisplayInfo{ value = 10, display = new []{8,4,0,2,1} } },

        };
    }
}
