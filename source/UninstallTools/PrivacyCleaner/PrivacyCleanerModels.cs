/*
    EBUninstaller Pro - Open Source Professional Windows Uninstaller
    Privacy Cleaner Models
*/

using System;
using System.Collections.Generic;

namespace UninstallTools.PrivacyCleaner
{
    public enum PrivacyTargetType
    {
        BrowserChromeHistory,
        BrowserChromeCookies,
        BrowserChromeDownloads,
        BrowserEdgeHistory,
        BrowserEdgeCookies,
        BrowserEdgeDownloads,
        BrowserFirefoxHistory,
        BrowserFirefoxCookies,
        BrowserBraveHistory,
        BrowserBraveCookies,
        BrowserOperaHistory,
        BrowserOperaCookies,
        WindowsRecentDocuments,
        WindowsRunHistory,
        WindowsJumpLists,
        WindowsExplorerSearchHistory
    }

    public sealed class PrivacyItem
    {
        public string TargetPathOrKey { get; set; }
        public string Description { get; set; }
        public long Size { get; set; }
        public bool IsSelected { get; set; } = true;
    }

    public sealed class PrivacyCategory
    {
        public PrivacyTargetType TargetType { get; set; }
        public string GroupName { get; set; } // "Google Chrome", "Windows Privacy", etc.
        public string ItemName { get; set; }
        public string Description { get; set; }
        public string Warning { get; set; }
        public bool IsSelected { get; set; } = true;
        public int ItemCount { get; set; }
        public long TotalSizeBytes { get; set; }
        public List<PrivacyItem> Items { get; } = new();

        public override string ToString() => $"[{GroupName}] {ItemName}: {ItemCount} items";
    }

    public sealed class PrivacyCleanResult
    {
        public int CleanedItemsCount { get; set; }
        public long BytesFreed { get; set; }
        public int FailedCount { get; set; }
        public List<string> Errors { get; } = new();
        public bool Success => FailedCount == 0;
    }
}
