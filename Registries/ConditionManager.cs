using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AtlyssTools.Utility;
using BepInEx;
using Newtonsoft.Json;
using UnityEngine;

namespace AtlyssTools.Registries;

public class ConditionManager : ScriptablesManager<ScriptableStatusCondition>
{
    protected override void RegisterInternal(ScriptableObject obj)
    {
        GameManager._current._cachedScriptableConditions.Add(obj.name, obj as ScriptableStatusCondition);
    }

    protected override ScriptableObject GetFromCacheInternal(string objName)
    {
        return GameManager._current.LocateCondition(objName);
    }
    
    protected override IList InternalGetCached()
    {
        return GameManager._current._cachedScriptableConditions.Values.ToList();
    }
    
    public static ConditionManager Instance => _instance ??= new();
    private static ConditionManager _instance;
}