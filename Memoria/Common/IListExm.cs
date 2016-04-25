using System;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace Memoria
{
    public static class IListExm
    {
        public static void InitializeElements<T>(this IList<T> list) where T : new()
        {
            for (int i = 0; i < list.Count; i++)
                list[i] = new T();
        }

        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list == null || list.Count == 0;
        }

        public static void Replace<T>(this IList<T> list, int index, T item, T defaultValue = default(T))
        {
            while (list.Count < index)
                list.Add(defaultValue);

            if (index == list.Count)
                list.Add(item);
            else
                list[index] = item;
        }

        public static int Insert<T>(this IList<T> list, T item, IComparer<T> comparer = null)
        {
            if (comparer == null)
                comparer = Comparer<T>.Default;

            int i = 0;
            while (i < list.Count && comparer.Compare(list[i], item) < 0)
                i++;

            list.Insert(i, item);
            return i;
        }

        public static void Swap<T>(this IList<T> list, int firstIndex, int secondIndex)
        {
            T tmp = list[firstIndex];
            list[firstIndex] = list[secondIndex];
            list[secondIndex] = tmp;
        }

        public static T GetOrCreate<T>(this IList<T> list, int index, Func<int, T> fabric) where T : class
        {
            T result = list[index];
            if (result == null)
            {
                result = fabric(index);
                list[index] = result;
            }
            return result;
        }
    }
}