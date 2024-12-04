using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AtlyssTools.Utility;
using Newtonsoft.Json;
using UnityEngine;

namespace AtlyssTools.Registries;

public abstract class BaseScriptablesManager
{
    protected abstract void RegisterInternal(ScriptableObject obj);
    protected abstract ScriptableObject GetFromCacheInternal(string objName);
    public abstract System.Type GetObjectType();
    public abstract IList GetModdedObjects();
    protected abstract IList InternalGetCached();
    public abstract LoaderStateManager GetStateManager();
    public abstract void OnModLoad(AtlyssToolsLoader.AtlyssToolsLoaderModInfo modInfo);
    public abstract void RegisterModObject(AtlyssToolsLoader.AtlyssToolsLoaderModInfo modInfo, ScriptableObject obj);
    public abstract JsonSerializerSettings GetJsonSettings();
}

public abstract class ScriptablesManager<T> : BaseScriptablesManager where T : ScriptableObject
{
    private class ScriptablesStateManager(ScriptablesManager<T> manager) : LoaderStateManager
    {
        public void PreCacheInit() => manager.PreCacheInit();
        public void PostCacheInit() => manager.PostCacheInit();
        public void PreLibraryInit() => manager.PreLibraryInit();
        public void PostLibraryInit() => manager.PostLibraryInit();
    }


    public ScriptablesManager()
    {
        StateManager = new ScriptablesStateManager(this);
    }

    void Register(T obj)
    {
        if (obj == null)
        {
            Plugin.Logger.LogError("Attempted to register a null object");
            return;
        }

        if (GetFromCache(obj.name) != null)
        {
            Plugin.Logger.LogError($"Object {obj.name} is already in the cache");
            return;
        }
        Plugin.Logger.LogInfo($"Registering {obj.name}");
        RegisterInternal(obj);
    }

    public override System.Type GetObjectType()
    {
        return typeof(T);
    }

    public override IList GetModdedObjects()
    {
        return AtlyssToolsLoader.GetScriptableObjects<T>();
    }

    public override void RegisterModObject(AtlyssToolsLoader.AtlyssToolsLoaderModInfo modInfo, ScriptableObject obj)
    {
    }


    public List<T> GetModded()
    {
        return GetModdedObjects().Cast<T>().ToList();
    }

    public List<T> GetCached()
    {
        return InternalGetCached().Cast<T>().ToList();
    }

    public void LoadAllFromAssets()
    {
        // get skills from AtlyssToolsLoader
        var objects = AtlyssToolsLoader.GetScriptableObjects<T>();
        // for each, check if it's in the cache and add it if not
        foreach (var obj in objects)
        {
            Register(obj);
        }
    }

    public override void OnModLoad(AtlyssToolsLoader.AtlyssToolsLoaderModInfo modInfo)
    {
        // asset bundles already loaded
        modInfo.LoadScriptableType<T>();
    }

    public readonly LoaderStateManager StateManager;
    public override LoaderStateManager GetStateManager() => StateManager;
    public virtual T GetFromCache(string objName) => GetFromCacheInternal(objName) as T;

    public virtual void PreCacheInit()
    {
    }

    public virtual void PostCacheInit() => LoadAllFromAssets();

    public virtual void PreLibraryInit()
    {
    }

    public virtual void PostLibraryInit()
    {
    }
}