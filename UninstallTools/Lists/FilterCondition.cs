/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Klocman.Extensions;
using UninstallTools.Properties;
using UninstallTools.Uninstaller;

namespace UninstallTools.Lists
{
    public sealed class FilterCondition : ITestEntry, ICloneable
    {
        public FilterCondition() : this(null)
        {
        }

        public FilterCondition(string filterText) : this (filterText, ComparisonMethod.Any, null)
        {
        }

        public FilterCondition(string filterText, ComparisonMethod comparisonMethod, string targetPropertyName)
        {
            if (string.IsNullOrEmpty(targetPropertyName))
                TargetProperty = ComparisonTargetInfo.AllTargetComparison;
            else
                TargetPropertyId = targetPropertyName;

            FilterText = filterText ?? Localisation.UninstallListEditor_NewCondition;
            ComparisonMethod = comparisonMethod;
        }
        
        public object Clone()
        {
            return new FilterCondition (FilterText, ComparisonMethod, null)
            {
                TargetProperty = TargetProperty,
            };
        }

        /// <summary>
        ///     Negate results of TestString, null is not affected.
        /// </summary>
        public bool InvertResults { get; set; }

        public ComparisonMethod ComparisonMethod { get; set; }
        public string FilterText { get; set; }

        public string TargetPropertyId
        {
            get { return TargetProperty?.Id ?? string.Empty; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    TargetProperty = ComparisonTargetInfo.AllTargetComparison;
                }
                else
                {
                    TargetProperty = ComparisonTargetInfo.ComparisonTargets
                        .FirstOrDefault(x => x.Id.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                                     ?? ComparisonTargetInfo.AllTargetComparison;
                }
            }
        }

        private ComparisonTargetInfo TargetProperty { get; set; }

        /// <summary>
        ///     Test if the input matches this condition. Returns null if it is impossible to determine.
        /// </summary>
        public bool? TestEntry(ApplicationUninstallerEntry input)
        {
            if (!Enabled) return null;

            var targets = ReferenceEquals(TargetProperty, ComparisonTargetInfo.AllTargetComparison)
                ? ComparisonTargetInfo.ComparisonTargets.Where(x => x.PossibleStrings == null)
                    .Select(x => x.Getter(input))
                : new[] {TargetProperty.Getter(input)};

            bool? result = null;

            foreach (var target in targets.Where(target => !string.IsNullOrEmpty(target)))
            {
                try
                {
                    switch (ComparisonMethod)
                    {
                        case ComparisonMethod.Equals:
                            result = target.Equals(FilterText, StringComparison.InvariantCultureIgnoreCase);
                            break;

                        case ComparisonMethod.Any:
                            result = target.ContainsAny(
                                FilterText.Split((char[]) null, StringSplitOptions.RemoveEmptyEntries),
                                StringComparison.InvariantCultureIgnoreCase);
                            break;

                        case ComparisonMethod.StartsWith:
                            result = target.StartsWith(FilterText, StringComparison.InvariantCultureIgnoreCase);
                            break;

                        case ComparisonMethod.EndsWith:
                            result = target.EndsWith(FilterText, StringComparison.InvariantCultureIgnoreCase);
                            break;

                        case ComparisonMethod.Contains:
                            result = target.Contains(FilterText, StringComparison.InvariantCultureIgnoreCase);
                            break;

                        case ComparisonMethod.Regex:
                            result = Regex.IsMatch(target, FilterText, RegexOptions.CultureInvariant);
                            break;

                        default:
                            throw new InvalidOperationException("Unknown FilterComparisonMethod");
                    }
                }
                catch (InvalidOperationException)
                {
                    throw;
                }
                catch
                {
                    //result = null;
                }

                if (result == true)
                    return !InvertResults;
            }

            if (!result.HasValue) return null;
            return InvertResults ? !result.Value : result.Value;
        }

        public bool Enabled { get; set; } = true;

        public override string ToString()
        {
            return $"{FilterText}"; // | {ComparisonMethod.GetLocalisedName()} {TargetProperty.DisplayName}";
        }
    }
}