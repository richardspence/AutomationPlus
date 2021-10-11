using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutomationPlus
{
    static class DelayGateInfoPool
    {
        private static List<DelayGateInfo> _pool = new List<DelayGateInfo>();
        public static  DelayGateInfo getOrCreate()
        {
            var result = _pool.LastOrDefault();
            if (result != null)
            {
                _pool.RemoveAt(_pool.Count -1);
                return result;
            }
            return new DelayGateInfo();
        }

        public static  void release(DelayGateInfo obj)
        {
            // 10 is our max pool size
            if(DelayGateInfoPool._pool.Count < 10)
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

        public bool AddTick(int maxTicks)
        {
            var didChange = false;
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
                    if(first == null)
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
                        this.journal.Remove(first);
                        DelayGateInfoPool.release(first);
                        sumTicks -= first.ticks;
                        didChange = true;
                    }
                }
            }

            return didChange;
        }
           
        public List<DelayGateInfo> journal = new List<DelayGateInfo>();
    }

    [SerializationConfig(MemberSerialization.OptIn)]
    public class DelayGate : LogicGate, ISingleSliderControl, ISliderControl
    {

        [Serialize]
        private float delayAmount = .1f;
        [Serialize]
        private DelayGateInfoObj info = new DelayGateInfoObj();
        [Serialize]

        private int delayTicksRemaining;
        private MeterController meter;
        [MyCmpAdd]
        private CopyBuildingSettings copyBuildingSettings;
        private static readonly EventSystem.IntraObjectHandler<DelayGate> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<DelayGate>((System.Action<DelayGate, object>)((component, data) => component.OnCopySettings(data)));

        public float DelayAmount
        {
            get => this.delayAmount;
            set
            {
                this.delayAmount = value;
                int delayAmountTicks = this.DelayAmountTicks;
                if (this.delayTicksRemaining <= delayAmountTicks)
                    return;
                this.delayTicksRemaining = delayAmountTicks;
            }
        }

        private int DelayAmountTicks => Mathf.RoundToInt(this.delayAmount / LogicCircuitManager.ClockTickInterval);

        public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.LOGIC_BUFFER_SIDE_SCREEN.TITLE";

        public string SliderUnits => (string)UI.UNITSUFFIXES.SECOND;

        public int SliderDecimalPlaces(int index) => 1;

        public float GetSliderMin(int index) => 0.1f;

        public float GetSliderMax(int index) => 10f;

        public float GetSliderValue(int index) => this.DelayAmount;

        public void SetSliderValue(float value, int index) => this.DelayAmount = value;

        public string GetSliderTooltipKey(int index) => "STRINGS.UI.UISIDESCREENS.LOGIC_BUFFER_SIDE_SCREEN.TOOLTIP";

        string ISliderControl.GetSliderTooltip() => string.Format((string)"Will delay the signal till {0} seconds after receiving the initial signal.", (object)this.DelayAmount);

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.Subscribe<DelayGate>(-905833192, DelayGate.OnCopySettingsDelegate);
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
            this.meter = new MeterController((KAnimControllerBase)this.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.UserSpecified, Grid.SceneLayer.LogicGatesFront, Vector3.zero, (string[])null);
            this.meter.SetPositionPercent(1f);
        }

        private void Update() => this.meter.SetPositionPercent((this.delayTicksRemaining <= 0 ? 1f : (float)(this.DelayAmountTicks - this.delayTicksRemaining) / (float)this.DelayAmountTicks));


        public override void LogicTick()
        {
            var didChange = this.info.AddTick(this.DelayAmountTicks);
            var c1 = (Game.Instance.logicCircuitSystem.GetNetworkForCell(this.OutputCellOne) is LogicCircuitNetwork);
            var c2 = (Game.Instance.logicCircuitSystem.GetNetworkForCell(this.InputCellOne) is LogicCircuitNetwork);
            if (this.delayTicksRemaining > 0)

            {

               
                if (c1 && c2)
                {
                    --this.delayTicksRemaining;
                    didChange = true;
                }
                else
                {
                    this.delayTicksRemaining = this.DelayAmountTicks;
                }
                
            }
            if (this.delayTicksRemaining > 0)
            {
                if (didChange)
                {
                    this.OnDelay();
                }
                return;
            }
        }

        protected override int GetCustomValue(int val1, int val2)
        {
            var last = this.info.journal.LastOrDefault();
            if(last == null || last.value != val1)
            {
                var info = DelayGateInfoPool.getOrCreate();
                info.value = val1;
                this.info.Add(info);
            }
            if (this.delayTicksRemaining > 0)
            {
                return 0;
            }
            else
            {
                return this.info.journal.FirstOrDefault()?.value ?? 0;
            }

        }

        private void OnDelay()
        {
             var c1 = (Game.Instance.logicCircuitSystem.GetNetworkForCell(this.OutputCellOne) is LogicCircuitNetwork);
            if (this.cleaningUp)
                return;
            this.delayTicksRemaining = 0;
            this.meter.SetPositionPercent(1f);
            if (this.outputValueOne == 0 || !(Game.Instance.logicCircuitSystem.GetNetworkForCell(this.OutputCellOne) is LogicCircuitNetwork))
                return;
            this.RefreshAnimation();
        }
    }
}
