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
    public class ShellHandlerAuditRecord
    {
        public string HandlerName { get; set; } = string.Empty;
        public string Clsid { get; set; } = string.Empty;
        public string ModulePath { get; set; } = string.Empty;
        public string ShellLocation { get; set; } = string.Empty;
        public bool FileExistsOnDisk { get; set; }
        public bool IsOrphaned => !FileExistsOnDisk;
        public bool IsSystemCritical { get; set; }
        public bool IsSelected { get; set; } = true;
    }

    /// <summary>
    /// Audits and repairs Windows Explorer Shell Extension Handlers to fix context menu lag and crashes.
    /// </summary>
    public static class ShellHandlerAuditEngine
    {
        private static readonly string[] ShellTargets =
        {
            @"*\shellex\ContextMenuHandlers",
            @"Directory\shellex\ContextMenuHandlers",
            @"Directory\Background\shellex\ContextMenuHandlers",
            @"Folder\shellex\ContextMenuHandlers",
            @"Drive\shellex\ContextMenuHandlers"
        };

        /// <summary>
        /// Scans all registered context menu shell extensions across ClassesRoot.
        /// </summary>
        public static List<ShellHandlerAuditRecord> ScanShellHandlers()
        {
            var list = new List<ShellHandlerAuditRecord>();

            foreach (var target in ShellTargets)
            {
                try
                {
                    using var key = Registry.ClassesRoot.OpenSubKey(target, false);
                    if (key == null) continue;

                    foreach (var subName in key.GetSubKeyNames())
                    {
                        try
                        {
                            using var subKey = key.OpenSubKey(subName);
                            if (subKey == null) continue;

                            var clsid = subKey.GetValue("")?.ToString() ?? string.Empty;
                            string modulePath = string.Empty;

                            if (!string.IsNullOrEmpty(clsid))
                            {
                                modulePath = ResolveClsidModule(clsid);
                            }

                            bool exists = string.IsNullOrEmpty(modulePath) || File.Exists(modulePath);
                            bool isSys = IsSystemExtension(subName, modulePath);

                            list.Add(new ShellHandlerAuditRecord
                            {
                                HandlerName = subName,
                                Clsid = clsid,
                                ModulePath = modulePath,
                                ShellLocation = target,
                                FileExistsOnDisk = exists,
                                IsSystemCritical = isSys
                            });
                        }
                        catch { }
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Error(LogCategory.Registry, "Failed to scan shell target " + target + ": " + ex.Message);
                }
            }

            return list.OrderBy(h => h.HandlerName).ToList();
        }

        private static string ResolveClsidModule(string clsid)
        {
            try
            {
                using var key = Registry.ClassesRoot.OpenSubKey(@"CLSID\" + clsid + @"\InProcServer32");
                if (key != null)
                {
                    var val = key.GetValue("")?.ToString() ?? string.Empty;
                    return Environment.ExpandEnvironmentVariables(val).Trim('\"');
                }
            }
            catch { }
            return string.Empty;
        }

        private static bool IsSystemExtension(string handlerName, string modulePath)
        {
            var lowerName = handlerName.ToLowerInvariant();
            var lowerPath = modulePath.ToLowerInvariant();

            if (lowerName.Contains("windows") || lowerName.Contains("microsoft") ||
                lowerPath.Contains("system32") || lowerPath.Contains("windows"))
            {
                return true;
            }

            return false;
        }
    }
}
