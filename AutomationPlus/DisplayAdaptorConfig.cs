using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace AutomationPlus
{
    public class DisplayAdaptorConfig : IBuildingConfig
    {
        public static string ID = "DisplayAdaptor";

        public override BuildingDef CreateBuildingDef()
        {
            string id = ID;
            float[] tieR0_1 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
            string[] refinedMetals = MATERIALS.REFINED_METALS;
            EffectorValues none = TUNING.NOISE_POLLUTION.NONE;
            EffectorValues tieR0_2 = TUNING.BUILDINGS.DECOR.PENALTY.TIER0;
            EffectorValues noise = none;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, 2, 5, "display_adaptor_kanim", 30, 30f, tieR0_1, refinedMetals, 1600f, BuildLocationRule.Anywhere, tieR0_2, noise);
            buildingDef.Overheatable = false;
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.PermittedRotations = PermittedRotations.R360;
            buildingDef.ViewMode = OverlayModes.Logic.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.ObjectLayer = ObjectLayer.LogicGate;
            buildingDef.SceneLayer = Grid.SceneLayer.LogicGatesFront;
            buildingDef.AlwaysOperational = true;
            buildingDef.LogicInputPorts = new List<LogicPorts.Port>()   {
                LogicPorts.Port.RibbonInputPort(DisplayAdaptor.INPUT_PORT_ID, new CellOffset(0, 2), (string) DisplayAdaptorStrings.LOGIC_PORT, (string) DisplayAdaptorStrings.INPUT_PORT_ACTIVE, (string) DisplayAdaptorStrings.INPUT_PORT_INACTIVE , true),
            };
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()    {
                LogicPorts.Port.RibbonOutputPort(DisplayAdaptor.OUTPUT_PORT_ID1, new CellOffset(1, 4), (string) DisplayAdaptorStrings.LOGIC_PORT_OUTPUT, (string) DisplayAdaptorStrings.OUTPUT_PORT_ACTIVE, (string) DisplayAdaptorStrings.OUTPUT_PORT_INACTIVE, true),
                LogicPorts.Port.RibbonOutputPort(DisplayAdaptor.OUTPUT_PORT_ID2, new CellOffset(1, 3), (string) DisplayAdaptorStrings.LOGIC_PORT_OUTPUT, (string) DisplayAdaptorStrings.OUTPUT_PORT_ACTIVE, (string) DisplayAdaptorStrings.OUTPUT_PORT_INACTIVE, true),
                LogicPorts.Port.RibbonOutputPort(DisplayAdaptor.OUTPUT_PORT_ID3, new CellOffset(1, 2), (string) DisplayAdaptorStrings.LOGIC_PORT_OUTPUT, (string) DisplayAdaptorStrings.OUTPUT_PORT_ACTIVE, (string) DisplayAdaptorStrings.OUTPUT_PORT_INACTIVE, true),
                LogicPorts.Port.RibbonOutputPort(DisplayAdaptor.OUTPUT_PORT_ID4, new CellOffset(1, 1), (string) DisplayAdaptorStrings.LOGIC_PORT_OUTPUT, (string) DisplayAdaptorStrings.OUTPUT_PORT_ACTIVE, (string) DisplayAdaptorStrings.OUTPUT_PORT_INACTIVE, true),
                LogicPorts.Port.RibbonOutputPort(DisplayAdaptor.OUTPUT_PORT_ID5, new CellOffset(1, 0), (string) DisplayAdaptorStrings.LOGIC_PORT_OUTPUT, (string) DisplayAdaptorStrings.OUTPUT_PORT_ACTIVE, (string) DisplayAdaptorStrings.OUTPUT_PORT_INACTIVE, true),
            };
            SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Open_DoorInternal", TUNING.NOISE_POLLUTION.NOISY.TIER3);
            SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Close_DoorInternal", TUNING.NOISE_POLLUTION.NOISY.TIER3);
            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
            return buildingDef;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<DisplayAdaptor>();
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
        }
    }
}
