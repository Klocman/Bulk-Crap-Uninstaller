/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Text.RegularExpressions;

namespace Klocman.Tools
{
    public static class GuidTools
    {
        private const string GuidMatchPattern =
            "^[A-Fa-f0-9]{32}$|^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$";

        private static readonly Regex GuidMatchRegex = new(GuidMatchPattern, RegexOptions.Compiled);

        /// <summary>
        /// Extract and parse a guid from the supplied string. Throws if no guid is found.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value of 'source' cannot be null. </exception>
        /// <exception cref="ArgumentException">Failed to parse the input</exception>
        public static Guid ExtractGuidFromString(string source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            try
            {
                var braceIndex = source.IndexOfAny(new[] {'{', '('});
                if (braceIndex >= 0)
                {
                    var endingBraceIndex = source.IndexOfAny(new[] {'}', ')'});
                    if (endingBraceIndex < 0)
                        throw new ArgumentException("Invalid brace format");

                    source = source.Substring(braceIndex, endingBraceIndex - braceIndex + 1);
                }
                return new Guid(source);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Failed to parse the input", ex);
            }
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Try to parse the supplied string into a guid. Faster than catching exceptions.
        /// </summary>
        public static bool GuidTryParse(string s, out Guid result)
        {
            result = Guid.Empty;
            if (string.IsNullOrEmpty(s) || !GuidMatchRegex.IsMatch(s))
            {
                return false;
            }
            result = new Guid(s);
            return true;
        }

        /// <summary>
        /// Try to extract and parse a guid from the supplied string.
        /// result = Guid.Empty if operation fails.
        /// </summary>
        public static bool TryExtractGuid(string source, out Guid result)
        {
            result = Guid.Empty;
            if (string.IsNullOrEmpty(source))
                return false;

            var braceIndex = source.IndexOfAny(new[] {'{', '('});
            if (braceIndex >= 0)
            {
                var endingBraceIndex = source.IndexOfAny(new[] {'}', ')'});
                source = endingBraceIndex > braceIndex
                    ? source.Substring(braceIndex, endingBraceIndex - braceIndex + 1)
                    : source.Substring(braceIndex + 1);
            }

            return GuidTryParse(source, out result);
        }
    }
}