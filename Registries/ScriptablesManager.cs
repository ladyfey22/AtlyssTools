using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AtlyssTools.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using JsonUtility = AtlyssTools.Utility.JsonUtility;

namespace AtlyssTools.Registries;

public abstract class BaseScriptablesManager
{
    /// <summary>
    /// Gets the type of object that the manager is managing.
    /// </summary>
    /// <returns>The type of object that the manager is managing.</returns>
    public abstract System.Type GetObjectType();

    /// <summary>
    /// Gets the cached objects for the manager, which is used to store the objects.
    /// If this is a scriptable object with a base game cache, it should return the GameManager's cached objects.
    /// If this has no default cache, it should keep its own cache.
    /// </summary>
    /// <returns> The cached objects for the manager. </returns>
    
    protected abstract IDictionary InternalGetCached();
    
    /// <summary>
    /// Gets the state manager for the object, which is used to call the various OnState methods.
    /// </summary>
    /// <returns></returns>
    public abstract LoaderStateManager GetStateManager();
    
    /// <summary>
    /// Called when a mod is loaded.
    /// </summary>
    /// <param name="modInfo"></param>
    public abstract void OnModLoad(AtlyssToolsLoaderModInfo modInfo);
    
    /// <summary>
    /// Creates a new instance of the object.
    /// </summary>
    /// <returns>A new instance of the scriptable object.</returns>
    public abstract ScriptableObject Instantiate();

    /// <summary>
    /// Gets the name of the object from the ScriptableObject.
    /// </summary>
    /// <param name="obj">The base object to get the name from.</param>
    /// <returns>The name of the object.</returns>
    public abstract string GetName(ScriptableObject obj);
    
    /// <summary>
    /// Gets the name of the object from a JSON file.
    /// </summary>
    /// <param name="obj">The base object to get the name from.</param>
    /// <returns>The name of the object, or null if failed to locate.</returns>
    public abstract string GetJsonName(JObject obj);
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

    public virtual IList GetModdedObjects()
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

    public void LoadToCache()
    {
        // get skills from AtlyssToolsLoader
        var objects = AtlyssToolsLoader.GetScriptableObjects<T>();
        // for each, check if it's in the cache and add it if not
        foreach (var obj in objects)
        {
            Register(obj);
        }
    }

    public override void OnModLoad(AtlyssToolsLoaderModInfo modInfo)
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
        LoadToCache();
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