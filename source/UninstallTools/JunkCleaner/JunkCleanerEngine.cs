/*
    OpenUninstall Pro - Open Source Professional Windows Uninstaller
    Junk Cleaner Engine Subsystem
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UninstallTools.Core;
using UninstallTools.FileSystemEngine;

namespace UninstallTools.JunkCleaner
{
    public static class JunkCleanerEngine
    {
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        private static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, uint dwFlags);

        private const uint SHERB_NOCONFIRMATION = 0x00000001;
        private const uint SHERB_NOPROGRESSUI = 0x00000002;
        private const uint SHERB_NOSOUND = 0x00000004;

        /// <summary>
        /// Scans all junk categories asynchronously and computes size and file lists.
        /// </summary>
        public static async Task<List<JunkCategory>> ScanJunkAsync(
            IReadOnlyCollection<string> exclusions = null,
            Action<string> progressCallback = null,
            CancellationToken cancellationToken = default)
        {
            var categories = InitializeCategories();
            var exclusionSet = new HashSet<string>(exclusions ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);

            StructuredLogger.Info(LogCategory.JunkCleaner, "Starting junk scan across all categories");

            foreach (var cat in categories)
            {
                if (cancellationToken.IsCancellationRequested) break;

                progressCallback?.Invoke($"Scanning {cat.Name}...");

                await Task.Run(() =>
                {
                    ScanCategory(cat, exclusionSet, cancellationToken);
                }, cancellationToken).ConfigureAwait(false);
            }

            StructuredLogger.Info(LogCategory.JunkCleaner,
                $"Junk scan completed. Found {categories.Sum(c => c.ItemCount)} files ({categories.Sum(c => c.TotalSizeBytes) / (1024.0 * 1024.0):F2} MB)");

            return categories;
        }

        private static List<JunkCategory> InitializeCategories()
        {
            return new List<JunkCategory>
            {
                new()
                {
                    CategoryType = JunkCategoryType.UserTemp,
                    Name = "User Temporary Files",
                    Description = "Temporary files created by running applications in user temporary folder (%TEMP%).",
                    IsEnabled = true
                },
                new()
                {
                    CategoryType = JunkCategoryType.WindowsTemp,
                    Name = "Windows Temporary Files",
                    Description = "Temporary files created by Windows system services and updates (%WINDIR%\\Temp).",
                    IsEnabled = true
                },
                new()
                {
                    CategoryType = JunkCategoryType.WindowsUpdateDownloadCache,
                    Name = "Windows Update Download Cache",
                    Description = "Downloaded installation packages for completed Windows updates (SoftwareDistribution\\Download).",
                    IsEnabled = true
                },
                new()
                {
                    CategoryType = JunkCategoryType.CrashDumps,
                    Name = "Memory & Crash Dumps",
                    Description = "Application crash dumps, BSOD minidumps, and WER crash reports.",
                    IsEnabled = true
                },
                new()
                {
                    CategoryType = JunkCategoryType.SystemLogs,
                    Name = "System & Application Logs",
                    Description = "Accumulated text and setup log files across Windows and AppData.",
                    IsEnabled = true
                },
                new()
                {
                    CategoryType = JunkCategoryType.ThumbnailCache,
                    Name = "Windows Thumbnail Cache",
                    Description = "Cached image/video thumbnail database files in Windows Explorer.",
                    IsEnabled = true
                },
                new()
                {
                    CategoryType = JunkCategoryType.BrowserCacheChrome,
                    Name = "Google Chrome Cache",
                    Description = "Temporary web cache files for Google Chrome.",
                    IsEnabled = true
                },
                new()
                {
                    CategoryType = JunkCategoryType.BrowserCacheEdge,
                    Name = "Microsoft Edge Cache",
                    Description = "Temporary web cache files for Microsoft Edge.",
                    IsEnabled = true
                },
                new()
                {
                    CategoryType = JunkCategoryType.BrowserCacheFirefox,
                    Name = "Mozilla Firefox Cache",
                    Description = "Temporary web cache files for Mozilla Firefox.",
                    IsEnabled = true
                },
                new()
                {
                    CategoryType = JunkCategoryType.BrowserCacheBrave,
                    Name = "Brave Browser Cache",
                    Description = "Temporary web cache files for Brave Browser.",
                    IsEnabled = true
                },
                new()
                {
                    CategoryType = JunkCategoryType.BrowserCacheOpera,
                    Name = "Opera Browser Cache",
                    Description = "Temporary web cache files for Opera Browser.",
                    IsEnabled = true
                },
                new()
                {
                    CategoryType = JunkCategoryType.RecycleBin,
                    Name = "Recycle Bin",
                    Description = "Deleted files stored in Windows Recycle Bin.",
                    IsEnabled = true
                }
            };
        }

        private static void ScanCategory(JunkCategory cat, HashSet<string> exclusions, CancellationToken token)
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

            var targetDirs = new List<string>();
            string searchPattern = "*.*";

            switch (cat.CategoryType)
            {
                case JunkCategoryType.UserTemp:
                    targetDirs.Add(Path.GetTempPath());
                    break;

                case JunkCategoryType.WindowsTemp:
                    if (!string.IsNullOrEmpty(winDir))
                        targetDirs.Add(Path.Combine(winDir, "Temp"));
                    break;

                case JunkCategoryType.WindowsUpdateDownloadCache:
                    if (!string.IsNullOrEmpty(winDir))
                        targetDirs.Add(Path.Combine(winDir, "SoftwareDistribution", "Download"));
                    break;

                case JunkCategoryType.CrashDumps:
                    if (!string.IsNullOrEmpty(localAppData))
                        targetDirs.Add(Path.Combine(localAppData, "CrashDumps"));
                    if (!string.IsNullOrEmpty(winDir))
                        targetDirs.Add(Path.Combine(winDir, "Minidump"));
                    break;

                case JunkCategoryType.SystemLogs:
                    if (!string.IsNullOrEmpty(winDir))
                        targetDirs.Add(Path.Combine(winDir, "Logs"));
                    searchPattern = "*.log";
                    break;

                case JunkCategoryType.ThumbnailCache:
                    if (!string.IsNullOrEmpty(localAppData))
                        targetDirs.Add(Path.Combine(localAppData, "Microsoft", "Windows", "Explorer"));
                    searchPattern = "thumbcache_*.db";
                    break;

                case JunkCategoryType.BrowserCacheChrome:
                    if (!string.IsNullOrEmpty(localAppData))
                        targetDirs.Add(Path.Combine(localAppData, "Google", "Chrome", "User Data", "Default", "Cache"));
                    break;

                case JunkCategoryType.BrowserCacheEdge:
                    if (!string.IsNullOrEmpty(localAppData))
                        targetDirs.Add(Path.Combine(localAppData, "Microsoft", "Edge", "User Data", "Default", "Cache"));
                    break;

                case JunkCategoryType.BrowserCacheFirefox:
                    if (!string.IsNullOrEmpty(localAppData))
                    {
                        var fxProfiles = Path.Combine(localAppData, "Mozilla", "Firefox", "Profiles");
                        if (Directory.Exists(fxProfiles))
                        {
                            try
                            {
                                foreach (var p in Directory.GetDirectories(fxProfiles))
                                {
                                    targetDirs.Add(Path.Combine(p, "cache2"));
                                }
                            }
                            catch { }
                        }
                    }
                    break;

                case JunkCategoryType.BrowserCacheBrave:
                    if (!string.IsNullOrEmpty(localAppData))
                        targetDirs.Add(Path.Combine(localAppData, "BraveSoftware", "Brave-Browser", "User Data", "Default", "Cache"));
                    break;

                case JunkCategoryType.BrowserCacheOpera:
                    if (!string.IsNullOrEmpty(appData))
                        targetDirs.Add(Path.Combine(appData, "Opera Software", "Opera Stable", "Cache"));
                    break;

                case JunkCategoryType.RecycleBin:
                    // Recycle bin items count from system
                    cat.ItemCount = 1;
                    cat.TotalSizeBytes = 0; // Calculated via shell
                    return;
            }

            foreach (var dir in targetDirs.Where(Directory.Exists))
            {
                if (token.IsCancellationRequested) return;

                try
                {
                    var di = new DirectoryInfo(dir);
                    if ((di.Attributes & FileAttributes.ReparsePoint) != 0) continue;

                    var files = di.GetFiles(searchPattern, SearchOption.AllDirectories);
                    foreach (var fi in files)
                    {
                        if (token.IsCancellationRequested) return;

                        if (exclusions.Contains(fi.FullName)) continue;
                        if (SecurityGuard.IsPathProtected(fi.FullName)) continue;

                        try
                        {
                            cat.Items.Add(new JunkFileItem
                            {
                                FilePath = fi.FullName,
                                Size = fi.Length,
                                LastModified = fi.LastWriteTimeUtc
                            });
                            cat.TotalSizeBytes += fi.Length;
                            cat.ItemCount++;
                        }
                        catch { }
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Warning(LogCategory.JunkCleaner, $"Failed scanning dir {dir}", ex.Message);
                }
            }
        }

        /// <summary>
        /// Cleans the selected items across enabled categories safely.
        /// </summary>
        public static async Task<JunkCleanResult> CleanJunkAsync(
            IEnumerable<JunkCategory> categories,
            Action<string> progressCallback = null,
            CancellationToken cancellationToken = default)
        {
            var result = new JunkCleanResult();

            StructuredLogger.Info(LogCategory.JunkCleaner, "Starting junk cleanup");

            foreach (var cat in categories.Where(c => c.IsEnabled))
            {
                if (cancellationToken.IsCancellationRequested) break;

                progressCallback?.Invoke($"Cleaning {cat.Name}...");

                if (cat.CategoryType == JunkCategoryType.RecycleBin)
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        try
                        {
                            SHEmptyRecycleBin(IntPtr.Zero, null, SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND);
                            result.DeletedFilesCount++;
                        }
                        catch (Exception ex)
                        {
                            result.FailedCount++;
                            result.Errors.Add($"Recycle Bin empty error: {ex.Message}");
                        }
                    }
                    continue;
                }

                await Task.Run(() =>
                {
                    foreach (var item in cat.Items.Where(i => i.IsSelected))
                    {
                        if (cancellationToken.IsCancellationRequested) break;

                        try
                        {
                            if (File.Exists(item.FilePath))
                            {
                                File.Delete(item.FilePath);
                                result.DeletedFilesCount++;
                                result.BytesFreed += item.Size;
                            }
                        }
                        catch (Exception ex)
                        {
                            // File might be locked by running process
                            result.FailedCount++;
                            StructuredLogger.Trace(LogCategory.JunkCleaner, $"File locked/skipped: {item.FilePath}", ex.Message);
                        }
                    }
                }, cancellationToken).ConfigureAwait(false);
            }

            StructuredLogger.Info(LogCategory.JunkCleaner,
                $"Junk cleanup finished: {result.DeletedFilesCount} files removed ({result.BytesFreed / (1024.0 * 1024.0):F2} MB freed), {result.FailedCount} locked/failed.");

            return result;
        }
    }
}
