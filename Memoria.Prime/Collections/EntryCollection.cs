using System;
using System.Collections.Generic;

namespace Memoria.Prime.Collections
{
    public class EntryCollection<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _dic;
        private readonly TValue _defaultElement;

        public EntryCollection(TValue defaultValue)
        {
            _dic = new Dictionary<TKey, TValue>();
            _defaultElement = defaultValue;
        }

        public Boolean Contains(TKey key) => _dic.ContainsKey(key);
        public Boolean TryGet(TKey key, out TValue value) => _dic.TryGetValue(key, out value);
        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (_dic.TryGetValue(key, out value))
                    return value;

                return _defaultElement;
            }
            set => _dic[key] = value;
        }
    }
}