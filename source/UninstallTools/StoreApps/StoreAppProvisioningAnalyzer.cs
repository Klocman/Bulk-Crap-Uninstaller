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
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.StoreApps
{
    public class ProvisionedAppxPackageRecord
    {
        public string PackageFullName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string InstallLocation { get; set; } = string.Empty;
        public long EstimatedSizeBytes { get; set; }
        public bool IsStagedOnly { get; set; }
        public bool IsSystemCritical { get; set; }
    }

    /// <summary>
    /// Scans, analyzes, and deprovisions staged AppX and MSIX packages across all Windows user stores.
    /// </summary>
    public static class StoreAppProvisioningAnalyzer
    {
        private const string AppxStoreKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Appx\AppxAllUserStore\Applications";

        private static readonly HashSet<string> CriticalPackages = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Microsoft.WindowsStore", "Microsoft.Windows.ShellExperienceHost", "Microsoft.Windows.StartMenuExperienceHost",
            "Microsoft.Windows.Search", "Microsoft.UI.Xaml", "Microsoft.VCLibs", "Microsoft.NET.Native.Runtime"
        };

        /// <summary>
        /// Scans all provisioned and staged Windows Store packages.
        /// </summary>
        public static List<ProvisionedAppxPackageRecord> ScanProvisionedPackages()
        {
            var list = new List<ProvisionedAppxPackageRecord>();

            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(AppxStoreKey, false);
                if (key == null) return list;

                foreach (var pkgName in key.GetSubKeyNames())
                {
                    try
                    {
                        using var pkgKey = key.OpenSubKey(pkgName);
                        if (pkgKey == null) continue;

                        var path = pkgKey.GetValue("Path")?.ToString() ?? string.Empty;
                        var isCritical = CriticalPackages.Any(c => pkgName.IndexOf(c, StringComparison.OrdinalIgnoreCase) >= 0);

                        long size = 0;
                        if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                        {
                            try
                            {
                                var dirInfo = new DirectoryInfo(path);
                                size = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(f => f.Length);
                            }
                            catch { }
                        }

                        var parts = pkgName.Split('_');
                        var displayName = parts.Length > 0 ? parts[0] : pkgName;
                        var version = parts.Length > 1 ? parts[1] : string.Empty;
                        var publisher = parts.Length > 4 ? parts[4] : "Microsoft Corporation";

                        list.Add(new ProvisionedAppxPackageRecord
                        {
                            PackageFullName = pkgName,
                            DisplayName = displayName,
                            Version = version,
                            Publisher = publisher,
                            InstallLocation = path,
                            EstimatedSizeBytes = size,
                            IsSystemCritical = isCritical
                        });
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.StoreApps, "Failed to scan AppX AllUserStore: " + ex.Message);
            }

            return list.OrderBy(p => p.DisplayName).ToList();
        }
    }
}
