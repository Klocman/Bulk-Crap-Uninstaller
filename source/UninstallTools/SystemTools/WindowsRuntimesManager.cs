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
using System.Text.RegularExpressions;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    /// <summary>
    /// Category of shared runtime / redistributable component.
    /// </summary>
    public enum RuntimeCategory
    {
        VisualCpp,
        DotNet,
        DirectX,
        Java,
        WebView2,
        Vulkan,
        OpenAL,
        Other
    }

    /// <summary>
    /// Represents an installed runtime or shared system redistributable.
    /// </summary>
    public class RuntimeItem
    {
        public string Name { get; set; } = string.Empty;
        public RuntimeCategory Category { get; set; } = RuntimeCategory.Other;
        public string Version { get; set; } = string.Empty;
        public string Architecture { get; set; } = "x64";
        public string Publisher { get; set; } = string.Empty;
        public string InstallPath { get; set; } = string.Empty;
        public string RegistryKeyPath { get; set; } = string.Empty;
        public string UninstallString { get; set; } = string.Empty;
        public bool IsSuperseded { get; set; }
        public string SupersededBy { get; set; } = string.Empty;
        public long EstimatedSizeBytes { get; set; }
        public bool IsSystemCritical { get; set; }
    }

    /// <summary>
    /// Discovers, inspects, and manages shared runtimes and redistributable packages
    /// (Visual C++, .NET Framework/.NET Core, DirectX, Java, WebView2, Vulkan).
    /// </summary>
    public static class WindowsRuntimesManager
    {
        /// <summary>
        /// Scans the system for all installed runtimes across the registry, dotnet CLI, and known paths.
        /// </summary>
        public static List<RuntimeItem> ScanInstalledRuntimes()
        {
            var runtimes = new List<RuntimeItem>();

            try
            {
                ScanVisualCppRuntimes(runtimes);
                ScanDotNetRuntimes(runtimes);
                ScanDirectXRuntimes(runtimes);
                ScanJavaRuntimes(runtimes);
                ScanWebView2Runtime(runtimes);
                ScanVulkanRuntimes(runtimes);
                ScanOpenAL(runtimes);
                MarkSupersededRuntimes(runtimes);
            }
            catch (Exception ex)
            {
                StructuredLogger.Error($"Error scanning installed runtimes: {ex.Message}");
            }

            return runtimes.OrderBy(r => r.Category).ThenBy(r => r.Name).ToList();
        }

        private static void ScanVisualCppRuntimes(List<RuntimeItem> list)
        {
            var uninstallKeys = new[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            foreach (var keyPath in uninstallKeys)
            {
                using var baseKey = Registry.LocalMachine.OpenSubKey(keyPath);
                if (baseKey == null) continue;

                foreach (var subKeyName in baseKey.GetSubKeyNames())
                {
                    using var subKey = baseKey.OpenSubKey(subKeyName);
                    if (subKey == null) continue;

                    var displayName = subKey.GetValue("DisplayName")?.ToString() ?? string.Empty;
                    if (string.IsNullOrEmpty(displayName)) continue;

                    if (displayName.IndexOf("Visual C++", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        displayName.IndexOf("Microsoft Visual C++", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        var version = subKey.GetValue("DisplayVersion")?.ToString() ?? string.Empty;
                        var publisher = subKey.GetValue("Publisher")?.ToString() ?? "Microsoft Corporation";
                        var uninstall = subKey.GetValue("UninstallString")?.ToString() ?? string.Empty;
                        var arch = displayName.Contains("x64") ? "x64" : (displayName.Contains("x86") ? "x86" : (displayName.Contains("ARM64") ? "ARM64" : "x86"));

                        list.Add(new RuntimeItem
                        {
                            Name = displayName,
                            Category = RuntimeCategory.VisualCpp,
                            Version = version,
                            Architecture = arch,
                            Publisher = publisher,
                            RegistryKeyPath = $@"{keyPath}\{subKeyName}",
                            UninstallString = uninstall,
                            EstimatedSizeBytes = 30 * 1024 * 1024,
                            IsSystemCritical = true
                        });
                    }
                }
            }
        }

        private static void ScanDotNetRuntimes(List<RuntimeItem> list)
        {
            // 1. .NET Framework from Registry
            try
            {
                using var ndpKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full");
                if (ndpKey != null)
                {
                    var release = ndpKey.GetValue("Release");
                    var version = ndpKey.GetValue("Version")?.ToString() ?? "4.x";
                    if (release != null)
                    {
                        list.Add(new RuntimeItem
                        {
                            Name = $".NET Framework {GetNetFrameworkVersionName((int)release)}",
                            Category = RuntimeCategory.DotNet,
                            Version = version,
                            Architecture = Environment.Is64BitOperatingSystem ? "x64" : "x86",
                            Publisher = "Microsoft Corporation",
                            RegistryKeyPath = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full",
                            EstimatedSizeBytes = 150 * 1024 * 1024,
                            IsSystemCritical = true
                        });
                    }
                }
            }
            catch { }

            // 2. Modern .NET (Core / .NET 5/6/7/8/9) via dotnet --list-runtimes or folder inspection
            var dotnetFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "dotnet", "shared");
            if (Directory.Exists(dotnetFolder))
            {
                foreach (var runtimeTypeDir in Directory.GetDirectories(dotnetFolder))
                {
                    var typeName = Path.GetFileName(runtimeTypeDir);
                    foreach (var verDir in Directory.GetDirectories(runtimeTypeDir))
                    {
                        var verName = Path.GetFileName(verDir);
                        list.Add(new RuntimeItem
                        {
                            Name = $"{typeName} {verName}",
                            Category = RuntimeCategory.DotNet,
                            Version = verName,
                            Architecture = "x64",
                            Publisher = "Microsoft Corporation",
                            InstallPath = verDir,
                            EstimatedSizeBytes = 50 * 1024 * 1024,
                            IsSystemCritical = false
                        });
                    }
                }
            }

            var dotnetFolderX86 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "dotnet", "shared");
            if (Directory.Exists(dotnetFolderX86))
            {
                foreach (var runtimeTypeDir in Directory.GetDirectories(dotnetFolderX86))
                {
                    var typeName = Path.GetFileName(runtimeTypeDir);
                    foreach (var verDir in Directory.GetDirectories(runtimeTypeDir))
                    {
                        var verName = Path.GetFileName(verDir);
                        list.Add(new RuntimeItem
                        {
                            Name = $"{typeName} {verName} (x86)",
                            Category = RuntimeCategory.DotNet,
                            Version = verName,
                            Architecture = "x86",
                            Publisher = "Microsoft Corporation",
                            InstallPath = verDir,
                            EstimatedSizeBytes = 45 * 1024 * 1024,
                            IsSystemCritical = false
                        });
                    }
                }
            }
        }

        private static string GetNetFrameworkVersionName(int releaseKey)
        {
            if (releaseKey >= 533320) return "4.8.1";
            if (releaseKey >= 528040) return "4.8";
            if (releaseKey >= 461808) return "4.7.2";
            if (releaseKey >= 461308) return "4.7.1";
            if (releaseKey >= 460798) return "4.7";
            if (releaseKey >= 394802) return "4.6.2";
            if (releaseKey >= 394254) return "4.6.1";
            if (releaseKey >= 393295) return "4.6";
            if (releaseKey >= 379893) return "4.5.2";
            if (releaseKey >= 378675) return "4.5.1";
            if (releaseKey >= 378389) return "4.5";
            return "4.0";
        }

        private static void ScanDirectXRuntimes(List<RuntimeItem> list)
        {
            try
            {
                using var dxKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\DirectX");
                if (dxKey != null)
                {
                    var version = dxKey.GetValue("Version")?.ToString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(version))
                    {
                        list.Add(new RuntimeItem
                        {
                            Name = "DirectX Runtime",
                            Category = RuntimeCategory.DirectX,
                            Version = version,
                            Architecture = Environment.Is64BitOperatingSystem ? "x64" : "x86",
                            Publisher = "Microsoft Corporation",
                            RegistryKeyPath = @"SOFTWARE\Microsoft\DirectX",
                            EstimatedSizeBytes = 80 * 1024 * 1024,
                            IsSystemCritical = true
                        });
                    }
                }
            }
            catch { }
        }

        private static void ScanJavaRuntimes(List<RuntimeItem> list)
        {
            var javaKeys = new[]
            {
                @"SOFTWARE\JavaSoft\Java Runtime Environment",
                @"SOFTWARE\JavaSoft\Java Development Kit",
                @"SOFTWARE\JavaSoft\JDK",
                @"SOFTWARE\JavaSoft\JRE",
                @"SOFTWARE\WOW6432Node\JavaSoft\Java Runtime Environment"
            };

            foreach (var keyPath in javaKeys)
            {
                try
                {
                    using var jKey = Registry.LocalMachine.OpenSubKey(keyPath);
                    if (jKey == null) continue;

                    var currentVer = jKey.GetValue("CurrentVersion")?.ToString() ?? string.Empty;
                    foreach (var subKeyName in jKey.GetSubKeyNames())
                    {
                        using var sub = jKey.OpenSubKey(subKeyName);
                        var home = sub?.GetValue("JavaHome")?.ToString() ?? string.Empty;

                        list.Add(new RuntimeItem
                        {
                            Name = $"Java {subKeyName} ({Path.GetFileName(keyPath)})",
                            Category = RuntimeCategory.Java,
                            Version = subKeyName,
                            Architecture = keyPath.Contains("WOW6432Node") ? "x86" : "x64",
                            Publisher = "Oracle Corporation",
                            InstallPath = home,
                            RegistryKeyPath = $@"{keyPath}\{subKeyName}",
                            EstimatedSizeBytes = 120 * 1024 * 1024,
                            IsSystemCritical = false
                        });
                    }
                }
                catch { }
            }
        }

        private static void ScanWebView2Runtime(List<RuntimeItem> list)
        {
            try
            {
                using var wv2Key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}");
                if (wv2Key != null)
                {
                    var version = wv2Key.GetValue("pv")?.ToString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(version))
                    {
                        list.Add(new RuntimeItem
                        {
                            Name = "Microsoft Edge WebView2 Runtime",
                            Category = RuntimeCategory.WebView2,
                            Version = version,
                            Architecture = "x64",
                            Publisher = "Microsoft Corporation",
                            RegistryKeyPath = @"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}",
                            EstimatedSizeBytes = 200 * 1024 * 1024,
                            IsSystemCritical = true
                        });
                    }
                }
            }
            catch { }
        }

        private static void ScanVulkanRuntimes(List<RuntimeItem> list)
        {
            try
            {
                using var vkKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Khronos\Vulkan\ExplicitLayers");
                if (vkKey != null)
                {
                    list.Add(new RuntimeItem
                    {
                        Name = "Vulkan Runtime Components",
                        Category = RuntimeCategory.Vulkan,
                        Version = "Detected",
                        Architecture = "x64",
                        Publisher = "Khronos Group",
                        RegistryKeyPath = @"SOFTWARE\Khronos\Vulkan",
                        EstimatedSizeBytes = 15 * 1024 * 1024,
                        IsSystemCritical = false
                    });
                }
            }
            catch { }
        }

        private static void ScanOpenAL(List<RuntimeItem> list)
        {
            var openAlDll = Path.Combine(Environment.SystemDirectory, "OpenAL32.dll");
            if (File.Exists(openAlDll))
            {
                var fv = FileVersionInfo.GetVersionInfo(openAlDll);
                list.Add(new RuntimeItem
                {
                    Name = "OpenAL Audio Library",
                    Category = RuntimeCategory.OpenAL,
                    Version = fv.FileVersion ?? "1.0",
                    Architecture = "x64",
                    Publisher = fv.CompanyName ?? "Creative Labs",
                    InstallPath = openAlDll,
                    EstimatedSizeBytes = 2 * 1024 * 1024,
                    IsSystemCritical = false
                });
            }
        }

        private static void MarkSupersededRuntimes(List<RuntimeItem> list)
        {
            // Visual C++ 2015, 2017, 2019 are binary-compatible and consolidated into Visual C++ 2015-2022
            var hasUnifiedVc = list.Any(r => r.Category == RuntimeCategory.VisualCpp && r.Name.Contains("2015-2022"));

            if (hasUnifiedVc)
            {
                foreach (var item in list.Where(r => r.Category == RuntimeCategory.VisualCpp))
                {
                    if ((item.Name.Contains("2015") || item.Name.Contains("2017") || item.Name.Contains("2019")) && !item.Name.Contains("2015-2022"))
                    {
                        item.IsSuperseded = true;
                        item.SupersededBy = "Microsoft Visual C++ 2015-2022 Redistributable";
                    }
                }
            }
        }
    }
}
