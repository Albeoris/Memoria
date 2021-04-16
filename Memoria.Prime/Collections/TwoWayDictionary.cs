using System;
using System.Collections;
using System.Collections.Generic;

namespace Memoria.Prime.Collections
{
    public class TwoWayDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey,TValue>>
    {
        private readonly Dictionary<TKey, TValue> _fwdDictionary;
        private readonly Dictionary<TValue, TKey> _revDictionary;
        private readonly Boolean _allowDublicateValues;

        public TwoWayDictionary(Boolean allowDublicateValues = false)
            : this(null, null)
        {
            _allowDublicateValues = allowDublicateValues;
        }

        public TwoWayDictionary(IEqualityComparer<TKey> keyComparer)
            : this(keyComparer, null)
        {
        }

        public TwoWayDictionary(IEqualityComparer<TValue> valueComparer)
            : this(null, valueComparer)
        {
        }

        public TwoWayDictionary(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
        {
            _fwdDictionary = new Dictionary<TKey, TValue>(keyComparer);
            _revDictionary = new Dictionary<TValue, TKey>(valueComparer);
        }

        public void Add(TKey key, TValue value)
        {
            try
            {
                _fwdDictionary.Add(key, value);
            }
            catch (ArgumentException)
            {
                Log.Error($"Forward: {key}, {value}, ({_fwdDictionary[key]})");
                throw;
            }

            if (!_allowDublicateValues || !_revDictionary.ContainsKey(value))
            {
                _revDictionary.Add(value, key);
            }
        }

        public void RemoveByKey(TKey key)
        {
            TValue value;
            if (TryGetValue(key, out value))
            {
                _fwdDictionary.Remove(key);
                _revDictionary.Remove(value);
            }
        }

        public void RemoveByValue(TValue value)
        {
            TKey key;
            if (TryGetKey(value, out key))
            {
                _fwdDictionary.Remove(key);
                _revDictionary.Remove(value);
            }
        }

        public Boolean TryGetValue(TKey key, out TValue value)
        {
            return _fwdDictionary.TryGetValue(key, out value);
        }

        public Boolean TryGetKey(TValue value, out TKey key)
        {
            return _revDictionary.TryGetValue(value, out key);
        }

        public TValue GetValue(TKey key)
        {
            return _fwdDictionary[key];
        }

        public TKey GetKey(TValue value)
        {
            return _revDictionary[value];
        }

        public Boolean ContainsValue(TValue value)
        {
            return _revDictionary.ContainsKey(value);
        }

        public Boolean ContainsKey(TKey key)
        {
            return _fwdDictionary.ContainsKey(key);
        }

        public void Clear()
        {
            _fwdDictionary.Clear();
            _revDictionary.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _fwdDictionary.GetEnumerator();
        }
        public TValue this[TKey key] {
            get
            {
                return _fwdDictionary[key];
            }
            set
			{
                _fwdDictionary[key] = value;
                if (!_allowDublicateValues || !_revDictionary.ContainsKey(value))
                {
                    _revDictionary[value] = key;
                }
            }
        }
    }
}
