using HarmonyLib;
using static STRINGS.UI.BUILDCATEGORIES;

namespace AutomationPlus
{
    public class Patches : KMod.UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            
        }


        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Initialize_Patch
        {
            public static void Prefix()
            {
                Debug.Log("I execute before Db.Initialize!");
            }

            public static void Postfix()
            {
                Debug.Log("I execute after Db.Initialize!");
            }

            
        }

        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
            {
                StringUtils.AddBuildingStrings(DelayGateConfig.ID, DELAYGATE.NAME, DELAYGATE.DESC, DELAYGATE.EFFECT);
                StringUtils.AddBuildingStrings(PnrgGateConfig.ID, PnrgGateStrings.NAME, PnrgGateStrings.DESC, PnrgGateStrings.EFFECT);
                StringUtils.AddBuildingStrings(DisplayAdaptorConfig.ID, DisplayAdaptorStrings.NAME, DisplayAdaptorStrings.DESC, DisplayAdaptorStrings.EFFECT);
                StringUtils.AddBuildingStrings(AluGateConfig.ID, AluGateStrings.NAME, AluGateStrings.DESC, AluGateStrings.EFFECT);

                BuildingUtils.AddBuildingToPlanScreen(PlanMenuCategory.Automation, DelayGateConfig.ID);
                BuildingUtils.AddBuildingToPlanScreen(PlanMenuCategory.Automation, PnrgGateConfig.ID);
                BuildingUtils.AddBuildingToPlanScreen(PlanMenuCategory.Automation, DisplayAdaptorConfig.ID);
                BuildingUtils.AddBuildingToPlanScreen(PlanMenuCategory.Automation, AluGateConfig.ID);
            }

            [HarmonyPatch(typeof(Db))]
            [HarmonyPatch("Initialize")]
            public static class Db_Initialize_Patch
            {
                public static void Postfix()
                {
                    BuildingUtils.AddBuildingToTechnology("LogicCircuits", DelayGateConfig.ID);
                    BuildingUtils.AddBuildingToTechnology("LogicCircuits", PnrgGateConfig.ID);
                    BuildingUtils.AddBuildingToTechnology("LogicCircuits", DisplayAdaptorConfig.ID);
                    BuildingUtils.AddBuildingToTechnology("LogicCircuits", AluGateConfig.ID);
                }
            }
        }
    }

}
