using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace AtlyssTools.Registries;

/*
ScriptableChestpiece
ScriptableArmorDye
ScriptableCape
ScriptableClassTome
ScriptableHelm
ScriptableLeggings
ScriptableRing
ScriptableShield
ScriptableSkillScroll
ScriptableStatusConsumable
ScriptableTradeItem
ScriptableWeapon
*/
public abstract class ItemManager<T> : ScriptablesManager<T> where T : ScriptableObject
{
    protected override IDictionary InternalGetCached()
    {
        return GameManager._current._cachedScriptableItems;
    }

    public override string GetName(ScriptableObject obj)
    {
        return ((ScriptableItem)obj)._itemName;
    }
    
    public override string GetJsonName(JObject obj)
    {
        return obj["_itemName"]?.Value<string>();
    }
}

[ManagerAttribute]
public class ChestpieceManager : ItemManager<ScriptableChestpiece>
{
    protected ChestpieceManager() { }
    public static ChestpieceManager Instance => _instance ??= new();
    private static ChestpieceManager _instance;
}

[ManagerAttribute]
public class ArmorDyeManager : ItemManager<ScriptableArmorDye>
{
    protected ArmorDyeManager() { }
    public static ArmorDyeManager Instance => _instance ??= new();
    private static ArmorDyeManager _instance;
}

[ManagerAttribute]
public class CapeManager : ItemManager<ScriptableCape>
{
    protected CapeManager() { }
    public static CapeManager Instance => _instance ??= new();
    private static CapeManager _instance;
}

[ManagerAttribute]
public class ClassTomeManager : ItemManager<ScriptableClassTome>
{
    protected ClassTomeManager() { }
    public static ClassTomeManager Instance => _instance ??= new();
    private static ClassTomeManager _instance;
}

[ManagerAttribute]
public class HelmManager : ItemManager<ScriptableHelm>
{
    protected HelmManager() { }
    public static HelmManager Instance => _instance ??= new();
    private static HelmManager _instance;
}

[ManagerAttribute]
public class LeggingsManager : ItemManager<ScriptableLeggings>
{
    protected LeggingsManager() { }
    public static LeggingsManager Instance => _instance ??= new();
    private static LeggingsManager _instance;
}

[ManagerAttribute]
public class RingManager : ItemManager<ScriptableRing>
{
    protected RingManager() { }
    public static RingManager Instance => _instance ??= new();
    private static RingManager _instance;
}

[ManagerAttribute]
public class ShieldManager : ItemManager<ScriptableShield>
{
    protected ShieldManager() { }
    public static ShieldManager Instance => _instance ??= new();
    private static ShieldManager _instance;
}

[ManagerAttribute]
public class SkillScrollManager : ItemManager<ScriptableSkillScroll>
{
    protected SkillScrollManager() { }
    public static SkillScrollManager Instance => _instance ??= new();
    private static SkillScrollManager _instance;
}

[ManagerAttribute]
public class StatusConsumableManager : ItemManager<ScriptableStatusConsumable>
{
    protected StatusConsumableManager() { }
    public static StatusConsumableManager Instance => _instance ??= new();
    private static StatusConsumableManager _instance;
}

[ManagerAttribute]
public class TradeItemManager : ItemManager<ScriptableTradeItem>
{
    protected TradeItemManager() { }
    public static TradeItemManager Instance => _instance ??= new();
    private static TradeItemManager _instance;
}

[ManagerAttribute]
public class WeaponManager : ItemManager<ScriptableWeapon>
{
    protected WeaponManager() { }
    public static WeaponManager Instance => _instance ??= new();
    private static WeaponManager _instance;
}