using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AtlyssTools.Registries;

public abstract class CachelessManager<T> : ScriptablesManager<T> where T : ScriptableObject
{
    private readonly Dictionary<string, T> _cached = new();


    public override void PreCacheInit()
    {
        _cached.Clear();

        T[] assets = UnityEngine.Resources.LoadAll<T>("");
        foreach (T asset in assets)
        {
            string name = GetName(asset);

            if (_cached.ContainsKey(name))
                Plugin.Logger.LogError($"Duplicate asset name {name} for type {typeof(T)}");
            else
            {
                _cached[name] = asset;
            }
        }
    }

    public override string GetName(ScriptableObject obj)
    {
        return obj.name;
    }

    // we want to load the asset directly if we are cacheless


    protected override IDictionary InternalGetCached()
    {
        return _cached;
    }
}

[ManagerAttribute]
public class ArmorRenderManager : CachelessManager<ScriptableArmorRender>
{
    private ArmorRenderManager()
    {
    }

    public static ArmorRenderManager Instance => _instance ??= new();
    private static ArmorRenderManager _instance;
}

[ManagerAttribute]
public class ShopkeepManager : CachelessManager<ScriptableShopkeep>
{
    private ShopkeepManager()
    {
    }

    public override string GetName(ScriptableObject obj)
    {
        return ((ScriptableShopkeep)obj)._shopName;
    }

    public static ShopkeepManager Instance => _instance ??= new();
    private static ShopkeepManager _instance;
}

[ManagerAttribute]
public class CastEffectCollectionManager : CachelessManager<CastEffectCollection>
{
    private CastEffectCollectionManager()
    {
    }

    public static CastEffectCollectionManager Instance => _instance ??= new();
    private static CastEffectCollectionManager _instance;
}