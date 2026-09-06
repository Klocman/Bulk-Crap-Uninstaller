/*
    EBUninstaller Pro - Crash Dump & Memory Dump Cleaner
    Detection, metadata inspection, and safe purging of Windows crash dumps and WER reports.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    public enum CrashDumpKind
    {
        KernelMemoryDump,
        Minidump,
        UserModeCrashDump,
        WindowsErrorReporting,
        LiveKernelReport
    }

    public class CrashDumpItem
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public CrashDumpKind Kind { get; set; }
        public long SizeBytes { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TargetProcess { get; set; } = string.Empty;
        public bool IsSelected { get; set; } = true;
    }

    public static class CrashDumpCleaner
    {
        private static readonly string WindowsDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        private static readonly string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private static readonly string ProgramData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        public static List<CrashDumpItem> ScanCrashDumps(Action<string>? onProgress = null)
        {
            var results = new List<CrashDumpItem>();

            // 1. Full Kernel Memory Dump (C:\Windows\MEMORY.DMP)
            onProgress?.Invoke("Checking Windows Kernel Memory Dump...");
            string kernelDumpPath = Path.Combine(WindowsDir, "MEMORY.DMP");
            if (File.Exists(kernelDumpPath))
            {
                try
                {
                    var fi = new FileInfo(kernelDumpPath);
                    results.Add(new CrashDumpItem
                    {
                        FilePath = fi.FullName,
                        FileName = fi.Name,
                        Kind = CrashDumpKind.KernelMemoryDump,
                        SizeBytes = fi.Length,
                        CreatedDate = fi.LastWriteTimeUtc,
                        TargetProcess = "System (Kernel BSOD)"
                    });
                }
                catch (Exception ex)
                {
                    StructuredLogger.Log(LogLevel.Warning, "CrashDumpCleaner", $"Error reading MEMORY.DMP: {ex.Message}");
                }
            }

            // 2. Windows Minidumps (C:\Windows\Minidump\*.dmp)
            string minidumpDir = Path.Combine(WindowsDir, "Minidump");
            ScanDumpDirectory(minidumpDir, "*.dmp", CrashDumpKind.Minidump, "Kernel BugCheck", results, onProgress);

            // 3. User Mode Crash Dumps (%LOCALAPPDATA%\CrashDumps\*.dmp)
            string userDumpDir = Path.Combine(LocalAppData, "CrashDumps");
            ScanDumpDirectory(userDumpDir, "*.dmp", CrashDumpKind.UserModeCrashDump, null, results, onProgress);

            // 4. Live Kernel Reports (C:\Windows\LiveKernelReports)
            string liveKernelDir = Path.Combine(WindowsDir, "LiveKernelReports");
            ScanDumpDirectory(liveKernelDir, "*.dmp", CrashDumpKind.LiveKernelReport, "Live Kernel", results, onProgress);

            // 5. Windows Error Reporting Archives
            string werUserDir = Path.Combine(LocalAppData, @"Microsoft\Windows\WER\ReportArchive");
            ScanWerReports(werUserDir, results, onProgress);

            string werSystemDir = Path.Combine(ProgramData, @"Microsoft\Windows\WER\ReportArchive");
            ScanWerReports(werSystemDir, results, onProgress);

            return results;
        }

        private static void ScanDumpDirectory(string dirPath, string searchPattern, CrashDumpKind kind, string? defaultProcess, List<CrashDumpItem> results, Action<string>? onProgress)
        {
            if (!Directory.Exists(dirPath)) return;
            onProgress?.Invoke($"Scanning: {dirPath}");

            try
            {
                var dirInfo = new DirectoryInfo(dirPath);
                foreach (var file in dirInfo.GetFiles(searchPattern, SearchOption.AllDirectories))
                {
                    string processName = defaultProcess ?? ExtractProcessFromDumpName(file.Name);
                    results.Add(new CrashDumpItem
                    {
                        FilePath = file.FullName,
                        FileName = file.Name,
                        Kind = kind,
                        SizeBytes = file.Length,
                        CreatedDate = file.LastWriteTimeUtc,
                        TargetProcess = processName
                    });
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Warning, "CrashDumpCleaner", $"Error scanning {dirPath}: {ex.Message}");
            }
        }

        private static void ScanWerReports(string werDir, List<CrashDumpItem> results, Action<string>? onProgress)
        {
            if (!Directory.Exists(werDir)) return;
            onProgress?.Invoke($"Scanning WER archives: {werDir}");

            try
            {
                var dirInfo = new DirectoryInfo(werDir);
                foreach (var reportDir in dirInfo.GetDirectories())
                {
                    try
                    {
                        long dirSize = reportDir.GetFiles("*", SearchOption.AllDirectories).Sum(f => f.Length);
                        if (dirSize > 0)
                        {
                            results.Add(new CrashDumpItem
                            {
                                FilePath = reportDir.FullName,
                                FileName = reportDir.Name,
                                Kind = CrashDumpKind.WindowsErrorReporting,
                                SizeBytes = dirSize,
                                CreatedDate = reportDir.LastWriteTimeUtc,
                                TargetProcess = reportDir.Name.Split('_')[0]
                            });
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Warning, "CrashDumpCleaner", $"Error scanning WER directory {werDir}: {ex.Message}");
            }
        }

        private static string ExtractProcessFromDumpName(string fileName)
        {
            int firstDot = fileName.IndexOf(".exe", StringComparison.OrdinalIgnoreCase);
            if (firstDot > 0)
                return fileName.Substring(0, firstDot + 4);

            return fileName.Replace(".dmp", "");
        }

        public static (int deletedCount, long freedBytes) DeleteCrashDumps(IEnumerable<CrashDumpItem> items, Action<string>? onProgress = null)
        {
            int count = 0;
            long freed = 0;

            foreach (var item in items)
            {
                if (!item.IsSelected || SecurityGuard.IsProtectedPath(item.FilePath))
                    continue;

                onProgress?.Invoke($"Deleting: {item.FileName}");
                try
                {
                    if (File.Exists(item.FilePath))
                    {
                        File.Delete(item.FilePath);
                        count++;
                        freed += item.SizeBytes;
                    }
                    else if (Directory.Exists(item.FilePath))
                    {
                        Directory.Delete(item.FilePath, true);
                        count++;
                        freed += item.SizeBytes;
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Log(LogLevel.Warning, "CrashDumpCleaner", $"Failed to delete {item.FilePath}: {ex.Message}");
                }
            }

            return (count, freed);
        }
    }
}
