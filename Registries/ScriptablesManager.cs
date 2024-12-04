using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AtlyssTools.Utility;
using Newtonsoft.Json;
using UnityEngine;

namespace AtlyssTools.Registries;

public abstract class BaseScriptablesManager
{
    public abstract System.Type GetObjectType();
    public abstract IList GetModdedObjects();
    protected abstract IDictionary InternalGetCached();
    public abstract LoaderStateManager GetStateManager();
    public abstract void OnModLoad(AtlyssToolsLoader.AtlyssToolsLoaderModInfo modInfo);
    public abstract void RegisterModObject(AtlyssToolsLoader.AtlyssToolsLoaderModInfo modInfo, ScriptableObject obj);
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


    public ScriptablesManager()
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

        if (GetFromCache(obj.name) != null)
        {
            Plugin.Logger.LogError($"Object {obj.name} is already in the cache for type {typeof(T)}");
            return;
        }
        
        IDictionary cache = InternalGetCached();
        if(cache == null)
        {
            Plugin.Logger.LogError($"Cache is null for type {typeof(T)}");
            return;
        }
        string name = GetName(obj);
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

    public override void RegisterModObject(AtlyssToolsLoader.AtlyssToolsLoaderModInfo modInfo, ScriptableObject obj)
    {
    }


    public List<T> GetModded()
    {
        return GetModdedObjects().Cast<T>().ToList();
    }

    public Dictionary<string, T>  GetCached()
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
    
    public void ReplaceAll(T to, T from)
    {
        // copy the values
        System.Type sourceType = from.GetType();
        System.Type destType = to.GetType();
        
        if(sourceType != destType)
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
    
    public override ScriptableObject Instantiate()
    {
        return ScriptableObject.CreateInstance<T>();
    }
}