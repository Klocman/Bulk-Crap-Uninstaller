/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UninstallTools.Junk.Confidence
{
    public sealed class ConfidenceCollection : IEnumerable<ConfidenceRecord>
    {
        private readonly List<ConfidenceRecord> _items = new();

        internal ConfidenceCollection()
        {
        }

        public bool IsEmpty => _items.Count == 0;

        public IEnumerable<ConfidenceRecord> ConfidenceParts => _items;

        public ConfidenceLevel GetConfidence()
        {
            if (_items.Count < 1)
                return ConfidenceLevel.Unknown;

            var result = GetRawConfidence();

            if (result < 0)
                return ConfidenceLevel.Bad;
            if (result < 2)
                return ConfidenceLevel.Questionable;
            if (result < 5)
                return ConfidenceLevel.Good;
            return ConfidenceLevel.VeryGood;
        }

        // Returns a number representing the confidence. 0 is a mid-point.
        public int GetRawConfidence()
        {
            var result = 0;
            _items.ForEach(x => result += x.Change);
            return result;
        }

        public string GetWorstOffender()
        {
            var lowest = _items.Min(x => x.Change);
            var item = _items.FirstOrDefault(x => x.Change.Equals(lowest));

            if (item != null) return item.Reason ?? string.Empty;
            return string.Empty;
        }

        internal void Add(int value)
        {
            _items.Add(new ConfidenceRecord(value));
        }

        internal void Add(ConfidenceRecord value)
        {
            _items.Add(value);
        }

        internal void AddRange(IEnumerable<ConfidenceRecord> values)
        {
            _items.AddRange(values.Where(x => !_items.Contains(x)));
        }

        public IEnumerator<ConfidenceRecord> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _items).GetEnumerator();
        }
    }
}