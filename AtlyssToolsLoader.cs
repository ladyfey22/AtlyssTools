using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AtlyssTools;

public class AtlyssToolsLoader
{
    public class AtlyssToolsLoaderModInfo : IDisposable
    {
        public readonly List<AssetBundle> Bundles = new();
        public readonly List<ScriptableSkill> Skills = new();
        public readonly List<ScriptableStatusCondition> Conditions = new();
        public readonly Dictionary<string, Object> PathToAsset = new();


        private readonly Dictionary<System.Type, IList> _scriptableObjects;


        public string ModPath;
        public string ModId;

        public AtlyssToolsLoaderModInfo()
        {
            _scriptableObjects = new()
            {
                { typeof(ScriptableSkill), Skills },
                { typeof(ScriptableStatusCondition), Conditions }
            };
        }

        public void Dispose()
        {
            foreach (var bundle in Bundles)
            {
                bundle.Unload(true);
            }
        }

        public void Initialize()
        {
            LoadJsonObjects<ScriptableSkill>();
            LoadJsonObjects<ScriptableStatusCondition>();
        }

        public void AddAssetBundle(AssetBundle bundle)
        {
            Bundles.Add(bundle);

            // add all the skills
            LoadAssetObjects<ScriptableSkill>();
            LoadAssetObjects<ScriptableStatusCondition>();
        }


        public List<T> GetScriptableObjects<T>() where T : ScriptableObject
        {
            if (_scriptableObjects.ContainsKey(typeof(T)))
            {
                return (List<T>)_scriptableObjects[typeof(T)];
            }

            return new();
        }

        private void LoadAssetObjects<T>() where T : ScriptableObject
        {
            foreach (var bundle in Bundles)
            {
                T[] objects = bundle.LoadAllAssets<T>();
                foreach (var obj in objects)
                {
                    ((List<T>)_scriptableObjects[typeof(T)])
                        .Add(obj); // we don't need to mark the paths, since we're loading from the asset bundles. unity will handle it.
                }
            }
        }

        private void LoadJsonObjects<T>() where T : ScriptableObject
        {
            // we want to load it from Assets/TypeName folder
            string path = ModPath + "/Assets/" + typeof(T).Name;
            if (!Directory.Exists(path))
            {
                return;
            }

            string[] files = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                // cut off the ModPath/Assets/ and .json, because the LoadJsonObject method will add it back

                string relativePath = file.Replace(ModPath + "/Assets/", "");
                relativePath = relativePath.Replace(".json", "");
                LoadJsonObject<T>(relativePath);
            }
        }

        private T LoadJsonObject<T>(string path) where T : ScriptableObject
        {
            return LoadJsonObject(path, typeof(T)) as T;
        }

        private Object LoadJsonObject(string path, System.Type type)
        {
            path = path.Replace(".json", "");

            if (PathToAsset.TryGetValue(path, out var value))
            {
                return value;
            }

            if (!File.Exists(ModPath + "/Assets/" + path + ".json"))
            {
                return null;
            }

            // means either it's not loaded, or it's not in the json assets
            string json = File.ReadAllText(ModPath + "/Assets/" + path + ".json");
            if (string.IsNullOrEmpty(json))
            {
                Plugin.Logger.LogError($"Failed to load json asset {path}");
                return null;
            }

            Object obj = Utility.JsonUtility.LoadFromJson(json, type) as Object;

            if (obj == null)
            {
                Plugin.Logger.LogError($"Failed to load json asset {path}");
                return null;
            }

            PathToAsset.Add(path, obj);
            _scriptableObjects[type].Add(obj);
            return obj;
        }

        public T LoadModAsset<T>(string assetName) where T : Object
        {
            return LoadModAsset(assetName, typeof(T)) as T;
        }

        public Object LoadModAsset(string assetName, System.Type type)
        {
            // check if it's in the asset bundles
            foreach (var bundle in Bundles)
            {
                Object obj = bundle.LoadAsset(assetName, type);
                if (obj != null)
                {
                    return obj;
                }
            }

            // if the type isn't inherited from ScriptableObject, we can't load it from json
            if (!typeof(ScriptableObject).IsAssignableFrom(type))
            {
                return null;
            }

            // check if it's in the json assets
            ScriptableObject returnV = LoadJsonObject(assetName, type) as ScriptableObject;
            if (returnV != null)
            {
                return returnV;
            }

            return null;
        }
    }

    public readonly Dictionary<string, AtlyssToolsLoaderModInfo> ModInfos = new();

    public static void LoadPlugin(string modName, string modPath)
    {
        modName = modName.ToLower();
        AtlyssToolsLoaderModInfo modInfo;
        if (Instance.ModInfos.ContainsKey(modName)) // i don't know why we'd reload a mod, but we can
        {
            modInfo = Instance.ModInfos[modName];
        }
        else
        {
            modInfo = new() { ModId = modName, ModPath = modPath };
            Instance.ModInfos.Add(modName, modInfo);
        }

        // find all files in ModPath/Assets, if it doesn't end in .manifest
        string[] files = Directory.GetFiles(modPath + "/Assets", "*", System.IO.SearchOption.AllDirectories);
        foreach (var file in files)
        {
            if (file.EndsWith(".manifest"))
            {
                continue;
            }

            string manifest = file + ".manifest";
            if (!File.Exists(manifest)) // not an asset bundle
            {
                continue;
            }

            AssetBundle bundle = AssetBundle.LoadFromFile(file);
            if (bundle != null)
            {
                modInfo.AddAssetBundle(bundle);
            }
        }

        // now initialize the mod
        modInfo.Initialize();
    }

    public static void UnloadMod(string modName)
    {
        if (!Instance.ModInfos.ContainsKey(modName))
        {
            return;
        }

        Instance.ModInfos[modName].Dispose();
        Instance.ModInfos.Remove(modName);
    }
    
    public static List<T> GetScriptableObjects<T>() where T : ScriptableObject
    {
        List<T> objects = new();
        foreach (var modInfo in Instance.ModInfos.Values)
        {
            objects.AddRange(modInfo.GetScriptableObjects<T>());
        }

        return objects;
    }
    
    public static List<T> GetScriptableObjects<T>(string modName) where T : ScriptableObject
    {
        if (!Instance.ModInfos.ContainsKey(modName))
        {
            return null;
        }

        return Instance.ModInfos[modName].GetScriptableObjects<T>();
    }

    public static T LoadAsset<T>(string assetName) where T : Object
    {
        return LoadAsset(assetName, typeof(T)) as T;
    }

    public static Object LoadAsset(string assetName, System.Type type)
    {
        if (string.IsNullOrEmpty(assetName))
        {
            return null;
        }

        // we use : to mark a specific mod's assets. default means to return from Resources
        if (assetName.Contains(":"))
        {
            string[] parts = assetName.Split(':');
            if (parts.Length != 2)
            {
                Plugin.Logger.LogError($"Failed to load {assetName}");
                return null;
            }

            return Instance.ModInfos[parts[0].ToLower()].LoadModAsset(parts[1], type);
        }

        // check the resources in each mod bundle
        foreach (var modInfo in Instance.ModInfos.Values)
        {
            Object obj = modInfo.LoadModAsset(assetName, type);
            if (obj != null)
            {
                return obj;
            }
        }

        // check the base resources

        Object r = UnityEngine.Resources.Load(assetName, type);
        if (r != null)
        {
            return r;
        }

        Plugin.Logger.LogError($"Failed to load {assetName}");
        return null;
    }


    // list of assets that need to be registered

    private static AtlyssToolsLoader _instance;

    public static AtlyssToolsLoader Instance => _instance ??= new();
}