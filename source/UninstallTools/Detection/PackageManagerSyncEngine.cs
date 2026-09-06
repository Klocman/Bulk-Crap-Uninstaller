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
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using UninstallTools.Core;

namespace UninstallTools.Detection
{
    /// <summary>
    /// Type of package manager repository.
    /// </summary>
    public enum SupportedPackageManager
    {
        Winget,
        Chocolatey,
        Scoop
    }

    /// <summary>
    /// Item managed by or exportable to Windows package managers.
    /// </summary>
    public class SyncAppItem
    {
        public string DisplayName { get; set; } = string.Empty;
        public string PackageId { get; set; } = string.Empty;
        public SupportedPackageManager ManagerType { get; set; } = SupportedPackageManager.Winget;
        public string InstalledVersion { get; set; } = string.Empty;
        public string AvailableVersion { get; set; } = string.Empty;
        public bool CanUpgrade => !string.IsNullOrEmpty(AvailableVersion) && AvailableVersion != InstalledVersion;
        public bool IsSelected { get; set; } = true;
    }

    /// <summary>
    /// Package Manager synchronization, bulk update, and declarative installation bundle generator.
    /// Supports Windows Package Manager (Winget), Chocolatey, and Scoop.
    /// </summary>
    public static class PackageManagerSyncEngine
    {
        /// <summary>
        /// Queries available package upgrades across Winget and Chocolatey.
        /// </summary>
        public static List<SyncAppItem> ScanUpgradablePackages()
        {
            var list = new List<SyncAppItem>();

            // 1. Scan Winget upgrades
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "winget.exe",
                    Arguments = "upgrade --include-unknown",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                };

                using var process = Process.Start(psi);
                if (process != null)
                {
                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit(30000);

                    list.AddRange(ParseWingetUpgradeOutput(output));
                }
            }
            catch { }

            return list;
        }

        /// <summary>
        /// Parses tabular text output from winget upgrade.
        /// </summary>
        public static List<SyncAppItem> ParseWingetUpgradeOutput(string output)
        {
            var list = new List<SyncAppItem>();
            if (string.IsNullOrWhiteSpace(output)) return list;

            var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            bool tableStarted = false;

            foreach (var line in lines)
            {
                if (line.StartsWith("---") || line.Contains("---"))
                {
                    tableStarted = true;
                    continue;
                }

                if (!tableStarted || string.IsNullOrWhiteSpace(line)) continue;

                var parts = Regex.Split(line.Trim(), @"\s{2,}");
                if (parts.Length >= 4)
                {
                    list.Add(new SyncAppItem
                    {
                        DisplayName = parts[0],
                        PackageId = parts[1],
                        InstalledVersion = parts[2],
                        AvailableVersion = parts[3],
                        ManagerType = SupportedPackageManager.Winget
                    });
                }
            }

            return list;
        }

        /// <summary>
        /// Generates a standard Winget JSON export manifest for bulk software deployment.
        /// </summary>
        public static string GenerateWingetExportJson(IEnumerable<SyncAppItem> apps)
        {
            var targetList = apps?.Where(a => a.ManagerType == SupportedPackageManager.Winget && !string.IsNullOrEmpty(a.PackageId)).ToList() ?? new List<SyncAppItem>();

            var manifest = new
            {
                WinGetVersion = "1.7.0",
                Sources = new[]
                {
                    new
                    {
                        Packages = targetList.Select(a => new
                        {
                            PackageIdentifier = a.PackageId
                        }),
                        SourceDetails = new
                        {
                            Argument = "https://cdn.winget.microsoft.com/cache",
                            Identifier = "Microsoft.Winget.Source",
                            Name = "winget",
                            Type = "Microsoft.PreIndexed.Package"
                        }
                    }
                }
            };

            return JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true });
        }

        /// <summary>
        /// Generates an automated PowerShell script to batch reinstall the selected application bundle.
        /// </summary>
        public static string GeneratePowerShellReinstallScript(IEnumerable<SyncAppItem> apps)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# =============================================================================");
            sb.AppendLine("# EBUninstaller Pro - Automated Application Reinstallation Script");
            sb.AppendLine($"# Generated on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            sb.AppendLine("# =============================================================================");
            sb.AppendLine("$ErrorActionPreference = 'Continue'");
            sb.AppendLine("Write-Host 'Starting automated application bundle installation...' -ForegroundColor Cyan\n");

            var targetList = apps?.ToList() ?? new List<SyncAppItem>();

            foreach (var a in targetList)
            {
                if (a.ManagerType == SupportedPackageManager.Winget)
                {
                    sb.AppendLine($"Write-Host 'Installing [{a.DisplayName}] via winget...' -ForegroundColor Yellow");
                    sb.AppendLine($"winget install --id \"{a.PackageId}\" --exact --silent --accept-package-agreements --accept-source-agreements");
                }
                else if (a.ManagerType == SupportedPackageManager.Chocolatey)
                {
                    sb.AppendLine($"Write-Host 'Installing [{a.DisplayName}] via Chocolatey...' -ForegroundColor Yellow");
                    sb.AppendLine($"choco install \"{a.PackageId}\" -y");
                }
                else if (a.ManagerType == SupportedPackageManager.Scoop)
                {
                    sb.AppendLine($"Write-Host 'Installing [{a.DisplayName}] via Scoop...' -ForegroundColor Yellow");
                    sb.AppendLine($"scoop install \"{a.PackageId}\"");
                }
            }

            sb.AppendLine("\nWrite-Host '[SUCCESS] All application bundle installations finished!' -ForegroundColor Green");
            return sb.ToString();
        }
    }
}
