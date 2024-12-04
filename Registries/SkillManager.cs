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


    protected override void RegisterInternal(ScriptableObject obj)
    {
        ScriptableSkill skill = obj as ScriptableSkill;
        if(skill == null)
        {
            Plugin.Logger.LogError("Attempted to register {obj.name} as a skill, but it is not a skill");
            return;
        }
        
        if(GameManager._current._cachedScriptableSkills.ContainsKey(skill._skillName))
        {
            Plugin.Logger.LogError($"Skill {skill._skillName} already registered");
            return;
        }
        
        GameManager._current._cachedScriptableSkills.Add(skill._skillName, skill);
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
    
    public override JsonSerializerSettings GetJsonSettings()
    {
        List<JsonConverter> converters = new()
        {    
            new AssetConverter<Sprite>(),
            new AssetConverter<CastEffectCollection>(),
            new AssetConverter<ScriptableWeaponType>(),
            new AssetConverter<ScriptableItem>(),
            new ScriptableConditionConverter(),
            new AssetConverter<AudioClip>(),
            new AssetConverter<ScriptableCombatElement>(),
            new AssetConverter<ScriptablePlayerBaseClass>(),
            new Vector3Converter(),
            new GameObjectConverter(),
            new ColorConverter(),
            new ScriptableSkillBaseConverter(),
        };
        
        return new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            TypeNameHandling = TypeNameHandling.All,
            Converters = converters
        };
    }

    internal static SkillManager Instance => _instance ??= new();
    private static SkillManager _instance;
}