using System;

namespace AtlyssTools.Registries;

[AttributeUsage(AttributeTargets.Class)]
public class ManagerAttribute : Attribute
{
    // just tag the class with this attribute to have it automatically registered
}