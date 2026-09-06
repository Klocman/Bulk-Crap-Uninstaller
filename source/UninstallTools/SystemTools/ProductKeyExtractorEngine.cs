/*
 * EBUninstaller Pro - Application Uninstaller & System Optimization Suite
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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public class ProductKeyRecord
    {
        public string ProductName { get; set; } = string.Empty;
        public string ProductKey { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string RegistryPath { get; set; } = string.Empty;
        public DateTime ExtractedDateUtc { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Scans, decodes, and safely exports digital product keys and license identifiers
    /// before software deinstallation to prevent loss of purchased license credentials.
    /// </summary>
    public static class ProductKeyExtractorEngine
    {
        private const string DigitalChars = "BCDFGHJKMPQRTVWXY2346789";

        /// <summary>
        /// Scans known registry license stores to extract product keys.
        /// </summary>
        public static List<ProductKeyRecord> ExtractAllProductKeys()
        {
            var list = new List<ProductKeyRecord>();

            // 1. Extract Windows OS Product Key
            ExtractWindowsProductKey(list);

            // 2. Extract Microsoft Office Product Keys
            ExtractOfficeProductKeys(list);

            return list;
        }

        private static void ExtractWindowsProductKey(List<ProductKeyRecord> list)
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", false);
                if (key == null) return;

                var prodName = key.GetValue("ProductName")?.ToString() ?? "Windows Operating System";
                var prodId = key.GetValue("ProductId")?.ToString() ?? string.Empty;
                var rawDpid = key.GetValue("DigitalProductId") as byte[];

                string keyDecoded = string.Empty;
                if (rawDpid != null && rawDpid.Length >= 67)
                {
                    keyDecoded = DecodeDigitalProductId(rawDpid);
                }

                if (!string.IsNullOrEmpty(keyDecoded))
                {
                    list.Add(new ProductKeyRecord
                    {
                        ProductName = prodName,
                        ProductKey = keyDecoded,
                        ProductId = prodId,
                        Publisher = "Microsoft Corporation",
                        RegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion"
                    });
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.SystemTools, $"Failed to extract Windows product key: {ex.Message}");
            }
        }

        private static void ExtractOfficeProductKeys(List<ProductKeyRecord> list)
        {
            string[] officePaths =
            {
                @"SOFTWARE\Microsoft\Office\16.0\Registration",
                @"SOFTWARE\Microsoft\Office\15.0\Registration",
                @"SOFTWARE\Microsoft\Office\14.0\Registration",
                @"SOFTWARE\WOW6432Node\Microsoft\Office\16.0\Registration"
            };

            foreach (var path in officePaths)
            {
                try
                {
                    using var regKey = Registry.LocalMachine.OpenSubKey(path, false);
                    if (regKey == null) continue;

                    foreach (var sub in regKey.GetSubKeyNames())
                    {
                        using var subKey = regKey.OpenSubKey(sub);
                        if (subKey == null) continue;

                        var prodName = subKey.GetValue("ProductName")?.ToString() ?? subKey.GetValue("ConvertToEdition")?.ToString();
                        var rawDpid = subKey.GetValue("DigitalProductId") as byte[];

                        if (!string.IsNullOrEmpty(prodName) && rawDpid != null && rawDpid.Length >= 67)
                        {
                            var decoded = DecodeDigitalProductId(rawDpid);
                            if (!string.IsNullOrEmpty(decoded) && !list.Any(l => l.ProductKey == decoded))
                            {
                                list.Add(new ProductKeyRecord
                                {
                                    ProductName = prodName,
                                    ProductKey = decoded,
                                    ProductId = subKey.GetValue("ProductID")?.ToString() ?? string.Empty,
                                    Publisher = "Microsoft Corporation",
                                    RegistryPath = $@"{path}\{sub}"
                                });
                            }
                        }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Decodes a 25-character base-24 Windows/Office DigitalProductId byte array.
        /// </summary>
        public static string DecodeDigitalProductId(byte[] digitalProductId)
        {
            if (digitalProductId == null || digitalProductId.Length < 67) return string.Empty;

            try
            {
                var hexPid = new byte[15];
                Array.Copy(digitalProductId, 52, hexPid, 0, 15);

                var isWin8OrHigher = (byte)((digitalProductId[66] / 6) & 1);
                hexPid[14] = (byte)((hexPid[14] & 0xF7) | ((isWin8OrHigher & 2) * 4));

                var decodedChars = new char[29];
                int last = 0;

                for (int i = 24; i >= 0; i--)
                {
                    int current = 0;
                    for (int j = 14; j >= 0; j--)
                    {
                        current = (current * 256) ^ hexPid[j];
                        hexPid[j] = (byte)(current / 24);
                        current %= 24;
                        last = current;
                    }

                    decodedChars[i] = DigitalChars[current];
                }

                if (isWin8OrHigher != 0)
                {
                    var keypart1 = new string(decodedChars, 1, last);
                    var insert = "N";
                    var keypart2 = new string(decodedChars, last + 1, decodedChars.Length - (last + 1));
                    var fullKey = keypart1 + insert + keypart2;
                    return FormatProductKey(fullKey.Substring(0, 25));
                }

                return FormatProductKey(new string(decodedChars, 0, 25));
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string FormatProductKey(string raw25Chars)
        {
            if (string.IsNullOrWhiteSpace(raw25Chars) || raw25Chars.Length < 25) return raw25Chars;

            var sb = new StringBuilder();
            for (int i = 0; i < 25; i++)
            {
                sb.Append(raw25Chars[i]);
                if ((i + 1) % 5 == 0 && i < 24)
                {
                    sb.Append('-');
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Exports extracted license records into a secure backup file.
        /// </summary>
        public static bool ExportKeys(IEnumerable<ProductKeyRecord> keys, string targetFilePath)
        {
            if (keys == null || string.IsNullOrWhiteSpace(targetFilePath)) return false;

            try
            {
                var dir = Path.GetDirectoryName(targetFilePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var json = JsonSerializer.Serialize(keys, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(targetFilePath, json, Encoding.UTF8);

                StructuredLogger.Info(LogCategory.SystemTools, $"Exported {keys.Count()} product keys to: {targetFilePath}");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.SystemTools, $"Failed to export product keys: {ex.Message}");
                return false;
            }
        }
    }
}
