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
        GameManager._current._cachedScriptablePlayerClasses.Add(obj.name, obj as ScriptablePlayerBaseClass);
    }

    protected override ScriptableObject GetFromCacheInternal(string objName)
    {
        return GameManager._current.LocateClass(objName);
    }
    
    protected override IList InternalGetCached()
    {
        return GameManager._current._cachedScriptablePlayerClasses.Values.ToList();
    }
    
    
    public static ClassManager Instance => _instance ??= new();

    private static ClassManager _instance;
}