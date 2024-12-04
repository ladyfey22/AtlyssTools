using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AtlyssTools.Registries;

public abstract class CachelessManager<T> : ScriptablesManager<T> where T : ScriptableObject
{
    protected override IDictionary InternalGetCached()
    {
        return null; // the game doesn't cache these
    }
}


public class StatusConditionManager : ScriptablesManager<ScriptableStatusCondition>
{
    protected override IDictionary InternalGetCached()
    {
        // filter only status conditions
        return GameManager._current._cachedScriptableConditions.Where(x => x.Value is ScriptableStatusCondition).ToDictionary(x => x.Key, x => x.Value);
    }
    
    public override string GetName(ScriptableObject obj)
    {
        return ((ScriptableStatusCondition)obj)._conditionName;
    }
    
    public static StatusConditionManager Instance => _instance ??= new();
    private static StatusConditionManager _instance;
}

public class ClassManager : ScriptablesManager<ScriptablePlayerBaseClass>
{
    protected override IDictionary InternalGetCached()
    {
        return GameManager._current._cachedScriptablePlayerClasses;
    }
    
    public override string GetName(ScriptableObject obj)
    {
        return ((ScriptablePlayerBaseClass)obj)._className;
    }


    public static ClassManager Instance => _instance ??= new();

    private static ClassManager _instance;
}