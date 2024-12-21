using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AtlyssTools.Utility;

public class AssetConverter<T> : JsonConverter<T> where T : Object
{
    public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
    {
        // write the name of the object
        writer.WriteValue(value.name);
    }

    public override T ReadJson(JsonReader reader, System.Type objectType, T existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var name = reader!.Value?.ToString();

        var returnV = AtlyssToolsLoader.LoadAsset<T>(name);


        return returnV;
    }
}

// unity vector3, because Normalize causes a loop
public class Vector3Converter : JsonConverter<Vector3>
{
    public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
    {
        // write in 3 separate values
        writer.WriteStartArray();
        writer.WriteValue(value.x);
        writer.WriteValue(value.y);
        writer.WriteValue(value.z);
        writer.WriteEndArray();
    }

    public override Vector3 ReadJson(JsonReader reader, System.Type objectType, Vector3 existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        // read in 3 separate values
        reader.Read();
        if (reader.Value != null)
        {
            var x = Convert.ToSingle(reader.Value);
            reader.Read();
            var y = Convert.ToSingle(reader.Value);
            reader.Read();
            var z = Convert.ToSingle(reader.Value);
            reader.Read();//End of array
            return new(x, y, z);
        }

        return Vector3.zero;
    }
}

//	_skillName,_skillIcon, _skillControlType,_skillDamageType,_skillDescription,_skillType,_skillCost,_skillCooldown,_skillCastTime,_skillRange,_skillTarget,_skillEffects,_skillWeaponType,_skillCondition,_skillSound,_skillPrefab,_skillCombatElement,_skillRankParams,_skillRanks,_movementSpeedDecrease,_strafeDirection,_groundOnlySkill,_airTimeLockBuffer,_enableAirTimeMod,_lockMoveControlTime,_lockCombatControlTime,_timeBeforeAttackForce,_attackForceApply,_attackForceDampen,_attackForceInputInfluence,_castEffectCollection,_useGlyphEffect,_skillChargeAnimation,_skillCastAnimation,_animChargeSmoothing,_animCastSmoothing,_setSheatheCondition,_applySheatheCondition,_hideWeapon

// scriptable objects need their own converters, since ScriptableObjects need to be instantiated using ScriptableObject,Instantiate
public class ColorConverter : JsonConverter<Color>
{
    public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
    {
        // write in 4 separate values
        writer.WriteStartArray();
        writer.WriteValue(value.r);
        writer.WriteValue(value.g);
        writer.WriteValue(value.b);
        writer.WriteValue(value.a);
        writer.WriteEndArray();
    }

    public override Color ReadJson(JsonReader reader, System.Type objectType, Color existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        reader.Read();
        if (reader.Value != null)
        {
            // write the type of the object
            var r = (float)(double)reader.Value;
            reader.Read();
            var g = (float)(double)reader.Value;
            reader.Read();
            var b = (float)(double)reader.Value;
            reader.Read();
            var a = (float)(double)reader.Value;
            // end array
            reader.Read();
            return new(r, g, b, a);
        }

        return Color.white;
    }
}

public class BaseConverter<T> : JsonConverter<T> where T : ScriptableObject
{
    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override T ReadJson(JsonReader reader, System.Type objectType, T existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        // check the depth. 0 depth = the object we need to read directly, anything past that, scriptable objects should be a string referring to the asset

        if (reader.Depth == 0)
        {
            var obj = ScriptableObject.CreateInstance<T>();
            serializer.Populate(reader, obj);
            return obj;
        }

        // read the asset name
        var name = reader.Value?.ToString();
        if (string.IsNullOrEmpty(name)) return null;

        var asset = AtlyssToolsLoader.LoadAsset<T>(name);

        if (asset == null) Plugin.Logger.LogError($"Failed to load asset {name} in {JsonUtility.currentFilePath}");

        return asset;
    }
}

public class Vector2Converter : JsonConverter<Vector2>
{
    public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
    {
        // write in 2 separate values
        writer.WriteStartArray();
        writer.WriteValue(value.x);
        writer.WriteValue(value.y);
        writer.WriteEndArray();
    }

    public override Vector2 ReadJson(JsonReader reader, System.Type objectType, Vector2 existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        // read in 2 separate values
        reader.Read();
        var x = Convert.ToSingle(reader.Value);
        reader.Read();
        var y = Convert.ToSingle(reader.Value);
        reader.Read();
        return new(x, y);
    }
}

// vector4
public class Vector4Converter : JsonConverter<Vector4>
{
    public override void WriteJson(JsonWriter writer, Vector4 value, JsonSerializer serializer)
    {
        // write in 4 separate values
        writer.WriteStartArray();
        writer.WriteValue(value.x);
        writer.WriteValue(value.y);
        writer.WriteValue(value.z);
        writer.WriteValue(value.w);
        writer.WriteEndArray();
    }

