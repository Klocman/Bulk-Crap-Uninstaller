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
using System.Text;
using System.Text.RegularExpressions;
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    /// <summary>
    /// Represents a broken desktop or start menu shortcut.
    /// </summary>
    public class BrokenShortcutItem
    {
        public string ShortcutName { get; set; } = string.Empty;
        public string ShortcutPath { get; set; } = string.Empty;
        public string TargetPath { get; set; } = string.Empty;
        public string LocationCategory { get; set; } = string.Empty;
        public bool IsBroken { get; set; }
        public bool IsSelected { get; set; } = true;
    }

    /// <summary>
    /// Scans Desktop, Start Menu, Quick Launch, and Taskbar pinning folders for dead or broken .lnk and .url shortcuts.
    /// </summary>
    public static class ShortcutResidualsCleaner
    {
        /// <summary>
        /// Scans all standard shortcut locations across system and user profiles.
        /// </summary>
        public static List<BrokenShortcutItem> ScanBrokenShortcuts()
        {
            var list = new List<BrokenShortcutItem>();

            var locations = new[]
            {
                new { Cat = "Start Menu (All Users)", Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs") },
                new { Cat = "Start Menu (Current User)", Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs") },
                new { Cat = "Desktop (Public)", Path = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) },
                new { Cat = "Desktop (User)", Path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) },
                new { Cat = "Quick Launch", Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Microsoft\Internet Explorer\Quick Launch") }
            };

            foreach (var loc in locations)
            {
                if (!Directory.Exists(loc.Path)) continue;

                try
                {
                    var files = Directory.GetFiles(loc.Path, "*.lnk", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        var target = ResolveLnkTarget(file);
                        bool isBroken = false;

                        if (!string.IsNullOrEmpty(target) && !SecurityGuard.IsCriticalPath(target))
                        {
                            if (!File.Exists(target) && !Directory.Exists(target))
                            {
                                isBroken = true;
                            }
                        }

                        if (isBroken)
                        {
                            list.Add(new BrokenShortcutItem
                            {
                                ShortcutName = Path.GetFileNameWithoutExtension(file),
                                ShortcutPath = file,
                                TargetPath = target,
                                LocationCategory = loc.Cat,
                                IsBroken = true
                            });
                        }
                    }
                }
                catch { }
            }

            return list;
        }

        /// <summary>
        /// Resolves the target executable path from a .lnk binary file without COM dependency.
        /// </summary>
        public static string ResolveLnkTarget(string file)
        {
            if (string.IsNullOrWhiteSpace(file) || !File.Exists(file)) return string.Empty;

            try
            {
                var bytes = File.ReadAllBytes(file);
                if (bytes.Length < 0x60) return string.Empty;

                // Simple LNK binary target heuristic: Search for drive-letter rooted path (e.g. C:\...)
                var ascii = Encoding.ASCII.GetString(bytes);
                var match = Regex.Match(ascii, @"[a-zA-Z]:\\[^/\:\*\?""<>\|\x00-\x1F]+\.(?:exe|bat|cmd|dll|msc)", RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    return match.Value;
                }
            }
            catch { }

            return string.Empty;
        }

        /// <summary>
        /// Deletes the selected broken shortcuts safely.
        /// </summary>
        public static int CleanShortcuts(IEnumerable<BrokenShortcutItem> shortcuts)
        {
            var targets = shortcuts?.Where(s => s.IsSelected && s.IsBroken).ToList() ?? new List<BrokenShortcutItem>();
            int deleted = 0;

            foreach (var s in targets)
            {
                try
                {
                    if (File.Exists(s.ShortcutPath))
                    {
                        File.SetAttributes(s.ShortcutPath, FileAttributes.Normal);
                        File.Delete(s.ShortcutPath);
                        deleted++;
                        StructuredLogger.Info($"Deleted broken shortcut: {s.ShortcutPath}");
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Error($"Cannot delete shortcut {s.ShortcutPath}: {ex.Message}");
                }
            }

            return deleted;
        }
    }
}
