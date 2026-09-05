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
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.Startup
{
    public class DelayedStartupItem
    {
        public string EntryName { get; set; } = string.Empty;
        public string ExecutablePath { get; set; } = string.Empty;
        public string Arguments { get; set; } = string.Empty;
        public int DelaySeconds { get; set; } = 30;
        public bool IsEnabled { get; set; } = true;
        public DateTime ConfiguredDateUtc { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Configures delayed startup triggers for non-critical startup applications
    /// to eliminate login boot freezes and boost initial desktop responsiveness.
    /// </summary>
    public static class StartupDelayOptimizer
    {
        private const string DelayKeyPath = @"Software\EBUninstallerPro\DelayedStartup";

        /// <summary>
        /// Retrieves all applications currently configured for delayed startup.
        /// </summary>
        public static List<DelayedStartupItem> GetDelayedStartupItems()
        {
            var list = new List<DelayedStartupItem>();

            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(DelayKeyPath, false);
                if (key == null) return list;

                foreach (var subName in key.GetSubKeyNames())
                {
                    try
                    {
                        using var subKey = key.OpenSubKey(subName);
                        if (subKey == null) continue;

                        var exe = subKey.GetValue("ExecutablePath")?.ToString() ?? string.Empty;
                        var args = subKey.GetValue("Arguments")?.ToString() ?? string.Empty;
                        var delay = Convert.ToInt32(subKey.GetValue("DelaySeconds", 30));
                        var enabled = Convert.ToBoolean(subKey.GetValue("IsEnabled", true));

                        list.Add(new DelayedStartupItem
                        {
                            EntryName = subName,
                            ExecutablePath = exe,
                            Arguments = args,
                            DelaySeconds = delay,
                            IsEnabled = enabled
                        });
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.General, $"Failed to query delayed startup items: {ex.Message}");
            }

            return list;
        }

        /// <summary>
        /// Configures an application to launch with a staggered delay after Windows login.
        /// </summary>
        public static bool ConfigureDelay(string entryName, string executablePath, string arguments, int delaySeconds = 30)
        {
            if (string.IsNullOrWhiteSpace(entryName) || string.IsNullOrWhiteSpace(executablePath))
                return false;

            try
            {
                using (var rootKey = Registry.CurrentUser.CreateSubKey(DelayKeyPath))
                using (var itemKey = rootKey?.CreateSubKey(entryName))
                {
                    if (itemKey == null) return false;

                    itemKey.SetValue("ExecutablePath", executablePath);
                    itemKey.SetValue("Arguments", arguments ?? string.Empty);
                    itemKey.SetValue("DelaySeconds", delaySeconds);
                    itemKey.SetValue("IsEnabled", true);
                    itemKey.SetValue("ConfiguredDateUtc", DateTime.UtcNow.ToString("o"));
                }

                // Register Scheduled Task with Delay Trigger
                CreateScheduledTaskDelay(entryName, executablePath, arguments, delaySeconds);

                StructuredLogger.Info(LogCategory.General, $"Configured startup delay ({delaySeconds}s) for {entryName}");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.General, $"Failed to configure startup delay for {entryName}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Removes delayed startup configuration and scheduled task wrapper.
        /// </summary>
        public static bool RemoveDelay(string entryName)
        {
            if (string.IsNullOrWhiteSpace(entryName)) return false;

            try
            {
                using var rootKey = Registry.CurrentUser.OpenSubKey(DelayKeyPath, true);
                rootKey?.DeleteSubKeyTree(entryName, false);

                RemoveScheduledTask(entryName);

                StructuredLogger.Info(LogCategory.General, $"Removed startup delay configuration for {entryName}");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.General, $"Failed to remove startup delay for {entryName}", ex.Message);
                return false;
            }
        }

        private static void CreateScheduledTaskDelay(string taskName, string exePath, string args, int delaySeconds)
        {
            try
            {
                var safeTaskName = "EBUninstaller_Delay_" + Regex.Replace(taskName, @"[^a-zA-Z0-9_-]", "");
                var delayMinutes = Math.Max(1, delaySeconds / 60);

                var psi = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = $"/Create /TN \"{safeTaskName}\" /TR \"\\\"{exePath}\\\" {args}\" /SC ONLOGON /DELAY 0000:{delaySeconds:D2} /F /RL LIMITED",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                Process.Start(psi)?.WaitForExit(3000);
            }
            catch { }
        }

        private static void RemoveScheduledTask(string taskName)
        {
            try
            {
                var safeTaskName = "EBUninstaller_Delay_" + Regex.Replace(taskName, @"[^a-zA-Z0-9_-]", "");
                var psi = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = $"/Delete /TN \"{safeTaskName}\" /F",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                Process.Start(psi)?.WaitForExit(3000);
            }
            catch { }
        }
    }
}
