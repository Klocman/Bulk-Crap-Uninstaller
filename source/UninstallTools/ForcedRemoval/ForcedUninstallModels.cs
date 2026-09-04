/*
    OpenUninstall Pro - Open Source Professional Windows Uninstaller
    Forced Removal Models
*/

using System;
using System.Collections.Generic;

namespace UninstallTools.ForcedRemoval
{
    public enum ForcedRemovalConfidence
    {
        High,
        Medium,
        Low
    }

    public enum ForcedRemovalItemType
    {
        File,
        Directory,
        RegistryKey,
        RegistryValue,
        Service,
        ScheduledTask,
        StartupEntry,
        Shortcut
    }

    public sealed class ForcedRemovalItem
    {
        public ForcedRemovalItemType ItemType { get; set; }
        public string PathOrKey { get; set; }
        public string ValueName { get; set; }
        public string Description { get; set; }
        public ForcedRemovalConfidence Confidence { get; set; } = ForcedRemovalConfidence.Medium;
        public int ConfidenceScore { get; set; }
        public long Size { get; set; }
        public bool IsSelected { get; set; } = true;
        public string MatchReason { get; set; }

        public override string ToString()
        {
            return $"[{Confidence}] [{ItemType}] {PathOrKey}" +
                   (string.IsNullOrEmpty(ValueName) ? "" : $" -> {ValueName}") +
                   $" (Reason: {MatchReason})";
        }
    }

    public sealed class ForcedRemovalPlan
    {
        public string PlanId { get; set; } = Guid.NewGuid().ToString("N");
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public string SearchQuery { get; set; }
        public string TargetInstallLocation { get; set; }
        public List<ForcedRemovalItem> Items { get; } = new();

        public int HighConfidenceCount => Items.FindAll(i => i.Confidence == ForcedRemovalConfidence.High).Count;
        public int MediumConfidenceCount => Items.FindAll(i => i.Confidence == ForcedRemovalConfidence.Medium).Count;
        public int LowConfidenceCount => Items.FindAll(i => i.Confidence == ForcedRemovalConfidence.Low).Count;
        public long TotalSizeBytes => Items.ConvertAll(i => i.Size).Sum();
    }

    public sealed class ForcedRemovalExecutionResult
    {
        public int RemovedItemsCount { get; set; }
        public int FailedItemsCount { get; set; }
        public int BlockedCount { get; set; }
        public string BackupId { get; set; }
        public List<string> RemovedItemDetails { get; } = new();
        public List<string> Errors { get; } = new();
        public bool Success => FailedItemsCount == 0;
    }
}
