using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace AutomationPlus
{
    class PnrgGateConfig : IBuildingConfig
    {
        public static string ID = "PnrgGate";

        public override BuildingDef CreateBuildingDef()
        {
            string id = PnrgGateConfig.ID;
            float[] tieR0_1 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
            string[] refinedMetals = MATERIALS.REFINED_METALS;
            EffectorValues none = TUNING.NOISE_POLLUTION.NONE;
            EffectorValues tieR0_2 = TUNING.BUILDINGS.DECOR.PENALTY.TIER0;
            EffectorValues noise = none;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, 2, 1, "logic_generator_random_kanim", 30, 30f, tieR0_1, refinedMetals, 1600f, BuildLocationRule.Anywhere, tieR0_2, noise);
            buildingDef.Overheatable = false;
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.PermittedRotations = PermittedRotations.R360;
            buildingDef.ViewMode = OverlayModes.Logic.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.ObjectLayer = ObjectLayer.LogicGate;
            buildingDef.SceneLayer = Grid.SceneLayer.LogicGatesFront;
            buildingDef.AlwaysOperational = true;
            buildingDef.LogicInputPorts = new List<LogicPorts.Port>()
    {
      new LogicPorts.Port(PnrgGate.INPUT_PORT_ID, new CellOffset(0, 0), (string) PnrgGateStrings.LOGIC_PORT, (string) PnrgGateStrings.INPUT_PORT_ACTIVE, (string) PnrgGateStrings.INPUT_PORT_INACTIVE,  true, LogicPortSpriteType.ResetUpdate , true),
    };
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
    {
      LogicPorts.Port.RibbonOutputPort(PnrgGate.OUTPUT_PORT_ID, new CellOffset(1, 0), (string) PnrgGateStrings.LOGIC_PORT_OUTPUT, (string) PnrgGateStrings.OUTPUT_PORT_ACTIVE, (string) PnrgGateStrings.OUTPUT_PORT_INACTIVE, true)
    };
            //SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Open_DoorInternal", TUNING.NOISE_POLLUTION.NOISY.TIER3);
            //SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Close_DoorInternal", TUNING.NOISE_POLLUTION.NOISY.TIER3);
            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, PnrgGateConfig.ID);
            return buildingDef;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<PnrgGate>();
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
        }
    }
}
