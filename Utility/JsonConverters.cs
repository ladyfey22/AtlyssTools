using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AtlyssTools.Registries;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            ScriptablesManager<T> manager = AtlyssToolsLoader.Instance.GetManager(typeof(T)) as ScriptablesManager<T>;
            if (manager == null) throw new("Manager not found");
            JObject jObject = JObject.Load(reader);
            string cachedName = manager.GetJsonName(jObject);
            
            T obj = manager.GetFromCache(cachedName);
            if (obj == null)
            {
                Plugin.Logger.LogDebug($"Failed to load {cachedName} from cache for type {typeof(T)}");
                Plugin.Logger.LogDebug($"Creating new object of type {typeof(T)}");
                obj = ScriptableObject.CreateInstance<T>();
            }

            serializer.Populate(jObject.CreateReader(), obj);
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

// arrays
public class ArrayConverter : JsonConverter
{
    public override bool CanWrite => false;
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        // not implemented :3
    }
    
    
    private int FindItemIndex(IList list, JObject criteria) // we don't know the type of the list, so we can't use generics (well, we could, but it would be a pain)
    {
        if (criteria.ContainsKey("index"))
        {
            int idx = criteria["index"].Value<int>();
            return (idx >= 0 && idx < list.Count) ? idx : -1;
        }

        foreach (var item in list)
        {
            bool match = true;

            foreach (var property in criteria.Properties())
            {
                if (property.Name == "value") continue; // Skip "value", since it's for modification

                // first check if the field exists
                var field = item.GetType().GetField(property.Name);
                
                if(field == null)
                {
                    Plugin.Logger.LogError($"JSON Load Error: Array modification field {property.Name} does not exist for type {item.GetType()}");
                    match = false;
                    break;
                }
                
                var itemValue = field.GetValue(item)?.ToString();
                
                if (itemValue == null || itemValue != property.Value.ToString())
                {
                    match = false;
                    break;
                }
            }

            if (match) return list.IndexOf(item);
        }

        return -1;
    }

    public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.StartArray)
        {
            var elementType = objectType.GetElementType();
            
            if(elementType == null)
                return existingValue;
            
            var list = new List<object>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndArray)
                    break;

                var item = serializer.Deserialize(reader, elementType);
                list.Add(item);
            }

            var array = Array.CreateInstance(elementType, list.Count);
            for (var i = 0; i < list.Count; i++)
            {
                array.SetValue(list[i], i);
            }
            // logging
            return array;
        }
        
        if(reader.TokenType == JsonToken.StartObject)
        {
            JObject jObject = JObject.Load(reader);
            // if there is no existing value, we need to create a new Array
            // if there is an existing value, we need to copy it, modify the copy, and return the copy

            System.Type itemType = objectType.GetElementType();
            if(itemType == null) 
                return existingValue;
            
            var list = new List<object>();
            // copy the existing values
            if (existingValue != null)
            {
                foreach (var item in (Array)existingValue)
                {
                    list.Add(item);
                }
            }
            
            // add, remove, modify
            
            if (jObject.ContainsKey("add") && jObject["add"] is JArray addArray)
            {
                foreach (JObject item in addArray.Children<JObject>())
                {
                    var newItem = serializer.Deserialize(item.CreateReader(), itemType);
                    list.Add(newItem);
                }
            }
            
            if (jObject.ContainsKey("remove") && jObject["remove"] is JArray removeArray)
            {
                HashSet<int> indicesToRemove = new HashSet<int>();
                foreach(JObject condition in removeArray.Children<JObject>())
                {
                    int index = FindItemIndex(list, condition);
                    if (index >= 0)
                        indicesToRemove.Add(index);
                }
                
                // remove in reverse order
                foreach (var index in indicesToRemove.OrderByDescending(x => x))
                {
                    list.RemoveAt(index);
                }
            }
            
            if (jObject.ContainsKey("modify") && jObject["modify"] is JArray modifyArray)
            {
                foreach (JObject modifyObject in modifyArray.Children<JObject>())
                {
                    int index = FindItemIndex(list, modifyObject);
                    if(index >= 0 && modifyObject.ContainsKey("value") && modifyObject["value"] is JObject)
                    {
                        serializer.Populate(modifyObject["value"].CreateReader(), list[index]);
                    }
                    else
                    {
                        Plugin.Logger.LogError("JSON Load Error: Failed to modify item in array of " + itemType);
                    }
                }
            }
         
            // we need to convert the list to an array
            var array = Array.CreateInstance(itemType, list.Count);
            for (var i = 0; i < list.Count; i++)
            {
                array.SetValue(list[i], i);
            }
            
            return array;
        }
        
        
        return existingValue;
    }

    public override bool CanConvert(System.Type objectType)
    {
        return objectType.IsArray;
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

    private static JsonConverter CreateGenericBase(System.Type type)
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
        if (!ScriptableTypes.Contains(type) && !AssetTypes.Contains(type)) 
            throw new("Type must be a ScriptableObject or Asset");


        if (_converters == null)
        {
            // get the associated manager
            _converters =
            [
                new Vector3Converter(),
                new ColorConverter(),
                new Vector2Converter(),
                new Vector4Converter(),
                new ArrayConverter()
            ];

            foreach (var scriptableType in ScriptableTypes) 
                _converters.Add(CreateGenericBase(scriptableType));

            foreach (var assetType in AssetTypes) 
                _converters.Add(CreateAssetConverter(assetType));
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
