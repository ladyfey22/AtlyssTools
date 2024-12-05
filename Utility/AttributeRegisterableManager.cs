using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// assemblyinfo
using System.Reflection;
using UnityEngine;

namespace AtlyssTools.Utility;

// this will register attributes that match the type provided

public interface IAttributeRegisterableManager
{
    string Name { get; }
    IDictionary GetRegistered();
    void Register(System.Type type, string modId);
    bool CanRegister(System.Type obj);
    
    // this is the type of attribute we are looking for
    System.Type AttributeType { get; }
}

public class AttributeRegisterableManager<TBase, TAttributeType> : IAttributeRegisterableManager where TAttributeType : Attribute where TBase : class
{
    Dictionary<string, List<TBase>> _registered = new();

    public string Name => typeof(TBase).Name;
    public System.Type AttributeType => typeof(TAttributeType);

    public IDictionary GetRegistered()
    {
        return _registered;
    }
    
    public List<TBase> GetRegisteredList()
    {
        return _registered.Values.SelectMany(x => x).ToList();
    }
    
    public Dictionary<string, List<TBase>> GetRegisteredDict()
    {
        return _registered;
    }
    
    public List<TBase> GetRegistered(string modId)
    {
        if (_registered.TryGetValue(modId, out var registered))
        {
            return registered;
        }
        
        return new();
    }

    public void Register(System.Type objType, string modId)
    {
        if (!CanRegister(objType))
        {
            return;
        }
        
        if(!objType.IsSubclassOf(typeof(TBase)))
        {
            Plugin.Logger.LogError($"Object {objType.Name} is not of type {typeof(TBase).Name}");
            return;
        }
        
        if (!_registered.ContainsKey(modId))
        {
            _registered.Add(modId, []);
        }
        
        var obj = Activator.CreateInstance(objType) as TBase;
        _registered[modId].Add(obj);
    }
    
    public bool CanRegister(System.Type obj)
    {
        // check if the object has the attribute
        return obj.GetCustomAttributes(AttributeType, true).Length > 0;
    }
}