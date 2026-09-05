/*
    EBUninstaller Pro - Empty Directory Cleaner
    Safe detection and cleanup of orphaned, empty folders with SecurityGuard protection.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.FileSystemEngine
{
    public class EmptyDirectoryItem
    {
        public string Path { get; set; } = string.Empty;
        public string ParentFolder { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }
        public bool IsSelected { get; set; } = true;
    }

    public class EmptyDirectoryCleaner
    {
        private static readonly string[] DefaultScanRoots = new[]
        {
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents")
        };

        public static List<EmptyDirectoryItem> ScanForEmptyDirectories(IEnumerable<string>? customRoots = null, Action<string>? onProgress = null)
        {
            var roots = customRoots?.Where(Directory.Exists).ToList() ?? DefaultScanRoots.Where(Directory.Exists).ToList();
            var results = new List<EmptyDirectoryItem>();

            foreach (var root in roots)
            {
                onProgress?.Invoke($"Scanning: {root}");
                try
                {
                    ScanDirectoryRecursive(root, results, onProgress);
                }
                catch (Exception ex)
                {
                    StructuredLogger.Log(LogLevel.Warning, "EmptyDirectoryCleaner", $"Failed to scan {root}: {ex.Message}");
                }
            }

            return results;
        }

        private static void ScanDirectoryRecursive(string dirPath, List<EmptyDirectoryItem> results, Action<string>? onProgress)
        {
            if (SecurityGuard.IsProtectedPath(dirPath))
                return;

            string[] subDirs;
            try
            {
                subDirs = Directory.GetDirectories(dirPath);
            }
            catch
            {
                return;
            }

            foreach (var subDir in subDirs)
            {
                ScanDirectoryRecursive(subDir, results, onProgress);
            }

            try
            {
                // Re-check after subdirectories check
                var remainingDirs = Directory.GetDirectories(dirPath);
                var remainingFiles = Directory.GetFiles(dirPath);

                if (remainingDirs.Length == 0 && remainingFiles.Length == 0)
                {
                    // Check that it is not one of our root paths
                    if (!DefaultScanRoots.Any(r => string.Equals(r.TrimEnd('\\'), dirPath.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase)))
                    {
                        var dirInfo = new DirectoryInfo(dirPath);
                        results.Add(new EmptyDirectoryItem
                        {
                            Path = dirPath,
                            ParentFolder = dirInfo.Parent?.FullName ?? string.Empty,
                            LastModified = dirInfo.LastWriteTimeUtc
                        });
                    }
                }
            }
            catch
            {
                // Ignore inaccessible directories
            }
        }

        public static int DeleteEmptyDirectories(IEnumerable<EmptyDirectoryItem> items, Action<string>? onProgress = null)
        {
            int deletedCount = 0;
            var orderedItems = items.OrderByDescending(i => i.Path.Length).ToList();

            foreach (var item in orderedItems)
            {
                if (!item.IsSelected || SecurityGuard.IsProtectedPath(item.Path))
                    continue;

                onProgress?.Invoke($"Removing: {item.Path}");
                try
                {
                    if (Directory.Exists(item.Path))
                    {
                        var files = Directory.GetFiles(item.Path);
                        var subdirs = Directory.GetDirectories(item.Path);
                        if (files.Length == 0 && subdirs.Length == 0)
                        {
                            Directory.Delete(item.Path, false);
                            deletedCount++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Log(LogLevel.Warning, "EmptyDirectoryCleaner", $"Could not delete {item.Path}: {ex.Message}");
                }
            }

            return deletedCount;
        }
    }
}
