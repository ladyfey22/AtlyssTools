using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace AtlyssTools.Registries;

public class CreepManager : ScriptablesManager<ScriptableCreep>
{
    protected CreepManager() { }

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

    public static CreepManager Instance => _instance ??= new();
    private static CreepManager _instance;
}

public class QuestManager : ScriptablesManager<ScriptableQuest>
{
    protected QuestManager()
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
    
    public override string GetJsonName(JObject obj)
    {
        return obj["_questName"]?.Value<string>();
    }

    public static QuestManager Instance => _instance ??= new();
    private static QuestManager _instance;
}

public class PlayerRaceManager : ScriptablesManager<ScriptablePlayerRace>
{
    protected PlayerRaceManager()
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
    
    public override string GetJsonName(JObject obj)
    {
        return obj["_raceName"]?.Value<string>();
    }

    public static PlayerRaceManager Instance => _instance ??= new();
    private static PlayerRaceManager _instance;
}

public class CombatElementManager : ScriptablesManager<ScriptableCombatElement>
{
    protected CombatElementManager() { }

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

    public static CombatElementManager Instance => _instance ??= new();
    private static CombatElementManager _instance;
}

public class StatModifierManager : ScriptablesManager<ScriptableStatModifier>
{
    protected StatModifierManager()
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
    
    public override string GetJsonName(JObject obj)
    {
        return obj["_modifierTag"]?.Value<string>();
    }

    public static StatModifierManager Instance => _instance ??= new();
    private static StatModifierManager _instance;
}

[ManagerAttribute]
public class PlayerBaseClassManager : ScriptablesManager<ScriptablePlayerBaseClass>
{
    protected PlayerBaseClassManager()
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
    
    public override string GetJsonName(JObject obj)
    {
        return obj["_className"]?.Value<string>();
    }

    public static PlayerBaseClassManager Instance => _instance ??= new();
    private static PlayerBaseClassManager _instance;
}