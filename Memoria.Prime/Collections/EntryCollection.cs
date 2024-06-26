using System;
using System.Collections.Generic;

namespace Memoria.Prime.Collections
{
    public class EntryCollection<TKey, TValue> : Dictionary<TKey, TValue>
    {
        private readonly TValue _defaultElement;

        public EntryCollection(TValue defaultValue) : base()
        {
            _defaultElement = defaultValue;
        }

        new public TValue this[TKey key]
        {
            get => TryGetValue(key, out TValue value) ? value : _defaultElement;
            set => base[key] = value;
        }
    }
}