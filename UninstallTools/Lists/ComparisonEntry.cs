using System;
using System.Linq;
using System.Text.RegularExpressions;
using Klocman.Extensions;
using Klocman.Localising;
using UninstallTools.Uninstaller;

namespace UninstallTools.Lists
{
    public sealed class ComparisonEntry
    {
        public ComparisonEntry()
        {
            TargetProperty = ComparisonTargetInfo.AllTargetComparison;
            ComparisonMethod = FilterComparisonMethod.Any;
        }

        /// <summary>
        ///     Negate results of TestString, null is not affected.
        /// </summary>
        public bool InvertResults { get; set; }

        public FilterComparisonMethod ComparisonMethod { get; set; }
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
            var targets = ReferenceEquals(TargetProperty, ComparisonTargetInfo.AllTargetComparison) 
                ? ComparisonTargetInfo.ComparisonTargets.Where(x=>x.PossibleStrings == null).Select(x=>x.Getter(input))
                : new [] { TargetProperty.Getter(input) };

            bool? result = null;

            foreach (var target in targets.Where(target => !string.IsNullOrEmpty(target)))
            {
                try
                {
                    switch (ComparisonMethod)
                    {
                        case FilterComparisonMethod.Equals:
                            result = target.Equals(FilterText, StringComparison.InvariantCultureIgnoreCase);
                            break;

                        case FilterComparisonMethod.Any:
                            result = target.ContainsAny(
                                FilterText.Split((char[])null, StringSplitOptions.RemoveEmptyEntries),
                                StringComparison.InvariantCultureIgnoreCase);
                            break;

                        case FilterComparisonMethod.StartsWith:
                            result = target.StartsWith(FilterText, StringComparison.InvariantCultureIgnoreCase);
                            break;

                        case FilterComparisonMethod.EndsWith:
                            result = target.EndsWith(FilterText, StringComparison.InvariantCultureIgnoreCase);
                            break;

                        case FilterComparisonMethod.Contains:
                            result = target.Contains(FilterText, StringComparison.InvariantCultureIgnoreCase);
                            break;

                        case FilterComparisonMethod.Regex:
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

        public override string ToString()
        {
            return $"{FilterText} {ComparisonMethod.GetLocalisedName()} {TargetProperty.DisplayName}";
        }
    }
}