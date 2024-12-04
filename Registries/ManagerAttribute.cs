using System;

namespace AtlyssTools.Registries;

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class ManagerAttribute : System.Attribute
{
    // just tag the class with this attribute to have it automatically registered
}