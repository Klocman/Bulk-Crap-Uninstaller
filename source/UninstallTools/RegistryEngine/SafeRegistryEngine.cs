/*
    OpenUninstall Pro - Open Source Professional Windows Uninstaller
    Safe Registry Subsystem
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.RegistryEngine
{
    public enum RegistryOperationType
    {
        DeleteKey,
        DeleteValue,
        ModifyValue
    }

    public sealed class RegistryOperationItem
    {
        public RegistryOperationType OperationType { get; set; }
        public string KeyPath { get; set; }
        public string ValueName { get; set; }
        public object OldValue { get; set; }
        public RegistryValueKind ValueKind { get; set; }
        public RegistryView View { get; set; } = RegistryView.Default;
        public bool IsApproved { get; set; } = true;

        public override string ToString()
        {
            if (OperationType == RegistryOperationType.DeleteKey)
                return $"DeleteKey: {KeyPath} [{View}]";
            return $"DeleteValue: {KeyPath}\\{ValueName} [{View}]";
        }
    }

    public sealed class RegistryOperationPlan
    {
        public string PlanId { get; set; } = Guid.NewGuid().ToString("N");
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string TargetApplication { get; set; }
        public List<RegistryOperationItem> Operations { get; } = new();
        public string BackupFilePath { get; set; }
    }

    public static class SafeRegistryEngine
    {
        private const int MaxRecursiveDepth = 25;

        /// <summary>
        /// Safely opens a registry hive root using 32 or 64-bit view.
        /// </summary>
        public static RegistryKey OpenBaseKey(RegistryHive hive, RegistryView view = RegistryView.Default)
        {
            return RegistryKey.OpenBaseKey(hive, view);
        }

        /// <summary>
        /// Parses a string registry path (e.g. HKEY_LOCAL_MACHINE\Software\Foo or HKLM\Software\Foo) into BaseKey and SubPath.
        /// </summary>
        public static bool ParseRegistryPath(string fullPath, out RegistryHive hive, out string subPath)
        {
            hive = RegistryHive.LocalMachine;
            subPath = string.Empty;

            if (string.IsNullOrWhiteSpace(fullPath)) return false;

            var normalized = SecurityGuard.NormalizeRegistryPath(fullPath);
            var firstSlash = normalized.IndexOf('\\');
            var rootName = firstSlash > 0 ? normalized.Substring(0, firstSlash) : normalized;
            subPath = firstSlash > 0 ? normalized.Substring(firstSlash + 1) : string.Empty;

            switch (rootName.ToUpperInvariant())
            {
                case "HKEY_LOCAL_MACHINE":
                case "HKLM":
                    hive = RegistryHive.LocalMachine;
                    return true;
                case "HKEY_CURRENT_USER":
                case "HKCU":
                    hive = RegistryHive.CurrentUser;
                    return true;
                case "HKEY_CLASSES_ROOT":
                case "HKCR":
                    hive = RegistryHive.ClassesRoot;
                    return true;
                case "HKEY_USERS":
                case "HKU":
                    hive = RegistryHive.Users;
                    return true;
                case "HKEY_CURRENT_CONFIG":
                case "HKCC":
                    hive = RegistryHive.CurrentConfig;
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Opens a subkey safely with read or write permission.
        /// </summary>
        public static RegistryKey OpenKey(string fullPath, bool writable = false, RegistryView view = RegistryView.Default)
        {
            if (!ParseRegistryPath(fullPath, out var hive, out var subPath))
                return null;

            try
            {
                var baseKey = RegistryKey.OpenBaseKey(hive, view);
                if (string.IsNullOrEmpty(subPath))
                    return baseKey;

                return baseKey.OpenSubKey(subPath, writable);
            }
            catch (Exception ex)
            {
                StructuredLogger.Debug(LogCategory.Registry, $"Failed to open registry key: {fullPath}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Checks if a registry key exists.
        /// </summary>
        public static bool KeyExists(string fullPath, RegistryView view = RegistryView.Default)
        {
            using var key = OpenKey(fullPath, false, view);
            return key != null;
        }

        /// <summary>
        /// Safely enumerates subkeys with recursion depth limit to prevent infinite loops.
        /// </summary>
        public static IEnumerable<string> EnumerateSubKeysRecursive(string fullPath, int maxDepth = MaxRecursiveDepth, RegistryView view = RegistryView.Default)
        {
            var results = new List<string>();
            EnumerateSubKeysInternal(fullPath, 0, maxDepth, view, results);
            return results;
        }

        private static void EnumerateSubKeysInternal(string fullPath, int currentDepth, int maxDepth, RegistryView view, List<string> accumulator)
        {
            if (currentDepth > maxDepth) return;

            using var key = OpenKey(fullPath, false, view);
            if (key == null) return;

            string[] subKeyNames;
            try
            {
                subKeyNames = key.GetSubKeyNames();
            }
            catch
            {
                return;
            }

            foreach (var subName in subKeyNames)
            {
                var childPath = $"{fullPath}\\{subName}";
                accumulator.Add(childPath);
                EnumerateSubKeysInternal(childPath, currentDepth + 1, maxDepth, view, accumulator);
            }
        }

        /// <summary>
        /// Exports the given registry keys into a standard .reg file before modification.
        /// </summary>
        public static bool ExportToRegFile(IEnumerable<string> fullKeyPaths, string outputFilePath)
        {
            if (fullKeyPaths == null || string.IsNullOrWhiteSpace(outputFilePath))
                return false;

            try
            {
                var dir = Path.GetDirectoryName(outputFilePath);
                if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var sb = new StringBuilder();
                sb.AppendLine("Windows Registry Editor Version 5.00");
                sb.AppendLine();

                foreach (var path in fullKeyPaths)
                {
                    ExportKeyToRegInternal(path, sb, RegistryView.Default);
                }

                File.WriteAllText(outputFilePath, sb.ToString(), Encoding.Unicode);
                StructuredLogger.Info(LogCategory.Registry, $"Exported registry backup to: {outputFilePath}");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.Registry, $"Failed to export registry to {outputFilePath}", ex.Message);
                return false;
            }
        }

        private static void ExportKeyToRegInternal(string fullPath, StringBuilder sb, RegistryView view)
        {
            var normalized = SecurityGuard.NormalizeRegistryPath(fullPath);
            using var key = OpenKey(normalized, false, view);
            if (key == null) return;

            sb.AppendLine($"[{normalized}]");

            try
            {
                foreach (var valueName in key.GetValueNames())
                {
                    var val = key.GetValue(valueName);
                    var kind = key.GetValueKind(valueName);
                    var nameFormatted = string.IsNullOrEmpty(valueName) ? "@" : $"\"{EscapeRegString(valueName)}\"";

                    switch (kind)
                    {
                        case RegistryValueKind.String:
                            sb.AppendLine($"{nameFormatted}=\"{EscapeRegString(val?.ToString() ?? string.Empty)}\"");
                            break;
                        case RegistryValueKind.DWord:
                            var dwordVal = (int)(val ?? 0);
                            sb.AppendLine($"{nameFormatted}=dword:{dwordVal:x8}");
                            break;
                        case RegistryValueKind.QWord:
                            var qwordVal = (long)(val ?? 0);
                            sb.AppendLine($"{nameFormatted}=hex(b):{FormatHexQWord(qwordVal)}");
                            break;
                        case RegistryValueKind.Binary:
                            var bytes = val as byte[] ?? Array.Empty<byte>();
                            sb.AppendLine($"{nameFormatted}=hex:{FormatHexBytes(bytes)}");
                            break;
                        default:
                            sb.AppendLine($"{nameFormatted}=\"{EscapeRegString(val?.ToString() ?? string.Empty)}\"");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Registry, $"Error reading values for {fullPath}", ex.Message);
            }

            sb.AppendLine();

            try
            {
                foreach (var subKey in key.GetSubKeyNames())
                {
                    ExportKeyToRegInternal($"{normalized}\\{subKey}", sb, view);
                }
            }
            catch
            {
                // Ignore subkey access failures
            }
        }

        private static string EscapeRegString(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private static string FormatHexBytes(byte[] bytes)
        {
            return string.Join(",", bytes.Select(b => b.ToString("x2")));
        }

        private static string FormatHexQWord(long val)
        {
            var bytes = BitConverter.GetBytes(val);
            return string.Join(",", bytes.Select(b => b.ToString("x2")));
        }

        /// <summary>
        /// Executes a prepared registry operation plan with safety checks, automatic backup, and error classification.
        /// </summary>
        public static RegistryExecutionResult ExecutePlan(RegistryOperationPlan plan, string backupDir = null)
        {
            var result = new RegistryExecutionResult();
            if (plan == null || plan.Operations.Count == 0)
                return result;

            // Step 1: Pre-operation backup
            if (!string.IsNullOrWhiteSpace(backupDir))
            {
                var backupPath = Path.Combine(backupDir, $"RegBackup_{plan.PlanId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.reg");
                var keysToBackup = plan.Operations
                    .Where(o => o.IsApproved)
                    .Select(o => o.KeyPath)
                    .Distinct()
                    .ToList();

                if (ExportToRegFile(keysToBackup, backupPath))
                {
                    plan.BackupFilePath = backupPath;
                    result.BackupPath = backupPath;
                }
            }

            // Step 2: Execute operations
            foreach (var op in plan.Operations.Where(o => o.IsApproved))
            {
                if (SecurityGuard.IsRegistryKeyProtected(op.KeyPath))
                {
                    StructuredLogger.Warning(LogCategory.Security, $"Blocked registry operation on protected key: {op.KeyPath}");
                    result.BlockedCount++;
                    continue;
                }

                try
                {
                    if (op.OperationType == RegistryOperationType.DeleteKey)
                    {
                        if (DeleteSubKeyTreeSafe(op.KeyPath, op.View))
                        {
                            result.DeletedKeysCount++;
                            StructuredLogger.Info(LogCategory.Registry, $"Deleted registry key: {op.KeyPath}");
                        }
                    }
                    else if (op.OperationType == RegistryOperationType.DeleteValue)
                    {
                        if (DeleteValueSafe(op.KeyPath, op.ValueName, op.View))
                        {
                            result.DeletedValuesCount++;
                            StructuredLogger.Info(LogCategory.Registry, $"Deleted registry value: {op.KeyPath}\\{op.ValueName}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.FailedCount++;
                    result.Errors.Add($"Operation {op} failed: {ex.Message}");
                    StructuredLogger.Error(LogCategory.Registry, $"Registry operation failed: {op}", ex.Message);
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes a subkey tree safely with permission acquisition if locked.
        /// </summary>
        public static bool DeleteSubKeyTreeSafe(string fullPath, RegistryView view = RegistryView.Default)
        {
            if (SecurityGuard.IsRegistryKeyProtected(fullPath)) return false;

            if (!ParseRegistryPath(fullPath, out var hive, out var subPath))
                return false;

            if (string.IsNullOrEmpty(subPath)) return false; // Never delete root hive!

            var lastSlash = subPath.LastIndexOf('\\');
            var parentPath = lastSlash > 0 ? subPath.Substring(0, lastSlash) : string.Empty;
            var targetName = lastSlash > 0 ? subPath.Substring(lastSlash + 1) : subPath;

            using var baseKey = RegistryKey.OpenBaseKey(hive, view);
            using var parentKey = string.IsNullOrEmpty(parentPath) ? baseKey : baseKey.OpenSubKey(parentPath, true);

            if (parentKey == null) return false;

            try
            {
                parentKey.DeleteSubKeyTree(targetName, false);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                // Attempt to grant write permissions if running as admin
                if (TryTakeKeyOwnership(fullPath, view))
                {
                    try
                    {
                        using var retryParent = string.IsNullOrEmpty(parentPath) ? baseKey : baseKey.OpenSubKey(parentPath, true);
                        retryParent?.DeleteSubKeyTree(targetName, false);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Deletes a value safely.
        /// </summary>
        public static bool DeleteValueSafe(string fullPath, string valueName, RegistryView view = RegistryView.Default)
        {
            if (SecurityGuard.IsRegistryKeyProtected(fullPath)) return false;

            using var key = OpenKey(fullPath, true, view);
            if (key == null) return false;

            try
            {
                key.DeleteValue(valueName, false);
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Registry, $"Failed to delete value {valueName} in {fullPath}", ex.Message);
                return false;
            }
        }

        private static bool TryTakeKeyOwnership(string fullPath, RegistryView view)
        {
            try
            {
                using var key = OpenKey(fullPath, true, view);
                if (key == null) return false;

                var security = key.GetAccessControl();
                var user = WindowsIdentity.GetCurrent().User;
                if (user != null)
                {
                    security.SetOwner(user);
                    var rule = new RegistryAccessRule(user, RegistryRights.FullControl,
                        InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                        PropagationFlags.None, AccessControlType.Allow);
                    security.AddAccessRule(rule);
                    key.SetAccessControl(security);
                    return true;
                }
            }
            catch
            {
                // AccessControl manipulation might fail on non-elevated or protected systems
            }
            return false;
        }
    }

    public sealed class RegistryExecutionResult
    {
        public int DeletedKeysCount { get; set; }
        public int DeletedValuesCount { get; set; }
        public int BlockedCount { get; set; }
        public int FailedCount { get; set; }
        public string BackupPath { get; set; }
        public List<string> Errors { get; } = new();
        public bool Success => FailedCount == 0;
    }
}
