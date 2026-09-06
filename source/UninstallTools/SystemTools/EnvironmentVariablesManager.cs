/*
    EBUninstaller Pro - Windows Environment Variables & PATH Orphan Cleaner
    Auditing, invalid directory detection, backup, and cleanup of System & User PATH environment variables.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public class PathEntryItem
    {
        public string RawPath { get; set; } = string.Empty;
        public string ExpandedPath { get; set; } = string.Empty;
        public bool IsUserLevel { get; set; }
        public bool ExistsOnDisk { get; set; }
        public bool IsDuplicate { get; set; }
    }

    public class EnvVarReport
    {
        public List<PathEntryItem> SystemPathEntries { get; set; } = new();
        public List<PathEntryItem> UserPathEntries { get; set; } = new();
        public int TotalInvalidEntries => SystemPathEntries.Count(p => !p.ExistsOnDisk) + UserPathEntries.Count(p => !p.ExistsOnDisk);
        public int TotalDuplicates => SystemPathEntries.Count(p => p.IsDuplicate) + UserPathEntries.Count(p => p.IsDuplicate);
    }

    public static class EnvironmentVariablesManager
    {
        private const string SystemEnvKey = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
        private const string UserEnvKey = @"Environment";

        public static EnvVarReport AnalyzePathVariables()
        {
            var report = new EnvVarReport();

            // Read System PATH
            string sysPathRaw = GetSystemPathRaw();
            report.SystemPathEntries = ParsePathString(sysPathRaw, false);

            // Read User PATH
            string userPathRaw = GetUserPathRaw();
            report.UserPathEntries = ParsePathString(userPathRaw, true);

            return report;
        }

        public static string GetSystemPathRaw()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(SystemEnvKey, false);
                return key?.GetValue("Path", string.Empty, RegistryValueOptions.DoNotExpandEnvironmentNames) as string ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetUserPathRaw()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(UserEnvKey, false);
                return key?.GetValue("Path", string.Empty, RegistryValueOptions.DoNotExpandEnvironmentNames) as string ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static List<PathEntryItem> ParsePathString(string rawPath, bool isUserLevel)
        {
            var results = new List<PathEntryItem>();
            if (string.IsNullOrWhiteSpace(rawPath)) return results;

            var segments = rawPath.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var seg in segments)
            {
                string trimmed = seg.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                string expanded = Environment.ExpandEnvironmentVariables(trimmed);
                bool exists = Directory.Exists(expanded) || File.Exists(expanded);
                bool isDup = seen.Contains(expanded);

                seen.Add(expanded);

                results.Add(new PathEntryItem
                {
                    RawPath = trimmed,
                    ExpandedPath = expanded,
                    IsUserLevel = isUserLevel,
                    ExistsOnDisk = exists,
                    IsDuplicate = isDup
                });
            }

            return results;
        }

        public static bool CleanInvalidPathEntries(bool cleanSystem = true, bool cleanUser = true)
        {
            try
            {
                BackupEnvironmentVariables();

                if (cleanSystem)
                {
                    string sysRaw = GetSystemPathRaw();
                    var sysEntries = ParsePathString(sysRaw, false);
                    var validSys = sysEntries.Where(e => e.ExistsOnDisk && !e.IsDuplicate).Select(e => e.RawPath);
                    string newSysPath = string.Join(";", validSys);

                    using var key = Registry.LocalMachine.OpenSubKey(SystemEnvKey, true);
                    if (key != null)
                    {
                        key.SetValue("Path", newSysPath, RegistryValueKind.ExpandString);
                        StructuredLogger.Log(LogLevel.Info, "EnvironmentVariablesManager", "Cleaned invalid entries from System PATH.");
                    }
                }

                if (cleanUser)
                {
                    string userRaw = GetUserPathRaw();
                    var userEntries = ParsePathString(userRaw, true);
                    var validUser = userEntries.Where(e => e.ExistsOnDisk && !e.IsDuplicate).Select(e => e.RawPath);
                    string newUserPath = string.Join(";", validUser);

                    using var key = Registry.CurrentUser.OpenSubKey(UserEnvKey, true);
                    if (key != null)
                    {
                        key.SetValue("Path", newUserPath, RegistryValueKind.ExpandString);
                        StructuredLogger.Log(LogLevel.Info, "EnvironmentVariablesManager", "Cleaned invalid entries from User PATH.");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Error, "EnvironmentVariablesManager", $"Failed to clean PATH variables: {ex.Message}");
                return false;
            }
        }

        public static string BackupEnvironmentVariables()
        {
            try
            {
                string backupDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EBUninstallerPro", "Backups", "EnvVars");
                if (!Directory.Exists(backupDir))
                    Directory.CreateDirectory(backupDir);

                string backupPath = Path.Combine(backupDir, $"env_backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.reg");

                var sb = new System.Text.StringBuilder();
                sb.AppendLine("Windows Registry Editor Version 5.00");
                sb.AppendLine();
                sb.AppendLine(@"[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Environment]");
                var escapedSys = GetSystemPathRaw().Replace(@"\", @"\\").Replace("\"", "\\\"");
                sb.AppendLine($"\"Path\"=\"{escapedSys}\"");
                sb.AppendLine();
                sb.AppendLine(@"[HKEY_CURRENT_USER\Environment]");
                var escapedUser = GetUserPathRaw().Replace(@"\", @"\\").Replace("\"", "\\\"");
                sb.AppendLine($"\"Path\"=\"{escapedUser}\"");

                File.WriteAllText(backupPath, sb.ToString(), System.Text.Encoding.Unicode);
                StructuredLogger.Log(LogLevel.Info, "EnvironmentVariablesManager", $"Backed up environment variables to {backupPath}");
                return backupPath;
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Error, "EnvironmentVariablesManager", $"Failed to backup environment variables: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
