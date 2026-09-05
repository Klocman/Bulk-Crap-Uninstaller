/*
    EBUninstaller Pro - Registry Optimizer & Integrity Analyzer Engine
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using UninstallTools.Backup;
using UninstallTools.Core;

namespace UninstallTools.RegistryEngine
{
    public enum RegistryIssueType
    {
        MissingSharedDll,
        InvalidAppPath,
        OrphanedMuiCache,
        InvalidStartupEntry,
        BrokenFileAssociation
    }

    public sealed class RegistryIssue
    {
        public RegistryIssueType IssueType { get; set; }
        public string KeyPath { get; set; }
        public string ValueName { get; set; }
        public string ValueData { get; set; }
        public string Description { get; set; }
        public bool IsSelectedForFix { get; set; } = true;
    }

    public sealed class RegistryOptimizationScanResult
    {
        public List<RegistryIssue> Issues { get; set; } = new();
        public int TotalKeysScanned { get; set; }
        public DateTime ScanTime { get; set; } = DateTime.UtcNow;
    }

    public static class RegistryOptimizerEngine
    {
        public static RegistryOptimizationScanResult ScanRegistryIssues()
        {
            var result = new RegistryOptimizationScanResult();

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return result;

            try
            {
                // 1. Scan SharedDLLs for missing file paths
                ScanSharedDlls(result);

                // 2. Scan App Paths for non-existent executables
                ScanAppPaths(result);

                // 3. Scan MuiCache for dead references
                ScanMuiCache(result);
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Registry, "Registry scan encountered error", ex.Message);
            }

            StructuredLogger.Info(LogCategory.Registry, $"Registry optimization scan found {result.Issues.Count} issues.");
            return result;
        }

        private static void ScanSharedDlls(RegistryOptimizationScanResult result)
        {
            const string sharedDllsPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\SharedDLLs";
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(sharedDllsPath, false);
                if (key != null)
                {
                    foreach (var valName in key.GetValueNames())
                    {
                        result.TotalKeysScanned++;
                        if (!string.IsNullOrEmpty(valName) && valName.Contains(":") && !File.Exists(valName))
                        {
                            result.Issues.Add(new RegistryIssue
                            {
                                IssueType = RegistryIssueType.MissingSharedDll,
                                KeyPath = $@"HKLM\{sharedDllsPath}",
                                ValueName = valName,
                                ValueData = key.GetValue(valName)?.ToString(),
                                Description = $"Shared DLL reference points to missing file: {valName}"
                            });
                        }
                    }
                }
            }
            catch { }
        }

        private static void ScanAppPaths(RegistryOptimizationScanResult result)
        {
            const string appPaths = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(appPaths, false);
                if (key != null)
                {
                    foreach (var subKeyName in key.GetSubKeyNames())
                    {
                        result.TotalKeysScanned++;
                        using var sub = key.OpenSubKey(subKeyName, false);
                        var defaultVal = sub?.GetValue(null)?.ToString();
                        if (!string.IsNullOrEmpty(defaultVal))
                        {
                            var cleanPath = defaultVal.Trim('"');
                            if (cleanPath.Contains(":") && !File.Exists(cleanPath))
                            {
                                result.Issues.Add(new RegistryIssue
                                {
                                    IssueType = RegistryIssueType.InvalidAppPath,
                                    KeyPath = $@"HKLM\{appPaths}\{subKeyName}",
                                    ValueName = "@",
                                    ValueData = defaultVal,
                                    Description = $"App Path entry for '{subKeyName}' points to non-existent executable: {cleanPath}"
                                });
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private static void ScanMuiCache(RegistryOptimizationScanResult result)
        {
            const string muiPath = @"Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\MuiCache";
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(muiPath, false);
                if (key != null)
                {
                    foreach (var valName in key.GetValueNames())
                    {
                        result.TotalKeysScanned++;
                        var filePart = valName.Split(new[] { ".FriendlyAppName", ".ApplicationCompany" }, StringSplitOptions.None)[0];
                        if (filePart.Contains(":") && !File.Exists(filePart) && !SecurityGuard.IsProtectedPath(filePart))
                        {
                            result.Issues.Add(new RegistryIssue
                            {
                                IssueType = RegistryIssueType.OrphanedMuiCache,
                                KeyPath = $@"HKCU\{muiPath}",
                                ValueName = valName,
                                ValueData = key.GetValue(valName)?.ToString(),
                                Description = $"Obsolete MUI cache entry for deleted application: {filePart}"
                            });
                        }
                    }
                }
            }
            catch { }
        }

        public static int FixRegistryIssues(IEnumerable<RegistryIssue> issues, bool createBackup = true)
        {
            int fixedCount = 0;
            var issuesList = issues != null ? new List<RegistryIssue>(issues) : new List<RegistryIssue>();

            if (issuesList.Count == 0) return 0;

            if (createBackup)
            {
                var keysToBackup = new List<string>();
                foreach (var iss in issuesList)
                {
                    if (!keysToBackup.Contains(iss.KeyPath))
                        keysToBackup.Add(iss.KeyPath);
                }
                BackupManager.CreateBackup("Registry Optimization", "1.0", "EBUninstaller Pro", keysToBackup, null, false);
            }

            foreach (var iss in issuesList)
            {
                if (SecurityGuard.IsProtectedRegistryKey(iss.KeyPath)) continue;

                try
                {
                    if (iss.KeyPath.StartsWith(@"HKLM\", StringComparison.OrdinalIgnoreCase))
                    {
                        var sub = iss.KeyPath.Substring(5);
                        using var key = Registry.LocalMachine.OpenSubKey(sub, true);
                        if (key != null)
                        {
                            if (iss.ValueName == "@")
                            {
                                Registry.LocalMachine.DeleteSubKeyTree(sub, false);
                            }
                            else
                            {
                                key.DeleteValue(iss.ValueName, false);
                            }
                            fixedCount++;
                        }
                    }
                    else if (iss.KeyPath.StartsWith(@"HKCU\", StringComparison.OrdinalIgnoreCase))
                    {
                        var sub = iss.KeyPath.Substring(5);
                        using var key = Registry.CurrentUser.OpenSubKey(sub, true);
                        if (key != null)
                        {
                            key.DeleteValue(iss.ValueName, false);
                            fixedCount++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Warning(LogCategory.Registry, $"Failed to fix registry issue: {iss.KeyPath}", ex.Message);
                }
            }

            StructuredLogger.Info(LogCategory.Registry, $"Fixed {fixedCount} registry issues.");
            return fixedCount;
        }
    }
}
