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
using System.Runtime.InteropServices;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public class WinSxSReport
    {
        public double ActualSizeMB { get; set; }
        public double SharedWithWindowsMB { get; set; }
        public double BackupsAndDisabledFeaturesMB { get; set; }
        public int NumberOfSupersededPackages { get; set; }
        public bool ComponentCleanupRecommended { get; set; }
        public string RawOutput { get; set; } = string.Empty;
    }

    /// <summary>
    /// Audits and cleans the Windows Component Store (WinSxS) to safely reclaim space from superseded update packages.
    /// </summary>
    public static class WinSxSStoreAnalyzer
    {
        /// <summary>
        /// Analyzes the WinSxS component store using DISM.
        /// </summary>
        public static WinSxSReport AnalyzeComponentStore()
        {
            var report = new WinSxSReport();
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return report;

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "dism.exe"),
                    Arguments = "/Online /Cleanup-Image /AnalyzeComponentStore",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi);
                if (proc != null)
                {
                    string output = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit();
                    report.RawOutput = output;

                    var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        var lower = line.ToLowerInvariant();
                        if (lower.Contains("actual size of component store") && line.Contains(":"))
                        {
                            var parts = line.Split(':');
                            if (parts.Length > 1) ParseSize(parts[1], out var sizeMB);
                        }
                        else if (lower.Contains("component store cleanup recommended") && line.Contains(":"))
                        {
                            report.ComponentCleanupRecommended = lower.Contains("yes");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.SystemTools, "Failed to analyze WinSxS component store: " + ex.Message);
            }

            return report;
        }

        private static void ParseSize(string raw, out double sizeMB)
        {
            sizeMB = 0;
            if (string.IsNullOrWhiteSpace(raw)) return;

            var clean = raw.Trim().Replace("GB", "").Replace("MB", "").Trim();
            if (double.TryParse(clean, out var val))
            {
                if (raw.Contains("GB")) sizeMB = val * 1024.0;
                else sizeMB = val;
            }
        }

        /// <summary>
        /// Runs DISM StartComponentCleanup to remove superseded versions of component updates.
        /// </summary>
        public static bool RunComponentCleanup(bool resetBase = false)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return false;

            try
            {
                string args = "/Online /Cleanup-Image /StartComponentCleanup";
                if (resetBase)
                {
                    args = args + " /ResetBase";
                }

                var psi = new ProcessStartInfo
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "dism.exe"),
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi);
                if (proc != null)
                {
                    proc.WaitForExit();
                    StructuredLogger.Info(LogCategory.SystemTools, "Executed DISM StartComponentCleanup. ExitCode: " + proc.ExitCode);
                    return proc.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.SystemTools, "Failed to execute StartComponentCleanup: " + ex.Message);
            }

            return false;
        }
    }
}
