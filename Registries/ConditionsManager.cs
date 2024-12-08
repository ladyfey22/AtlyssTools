using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace AtlyssTools.Registries;

public class ConditionManager<T> : ScriptablesManager<T> where T : ScriptableObject 
{
    protected ConditionManager() { }
    
    protected override IDictionary InternalGetCached()
    {
        return GameManager._current._cachedScriptableConditions;
    }

    public override string GetName(ScriptableObject obj)
    {
        ScriptableCondition condition = (ScriptableCondition)obj;
        return $"{condition._conditionName}_{condition._conditionRank}";
    }

    public override string GetJsonName(JObject obj)
    {
        return obj["_conditionName"]?.Value<string>();
    }
}

[ManagerAttribute]
public class StatusConditionManager : ConditionManager<ScriptableStatusCondition>
{
    protected StatusConditionManager() { }
    public static StatusConditionManager Instance => _instance ??= new();
    private static StatusConditionManager _instance;
}

[ManagerAttribute]
public class SceneTransferConditionManager : ConditionManager<ScriptableSceneTransferCondition>
{
    protected SceneTransferConditionManager() { }
    public static SceneTransferConditionManager Instance => _instance ??= new();
    private static SceneTransferConditionManager _instance;
}

[ManagerAttribute]
public class PolymorphConditionManager : ConditionManager<ScriptablePolymorphCondition>
{
    protected PolymorphConditionManager() { }
    public static PolymorphConditionManager Instance => _instance ??= new();
    private static PolymorphConditionManager _instance;
}