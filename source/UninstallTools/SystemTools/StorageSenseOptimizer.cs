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
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public class StorageSenseConfig
    {
        public bool IsStorageSenseEnabled { get; set; } = true;
        public int RecycleBinCleanupDays { get; set; } = 30;
        public int DownloadsCleanupDays { get; set; } = 0; // 0 = never
        public int Cadence { get; set; } = 1; // 1 = Every month, 7 = Every week, 0 = Low disk space
    }

    /// <summary>
    /// Configures Windows Storage Sense automated maintenance and temporary storage retention policies.
    /// </summary>
    public static class StorageSenseOptimizer
    {
        private const string StoragePolicyKey = @"Software\Microsoft\Windows\CurrentVersion\StorageSense\Parameters\StoragePolicy";

        /// <summary>
        /// Reads current Storage Sense policy settings from the registry.
        /// </summary>
        public static StorageSenseConfig GetStorageSensePolicy()
        {
            var config = new StorageSenseConfig();

            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(StoragePolicyKey, false);
                if (key != null)
                {
                    config.IsStorageSenseEnabled = Convert.ToInt32(key.GetValue("01", 1)) != 0;
                    config.RecycleBinCleanupDays = Convert.ToInt32(key.GetValue("04", 30));
                    config.DownloadsCleanupDays = Convert.ToInt32(key.GetValue("32", 0));
                    config.Cadence = Convert.ToInt32(key.GetValue("2048", 1));
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.SystemTools, "Failed to read Storage Sense policy: " + ex.Message);
            }

            return config;
        }

        /// <summary>
        /// Applies Storage Sense automated cleanup policies.
        /// </summary>
        public static bool SetStorageSensePolicy(StorageSenseConfig config)
        {
            if (config == null) return false;

            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(StoragePolicyKey);
                if (key != null)
                {
                    key.SetValue("01", config.IsStorageSenseEnabled ? 1 : 0, RegistryValueKind.DWord);
                    key.SetValue("04", config.RecycleBinCleanupDays, RegistryValueKind.DWord);
                    key.SetValue("32", config.DownloadsCleanupDays, RegistryValueKind.DWord);
                    key.SetValue("2048", config.Cadence, RegistryValueKind.DWord);

                    StructuredLogger.Info(LogCategory.SystemTools, "Updated Windows Storage Sense policy.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.SystemTools, "Failed to update Storage Sense policy: " + ex.Message);
            }

            return false;
        }
    }
}
