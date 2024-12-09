using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace AtlyssTools.Registries;

public class CreepManager : ScriptablesManager<ScriptableCreep>
{
    private static CreepManager _instance;

    protected CreepManager()
    {
    }

    public static CreepManager Instance => _instance ??= new();

    protected override IDictionary InternalGetCached()
    {
        return GameManager._current._cachedScriptableCreeps;
    }

    public override string GetName(ScriptableObject obj)
    {
        return ((ScriptableCreep)obj)._creepName;
    }

    public override string GetJsonName(JObject obj)
    {
        return obj["_creepName"]?.Value<string>();
    }
}

public class QuestManager : ScriptablesManager<ScriptableQuest>
{
    private static QuestManager _instance;

    protected QuestManager()
    {
    }

    public static QuestManager Instance => _instance ??= new();

    protected override IDictionary InternalGetCached()
    {
        return GameManager._current._cachedScriptableQuests;
    }

    public override string GetName(ScriptableObject obj)
    {
        return ((ScriptableQuest)obj)._questName;
    }

    public override string GetJsonName(JObject obj)
    {
        return obj["_questName"]?.Value<string>();
    }
}

public class PlayerRaceManager : ScriptablesManager<ScriptablePlayerRace>
{
    private static PlayerRaceManager _instance;

    protected PlayerRaceManager()
    {
    }

    public static PlayerRaceManager Instance => _instance ??= new();

    protected override IDictionary InternalGetCached()
    {
        return GameManager._current._cachedScriptableRaces;
    }

    public override string GetName(ScriptableObject obj)
    {
        return ((ScriptablePlayerRace)obj)._raceName;
    }

    public override string GetJsonName(JObject obj)
    {
        return obj["_raceName"]?.Value<string>();
    }
}

public class CombatElementManager : ScriptablesManager<ScriptableCombatElement>
{
    private static CombatElementManager _instance;

    protected CombatElementManager()
    {
    }

    public static CombatElementManager Instance => _instance ??= new();

    protected override IDictionary InternalGetCached()
    {
        return GameManager._current._cachedScriptableCombatElements;
    }

    public override string GetName(ScriptableObject obj)
    {
        return ((ScriptableCombatElement)obj)._elementName;
    }

    public override string GetJsonName(JObject obj)
    {
        return obj["_elementName"]?.Value<string>();
    }
}

public class StatModifierManager : ScriptablesManager<ScriptableStatModifier>
{
    private static StatModifierManager _instance;

    protected StatModifierManager()
    {
    }

    public static StatModifierManager Instance => _instance ??= new();

    protected override IDictionary InternalGetCached()
    {
        return GameManager._current._cachedScriptableStatModifiers;
    }

    public override string GetName(ScriptableObject obj)
    {
        return ((ScriptableStatModifier)obj)._modifierTag;
    }

    public override string GetJsonName(JObject obj)
    {
        return obj["_modifierTag"]?.Value<string>();
    }
}

[ManagerAttribute]
public class PlayerBaseClassManager : ScriptablesManager<ScriptablePlayerBaseClass>
{
    private static PlayerBaseClassManager _instance;

    protected PlayerBaseClassManager()
    {
    }

    public static PlayerBaseClassManager Instance => _instance ??= new();

    protected override IDictionary InternalGetCached()
    {
        return GameManager._current._cachedScriptablePlayerClasses;
    }

    public override string GetName(ScriptableObject obj)
    {
        return ((ScriptablePlayerBaseClass)obj)._className;
    }

    public override string GetJsonName(JObject obj)
    {
        return obj["_className"]?.Value<string>();
    }
}