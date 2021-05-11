/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace Klocman.Extensions
{
    public sealed class ReadOnlyDictionaryWrapper<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _baseDictionary;

        public ReadOnlyDictionaryWrapper(IDictionary<TKey, TValue> baseDictionary)
        {
            _baseDictionary = baseDictionary;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _baseDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new InvalidOperationException();
        }

        public void Clear()
        {
            throw new InvalidOperationException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _baseDictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _baseDictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new InvalidOperationException();
        }

        public int Count => _baseDictionary.Count;
        public bool IsReadOnly => true;
        public bool ContainsKey(TKey key)
        {
            return _baseDictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            throw new InvalidOperationException();
        }

        public bool Remove(TKey key)
        {
            throw new InvalidOperationException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _baseDictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get { return _baseDictionary[key]; }
            set { throw new InvalidOperationException(); }
        }

        public ICollection<TKey> Keys => _baseDictionary.Keys;
        public ICollection<TValue> Values => _baseDictionary.Values;
    }
}