using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtlyssTools.Registries;

public class CreepManager : ScriptablesManager<ScriptableCreep>
{
    private CreepManager()
    {
    }

    protected override IDictionary InternalGetCached()
    {
        return GameManager._current._cachedScriptableCreeps;
    }

    public override string GetName(ScriptableObject obj)
    {
        return ((ScriptableCreep)obj)._creepName;
    }

    public static CreepManager Instance => _instance ??= new();
    private static CreepManager _instance;
}

public class QuestManager : ScriptablesManager<ScriptableQuest>
{
    private QuestManager()
    {
    }

    protected override IDictionary InternalGetCached()
    {
        return GameManager._current._cachedScriptableQuests;
    }

    public override string GetName(ScriptableObject obj)
    {
        return ((ScriptableQuest)obj)._questName;
    }

    public static QuestManager Instance => _instance ??= new();
    private static QuestManager _instance;
}

public class PlayerRaceManager : ScriptablesManager<ScriptablePlayerRace>
{
    private PlayerRaceManager()
    {
    }

    protected override IDictionary InternalGetCached()
    {
        return GameManager._current._cachedScriptableRaces;
    }

    public override string GetName(ScriptableObject obj)
    {
        return ((ScriptablePlayerRace)obj)._raceName;
    }

    public static PlayerRaceManager Instance => _instance ??= new();
    private static PlayerRaceManager _instance;
}

public class CombatElementManager : ScriptablesManager<ScriptableCombatElement>
{
    private CombatElementManager()
    {
    }

    protected override IDictionary InternalGetCached()
    {
        return GameManager._current._cachedScriptableCombatElements;
    }

    public override string GetName(ScriptableObject obj)
    {
        return ((ScriptableCombatElement)obj)._elementName;
    }

    public static CombatElementManager Instance => _instance ??= new();
    private static CombatElementManager _instance;
}

public class StatModifierManager : ScriptablesManager<ScriptableStatModifier>
{
    private StatModifierManager()
    {
    }

    protected override IDictionary InternalGetCached()
    {
        return GameManager._current._cachedScriptableStatModifiers;
    }

    public override string GetName(ScriptableObject obj)
    {
        return ((ScriptableStatModifier)obj)._modifierTag;
    }

    public static StatModifierManager Instance => _instance ??= new();
    private static StatModifierManager _instance;
}

[ManagerAttribute]
public class PlayerBaseClassManager : ScriptablesManager<ScriptablePlayerBaseClass>
{
    private PlayerBaseClassManager()
    {
    }

    protected override IDictionary InternalGetCached()
    {
        return GameManager._current._cachedScriptablePlayerClasses;
    }

    public override string GetName(ScriptableObject obj)
    {
        return ((ScriptablePlayerBaseClass)obj)._className;
    }

    public static PlayerBaseClassManager Instance => _instance ??= new();
    private static PlayerBaseClassManager _instance;
}