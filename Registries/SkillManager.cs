using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AtlyssTools.Utility;
using BepInEx;
using Newtonsoft.Json;
using UnityEngine;

namespace AtlyssTools.Registries;

[ManagerAttribute]
public class SkillManager : ScriptablesManager<ScriptableSkill>
{
    public readonly List<string> GeneralSkills = new();

    private SkillManager()
    {
    }

    public void RegisterGeneralSkill(string skillName)
    {
        if (GeneralSkills.Contains(skillName))
        {
            Plugin.Logger.LogWarning($"General skill {skillName} already registered");
            return;
        }

        if (AtlyssToolsLoader.Instance.State > LoaderStateMachine.LoadState.PreLibraryInit)
        {
            Plugin.Logger.LogWarning($"General skill {skillName} registered after skill library load");
            return;
        }

        GeneralSkills.Add(skillName);
    }

    protected override IDictionary InternalGetCached()
    {
        return GameManager._current._cachedScriptableSkills;
    }

    public override string GetName(ScriptableObject obj)
    {
        return ((ScriptableSkill)obj)._skillName;
    }

    public override void PreLibraryInit()
    {
        base.PreLibraryInit();
        foreach (string skill in Instance.GeneralSkills)
        {
            if (!GameManager._current._cachedScriptableSkills.TryGetValue(skill, out var cachedSkill))
            {
                Plugin.Logger.LogError($"General skill {skill} not found in cache");
                continue;
            }

            List<ScriptableSkill> generalSkills = GameManager._current._statLogics._generalSkills.ToList();
            generalSkills.Add(cachedSkill);
            GameManager._current._statLogics._generalSkills = generalSkills.ToArray();
        }
    }


    internal static SkillManager Instance => _instance ??= new();
    private static SkillManager _instance;
}