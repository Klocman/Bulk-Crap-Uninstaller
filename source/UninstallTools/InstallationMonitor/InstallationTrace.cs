/*
    EBUninstaller Pro - Open Source Professional Windows Uninstaller
    Installation Monitor Models and Trace Definitions
*/

using System;
using System.Collections.Generic;

namespace UninstallTools.InstallationMonitor
{
    public enum TraceItemChangeType
    {
        Added,
        Modified,
        Removed
    }

    public enum TraceItemCategory
    {
        File,
        Directory,
        RegistryKey,
        RegistryValue,
        Service,
        ScheduledTask,
        StartupEntry,
        Shortcut,
        EnvironmentVariable
    }

    public sealed class TraceItem
    {
        public TraceItemCategory Category { get; set; }
        public TraceItemChangeType ChangeType { get; set; }
        public string PathOrIdentifier { get; set; }
        public string ValueName { get; set; }
        public string OldData { get; set; }
        public string NewData { get; set; }
        public long Size { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public override string ToString()
        {
            return $"[{ChangeType}] [{Category}] {PathOrIdentifier}" +
                   (string.IsNullOrEmpty(ValueName) ? "" : $" -> {ValueName}");
        }
    }

    public sealed class InstallationSnapshot
    {
        public string SnapshotId { get; set; } = Guid.NewGuid().ToString("N");
        public DateTime CapturedAt { get; set; } = DateTime.UtcNow;
        public string Description { get; set; }

        public Dictionary<string, long> FileMetadata { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> Directories { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> RegistryKeys { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> RegistryValues { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> Services { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> ScheduledTasks { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> StartupEntries { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> EnvironmentVariables { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }

    public sealed class InstallationTrace
    {
        public string TraceId { get; set; } = Guid.NewGuid().ToString("N");
        public int Version { get; set; } = 1;
        public string ApplicationName { get; set; }
        public string InstallerExecutablePath { get; set; }
        public DateTime MonitoringStartedAt { get; set; } = DateTime.UtcNow;
        public DateTime MonitoringStoppedAt { get; set; } = DateTime.UtcNow;
        public TimeSpan Duration => MonitoringStoppedAt - MonitoringStartedAt;

        public List<TraceItem> Items { get; set; } = new();

        public int TotalChangesCount => Items.Count;
    }

    public sealed class SnapshotDiffResult
    {
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public string BeforeSnapshotId { get; set; }
        public string AfterSnapshotId { get; set; }
        public List<TraceItem> AddedItems { get; } = new();
        public List<TraceItem> ModifiedItems { get; } = new();
        public List<TraceItem> RemovedItems { get; } = new();

        public int TotalDiffCount => AddedItems.Count + ModifiedItems.Count + RemovedItems.Count;
    }
}
