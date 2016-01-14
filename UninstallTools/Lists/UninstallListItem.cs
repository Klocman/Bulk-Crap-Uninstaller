using System;
using System.Collections.Generic;
using System.Linq;
using Klocman.Extensions;
using Klocman.Tools;
using UninstallTools.Properties;
using UninstallTools.Uninstaller;

namespace UninstallTools.Lists
{
    public class UninstallListItem
    {
        public UninstallListItem()
        {
            ComparisonEntries.Add(new ComparisonEntry { FilterText = Localisation.UninstallListEditor_NewFilter });
        }

        public UninstallListItem(string filterText)
        {
            if (string.IsNullOrEmpty(filterText))
                throw new ArgumentException(Localisation.UninstallListItem_ValueEmpty, nameof(filterText));

            if (filterText.ContainsAny(StringTools.NewLineChars, StringComparison.Ordinal))
                throw new ArgumentException(Localisation.UninstallListItem_NewLineInValue, nameof(filterText));

            ComparisonEntries.Add(new ComparisonEntry { FilterText = filterText });
        }

        /// <summary>
        ///     Comparison rules of this filter
        /// </summary>
        public List<ComparisonEntry> ComparisonEntries { get; set; } = new List<ComparisonEntry>();


        /// <summary>
        ///     Test if the input matches this filter. Returns null if it is impossible to determine.
        /// </summary>
        public bool? TestEntry(ApplicationUninstallerEntry input)
        {
            if (input == null) return null;

            var filteredCompEntries = ComparisonEntries.Where(x => !string.IsNullOrEmpty(x.FilterText)).ToList();
            if (filteredCompEntries.Count < 1) return null;

            var tests = filteredCompEntries.Select(x => x.TestEntry(input)).Where(x => x.HasValue)
                .Select(x => x.Value).ToList();
            if (tests.Count < 1) return null;

            return tests.All(x => x);
        }

        public override string ToString()
        {
            if (ComparisonEntries.Count < 1)
                return "Empty filter";
            return ComparisonEntries.Count == 1
                ? ComparisonEntries.First().ToString()
                : $"Filter with {ComparisonEntries.Count} conditions";
        }
    }
}