using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AtlyssTools.Registries;
using AtlyssTools.Utility;
using BepInEx;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AtlyssTools;

public class AtlyssToolsLoader
{
    public class AtlyssToolsLoaderModInfo
    {
        public readonly List<AssetBundle> Bundles = new();
        public readonly Dictionary<string, Object> PathToAsset = new();
        
        private readonly Dictionary<System.Type, IList> _scriptableObjects = new();

        
        // delegate lists
        public readonly List<Action> OnPreLibraryInit = new();
        public readonly List<Action> OnPostLibraryInit = new();
        public readonly List<Action> OnPreCacheInit = new();
        public readonly List<Action> OnPostCacheInit = new();

        public string ModPath;
        public string ModId;

        public void Dispose()
        {
            foreach (var bundle in Bundles)
            {
                bundle.Unload(true);
            }
        }

        public void Initialize()
        {
        }
        
        public void LoadScriptableType<T>() where T : ScriptableObject
        {
            // check if the type is in the list, if not, add it
            if (!_scriptableObjects.ContainsKey(typeof(T)))
            {
                _scriptableObjects.Add(typeof(T), new List<T>());
            }
            
            Plugin.Logger.LogInfo($"Loading {typeof(T).Name} from {ModId}");
            
            LoadJsonObjects<T>();
            LoadAssetObjects<T>();
            
            Plugin.Logger.LogInfo($"Loaded {typeof(T).Name} from {ModId}. Assets: {_scriptableObjects[typeof(T)].Count}");
        }

