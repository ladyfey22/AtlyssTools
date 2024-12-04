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
    }

    public override T ReadJson(JsonReader reader, System.Type objectType, T existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        return AtlyssToolsLoader.LoadAsset<T>(reader!.Value?.ToString());
    }
}

public class GameObjectConverter : JsonConverter<GameObject>
{
    public override void WriteJson(JsonWriter writer, GameObject value, JsonSerializer serializer)
    {
    }

    public override GameObject ReadJson(JsonReader reader, System.Type objectType, GameObject existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        return AtlyssToolsLoader.LoadAsset<GameObject>(reader!.Value?.ToString()); // prefab
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

public class ScriptableStatusConditionBaseConverter : JsonConverter<ScriptableStatusCondition>
{
    public override bool CanWrite =>
        false; // we don't need to read, only write. We can use the default serialization for reading
    
    public override void WriteJson(JsonWriter writer, ScriptableStatusCondition value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override ScriptableStatusCondition ReadJson(JsonReader reader, System.Type objectType,
        ScriptableStatusCondition existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        ScriptableStatusCondition condition = ScriptableObject.CreateInstance<ScriptableStatusCondition>();

        // we can use newtonsoft's populate method to fill in the values
        serializer.Populate(reader, condition);

        return condition;
    }
}

public class ScriptableSkillBaseConverter : JsonConverter<ScriptableSkill>
{
    public override bool CanWrite =>
        false; // we don't need to read, only write. We can use the default serialization for reading

    public override void WriteJson(JsonWriter writer, ScriptableSkill value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override ScriptableSkill ReadJson(JsonReader reader, System.Type objectType, ScriptableSkill existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        ScriptableSkill skill = ScriptableObject.CreateInstance<ScriptableSkill>();

        // we can use newtonsoft's populate method to fill in the values
        serializer.Populate(reader, skill);

        return skill;
    }
}

public class ScriptableClassBaseConverter : JsonConverter<ScriptablePlayerBaseClass>
{
    public override bool CanWrite =>
        false; // we don't need to read, only write. We can use the default serialization for reading

    public override void WriteJson(JsonWriter writer, ScriptablePlayerBaseClass value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override ScriptablePlayerBaseClass ReadJson(JsonReader reader, System.Type objectType,
        ScriptablePlayerBaseClass existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        ScriptablePlayerBaseClass skill = ScriptableObject.CreateInstance<ScriptablePlayerBaseClass>();

        // we can use newtonsoft's populate method to fill in the values
        serializer.Populate(reader, skill);

        return skill;
    }
}

public class ScriptableConditionConverter : JsonConverter<ScriptableCondition>
{
    public override void WriteJson(JsonWriter writer, ScriptableCondition value, JsonSerializer serializer)
    {
    }

    public override ScriptableCondition ReadJson(JsonReader reader, System.Type objectType,
        ScriptableCondition existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        ScriptableCondition condition =
            AtlyssToolsLoader.LoadAsset<ScriptableStatusCondition>(reader!.Value?.ToString());
        return condition;
    }
}

public class ScriptableItemBaseConverter : JsonConverter<ScriptableItem>
{
    public override void WriteJson(JsonWriter writer, ScriptableItem value, JsonSerializer serializer)
    {
    }

    public override ScriptableItem ReadJson(JsonReader reader, System.Type objectType, ScriptableItem existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        
        // we can use newtonsoft's populate method to fill in the values
        serializer.Populate(reader, item);
        
        return item;
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

    public override Vector2 ReadJson(JsonReader reader, System.Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        // read in 2 separate values
        reader.Read();
        float x = (float)reader.Value;
        reader.Read();
        float y = (float)reader.Value;
        return new Vector2(x, y);
    }
}



public class JsonUtility
{
    public static JsonSerializerSettings GetSettings(System.Type type)
    {
        // get the associated manager
        BaseScriptablesManager manager = AtlyssToolsLoader.Instance.GetManager(type);
        
        if(manager != null)
        {
            return manager.GetJsonSettings();
        }
        throw new Exception($"No manager found for type {type}");
    }

    public static T LoadFromJson<T>(string json)
    {
        // apply our custom serialization
        JsonSerializerSettings settings = GetSettings(typeof(T));
        return JsonConvert.DeserializeObject<T>(json, settings);
    }

    public static T ReadFromFile<T>(string path)
    {
        string json = File.ReadAllText(path);
        return LoadFromJson<T>(json);
    }

    public static object LoadFromJson(string json, System.Type type)
    {
        JsonSerializerSettings settings = GetSettings(type);
        return JsonConvert.DeserializeObject(json, type, settings);
    }

    public static object ReadFromFile(string path, System.Type type)
    {
        string json = File.ReadAllText(path);
        Plugin.Logger.LogInfo($"Reading {type.Name} from {path}");
        return LoadFromJson(json, type);
    }
}