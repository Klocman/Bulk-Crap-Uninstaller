/*
    EBUninstaller Pro - Windows Package Managers Integration & Cache Cleaner
    Detection, update auditing, and cache purging for WinGet, Chocolatey, and Scoop.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UninstallTools.Core;

namespace UninstallTools.Detection
{
    public enum PackageManagerType
    {
        WinGet,
        Chocolatey,
        Scoop,
        Unknown
    }

    public class ManagedPackageItem
    {
        public string PackageId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string InstalledVersion { get; set; } = string.Empty;
        public string AvailableVersion { get; set; } = string.Empty;
        public PackageManagerType Manager { get; set; }
        public bool HasUpdate => !string.IsNullOrEmpty(AvailableVersion) && !string.Equals(InstalledVersion, AvailableVersion, StringComparison.OrdinalIgnoreCase);
        public string Source { get; set; } = string.Empty;
    }

    public static class PackageManagerUpdateEngine
    {
        public static List<ManagedPackageItem> ScanPackages(Action<string>? onProgress = null)
        {
            var results = new List<ManagedPackageItem>();
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return results;

            // 1. Scan WinGet Packages
            onProgress?.Invoke("Querying Windows Package Manager (WinGet)...");
            ScanWinget(results);

            // 2. Scan Chocolatey Packages
            onProgress?.Invoke("Querying Chocolatey packages...");
            ScanChocolatey(results);

            // 3. Scan Scoop Packages
            onProgress?.Invoke("Querying Scoop packages...");
            ScanScoop(results);

            return results;
        }

        private static void ScanWinget(List<ManagedPackageItem> results)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "winget.exe",
                    Arguments = "list --accept-source-agreements",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi);
                if (proc != null)
                {
                    string output = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit();

                    var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    bool headerPassed = false;

                    foreach (var line in lines)
                    {
                        if (line.StartsWith("---") || line.StartsWith("==="))
                        {
                            headerPassed = true;
                            continue;
                        }
                        if (!headerPassed) continue;

                        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 3)
                        {
                            string id = parts[1];
                            string ver = parts[2];
                            string avail = parts.Length >= 4 ? parts[3] : ver;

                            results.Add(new ManagedPackageItem
                            {
                                PackageId = id,
                                Name = parts[0],
                                InstalledVersion = ver,
                                AvailableVersion = avail,
                                Manager = PackageManagerType.WinGet,
                                Source = "winget"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Warning, "PackageManagerUpdateEngine", $"WinGet query error: {ex.Message}");
            }
        }

        private static void ScanChocolatey(List<ManagedPackageItem> results)
        {
            string chocoPath = @"C:\ProgramData\chocolatey\bin\choco.exe";
            if (!File.Exists(chocoPath)) return;

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = chocoPath,
                    Arguments = "list --local-only --limit-output",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi);
                if (proc != null)
                {
                    string output = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit();

                    var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        var parts = line.Split('|');
                        if (parts.Length >= 2)
                        {
                            results.Add(new ManagedPackageItem
                            {
                                PackageId = parts[0],
                                Name = parts[0],
                                InstalledVersion = parts[1],
                                Manager = PackageManagerType.Chocolatey,
                                Source = "chocolatey"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Warning, "PackageManagerUpdateEngine", $"Choco query error: {ex.Message}");
            }
        }

        private static void ScanScoop(List<ManagedPackageItem> results)
        {
            string scoopAppsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"scoop\apps");
            if (!Directory.Exists(scoopAppsDir)) return;

            try
            {
                var dirInfo = new DirectoryInfo(scoopAppsDir);
                foreach (var appDir in dirInfo.GetDirectories())
                {
                    if (string.Equals(appDir.Name, "scoop", StringComparison.OrdinalIgnoreCase)) continue;

                    string currentLink = Path.Combine(appDir.FullName, "current");
                    string version = "installed";
                    if (Directory.Exists(currentLink) || File.Exists(currentLink))
                    {
                        var versionDirs = appDir.GetDirectories().Where(d => d.Name != "current").OrderByDescending(d => d.LastWriteTimeUtc).ToList();
                        if (versionDirs.Count > 0)
                            version = versionDirs[0].Name;
                    }

                    results.Add(new ManagedPackageItem
                    {
                        PackageId = appDir.Name,
                        Name = appDir.Name,
                        InstalledVersion = version,
                        Manager = PackageManagerType.Scoop,
                        Source = "scoop"
                    });
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Warning, "PackageManagerUpdateEngine", $"Scoop query error: {ex.Message}");
            }
        }

        public static (long freedBytes, int deletedFiles) CleanPackageCaches(Action<string>? onProgress = null)
        {
            long freed = 0;
            int count = 0;

            var cacheDirs = new List<string>
            {
                @"C:\ProgramData\chocolatey\cache",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"scoop\cache"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\WinGet\Packages")
            };

            foreach (var dir in cacheDirs)
            {
                if (!Directory.Exists(dir)) continue;
                onProgress?.Invoke($"Purging cache: {dir}");

                try
                {
                    var dirInfo = new DirectoryInfo(dir);
                    foreach (var file in dirInfo.GetFiles("*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            long sz = file.Length;
                            file.Delete();
                            freed += sz;
                            count++;
                        }
                        catch { }
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Log(LogLevel.Warning, "PackageManagerUpdateEngine", $"Error cleaning cache {dir}: {ex.Message}");
                }
            }

            return (freed, count);
        }
    }
}