    public override Vector4 ReadJson(JsonReader reader, System.Type objectType, Vector4 existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        // read in 4 separate values
        reader.Read();
        var x = Convert.ToSingle(reader.Value);
        reader.Read();
        var y = Convert.ToSingle(reader.Value);
        reader.Read();
        var z = Convert.ToSingle(reader.Value);
        reader.Read();
        var w = Convert.ToSingle(reader.Value);
        reader.Read();
        return new(x, y, z, w);
    }
}

public class JsonUtility
{
    public static string currentFilePath; // for debugging

    private static List<JsonConverter> _converters;

    private static readonly List<System.Type> ScriptableTypes =
    [
        typeof(ScriptableArmorRender),
        typeof(ScriptableCombatElement),
        typeof(ScriptableCreep),
        typeof(ScriptableDialogData),
        typeof(ScriptableEmoteList),
        typeof(ScriptableLootTable),
        typeof(ScriptableMapData),
        typeof(ScriptablePlayerBaseClass),
        typeof(ScriptablePlayerRace),
        typeof(ScriptablePolymorphCondition),
        typeof(ScriptableQuest),
        typeof(ScriptableSceneTransferCondition),
        typeof(ScriptableShopkeep),
        typeof(ScriptableSkill),
        typeof(ScriptableStatAttribute),
        typeof(ScriptableStatModifier),
        typeof(ScriptableStatModifierTable),
        typeof(ScriptableStatusCondition),
        typeof(ScriptableWeaponProjectileSet),
        typeof(ScriptableWeaponType),
        typeof(CastEffectCollection),
        typeof(ScriptableChestpiece),
        typeof(ScriptableArmor),
        typeof(ScriptableArmorDye),
        typeof(ScriptableCape),
        typeof(ScriptableClassTome),
        typeof(ScriptableHelm),
        typeof(ScriptableLeggings),
        typeof(ScriptableRing),
        typeof(ScriptableShield),
        typeof(ScriptableSkillScroll),
        typeof(ScriptableStatusConsumable),
        typeof(ScriptableTradeItem),
        typeof(ScriptableWeapon),
        typeof(ScriptableItem),
        typeof(ScriptableCondition),
        typeof(CastEffectCollection)
    ];

    private static readonly List<System.Type> AssetTypes =
    [
        typeof(Texture),
        typeof(AudioClip),
        typeof(Sprite),
        typeof(GameObject),
        typeof(Mesh)
    ];

    private static JsonConverter CreateGneericBase(System.Type type)
    {
        var baseConverterType = typeof(BaseConverter<>);
        System.Type[] typeArgs = [type];
        var genericType = baseConverterType.MakeGenericType(typeArgs);
        return (JsonConverter)Activator.CreateInstance(genericType);
    }

    private static JsonConverter CreateAssetConverter(System.Type type)
    {
        var assetConverterType = typeof(AssetConverter<>);
        System.Type[] typeArgs = [type];
        var genericType = assetConverterType.MakeGenericType(typeArgs);
        return (JsonConverter)Activator.CreateInstance(genericType);
    }

    public static JsonSerializerSettings GetSettings(System.Type type)
    {
        // double check Type is a ScriptableObject in the list
        if (!ScriptableTypes.Contains(type) && !AssetTypes.Contains(type)) throw new("Type must be a ScriptableObject");


        if (_converters == null)
        {
            // get the associated manager
            _converters =
            [
                new Vector3Converter(),
                new ColorConverter(),
                new Vector2Converter(),
                new Vector4Converter()
            ];

            foreach (var scriptableType in ScriptableTypes) _converters.Add(CreateGneericBase(scriptableType));

            foreach (var assetType in AssetTypes) _converters.Add(CreateAssetConverter(assetType));

            // double check Type is a ScriptableObject
            if (!type.IsSubclassOf(typeof(ScriptableObject))) throw new("Type must be a ScriptableObject");
        }


        return new()
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            TypeNameHandling = TypeNameHandling.Auto,
            Converters = _converters
        };
    }

    public static object LoadFromJson(string json, System.Type type)
    {
        if (string.IsNullOrEmpty(json)) return null;

        // apply our custom serialization
        var settings = GetSettings(type);
        return JsonConvert.DeserializeObject(json, type, settings);
    }

    public static T LoadFromJson<T>(string json)
    {
        if (string.IsNullOrEmpty(json)) return default;

        // apply our custom serialization
        var settings = GetSettings(typeof(T));
        var returnV = JsonConvert.DeserializeObject<T>(json, settings);
        return returnV;
    }

    public static T ReadFromFile<T>(string path) where T : ScriptableObject
    {
        if (string.IsNullOrEmpty(path))
        {
            Plugin.Logger.LogError("Path is null or empty");
            return null;
        }

        if (!File.Exists(path))
        {
            Plugin.Logger.LogError($"File {path} does not exist");
            return null;
        }

        currentFilePath = path;

        var json = File.ReadAllText(path);
        return LoadFromJson<T>(json);
    }
}
