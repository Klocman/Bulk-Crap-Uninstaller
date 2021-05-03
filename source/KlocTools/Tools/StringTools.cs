/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Klocman.Extensions;

namespace Klocman.Tools
{
    public static class StringTools
    {
        /// <summary>
        ///     Atetmpt to extract percentage from a string. "Test 63% of tests" returns 63. "Nope 11" returns -1;
        /// </summary>
        /// <param name="toParse">String to parse. Only the first percent sign is parsed.</param>
        public static int TryExtractPercentage(string toParse)
        {
            var index = toParse.IndexOf('%');
            if (index <= 0)
                return -1;

            var numIndex = -1;
            for (var i = index - 1; i >= 0; i--)
            {
                if (toParse[i] >= '0' && toParse[i] <= '9')
                    numIndex = i;
                else
                    break;
            }

            if (numIndex < index && numIndex >= 0)
            {
                var number = toParse.Substring(numIndex, index - numIndex);
                int resultInt;
                if (int.TryParse(number, out resultInt))
                    return resultInt;
            }

            return -1;
        }

        /// <summary>
        ///     Calculate string similarity by using Damereau-Levenshein Distance algorithm.
        ///     The lower the score, the more similar the strings are.
        /// </summary>
        public static int CompareSimilarity(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(t))
                    return 0;
                return t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            var n = s.Length;
            var m = t.Length;
            var d = new int[n + 1, m + 1];

            // initialize the top and right of the table to 0, 1, 2, ...
            for (var i = 0; i <= n; d[i, 0] = i++)
            {
            }
            for (var j = 1; j <= m; d[0, j] = j++)
            {
            }

            for (var i = 1; i <= n; i++)
            {
                for (var j = 1; j <= m; j++)
                {
                    var cost = t[j - 1] == s[i - 1] ? 0 : 1;
                    var min1 = d[i - 1, j] + 1;
                    var min2 = d[i, j - 1] + 1;
                    var min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }

        public static IEnumerable<char> InvalidFileNameChars => Path.GetInvalidFileNameChars();

        public static IEnumerable<char> InvalidPathChars => Path.GetInvalidPathChars();

        public static IEnumerable<string> NewLineChars => new[] { Environment.NewLine, "\n", "\r" };

        /// <summary>
        ///     Get a unique name based on the supplied baseName. If baseName is found in the otherItems enumerable it is postfixed
        ///     by a number.
        ///     The number is increased by 1 if the new name with the number is in the enumerable.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="otherItems"></param>
        /// <returns>baseName if it was not found in otherItems, or baseName(number) if it was</returns>
        /// <exception cref="ArgumentNullException">The value of 'baseName' cannot be null. </exception>
        /// <exception cref="ArgumentOutOfRangeException">Unique name reached int.MaxValue</exception>
        public static string GetUniqueName(string baseName, IEnumerable<string> otherItems)
        {
            if (baseName == null)
                throw new ArgumentNullException(nameof(baseName));

            baseName = baseName.Trim().SafeNormalize();

            if (otherItems == null)
                return baseName;

            var baseNameCounted = baseName;
            var otherItemList = otherItems.Select(x => x.ToLower()).ToList();

            for (var i = 0; i < int.MaxValue; i++)
            {
                if (i != 0)
                    baseNameCounted = $"{baseName} ({i})";

                if (!otherItemList.Contains(baseNameCounted.ToLower()))
                    return baseNameCounted;
            }
            throw new ArgumentOutOfRangeException(nameof(otherItems), @"Unique name reached int.MaxValue");
        }

        public static bool StringContainsFilter(string input, string filter)
        {
            input = input.StripAccents();
            var filters = filter.StripAccents().Split(' ');
            return filters.All(str => input.IndexOf(str, StringComparison.OrdinalIgnoreCase) != -1);
        }

        /// <summary>
        ///     Strip version number from the end of a string. "MyApp 1.023.1" -> "MyApp"
        ///     If string is null or empty, string.Empty is returned.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StripStringFromVersionNumber(string input)
        {
            if (input == null)
                return string.Empty;

            int previousLen;
            do
            {
                previousLen = input.Length;

                input = input.Trim();

                if (input.Length == 0)
                    return string.Empty;

                if (input.EndsWith(")", StringComparison.Ordinal))
                {
                    var bracketLocation = input.LastIndexOf('(');
                    if (bracketLocation > 4)
                    {
                        input = input.Substring(0, bracketLocation).TrimEnd();
                    }
                }

                input = input.ExtendedTrimEndAny(
                    new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", ".", ",", "-", "_" },
                    StringComparison.InvariantCultureIgnoreCase).TrimEnd();

                if (input.EndsWith(" v", StringComparison.InvariantCultureIgnoreCase))
                    input = input.Substring(0, input.Length - 2);

                input = input.ExtendedTrimEndAny(new[] { "Application", "Helper", " v", " CE" },
                    StringComparison.InvariantCultureIgnoreCase);

                input = input.TrimEnd();
            } while (previousLen != input.Length);

            return input;
        }

        public static string ReplaceNonCharacters(string input, char replacement)
        {
            var sb = new StringBuilder(input.Length);
            for (var i = 0; i < input.Length; i++)
            {
                if (char.IsSurrogatePair(input, i))
                {
                    int c = char.ConvertToUtf32(input, i);
                    i++;
                    if (IsValidCodePoint(c))
                        sb.Append(char.ConvertFromUtf32(c));
                    else
                        sb.Append(replacement);
                }
                else
                {
                    char c = input[i];
                    if (IsValidCodePoint(c))
                        sb.Append(c);
                    else
                        sb.Append(replacement);
                }
            }
            return sb.ToString();
        }

        private static bool IsValidCodePoint(int point)
        {
            return point < 0xfdd0 || point >= 0xfdf0 && (point & 0xffff) != 0xffff && (point & 0xfffe) != 0xfffe && point <= 0x10ffff;
        }

        public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
        {
            var sb = new StringBuilder();

            var previousIndex = 0;
            var index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }
    }
}