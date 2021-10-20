using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            var didChange = sumTicks+1 == maxTicks;
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
    public class DelayGate_OLD : LogicGate, ISingleSliderControl, ISliderControl
    {
        private Color activeTintColor = new Color(0.5411765f, 0.9882353f, 0.2980392f);
        private Color inactiveTintColor = Color.red;
        [Serialize]
        private float delayAmount = .1f;
        [Serialize]
        private DelayGateInfoObj info = new DelayGateInfoObj();
        private static KAnimHashedString InputSymbol = (KAnimHashedString)"light_bloom_0";
        private static KAnimHashedString OutputSymbol = (KAnimHashedString)"light_bloom_1";


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
            }
        }

        [Serialize]
        private int CurrentValue;

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
            this.meter?.SetPositionPercent(1f);
        }

        private void Update() {
            this.meter.SetPositionPercent(0f);
        }


        public override void LogicTick()
        {
            var didChange = this.info.AddTick(this.DelayAmountTicks);
            var c1 = (Game.Instance.logicCircuitSystem.GetNetworkForCell(this.OutputCellOne) is LogicCircuitNetwork);
            var c2 = (Game.Instance.logicCircuitSystem.GetNetworkForCell(this.InputCellOne) is LogicCircuitNetwork);
            if(!c1 || !c2)
            {
                this.info.clear();
            }
            if (didChange)
            {
                this.OnDelay();
            }
        }

        protected override int GetCustomValue(int val1, int val2)
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
                outValue= this.info.journal.FirstOrDefault()?.value ?? 0;
            }
            this.RefreshColors(val1, outValue);
            return outValue;
        }

        private void OnDelay()
         {
            var c1 = (Game.Instance.logicCircuitSystem.GetNetworkForCell(this.OutputCellOne) is LogicCircuitNetwork);
            var c2 = (Game.Instance.logicCircuitSystem.GetNetworkForCell(this.InputCellOne) is LogicCircuitNetwork);
            if (this.cleaningUp)
                return;
            if (!c1)
                return;
            this.meter?.SetPositionPercent(1f);
            this.RefreshAnimation();
            this.RefreshColors(this.GetPortValue(LogicGateBase.PortId.InputOne), this.outputValueOne);
        }

        private void RefreshColors(int inputVal, int outputValue)
        {
            var outputNetwork = (Game.Instance.logicCircuitSystem.GetNetworkForCell(this.OutputCellOne) as LogicCircuitNetwork);
            var inputNetwork = (Game.Instance.logicCircuitSystem.GetNetworkForCell(this.InputCellOne) as LogicCircuitNetwork);
            var component = this.GetComponent<KBatchedAnimController>();
            int num = 0;
            if(inputVal > 0)
            {
                num = 1;
            }if(outputValue > 0)
            {
                num = num + 4;
            }
            if(num > 0)
            {
                component.Play((HashedString)("on_" + num.ToString()));
            }
            else
            {
                component.Play((HashedString)"off");

            }

        }
    }
}
