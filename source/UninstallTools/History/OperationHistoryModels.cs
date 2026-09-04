/*
    OpenUninstall Pro - Open Source Professional Windows Uninstaller
    Operation History Models
*/

using System;
using System.Collections.Generic;

namespace UninstallTools.History
{
    public enum HistoryOperationStatus
    {
        Success,
        Partial,
        Failed
    }

    public sealed class OperationHistoryEntry
    {
        public string HistoryId { get; set; } = Guid.NewGuid().ToString("N");
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string ApplicationName { get; set; }
        public string Publisher { get; set; }
        public string OperationType { get; set; } // "Uninstall", "ForcedRemoval", "JunkClean", etc.
        public HistoryOperationStatus Status { get; set; } = HistoryOperationStatus.Success;
        public int DetectedItemsCount { get; set; }
        public int DeletedItemsCount { get; set; }
        public int FailedItemsCount { get; set; }
        public int RestoredItemsCount { get; set; }
        public string BackupId { get; set; }
        public List<string> RemovedItems { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public List<string> Errors { get; set; } = new();

        public override string ToString() => $"[{Timestamp:yyyy-MM-dd HH:mm}] [{Status}] {OperationType} - {ApplicationName}";
    }
}
