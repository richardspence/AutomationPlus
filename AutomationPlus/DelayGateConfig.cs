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
        public static string anim = "logic_delay_kanim";
        //public static string anim = "logic_buffer_kanim";
        
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
            buildingDef.SceneLayer = Grid.SceneLayer.LogicGatesFront;
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
            buildingDef.LogicInputPorts = new List<LogicPorts.Port>()
    {
      new LogicPorts.Port(DelayGate.INPUT_PORT_ID, new CellOffset(0, 0), (string) DELAYGATE.LOGIC_PORT, (string) DELAYGATE.INPUT_PORT_ACTIVE, (string) DELAYGATE.INPUT_PORT_INACTIVE,  true, LogicPortSpriteType.Input , true),
    };
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
    {
      new LogicPorts.Port(DelayGate.OUTPUT_PORT_ID, new CellOffset(1, 0), (string) DELAYGATE.LOGIC_PORT_OUTPUT, (string) DELAYGATE.OUTPUT_PORT_ACTIVE, (string) DELAYGATE.OUTPUT_PORT_INACTIVE, true, LogicPortSpriteType.Output),
    };
            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
            return buildingDef;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<DelayGate>();
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
        }

    }



}
