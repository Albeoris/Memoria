using System;
using System.Collections;
using System.Collections.Generic;

namespace Memoria.Prime.Collections
{
    public static class EntryCollection
    {
        public static EntryCollection<TValue> CreateWithDefaultElement<TValue>(Int32 count) where TValue : class, new()
        {
            TValue minValue = new TValue();

            Dictionary<Int32, TValue> dic = new Dictionary<Int32, TValue>(count);
            if (count > 0)
            {
                dic.Add(0, minValue);
                for (Int32 i = 1; i < count; i++)
                    dic.Add(i, new TValue());
            }
            
            return new EntryCollection<TValue>(dic, minValue);
        }

        public static EntryCollection<TValue> CreateWithDefaultElement<TValue>(ICollection<TValue> collection) where TValue : class
        {
            Int32 minKey = Int32.MaxValue;
            TValue minValue = null;

            Int32 key = 0;
            Dictionary<Int32, TValue> dic = new Dictionary<Int32, TValue>(collection.Count);
            foreach (TValue item in collection)
            {
                if (minValue == null)
                    minValue = item;

                AddItemSafe(key++, item, dic, ref minKey, ref minValue);
            }

            return new EntryCollection<TValue>(dic, minValue);
        }

        public static EntryCollection<TValue> CreateWithDefaultElement<TValue>(ICollection<TValue> collection, Func<TValue, Int32> keySelector) where TValue : class
        {
            Int32 minKey = Int32.MaxValue;
            TValue minValue = null;

            Dictionary<Int32, TValue> dic = new Dictionary<Int32, TValue>(collection.Count);
            foreach (TValue item in collection)
            {
                Int32 key = keySelector(item);
                AddItemSafe(key, item, dic, ref minKey, ref minValue);
            }

            return new EntryCollection<TValue>(dic, minValue);
        }

        public static EntryCollection<TValue> CreateWithDefaultElement<TSource, TValue>(ICollection<TSource> collection, Func<TSource, Int32> keySelector, Func<TSource, TValue> valueSelector) where TValue : class
        {
            Int32 minKey = Int32.MaxValue;
            TValue minValue = null;

            Dictionary<Int32, TValue> dic = new Dictionary<Int32, TValue>(collection.Count);
            foreach (TSource source in collection)
            {
                Int32 key = keySelector(source);
                TValue item = valueSelector(source);
                AddItemSafe<TValue>(key, item, dic, ref minKey, ref minValue);
            }

            return new EntryCollection<TValue>(dic, minValue);
        }

        private static void AddItemSafe<TValue>(Int32 key, TValue item, Dictionary<Int32, TValue> dic, ref Int32 minKey, ref TValue minValue) where TValue : class
        {
            if (key < minKey)
            {
                minKey = key;
                minValue = item;
            }

            try
            {
                dic.Add(key, item);
            }
            catch (ArgumentException ex)
            {
                Log.Error(ex, "Cannot add {0} with key {1} to the entry collection because an element with the same key already exists.", item, key);
            }
        }
    }

    public sealed class EntryCollection<TValue> : IEnumerable<TValue> where TValue : class
    {
        private readonly Dictionary<Int32, TValue> _dic;
        private readonly TValue _defaultElement;

        public EntryCollection(Dictionary<Int32, TValue> dic, TValue defaultValue)
        {
            _dic = dic;
            _defaultElement = defaultValue;
        }

        public Boolean Contains(UInt32 key) => _dic.ContainsKey((Int32)key);
        public Boolean Contains(Int32 key) => _dic.ContainsKey(key);

        public Boolean TryGet(UInt32 key, out TValue value) => _dic.TryGetValue((Int32)key, out value);
        public Boolean TryGet(Int32 key, out TValue value) => _dic.TryGetValue(key, out value);

        public TValue this[UInt32 key] => this[(Int32)key];
        public TValue this[Int32 key]
        {
            get
            {
                TValue value;
                if (_dic.TryGetValue(key, out value))
                    return value;

                if (_defaultElement != null)
                {
                    Log.Warning("There is no element of type {0} with the key {1}.", TypeCache<TValue>.Type.FullName, key);
                    return _defaultElement;
                }

                throw new ArgumentException($"There is no element of type {TypeCache<TValue>.Type.FullName} with the key {key}.");
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _dic.Values.GetEnumerator();
        }
    }
}