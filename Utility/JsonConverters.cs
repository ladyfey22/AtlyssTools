using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AtlyssTools.Registries;
using Newtonsoft.Json;
using UnityEngine;

namespace AtlyssTools.Utility;

public class AssetConverter<T> : JsonConverter<T> where T : UnityEngine.Object
{
    public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
    {
        // write the name of the object
        writer.WriteValue(value.name);
    }

    public override T ReadJson(JsonReader reader, System.Type objectType, T existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        string name = (reader!.Value?.ToString());

        T returnV = AtlyssToolsLoader.LoadAsset<T>(name);


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
            float x = (float)reader.Value;
            reader.Read();
            float y = (float)reader.Value;
            reader.Read();
            float z = (float)reader.Value;
            return new Vector3(x, y, z);
        }
        else
        {
            return Vector3.zero;
        }
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
            float r = (float)((double)reader.Value);
            reader.Read();
            float g = (float)((double)reader.Value);
            reader.Read();
            float b = (float)((double)reader.Value);
            reader.Read();
            float a = (float)((double)reader.Value);
            // end array
            reader.Read();
            return new Color(r, g, b, a);
        }
        else
        {
            return Color.white;
        }
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
        T obj = ScriptableObject.CreateInstance<T>();
        serializer.Populate(reader, obj);
        return obj;
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
        float x = (float)reader.Value;
        reader.Read();
        float y = (float)reader.Value;
        return new Vector2(x, y);
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
        float x = (float)reader.Value;
        reader.Read();
        float y = (float)reader.Value;
        reader.Read();
        float z = (float)reader.Value;
        reader.Read();
        float w = (float)reader.Value;
        return new Vector4(x, y, z, w);
    }
}

public class JsonUtility
{
    public static JsonSerializerSettings GetSettings(System.Type type)
    {
        // get the associated manager
        List<JsonConverter> converters = new List<JsonConverter>()
        {
            // all the scriptable object converters
            new AssetConverter<ScriptableArmorRender>(),
            new AssetConverter<ScriptableCombatElement>(),
            new AssetConverter<ScriptableCreep>(),
            new AssetConverter<ScriptableDialogData>(),
            new AssetConverter<ScriptableEmoteList>(),
            new AssetConverter<ScriptableLootTable>(),
            new AssetConverter<ScriptableMapData>(),
            new AssetConverter<ScriptablePlayerBaseClass>(),
            new AssetConverter<ScriptablePlayerRace>(),
            new AssetConverter<ScriptablePolymorphCondition>(),
            new AssetConverter<ScriptableQuest>(),
            new AssetConverter<ScriptableSceneTransferCondition>(),
            new AssetConverter<ScriptableShopkeep>(),
            new AssetConverter<ScriptableSkill>(),
            new AssetConverter<ScriptableStatAttribute>(),
            new AssetConverter<ScriptableStatModifier>(),
            new AssetConverter<ScriptableStatModifierTable>(),
            new AssetConverter<ScriptableStatusCondition>(),
            new AssetConverter<ScriptableWeaponProjectileSet>(),
            new AssetConverter<ScriptableWeaponType>(),
            new AssetConverter<CastEffectCollection>(),


            // items
            new AssetConverter<ScriptableChestpiece>(),
            new AssetConverter<ScriptableArmor>(),
            new AssetConverter<ScriptableArmorDye>(),
            new AssetConverter<ScriptableCape>(),
            new AssetConverter<ScriptableClassTome>(),
            new AssetConverter<ScriptableHelm>(),
            new AssetConverter<ScriptableLeggings>(),
            new AssetConverter<ScriptableRing>(),
            new AssetConverter<ScriptableShield>(),
            new AssetConverter<ScriptableSkillScroll>(),
            new AssetConverter<ScriptableStatusConsumable>(),
            new AssetConverter<ScriptableTradeItem>(),
            new AssetConverter<ScriptableWeapon>(),
            new AssetConverter<ScriptableItem>(),

            new AssetConverter<ScriptableCondition>(),

            //
            new AssetConverter<CastEffectCollection>(),

            // custom converter
            new AssetConverter<Texture>(),
            new AssetConverter<AudioClip>(),
            new AssetConverter<Sprite>(),
            new Vector3Converter(),
            new AssetConverter<GameObject>(),
            new ColorConverter(),
            new Vector2Converter(),
            new Vector4Converter(),
            new AssetConverter<Mesh>(),
        };

        converters.RemoveAll(c =>
            (c.GetType().GetGenericArguments().Length > 0) && (c.GetType().GetGenericArguments()[0] == type ||
                                                               type.IsSubclassOf(
                                                                   c.GetType().GetGenericArguments()[0])));

        // double check Type is a ScriptableObject
        if (!type.IsSubclassOf(typeof(ScriptableObject)))
        {
            throw new Exception("Type must be a ScriptableObject");
        }


        // we have to do this through ugly reflection

        // add the base converter
        System.Type baseConverterType = typeof(BaseConverter<>);
        System.Type[] typeArgs = [type];
        System.Type genericType = baseConverterType.MakeGenericType(typeArgs);
        converters.Add((JsonConverter)Activator.CreateInstance(genericType));


        return new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            TypeNameHandling = TypeNameHandling.Auto,
            Converters = converters
        };
    }

    public static object LoadFromJson(string json, System.Type type)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        // apply our custom serialization
        JsonSerializerSettings settings = GetSettings(type);

        return JsonConvert.DeserializeObject(json, type, settings);
    }

    public static T LoadFromJson<T>(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return default;
        }

        // apply our custom serialization
        JsonSerializerSettings settings = GetSettings(typeof(T));
        return JsonConvert.DeserializeObject<T>(json, settings);
    }

    public static T ReadFromFile<T>(string path) where T : ScriptableObject
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        if (!File.Exists(path))
        {
            return null;
        }

        string json = File.ReadAllText(path);
        return LoadFromJson<T>(json);
    }
}