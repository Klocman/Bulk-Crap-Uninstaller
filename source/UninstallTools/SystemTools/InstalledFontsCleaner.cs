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
using UninstallTools.RegistryEngine;

namespace UninstallTools.SystemTools
{
    /// <summary>
    /// Represents an installed font item from the Windows registry.
    /// </summary>
    public class InstalledFontItem
    {
        public string FontName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FullFontPath { get; set; } = string.Empty;
        public string RegistryRoot { get; set; } = "HKLM";
        public bool IsOrphaned { get; set; }
        public bool IsSystemDefault { get; set; }
        public bool IsSelected { get; set; } = true;
    }

    /// <summary>
    /// Scans, previews, and cleans orphaned or leftover font registrations from deleted third-party software.
    /// </summary>
    public static class InstalledFontsCleaner
    {
        private static readonly HashSet<string> SystemDefaultFontFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "arial.ttf", "arialbd.ttf", "ariali.ttf", "arialbi.ttf", "calibri.ttf", "calibrib.ttf", "calibrii.ttf",
            "calibriz.ttf", "cour.ttf", "courbd.ttf", "couri.ttf", "courbi.ttf", "segoeui.ttf", "segoeuib.ttf",
            "segoeuii.ttf", "segoeuiz.ttf", "tahoma.ttf", "tahomabd.ttf", "times.ttf", "timesbd.ttf", "timesi.ttf",
            "timesbi.ttf", "verdana.ttf", "verdanab.ttf", "verdanai.ttf", "verdanaz.ttf", "consola.ttf", "consolab.ttf"
        };

        /// <summary>
        /// Scans all registered fonts across HKLM and HKCU.
        /// </summary>
        public static List<InstalledFontItem> ScanInstalledFonts()
        {
            var list = new List<InstalledFontItem>();
            var winFontsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts");
            var userFontsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Windows", "Fonts");

            // 1. Scan HKLM Fonts
            ScanFontKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts", "HKLM", winFontsDir, list);

            // 2. Scan HKCU User-Installed Fonts
            ScanFontKey(Registry.CurrentUser, @"Software\Microsoft\Windows NT\CurrentVersion\Fonts", "HKCU", userFontsDir, list);

            return list.OrderBy(f => f.FontName).ToList();
        }

        private static void ScanFontKey(RegistryKey rootKey, string subKeyPath, string rootLabel, string defaultDir, List<InstalledFontItem> list)
        {
            try
            {
                using var key = rootKey.OpenSubKey(subKeyPath);
                if (key == null) return;

                foreach (var fontValName in key.GetValueNames())
                {
                    var fileVal = key.GetValue(fontValName)?.ToString() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(fileVal)) continue;

                    string fullPath;
                    if (Path.IsPathRooted(fileVal))
                    {
                        fullPath = fileVal;
                    }
                    else
                    {
                        fullPath = Path.Combine(defaultDir, fileVal);
                    }

                    var isSystem = SystemDefaultFontFiles.Contains(Path.GetFileName(fileVal));
                    var isOrphaned = !File.Exists(fullPath);

                    list.Add(new InstalledFontItem
                    {
                        FontName = fontValName,
                        FileName = fileVal,
                        FullFontPath = fullPath,
                        RegistryRoot = rootLabel,
                        IsOrphaned = isOrphaned,
                        IsSystemDefault = isSystem
                    });
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error($"Error scanning font key {subKeyPath}: {ex.Message}");
            }
        }

        /// <summary>
        /// Cleans orphaned font registry entries with automated backup.
        /// </summary>
        public static int CleanOrphanedFonts(IEnumerable<InstalledFontItem> fonts, string backupDirectory = null)
        {
            var targets = fonts?.Where(f => f.IsSelected && f.IsOrphaned && !f.IsSystemDefault).ToList() ?? new List<InstalledFontItem>();
            if (!targets.Any()) return 0;

            int cleaned = 0;

            if (!string.IsNullOrEmpty(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
                var backupFile = Path.Combine(backupDirectory, $"Fonts_Backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.reg");
                var keyList = new List<string>
                {
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts",
                    @"HKEY_CURRENT_USER\Software\Microsoft\Windows NT\CurrentVersion\Fonts"
                };
                SafeRegistryEngine.ExportRegistryKeys(keyList, backupFile);
            }

            foreach (var f in targets)
            {
                try
                {
                    var root = f.RegistryRoot == "HKCU" ? Registry.CurrentUser : Registry.LocalMachine;
                    var subPath = f.RegistryRoot == "HKCU"
                        ? @"Software\Microsoft\Windows NT\CurrentVersion\Fonts"
                        : @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts";

                    using var key = root.OpenSubKey(subPath, true);
                    if (key != null)
                    {
                        key.DeleteValue(f.FontName, false);
                        cleaned++;
                        StructuredLogger.Info($"Cleaned orphaned font registration: {f.FontName}");
                    }
                }
                catch { }
            }

            return cleaned;
        }
    }
}
