using System;
using System.Text.RegularExpressions;
using Klocman.Extensions;
using Klocman.Tools;
using UninstallTools.Properties;

namespace UninstallTools.Lists
{
    public class UninstallListItem
    {
        public UninstallListItem()
        {
            ComparisonMethod = FilterComparisonMethod.Equals;
        }

        public UninstallListItem(string filterText)
            : this()
        {
            if (string.IsNullOrEmpty(filterText))
                throw new ArgumentException(Localisation.UninstallListItem_ValueEmpty, nameof(filterText));

            if (filterText.ContainsAny(StringTools.NewLineChars, StringComparison.Ordinal))
                throw new ArgumentException(Localisation.UninstallListItem_NewLineInValue, nameof(filterText));

            FilterText = filterText;
        }

        public FilterComparisonMethod ComparisonMethod { get; set; }
        public string FilterText { get; set; }

        public bool TestString(string input)
        {
            if (string.IsNullOrEmpty(FilterText) || string.IsNullOrEmpty(input))
                return false;

            try
            {
                switch (ComparisonMethod)
                {
                    case FilterComparisonMethod.Equals:
                        return input.Equals(FilterText, StringComparison.InvariantCultureIgnoreCase);

                    case FilterComparisonMethod.Any:
                        return input.ContainsAny(FilterText.Split((char[]) null, StringSplitOptions.RemoveEmptyEntries),
                            StringComparison.InvariantCultureIgnoreCase);

                    case FilterComparisonMethod.StartsWith:
                        return input.StartsWith(FilterText, StringComparison.InvariantCultureIgnoreCase);

                    case FilterComparisonMethod.EndsWith:
                        return input.EndsWith(FilterText, StringComparison.InvariantCultureIgnoreCase);

                    case FilterComparisonMethod.Contains:
                        return input.Contains(FilterText, StringComparison.InvariantCultureIgnoreCase);

                    case FilterComparisonMethod.Regex:
                        return Regex.IsMatch(input, FilterText, RegexOptions.CultureInvariant);

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
                return false;
            }
        }
    }
}