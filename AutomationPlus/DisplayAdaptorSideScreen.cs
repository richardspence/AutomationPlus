using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutomationPlus
{
    class DisplayAdaptorSideScreen : SideScreenContent
    {
        private DisplayAdaptor target;
        public DisplayAdaptorSideScreen()
        {
            titleKey = StringUtils.GetStringKey(()=>DisplayAdaptorStrings.SideScreen.TITLE);
        }
        private GameObject toggle;
        private GameObject combo;
        private GameObject invalidWarning;
        private GameObject lblInput1;
        private GameObject lblInput2;
        private GameObject lblOutput1;

        public override bool IsValidForTarget(GameObject target)
        {
            return target.GetComponent<DisplayAdaptor>() != null;
        }

        private void BuildUi()
        {
            var row = new PPanel("BitRow")
            {
                FlexSize = Vector2.right,
                Alignment = TextAnchor.MiddleCenter,
                Spacing = 10,
                Direction = PanelDirection.Horizontal,
                Margin = new RectOffset(2,2, 2, 2),
                
            }.AddChild(new PLabel("BitLabel")
            {
                TextAlignment = TextAnchor.MiddleRight,
                ToolTip = StringUtils.Get( () => DisplayAdaptorStrings.SideScreen.HexMode.Tooltip ),
                Text = StringUtils.Get(() => DisplayAdaptorStrings.SideScreen.HexMode.Label),
                TextStyle = PUITuning.Fonts.TextDarkStyle
            });;
            var cb = new PCheckBox
            {
                ToolTip = StringUtils.Get(() => DisplayAdaptorStrings.SideScreen.HexMode.Tooltip),
                TextStyle = PUITuning.Fonts.TextLightStyle,
                TextAlignment = TextAnchor.
                MiddleLeft,
                OnChecked = (s, e)=> OnToggleChanged(e),
            };
            cb.OnRealize += (t)=>
            {
                this.toggle = t;
                this.UpdateVisuals();
            };
            row.AddChild(cb).AddTo(gameObject);


            var row2 = new PPanel("StatusRow").AddTo(gameObject);

            lblInput1 = new PLabel("lblInput1")
            {
                TextAlignment = TextAnchor.MiddleRight,
                ToolTip = "",
                Text = " ",
                TextStyle = PUITuning.Fonts.TextDarkStyle
            }.AddTo(row2);
        }

        protected override void OnPrefabInit()
        {
            var baseLayout = gameObject.GetComponent<BoxLayoutGroup>();
            var margin = new RectOffset(4, 4, 4, 4);
            if (baseLayout != null)
                baseLayout.Params = new BoxLayoutParams()
                {
                    Margin = margin,
                    Direction = PanelDirection.Vertical,
                    Alignment =
                    TextAnchor.UpperCenter,
                    Spacing = 8
                };
            BuildUi();
            ContentContainer = gameObject;
            base.OnPrefabInit();
            UpdateVisuals();
        }

        private void OnToggleChanged(int newValue)
        {
            Debug.Log($"SS:New Value: {newValue}");
            this.target.IsHexMode = newValue != PCheckBox.STATE_CHECKED;
            this.UpdateVisuals();
        }

        public override void SetTarget(GameObject target)
        {

            Debug.Log($"SS:SetTarget");
            base.SetTarget(target);
            this.target = target.GetComponent<DisplayAdaptor>();
            this.UpdateVisuals();
        }


        private void UpdateVisuals()
        {
            Debug.Break();
            Debug.Log($"SS:UpdateVisuals");
            if (this.toggle != null && this.target != null)
            {
                PCheckBox.SetCheckState(this.toggle, target.IsHexMode ? PCheckBox.STATE_CHECKED: PCheckBox.STATE_UNCHECKED);
                if(lblInput1)
                    PUIElements.SetText(lblInput1, $"{target.GetInputDisplayValue()}");
            }
        }
    }
}
