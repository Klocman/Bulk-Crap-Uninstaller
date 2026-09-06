/*
    EBUninstaller Pro - Windows Font Residuals & Orphan Cleaner
    Detects and safely removes orphaned font registry entries pointing to missing font files.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    public class FontResidualItem
    {
        public string FontName { get; set; } = string.Empty;
        public string FontFileName { get; set; } = string.Empty;
        public string ResolvedPath { get; set; } = string.Empty;
        public string RegistryKeyPath { get; set; } = string.Empty;
        public bool IsCurrentUser { get; set; }
        public bool IsOrphaned { get; set; }
    }

    public static class FontResidualsCleaner
    {
        private const string SystemFontsKey = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts";
        private const string UserFontsKey = @"Software\Microsoft\Windows NT\CurrentVersion\Fonts";

        public static List<FontResidualItem> ScanFontResiduals(bool orphanedOnly = false)
        {
            var results = new List<FontResidualItem>();
            string fontsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts");
            string userFontsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Windows", "Fonts");

            // Scan HKLM System Fonts
            ScanFontsKey(Registry.LocalMachine, SystemFontsKey, fontsDir, false, results);

            // Scan HKCU User Fonts
            ScanFontsKey(Registry.CurrentUser, UserFontsKey, userFontsDir, true, results);

            if (orphanedOnly)
                return results.Where(f => f.IsOrphaned).OrderBy(f => f.FontName).ToList();

            return results.OrderBy(f => f.FontName).ToList();
        }

        private static void ScanFontsKey(RegistryKey rootKey, string subKeyPath, string defaultDir, bool isCurrentUser, List<FontResidualItem> list)
        {
            try
            {
                using var key = rootKey.OpenSubKey(subKeyPath, false);
                if (key == null) return;

                foreach (var valName in key.GetValueNames())
                {
                    try
                    {
                        string fontFile = key.GetValue(valName) as string ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(fontFile)) continue;

                        string resolvedPath;
                        if (Path.IsPathRooted(fontFile))
                        {
                            resolvedPath = Environment.ExpandEnvironmentVariables(fontFile);
                        }
                        else
                        {
                            resolvedPath = Path.Combine(defaultDir, fontFile);
                            if (!File.Exists(resolvedPath))
                            {
                                // Also check system Fonts directory as fallback for user fonts
                                string sysFallback = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts", fontFile);
                                if (File.Exists(sysFallback))
                                    resolvedPath = sysFallback;
                            }
                        }

                        bool exists = File.Exists(resolvedPath);

                        list.Add(new FontResidualItem
                        {
                            FontName = valName,
                            FontFileName = fontFile,
                            ResolvedPath = resolvedPath,
                            RegistryKeyPath = $@"{rootKey.Name}\{subKeyPath}",
                            IsCurrentUser = isCurrentUser,
                            IsOrphaned = !exists
                        });
                    }
                    catch
                    {
                        // Ignore inaccessible individual value
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Warning, "FontResidualsCleaner", $"Failed to scan fonts registry {subKeyPath}: {ex.Message}");
            }
        }

        public static bool RemoveFontResidual(FontResidualItem item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.FontName)) return false;

            try
            {
                RegistryKey rootKey = item.IsCurrentUser ? Registry.CurrentUser : Registry.LocalMachine;
                string subKeyPath = item.IsCurrentUser ? UserFontsKey : SystemFontsKey;

                using var key = rootKey.OpenSubKey(subKeyPath, true);
                if (key == null) return false;

                key.DeleteValue(item.FontName, false);
                StructuredLogger.Log(LogLevel.Info, "FontResidualsCleaner", $"Removed orphaned font registry value: '{item.FontName}'");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Error, "FontResidualsCleaner", $"Failed to remove font registry value '{item.FontName}': {ex.Message}");
                return false;
            }
        }
    }
}
