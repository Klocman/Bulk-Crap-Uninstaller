/*
    EBUninstaller Pro - Windows Shell Icon & Thumbnail Cache Rebuilder
    Detection, sizing, purging, and live Explorer regeneration for icon and thumbnail caches.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public class ShellCacheItem
    {
        public string CacheName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public bool Exists { get; set; }
    }

    public static class ShellCacheRebuilder
    {
        public static List<ShellCacheItem> ScanShellCaches()
        {
            var results = new List<ShellCacheItem>();
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

            // 1. Legacy IconCache.db in LocalAppData
            string legacyIconCache = Path.Combine(localAppData, "IconCache.db");
            AddCacheItem(results, "Legacy Windows Icon Cache", "Primary icon cache database (IconCache.db)", legacyIconCache);

            // 2. Windows Explorer Icon & Thumbnail caches in %LOCALAPPDATA%\Microsoft\Windows\Explorer
            string explorerCacheDir = Path.Combine(localAppData, "Microsoft", "Windows", "Explorer");
            if (Directory.Exists(explorerCacheDir))
            {
                try
                {
                    var di = new DirectoryInfo(explorerCacheDir);
                    foreach (var f in di.EnumerateFiles("iconcache_*.db"))
                    {
                        AddCacheItem(results, $"Explorer Icon Cache ({f.Name})", "Per-resolution desktop icon cache", f.FullName);
                    }
                    foreach (var f in di.EnumerateFiles("thumbcache_*.db"))
                    {
                        AddCacheItem(results, $"Explorer Thumbnail Cache ({f.Name})", "Generated image/video preview thumbnails", f.FullName);
                    }
                }
                catch { }
            }

            // 3. Windows Font Cache
            string fontCache = Path.Combine(winDir, "ServiceProfiles", "LocalService", "AppData", "Local", "FontCache", "FontCache-System.dat");
            AddCacheItem(results, "Windows Font Cache (System)", "Pre-rendered font glyph database", fontCache);

            return results.OrderBy(c => c.CacheName).ToList();
        }

        private static void AddCacheItem(List<ShellCacheItem> list, string name, string desc, string path)
        {
            bool exists = File.Exists(path);
            long sz = 0;
            if (exists)
            {
                try { sz = new FileInfo(path).Length; } catch { }
            }

            list.Add(new ShellCacheItem
            {
                CacheName = name,
                Description = desc,
                FilePath = path,
                SizeBytes = sz,
                Exists = exists
            });
        }

        public static (int cleanedCount, long freedBytes, bool restartedExplorer) RebuildShellCaches(bool restartExplorer = true)
        {
            int cleaned = 0;
            long freed = 0;
            bool explorerRestarted = false;

            try
            {
                // Kill explorer.exe if requested so cache files are not locked
                if (restartExplorer)
                {
                    try
                    {
                        foreach (var p in Process.GetProcessesByName("explorer"))
                        {
                            try { p.Kill(); p.WaitForExit(3000); } catch { }
                        }
                    }
                    catch { }
                }

                var caches = ScanShellCaches().Where(c => c.Exists);
                foreach (var cache in caches)
                {
                    try
                    {
                        long sz = cache.SizeBytes;
                        File.Delete(cache.FilePath);
                        freed += sz;
                        cleaned++;
                    }
                    catch { }
                }

                // Restart explorer.exe
                if (restartExplorer)
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo("explorer.exe") { UseShellExecute = true });
                        explorerRestarted = true;
                    }
                    catch { }
                }

                StructuredLogger.Log(LogLevel.Info, "ShellCacheRebuilder", $"Rebuilt {cleaned} shell cache files ({freed} bytes freed).");
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Error, "ShellCacheRebuilder", $"Failed to rebuild shell caches: {ex.Message}");
            }

            return (cleaned, freed, explorerRestarted);
        }
    }
}
