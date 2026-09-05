/*
    EBUninstaller Pro - Windows Hosts File Residuals & Security Manager
    Auditing, backup, reset, and cleanup of leftover network redirections in %WINDIR%\System32\drivers\etc\hosts.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public class HostEntryItem
    {
        public int LineNumber { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string Hostname { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public bool IsCommentedOut { get; set; }
        public bool IsDefaultLocalhost { get; set; }
        public string RawLine { get; set; } = string.Empty;
    }

    public static class WindowsHostsFileManager
    {
        public static string HostsFilePath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers", "etc", "hosts");

        public static List<HostEntryItem> ReadHostsFile()
        {
            var results = new List<HostEntryItem>();
            string path = HostsFilePath;

            if (!File.Exists(path)) return results;

            try
            {
                var lines = File.ReadAllLines(path, Encoding.UTF8);
                for (int i = 0; i < lines.Length; i++)
                {
                    string raw = lines[i];
                    string line = raw.Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    bool isCommented = line.StartsWith("#");
                    string content = isCommented ? line.TrimStart('#').Trim() : line;

                    // Parse IP and Hostname
                    string comment = string.Empty;
                    int hashIdx = content.IndexOf('#');
                    if (hashIdx >= 0)
                    {
                        comment = content.Substring(hashIdx + 1).Trim();
                        content = content.Substring(0, hashIdx).Trim();
                    }

                    var parts = content.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                    {
                        string ip = parts[0];
                        string host = parts[1];
                        bool isDefault = (ip == "127.0.0.1" || ip == "::1") && (host.Equals("localhost", StringComparison.OrdinalIgnoreCase));

                        results.Add(new HostEntryItem
                        {
                            LineNumber = i + 1,
                            IpAddress = ip,
                            Hostname = host,
                            Comment = comment,
                            IsCommentedOut = isCommented,
                            IsDefaultLocalhost = isDefault,
                            RawLine = raw
                        });
                    }
                    else if (!isCommented)
                    {
                        // Stale unparseable non-comment line
                        results.Add(new HostEntryItem
                        {
                            LineNumber = i + 1,
                            IpAddress = "-",
                            Hostname = content,
                            Comment = "Unparseable line",
                            IsCommentedOut = false,
                            IsDefaultLocalhost = false,
                            RawLine = raw
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Warning, "WindowsHostsFileManager", $"Failed to read hosts file: {ex.Message}");
            }

            return results;
        }

        public static bool ResetHostsToDefault()
        {
            string path = HostsFilePath;

            try
            {
                // Backup before reset
                BackupHostsFile();

                var defaultContent = new StringBuilder();
                defaultContent.AppendLine("# Copyright (c) 1993-2009 Microsoft Corp.");
                defaultContent.AppendLine("#");
                defaultContent.AppendLine("# This is a sample HOSTS file used by Microsoft TCP/IP for Windows.");
                defaultContent.AppendLine("#");
                defaultContent.AppendLine("# localhost name resolution is handled within DNS itself.");
                defaultContent.AppendLine("#\t127.0.0.1       localhost");
                defaultContent.AppendLine("#\t::1             localhost");
                defaultContent.AppendLine();

                File.WriteAllText(path, defaultContent.ToString(), Encoding.UTF8);
                StructuredLogger.Log(LogLevel.Info, "WindowsHostsFileManager", "Reset hosts file to Microsoft default.");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Error, "WindowsHostsFileManager", $"Failed to reset hosts file: {ex.Message}");
                return false;
            }
        }

        public static string BackupHostsFile()
        {
            string path = HostsFilePath;
            if (!File.Exists(path)) return string.Empty;

            try
            {
                string backupDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EBUninstallerPro", "Backups", "Hosts");
                if (!Directory.Exists(backupDir))
                    Directory.CreateDirectory(backupDir);

                string backupPath = Path.Combine(backupDir, $"hosts_backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.bak");
                File.Copy(path, backupPath, true);
                StructuredLogger.Log(LogLevel.Info, "WindowsHostsFileManager", $"Created hosts backup at: {backupPath}");
                return backupPath;
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Error, "WindowsHostsFileManager", $"Failed to backup hosts file: {ex.Message}");
                return string.Empty;
            }
        }

        public static bool RemoveHostEntry(HostEntryItem item)
        {
            if (item == null) return false;
            string path = HostsFilePath;
            if (!File.Exists(path)) return false;

            try
            {
                var lines = File.ReadAllLines(path, Encoding.UTF8).ToList();
                lines.RemoveAll(l => l.Contains(item.Hostname) && (l.Contains(item.IpAddress) || item.IpAddress == "-"));

                File.WriteAllLines(path, lines, Encoding.UTF8);
                StructuredLogger.Log(LogLevel.Info, "WindowsHostsFileManager", $"Removed hosts entry: {item.IpAddress} -> {item.Hostname}");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Error, "WindowsHostsFileManager", $"Failed to remove hosts entry '{item.Hostname}': {ex.Message}");
                return false;
            }
        }
    }
}
