using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace AtlyssTools.Registries;

public class ConditionManager<T> : ScriptablesManager<T> where T : ScriptableObject
{
    protected ConditionManager()
    {
    }

    protected override IDictionary InternalGetCached()
    {
        return GameManager._current._cachedScriptableConditions;
    }

    public override string GetName(ScriptableObject obj)
    {
        var condition = (ScriptableCondition)obj;
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
    private static StatusConditionManager _instance;

    protected StatusConditionManager()
    {
    }

    public static StatusConditionManager Instance => _instance ??= new();
}

[ManagerAttribute]
public class SceneTransferConditionManager : ConditionManager<ScriptableSceneTransferCondition>
{
    private static SceneTransferConditionManager _instance;

    protected SceneTransferConditionManager()
    {
    }

    public static SceneTransferConditionManager Instance => _instance ??= new();
}

[ManagerAttribute]
public class PolymorphConditionManager : ConditionManager<ScriptablePolymorphCondition>
{
    private static PolymorphConditionManager _instance;

    protected PolymorphConditionManager()
    {
    }

    public static PolymorphConditionManager Instance => _instance ??= new();
}