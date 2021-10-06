using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutomationPlus
{
    [SerializationConfig(MemberSerialization.OptIn)]
    class PnrgGate : KMonoBehaviour, IRender200ms
    {
        [MyCmpAdd]
        private CopyBuildingSettings copyBuildingSettings;
        public static readonly HashedString INPUT_PORT_ID = new HashedString("PnrgGateInput");
        public static readonly HashedString OUTPUT_PORT_ID = new HashedString("PnrgGateOutput");
        private System.Random _random = new System.Random();

        private static readonly EventSystem.IntraObjectHandler<PnrgGate> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<PnrgGate>((component, data) => component.OnLogicValueChanged(data));
        //private static readonly EventSystem.IntraObjectHandler<PnrgGate> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<PnrgGate>(((component, data) => component.OnCopySettings(data)));
        private LogicPorts ports;
        private KBatchedAnimController kbac;

        private static KAnimHashedString BIT_ONE_SYMBOL = (KAnimHashedString)"bit1_bloom";
        private static KAnimHashedString BIT_TWO_SYMBOL = (KAnimHashedString)"bit2_bloom";
        private static KAnimHashedString BIT_THREE_SYMBOL = (KAnimHashedString)"bit3_bloom";
        private static KAnimHashedString BIT_FOUR_SYMBOL = (KAnimHashedString)"bit4_bloom";
        private static KAnimHashedString INPUT_SYMBOL = (KAnimHashedString)"input_light_bloom";
        private Color colorOn = new Color(0.3411765f, 0.7254902f, 0.3686275f);
        private Color colorOff = new Color(0.9529412f, 0.2901961f, 0.2784314f);

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
            this.kbac.Play((HashedString)"idle");
        }

        public void Render200ms(float dt)
        {

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
            if (logicValueChanged.portID != PnrgGate.INPUT_PORT_ID)
                return;
            if (logicValueChanged.newValue != 0)
            {
                this.currentValue = _random.Next() % 15;
            }
            this.GetComponent<LogicPorts>().SendSignal(PnrgGate.OUTPUT_PORT_ID, currentValue);
        }

        public void UpdateVisuals()
        {
            LogicCircuitNetwork inputNetwork = this.GetInputNetwork();
            LogicCircuitNetwork outputNetwork = this.GetOutputNetwork();
            int num = 0;
            if (inputNetwork != null)
            {
                ++num;
                this.kbac.SetSymbolTint(PnrgGate.INPUT_SYMBOL, LogicCircuitNetwork.IsBitActive(0, this.GetInputValue()) ? this.colorOn : this.colorOff);
            }
            if (outputNetwork != null)
            {
                num += 4;
                this.kbac.SetSymbolTint(PnrgGate.BIT_ONE_SYMBOL, this.IsBitActive(0) ? this.colorOn : this.colorOff);
                this.kbac.SetSymbolTint(PnrgGate.BIT_TWO_SYMBOL, this.IsBitActive(1) ? this.colorOn : this.colorOff);
                this.kbac.SetSymbolTint(PnrgGate.BIT_THREE_SYMBOL, this.IsBitActive(2) ? this.colorOn : this.colorOff);
                this.kbac.SetSymbolTint(PnrgGate.BIT_FOUR_SYMBOL, this.IsBitActive(3) ? this.colorOn : this.colorOff);
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
