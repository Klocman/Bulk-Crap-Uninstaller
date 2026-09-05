/*
    EBUninstaller Pro - Windows Update & Driver Residuals Cleaner Engine
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    public sealed class SystemResidualItem
    {
        public string Category { get; set; }
        public string Path { get; set; }
        public long SizeBytes { get; set; }
        public string Description { get; set; }
        public bool IsDirectory { get; set; }
        public bool IsSafeToClean { get; set; } = true;
    }

    public static class DriverAndSystemResidualsCleaner
    {
        public static List<SystemResidualItem> ScanSystemResiduals()
        {
            var items = new List<SystemResidualItem>();

            try
            {
                var winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

                // 1. Windows Update Download Cache
                var swDistDownload = Path.Combine(winDir, "SoftwareDistribution", "Download");
                ScanFolder(swDistDownload, "Windows Update", "Downloaded update installation files", items);

                // 2. Memory Crash Dumps
                var memoryDmp = Path.Combine(winDir, "MEMORY.DMP");
                if (File.Exists(memoryDmp))
                {
                    try
                    {
                        var fi = new FileInfo(memoryDmp);
                        items.Add(new SystemResidualItem
                        {
                            Category = "Crash Dumps",
                            Path = memoryDmp,
                            SizeBytes = fi.Length,
                            Description = "Kernel memory crash dump file",
                            IsDirectory = false
                        });
                    }
                    catch { }
                }

                var minidumpDir = Path.Combine(winDir, "Minidump");
                ScanFolder(minidumpDir, "Crash Dumps", "Mini memory crash dumps", items);

                // 3. Windows Error Reporting (WER)
                var werUser = Path.Combine(localAppData, "Microsoft", "Windows", "WER");
                ScanFolder(werUser, "Error Reporting", "User error report crash logs", items);

                var werSystem = Path.Combine(programData, "Microsoft", "Windows", "WER");
                ScanFolder(werSystem, "Error Reporting", "System error report crash logs", items);

                // 4. GPU Shader Caches (DirectX / Vulkan)
                var d3dCache = Path.Combine(localAppData, "D3DSCache");
                ScanFolder(d3dCache, "Shader Cache", "DirectX shader cache files", items);

                var nvidiaCache = Path.Combine(localAppData, "NVIDIA", "DXCache");
                ScanFolder(nvidiaCache, "Shader Cache", "NVIDIA DirectX shader cache", items);

                var amdCache = Path.Combine(localAppData, "AMD", "DxCache");
                ScanFolder(amdCache, "Shader Cache", "AMD DirectX shader cache", items);
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.JunkCleaner, "Error scanning system residuals", ex.Message);
            }

            StructuredLogger.Info(LogCategory.JunkCleaner, $"System residuals scan found {items.Count} items.");
            return items;
        }

        private static void ScanFolder(string folderPath, string category, string desc, List<SystemResidualItem> list)
        {
            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath)) return;
            if (SecurityGuard.IsProtectedPath(folderPath)) return;

            try
            {
                var dirInfo = new DirectoryInfo(folderPath);
                foreach (var file in dirInfo.GetFiles("*.*", SearchOption.AllDirectories))
                {
                    try
                    {
                        list.Add(new SystemResidualItem
                        {
                            Category = category,
                            Path = file.FullName,
                            SizeBytes = file.Length,
                            Description = desc,
                            IsDirectory = false
                        });
                    }
                    catch { }
                }
            }
            catch { }
        }

        public static (int cleanedCount, long freedBytes) CleanResiduals(IEnumerable<SystemResidualItem> items)
        {
            int count = 0;
            long freed = 0;

            if (items == null) return (0, 0);

            foreach (var item in items)
            {
                if (SecurityGuard.IsProtectedPath(item.Path)) continue;

                try
                {
                    if (File.Exists(item.Path))
                    {
                        File.Delete(item.Path);
                        count++;
                        freed += item.SizeBytes;
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Warning(LogCategory.JunkCleaner, $"Failed to delete residual: {item.Path}", ex.Message);
                }
            }

            StructuredLogger.Info(LogCategory.JunkCleaner, $"Cleaned {count} residual files, freed {freed / (1024 * 1024.0):F2} MB.");
            return (count, freed);
        }
    }
}
