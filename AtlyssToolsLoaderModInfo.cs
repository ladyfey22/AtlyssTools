using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using JsonUtility = AtlyssTools.Utility.JsonUtility;
using Object = UnityEngine.Object;

namespace AtlyssTools;

public class AtlyssToolsLoaderModInfo
{
    private static readonly System.Type[] NonConcreteType =
    [
        typeof(ScriptableCondition),
        typeof(ScriptableItem)
    ];

    private static readonly List<System.Type> ConcreteType =
    [
        typeof(ScriptableStatusCondition),
        typeof(ScriptableSceneTransferCondition),
        typeof(ScriptablePolymorphCondition),

        typeof(ScriptableChestpiece),
        typeof(ScriptableArmorDye),
        typeof(ScriptableCape),
        typeof(ScriptableClassTome),
        typeof(ScriptableHelm),
        typeof(ScriptableLeggings),
        typeof(ScriptableRing),
        typeof(ScriptableShield),
        typeof(ScriptableSkillScroll),
        typeof(ScriptableStatusConsumable),
        typeof(ScriptableTradeItem),
        typeof(ScriptableWeapon)
    ];

    private readonly Dictionary<System.Type, IList> _scriptableObjects = new();
    public readonly List<AssetBundle> Bundles = new();
    public readonly List<Action> OnPostCacheInit = new();
    public readonly List<Action> OnPostLibraryInit = new();
    public readonly List<Action> OnPreCacheInit = new();


    // delegate lists
    public readonly List<Action> OnPreLibraryInit = new();
    public readonly Dictionary<string, Object> PathToAsset = new();
    public string ModId;

    public string ModPath;

    public void Dispose()
    {
        foreach (var bundle in Bundles) 
            bundle.Unload(true);
    }

    public void Initialize()
    {
    }

    public void LoadScriptableType<T>() where T : ScriptableObject
    {
        // check if the type is in the list, if not, add it
        if (!_scriptableObjects.ContainsKey(typeof(T))) _scriptableObjects.Add(typeof(T), new List<T>());

        LoadJsonObjects<T>();
        LoadAssetObjects<T>();
    }

    public void LoadAssetBundles()
    {
        // check that the assets folder exists
        if (!Directory.Exists(ModPath + "/Assets")) return;

        var files = Directory.GetFiles(ModPath + "/Assets", "*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            if (file.EndsWith(".manifest")) continue;

            var manifest = file + ".manifest";
            if (!File.Exists(manifest)) // not an asset bundle
                continue;

            var bundle = AssetBundle.LoadFromFile(file);
            if (bundle != null) Bundles.Add(bundle);
        }
    }


    public List<T> GetModScriptableObjects<T>() where T : ScriptableObject
    {
        return _scriptableObjects.ContainsKey(typeof(T)) ? _scriptableObjects[typeof(T)] as List<T> : [];
    }

    public IList GetModScriptableObjects(System.Type type)
    {
        return _scriptableObjects.TryGetValue(type, out var o) ? o : new List<Object>();
    }

    public void LoadAssetObjects<T>() where T : ScriptableObject
    {
        foreach (var bundle in Bundles)
        {
            var objects = bundle.LoadAllAssets<T>();
            foreach (var obj in objects)
                ((List<T>)_scriptableObjects[typeof(T)])
                    .Add(obj); // we don't need to mark the paths, since we're loading from the asset bundles. unity will handle it.
        }
    }

    private void LoadJsonObjects<T>() where T : ScriptableObject
    {
        // we want to load it from Assets/TypeName folder
        var path = ModPath + "/Assets/" + typeof(T).Name;
        if (!Directory.Exists(path))
            return;

        foreach (var file in Directory.GetFiles(path, "*.json", SearchOption.AllDirectories))
        {
            // cut off the ModPath/Assets/ and .json, because the LoadJsonObject method will add it back

            var relativePath = file.Replace(ModPath + "/Assets/", "").Replace(".json", "").Replace("\\", "/");
            var value = LoadJsonObject(relativePath, typeof(T));

            if (value == null)
            {
                Plugin.Logger.LogError($"Failed to load json object {file} for type {typeof(T).Name}, mod {ModId}");
            }
        }
    }

    private Object LoadJsonObject(string path, System.Type type)
    {
        if (string.IsNullOrEmpty(path)) return null;

        // if this is a non-concrete type, we need to check the json for the type
        if (NonConcreteType.Contains(type))
        {
            // we need to deduce the type from the path
            var typeName = path.Split('/')[0].ToLower();
            type = ConcreteType.FirstOrDefault(t => t.Name.ToLower() == typeName);
            if (type == null)
            {
                return null;
            }
        }

        // check if the dictionary already has typeof(T). if not, add it
        if (!_scriptableObjects.ContainsKey(type))
        {
            // we need to create the list indirectly
            _scriptableObjects.Add(type, (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type)));
        }

        path = path.Replace(".json", "");

        if (PathToAsset.TryGetValue(path, out var value)) // if it's already loaded, return it
            return value;
        if (!File.Exists(ModPath + "/Assets/" + path + ".json")) 
            return null;

        var json = File.ReadAllText(ModPath + "/Assets/" + path + ".json");
        if (string.IsNullOrEmpty(json)) return null;

        // check that this is a ScriptableObject
        if (!type.IsSubclassOf(typeof(ScriptableObject)) && type != typeof(ScriptableObject))
        {
            Plugin.Logger.LogError($"Type {type.Name} is not a ScriptableObject");
            return null;
        }

        var obj = JsonUtility.LoadFromJson(json, type);
        if (obj == null)
        {
            Plugin.Logger.LogError($"Failed to load json asset {path}");
            return null;
        }

        // keep in mind, type may no longer be T, so we can't cast it to List<T>
        _scriptableObjects[type].Add(obj);
        PathToAsset.Add(path, obj as Object);
        // write out the type it was 
        return obj as Object;
    }

    public Object LoadModAsset(string assetName, System.Type type)
    {
        // check if it's in the asset bundles

        foreach (var bundle in Bundles)
        {
            var obj = bundle.LoadAsset(assetName, type);
            if (obj != null) return obj;
        }

        // check if T is a scriptable object
        if (type.IsSubclassOf(typeof(ScriptableObject)) || type == typeof(ScriptableObject))
        {
            var obj = LoadJsonObject(assetName, type);
            if (obj != null) return obj;
        }

        return null;
    }
}