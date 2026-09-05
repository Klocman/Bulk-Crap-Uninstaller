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
using System.Text.Json;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.Startup
{
    public class QuarantinedServiceRecord
    {
        public string ServiceName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int StartType { get; set; } = 3; // Manual by default
        public int ServiceType { get; set; } = 16; // Win32OwnProcess
        public string ObjectName { get; set; } = "LocalSystem";
        public DateTime QuarantineDateUtc { get; set; } = DateTime.UtcNow;
        public string Reason { get; set; } = "Disabled or removed during system optimization";
    }

    /// <summary>
    /// Safely captures service configurations into an isolated quarantine vault
    /// allowing instant one-click restoration of any disabled or deleted background service.
    /// </summary>
    public static class ServiceQuarantineEngine
    {
        private const string ServicesRegistryKey = @"SYSTEM\CurrentControlSet\Services";
        private static readonly string QuarantineDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "EBUninstallerPro", "Quarantine", "Services");

        /// <summary>
        /// Quarantines a service's configuration and metadata before modification or deletion.
        /// </summary>
        public static bool QuarantineService(string serviceName, string reason = null)
        {
            if (string.IsNullOrWhiteSpace(serviceName)) return false;

            try
            {
                using var root = Registry.LocalMachine.OpenSubKey($@"{ServicesRegistryKey}\{serviceName}", false);
                if (root == null) return false;

                var record = new QuarantinedServiceRecord
                {
                    ServiceName = serviceName,
                    DisplayName = root.GetValue("DisplayName")?.ToString() ?? serviceName,
                    ImagePath = root.GetValue("ImagePath")?.ToString() ?? string.Empty,
                    Description = root.GetValue("Description")?.ToString() ?? string.Empty,
                    StartType = Convert.ToInt32(root.GetValue("Start", 3)),
                    ServiceType = Convert.ToInt32(root.GetValue("Type", 16)),
                    ObjectName = root.GetValue("ObjectName")?.ToString() ?? "LocalSystem",
                    QuarantineDateUtc = DateTime.UtcNow,
                    Reason = reason ?? "Quarantined by EBUninstaller Pro"
                };

                if (!Directory.Exists(QuarantineDir))
                    Directory.CreateDirectory(QuarantineDir);

                var filePath = Path.Combine(QuarantineDir, $"{serviceName}.json");
                var json = JsonSerializer.Serialize(record, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json, System.Text.Encoding.UTF8);

                StructuredLogger.Info(LogCategory.Startup, $"Service '{serviceName}' quarantined to {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.Startup, $"Failed to quarantine service '{serviceName}'", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Lists all services currently stored in the quarantine repository.
        /// </summary>
        public static List<QuarantinedServiceRecord> ListQuarantinedServices()
        {
            var list = new List<QuarantinedServiceRecord>();
            if (!Directory.Exists(QuarantineDir)) return list;

            try
            {
                foreach (var file in Directory.GetFiles(QuarantineDir, "*.json"))
                {
                    try
                    {
                        var json = File.ReadAllText(file, System.Text.Encoding.UTF8);
                        var record = JsonSerializer.Deserialize<QuarantinedServiceRecord>(json);
                        if (record != null) list.Add(record);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.Startup, "Failed to read service quarantine directory", ex.Message);
            }

            return list.OrderByDescending(r => r.QuarantineDateUtc).ToList();
        }

        /// <summary>
        /// Restores a quarantined service back into the Windows Services registry hive.
        /// </summary>
        public static bool RestoreService(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName)) return false;

            var filePath = Path.Combine(QuarantineDir, $"{serviceName}.json");
            if (!File.Exists(filePath)) return false;

            try
            {
                var json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
                var record = JsonSerializer.Deserialize<QuarantinedServiceRecord>(json);
                if (record == null) return false;

                using (var svcKey = Registry.LocalMachine.CreateSubKey($@"{ServicesRegistryKey}\{record.ServiceName}"))
                {
                    if (svcKey == null) return false;

                    svcKey.SetValue("DisplayName", record.DisplayName);
                    svcKey.SetValue("ImagePath", record.ImagePath, RegistryValueKind.ExpandString);
                    if (!string.IsNullOrEmpty(record.Description))
                        svcKey.SetValue("Description", record.Description);

                    svcKey.SetValue("Start", record.StartType, RegistryValueKind.DWord);
                    svcKey.SetValue("Type", record.ServiceType, RegistryValueKind.DWord);
                    svcKey.SetValue("ObjectName", record.ObjectName);
                }

                File.Delete(filePath);
                StructuredLogger.Info(LogCategory.Startup, $"Service '{serviceName}' restored successfully from quarantine.");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.Startup, $"Failed to restore service '{serviceName}'", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Permanently purges a quarantined service snapshot.
        /// </summary>
        public static bool DeleteQuarantineRecord(string serviceName)
        {
            var filePath = Path.Combine(QuarantineDir, $"{serviceName}.json");
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    return true;
                }
                catch { }
            }
            return false;
        }
    }
}
