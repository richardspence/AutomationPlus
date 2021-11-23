﻿using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutomationPlus
{
    [SerializationConfig(MemberSerialization.OptIn)]
    class PnrgGate : KMonoBehaviour
    {
        [MyCmpAdd]
        private CopyBuildingSettings copyBuildingSettings;
        public static readonly HashedString INPUT_PORT_ID = new HashedString("PnrgGateInput");
        public static readonly HashedString OUTPUT_PORT_ID = new HashedString("PnrgGateOutput");
        private static System.Random _random = new System.Random();
        private bool hasRendered = false;

        private static readonly EventSystem.IntraObjectHandler<PnrgGate> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<PnrgGate>((component, data) => component.OnLogicValueChanged(data));
        //private static readonly EventSystem.IntraObjectHandler<PnrgGate> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<PnrgGate>(((component, data) => component.OnCopySettings(data)));
        private LogicPorts ports;
        private KBatchedAnimController kbac;

        Dictionary<int, HashedString> _animations = new Dictionary<int, HashedString>();

        [Serialize]
        private int setValue;
        [Serialize]
        private int currentValue;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            //this.Subscribe<PnrgGate>(-905833192, PnrgGate.OnCopySettingsDelegate);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            this.Subscribe<PnrgGate>(-801688580, PnrgGate.OnLogicValueChangedDelegate);
            this.ports = this.GetComponent<LogicPorts>();
            this.kbac = this.GetComponent<KBatchedAnimController>();
            this.kbac.Play((HashedString)"off");
            UpdateVisuals();
        }

        public int GetInputValue()
        {
            LogicPorts component = this.GetComponent<LogicPorts>();
            return component?.GetInputValue(PnrgGate.INPUT_PORT_ID) ?? 0;
        }

        public int GetOutputValue()
        {
            LogicPorts component = this.GetComponent<LogicPorts>();
            return component?.GetOutputValue(PnrgGate.OUTPUT_PORT_ID) ?? 0;
        }

        private LogicCircuitNetwork GetInputNetwork()
        {
            LogicCircuitNetwork logicCircuitNetwork = (LogicCircuitNetwork)null;
            if ((UnityEngine.Object)this.ports != (UnityEngine.Object)null)
                logicCircuitNetwork = Game.Instance.logicCircuitManager.GetNetworkForCell(this.ports.GetPortCell(PnrgGate.INPUT_PORT_ID));
            return logicCircuitNetwork;
        }

        private LogicCircuitNetwork GetOutputNetwork()
        {
            LogicCircuitNetwork logicCircuitNetwork = null;
            if (this.ports != null)
                logicCircuitNetwork = Game.Instance.logicCircuitManager.GetNetworkForCell(this.ports.GetPortCell(PnrgGate.OUTPUT_PORT_ID));
            return logicCircuitNetwork;
        }

        public void OnLogicValueChanged(object data)
        {
            LogicValueChanged logicValueChanged = (LogicValueChanged)data;
            var changed = false;
            if (logicValueChanged.portID == PnrgGate.INPUT_PORT_ID)
            {
                var i = GetInputValue();
                if (i != 0 && setValue == 0)
                {
                    this.currentValue = _random.Next() % 15;
                    setValue = i;
                    this.GetComponent<LogicPorts>().SendSignal(PnrgGate.OUTPUT_PORT_ID, currentValue);
                    changed = true;
                }
                else if (setValue != i)
                {
                    changed = true;
                    setValue = i;
                }
            }
            if (changed || !hasRendered)
            {
                UpdateVisuals();
            }
        }

        public void UpdateVisuals()
        {
            hasRendered = true;
            LogicCircuitNetwork inputNetwork = this.GetInputNetwork();
            LogicCircuitNetwork outputNetwork = this.GetOutputNetwork();
            if (inputNetwork != null || outputNetwork != null)
            {
                var firstBit = GetInputValue() > 0 ? 1 : 0;
                var key = firstBit << 4;
                var value = GetOutputValue();
                key |= value;
                if (!_animations.ContainsKey(key))
                {
                    var bit1 = 0x1 & value;
                    var bit2 = 0x1 & (value >> 1);
                    var bit3 = 0x1 & (value >> 2);
                    var bit4 = 0x1 & (value >> 3);
                    var valString = $"{bit1}{bit2}{bit3}{bit4}";
                    _animations.Add(key, $"on_{firstBit}_{valString}");
                }
                this.kbac.Play((HashedString)_animations[key], KAnim.PlayMode.Paused);
            }
            else
            {
                this.kbac.Play("off");
            }

        }

        public bool IsBitActive(int bit)
        {
            LogicCircuitNetwork logicCircuitNetwork = (LogicCircuitNetwork)null;
            if ((UnityEngine.Object)this.ports != (UnityEngine.Object)null)
                logicCircuitNetwork = Game.Instance.logicCircuitManager.GetNetworkForCell(this.ports.GetPortCell(PnrgGate.OUTPUT_PORT_ID));
            return logicCircuitNetwork != null && logicCircuitNetwork.IsBitActive(bit);
        }

    }
}
