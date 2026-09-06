/*
    EBUninstaller Pro - Open Source Professional Windows Uninstaller
    Privacy Cleaner Engine Subsystem
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using UninstallTools.Core;
using UninstallTools.FileSystemEngine;
using UninstallTools.RegistryEngine;

namespace UninstallTools.PrivacyCleaner
{
    public static class PrivacyCleanerEngine
    {
        public static async Task<List<PrivacyCategory>> ScanPrivacyTracksAsync(
            Action<string> progressCallback = null,
            CancellationToken cancellationToken = default)
        {
            var categories = InitializeCategories();
            StructuredLogger.Info(LogCategory.PrivacyCleaner, "Starting privacy tracks scan");

            foreach (var cat in categories)
            {
                if (cancellationToken.IsCancellationRequested) break;
                progressCallback?.Invoke($"Scanning {cat.GroupName} - {cat.ItemName}...");

                await Task.Run(() =>
                {
                    ScanCategory(cat, cancellationToken);
                }, cancellationToken).ConfigureAwait(false);
            }

            StructuredLogger.Info(LogCategory.PrivacyCleaner,
                $"Privacy scan complete. Found {categories.Sum(c => c.ItemCount)} track records.");

            return categories;
        }

        private static List<PrivacyCategory> InitializeCategories()
        {
            return new List<PrivacyCategory>
            {
                // Chrome
                new()
                {
                    TargetType = PrivacyTargetType.BrowserChromeHistory,
                    GroupName = "Google Chrome",
                    ItemName = "Browsing & Download History",
                    Description = "List of visited websites and downloaded files recorded by Chrome.",
                    Warning = null
                },
                new()
                {
                    TargetType = PrivacyTargetType.BrowserChromeCookies,
                    GroupName = "Google Chrome",
                    ItemName = "Cookies & Session Data",
                    Description = "Saved website authentication tokens and cookie files.",
                    Warning = "Cleaning cookies will log you out of active website accounts."
                },

                // Edge
                new()
                {
                    TargetType = PrivacyTargetType.BrowserEdgeHistory,
                    GroupName = "Microsoft Edge",
                    ItemName = "Browsing & Download History",
                    Description = "List of visited websites and downloaded files recorded by Edge.",
                    Warning = null
                },
                new()
                {
                    TargetType = PrivacyTargetType.BrowserEdgeCookies,
                    GroupName = "Microsoft Edge",
                    ItemName = "Cookies & Session Data",
                    Description = "Saved website authentication tokens and cookie files.",
                    Warning = "Cleaning cookies will log you out of active website accounts."
                },

                // Firefox
                new()
                {
                    TargetType = PrivacyTargetType.BrowserFirefoxHistory,
                    GroupName = "Mozilla Firefox",
                    ItemName = "Browsing History (places.sqlite)",
                    Description = "Website visit records stored by Firefox.",
                    Warning = null
                },
                new()
                {
                    TargetType = PrivacyTargetType.BrowserFirefoxCookies,
                    GroupName = "Mozilla Firefox",
                    ItemName = "Cookies & Session Store",
                    Description = "Saved cookies and session state files.",
                    Warning = "Cleaning cookies will log you out of active website accounts."
                },

                // Brave
                new()
                {
                    TargetType = PrivacyTargetType.BrowserBraveHistory,
                    GroupName = "Brave Browser",
                    ItemName = "Browsing History",
                    Description = "Visited websites recorded by Brave.",
                    Warning = null
                },
                new()
                {
                    TargetType = PrivacyTargetType.BrowserBraveCookies,
                    GroupName = "Brave Browser",
                    ItemName = "Cookies & Session Data",
                    Description = "Saved cookies and session storage in Brave.",
                    Warning = "Cleaning cookies will log you out of active website accounts."
                },

                // Opera
                new()
                {
                    TargetType = PrivacyTargetType.BrowserOperaHistory,
                    GroupName = "Opera Browser",
                    ItemName = "Browsing History",
                    Description = "Visited websites recorded by Opera.",
                    Warning = null
                },
                new()
                {
                    TargetType = PrivacyTargetType.BrowserOperaCookies,
                    GroupName = "Opera Browser",
                    ItemName = "Cookies & Session Data",
                    Description = "Saved cookies and session storage in Opera.",
                    Warning = "Cleaning cookies will log you out of active website accounts."
                },

                // Windows
                new()
                {
                    TargetType = PrivacyTargetType.WindowsRecentDocuments,
                    GroupName = "Windows System",
                    ItemName = "Recent Documents History",
                    Description = "Shortcuts to recently accessed files and folders in %APPDATA%\\Microsoft\\Windows\\Recent.",
                    Warning = null
                },
                new()
                {
                    TargetType = PrivacyTargetType.WindowsJumpLists,
                    GroupName = "Windows System",
                    ItemName = "Taskbar Jump Lists",
                    Description = "Taskbar recent files database (AutomaticDestinations & CustomDestinations).",
                    Warning = null
                },
                new()
                {
                    TargetType = PrivacyTargetType.WindowsRunHistory,
                    GroupName = "Windows System",
                    ItemName = "Run Dialog History (RunMRU)",
                    Description = "Commands typed into the Win+R Run dialog stored in registry.",
                    Warning = null
                },
                new()
                {
                    TargetType = PrivacyTargetType.WindowsExplorerSearchHistory,
                    GroupName = "Windows System",
                    ItemName = "File Explorer Search History",
                    Description = "Recent search terms typed into File Explorer search box.",
                    Warning = null
                }
            };
        }

        private static void ScanCategory(PrivacyCategory cat, CancellationToken token)
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            switch (cat.TargetType)
            {
                case PrivacyTargetType.BrowserChromeHistory:
                    AddFileTrack(cat, Path.Combine(localAppData, "Google", "Chrome", "User Data", "Default", "History"), "Chrome History database");
                    break;

                case PrivacyTargetType.BrowserChromeCookies:
                    AddFileTrack(cat, Path.Combine(localAppData, "Google", "Chrome", "User Data", "Default", "Network", "Cookies"), "Chrome Cookies database");
                    AddFileTrack(cat, Path.Combine(localAppData, "Google", "Chrome", "User Data", "Default", "Cookies"), "Chrome Legacy Cookies database");
                    break;

                case PrivacyTargetType.BrowserEdgeHistory:
                    AddFileTrack(cat, Path.Combine(localAppData, "Microsoft", "Edge", "User Data", "Default", "History"), "Edge History database");
                    break;

                case PrivacyTargetType.BrowserEdgeCookies:
                    AddFileTrack(cat, Path.Combine(localAppData, "Microsoft", "Edge", "User Data", "Default", "Network", "Cookies"), "Edge Cookies database");
                    AddFileTrack(cat, Path.Combine(localAppData, "Microsoft", "Edge", "User Data", "Default", "Cookies"), "Edge Legacy Cookies database");
                    break;

                case PrivacyTargetType.BrowserFirefoxHistory:
                    ScanFirefoxProfiles(cat, "places.sqlite", "Firefox History database");
                    break;

                case PrivacyTargetType.BrowserFirefoxCookies:
                    ScanFirefoxProfiles(cat, "cookies.sqlite", "Firefox Cookies database");
                    break;

                case PrivacyTargetType.BrowserBraveHistory:
                    AddFileTrack(cat, Path.Combine(localAppData, "BraveSoftware", "Brave-Browser", "User Data", "Default", "History"), "Brave History database");
                    break;

                case PrivacyTargetType.BrowserBraveCookies:
                    AddFileTrack(cat, Path.Combine(localAppData, "BraveSoftware", "Brave-Browser", "User Data", "Default", "Network", "Cookies"), "Brave Cookies database");
                    break;

                case PrivacyTargetType.BrowserOperaHistory:
                    AddFileTrack(cat, Path.Combine(appData, "Opera Software", "Opera Stable", "History"), "Opera History database");
                    break;

                case PrivacyTargetType.BrowserOperaCookies:
                    AddFileTrack(cat, Path.Combine(appData, "Opera Software", "Opera Stable", "Network", "Cookies"), "Opera Cookies database");
                    break;

                case PrivacyTargetType.WindowsRecentDocuments:
                    if (!string.IsNullOrEmpty(appData))
                    {
                        var recentDir = Path.Combine(appData, "Microsoft", "Windows", "Recent");
                        ScanFolderTracks(cat, recentDir, "*.*", "Recent Document Shortcut");
                    }
                    break;

                case PrivacyTargetType.WindowsJumpLists:
                    if (!string.IsNullOrEmpty(appData))
                    {
                        var autoDest = Path.Combine(appData, "Microsoft", "Windows", "Recent", "AutomaticDestinations");
                        var custDest = Path.Combine(appData, "Microsoft", "Windows", "Recent", "CustomDestinations");
                        ScanFolderTracks(cat, autoDest, "*.automaticDestinations-ms", "Automatic Jump List");
                        ScanFolderTracks(cat, custDest, "*.customDestinations-ms", "Custom Jump List");
                    }
                    break;

                case PrivacyTargetType.WindowsRunHistory:
                    ScanRegistryMRU(cat, @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\RunMRU", "Run Dialog History Item");
                    break;

                case PrivacyTargetType.WindowsExplorerSearchHistory:
                    ScanRegistryMRU(cat, @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\WordWheelQuery", "Explorer Search Term");
                    break;
            }
        }

        private static void AddFileTrack(PrivacyCategory cat, string filePath, string description)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    var fi = new FileInfo(filePath);
                    cat.Items.Add(new PrivacyItem
                    {
                        TargetPathOrKey = filePath,
                        Description = description,
                        Size = fi.Length
                    });
                    cat.ItemCount++;
                    cat.TotalSizeBytes += fi.Length;
                }
                catch { }
            }
        }

        private static void ScanFirefoxProfiles(PrivacyCategory cat, string targetFile, string description)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (string.IsNullOrEmpty(appData)) return;

            var fxProfiles = Path.Combine(appData, "Mozilla", "Firefox", "Profiles");
            if (!Directory.Exists(fxProfiles)) return;

            try
            {
                foreach (var profile in Directory.GetDirectories(fxProfiles))
                {
                    var file = Path.Combine(profile, targetFile);
                    AddFileTrack(cat, file, description);
                }
            }
            catch { }
        }

        private static void ScanFolderTracks(PrivacyCategory cat, string dirPath, string searchPattern, string description)
        {
            if (string.IsNullOrEmpty(dirPath) || !Directory.Exists(dirPath)) return;

            try
            {
                var di = new DirectoryInfo(dirPath);
                foreach (var fi in di.GetFiles(searchPattern))
                {
                    cat.Items.Add(new PrivacyItem
                    {
                        TargetPathOrKey = fi.FullName,
                        Description = $"{description} ({fi.Name})",
                        Size = fi.Length
                    });
                    cat.ItemCount++;
                    cat.TotalSizeBytes += fi.Length;
                }
            }
            catch { }
        }

        private static void ScanRegistryMRU(PrivacyCategory cat, string regKeyPath, string description)
        {
            using var key = SafeRegistryEngine.OpenKey(regKeyPath);
            if (key == null) return;

            try
            {
                foreach (var valName in key.GetValueNames())
                {
                    if (valName.Equals("MRUList", StringComparison.OrdinalIgnoreCase)) continue;

                    var val = key.GetValue(valName)?.ToString();
                    cat.Items.Add(new PrivacyItem
                    {
                        TargetPathOrKey = $"{regKeyPath}|{valName}",
                        Description = $"{description}: '{val}'",
                        Size = 0
                    });
                    cat.ItemCount++;
                }
            }
            catch { }
        }

        /// <summary>
        /// Cleans selected privacy items safely.
        /// </summary>
        public static async Task<PrivacyCleanResult> CleanPrivacyTracksAsync(
            IEnumerable<PrivacyCategory> categories,
            Action<string> progressCallback = null,
            CancellationToken cancellationToken = default)
        {
            var result = new PrivacyCleanResult();
            StructuredLogger.Info(LogCategory.PrivacyCleaner, "Executing privacy cleanup");

            foreach (var cat in categories.Where(c => c.IsSelected))
            {
                if (cancellationToken.IsCancellationRequested) break;

                progressCallback?.Invoke($"Cleaning {cat.GroupName} - {cat.ItemName}...");

                await Task.Run(() =>
                {
                    foreach (var item in cat.Items.Where(i => i.IsSelected))
                    {
                        if (cancellationToken.IsCancellationRequested) break;

                        if (item.TargetPathOrKey.Contains('|'))
                        {
                            // Registry value
                            var split = item.TargetPathOrKey.Split('|');
                            if (SafeRegistryEngine.DeleteValueSafe(split[0], split[1]))
                            {
                                result.CleanedItemsCount++;
                            }
                            else
                            {
                                result.FailedCount++;
                            }
                        }
                        else
                        {
                            // File track
                            try
                            {
                                if (File.Exists(item.TargetPathOrKey))
                                {
                                    File.Delete(item.TargetPathOrKey);
                                    result.CleanedItemsCount++;
                                    result.BytesFreed += item.Size;
                                }
                            }
                            catch (Exception ex)
                            {
                                result.FailedCount++;
                                StructuredLogger.Trace(LogCategory.PrivacyCleaner, $"Cannot delete track {item.TargetPathOrKey}", ex.Message);
                            }
                        }
                    }
                }, cancellationToken).ConfigureAwait(false);
            }

            StructuredLogger.Info(LogCategory.PrivacyCleaner,
                $"Privacy cleanup finished: {result.CleanedItemsCount} items cleaned, {result.FailedCount} locked/failed.");

            return result;
        }
    }
}
