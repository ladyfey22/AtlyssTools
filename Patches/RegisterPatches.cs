// harmony patch


using System.Collections.Generic;
using System.Linq;
using AtlyssTools.Registries;
using AtlyssTools.Utility;
using HarmonyLib;
using UnityEngine;

namespace AtlyssTools.Patches
{
    [HarmonyPatch(typeof(GameManager), "Cache_ScriptableAssets")]
    public class GameManagerPatches
    {
        [HarmonyPrefix]
        private static void Prefix(GameManager __instance)
        {
            // write a test message to the console
            Plugin.Logger.LogInfo("GameManager.Cache_ScriptableAssets prefix patch");
            // add our custom skills to the cache
            // do a dump of the skills
            AtlyssToolsLoader.Instance.State = LoaderStateMachine.LoadState.PreCacheInit; // run the pre cache init
        }

        [HarmonyPostfix]
        private static void Postfix(GameManager __instance)
        {
            // write a test message to the console
            Plugin.Logger.LogInfo("GameManager.Cache_ScriptableAssets postfix patch");
            // add our custom skills to the cache
            // do a dump of the skills
            AtlyssToolsLoader.Instance.State = LoaderStateMachine.LoadState.PostCacheInit; // run the post cache init

            // check if the hug condition is in the cache
            ScriptableCondition hugC = StatusConditionManager.Instance.GetFromCache("Hugged");

            if (hugC != null)
            {
                Plugin.Logger.LogInfo("Hug condition found in cache");
            }
            else
            {
                Plugin.Logger.LogError("Hug condition not found in cache");
            }

            // check if the hug skill is in the cache
            ScriptableSkill hugSkill = SkillManager.Instance.GetFromCache("Hug");

            if (hugSkill != null)
            {
                Plugin.Logger.LogInfo("Hug skill found in cache");
                // associated condition

                ScriptableCondition hugCondition = hugSkill._skillRanks[0]._selfConditionOutput;
                if (hugCondition != null)
                {
                    Plugin.Logger.LogInfo(hugCondition._conditionName);
                }
                else
                {
                    Plugin.Logger.LogError("Hug condition not found");
                }
            }
            else
            {
                Plugin.Logger.LogError("Hug skill not found in cache");
            }
        }
    }

    [HarmonyPatch(typeof(PlayerCasting), "Init_SkillLibrary")]
    public class CastingPatches
    {
        [HarmonyPrefix]
        private static void Prefix(PlayerCasting __instance)
        {
            AtlyssToolsLoader.Instance.State = LoaderStateMachine.LoadState.PreLibraryInit; // run the post library init
        }

        [HarmonyPostfix]
        private static void Postfix(PlayerCasting __instance)
        {
            AtlyssToolsLoader.Instance.State =
                LoaderStateMachine.LoadState.PostLibraryInit; // run the post library init
        }
    }
}