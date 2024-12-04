using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AtlyssTools.Utility;
using BepInEx;
using Newtonsoft.Json;
using UnityEngine;

namespace AtlyssTools.Registries;

public class ConditionManager : ScriptablesManager<ScriptableStatusCondition>
{
    protected override void RegisterInternal(ScriptableObject obj)
    {
        ScriptableStatusCondition condition = obj as ScriptableStatusCondition;
        if(condition == null)
        {
            Plugin.Logger.LogError("Attempted to register {obj.name} as a status condition, but it is not a status condition");
            return;
        }
        
        if(GameManager._current._cachedScriptableConditions.ContainsKey(condition._conditionName))
        {
            Plugin.Logger.LogError($"Status condition {condition._conditionName} already registered");
            return;
        }
        
        GameManager._current._cachedScriptableConditions.Add(condition._conditionName, condition);
    }

    protected override ScriptableObject GetFromCacheInternal(string objName)
    {
        return GameManager._current.LocateCondition(objName);
    }

    protected override IList InternalGetCached()
    {
        return GameManager._current._cachedScriptableConditions.Values.ToList();
    }
    
    public override JsonSerializerSettings GetJsonSettings()
    {
        List<JsonConverter> converters = new()
        {    
            new AssetConverter<Sprite>(),
            new AssetConverter<CastEffectCollection>(),
            new AssetConverter<ScriptableWeaponType>(),
            new AssetConverter<ScriptableItem>(),
            new AssetConverter<ScriptableSkill>(),
            new AssetConverter<AudioClip>(),
            new AssetConverter<ScriptableCombatElement>(),
            new Vector3Converter(),
            new GameObjectConverter(),
            new ColorConverter(),
            new ScriptableStatusConditionBaseConverter(),
        };
        
        return new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            TypeNameHandling = TypeNameHandling.All,
            Converters = converters
        };
    }

    public static ConditionManager Instance => _instance ??= new();
    private static ConditionManager _instance;
}