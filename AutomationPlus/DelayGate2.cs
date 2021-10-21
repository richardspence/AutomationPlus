using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static EventSystem;

namespace AutomationPlus
{
    static class DelayGateInfoPool
    {
        private static List<DelayGateInfo> _pool = new List<DelayGateInfo>();
        public static DelayGateInfo getOrCreate()
        {
            var result = _pool.LastOrDefault();
            if (result != null)
            {
                _pool.RemoveAt(_pool.Count - 1);
                return result;
            }
            return new DelayGateInfo();
        }

        public static void release(DelayGateInfo obj)
        {
            // 10 is our max pool size
            if (DelayGateInfoPool._pool.Count < 10)
            {
                DelayGateInfoPool._pool.Add(obj);
                obj.ticks = 0;
                obj.value = 0;
            }
        }
    }

    [Serializable]
    public class DelayGateInfo
    {
        public int value;
        public int ticks = 0;
    }
    [Serializable]
    public class DelayGateInfoObj
    {
        public int sumTicks = 0;
        public void Add(DelayGateInfo info)
        {
            this.journal.Add(info);
        }

        public void clear()
        {
            this.journal.ForEach(DelayGateInfoPool.release);
            this.journal.Clear();
            this.sumTicks = 0;
        }

        public bool AddTick(int maxTicks)
        {
            var didChange = sumTicks + 1 == maxTicks;
            var last = this.journal.LastOrDefault();
            if (last != null)
            {
                last.ticks++;
                didChange = last.ticks == 1;
                sumTicks++;

                while (sumTicks > maxTicks)
                {
                    var delta = sumTicks - maxTicks;
                    var first = this.journal.FirstOrDefault();
                    if (first == null)
                    {
                        this.sumTicks = 0;
                        didChange = true;
                        break;
                    }
                    if (first.ticks > delta)
                    {
                        first.ticks -= delta;
                        sumTicks -= delta;
                    }
                    else
                    {
                        sumTicks -= first.ticks;
                        didChange = true;
                        this.journal.Remove(first);
                        DelayGateInfoPool.release(first);
                    }
                }
            }

            return didChange;
        }

        public List<DelayGateInfo> journal = new List<DelayGateInfo>();
    }

    [SerializationConfig(MemberSerialization.OptIn)]
    class DelayGate : KMonoBehaviour, ISingleSliderControl, ISliderControl, ILogicEventSender, IRenderEveryTick
    {
        [Serialize]
        private float delayAmount = .1f;
        [Serialize]
        private DelayGateInfoObj info = new DelayGateInfoObj();

        [MyCmpAdd]
        private CopyBuildingSettings copyBuildingSettings;
        public static readonly HashedString INPUT_PORT_ID = new HashedString("DelayGateInput");
        public static readonly HashedString OUTPUT_PORT_ID = new HashedString("DelayGateOutput");
        private System.Random _random = new System.Random();

        private static readonly EventSystem.IntraObjectHandler<DelayGate> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<DelayGate>((component, data) => component.OnLogicValueChanged(data));
        private LogicPorts ports;
        private KBatchedAnimController kbac;

        public float DelayAmount
        {
            get => this.delayAmount;
            set
            {
                this.delayAmount = value;
            }
        }
        [Serialize]
        private int CurrentValue;
        private bool connected;
        private LogicPortVisualizer outputOne;
        private float elapsedTime;

        private int DelayAmountTicks => Mathf.RoundToInt(this.delayAmount / LogicCircuitManager.ClockTickInterval);

        public string SliderTitleKey => "AutomationPlus.DELAYGATE.DELAYGATE_SIDESCREEN.TITLE";

        public string SliderUnits => (string)UI.UNITSUFFIXES.SECOND;

        public int SliderDecimalPlaces(int index) => 1;

        public float GetSliderMin(int index) => 0.1f;

        public float GetSliderMax(int index) => 10f;

        public float GetSliderValue(int index) => this.DelayAmount;

        public void SetSliderValue(float value, int index) => this.DelayAmount = value;

        public string GetSliderTooltipKey(int index) => "AutomationPlus.DELAYGATE.DELAYGATE_SIDESCREEN.TOOLTIP";

        string ISliderControl.GetSliderTooltip() => string.Format((string)Strings.Get(GetSliderTooltipKey(0)), (object)this.DelayAmount);

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.Subscribe<DelayGate>(-905833192, (IntraObjectHandler<DelayGate>)((comp, data) => comp.OnCopySettings(data)));
        }

        private void OnCopySettings(object data)
        {
            DelayGate component = ((GameObject)data).GetComponent<DelayGate>();
            if (!((UnityEngine.Object)component != (UnityEngine.Object)null))
                return;
            this.DelayAmount = component.DelayAmount;
        }


