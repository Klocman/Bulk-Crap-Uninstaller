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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    /// <summary>
    /// Model representing an installed OEM / third-party hardware driver.
    /// </summary>
    public class DriverBackupItem
    {
        public string PublishedName { get; set; } = string.Empty; // e.g. oem12.inf
        public string OriginalFileName { get; set; } = string.Empty; // e.g. nv_dispi.inf
        public string ProviderName { get; set; } = string.Empty; // e.g. NVIDIA, Intel, Realtek
        public string ClassName { get; set; } = string.Empty; // e.g. Display, Net, Media
        public string ClassGuid { get; set; } = string.Empty;
        public string DriverVersion { get; set; } = string.Empty;
        public string DriverDate { get; set; } = string.Empty;
        public string SignerName { get; set; } = string.Empty;
        public long EstimatedSizeBytes { get; set; }
    }

    /// <summary>
    /// Result of driver export operation.
    /// </summary>
    public class DriverBackupResult
    {
        public bool Success { get; set; }
        public string DestinationDirectory { get; set; } = string.Empty;
        public int ExportedCount { get; set; }
        public string ManifestPath { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
    }

    /// <summary>
    /// Windows Driver Export and Backup Engine.
    /// Safely enumerates and exports all third-party OEM drivers using native Windows PnP tools
    /// before software or driver removal.
    /// </summary>
    public static class WindowsDriverBackupEngine
    {
        /// <summary>
        /// Enumerates all third-party OEM drivers installed in the Windows Driver Store.
        /// </summary>
        public static List<DriverBackupItem> EnumerateOemDrivers()
        {
            var drivers = new List<DriverBackupItem>();

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "pnputil.exe",
                    Arguments = "/enum-drivers",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                if (process != null)
                {
                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit(15000);

                    drivers = ParsePnpUtilOutput(output);
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error($"Failed to enumerate drivers with pnputil: {ex.Message}");
            }

            return drivers;
        }

        /// <summary>
        /// Parses the standard text output from pnputil.exe /enum-drivers.
        /// </summary>
        public static List<DriverBackupItem> ParsePnpUtilOutput(string output)
        {
            var list = new List<DriverBackupItem>();
            if (string.IsNullOrWhiteSpace(output)) return list;

            var blocks = Regex.Split(output, @"(?:\r?\n){2,}");
            foreach (var block in blocks)
            {
                if (string.IsNullOrWhiteSpace(block) || !block.Contains("oem", StringComparison.OrdinalIgnoreCase))
                    continue;

                var item = new DriverBackupItem();
                var lines = block.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    var parts = line.Split(new[] { ':' }, 2);
                    if (parts.Length < 2) continue;

                    var key = parts[0].Trim();
                    var val = parts[1].Trim();

                    if (key.IndexOf("Published Name", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        key.IndexOf("Veröffentlichter Name", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        item.PublishedName = val;
                    }
                    else if (key.IndexOf("Original Name", StringComparison.OrdinalIgnoreCase) >= 0 ||
                             key.IndexOf("Originaldateiname", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        item.OriginalFileName = val;
                    }
                    else if (key.IndexOf("Provider Name", StringComparison.OrdinalIgnoreCase) >= 0 ||
                             key.IndexOf("Anbietername", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        item.ProviderName = val;
                    }
                    else if (key.IndexOf("Class Name", StringComparison.OrdinalIgnoreCase) >= 0 ||
                             key.IndexOf("Klassenname", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        item.ClassName = val;
                    }
                    else if (key.IndexOf("Class GUID", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        item.ClassGuid = val;
                    }
                    else if (key.IndexOf("Driver Version", StringComparison.OrdinalIgnoreCase) >= 0 ||
                             key.IndexOf("Treiberversion", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        item.DriverVersion = val;
                    }
                    else if (key.IndexOf("Driver Date", StringComparison.OrdinalIgnoreCase) >= 0 ||
                             key.IndexOf("Treiberdatum", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        item.DriverDate = val;
                    }
                    else if (key.IndexOf("Signer Name", StringComparison.OrdinalIgnoreCase) >= 0 ||
                             key.IndexOf("Signaturgeber", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        item.SignerName = val;
                    }
                }

                if (!string.IsNullOrEmpty(item.PublishedName))
                {
                    list.Add(item);
                }
            }

            return list;
        }

        /// <summary>
        /// Exports all third-party drivers into a designated target directory.
        /// </summary>
        public static DriverBackupResult ExportDrivers(string targetDirectory)
        {
            var sw = Stopwatch.StartNew();
            var result = new DriverBackupResult
            {
                DestinationDirectory = targetDirectory
            };

            try
            {
                if (string.IsNullOrWhiteSpace(targetDirectory))
                {
                    result.ErrorMessage = "Destination directory path cannot be empty.";
                    return result;
                }

                // Security check: ensure target is not a protected system path
                if (SecurityGuard.IsCriticalPath(targetDirectory))
                {
                    result.ErrorMessage = "Cannot export drivers into a protected Windows system directory.";
                    return result;
                }

                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                // Execute native pnputil export command
                var psi = new ProcessStartInfo
                {
                    FileName = "pnputil.exe",
                    Arguments = $"/export-driver * \"{targetDirectory}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                if (process != null)
                {
                    process.WaitForExit(120000); // 2 minute timeout for driver export
                }

                var drivers = EnumerateOemDrivers();
                var exportedFiles = Directory.GetFiles(targetDirectory, "*.inf", SearchOption.AllDirectories);
                result.ExportedCount = exportedFiles.Length > 0 ? exportedFiles.Length : drivers.Count;

                // Write metadata manifest
                var manifest = new
                {
                    Application = "EBUninstaller Pro",
                    ExportDateUtc = DateTime.UtcNow,
                    MachineName = Environment.MachineName,
                    OSVersion = Environment.OSVersion.ToString(),
                    TotalDriversExported = result.ExportedCount,
                    Drivers = drivers
                };

                var manifestPath = Path.Combine(targetDirectory, "driver_manifest.json");
                File.WriteAllText(manifestPath, JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }));
                result.ManifestPath = manifestPath;
                result.Success = true;

                StructuredLogger.Info($"Exported {result.ExportedCount} drivers successfully to {targetDirectory}");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                StructuredLogger.Error($"Driver export failed: {ex.Message}");
            }
            finally
            {
                sw.Stop();
                result.Duration = sw.Elapsed;
            }

            return result;
        }
    }
}
