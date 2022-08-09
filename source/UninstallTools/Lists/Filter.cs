/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Klocman.Extensions;
using Klocman.Tools;
using UninstallTools.Properties;

namespace UninstallTools.Lists
{
    public class Filter : ITestEntry
    {
        public Filter()
        {
        }

        public Filter(string name, string filterText)
        {
            if (!string.IsNullOrEmpty(name))
                Name = name;

            if (string.IsNullOrEmpty(filterText))
                throw new ArgumentException(Localisation.UninstallListItem_ValueEmpty, nameof(filterText));

            if (filterText.ContainsAny(StringTools.NewLineChars, StringComparison.Ordinal))
                throw new ArgumentException(Localisation.UninstallListItem_NewLineInValue, nameof(filterText));

            ComparisonEntries.Add(new FilterCondition { FilterText = filterText });
        }

        public Filter(string name, bool exclude, params FilterCondition[] conditions)
        {
            if (!string.IsNullOrEmpty(name))
                Name = name;
            Exclude = exclude;
            ComparisonEntries.AddRange(conditions);
        }

        public string Name { get; set; } = Localisation.UninstallListEditor_NewFilter;

        /// <summary>
        /// Exclude items matched by this entry from results of the parent uninstall list
        /// </summary>
        public bool Exclude { get; set; }

        /// <summary>
        ///     Comparison rules of this filter
        /// </summary>
        public List<FilterCondition> ComparisonEntries { get; set; } = new();


        /// <summary>
        ///     Test if the input matches this filter. Returns null if it is impossible to determine.
        /// </summary>
        public bool? TestEntry(ApplicationUninstallerEntry input)
        {
            if (!Enabled || input == null) return null;

            var filteredCompEntries = ComparisonEntries.Where(x => !string.IsNullOrEmpty(x.FilterText)).ToList();
            if (filteredCompEntries.Count < 1) return null;

            var tests = filteredCompEntries.Select(x => x.TestEntry(input)).Where(x => x.HasValue).Select(x => x.Value).ToList();
            if (tests.Count < 1) return null;

            return tests.All(x => x);
        }

        public bool Enabled { get; set; } = true;

        public override string ToString()
        {
            return $"{Name} | {ExcludeToString(Exclude)}";
        }

        public static string ExcludeToString(bool exclude)
        {
            return exclude ? "Exclude" : "Include";
        }
    }
}