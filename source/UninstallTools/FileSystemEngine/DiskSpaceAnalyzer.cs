/*
    EBUninstaller Pro - Disk Space & Large File Analyzer Engine
    Detection, category breakdown, and ranking of large files and drive usage.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.FileSystemEngine
{
    public enum FileTypeCategory
    {
        ApplicationsAndExecutables,
        DiskImagesAndIsos,
        ArchivesAndZips,
        MediaAndVideos,
        Documents,
        LogsAndDumps,
        Other
    }

    public class LargeFileItem
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public DateTime LastModified { get; set; }
        public FileTypeCategory Category { get; set; }
        public bool IsProtected { get; set; }
    }

    public class DiskSpaceReport
    {
        public string RootPath { get; set; } = string.Empty;
        public long TotalScannedBytes { get; set; }
        public int TotalFilesCount { get; set; }
        public Dictionary<FileTypeCategory, long> CategorySizes { get; set; } = new();
        public List<LargeFileItem> TopLargestFiles { get; set; } = new();
    }

    public static class DiskSpaceAnalyzer
    {
        private static readonly HashSet<string> ExecutableExts = new(StringComparer.OrdinalIgnoreCase) { ".exe", ".msi", ".dll", ".sys", ".drv" };
        private static readonly HashSet<string> DiskImageExts = new(StringComparer.OrdinalIgnoreCase) { ".iso", ".vmdk", ".vhdx", ".vhd", ".img", ".bin" };
        private static readonly HashSet<string> ArchiveExts = new(StringComparer.OrdinalIgnoreCase) { ".zip", ".rar", ".7z", ".tar", ".gz", ".bz2", ".cab" };
        private static readonly HashSet<string> MediaExts = new(StringComparer.OrdinalIgnoreCase) { ".mp4", ".mkv", ".avi", ".mov", ".wmv", ".mp3", ".wav", ".flac" };
        private static readonly HashSet<string> DocumentExts = new(StringComparer.OrdinalIgnoreCase) { ".pdf", ".docx", ".xlsx", ".pptx", ".txt", ".odt" };
        private static readonly HashSet<string> LogExts = new(StringComparer.OrdinalIgnoreCase) { ".log", ".dmp", ".etl", ".evtx", ".bak", ".tmp" };

        public static DiskSpaceReport AnalyzeDirectory(string rootPath, int topFilesCount = 100, Action<string>? onProgress = null)
        {
            var report = new DiskSpaceReport { RootPath = rootPath };
            if (!Directory.Exists(rootPath)) return report;

            var allFiles = new List<LargeFileItem>();
            foreach (FileTypeCategory cat in Enum.GetValues(typeof(FileTypeCategory)))
            {
                report.CategorySizes[cat] = 0;
            }

            try
            {
                onProgress?.Invoke($"Scanning storage on {rootPath}...");
                var dirInfo = new DirectoryInfo(rootPath);

                var files = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories);
                foreach (var f in files)
                {
                    try
                    {
                        long sz = f.Length;
                        string ext = f.Extension.ToLowerInvariant();
                        var cat = ClassifyExtension(ext);
                        bool isProtected = SecurityGuard.IsProtectedPath(f.FullName);

                        report.TotalScannedBytes += sz;
                        report.TotalFilesCount++;
                        report.CategorySizes[cat] += sz;

                        if (sz >= 10 * 1024 * 1024) // Collect files >= 10 MB for ranking
                        {
                            allFiles.Add(new LargeFileItem
                            {
                                FilePath = f.FullName,
                                FileName = f.Name,
                                Extension = ext,
                                SizeBytes = sz,
                                LastModified = f.LastWriteTimeUtc,
                                Category = cat,
                                IsProtected = isProtected
                            });
                        }
                    }
                    catch
                    {
                        // Ignore inaccessible file
                    }
                }

                report.TopLargestFiles = allFiles.OrderByDescending(f => f.SizeBytes).Take(topFilesCount).ToList();
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Warning, "DiskSpaceAnalyzer", $"Error scanning {rootPath}: {ex.Message}");
            }

            return report;
        }

        public static FileTypeCategory ClassifyExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension)) return FileTypeCategory.Other;

            if (ExecutableExts.Contains(extension)) return FileTypeCategory.ApplicationsAndExecutables;
            if (DiskImageExts.Contains(extension)) return FileTypeCategory.DiskImagesAndIsos;
            if (ArchiveExts.Contains(extension)) return FileTypeCategory.ArchivesAndZips;
            if (MediaExts.Contains(extension)) return FileTypeCategory.MediaAndVideos;
            if (DocumentExts.Contains(extension)) return FileTypeCategory.Documents;
            if (LogExts.Contains(extension)) return FileTypeCategory.LogsAndDumps;

            return FileTypeCategory.Other;
        }

        public static bool DeleteLargeFile(LargeFileItem fileItem)
        {
            if (fileItem.IsProtected || SecurityGuard.IsProtectedPath(fileItem.FilePath))
                return false;

            try
            {
                if (File.Exists(fileItem.FilePath))
                {
                    File.Delete(fileItem.FilePath);
                    StructuredLogger.Log(LogLevel.Info, "DiskSpaceAnalyzer", $"Deleted large file {fileItem.FilePath}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Error, "DiskSpaceAnalyzer", $"Failed to delete {fileItem.FilePath}: {ex.Message}");
            }

            return false;
        }
    }
}
