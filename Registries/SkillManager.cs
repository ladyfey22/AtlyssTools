using System.Collections.Generic;
using System.IO;
using System.Linq;
using AtlyssTools.Utility;
using BepInEx;
using Newtonsoft.Json;
using UnityEngine;

namespace AtlyssTools.Registries;

public class SkillManager
{
    public static ScriptableSkill GetFromCache(string skillName)
    {
        return GameManager._current.LocateSkill(skillName);
    }


    public static void Register(ScriptableSkill skill)
    {
        // double check that the skill isn't already in the cache
        if (GameManager._current._cachedScriptableSkills.ContainsKey(skill._skillName))
        {
            Plugin.Logger.LogError($"Skill {skill._skillName} is already in the cache");
            return;
        }

        GameManager._current._cachedScriptableSkills.Add(skill._skillName, skill);
    }

    public static void RegisterGeneralSkill(ScriptableSkill skill)
    {
        // double check that the skill is in the cache and not already in the general skills list
        if (!GameManager._current._cachedScriptableSkills.ContainsKey(skill._skillName))
        {
            // add it to the cache
            Register(skill);
        }

        if (GameManager._current._statLogics._generalSkills.Contains(skill))
        {
            Plugin.Logger.LogError($"Skill {skill._skillName} is already in the general skills list");
            return;
        }

        ScriptableSkill[] generalSkills = GameManager._current._statLogics._generalSkills;
        List<ScriptableSkill> newGeneralSkills = generalSkills.ToList();
        newGeneralSkills.Add(skill);
        GameManager._current._statLogics._generalSkills = newGeneralSkills.ToArray();
    }

    public static void LoadAllFromAssets()
    {
        // get skills from AtlyssToolsLoader
        var skills = AtlyssToolsLoader.GetScriptableObjects<ScriptableSkill>();
        // for each, check if it's in the cache and add it if not
        foreach (var skill in skills)
        {
            if (!GameManager._current._cachedScriptableSkills.ContainsKey(skill._skillName))
            {
                Register(skill);
            }
        }
    }
}