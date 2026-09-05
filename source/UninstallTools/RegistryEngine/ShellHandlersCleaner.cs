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

namespace UninstallTools.RegistryEngine
{
    /// <summary>
    /// Represents an Explorer Context Menu Shell Extension handler.
    /// </summary>
    public class ShellHandlerItem
    {
        public string HandlerName { get; set; } = string.Empty;
        public string TargetClass { get; set; } = string.Empty; // e.g. "*", "Directory", "Folder", "Drive"
        public string Clsid { get; set; } = string.Empty;
        public string ModulePath { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string RegistryKeyPath { get; set; } = string.Empty;
        public bool IsOrphaned { get; set; }
        public bool IsSelected { get; set; } = true;
    }

    /// <summary>
    /// Scans and purges broken or orphaned Windows Shell Context Menu Handlers.
    /// </summary>
    public static class ShellHandlersCleaner
    {
        private static readonly string[] ContextMenuKeyRoots = new[]
        {
            @"*\shellex\ContextMenuHandlers",
            @"Directory\shellex\ContextMenuHandlers",
            @"Folder\shellex\ContextMenuHandlers",
            @"Drive\shellex\ContextMenuHandlers",
            @"AllFilesystemObjects\shellex\ContextMenuHandlers"
        };

        /// <summary>
        /// Scans all context menu handlers in HKCR.
        /// </summary>
        public static List<ShellHandlerItem> ScanShellHandlers()
        {
            var list = new List<ShellHandlerItem>();

            foreach (var rootPath in ContextMenuKeyRoots)
            {
                try
                {
                    using var key = Registry.ClassesRoot.OpenSubKey(rootPath);
                    if (key == null) continue;

                    var targetClass = rootPath.Split('\\')[0];

                    foreach (var handlerName in key.GetSubKeyNames())
                    {
                        using var sub = key.OpenSubKey(handlerName);
                        if (sub == null) continue;

                        var defaultVal = sub.GetValue(null)?.ToString() ?? string.Empty;
                        var clsid = string.IsNullOrEmpty(defaultVal) ? handlerName : defaultVal;

                        var modulePath = ResolveClsidModule(clsid);
                        var isOrphaned = false;

                        if (!string.IsNullOrEmpty(modulePath))
                        {
                            if (!File.Exists(modulePath) && !SecurityGuard.IsCriticalPath(modulePath))
                            {
                                isOrphaned = true;
                            }
                        }

                        list.Add(new ShellHandlerItem
                        {
                            HandlerName = handlerName,
                            TargetClass = targetClass,
                            Clsid = clsid,
                            ModulePath = modulePath,
                            RegistryKeyPath = $@"{rootPath}\{handlerName}",
                            IsOrphaned = isOrphaned
                        });
                    }
                }
                catch { }
            }

            return list;
        }

        private static string ResolveClsidModule(string clsid)
        {
            if (string.IsNullOrWhiteSpace(clsid) || !clsid.StartsWith("{")) return string.Empty;

            try
            {
                using var clsidKey = Registry.ClassesRoot.OpenSubKey($@"CLSID\{clsid}\InprocServer32");
                if (clsidKey != null)
                {
                    var val = clsidKey.GetValue(null)?.ToString() ?? string.Empty;
                    return val.Trim('\"');
                }
            }
            catch { }

            return string.Empty;
        }

        /// <summary>
        /// Removes selected orphaned shell handlers with a backup.
        /// </summary>
        public static int RemoveShellHandlers(IEnumerable<ShellHandlerItem> handlers, string backupDirectory = null)
        {
            var targets = handlers?.Where(h => h.IsSelected && h.IsOrphaned).ToList() ?? new List<ShellHandlerItem>();
            if (!targets.Any()) return 0;

            int cleaned = 0;

            if (!string.IsNullOrEmpty(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
                var backupFile = Path.Combine(backupDirectory, $"ShellHandlers_Backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.reg");
                var fullKeys = targets.Select(t => $@"HKEY_CLASSES_ROOT\{t.RegistryKeyPath}").ToList();
                SafeRegistryEngine.ExportRegistryKeys(fullKeys, backupFile);
            }

            foreach (var h in targets)
            {
                try
                {
                    var parts = h.RegistryKeyPath.Split(new[] { '\\' }, 2);
                    if (parts.Length == 2)
                    {
                        using var parent = Registry.ClassesRoot.OpenSubKey(parts[0], true);
                        if (parent != null)
                        {
                            parent.DeleteSubKeyTree(parts[1], false);
                            cleaned++;
                            StructuredLogger.Info($"Removed orphaned context menu handler: {h.HandlerName}");
                        }
                    }
                }
                catch { }
            }

            return cleaned;
        }
    }
}
