using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            LoadJsonObjects<T>();
            LoadAssetObjects<T>();
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
                Plugin.Logger.LogError($"Failed to load {typeof(T).Name} from {ModId}. Path {path} does not exist");
                return;
            }

            string[] files = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                // cut off the ModPath/Assets/ and .json, because the LoadJsonObject method will add it back

                string relativePath = file.Replace(ModPath + "/Assets/", "");
                relativePath = relativePath.Replace(".json", "");
                relativePath = relativePath.Replace("\\", "/");
                LoadJsonObject<T>(relativePath);
            }
        }

        private static readonly System.Type[] NonConcreteType = new System.Type[]
        {
            typeof(ScriptableCondition)
        };
        
        private static List<System.Type> ConcreteType = new List<System.Type>()
        {
            typeof(ScriptableStatusCondition),
            typeof(ScriptableSceneTransferCondition),
            typeof(ScriptablePolymorphCondition),
        };
        
        private T LoadJsonObject<T>(string path) where T : Object
        {
            if(string.IsNullOrEmpty(path))
            {
                return null;
            }           
            
            System.Type type = typeof(T);
            
            // if this is a non-concrete type, we need to check the json for the type
            if (NonConcreteType.Contains(type))
            {
                // we need to deduce the type from the path
                string[] parts = path.Split('/');
                if (parts.Length < 2)
                {
                    return null;
                }

                string typeName = parts[0].ToLower();
                //find the one that matches a type in ConcreteType
                foreach (var t in ConcreteType)
                {
                    if (t.Name.ToLower() == typeName)
                    {
                        type = t;
                        break;
                    }
                }
                
                if (type == typeof(T))
                {
                    return null;
                }
            }
            
            // check if the dictionary already has typeof(T). if not, add it
            if (!_scriptableObjects.ContainsKey(type))
            {
                _scriptableObjects.Add(typeof(T), new List<T>());
            }

            path = path.Replace(".json", "");

            if (PathToAsset.TryGetValue(path, out var value))
            {
                return value as T;
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

            // check that this is a ScriptableObject
            if (!type.IsSubclassOf(typeof(ScriptableObject)) && type != typeof(ScriptableObject))
            {
                Plugin.Logger.LogError($"Type {typeof(T).Name} is not a ScriptableObject");
                return null;
            }

            T obj = Utility.JsonUtility.LoadFromJson(json, type) as T;

            if (obj == null)
            {
                Plugin.Logger.LogError($"Failed to load json asset {path}");
                return null;
            }

            ((List<T>)_scriptableObjects[typeof(T)]).Add(obj);
            PathToAsset.Add(path, obj);
            return obj;
        }

        public T LoadModAsset<T>(string assetName) where T : Object
        {
            // check if it's in the asset bundles
            foreach (var bundle in Bundles)
            {
                T obj = bundle.LoadAsset<T>(assetName);
                if (obj != null)
                {
                    return obj;
                }
            }
            
            // check if T is a scriptable object
            if (typeof(T).IsSubclassOf(typeof(ScriptableObject)) || typeof(T) == typeof(ScriptableObject))
            {
                T obj = LoadJsonObject<T>(assetName);
                if (obj != null)
                {
                    return obj;
                }
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
            { typeof(ScriptableStatusCondition), StatusConditionManager.Instance },
            { typeof(ScriptablePlayerBaseClass), ClassManager.Instance }
        };
        // register the managers state machines
        foreach (var manager in _managers)
        {
            _stateMachine.RegisterManager(manager.Value.GetStateManager());
        }

        _stateMachine.RegisterManager(new AtlyssLoaderStateManager());
    }
    
    public BaseScriptablesManager GetManager(System.Type type)
    {
        if (_managers.TryGetValue(type, out var manager))
        {
            return manager;
        }

        return null;
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

        if (!Directory.Exists(modPath + "/Assets"))
        {
            Plugin.Logger.LogError($"Mod {modName} does not have an Assets folder");
            return;
        }

        // find all files in ModPath/Assets, if it doesn't end in .manifest
        modInfo.LoadAssetBundles();
        // Managers
        foreach (var manager in Instance._managers)
        {
            manager.Value.OnModLoad(modInfo);
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
        if (string.IsNullOrEmpty(assetName))
        {
            return null;
        }
        
        // replace \\ with / for windows
        assetName = assetName.Replace("\\", "/");

        // we use : to mark a specific mod's assets. default means to return from Resources
        if (assetName.Contains(":"))
        {
            string[] parts = assetName.Split(':');
            if (parts.Length != 2)
            {
                Plugin.Logger.LogError($"Failed to load {assetName}");
                return null;
            }

            return Instance.ModInfos[parts[0].ToLower()].LoadModAsset<T>(parts[1]);
        }

        // check the resources in each mod bundle
        foreach (var modInfo in Instance.ModInfos.Values)
        {
            T obj = modInfo.LoadModAsset<T>(assetName);
            if (obj != null)
            {
                return obj;
            }
        }

        // check the base resources

        T r = UnityEngine.Resources.Load<T>(assetName);
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