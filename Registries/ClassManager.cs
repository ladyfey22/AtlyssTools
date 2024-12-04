using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AtlyssTools.Utility;
using BepInEx;
using Newtonsoft.Json;
using UnityEngine;

namespace AtlyssTools.Registries;

public class ClassManager : ScriptablesManager<ScriptablePlayerBaseClass>
{
    protected override void RegisterInternal(ScriptableObject obj)
    {
        ScriptablePlayerBaseClass playerClass = obj as ScriptablePlayerBaseClass;
        if(playerClass == null)
        {
            Plugin.Logger.LogError("Attempted to register {obj.name} as a player class, but it is not a player class");
            return;
        }
        
        if(GameManager._current._cachedScriptablePlayerClasses.ContainsKey(playerClass._className))
        {
            Plugin.Logger.LogError($"Player class {playerClass._className} already registered");
            return;
        }
        
        GameManager._current._cachedScriptablePlayerClasses.Add(playerClass._className, playerClass);
    }

    protected override ScriptableObject GetFromCacheInternal(string objName)
    {
        return GameManager._current.LocateClass(objName);
    }

    protected override IList InternalGetCached()
    {
        return GameManager._current._cachedScriptablePlayerClasses.Values.ToList();
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
            new AssetConverter<ScriptableSkill>(),
            new AssetConverter<AudioClip>(),
            new AssetConverter<ScriptableCombatElement>(),
            new Vector3Converter(),
            new GameObjectConverter(),
            new ColorConverter(),
            new ScriptableClassBaseConverter(),
        };
        
        return new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            TypeNameHandling = TypeNameHandling.All,
            Converters = converters
        };
    }


    public static ClassManager Instance => _instance ??= new();

    private static ClassManager _instance;
}