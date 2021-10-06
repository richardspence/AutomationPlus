using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace AutomationPlus
{
    class DelayGateConfig : IBuildingConfig
    {
        public static string ID = "DelayGATE";
        public static string anim = "logic_buffer_kanim";
        public override BuildingDef CreateBuildingDef() => this.CreateBuildingDef(ID, anim, height: 1);
        protected BuildingDef CreateBuildingDef(
    string ID,
    string anim,
    int width = 2,
    int height = 2)
        {
            string id = ID;
            int width1 = width;
            int height1 = height;
            string anim1 = anim;
            float[] tieR0_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
            string[] refinedMetals = MATERIALS.REFINED_METALS;
            EffectorValues none = NOISE_POLLUTION.NONE;
            EffectorValues tieR0_2 = BUILDINGS.DECOR.PENALTY.TIER0;
            EffectorValues noise = none;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width1, height1, anim1, 10, 3f, tieR0_1, refinedMetals, 1600f, BuildLocationRule.Anywhere, tieR0_2, noise);
            buildingDef.ViewMode = OverlayModes.Logic.ID;
            buildingDef.ObjectLayer = ObjectLayer.LogicGate;
            buildingDef.SceneLayer = Grid.SceneLayer.LogicGates;
            buildingDef.ThermalConductivity = 0.05f;
            buildingDef.Floodable = false;
            buildingDef.Overheatable = false;
            buildingDef.Entombable = false;
            buildingDef.AudioCategory = "Metal";
            buildingDef.AudioSize = "small";
            buildingDef.BaseTimeUntilRepair = -1f;
            buildingDef.PermittedRotations = PermittedRotations.R360;
            buildingDef.DragBuild = true;
            LogicGateBase.uiSrcData = Assets.instance.logicModeUIData;
            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
            return buildingDef;
        }

        protected CellOffset[] InputPortOffsets => new CellOffset[1]
 {
    CellOffset.none
 };

        protected CellOffset[] OutputPortOffsets => new CellOffset[1]
        {
    new CellOffset(1, 0)
        };
        public override void DoPostConfigureComplete(GameObject go)
        {
            DelayGate logicGateBuffer = go.AddComponent<DelayGate>();
            logicGateBuffer.op = LogicGateBase.Op.CustomSingle;
            logicGateBuffer.inputPortOffsets = this.InputPortOffsets;
            logicGateBuffer.outputPortOffsets = this.OutputPortOffsets;
            logicGateBuffer.controlPortOffsets = null;
            go.GetComponent<KPrefabID>().prefabInitFn += (KPrefabID.PrefabFn)(game_object => game_object.GetComponent<DelayGate>().SetPortDescriptions(this.GetDescriptions()));
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
        }

        protected LogicGate.LogicGateDescriptions GetDescriptions() => new LogicGate.LogicGateDescriptions()
        {
            outputOne = new LogicGate.LogicGateDescriptions.Description()
            {
                name = DELAYGATE.OUTPUT_NAME,
                active = DELAYGATE.OUTPUT_ACTIVE,
                inactive = DELAYGATE.OUTPUT_INACTIVE
            }
        };


    }



}
