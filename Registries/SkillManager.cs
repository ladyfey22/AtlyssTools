using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AtlyssTools.Utility;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace AtlyssTools.Registries;

[ManagerAttribute]
public class SkillManager : ScriptablesManager<ScriptableSkill>
{
    private static SkillManager _instance;
    public readonly List<string> GeneralSkills = new();

    protected SkillManager()
    {
    }

    internal static SkillManager Instance => _instance ??= new();

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

    public override string GetJsonName(JObject obj)
    {
        return obj["_skillName"]?.Value<string>();
    }

    public override void PreLibraryInit()
    {
        base.PreLibraryInit();
        foreach (var skill in Instance.GeneralSkills)
        {
            if (!GameManager._current._cachedScriptableSkills.TryGetValue(skill, out var cachedSkill))
            {
                Plugin.Logger.LogError($"General skill {skill} not found in cache");
                continue;
            }

            var generalSkills = GameManager._current._statLogics._generalSkills.ToList();
            generalSkills.Add(cachedSkill);
            GameManager._current._statLogics._generalSkills = generalSkills.ToArray();
        }
    }
}