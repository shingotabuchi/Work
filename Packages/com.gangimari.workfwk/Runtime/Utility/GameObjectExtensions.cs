using UnityEngine;
using System;
using System.Reflection;

public static class GameObjectExtensions
{
    public static void SetActiveFast(this GameObject self, bool active)
    {
        if (self.activeSelf != active)
        {
            self.gameObject.SetActive(active);
        }
    }

    public static T CloneComponent<T>(this GameObject destination, T original) where T : Component
    {
        if (original == null || destination == null)
            throw new ArgumentNullException("Neither original nor destination can be null.");

        // 1) Add the same type of component
        var type = original.GetType();
        var copy = destination.AddComponent(type);

        // 2) Copy all serializable fields
        var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        foreach (var field in type.GetFields(flags))
        {
            // public or marked [SerializeField]
            if (field.IsPublic || Attribute.IsDefined(field, typeof(SerializeField)))
                field.SetValue(copy, field.GetValue(original));
        }

        return copy as T;
    }

    public static Component CloneComponent(this GameObject destination, Component original)
    {
        return destination.CloneComponent<Component>(original);
    }
}