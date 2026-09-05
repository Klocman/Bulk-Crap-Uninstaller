/*
    EBUninstaller Pro - Windows Context Menu Manager
    Auditing, enabling/disabling, and cleanup of 3rd-party Explorer context menu shell extensions.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.WindowsIntegration
{
    public class ContextMenuItem
    {
        public string Name { get; set; } = string.Empty;
        public string RegistryPath { get; set; } = string.Empty;
        public string Clsid { get; set; } = string.Empty;
        public string TargetModulePath { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string LocationType { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = true;
        public bool IsOrphaned { get; set; }
        public bool IsSystemCritical { get; set; }
    }

    public static class ContextMenuManager
    {
        private static readonly string[] ScanLocations = new[]
        {
            @"*\shellex\ContextMenuHandlers",
            @"Directory\shellex\ContextMenuHandlers",
            @"Directory\Background\shellex\ContextMenuHandlers",
            @"Folder\shellex\ContextMenuHandlers",
            @"Drive\shellex\ContextMenuHandlers",
            @"AllFilesystemObjects\shellex\ContextMenuHandlers"
        };

        private static readonly HashSet<string> ProtectedClsids = new(StringComparer.OrdinalIgnoreCase)
        {
            "{a2a9545d-a0c2-42b4-9708-a0b2badd77c8}", // Windows Defender
            "{90aa3a4e-1c62-44be-a8cc-af210c80e253}", // Shell Sharing
            "{7ad84985-03d7-477e-8a64-032252e58e38}", // Play To
            "{f81e9010-6ea4-11ce-a7ff-00aa003ca9f6}", // Shell New
            "{e2bf9676-5f0f-45ab-9bf8-b729f3f8b635}"  // Send To
        };

        public static List<ContextMenuItem> GetContextMenuItems()
        {
            var results = new List<ContextMenuItem>();
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return results;

            foreach (var loc in ScanLocations)
            {
                ScanRegistryKey(Registry.ClassesRoot, loc, results);
                ScanRegistryKey(Registry.CurrentUser, @"Software\Classes\" + loc, results);
            }

            return results;
        }

        private static void ScanRegistryKey(RegistryKey rootKey, string subKeyPath, List<ContextMenuItem> results)
        {
            try
            {
                using var baseKey = rootKey.OpenSubKey(subKeyPath);
                if (baseKey == null) return;

                foreach (var subName in baseKey.GetSubKeyNames())
                {
                    try
                    {
                        bool isExplicitlyDisabled = subName.StartsWith("-");
                        var cleanName = subName.TrimStart('-');

                        using var itemKey = baseKey.OpenSubKey(subName);
                        if (itemKey == null) continue;

                        string clsid = (itemKey.GetValue("") as string) ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(clsid) && Guid.TryParse(cleanName, out _))
                        {
                            clsid = cleanName;
                        }

                        var (targetModule, publisher) = ResolveClsid(clsid);
                        bool isOrphaned = !string.IsNullOrEmpty(targetModule) && !File.Exists(targetModule);
                        bool isProtected = ProtectedClsids.Contains(clsid) || (targetModule != null && SecurityGuard.IsProtectedPath(targetModule));

                        results.Add(new ContextMenuItem
                        {
                            Name = cleanName,
                            RegistryPath = $@"{rootKey.Name}\{subKeyPath}\{subName}",
                            Clsid = clsid,
                            TargetModulePath = targetModule ?? string.Empty,
                            Publisher = publisher ?? string.Empty,
                            LocationType = subKeyPath.Split('\\')[0],
                            IsEnabled = !isExplicitlyDisabled,
                            IsOrphaned = isOrphaned,
                            IsSystemCritical = isProtected
                        });
                    }
                    catch (Exception ex)
                    {
                        StructuredLogger.Log(LogLevel.Warning, "ContextMenuManager", $"Error reading {subName}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Warning, "ContextMenuManager", $"Error opening {subKeyPath}: {ex.Message}");
            }
        }

        public static (string? ModulePath, string? Publisher) ResolveClsid(string clsid)
        {
            if (string.IsNullOrWhiteSpace(clsid))
                return (null, null);

            try
            {
                using var clsidKey = Registry.ClassesRoot.OpenSubKey($@"CLSID\{clsid}\InprocServer32");
                if (clsidKey != null)
                {
                    string? path = clsidKey.GetValue("") as string;
                    if (!string.IsNullOrEmpty(path))
                    {
                        path = Environment.ExpandEnvironmentVariables(path.Trim('"'));
                        string? publisher = null;
                        if (File.Exists(path))
                        {
                            try
                            {
                                var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(path);
                                publisher = versionInfo.CompanyName;
                            }
                            catch { }
                        }
                        return (path, publisher);
                    }
                }
            }
            catch { }

            return (null, null);
        }

        public static bool ToggleItemStatus(ContextMenuItem item, bool enable)
        {
            if (item.IsSystemCritical)
                return false;

            try
            {
                int lastSlash = item.RegistryPath.LastIndexOf('\\');
                if (lastSlash < 0) return false;

                string parentPath = item.RegistryPath.Substring(0, lastSlash);
                string currentKeyName = item.RegistryPath.Substring(lastSlash + 1);

                RegistryKey root = item.RegistryPath.StartsWith("HKEY_LOCAL_MACHINE") ? Registry.LocalMachine :
                                   item.RegistryPath.StartsWith("HKEY_CURRENT_USER") ? Registry.CurrentUser : Registry.ClassesRoot;

                string relativeParent = parentPath.Substring(parentPath.IndexOf('\\') + 1);
                using var parentKey = root.OpenSubKey(relativeParent, true);
                if (parentKey == null) return false;

                string newKeyName = enable ? item.Name : ("-" + item.Name);
                if (currentKeyName == newKeyName) return true;

                // Move / Rename key
                CopyRegistryKey(parentKey, currentKeyName, newKeyName);
                parentKey.DeleteSubKeyTree(currentKeyName, false);
                item.IsEnabled = enable;
                item.RegistryPath = $@"{parentPath}\{newKeyName}";
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Error, "ContextMenuManager", $"Failed to toggle status: {ex.Message}");
                return false;
            }
        }

        public static bool DeleteItem(ContextMenuItem item)
        {
            if (item.IsSystemCritical)
                return false;

            try
            {
                int lastSlash = item.RegistryPath.LastIndexOf('\\');
                if (lastSlash < 0) return false;

                string parentPath = item.RegistryPath.Substring(0, lastSlash);
                string currentKeyName = item.RegistryPath.Substring(lastSlash + 1);

                RegistryKey root = item.RegistryPath.StartsWith("HKEY_LOCAL_MACHINE") ? Registry.LocalMachine :
                                   item.RegistryPath.StartsWith("HKEY_CURRENT_USER") ? Registry.CurrentUser : Registry.ClassesRoot;

                string relativeParent = parentPath.Substring(parentPath.IndexOf('\\') + 1);
                using var parentKey = root.OpenSubKey(relativeParent, true);
                if (parentKey != null)
                {
                    parentKey.DeleteSubKeyTree(currentKeyName, false);
                    return true;
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Error, "ContextMenuManager", $"Failed to delete item: {ex.Message}");
            }

            return false;
        }

        private static void CopyRegistryKey(RegistryKey parent, string sourceName, string destName)
        {
            using var source = parent.OpenSubKey(sourceName);
            using var dest = parent.CreateSubKey(destName);
            if (source == null || dest == null) return;

            foreach (var valName in source.GetValueNames())
            {
                dest.SetValue(valName, source.GetValue(valName)!, source.GetValueKind(valName));
            }
        }
    }
}
