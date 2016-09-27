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
    }
}