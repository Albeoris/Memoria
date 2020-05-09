using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Memoria.Prime
{
    public static class ExtensionMethodsIList
    {
        public static void InitializeElements<T>(this IList<T> list) where T : new()
        {
            for (Int32 i = 0; i < list.Count; i++)
                list[i] = new T();
        }

        public static Boolean IsNullOrEmpty<T>(this IList<T> list)
        {
            return list == null || list.Count == 0;
        }

        public static void Replace<T>(this IList<T> list, Int32 index, T item, T defaultValue = default(T))
        {
            while (list.Count < index)
                list.Add(defaultValue);

            if (index == list.Count)
                list.Add(item);
            else
                list[index] = item;
        }

        public static Int32 Insert<T>(this IList<T> list, T item, IComparer<T> comparer = null)
        {
            if (comparer == null)
                comparer = Comparer<T>.Default;

            Int32 i = 0;
            while (i < list.Count && comparer.Compare(list[i], item) < 0)
                i++;

            list.Insert(i, item);
            return i;
        }

        public static void Swap<T>(this IList<T> list, Int32 firstIndex, Int32 secondIndex)
        {
            T tmp = list[firstIndex];
            list[firstIndex] = list[secondIndex];
            list[secondIndex] = tmp;
        }

        public static T GetOrCreate<T>(this IList<T> list, Int32 index, Func<Int32, T> fabric) where T : class
        {
            T result = list[index];
            if (result == null)
            {
                result = fabric(index);
                list[index] = result;
            }
            return result;
        }

        private static readonly System.Random s_random = new System.Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int n = 0; n < 3; n++)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    Int32 k = s_random.Next(0, list.Count);
                    T value = list[k];
                    list[k] = list[i];
                    list[i] = value;
                }
            }
        }
    }
}