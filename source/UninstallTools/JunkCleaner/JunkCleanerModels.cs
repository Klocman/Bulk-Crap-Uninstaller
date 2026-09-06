/*
    EBUninstaller Pro - Open Source Professional Windows Uninstaller
    Junk Cleaner Models
*/

using System;
using System.Collections.Generic;

namespace UninstallTools.JunkCleaner
{
    public enum JunkCategoryType
    {
        WindowsTemp,
        UserTemp,
        WindowsUpdateDownloadCache,
        CrashDumps,
        SystemLogs,
        ThumbnailCache,
        BrowserCacheChrome,
        BrowserCacheEdge,
        BrowserCacheFirefox,
        BrowserCacheBrave,
        BrowserCacheOpera,
        ApplicationCaches,
        RecycleBin
    }

    public sealed class JunkFileItem
    {
        public string FilePath { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsSelected { get; set; } = true;

        public override string ToString() => $"{FilePath} ({Size / 1024.0:F1} KB)";
    }

    public sealed class JunkCategory
    {
        public JunkCategoryType CategoryType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; } = true;
        public long TotalSizeBytes { get; set; }
        public int ItemCount { get; set; }
        public List<JunkFileItem> Items { get; } = new();

        public override string ToString() => $"{Name}: {ItemCount} items ({TotalSizeBytes / (1024.0 * 1024.0):F2} MB)";
    }

    public sealed class JunkCleanResult
    {
        public int DeletedFilesCount { get; set; }
        public long BytesFreed { get; set; }
        public int FailedCount { get; set; }
        public List<string> Errors { get; } = new();
        public bool Success => FailedCount == 0;
    }
}
