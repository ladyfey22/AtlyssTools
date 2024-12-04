using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
}

[ManagerAttribute]
public class ChestpieceManager : ItemManager<ScriptableChestpiece>
{
    private ChestpieceManager()
    {
    }

    public static ChestpieceManager Instance => _instance ??= new();
    private static ChestpieceManager _instance;
}

[ManagerAttribute]
public class ArmorDyeManager : ItemManager<ScriptableArmorDye>
{
    private ArmorDyeManager()
    {
    }

    public static ArmorDyeManager Instance => _instance ??= new();
    private static ArmorDyeManager _instance;
}

[ManagerAttribute]
public class CapeManager : ItemManager<ScriptableCape>
{
    private CapeManager()
    {
    }

    public static CapeManager Instance => _instance ??= new();
    private static CapeManager _instance;
}

[ManagerAttribute]
public class ClassTomeManager : ItemManager<ScriptableClassTome>
{
    private ClassTomeManager()
    {
    }

    public static ClassTomeManager Instance => _instance ??= new();
    private static ClassTomeManager _instance;
}

[ManagerAttribute]
public class HelmManager : ItemManager<ScriptableHelm>
{
    private HelmManager()
    {
    }

    public static HelmManager Instance => _instance ??= new();
    private static HelmManager _instance;
}

[ManagerAttribute]
public class LeggingsManager : ItemManager<ScriptableLeggings>
{
    private LeggingsManager()
    {
    }

    public static LeggingsManager Instance => _instance ??= new();
    private static LeggingsManager _instance;
}

[ManagerAttribute]
public class RingManager : ItemManager<ScriptableRing>
{
    private RingManager()
    {
    }

    public static RingManager Instance => _instance ??= new();
    private static RingManager _instance;
}

[ManagerAttribute]
public class ShieldManager : ItemManager<ScriptableShield>
{
    private ShieldManager()
    {
    }

    public static ShieldManager Instance => _instance ??= new();
    private static ShieldManager _instance;
}

[ManagerAttribute]
public class SkillScrollManager : ItemManager<ScriptableSkillScroll>
{
    private SkillScrollManager()
    {
    }

    public static SkillScrollManager Instance => _instance ??= new();
    private static SkillScrollManager _instance;
}

[ManagerAttribute]
public class StatusConsumableManager : ItemManager<ScriptableStatusConsumable>
{
    private StatusConsumableManager()
    {
    }

    public static StatusConsumableManager Instance => _instance ??= new();
    private static StatusConsumableManager _instance;
}

[ManagerAttribute]
public class TradeItemManager : ItemManager<ScriptableTradeItem>
{
    private TradeItemManager()
    {
    }

    public static TradeItemManager Instance => _instance ??= new();
    private static TradeItemManager _instance;
}

[ManagerAttribute]
public class WeaponManager : ItemManager<ScriptableWeapon>
{
    private WeaponManager()
    {
    }

    public static WeaponManager Instance => _instance ??= new();
    private static WeaponManager _instance;
}