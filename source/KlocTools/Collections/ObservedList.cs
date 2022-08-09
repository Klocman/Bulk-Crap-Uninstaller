/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Klocman.Collections
{
    /// <summary>
    ///     Generic list with an ListChanged event that fires whenever items get added or removed (not modified).
    /// </summary>
    //[Serializable]
    public class ObservedList<T> : IList<T>
    {
        private readonly List<T> _itemList = new();
        public virtual int Count => _itemList.Count;
        public virtual bool IsReadOnly => false;

        public virtual T this[int index]
        {
            get { return _itemList[index]; }
            set { _itemList[index] = value; }
        }

        public virtual void Add(T item)
        {
            OnListChangedEvent();
            _itemList.Add(item);
        }

        public virtual void Clear()
        {
            OnListChangedEvent();
            _itemList.Clear();
        }

        public virtual bool Contains(T item)
        {
            return _itemList.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            _itemList.CopyTo(array, arrayIndex);
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return _itemList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual int IndexOf(T item)
        {
            return _itemList.IndexOf(item);
        }

        public virtual void Insert(int index, T item)
        {
            OnListChangedEvent();
            _itemList.Insert(index, item);
        }

        public virtual bool Remove(T item)
        {
            OnListChangedEvent();
            return _itemList.Remove(item);
        }

        public virtual void RemoveAt(int index)
        {
            OnListChangedEvent();
            _itemList.RemoveAt(index);
        }

        public event Action ListChanged;

        public void AddRange(IEnumerable<T> items)
        {
            var enumerable = items as IList<T> ?? items.ToList();
            if (!enumerable.Any())
                return;

            foreach (var item in enumerable)
                _itemList.Add(item);

            try
            {
                OnListChangedEvent();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <exception cref="Exception">A delegate callback throws an exception. </exception>
        public void OnListChangedEvent()
        {
            var handler = ListChanged;
            handler?.Invoke();
        }
    }
}