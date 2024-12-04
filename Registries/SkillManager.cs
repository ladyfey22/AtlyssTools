using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AtlyssTools.Utility;
using BepInEx;
using Newtonsoft.Json;
using UnityEngine;

namespace AtlyssTools.Registries;

public class SkillManager : ScriptablesManager<ScriptableSkill>
{
   public readonly List<string> GeneralSkills = new();
    
    public void RegisterGeneralSkill(string skillName)
    {
        if(GeneralSkills.Contains(skillName))
        {
            Plugin.Logger.LogWarning($"General skill {skillName} already registered");
            return;
        }
        
        if(AtlyssToolsLoader.Instance.State > LoaderStateMachine.LoadState.PreLibraryInit)
        {
            Plugin.Logger.LogWarning($"General skill {skillName} registered after skill library load");
            return;
        }
        
        GeneralSkills.Add(skillName);
    }
    
    
    protected override void RegisterInternal(ScriptableObject obj)
    {
        GameManager._current._cachedScriptableSkills.Add(obj.name, obj as ScriptableSkill);
    }
    
    protected override ScriptableObject GetFromCacheInternal(string objName)
    {
        return GameManager._current.LocateSkill(objName);
    }

    protected override IList InternalGetCached()
    {
        return GameManager._current._cachedScriptableSkills.Values.ToList();
    }

    public override void PreLibraryInit()
    {
        base.PreLibraryInit();
        foreach (string skill in Instance.GeneralSkills)
        {
            if(!GameManager._current._cachedScriptableSkills.TryGetValue(skill, out var cachedSkill))
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