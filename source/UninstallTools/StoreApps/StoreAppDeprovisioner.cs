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
using System.Linq;
using System.Text.RegularExpressions;
using UninstallTools.Core;

namespace UninstallTools.StoreApps
{
    /// <summary>
    /// Model representing a provisioned Windows AppX / MSIX package in the OS image.
    /// </summary>
    public class ProvisionedAppPackage
    {
        public string DisplayName { get; set; } = string.Empty;
        public string PackageName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Architecture { get; set; } = "neutral";
        public string Publisher { get; set; } = string.Empty;
        public bool IsSystemCritical { get; set; }
        public bool IsSelected { get; set; } = true;
    }

    /// <summary>
    /// Deprovisions pre-installed Windows Store (AppX/MSIX) bloatware packages from the Windows OS image
    /// so new user profiles on the system are created clean.
    /// </summary>
    public static class StoreAppDeprovisioner
    {
        private static readonly HashSet<string> ProtectedPackagePrefixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Microsoft.Windows.ShellExperienceHost",
            "Microsoft.Windows.StartMenuExperienceHost",
            "Microsoft.Windows.Cortana",
            "Microsoft.Windows.SecHealthUI",
            "Microsoft.DesktopAppInstaller",
            "Microsoft.UI.Xaml",
            "Microsoft.VCLibs",
            "Microsoft.NET.Native.Runtime",
            "Microsoft.NET.Native.Framework"
        };

        /// <summary>
        /// Retrieves all provisioned AppX packages staged in the Windows image via DISM.
        /// </summary>
        public static List<ProvisionedAppPackage> GetProvisionedPackages()
        {
            var packages = new List<ProvisionedAppPackage>();

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "dism.exe",
                    Arguments = "/Online /Get-ProvisionedAppxPackages",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                if (process != null)
                {
                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit(30000);

                    packages = ParseDismAppxOutput(output);
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error($"Failed to query provisioned AppX packages: {ex.Message}");
            }

            return packages;
        }

        /// <summary>
        /// Parses standard text output from DISM /Get-ProvisionedAppxPackages.
        /// </summary>
        public static List<ProvisionedAppPackage> ParseDismAppxOutput(string output)
        {
            var list = new List<ProvisionedAppPackage>();
            if (string.IsNullOrWhiteSpace(output)) return list;

            var blocks = Regex.Split(output, @"(?:\r?\n){2,}");
            foreach (var block in blocks)
            {
                if (string.IsNullOrWhiteSpace(block) || !block.Contains("DisplayName", StringComparison.OrdinalIgnoreCase))
                    continue;

                var item = new ProvisionedAppPackage();
                var lines = block.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    var parts = line.Split(new[] { ':' }, 2);
                    if (parts.Length < 2) continue;

                    var key = parts[0].Trim();
                    var val = parts[1].Trim();

                    if (key.IndexOf("DisplayName", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        item.DisplayName = val;
                    }
                    else if (key.IndexOf("PackageName", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        item.PackageName = val;
                    }
                    else if (key.IndexOf("Version", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        item.Version = val;
                    }
                    else if (key.IndexOf("Architecture", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        item.Architecture = val;
                    }
                }

                if (!string.IsNullOrEmpty(item.PackageName))
                {
                    item.IsSystemCritical = ProtectedPackagePrefixes.Any(p => item.PackageName.StartsWith(p, StringComparison.OrdinalIgnoreCase));
                    list.Add(item);
                }
            }

            return list;
        }

        /// <summary>
        /// Deprovisions a package from the Windows image using elevated DISM.
        /// </summary>
        public static bool DeprovisionPackage(string packageName)
        {
            if (string.IsNullOrWhiteSpace(packageName)) return false;

            if (ProtectedPackagePrefixes.Any(p => packageName.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
            {
                StructuredLogger.Warning($"Refusing to deprovision protected package: {packageName}");
                return false;
            }

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "dism.exe",
                    Arguments = $"/Online /Remove-ProvisionedAppxPackage /PackageName:{packageName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                process?.WaitForExit(60000);

                var success = process?.ExitCode == 0;
                StructuredLogger.Info($"Deprovisioned {packageName}: ExitCode {process?.ExitCode}");
                return success;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error($"Error deprovisioning {packageName}: {ex.Message}");
                return false;
            }
        }
    }
}
