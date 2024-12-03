using System.Collections.Generic;
using System.IO;
using System.Linq;
using AtlyssTools.Utility;
using BepInEx;
using Newtonsoft.Json;
using UnityEngine;

namespace AtlyssTools.Registries;

public class ConditionManager
{
    public static ScriptableSkill GetFromCache(string skillName)
    {
        return GameManager._current.LocateSkill(skillName);
    }


    public static void Register(ScriptableStatusCondition condition)
    {
        // double check that the skill isn't already in the cache
        string conditionName = $"{condition._conditionName}_{condition._conditionRank}";
        if (GameManager._current._cachedScriptableConditions.ContainsKey(conditionName))
        {
            Plugin.Logger.LogError($"Condition {condition._conditionName} is already in the cache");
            return;
        }

        GameManager._current._cachedScriptableConditions.Add(conditionName, condition);
    }

    public static void LoadAllFromAssets()
    {
        // get skills from AtlyssToolsLoader
        List<ScriptableStatusCondition> conditions = AtlyssToolsLoader.GetScriptableObjects<ScriptableStatusCondition>();
        // for each, check if it's in the cache and add it if not
        foreach (var condition in conditions)
        {
            string conditionName = $"{condition._conditionName}_{condition._conditionRank}";
            if (!GameManager._current._cachedScriptableConditions.ContainsKey(conditionName))
            {
                Register(condition);
            }
        }
    }
}