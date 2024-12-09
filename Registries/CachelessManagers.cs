using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace AtlyssTools.Registries;

public abstract class CachelessManager<T> : ScriptablesManager<T> where T : ScriptableObject
{
    private readonly Dictionary<string, T> _cached = new();

    public override void PreCacheInit()
    {
        _cached.Clear();

        var assets = UnityEngine.Resources.LoadAll<T>("");
        foreach (var asset in assets)
        {
            var name = GetName(asset);

            if (_cached.ContainsKey(name))
                Plugin.Logger.LogError($"Duplicate asset name {name} for type {typeof(T)}");
            else
                _cached[name] = asset;
        }
    }

    public override string GetName(ScriptableObject obj)
    {
        return obj.name;
    }

    public override string GetJsonName(JObject obj)
    {
        return obj["name"]?.Value<string>();
    }

    protected override IDictionary InternalGetCached()
    {
        return _cached;
    }
}

[ManagerAttribute]
public class ArmorRenderManager : CachelessManager<ScriptableArmorRender>
{
    private static ArmorRenderManager _instance;

    protected ArmorRenderManager()
    {
    }

    public static ArmorRenderManager Instance => _instance ??= new();
}

[ManagerAttribute]
public class ShopkeepManager : CachelessManager<ScriptableShopkeep>
{
    private static ShopkeepManager _instance;

    protected ShopkeepManager()
    {
    }

    public static ShopkeepManager Instance => _instance ??= new();

    public override string GetName(ScriptableObject obj)
    {
        return ((ScriptableShopkeep)obj)._shopName;
    }
}

[ManagerAttribute]
public class CastEffectCollectionManager : CachelessManager<CastEffectCollection>
{
    private static CastEffectCollectionManager _instance;

    protected CastEffectCollectionManager()
    {
    }

    public static CastEffectCollectionManager Instance => _instance ??= new();
}