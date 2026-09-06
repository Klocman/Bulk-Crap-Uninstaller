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
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.Detection
{
    public class UserSoftwareEntry
    {
        public string UserSid { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string InstallLocation { get; set; } = string.Empty;
    }

    /// <summary>
    /// Audits and differentiates per-user software installations across multiple Windows user profiles.
    /// </summary>
    public static class MultiUserSoftwareMatrixEngine
    {
        /// <summary>
        /// Scans all user profile registry hives mounted in HKEY_USERS.
        /// </summary>
        public static List<UserSoftwareEntry> ScanAllUserProfiles()
        {
            var list = new List<UserSoftwareEntry>();

            try
            {
                using var usersKey = Registry.Users;
                foreach (var sid in usersKey.GetSubKeyNames())
                {
                    if (sid.StartsWith("S-1-5-21-") && !sid.EndsWith("_Classes"))
                    {
                        var path = sid + @"\Software\Microsoft\Windows\CurrentVersion\Uninstall";
                        ScanSubKey(usersKey, path, sid, list);
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.Detection, "Failed to scan multi-user software matrix: " + ex.Message);
            }

            return list.OrderBy(u => u.ApplicationName).ToList();
        }

        private static void ScanSubKey(RegistryKey rootKey, string subKeyPath, string sid, List<UserSoftwareEntry> list)
        {
            try
            {
                using var key = rootKey.OpenSubKey(subKeyPath, false);
                if (key == null) return;

                foreach (var appSub in key.GetSubKeyNames())
                {
                    try
                    {
                        using var appKey = key.OpenSubKey(appSub);
                        if (appKey == null) continue;

                        var name = appKey.GetValue("DisplayName")?.ToString();
                        if (string.IsNullOrWhiteSpace(name)) continue;

                        var pub = appKey.GetValue("Publisher")?.ToString() ?? "Unknown";
                        var ver = appKey.GetValue("DisplayVersion")?.ToString() ?? "Unknown";
                        var loc = appKey.GetValue("InstallLocation")?.ToString() ?? string.Empty;

                        list.Add(new UserSoftwareEntry
                        {
                            UserSid = sid,
                            ApplicationName = name,
                            Publisher = pub,
                            Version = ver,
                            InstallLocation = loc
                        });
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}
