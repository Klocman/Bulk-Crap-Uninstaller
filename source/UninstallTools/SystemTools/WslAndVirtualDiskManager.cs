/*
    EBUninstaller Pro - Windows Subsystem for Linux (WSL) & Virtual Hard Disk Manager
    Auditing, orphan detection, disk compaction, and uninstallation of WSL distros and .vhdx virtual disks.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public class WslDistroItem
    {
        public string DistroGuid { get; set; } = string.Empty;
        public string DistributionName { get; set; } = string.Empty;
        public string VhdxPath { get; set; } = string.Empty;
        public string BasePath { get; set; } = string.Empty;
        public int WslVersion { get; set; } = 2;
        public long DiskSizeBytes { get; set; }
        public bool IsDefault { get; set; }
        public bool IsOrphanedDisk { get; set; }
    }

    public static class WslAndVirtualDiskManager
    {
        private const string LxssKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Lxss";

        public static List<WslDistroItem> GetWslDistros()
        {
            var results = new List<WslDistroItem>();

            try
            {
                using var lxssKey = Registry.CurrentUser.OpenSubKey(LxssKeyPath, false);
                if (lxssKey != null)
                {
                    string defaultGuid = lxssKey.GetValue("DefaultDistribution") as string ?? string.Empty;

                    foreach (var subKeyName in lxssKey.GetSubKeyNames())
                    {
                        try
                        {
                            using var distroKey = lxssKey.OpenSubKey(subKeyName);
                            if (distroKey == null) continue;

                            string distroName = distroKey.GetValue("DistributionName") as string ?? subKeyName;
                            string basePath = distroKey.GetValue("BasePath") as string ?? string.Empty;
                            int version = distroKey.GetValue("Version") as int? ?? 2;

                            string vhdxPath = string.Empty;
                            long sizeBytes = 0;

                            if (!string.IsNullOrEmpty(basePath))
                            {
                                basePath = Environment.ExpandEnvironmentVariables(basePath);
                                vhdxPath = Path.Combine(basePath, "ext4.vhdx");
                                if (File.Exists(vhdxPath))
                                {
                                    try
                                    {
                                        sizeBytes = new FileInfo(vhdxPath).Length;
                                    }
                                    catch { }
                                }
                            }

                            results.Add(new WslDistroItem
                            {
                                DistroGuid = subKeyName,
                                DistributionName = distroName,
                                BasePath = basePath,
                                VhdxPath = vhdxPath,
                                WslVersion = version,
                                DiskSizeBytes = sizeBytes,
                                IsDefault = subKeyName.Equals(defaultGuid, StringComparison.OrdinalIgnoreCase),
                                IsOrphanedDisk = false
                            });
                        }
                        catch { }
                    }
                }

                // Also scan common package directories for orphaned WSL vhdx disks
                ScanOrphanedVhdx(results);
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Warning, "WslManager", $"Failed to enumerate WSL distributions: {ex.Message}");
            }

            return results.OrderBy(d => d.DistributionName).ToList();
        }

        private static void ScanOrphanedVhdx(List<WslDistroItem> existing)
        {
            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string packagesDir = Path.Combine(localAppData, "Packages");
                string dockerWslDir = Path.Combine(localAppData, "Docker", "wsl");

                var scanDirs = new List<string>();
                if (Directory.Exists(dockerWslDir)) scanDirs.Add(dockerWslDir);
                if (Directory.Exists(packagesDir)) scanDirs.Add(packagesDir);

                var knownVhdx = new HashSet<string>(existing.Select(e => e.VhdxPath), StringComparer.OrdinalIgnoreCase);

                foreach (var sDir in scanDirs)
                {
                    try
                    {
                        var di = new DirectoryInfo(sDir);
                        foreach (var f in di.EnumerateFiles("*.vhdx", SearchOption.AllDirectories))
                        {
                            if (!knownVhdx.Contains(f.FullName))
                            {
                                existing.Add(new WslDistroItem
                                {
                                    DistroGuid = Guid.NewGuid().ToString("B"),
                                    DistributionName = $"Orphaned VHDX ({f.Name})",
                                    BasePath = f.DirectoryName ?? string.Empty,
                                    VhdxPath = f.FullName,
                                    WslVersion = 2,
                                    DiskSizeBytes = f.Length,
                                    IsDefault = false,
                                    IsOrphanedDisk = true
                                });
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        public static bool UnregisterDistro(string distroName)
        {
            if (string.IsNullOrWhiteSpace(distroName)) return false;

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "wsl.exe",
                    Arguments = $"--unregister \"{distroName}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi);
                proc?.WaitForExit(10000);
                bool ok = proc != null && proc.ExitCode == 0;
                StructuredLogger.Log(LogLevel.Info, "WslManager", $"Unregistered WSL distro: {distroName}");
                return ok;
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Error, "WslManager", $"Failed to unregister {distroName}: {ex.Message}");
                return false;
            }
        }

        public static bool CompactVhdx(string vhdxPath)
        {
            if (string.IsNullOrWhiteSpace(vhdxPath) || !File.Exists(vhdxPath)) return false;

            try
            {
                // Create temporary diskpart script
                string scriptPath = Path.Combine(Path.GetTempPath(), $"compact_{Guid.NewGuid():N}.txt");
                File.WriteAllText(scriptPath, $"select vdisk file=\"{vhdxPath}\"\nattach vdisk readonly\ncompact vdisk\ndetach vdisk\nexit\n");

                var psi = new ProcessStartInfo
                {
                    FileName = "diskpart.exe",
                    Arguments = $"/s \"{scriptPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi);
                proc?.WaitForExit(30000);

                try { File.Delete(scriptPath); } catch { }

                StructuredLogger.Log(LogLevel.Info, "WslManager", $"Compacted VHDX file: {vhdxPath}");
                return proc != null && proc.ExitCode == 0;
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Error, "WslManager", $"Failed to compact {vhdxPath}: {ex.Message}");
                return false;
            }
        }
    }
}
