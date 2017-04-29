using System;
using System.Linq;
using Memoria.Prime;
using UnityEngine;

namespace Memoria.Scenes
{
    internal static class ExtensionMethodsGameObject
    {
        public static T GetExactComponent<T>(this GameObject obj) where T : Component
        {
            return obj.GetComponents<T>().Single(c => c.GetType() == TypeCache<T>.Type);
        }

        public static T GetExactComponent<T>(this GameObject obj, Int32 index) where T : Component
        {
            return obj.GetComponents<T>().First(c => c.GetType() == TypeCache<T>.Type && --index < 0);
        }

        public static T[] GetExactComponents<T>(this GameObject obj) where T : Component
        {
            return obj.GetComponents<T>().Where(c => c.GetType() == TypeCache<T>.Type).ToArray();
        }

        public static T EnsureExactComponent<T>(this GameObject obj) where T : Component
        {
            T[] result = obj.GetComponents<T>();
            if (result.Length == 0)
                return obj.AddComponent<T>();

            T component = result.SingleOrDefault(c => c.GetType() == TypeCache<T>.Type);
            if (component != null)
                return component;

            return obj.AddComponent<T>();
        }
    }
}