using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AtlyssTools.Registries;

[ManagerAttribute]
public class StatusConditionManager : ScriptablesManager<ScriptableStatusCondition>
{
    private StatusConditionManager()
    {
    }

    protected override IDictionary InternalGetCached()
    {
        return GameManager._current._cachedScriptableConditions;
    }

    public override string GetName(ScriptableObject obj)
    {
        ScriptableStatusCondition condition = (ScriptableStatusCondition)obj;
        return $"{condition._conditionName}_{condition._conditionRank}";
    }

    public static StatusConditionManager Instance => _instance ??= new();
    private static StatusConditionManager _instance;
}

[ManagerAttribute]
public class SceneTransferConditionManager : ScriptablesManager<ScriptableSceneTransferCondition>
{
    private SceneTransferConditionManager()
    {
    }

    protected override IDictionary InternalGetCached()
    {
        return GameManager._current._cachedScriptableConditions;
    }

    public override string GetName(ScriptableObject obj)
    {
        return ((ScriptableSceneTransferCondition)obj)._conditionName;
    }

    public static SceneTransferConditionManager Instance => _instance ??= new();
    private static SceneTransferConditionManager _instance;
}

[ManagerAttribute]
public class PolymorphConditionManager : ScriptablesManager<ScriptablePolymorphCondition>
{
    private PolymorphConditionManager()
    {
    }

    protected override IDictionary InternalGetCached()
    {
        return GameManager._current._cachedScriptableConditions;
    }

    public override string GetName(ScriptableObject obj)
    {
        return ((ScriptablePolymorphCondition)obj)._conditionName;
    }

    public static PolymorphConditionManager Instance => _instance ??= new();
    private static PolymorphConditionManager _instance;
}