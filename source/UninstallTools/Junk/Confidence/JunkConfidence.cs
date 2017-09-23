/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.Linq;

namespace UninstallTools.Junk
{
    public sealed class JunkConfidence
    {
        private readonly List<ConfidencePart> _items = new List<ConfidencePart>();

        internal JunkConfidence()
        {
        }

        public bool IsEmpty => _items.Count == 0;

        public IEnumerable<ConfidencePart> ConfidenceParts => _items;

        public Confidence GetConfidence()
        {
            if (_items.Count < 1)
                return Confidence.Unknown;

            var result = GetRawConfidence();

            if (result < 0)
                return Confidence.Bad;
            if (result < 2)
                return Confidence.Questionable;
            if (result < 5)
                return Confidence.Good;
            return Confidence.VeryGood;
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
            _items.Add(new ConfidencePart(value));
        }

        internal void Add(ConfidencePart value)
        {
            _items.Add(value);
        }

        internal void AddRange(IEnumerable<ConfidencePart> values)
        {
            _items.AddRange(values.Where(x => !_items.Contains(x)));
        }
    }
}