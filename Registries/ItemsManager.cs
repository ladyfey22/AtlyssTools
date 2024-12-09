using System.Collections;
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
    private static ChestpieceManager _instance;

    protected ChestpieceManager()
    {
    }

    public static ChestpieceManager Instance => _instance ??= new();
}

[ManagerAttribute]
public class ArmorDyeManager : ItemManager<ScriptableArmorDye>
{
    private static ArmorDyeManager _instance;

    protected ArmorDyeManager()
    {
    }

    public static ArmorDyeManager Instance => _instance ??= new();
}

[ManagerAttribute]
public class CapeManager : ItemManager<ScriptableCape>
{
    private static CapeManager _instance;

    protected CapeManager()
    {
    }

    public static CapeManager Instance => _instance ??= new();
}

[ManagerAttribute]
public class ClassTomeManager : ItemManager<ScriptableClassTome>
{
    private static ClassTomeManager _instance;

    protected ClassTomeManager()
    {
    }

    public static ClassTomeManager Instance => _instance ??= new();
}

[ManagerAttribute]
public class HelmManager : ItemManager<ScriptableHelm>
{
    private static HelmManager _instance;

    protected HelmManager()
    {
    }

    public static HelmManager Instance => _instance ??= new();
}

[ManagerAttribute]
public class LeggingsManager : ItemManager<ScriptableLeggings>
{
    private static LeggingsManager _instance;

    protected LeggingsManager()
    {
    }

    public static LeggingsManager Instance => _instance ??= new();
}

[ManagerAttribute]
public class RingManager : ItemManager<ScriptableRing>
{
    private static RingManager _instance;

    protected RingManager()
    {
    }

    public static RingManager Instance => _instance ??= new();
}

[ManagerAttribute]
public class ShieldManager : ItemManager<ScriptableShield>
{
    private static ShieldManager _instance;

    protected ShieldManager()
    {
    }

    public static ShieldManager Instance => _instance ??= new();
}

[ManagerAttribute]
public class SkillScrollManager : ItemManager<ScriptableSkillScroll>
{
    private static SkillScrollManager _instance;

    protected SkillScrollManager()
    {
    }

    public static SkillScrollManager Instance => _instance ??= new();
}

[ManagerAttribute]
public class StatusConsumableManager : ItemManager<ScriptableStatusConsumable>
{
    private static StatusConsumableManager _instance;

    protected StatusConsumableManager()
    {
    }

    public static StatusConsumableManager Instance => _instance ??= new();
}

[ManagerAttribute]
public class TradeItemManager : ItemManager<ScriptableTradeItem>
{
    private static TradeItemManager _instance;

    protected TradeItemManager()
    {
    }

    public static TradeItemManager Instance => _instance ??= new();
}

[ManagerAttribute]
public class WeaponManager : ItemManager<ScriptableWeapon>
{
    private static WeaponManager _instance;

    protected WeaponManager()
    {
    }

    public static WeaponManager Instance => _instance ??= new();
}