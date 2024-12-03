// harmony patch


using System.Collections.Generic;
using AtlyssTools.Registries;
using HarmonyLib;
using UnityEngine;

namespace AtlyssTools.Patches
{
    [HarmonyPatch(typeof(GameManager), "Cache_ScriptableAssets")]
    public class GameManagerPatches
    {
        [HarmonyPostfix]
        private static void Postfix(GameManager __instance)
        {
            // write a test message to the console
            Plugin.Logger.LogInfo("GameManager.Cache_ScriptableAssets postfix patch");
            // add our custom skills to the cache
            SkillManager.LoadAllFromAssets();
            ConditionManager.LoadAllFromAssets();
            // do a dump of the skills
        }
    }

    [HarmonyPatch(typeof(PlayerCasting), "Init_SkillLibrary")]
    public class CastingPatches
    {
        [HarmonyPrefix]
        private static void Prefix(PlayerCasting __instance)
        {
            Plugin.Logger.LogInfo("PlayerCasting.Init_SkillLibrary postfix patch");


            // add a new skill to the general skills list
            /*
            ScriptableSkill jsonSkill = SkillManager.GetFromCache("Hug");
            if(jsonSkill == null)
            {
                Plugin.Logger.LogError("Failed to load skill from cache");
            }
            else
            {
                SkillManager.RegisterGeneralSkill(jsonSkill);
            }
            */
        }
    }
}