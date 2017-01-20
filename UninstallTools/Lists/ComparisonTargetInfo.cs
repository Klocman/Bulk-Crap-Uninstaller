/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Klocman.Localising;
using UninstallTools.Uninstaller;

namespace UninstallTools.Lists
{
    public sealed class ComparisonTargetInfo
    {
        static ComparisonTargetInfo()
        {
            var targets = typeof(ApplicationUninstallerEntry)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.GetCustomAttributes(typeof(ComparisonTargetAttribute), false).Any());

            var boolType = typeof(bool);
            var boolStrings = new[] { true.ToString(), false.ToString() };

            var results = new List<ComparisonTargetInfo>();

            foreach (var propertyInfo in targets)
            {
                string[] possibleStrings = null;
                var targetType = propertyInfo.PropertyType;
                if (targetType == boolType)
                    possibleStrings = boolStrings;
                else if (targetType.IsEnum)
                    possibleStrings = Enum.GetNames(targetType).OrderBy(x => x).ToArray();

                results.Add(new ComparisonTargetInfo(propertyInfo.Name, propertyInfo.GetLocalisedName(),
                    entry => propertyInfo.GetValue(entry, null)?.ToString(), possibleStrings));
            }

            ComparisonTargets = results;

            AllTargetComparison = new ComparisonTargetInfo(null, "All properties", null);
        }

        private ComparisonTargetInfo(string id, string displayName, Func<ApplicationUninstallerEntry, string> getter,
            string[] possibleStrings = null)
        {
            Getter = getter;
            PossibleStrings = possibleStrings;
            Id = id;
            DisplayName = displayName;
        }

        internal static ComparisonTargetInfo AllTargetComparison { get; }
        internal static IEnumerable<ComparisonTargetInfo> ComparisonTargets { get; }

        public string Id { get; }
        public string DisplayName { get; }
        public Func<ApplicationUninstallerEntry, string> Getter { get; }

        /// <summary>
        ///     If not null this target has a very limited amount of valid matches.
        ///     It will not be included in an overall search in that case.
        /// </summary>
        public string[] PossibleStrings { get; }
    }
}