        protected override void OnSpawn()
        {
            base.OnSpawn();
            this.Subscribe<DelayGate>(-801688580, DelayGate.OnLogicValueChangedDelegate);
            this.ports = this.GetComponent<LogicPorts>();
            this.kbac = this.GetComponent<KBatchedAnimController>();
            this.kbac.Play((HashedString)"off");
            Connect();
        }
        private void Connect() { 
            if (this.connected)
            {
                return;
            }
            this.connected = true;
            int outputCellOne = this.ports.GetPortCell(DelayGate.OUTPUT_PORT_ID);
            var logicCircuitSystem = Game.Instance.logicCircuitSystem;
            logicCircuitSystem.AddToNetworks(outputCellOne, this, true);
            var logicCircuitManager = Game.Instance.logicCircuitManager;
            this.outputOne = new LogicPortVisualizer(outputCellOne, LogicPortSpriteType.Output);
            logicCircuitManager.AddVisElem((ILogicUIElement)this.outputOne);
            LogicCircuitManager.ToggleNoWireConnected(false, this.gameObject);
        }

        private void Disconnect()
        {
            if (!this.connected)
                return;
            var logicCircuitSystem = Game.Instance.logicCircuitSystem;
            var logicCircuitManager = Game.Instance.logicCircuitManager;
            this.connected = false;
            int outputCellOne = this.ports.GetPortCell(DelayGate.OUTPUT_PORT_ID);
            logicCircuitSystem.RemoveFromNetworks(outputCellOne, (object)this, true);
            logicCircuitManager.RemoveVisElem((ILogicUIElement)this.outputOne);
            this.outputOne = (LogicPortVisualizer)null;
        }

        public int GetInputValue()
        {
            LogicPorts component = this.GetComponent<LogicPorts>();
            return component?.GetInputValue(DelayGate.INPUT_PORT_ID) ?? 0;
        }

        public int GetOutputValue()
        {
            LogicPorts component = this.GetComponent<LogicPorts>();
            return component?.GetOutputValue(DelayGate.OUTPUT_PORT_ID) ?? 0;
        }

        private LogicCircuitNetwork GetInputNetwork()
        {
            LogicCircuitNetwork logicCircuitNetwork = (LogicCircuitNetwork)null;
            if ((UnityEngine.Object)this.ports != (UnityEngine.Object)null)
                logicCircuitNetwork = Game.Instance.logicCircuitManager.GetNetworkForCell(this.ports.GetPortCell(DelayGate.INPUT_PORT_ID));
            return logicCircuitNetwork;
        }

        private LogicCircuitNetwork GetOutputNetwork()
        {
            LogicCircuitNetwork logicCircuitNetwork = null;
            if (this.ports != null)
                logicCircuitNetwork = Game.Instance.logicCircuitManager.GetNetworkForCell(this.ports.GetPortCell(DelayGate.OUTPUT_PORT_ID));
            return logicCircuitNetwork;
        }

        public void OnLogicValueChanged(object data)
        {
            LogicValueChanged logicValueChanged = (LogicValueChanged)data;
            if (logicValueChanged.portID == DelayGate.INPUT_PORT_ID)
            {
                var last = this.info.journal.LastOrDefault();
                if (last == null || last.value != logicValueChanged.newValue)
                {
                    var info = DelayGateInfoPool.getOrCreate();
                    info.value = logicValueChanged.newValue;
                    this.info.Add(info);
                }
            } 

            if(GetOutputNetwork() != null && GetInputNetwork() != null)
            {
                LogicCircuitManager.ToggleNoWireConnected(false, this.gameObject);
            }
            
            
        }

        public void RenderEveryTick(float delta) {
            //this.elapsedTime += delta;
            //while ((double)this.elapsedTime > (double)LogicCircuitManager.ClockTickInterval)
        }

        public void LogicTick()
        {
            var didChange = this.info.AddTick(this.DelayAmountTicks);
            var c1 = GetInputNetwork() != null;
            var c2 = GetOutputNetwork() != null;
            if (!c1 || !c2)
            {
                this.info.clear();
                kbac.Play((HashedString)"off");
            }
            else if (didChange)
            {
                this.CurrentValue = this.GetCustomValue(GetInputValue());
                this.GetComponent<LogicPorts>().SendSignal(DelayGate.OUTPUT_PORT_ID, CurrentValue);
            }
        }

        private void RefreshColors(int inputVal, int outputValue)
        {
            int num = 0;
            if (inputVal > 0)
            {
                num = 1;
            }
            if (outputValue > 0)
            {
                num = num + 4;
            }
            if (num > 0)
            {
                kbac.Play((HashedString)("on_" + num.ToString()));
            }
            else
            {
                kbac.Play((HashedString)"off");
            }
        }

        protected int GetCustomValue(int val1)
        {
            var last = this.info.journal.LastOrDefault();
            if (last == null || last.value != val1)
            {
                var info = DelayGateInfoPool.getOrCreate();
                info.value = val1;
                this.info.Add(info);
            }
            var outValue = 0;
            if (this.info.sumTicks == this.DelayAmountTicks)
            {
                outValue = this.info.journal.FirstOrDefault()?.value ?? 0;
            }
            this.RefreshColors(val1, outValue);
            return outValue;
        }

        public int GetLogicCell()
        {
            return this.ports.GetPortCell(DelayGate.OUTPUT_PORT_ID);
        }

        public int GetLogicValue()
        {
            return this.CurrentValue;
        }

        public void OnLogicNetworkConnectionChanged(bool connected)
        {
        }

        protected override void OnCleanUp()
        {
            this.Disconnect();
            base.OnCleanUp();
        }
    }
}
