using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Klocman
{
    public static class HelperTools
    {

        public static void SetupEncoding()
        {
            try
            {
                Console.OutputEncoding = Encoding.Unicode;
            }
            catch (IOException)
            {
                /*Old .NET v4 without support for unicode output*/
            }
        }

        public static int HandleHrefMessage(Exception ex)
        {
            var errorCode = Regex.Match(ex.Message, @"0x[\d\w]{8}")
                .Captures.Cast<Capture>().FirstOrDefault()?.Value;

            if (string.IsNullOrEmpty(errorCode) || errorCode.Length < 8)
                return (int) ReturnValue.FunctionFailedCode;

            int.TryParse(errorCode.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out var errorNumber);

            return errorNumber > 0 ? errorNumber : (int)ReturnValue.FunctionFailedCode;
        }

        /// <summary>
        ///     Convert object to PropertyName: Value format for writing it to console.
        ///     Only public properties with getters are processed.
        ///     Can be written directly with Console.WriteLine for an empty line after object.
        /// </summary>
        /// <param name="obj">Object to convert to string</param>
        /// <param name="provider">Provider used for converting values to strings</param>
        public static string ObjectToConsoleOutput(object obj, IFormatProvider provider = null)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (provider == null) provider = CultureInfo.InvariantCulture;

            var propInfos = obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(prop => prop.CanRead)
                .Select(prop => new { prop.Name, val = prop.GetValue(obj, null) })
                .ToList();

            var maxLen = propInfos.Max(x => x.Name.Length) + 2;

            var sb = new StringBuilder();

            foreach (var prop in propInfos)
            {
                sb.Append(prop.Name);
                sb.Append(':');
                sb.Append(' ', maxLen - prop.Name.Length);

                if (prop.val is string s)
                    sb.Append(s.Replace("\r\n", " ").Replace('\n', ' ').Replace('\r', ' '));
                if (prop.val is IConvertible convertible)
                    sb.Append(convertible.ToString(provider));
                else
                    sb.Append(prop.val);

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}