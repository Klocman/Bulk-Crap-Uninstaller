/*
    EBUninstaller Pro - Open Source Professional Windows Uninstaller
    Security Guard and System Protection Subsystem
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace UninstallTools.Core
{
    public static class SecurityGuard
    {
        private static readonly HashSet<string> ProtectedSystemPaths = new(StringComparer.OrdinalIgnoreCase);
        private static readonly HashSet<string> ProtectedRegistryKeys = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Regex DangerousMetacharacters = new(@"[&|;><`$\r\n]", RegexOptions.Compiled);

        static SecurityGuard()
        {
            InitializeProtectedPaths();
            InitializeProtectedRegistryKeys();
        }

        private static void InitializeProtectedPaths()
        {
            try
            {
                var winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                if (!string.IsNullOrWhiteSpace(winDir))
                {
                    ProtectedSystemPaths.Add(NormalizePath(winDir));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(winDir, "System32")));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(winDir, "SysWOW64")));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(winDir, "WinSxS")));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(winDir, "SystemApps")));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(winDir, "Boot")));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(winDir, "Fonts")));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(winDir, "assembly")));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(winDir, "Microsoft.NET")));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(winDir, "servicing")));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(winDir, "security")));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(winDir, "schemas")));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(winDir, "diagnostics")));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(winDir, "Drivers")));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(winDir, "System32", "drivers")));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(winDir, "System32", "config")));
                }

                var progFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                if (!string.IsNullOrWhiteSpace(progFiles))
                {
                    ProtectedSystemPaths.Add(NormalizePath(progFiles));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(progFiles, "Common Files")));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(progFiles, "Windows Defender")));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(progFiles, "Windows NT")));
                }

                var progFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                if (!string.IsNullOrWhiteSpace(progFilesX86))
                {
                    ProtectedSystemPaths.Add(NormalizePath(progFilesX86));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(progFilesX86, "Common Files")));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(progFilesX86, "Windows Defender")));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(progFilesX86, "Windows NT")));
                }

                var progData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                if (!string.IsNullOrWhiteSpace(progData))
                {
                    ProtectedSystemPaths.Add(NormalizePath(progData));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(progData, "Microsoft")));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(progData, "Microsoft", "Windows")));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(progData, "Microsoft", "Windows Defender")));
                }

                var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                if (!string.IsNullOrWhiteSpace(userProfile))
                {
                    ProtectedSystemPaths.Add(NormalizePath(userProfile));
                    ProtectedSystemPaths.Add(NormalizePath(Path.Combine(userProfile, "AppData")));
                }

                // Protect root drives
                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (drive.IsReady)
                    {
                        ProtectedSystemPaths.Add(NormalizePath(drive.RootDirectory.FullName));
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Security, "Failed to initialize some protected paths", ex.Message);
            }
        }

        private static void InitializeProtectedRegistryKeys()
        {
            var keys = new[]
            {
                @"HKEY_LOCAL_MACHINE\SAM",
                @"HKEY_LOCAL_MACHINE\SECURITY",
                @"HKEY_LOCAL_MACHINE\SYSTEM",
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet",
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control",
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services",
                @"HKEY_LOCAL_MACHINE\SYSTEM\Select",
                @"HKEY_LOCAL_MACHINE\BCD00000000",
                @"HKEY_LOCAL_MACHINE\HARDWARE",
                @"HKEY_LOCAL_MACHINE\SOFTWARE",
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft",
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows",
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT",
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion",
                @"HKEY_CURRENT_USER",
                @"HKEY_CURRENT_USER\Software",
                @"HKEY_CURRENT_USER\Software\Microsoft",
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows",
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows NT",
                @"HKEY_CLASSES_ROOT",
                @"HKEY_USERS",
                @"HKEY_CURRENT_CONFIG"
            };

            foreach (var key in keys)
            {
                ProtectedRegistryKeys.Add(NormalizeRegistryPath(key));
            }
        }

        public static bool IsAdministrator()
        {
            try
            {
                using var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsPathProtected(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return true;

            var normalized = NormalizePath(path);
            if (string.IsNullOrEmpty(normalized)) return true;

            // Direct match with protected system paths
            if (ProtectedSystemPaths.Contains(normalized))
                return true;

            // Check if normalized path is root of any drive (e.g. C:\ or D:\)
            var pathRoot = Path.GetPathRoot(normalized);
            if (string.Equals(normalized, pathRoot, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, pathRoot?.TrimEnd('\\', '/'), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Check if path is parent of windows directory or special system directory
            var winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            if (!string.IsNullOrWhiteSpace(winDir) && winDir.StartsWith(normalized, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public static bool IsRegistryKeyProtected(string registryKeyPath)
        {
            if (string.IsNullOrWhiteSpace(registryKeyPath)) return true;

            var normalized = NormalizeRegistryPath(registryKeyPath);
            if (string.IsNullOrEmpty(normalized)) return true;

            // Direct check
            if (ProtectedRegistryKeys.Contains(normalized))
                return true;

            // Root hives check
            var segments = normalized.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length <= 2 && (segments[0].StartsWith("HKEY_", StringComparison.OrdinalIgnoreCase) ||
                                         segments[0].StartsWith("HKLM", StringComparison.OrdinalIgnoreCase) ||
                                         segments[0].StartsWith("HKCU", StringComparison.OrdinalIgnoreCase) ||
                                         segments[0].StartsWith("HKCR", StringComparison.OrdinalIgnoreCase)))
            {
                // Never allow deleting Top-level or 2nd-level keys like HKLM\SOFTWARE or HKLM\SYSTEM
                return true;
            }

            return false;
        }

        public static bool IsReparsePointOrSymlink(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;

            try
            {
                if (Directory.Exists(path))
                {
                    var di = new DirectoryInfo(path);
                    return (di.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint;
                }
                if (File.Exists(path))
                {
                    var fi = new FileInfo(path);
                    return (fi.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint;
                }
            }
            catch
            {
                // If query fails, assume false but log
            }

            return false;
        }

        public static bool ContainsMetacharacters(string argument)
        {
            if (string.IsNullOrEmpty(argument)) return false;
            return DangerousMetacharacters.IsMatch(argument);
        }

        public static string SanitizeCommandLineArgument(string argument)
        {
            if (string.IsNullOrEmpty(argument)) return string.Empty;

            // If argument is already properly quoted, keep it, otherwise escape quotes and wrap
            var trimmed = argument.Trim();
            if (trimmed.StartsWith("\"") && trimmed.EndsWith("\"") && trimmed.Length >= 2)
            {
                var inner = trimmed.Substring(1, trimmed.Length - 2);
                return "\"" + inner.Replace("\"", "\\\"") + "\"";
            }

            return "\"" + trimmed.Replace("\"", "\\\"") + "\"";
        }

        public static string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return string.Empty;

            try
            {
                var expanded = Environment.ExpandEnvironmentVariables(path.Trim());
                var full = Path.GetFullPath(expanded);
                return full.TrimEnd('\\', '/');
            }
            catch
            {
                return path.Trim().TrimEnd('\\', '/');
            }
        }

        public static string NormalizeRegistryPath(string regKey)
        {
            if (string.IsNullOrWhiteSpace(regKey)) return string.Empty;

            var trimmed = regKey.Trim().TrimEnd('\\');
            if (trimmed.StartsWith(@"[", StringComparison.Ordinal) && trimmed.EndsWith(@"]", StringComparison.Ordinal))
                trimmed = trimmed.Substring(1, trimmed.Length - 2);

            // Canonicalize root hive prefixes
            if (trimmed.StartsWith(@"HKLM\", StringComparison.OrdinalIgnoreCase))
                trimmed = @"HKEY_LOCAL_MACHINE\" + trimmed.Substring(5);
            else if (trimmed.Equals("HKLM", StringComparison.OrdinalIgnoreCase))
                trimmed = @"HKEY_LOCAL_MACHINE";
            else if (trimmed.StartsWith(@"HKCU\", StringComparison.OrdinalIgnoreCase))
                trimmed = @"HKEY_CURRENT_USER\" + trimmed.Substring(5);
            else if (trimmed.Equals("HKCU", StringComparison.OrdinalIgnoreCase))
                trimmed = @"HKEY_CURRENT_USER";
            else if (trimmed.StartsWith(@"HKCR\", StringComparison.OrdinalIgnoreCase))
                trimmed = @"HKEY_CLASSES_ROOT\" + trimmed.Substring(5);
            else if (trimmed.Equals("HKCR", StringComparison.OrdinalIgnoreCase))
                trimmed = @"HKEY_CLASSES_ROOT";
            else if (trimmed.StartsWith(@"HKU\", StringComparison.OrdinalIgnoreCase))
                trimmed = @"HKEY_USERS\" + trimmed.Substring(4);
            else if (trimmed.Equals("HKU", StringComparison.OrdinalIgnoreCase))
                trimmed = @"HKEY_USERS";
            else if (trimmed.StartsWith(@"HKCC\", StringComparison.OrdinalIgnoreCase))
                trimmed = @"HKEY_CURRENT_CONFIG\" + trimmed.Substring(5);
            else if (trimmed.Equals("HKCC", StringComparison.OrdinalIgnoreCase))
                trimmed = @"HKEY_CURRENT_CONFIG";

            return trimmed;
        }
    }
}
