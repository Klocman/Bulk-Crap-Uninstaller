/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Klocman.Tools;

namespace Klocman.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        ///     Reverse the string using the specified pattern. The string is split into parts corresponding
        ///     to the pattern's values, then each of the parts is reversed and finally they are joined back.
        ///     Example: String("Tester") Pattern(1,3,2) -> T est er -> T tse re -> Result("Ttsere")
        /// </summary>
        /// <param name="value">String to reverse</param>
        /// <param name="pattern">
        ///     Pattern used to reverse the string.
        ///     Warning: The pattern has to have identical total length to the length of the string.
        /// </param>
        public static string Reverse(this string value, int[] pattern)
        {
            if (value == null)
                throw new NullReferenceException();
            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));
            if (value.Length != pattern.Sum())
                throw new ArgumentException(
                    "Pattern doesn't match the string. Sum of the pattern's parts has to have length equal to the length the string.");

            var returnString = new StringBuilder();

            var index = 0;

            // Iterate over the reversal pattern
            foreach (var length in pattern)
            {
                // Reverse the sub-string and append it
                returnString.Append(value.Substring(index, length).Reverse().ToArray());

                // Increment our posistion in the string
                index += length;
            }

            return returnString.ToString();
        }

        public static string ToHexString(this byte[] ba)
        {
            var hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

        #region Methods

        /// <summary>
        ///     Split string on newlines
        /// </summary>
        public static string[] SplitNewlines(this string value, StringSplitOptions options)
        {
            return value.Split(new[] { "\r\n", "\n" }, options);
        }

        /// <summary>
        ///     Append supplied string to this string and return the result.
        /// </summary>
        public static string Append(this string value, string append)
        {
            return String.Concat(value, append); // value + append;
        }

        /// <summary>
        ///     Append supplied strings to this string and return the result.
        /// </summary>
        public static string Append(this string value, params string[] append)
        {
            return String.Concat(value, String.Concat(append)); // value + append;
        }

        /// <summary>
        ///     Append supplied string to the base string if expression is true and return the result.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="expression">Appension will happen only if expression is equal to true</param>
        /// <param name="append">Strings to append, if more than one is supplied they will be concetrated using string.Concat</param>
        /// <returns>Base string with or without the appended extra string</returns>
        public static string AppendIf(this string value, bool expression, params string[] append)
        {
            return expression ? String.Concat(value, String.Concat(append)) : value;
        }

        public static StringBuilder AppendIf(this StringBuilder value, bool expression, string append)
        {
            if (expression)
                value.Append(append);
            return value;
        }

        public static StringBuilder AppendIfFormat(this StringBuilder value, bool expression, string format,
            params object[] args)
        {
            if (expression)
                value.AppendFormat(format, args);
            return value;
        }

        /// <exception cref="ArgumentException">
        ///     <paramref name="comparisonType" /> is not a valid
        ///     <see cref="T:System.StringComparison" /> value.
        /// </exception>
        /// <exception cref="ArgumentNullException">The value of 'value' cannot be null. </exception>
        public static bool Contains(this string value, string str, StringComparison comparisonType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.Contains(str, comparisonType);
        }

        /// <summary>
        ///     Check if base string contains all of the supplied strings.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="items">Items to be compared to the base string</param>
        /// <param name="comparisonType">Rules of the comparison</param>
        /// <returns>True if any of the items were found in the base string, else false</returns>
        public static bool ContainsAll(this string s, IEnumerable<string> items, StringComparison comparisonType)
        {
            return items.All(item => s.Contains(item, comparisonType));
        }

        /// <summary>
        ///     Check if base char array contains any of the supplied chars.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="items">Chars to be compared to the base array</param>
        /// <returns>True if any of the items were found in the base string, else false</returns>
        public static bool ContainsAny(this IEnumerable<char> s, IEnumerable<char> items)
        {
            return items.Any(s.Contains);
        }

        /// <summary>
        ///     Check if base string contains any of the supplied strings.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="items">Items to be compared to the base string</param>
        /// <param name="comparisonType">Rules of the comparison</param>
        /// <returns>True if any of the items were found in the base string, else false</returns>
        public static bool ContainsAny(this string s, IEnumerable<string> items, StringComparison comparisonType)
        {
            return items.Any(item => s.Contains(item, comparisonType));
        }

        /// <summary>
        ///     Check if base string starts with any of the supplied strings.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="items">Items to be compared to the base string</param>
        /// <param name="comparisonType">Rules of the comparison</param>
        /// <returns>True if any of the items were found in the base string, else false</returns>
        public static bool StartsWithAny(this string s, IEnumerable<string> items, StringComparison comparisonType)
        {
            return items.Any(item => s.StartsWith(item, comparisonType));
        }

        /// <summary>
        ///     Remove any non-ascii characters and replace them with their ascii counterparts.
        ///     Will remove most accents (for example ć -> c). IF no valid ASCII counterpart is found the char is replaced by "?"
        /// </summary>
        /// <returns>ASCII compliant string</returns>
        public static string DownconvertToAscii(this string input)
        {
            var tempContentsUnicode = Encoding.GetEncoding(1251).GetBytes(input);
            return Encoding.ASCII.GetString(tempContentsUnicode);
        }

        /// <summary>
        ///     Trim this string from all whitespaces and ending pronounciations (eg. '.' ','),
        ///     then remove any of the supplied items from the end of the resulting string.
        ///     This method is greedy, it will remove the same item multiple times if possible.
        ///     After every successful removal whitespaces and ending pronounciations are trimmed again.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="trimmers">Items to be trimmed from base string</param>
        /// <param name="comparisonType">How the items are compared to the base string</param>
        /// <returns>Trimmed version of the base string</returns>
        public static string ExtendedTrimEndAny(this string s, IEnumerable<string> trimmers,
            StringComparison comparisonType)
        {
            if (String.IsNullOrEmpty(s))
                return String.Empty;

            var trimmerList = trimmers as IList<string> ?? trimmers.ToList();
            var resultStr = s.Trim().Trim(',', '.', ' ');
            var rerun = true;
            while (rerun)
            {
                rerun = false;
                foreach (var trimmer in trimmerList)
                {
                    if (!resultStr.EndsWith(trimmer, comparisonType)) continue;

                    var cutNum = resultStr.Length - trimmer.Length;

                    // Exit the loop quickly if resultStr contains only the trimmer. Also checks for negative lenght.
                    if (cutNum <= 0)
                        return String.Empty;

                    resultStr = resultStr.Substring(0, cutNum);
                    resultStr = resultStr.Trim().Trim(',', '.', ' ');
                    rerun = true;
                    break;
                }
            }
            return resultStr;
        }

        /// <summary>
        /// Trim all whitespace and specified strings from start and end of this string.
        /// </summary>
        public static string ExtendedTrim(this string s, IEnumerable<string> trimmers,
            StringComparison comparisonType)
        {
            if (String.IsNullOrEmpty(s))
                return String.Empty;

            var trimmerList = trimmers as IList<string> ?? trimmers.ToList();
            var resultStr = s.Trim();

            bool rerun;
            do
            {
                rerun = false;
                foreach (var trimmer in trimmerList)
                {
                    if (resultStr.EndsWith(trimmer, comparisonType))
                    {
                        var cutNum = resultStr.Length - trimmer.Length;

                        if (cutNum <= 0)
                            return String.Empty;

                        resultStr = resultStr.Substring(0, cutNum).Trim();
                        rerun = true;
                    }

                    if (resultStr.StartsWith(trimmer, comparisonType))
                    {
                        var cutNum = resultStr.Length - trimmer.Length;

                        if (cutNum <= 0)
                            return String.Empty;

                        resultStr = resultStr.Substring(trimmer.Length).Trim();
                        rerun = true;
                    }

                    if (rerun)
                        break;
                }
            }
            while (rerun);

            return resultStr;
        }

        /// <summary>
        ///     Return index of first element found in this string.
        /// </summary>
        /// <param name="str">Target string</param>
        /// <param name="chars">Chars to look for</param>
        /// <param name="startIndex">Index to start the search at</param>
        /// <returns>Index of first found element or -1 if none were found</returns>
        public static int IndexOfAny(this string str, IEnumerable<char> chars, int startIndex)
        {
            var result = -1;
            foreach (var c in chars)
            {
                var i = str.IndexOf(c, startIndex);
                if (i >= 0 && (i < result || result < 0))
                    result = i;
            }
            return result;
        }

        /// <summary>
        ///     Same as inverted string.IsNullOrEmpty
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNotEmpty(this string input)
        {
            return !String.IsNullOrEmpty(input);
        }

        public static string RemoveInvalidPathChars(this string value)
        {
            if (value == null)
                return String.Empty;
            return StringTools.InvalidPathChars.Aggregate(value,
                (current, str) => current.Replace(str.ToString(), String.Empty));
        }

        /// <summary>
        ///     Remove all possible newline characters in a greedy way.
        /// </summary>
        public static string RemoveNewLines(this string value)
        {
            if (value == null)
                return String.Empty;

            return StringTools.NewLineChars.Aggregate(value, (current, str) => current.Replace(str, String.Empty));
        }

        /// <summary>
        ///     Removes all non-word characters using Regex (^\w).
        /// </summary>
        /// <returns>Stripped string</returns>
        public static string RemoveSpecialCharacters(this string value)
        {
            if (value == null)
                return String.Empty;
            return Regex.Replace(value, @"[^\w ]", String.Empty); //Path.GetInvalidFileNameChars()
        }

        /// <summary>
        ///     Remove diacritics (accents) from a string (for example ć -> c)
        /// </summary>
        /// <returns>ASCII compliant string</returns>
        public static string StripAccents(this string input)
        {
            var text = input.SafeNormalize(NormalizationForm.FormD);
            var chars =
                text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            return new string(chars).SafeNormalize(NormalizationForm.FormC);
        }

        /// <summary>
        ///     Convert to - camelCase.
        /// </summary>
        public static string ToCamelCase(this string baseStr)
        {
            if (String.IsNullOrEmpty(baseStr))
                return String.Empty;

            baseStr = baseStr.ToPascalCase();
            return baseStr.Substring(0, 1).ToLowerInvariant() + baseStr.Substring(1);
        }

        /// <summary>
        ///     Convert to - Normal case
        /// </summary>
        public static string ToNormalCase(this string baseStr)
        {
            if (String.IsNullOrEmpty(baseStr))
                return String.Empty;

            baseStr = baseStr.ToTitleCase().ToLowerInvariant();

            return baseStr.Substring(0, 1).ToUpperInvariant() + baseStr.Substring(1);
        }

        /// <summary>
        ///     Convert to - PascalCase.
        /// </summary>
        public static string ToPascalCase(this string baseStr)
        {
            baseStr = baseStr?.Trim();
            if (String.IsNullOrEmpty(baseStr))
                return String.Empty;

            if (!baseStr.Contains(" ")) return baseStr;

            baseStr = CultureInfo.GetCultureInfo("en-US").TextInfo.ToTitleCase(baseStr);
            return String.Join(String.Empty, baseStr.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        ///     Calculate a StringFormat object from supplied ContentAlignment.
        /// </summary>
        public static StringFormat ToStringFormat(this ContentAlignment a)
        {
            var cFormat = new StringFormat();
            var lNum = (int)Math.Log((double)a, 2);
            cFormat.LineAlignment = (StringAlignment)(lNum / 4);
            cFormat.Alignment = (StringAlignment)(lNum % 4);
            cFormat.Trimming = StringTrimming.None;
            return cFormat;
        }

        /// <summary>
        ///     Convert to - Title Case
        ///     Capitalize the first character and add a space before each capitalized letter (except the first character).
        /// </summary>
        public static string ToTitleCase(this string baseStr)
        {
            if (String.IsNullOrEmpty(baseStr))
                return String.Empty;

            const string pattern = @"(?<=\w)(?=[A-Z])";
            baseStr = Regex.Replace(baseStr.ToPascalCase(), pattern, " ", RegexOptions.None);
            return baseStr.Substring(0, 1).ToUpperInvariant() + baseStr.Substring(1);
        }

        /// <summary>
        /// Safe version of normalize that doesn't crash on invalid code points in string.
        /// Instead the points are replaced with question marks.
        /// </summary>
        public static string SafeNormalize(this string input, NormalizationForm normalizationForm = NormalizationForm.FormC)
        {
            try
            {
                return StringTools.ReplaceNonCharacters(input, '?').Normalize(normalizationForm);
            }
            catch (ArgumentException e)
            {
                throw new InvalidDataException("String contains invalid characters. Data: " + Encoding.UTF32.GetBytes(input).ToHexString(), e);
            }
        }

        #endregion Methods
    }
}