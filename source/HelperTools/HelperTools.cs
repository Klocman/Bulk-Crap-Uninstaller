/*
 * EBUninstaller Pro - Application Uninstaller & System Optimization Suite
 * HelperTools Shared Utilities Subsystem
 * Copyright (C) 2026 EBUninstaller Development Team & Contributors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Win32.Interop;

namespace Klocman
{
    internal static partial class HelperTools
    {
        private static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        private static readonly JsonSerializerOptions PrettyJsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Configures the standard console output and error streams for UTF-8/Unicode encoding.
        /// </summary>
        public static void SetupEncoding()
        {
            try
            {
                Console.OutputEncoding = Encoding.UTF8;
                Console.InputEncoding = Encoding.UTF8;
            }
            catch (IOException)
            {
                try
                {
                    Console.OutputEncoding = Encoding.Unicode;
                }
                catch
                {
                    // Fallback to default encoding
                }
            }
        }

        [GeneratedRegex(@"0x[\da-fA-F]{8}", RegexOptions.Compiled)]
        private static partial Regex HrefRegex();

        /// <summary>
        /// Try to extract the error code from an exception. The message is expected to contain a code in the format 0xXXXXXXXX
        /// otherwise HResult is returned as-is instead.
        /// </summary>
        public static ResultWin32 ExtractHrefCode(Exception error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));
            var code = ExtractHrefCode(error.Message);
            if (code == ResultWin32.INVALID_ERROR_CODE) return (ResultWin32)error.HResult;
            return code;
        }

        /// <summary>
        /// Try to extract the error code from an error message. The message is expected to contain a code in the format 0xXXXXXXXX
        /// where X is a hexadecimal digit. If the code is not found or is invalid, ResultWin32.INVALID_ERROR_CODE is returned.
        /// </summary>
        public static ResultWin32 ExtractHrefCode(string errorMessage)
        {
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));
            
            var match = HrefRegex().Match(errorMessage);
            if (!match.Success)
                return ResultWin32.INVALID_ERROR_CODE;

            var hexSpan = match.Value.AsSpan(2);
            if (uint.TryParse(hexSpan, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var errorNumber))
            {
                var code = (ResultWin32)errorNumber;
                return Enum.IsDefined(typeof(ResultWin32), code) ? code : (ResultWin32)errorNumber;
            }

            return ResultWin32.INVALID_ERROR_CODE;
        }

        /// <summary>
        /// Convert object to PropertyName: Value format for writing it to console.
        /// Only public properties with getters are processed.
        /// </summary>
        public static string ObjectToConsoleOutput(object obj, IFormatProvider provider = null)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            provider ??= CultureInfo.InvariantCulture;

            var propInfos = obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(prop => prop.CanRead)
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null));

            return KeyValueListToConsoleOutput(propInfos, provider);
        }

        /// <summary>
        /// Serializes an object to JSON for structured console streaming or pipe communication.
        /// </summary>
        public static string ObjectToJsonOutput(object obj, bool pretty = false)
        {
            if (obj == null) return "null";
            return JsonSerializer.Serialize(obj, pretty ? PrettyJsonOptions : DefaultJsonOptions);
        }

        /// <summary>
        /// Converts a collection of key-value pairs into formatted tabular key: value console text.
        /// </summary>
        public static string KeyValueListToConsoleOutput(ICollection<KeyValuePair<string, object>> propertyKeyValues,
            IFormatProvider provider = null)
        {
            if (propertyKeyValues == null) throw new ArgumentNullException(nameof(propertyKeyValues));
            if (propertyKeyValues.Count == 0) return string.Empty;

            provider ??= CultureInfo.InvariantCulture;
            var maxLen = propertyKeyValues.Max(x => x.Key?.Length ?? 0) + 2;

            var sb = new StringBuilder(propertyKeyValues.Count * 64);

            foreach (var prop in propertyKeyValues)
            {
                if (string.IsNullOrEmpty(prop.Key)) continue;

                sb.Append(prop.Key);
                sb.Append(':');
                sb.Append(' ', Math.Max(1, maxLen - prop.Key.Length));

                if (prop.Value is string s)
                {
                    sb.Append(s.Replace("\r\n", " ").Replace('\n', ' ').Replace('\r', ' '));
                }
                else if (prop.Value is IConvertible convertible)
                {
                    sb.Append(convertible.ToString(provider));
                }
                else if (prop.Value != null)
                {
                    sb.Append(prop.Value);
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Checks if the current process is running with administrative privileges.
        /// </summary>
        public static bool IsAdministrator()
        {
            try
            {
                using var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Formats a byte size into human readable string (KB, MB, GB, TB).
        /// </summary>
        public static string FormatBytes(long bytes)
        {
            if (bytes <= 0) return "0 B";
            string[] suffixes = { "B", "KB", "MB", "GB", "TB", "PB" };
            int i = 0;
            double dblBytes = bytes;
            while (dblBytes >= 1024.0 && i < suffixes.Length - 1)
            {
                dblBytes /= 1024.0;
                i++;
            }
            return $"{dblBytes:0.##} {suffixes[i]}";
        }

        /// <summary>
        /// Normalizes long Windows filesystem paths (>260 characters) with \\?\ prefix.
        /// </summary>
        public static string NormalizeLongPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return string.Empty;
            path = Path.GetFullPath(path);

            if (path.Length >= 250 && !path.StartsWith(@"\\?\") && !path.StartsWith(@"\\.\"))
            {
                return @"\\?\" + path;
            }

            return path;
        }

        /// <summary>
        /// Safely deletes a file if it exists, removing read-only attributes if necessary.
        /// </summary>
        public static bool SafeDeleteFile(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path)) return false;

                File.SetAttributes(path, FileAttributes.Normal);
                File.Delete(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Safely deletes a directory and all its contents recursively.
        /// </summary>
        public static bool SafeDeleteDirectory(string path, bool recursive = true)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return false;

                var di = new DirectoryInfo(path);
                foreach (var file in di.GetFiles("*", SearchOption.AllDirectories))
                {
                    try
                    {
                        file.Attributes = FileAttributes.Normal;
                        file.Delete();
                    }
                    catch { }
                }

                Directory.Delete(path, recursive);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
