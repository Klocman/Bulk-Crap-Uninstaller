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
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UninstallTools.Core;

namespace UninstallTools.FileSystemEngine
{
    /// <summary>
    /// Wipe pattern for sanitizing unallocated disk sectors.
    /// </summary>
    public enum FreeSpaceWipePattern
    {
        ZeroFill, // 1-pass Zero Fill (Standard)
        RandomFill, // 1-pass Pseudo-Random Fill
        TrimOnly // SSD TRIM Optimization
    }

    /// <summary>
    /// Progress event arguments during free space wipe.
    /// </summary>
    public class WipeProgressEventArgs : EventArgs
    {
        public long BytesWiped { get; set; }
        public long TotalFreeBytes { get; set; }
        public int Percentage => TotalFreeBytes > 0 ? (int)((BytesWiped * 100) / TotalFreeBytes) : 0;
        public string StatusMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// Sanitizes unallocated free disk space to prevent data recovery of previously deleted residual files.
    /// </summary>
    public static class FreeSpaceWiper
    {
        /// <summary>
        /// Asynchronously wipes free space on a target drive.
        /// </summary>
        public static async Task<bool> WipeFreeSpaceAsync(
            string driveRoot,
            FreeSpaceWipePattern pattern,
            IProgress<WipeProgressEventArgs> progress,
            CancellationToken cancellationToken)
        {
            var driveInfo = new DriveInfo(driveRoot);
            if (!driveInfo.IsReady) return false;

            if (pattern == FreeSpaceWipePattern.TrimOnly)
            {
                return RunSsdTrim(driveInfo.Name.TrimEnd('\\'));
            }

            var tempDir = Path.Combine(driveInfo.RootDirectory.FullName, ".ebu_wipe_temp");
            var tempFile = Path.Combine(tempDir, "wipe_cluster.tmp");

            try
            {
                Directory.CreateDirectory(tempDir);
                var totalFree = driveInfo.AvailableFreeSpace;
                var bufferSize = 4 * 1024 * 1024; // 4 MB buffer
                var buffer = new byte[bufferSize];

                if (pattern == FreeSpaceWipePattern.RandomFill)
                {
                    new Random().NextBytes(buffer);
                }

                long totalWritten = 0;

                using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, FileOptions.WriteThrough))
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            await fs.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
                            totalWritten += buffer.Length;

                            progress?.Report(new WipeProgressEventArgs
                            {
                                BytesWiped = totalWritten,
                                TotalFreeBytes = totalFree,
                                StatusMessage = $"Sanitizing free space: {totalWritten / (1024.0 * 1024.0):F1} MB written..."
                            });
                        }
                        catch (IOException)
                        {
                            // Disk is completely full -> sectors sanitized
                            break;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error($"Free space wipe error: {ex.Message}");
                return false;
            }
            finally
            {
                try
                {
                    if (File.Exists(tempFile)) File.Delete(tempFile);
                    if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
                }
                catch { }
            }
        }

        private static bool RunSsdTrim(string driveLetter)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = Path.Combine(Environment.SystemDirectory, "defrag.exe"),
                    Arguments = $"{driveLetter} /L",
                    UseShellExecute = true,
                    Verb = "runas"
                };

                using var process = Process.Start(psi);
                process?.WaitForExit(30000);
                return process?.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
