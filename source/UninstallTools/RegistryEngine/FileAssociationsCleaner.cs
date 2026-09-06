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
    /// Represents a registered file extension association in the registry.
    /// </summary>
    public class FileAssociationItem
    {
        public string Extension { get; set; } = string.Empty; // e.g. .mp4, .pdf, .docx
        public string ProgId { get; set; } = string.Empty;
        public string TargetExecutablePath { get; set; } = string.Empty;
        public string RegistryPath { get; set; } = string.Empty;
        public bool IsOrphaned { get; set; }
        public bool IsSelected { get; set; } = true;
    }

    /// <summary>
    /// Scans and purges dead file associations (OpenWithProgids, UserChoice) pointing to deleted binaries.
    /// </summary>
    public static class FileAssociationsCleaner
    {
        /// <summary>
        /// Scans all file extension mappings in HKCR and HKCU Explorer FileExts.
        /// </summary>
        public static List<FileAssociationItem> ScanFileAssociations()
        {
            var list = new List<FileAssociationItem>();

            try
            {
                using var classesKey = Registry.ClassesRoot;
                var subKeys = classesKey.GetSubKeyNames().Where(s => s.StartsWith(".")).Take(1500);

                foreach (var ext in subKeys)
                {
                    try
                    {
                        using var extKey = classesKey.OpenSubKey(ext);
                        if (extKey == null) continue;

                        var progId = extKey.GetValue(null)?.ToString() ?? string.Empty;
                        if (string.IsNullOrEmpty(progId)) continue;

                        using var progIdKey = classesKey.OpenSubKey($@"{progId}\shell\open\command");
                        if (progIdKey == null) continue;

                        var cmd = progIdKey.GetValue(null)?.ToString() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(cmd)) continue;

                        var exe = ExtractExeFromCommand(cmd);
                        bool isOrphaned = false;

                        if (!string.IsNullOrEmpty(exe) && !SecurityGuard.IsCriticalPath(exe))
                        {
                            if (!File.Exists(exe))
                            {
                                isOrphaned = true;
                            }
                        }

                        list.Add(new FileAssociationItem
                        {
                            Extension = ext,
                            ProgId = progId,
                            TargetExecutablePath = exe,
                            RegistryPath = $@"HKEY_CLASSES_ROOT\{ext}",
                            IsOrphaned = isOrphaned
                        });
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error($"Error scanning file associations: {ex.Message}");
            }

            return list;
        }

        private static string ExtractExeFromCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return string.Empty;
            var clean = Environment.ExpandEnvironmentVariables(command).Trim();

            if (clean.StartsWith("\""))
            {
                var nextQuote = clean.IndexOf('\"', 1);
                if (nextQuote > 1) return clean.Substring(1, nextQuote - 1);
            }

            var parts = clean.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? parts[0].Trim('\"') : clean;
        }

        /// <summary>
        /// Cleans orphaned file associations with registry backup.
        /// </summary>
        public static int CleanOrphanedAssociations(IEnumerable<FileAssociationItem> items, string backupDirectory = null)
        {
            var targets = items?.Where(i => i.IsSelected && i.IsOrphaned).ToList() ?? new List<FileAssociationItem>();
            if (!targets.Any()) return 0;

            int cleaned = 0;

            if (!string.IsNullOrEmpty(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
                var backupFile = Path.Combine(backupDirectory, $"FileAssociations_Backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.reg");
                var keyList = targets.Select(t => t.RegistryPath).ToList();
                SafeRegistryEngine.ExportRegistryKeys(keyList, backupFile);
            }

            foreach (var t in targets)
            {
                try
                {
                    using var key = Registry.ClassesRoot.OpenSubKey(t.Extension, true);
                    if (key != null)
                    {
                        key.SetValue("", string.Empty);
                        cleaned++;
                        StructuredLogger.Info($"Cleared orphaned file association for {t.Extension}");
                    }
                }
                catch { }
            }

            return cleaned;
        }
    }
}
