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
using System.Runtime.InteropServices;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.FileSystemEngine
{
    public class PendingBootDeletionItem
    {
        public string SourcePath { get; set; } = string.Empty;
        public string DestinationPath { get; set; } = string.Empty;
        public bool IsDeleteOperation => string.IsNullOrEmpty(DestinationPath);
        public bool FileExistsOnDisk { get; set; }
        public long FileSizeBytes { get; set; }
    }

    /// <summary>
    /// Manages boot-time locked file deletions via Win32 MoveFileEx (MOVEFILE_DELAY_UNTIL_REBOOT)
    /// and the PendingFileRenameOperations registry subkey.
    /// </summary>
    public static class BootTimeDeleterEngine
    {
        private const string SessionManagerKey = @"SYSTEM\CurrentControlSet\Control\Session Manager";
        private const string PendingOperationsValue = "PendingFileRenameOperations";

        [Flags]
        private enum MoveFileFlags : uint
        {
            MOVEFILE_REPLACE_EXISTING = 0x00000001,
            MOVEFILE_COPY_ALLOWED = 0x00000002,
            MOVEFILE_DELAY_UNTIL_REBOOT = 0x00000004,
            MOVEFILE_WRITE_THROUGH = 0x00000008
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, MoveFileFlags dwFlags);

        /// <summary>
        /// Schedules an in-use or locked file for automatic deletion on next Windows reboot.
        /// </summary>
        public static bool ScheduleFileForBootDeletion(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return false;

            if (SecurityGuard.IsCriticalPath(filePath))
            {
                StructuredLogger.Warning(LogCategory.FileSystem, $"Blocked attempt to schedule boot deletion on protected path: {filePath}");
                return false;
            }

            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    bool result = MoveFileEx(filePath, null, MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT);
                    if (result)
                    {
                        StructuredLogger.Info(LogCategory.FileSystem, $"Scheduled file for boot-time deletion: {filePath}");
                        return true;
                    }
                }

                // Fallback registry manipulation
                return AddPendingOperationToRegistry(filePath, null);
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.FileSystem, $"Failed to schedule boot deletion for {filePath}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Retrieves all files and directories currently scheduled for boot-time rename or deletion.
        /// </summary>
        public static List<PendingBootDeletionItem> GetPendingBootDeletions()
        {
            var list = new List<PendingBootDeletionItem>();

            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(SessionManagerKey, false);
                if (key == null) return list;

                var value = key.GetValue(PendingOperationsValue) as string[];
                if (value == null || value.Length == 0) return list;

                for (int i = 0; i < value.Length; i += 2)
                {
                    var src = value[i]?.TrimStart('\\', '?') ?? string.Empty;
                    var dst = (i + 1 < value.Length) ? value[i + 1]?.TrimStart('\\', '?') : string.Empty;

                    bool exists = File.Exists(src) || Directory.Exists(src);
                    long size = 0;
                    if (File.Exists(src))
                    {
                        try { size = new FileInfo(src).Length; } catch { }
                    }

                    list.Add(new PendingBootDeletionItem
                    {
                        SourcePath = src,
                        DestinationPath = dst,
                        FileExistsOnDisk = exists,
                        FileSizeBytes = size
                    });
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.FileSystem, $"Failed to read PendingFileRenameOperations: {ex.Message}");
            }

            return list;
        }

        /// <summary>
        /// Cancels a scheduled boot-time deletion for a specific path.
        /// </summary>
        public static bool CancelBootDeletion(string targetPath)
        {
            if (string.IsNullOrWhiteSpace(targetPath)) return false;

            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(SessionManagerKey, true);
                if (key == null) return false;

                var value = key.GetValue(PendingOperationsValue) as string[];
                if (value == null || value.Length == 0) return true;

                var newList = new List<string>();
                for (int i = 0; i < value.Length; i += 2)
                {
                    var src = value[i];
                    var dst = (i + 1 < value.Length) ? value[i + 1] : string.Empty;

                    if (!src.EndsWith(targetPath, StringComparison.OrdinalIgnoreCase))
                    {
                        newList.Add(src);
                        newList.Add(dst);
                    }
                }

                if (newList.Count > 0)
                {
                    key.SetValue(PendingOperationsValue, newList.ToArray(), RegistryValueKind.MultiString);
                }
                else
                {
                    key.DeleteValue(PendingOperationsValue, false);
                }

                StructuredLogger.Info(LogCategory.FileSystem, $"Canceled pending boot deletion for: {targetPath}");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.FileSystem, $"Failed to cancel boot deletion for {targetPath}: {ex.Message}");
                return false;
            }
        }

        private static bool AddPendingOperationToRegistry(string srcPath, string dstPath)
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(SessionManagerKey, true);
                if (key == null) return false;

                var existing = key.GetValue(PendingOperationsValue) as string[] ?? Array.Empty<string>();
                var formattedSrc = @"\??\" + srcPath;
                var formattedDst = string.IsNullOrEmpty(dstPath) ? "" : @"\??\" + dstPath;

                var updated = existing.Concat(new[] { formattedSrc, formattedDst }).ToArray();
                key.SetValue(PendingOperationsValue, updated, RegistryValueKind.MultiString);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
