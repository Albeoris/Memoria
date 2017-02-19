using System;
using System.Linq;
using UnityEngine;

namespace Memoria.Prime
{
    internal static class ExtensionMethodsGameObject
    {
        public static T GetExactComponent<T>(this GameObject obj)
        {
            return obj.GetComponents<T>().Single(c => c.GetType() == TypeCache<T>.Type);
        }

        public static T GetExactComponent<T>(this GameObject obj, Int32 index)
        {
            return obj.GetComponents<T>().First(c => c.GetType() == TypeCache<T>.Type && --index < 0);
        }

        public static T[] GetExactComponents<T>(this GameObject obj)
        {
            return obj.GetComponents<T>().Where(c => c.GetType() == TypeCache<T>.Type).ToArray();
        }
    }
}