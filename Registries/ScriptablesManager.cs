using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AtlyssTools.Utility;
using Newtonsoft.Json;
using UnityEngine;
using JsonUtility = AtlyssTools.Utility.JsonUtility;

namespace AtlyssTools.Registries;

public abstract class BaseScriptablesManager
{
    public abstract System.Type GetObjectType();
    public abstract IList GetModdedObjects();
    protected abstract IDictionary InternalGetCached();
    public abstract LoaderStateManager GetStateManager();
    public abstract void OnModLoad(AtlyssToolsLoader.AtlyssToolsLoaderModInfo modInfo);
    public abstract ScriptableObject Instantiate();
    public abstract string GetName(ScriptableObject obj);
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


    protected ScriptablesManager()
    {
        StateManager = new ScriptablesStateManager(this);
    }

    void Register(T obj)
    {
        if (obj == null)
        {
            Plugin.Logger.LogError("Attempted to register a null object for type " + typeof(T));
            return;
        }

        IDictionary cache = InternalGetCached();
        string name = GetName(obj);

        // check if it's already in the cache

        if (cache.Contains(name))
        {
            // replace it
            Plugin.Logger.LogInfo($"Replacing {name} in cache for type {typeof(T)}");
            Replace(cache[name] as T, obj);
            return;
        }

        cache.Add(name, obj);
    }

    public override System.Type GetObjectType()
    {
        return typeof(T);
    }

    public override IList GetModdedObjects()
    {
        return AtlyssToolsLoader.GetScriptableObjects<T>();
    }


    public List<T> GetModded()
    {
        return GetModdedObjects().Cast<T>().ToList();
    }

    public Dictionary<string, T> GetCached()
    {
        return InternalGetCached() as Dictionary<string, T>;
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

    public T GetFromCache(string objName)
    {
        IDictionary cache = InternalGetCached();
        if (cache.Contains(objName))
        {
            return cache[objName] as T;
        }

        return null;
    }

    public void Replace(T to, T from)
    {
        // copy the values
        System.Type sourceType = from.GetType();
        System.Type destType = to.GetType();

        if (sourceType != destType)
        {
            Plugin.Logger.LogError($"Source type {sourceType} does not match destination type {destType}");
            return;
        }

        // copy the values

        // get all fields
        System.Reflection.FieldInfo[] fields = sourceType.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(to, field.GetValue(from));
        }

        // get all properties
        System.Reflection.PropertyInfo[] properties = sourceType.GetProperties();
        foreach (System.Reflection.PropertyInfo property in properties)
        {
            property.SetValue(to, property.GetValue(from));
        }
    }

    public void ReplaceCachedAll(string destCache, string sourceCache)
    {
        T source = GetFromCache(sourceCache);
        T dest = GetFromCache(destCache);

        if (source == null)
        {
            Plugin.Logger.LogError($"Source cache {sourceCache} is null");
            return;
        }

        if (dest == null)
        {
            Plugin.Logger.LogError($"Destination cache {destCache} is null");
            return;
        }

        Replace(dest, source);
    }

    public virtual void PreCacheInit()
    {
    }

    public virtual void PostCacheInit()
    {
        LoadAllFromAssets();
    }

    public virtual void PreLibraryInit()
    {
    }

    public virtual void PostLibraryInit()
    {
    }

    public override ScriptableObject Instantiate()
    {
        return ScriptableObject.CreateInstance<T>();
    }
}