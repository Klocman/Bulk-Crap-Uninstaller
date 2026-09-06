/*
    EBUninstaller Pro - Duplicate File Scanner
    High-performance SHA-256 duplicate file detection and safe cleanup.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.FileSystemEngine
{
    public class DuplicateFileItem
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string DirectoryPath { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsOriginal { get; set; }
        public bool IsSelectedForRemoval { get; set; }
    }

    public class DuplicateFileGroup
    {
        public string ContentHashSha256 { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public List<DuplicateFileItem> Files { get; set; } = new();

        public long WastedSpaceBytes => (Files.Count - 1) * FileSizeBytes;
    }

    public class DuplicateFileScanner
    {
        public static List<DuplicateFileGroup> ScanForDuplicates(IEnumerable<string> scanPaths, long minFileSizeBytes = 1024, Action<string>? onProgress = null)
        {
            var results = new List<DuplicateFileGroup>();
            var allFiles = new List<FileInfo>();

            foreach (var path in scanPaths)
            {
                if (!Directory.Exists(path) || SecurityGuard.IsProtectedPath(path))
                    continue;

                onProgress?.Invoke($"Indexing files in: {path}");
                try
                {
                    var dirInfo = new DirectoryInfo(path);
                    var files = dirInfo.GetFiles("*", SearchOption.AllDirectories)
                        .Where(f => f.Length >= minFileSizeBytes && !SecurityGuard.IsProtectedPath(f.FullName));
                    allFiles.AddRange(files);
                }
                catch (Exception ex)
                {
                    StructuredLogger.Log(LogLevel.Warning, "DuplicateFileScanner", $"Error indexing {path}: {ex.Message}");
                }
            }

            // Group 1: By size
            var sizeGroups = allFiles.GroupBy(f => f.Length).Where(g => g.Count() > 1).ToList();
            int totalCandidates = sizeGroups.Sum(g => g.Count());
            int processedCount = 0;

            foreach (var sizeGroup in sizeGroups)
            {
                var hashMap = new Dictionary<string, List<FileInfo>>(StringComparer.OrdinalIgnoreCase);

                foreach (var file in sizeGroup)
                {
                    processedCount++;
                    onProgress?.Invoke($"Analyzing hash ({processedCount}/{totalCandidates}): {file.Name}");

                    try
                    {
                        string hash = CryptoHasher.ComputeFileHash(file.FullName);
                        if (!hashMap.ContainsKey(hash))
                            hashMap[hash] = new List<FileInfo>();

                        hashMap[hash].Add(file);
                    }
                    catch
                    {
                        // File might be locked
                    }
                }

                foreach (var kvp in hashMap.Where(k => k.Value.Count > 1))
                {
                    var sortedFiles = kvp.Value.OrderBy(f => f.CreationTimeUtc).ToList();
                    var group = new DuplicateFileGroup
                    {
                        ContentHashSha256 = kvp.Key,
                        FileSizeBytes = sizeGroup.Key
                    };

                    for (int i = 0; i < sortedFiles.Count; i++)
                    {
                        var f = sortedFiles[i];
                        group.Files.Add(new DuplicateFileItem
                        {
                            FilePath = f.FullName,
                            FileName = f.Name,
                            DirectoryPath = f.DirectoryName ?? string.Empty,
                            FileSizeBytes = f.Length,
                            CreatedDate = f.CreationTimeUtc,
                            ModifiedDate = f.LastWriteTimeUtc,
                            IsOriginal = (i == 0),
                            IsSelectedForRemoval = (i > 0) // Default to selecting all duplicates except oldest
                        });
                    }

                    results.Add(group);
                }
            }

            return results;
        }

        public static (int deletedCount, long freedBytes) DeleteDuplicates(IEnumerable<DuplicateFileItem> items, Action<string>? onProgress = null)
        {
            int count = 0;
            long freed = 0;

            foreach (var item in items)
            {
                if (!item.IsSelectedForRemoval || item.IsOriginal || SecurityGuard.IsProtectedPath(item.FilePath))
                    continue;

                onProgress?.Invoke($"Deleting duplicate: {item.FileName}");
                try
                {
                    if (File.Exists(item.FilePath))
                    {
                        long size = new FileInfo(item.FilePath).Length;
                        File.Delete(item.FilePath);
                        count++;
                        freed += size;
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Log(LogLevel.Warning, "DuplicateFileScanner", $"Failed to delete {item.FilePath}: {ex.Message}");
                }
            }

            return (count, freed);
        }
    }
}
