/*
    EBUninstaller Pro - Windows Event Log & Diagnostic Residuals Cleaner
    Auditing, record inspection, and safe purging of cluttered Windows event logs.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    public enum EventLogCategory
    {
        Application,
        Setup,
        System,
        DiagnosticTrace,
        Other
    }

    public class EventLogItem
    {
        public string LogName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public EventLogCategory Category { get; set; } = EventLogCategory.Other;
        public long RecordCount { get; set; }
        public long SizeBytes { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsCriticalProtected { get; set; }
        public bool IsSelected { get; set; } = true;
    }

    public static class EventLogResidualsCleaner
    {
        private static readonly HashSet<string> ProtectedLogs = new(StringComparer.OrdinalIgnoreCase)
        {
            "Security",
            "Microsoft-Windows-WindowsDefend/Operational",
            "Microsoft-Windows-BitLocker/BitLocker Management"
        };

        public static List<EventLogItem> ScanEventLogs(Action<string>? onProgress = null)
        {
            var results = new List<EventLogItem>();
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return results;

            onProgress?.Invoke("Enumerating standard Windows Event Logs...");
            try
            {
                var standardLogs = EventLog.GetEventLogs();
                foreach (var log in standardLogs)
                {
                    try
                    {
                        string name = log.LogDisplayName;
                        bool isCritical = ProtectedLogs.Contains(log.Log);
                        long count = 0;
                        try { count = log.Entries.Count; } catch { }

                        EventLogCategory cat = log.Log.ToLowerInvariant() switch
                        {
                            "application" => EventLogCategory.Application,
                            "setup" => EventLogCategory.Setup,
                            "system" => EventLogCategory.System,
                            _ => EventLogCategory.Other
                        };

                        results.Add(new EventLogItem
                        {
                            LogName = log.Log,
                            DisplayName = name,
                            Category = cat,
                            RecordCount = count,
                            IsCriticalProtected = isCritical,
                            IsSelected = !isCritical && count > 0
                        });
                    }
                    catch (Exception ex)
                    {
                        StructuredLogger.Log(LogLevel.Warning, "EventLogResidualsCleaner", $"Error reading event log {log.Log}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Warning, "EventLogResidualsCleaner", $"Error querying event logs: {ex.Message}");
            }

            // Also scan ETW log files in System32\winevt\Logs
            onProgress?.Invoke("Scanning winevt log files...");
            try
            {
                string winevtDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"System32\winevt\Logs");
                if (Directory.Exists(winevtDir))
                {
                    var existingNames = new HashSet<string>(results.Select(r => r.LogName), StringComparer.OrdinalIgnoreCase);
                    var dirInfo = new DirectoryInfo(winevtDir);

                    foreach (var file in dirInfo.GetFiles("*.evtx"))
                    {
                        string logName = Path.GetFileNameWithoutExtension(file.Name).Replace("%4", "/");
                        if (!existingNames.Contains(logName))
                        {
                            bool isProtected = ProtectedLogs.Contains(logName);
                            results.Add(new EventLogItem
                            {
                                LogName = logName,
                                DisplayName = file.Name,
                                Category = EventLogCategory.DiagnosticTrace,
                                SizeBytes = file.Length,
                                LastModified = file.LastWriteTimeUtc,
                                IsCriticalProtected = isProtected,
                                IsSelected = !isProtected
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Warning, "EventLogResidualsCleaner", $"Error scanning winevt directory: {ex.Message}");
            }

            return results;
        }

        public static (int clearedCount, long totalRecordsCleared) ClearEventLogs(IEnumerable<EventLogItem> items, Action<string>? onProgress = null)
        {
            int cleared = 0;
            long records = 0;

            foreach (var item in items)
            {
                if (!item.IsSelected || item.IsCriticalProtected)
                    continue;

                onProgress?.Invoke($"Clearing log: {item.DisplayName}");
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "wevtutil.exe"),
                        Arguments = $"cl \"{item.LogName}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using var proc = Process.Start(psi);
                    if (proc != null)
                    {
                        proc.WaitForExit();
                        if (proc.ExitCode == 0)
                        {
                            cleared++;
                            records += item.RecordCount;
                        }
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Log(LogLevel.Warning, "EventLogResidualsCleaner", $"Failed to clear event log {item.LogName}: {ex.Message}");
                }
            }

            return (cleared, records);
        }
    }
}