        public void LoadAssetBundles()
        {
            string[] files = Directory.GetFiles(ModPath + "/Assets", "*", System.IO.SearchOption.AllDirectories);
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
                    Bundles.Add(bundle);
                }
            }
            
        }


        public List<T> GetModScriptableObjects<T>() where T : ScriptableObject
        {
            if (_scriptableObjects.ContainsKey(typeof(T)))
            {
                return _scriptableObjects[typeof(T)] as List<T>;
            }

            return new();
        }
        
        public IList GetModScriptableObjects(System.Type type)
        {
            if (_scriptableObjects.TryGetValue(type, out var o))
            {
                return o;
            }

            return new List<Object>();
        }

        public void LoadAssetObjects<T>() where T : ScriptableObject
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
                Plugin.Logger.LogError($"Failed to load {typeof(T).Name} from {ModId}. Path { path } does not exist");
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
            Plugin.Logger.LogInfo($"Loaded {typeof(T).Name} from {ModId}. Path { path } Assets: {_scriptableObjects[typeof(T)].Count}");
        }
        
        private T LoadJsonObject<T>(string path) where T : ScriptableObject
        {
            T obj = LoadJsonObject(path, typeof(T)) as T;
            // check if the dictionary already has typeof(T). if not, add it
            if (!_scriptableObjects.ContainsKey(typeof(T)))
            {
                _scriptableObjects.Add(typeof(T), new List<T>());
            }
            
            if (obj != null)
            {
                ((List<T>)_scriptableObjects[typeof(T)]).Add(obj);
            }
            
            // add the path to the dictionary
            PathToAsset.Add(path, obj);
            
            return obj;
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
    
    private readonly LoaderStateMachine _stateMachine = new();

    public class AtlyssLoaderStateManager : LoaderStateManager
    {
        public void PreLibraryInit()
        {
            foreach (var modInfo in AtlyssToolsLoader.Instance.ModInfos.Values)
            {
                foreach (var action in modInfo.OnPreLibraryInit)
                {
                    action?.Invoke();
                }
            }
        }

        public void PostLibraryInit()
        {
            foreach (var modInfo in AtlyssToolsLoader.Instance.ModInfos.Values)
            {
                foreach (var action in modInfo.OnPostLibraryInit)
                {
                    action?.Invoke();
                }
            }
        }

        public void PreCacheInit()
        {
            foreach (var modInfo in AtlyssToolsLoader.Instance.ModInfos.Values)
            {
                foreach (var action in modInfo.OnPreCacheInit)
                {
                    action?.Invoke();
                }
            }
        }

        public void PostCacheInit()
        {
            foreach (var modInfo in AtlyssToolsLoader.Instance.ModInfos.Values)
            {
                foreach (var action in modInfo.OnPostCacheInit)
                {
                    action?.Invoke();
                }
            }
        }
    }

    private readonly Dictionary<System.Type, BaseScriptablesManager> _managers;
    
    AtlyssToolsLoader()
    {
        _managers = new()
        {
            { typeof(ScriptableSkill), SkillManager.Instance },
            { typeof(ScriptableCondition), ConditionManager.Instance },
            { typeof(ScriptablePlayerBaseClass), ClassManager.Instance }
        };
        // register the managers state machines
        foreach(var manager in _managers)
        {
            _stateMachine.RegisterManager(manager.Value.GetStateManager());
        }
    }
    
    public LoaderStateMachine.LoadState State
    {
        get => _stateMachine.State;
        set => _stateMachine.SetState(value);
    }

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
        
        if(!Directory.Exists(modPath + "/Assets"))
        {
            Plugin.Logger.LogError($"Mod {modName} does not have an Assets folder");
            return;
        }

        // find all files in ModPath/Assets, if it doesn't end in .manifest
        modInfo.LoadAssetBundles();
        Plugin.Logger.LogInfo($"Loading AtlyssTools mod asset bundles for {modName}, count: {modInfo.Bundles.Count}");
        
        // Managers
        Plugin.Logger.LogInfo($"Loading AtlyssTools mod managers for {modName}, count: {Instance._managers.Count}");
        foreach (var manager in Instance._managers)
        {
            Plugin.Logger.LogInfo($"Loading {manager.Value.GetObjectType().Name} from {modName}");
            manager.Value.OnModLoad(modInfo);
            Plugin.Logger.LogInfo($"Loaded {manager.Value.GetObjectType().Name} from {modName}: {modInfo.GetModScriptableObjects(manager.Value.GetObjectType()).Count}");
        }
        
        // now initialize the mod
        modInfo.Initialize();
        
        Plugin.Logger.LogInfo($"Loaded AtlyssTools mod {modName}");
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
            objects.AddRange(modInfo.GetModScriptableObjects<T>());
        }

        return objects;
    }
    
    public static List<T> GetScriptableObjects<T>(string modName) where T : ScriptableObject
    {
        if (!Instance.ModInfos.ContainsKey(modName))
        {
            return null;
        }

        return Instance.ModInfos[modName].GetModScriptableObjects<T>();
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
    
    // expose delegate lists
    public static void RegisterPreLibraryInit(string modName, Action action)
    {
        AtlyssToolsLoaderModInfo modInfo = GetModInfo(modName);
        modInfo?.OnPreLibraryInit.Add(action);
    }
    
    public static void RegisterPostLibraryInit(string modName, Action action)
    {
        AtlyssToolsLoaderModInfo modInfo = GetModInfo(modName);
        modInfo?.OnPostLibraryInit.Add(action);
    }
    
    public static void RegisterPreCacheInit(string modName, Action action)
    {
        AtlyssToolsLoaderModInfo modInfo = GetModInfo(modName);
        modInfo?.OnPreCacheInit.Add(action);
    }
    
    public static void RegisterPostCacheInit(string modName, Action action)
    {
        AtlyssToolsLoaderModInfo modInfo = GetModInfo(modName);
        modInfo?.OnPostCacheInit.Add(action);
    }
    
    public static AtlyssToolsLoaderModInfo GetModInfo(string modName)
    {
        modName = modName.ToLower();
        if (!Instance.ModInfos.ContainsKey(modName))
        {
            Plugin.Logger.LogError($"Mod {modName} not found");
            return null;
        }
        return Instance.ModInfos[modName];
    }


    // list of assets that need to be registered

    private static AtlyssToolsLoader _instance;

    public static AtlyssToolsLoader Instance => _instance ??= new();
}