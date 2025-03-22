using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtlyssTools.Utility;

public class CopyTools
{
    private static List<System.Type> ExternalTypes =
    [
        typeof(ScriptableObject),
        typeof(Texture2D),
        typeof(Sprite),
        typeof(Material),
        typeof(Shader),
        typeof(AudioClip),
        typeof(AnimationClip),
    ];
    
    /// <summary>
    /// Deep copy an object, creating new instances of all internal objects that aren't external (textures, scriptable objects, ect)
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    public static void DeepCopy(object source, object dest)
    {
        var sourceType = source.GetType();
        var destType = dest.GetType();
        if (sourceType != destType)
        {
            throw new System.Exception("Source and destination types must be the same");
        }
        
        // special case for lists and dictionaries. dest list/dictionary must be cleared and re-populated
        if (source is IList sourceList && dest is IList destList)
        {
            destList.Clear();
            foreach (var item in sourceList)
            {
                var newItem = System.Activator.CreateInstance(item.GetType());
                DeepCopy(item, newItem);
                destList.Add(newItem);
            }
            return;
        }
        
        if (source is IDictionary sourceDict && dest is IDictionary destDict)
        {
            destDict.Clear();
            foreach (DictionaryEntry entry in sourceDict)
            {
                var newKey = System.Activator.CreateInstance(entry.Key.GetType());
                var newValue = System.Activator.CreateInstance(entry.Value.GetType());
                DeepCopy(entry.Key, newKey);
                DeepCopy(entry.Value, newValue);
                destDict.Add(newKey, newValue);
            }
            return;
        }
        
        var fields = sourceType.GetFields();
        foreach (var field in fields)
        {
            // if the field type is an external type, or sublass of an external type, or a primitive type, just copy the value
            if (ExternalTypes.Contains(field.FieldType) || ExternalTypes.Exists(t => field.FieldType.IsSubclassOf(t)) || field.FieldType.IsPrimitive)
            {
                field.SetValue(dest, field.GetValue(source));
            }
            else
            {
                var sourceValue = field.GetValue(source);
                var destValue = System.Activator.CreateInstance(field.FieldType);
                DeepCopy(sourceValue, destValue);
                field.SetValue(dest, destValue);
            }
        }
    }
}