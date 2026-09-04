/*
    OpenUninstall Pro - Open Source Professional Windows Uninstaller
    Forced Removal Manager Subsystem
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using UninstallTools.Backup;
using UninstallTools.Core;
using UninstallTools.FileSystemEngine;
using UninstallTools.RegistryEngine;

namespace UninstallTools.ForcedRemoval
{
    public static class ForcedUninstallManager
    {
        /// <summary>
        /// Generates a comprehensive forced removal plan by analyzing an application query or folder path.
        /// </summary>
        public static ForcedRemovalPlan BuildPlan(string appNameOrPath, string publisher = null, Action<string> progressCallback = null)
        {
            if (string.IsNullOrWhiteSpace(appNameOrPath))
                throw new ArgumentNullException(nameof(appNameOrPath));

            var plan = new ForcedRemovalPlan
            {
                SearchQuery = appNameOrPath
            };

            string searchKeyword = appNameOrPath.Trim();
            string targetFolder = null;

            if (Directory.Exists(appNameOrPath))
            {
                targetFolder = SecurityGuard.NormalizePath(appNameOrPath);
                plan.TargetInstallLocation = targetFolder;
                searchKeyword = Path.GetFileName(targetFolder);
            }
            else if (File.Exists(appNameOrPath))
            {
                var normFile = SecurityGuard.NormalizePath(appNameOrPath);
                targetFolder = Path.GetDirectoryName(normFile);
                plan.TargetInstallLocation = targetFolder;
                searchKeyword = Path.GetFileNameWithoutExtension(normFile);
            }

            StructuredLogger.Info(LogCategory.ForcedRemoval, $"Building forced removal plan for '{searchKeyword}' (Folder: {targetFolder})");

            // 1. Scan Target Folder
            if (!string.IsNullOrEmpty(targetFolder) && !SecurityGuard.IsPathProtected(targetFolder))
            {
                progressCallback?.Invoke("Scanning installation folder...");
                var dirSize = SafeFileSystemEngine.GetDirectorySize(targetFolder, out _, out _);
                plan.Items.Add(new ForcedRemovalItem
                {
                    ItemType = ForcedRemovalItemType.Directory,
                    PathOrKey = targetFolder,
                    Description = "Target Installation Folder",
                    Confidence = ForcedRemovalConfidence.High,
                    ConfidenceScore = 100,
                    Size = dirSize,
                    MatchReason = "Exact install directory specified by user"
                });
            }

            // 2. Scan File System (ProgramData, AppData, Common AppData, LocalLow)
            progressCallback?.Invoke("Scanning application data folders...");
            ScanAppDataDirectories(searchKeyword, publisher, plan);

            // 3. Scan Registry (Uninstall, Software, App Paths, Run, Services, SharedDLLs, COM)
            progressCallback?.Invoke("Scanning system registry...");
            ScanRegistryHives(searchKeyword, publisher, targetFolder, plan);

            // 4. Scan Shortcuts (Desktop, Start Menu, Quick Launch)
            progressCallback?.Invoke("Scanning application shortcuts...");
            ScanShortcuts(searchKeyword, targetFolder, plan);

            // 5. Scan Services
            progressCallback?.Invoke("Scanning Windows services...");
            ScanServices(searchKeyword, targetFolder, plan);

            StructuredLogger.Info(LogCategory.ForcedRemoval,
                $"Forced removal plan generated with {plan.Items.Count} items. (High: {plan.HighConfidenceCount}, Medium: {plan.MediumConfidenceCount}, Low: {plan.LowConfidenceCount})");

            return plan;
        }

        private static void ScanAppDataDirectories(string keyword, string publisher, ForcedRemovalPlan plan)
        {
            var searchRoots = new List<string>
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            };

            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (!string.IsNullOrEmpty(localAppData))
            {
                var parent = Path.GetDirectoryName(localAppData);
                if (!string.IsNullOrEmpty(parent))
                {
                    var low = Path.Combine(parent, "LocalLow");
                    if (Directory.Exists(low)) searchRoots.Add(low);
                }
            }

            foreach (var root in searchRoots.Where(Directory.Exists).Distinct())
            {
                try
                {
                    foreach (var subDir in Directory.GetDirectories(root))
                    {
                        var dirName = Path.GetFileName(subDir);
                        if (SecurityGuard.IsPathProtected(subDir)) continue;

                        if (IsMatch(dirName, keyword))
                        {
                            var size = SafeFileSystemEngine.GetDirectorySize(subDir, out _, out _);
                            plan.Items.Add(new ForcedRemovalItem
                            {
                                ItemType = ForcedRemovalItemType.Directory,
                                PathOrKey = subDir,
                                Description = "Application Data Folder",
                                Confidence = ForcedRemovalConfidence.High,
                                ConfidenceScore = 90,
                                Size = size,
                                MatchReason = $"Directory name '{dirName}' matches '{keyword}'"
                            });
                        }
                        else if (!string.IsNullOrEmpty(publisher) && IsMatch(dirName, publisher))
                        {
                            // Look inside publisher folder for app
                            try
                            {
                                foreach (var pubSub in Directory.GetDirectories(subDir))
                                {
                                    var pubSubName = Path.GetFileName(pubSub);
                                    if (IsMatch(pubSubName, keyword))
                                    {
                                        var size = SafeFileSystemEngine.GetDirectorySize(pubSub, out _, out _);
                                        plan.Items.Add(new ForcedRemovalItem
                                        {
                                            ItemType = ForcedRemovalItemType.Directory,
                                            PathOrKey = pubSub,
                                            Description = "Publisher App Data Folder",
                                            Confidence = ForcedRemovalConfidence.High,
                                            ConfidenceScore = 95,
                                            Size = size,
                                            MatchReason = $"Publisher folder '{dirName}' contains app folder '{pubSubName}'"
                                        });
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }
        }

        private static void ScanRegistryHives(string keyword, string publisher, string targetFolder, ForcedRemovalPlan plan)
        {
            var uninstallRoots = new[]
            {
                @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Uninstall",
                @"HKEY_LOCAL_MACHINE\Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall",
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            foreach (var uRoot in uninstallRoots)
            {
                using var key = SafeRegistryEngine.OpenKey(uRoot);
                if (key == null) continue;

                foreach (var subName in key.GetSubKeyNames())
                {
                    var fullKey = $"{uRoot}\\{subName}";
                    using var subKey = key.OpenSubKey(subName);
                    if (subKey == null) continue;

                    var dispName = subKey.GetValue("DisplayName")?.ToString();
                    var pubName = subKey.GetValue("Publisher")?.ToString();
                    var instLoc = subKey.GetValue("InstallLocation")?.ToString();

                    if ((!string.IsNullOrEmpty(dispName) && IsMatch(dispName, keyword)) ||
                        IsMatch(subName, keyword) ||
                        (!string.IsNullOrEmpty(instLoc) && !string.IsNullOrEmpty(targetFolder) && instLoc.StartsWith(targetFolder, StringComparison.OrdinalIgnoreCase)))
                    {
                        plan.Items.Add(new ForcedRemovalItem
                        {
                            ItemType = ForcedRemovalItemType.RegistryKey,
                            PathOrKey = fullKey,
                            Description = "Uninstall Registry Entry",
                            Confidence = ForcedRemovalConfidence.High,
                            ConfidenceScore = 95,
                            MatchReason = $"Uninstall entry DisplayName '{dispName ?? subName}' matches '{keyword}'"
                        });
                    }
                }
            }

            // Software Keys (HKLM & HKCU)
            var softwareRoots = new[]
            {
                @"HKEY_LOCAL_MACHINE\Software",
                @"HKEY_LOCAL_MACHINE\Software\WOW6432Node",
                @"HKEY_CURRENT_USER\Software"
            };

            foreach (var sRoot in softwareRoots)
            {
                using var key = SafeRegistryEngine.OpenKey(sRoot);
                if (key == null) continue;

                foreach (var subName in key.GetSubKeyNames())
                {
                    var fullKey = $"{sRoot}\\{subName}";
                    if (SecurityGuard.IsRegistryKeyProtected(fullKey)) continue;

                    if (IsMatch(subName, keyword))
                    {
                        plan.Items.Add(new ForcedRemovalItem
                        {
                            ItemType = ForcedRemovalItemType.RegistryKey,
                            PathOrKey = fullKey,
                            Description = "Software Configuration Key",
                            Confidence = ForcedRemovalConfidence.High,
                            ConfidenceScore = 85,
                            MatchReason = $"Software key '{subName}' matches '{keyword}'"
                        });
                    }
                    else if (!string.IsNullOrEmpty(publisher) && IsMatch(subName, publisher))
                    {
                        using var pubKey = key.OpenSubKey(subName);
                        if (pubKey != null)
                        {
                            foreach (var appSub in pubKey.GetSubKeyNames())
                            {
                                if (IsMatch(appSub, keyword))
                                {
                                    plan.Items.Add(new ForcedRemovalItem
                                    {
                                        ItemType = ForcedRemovalItemType.RegistryKey,
                                        PathOrKey = $"{fullKey}\\{appSub}",
                                        Description = "Publisher Software Key",
                                        Confidence = ForcedRemovalConfidence.High,
                                        ConfidenceScore = 90,
                                        MatchReason = $"Publisher registry key '{subName}' contains subkey '{appSub}'"
                                    });
                                }
                            }
                        }
                    }
                }
            }

            // App Paths
            var appPathsRoot = @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\App Paths";
            using (var apKey = SafeRegistryEngine.OpenKey(appPathsRoot))
            {
                if (apKey != null)
                {
                    foreach (var sub in apKey.GetSubKeyNames())
                    {
                        if (IsMatch(sub, keyword))
                        {
                            plan.Items.Add(new ForcedRemovalItem
                            {
                                ItemType = ForcedRemovalItemType.RegistryKey,
                                PathOrKey = $"{appPathsRoot}\\{sub}",
                                Description = "Registered App Path",
                                Confidence = ForcedRemovalConfidence.High,
                                ConfidenceScore = 90,
                                MatchReason = $"App Path entry '{sub}' matches keyword"
                            });
                        }
                    }
                }
            }
        }

        private static void ScanShortcuts(string keyword, string targetFolder, ForcedRemovalPlan plan)
        {
            var shortcutFolders = new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory),
                Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu),
                Environment.GetFolderPath(Environment.SpecialFolder.Programs),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms)
            };

            foreach (var folder in shortcutFolders.Where(Directory.Exists).Distinct())
            {
                try
                {
                    foreach (var file in Directory.GetFiles(folder, "*.lnk", SearchOption.AllDirectories))
                    {
                        var fileName = Path.GetFileNameWithoutExtension(file);
                        if (IsMatch(fileName, keyword))
                        {
                            plan.Items.Add(new ForcedRemovalItem
                            {
                                ItemType = ForcedRemovalItemType.Shortcut,
                                PathOrKey = file,
                                Description = "Application Shortcut",
                                Confidence = ForcedRemovalConfidence.High,
                                ConfidenceScore = 85,
                                MatchReason = $"Shortcut file '{fileName}.lnk' matches keyword"
                            });
                        }
                    }
                }
                catch { }
            }
        }

        private static void ScanServices(string keyword, string targetFolder, ForcedRemovalPlan plan)
        {
            try
            {
                var services = ServiceController.GetServices();
                foreach (var s in services)
                {
                    if (IsMatch(s.ServiceName, keyword) || IsMatch(s.DisplayName, keyword))
                    {
                        plan.Items.Add(new ForcedRemovalItem
                        {
                            ItemType = ForcedRemovalItemType.Service,
                            PathOrKey = s.ServiceName,
                            Description = $"Windows Service ({s.DisplayName})",
                            Confidence = ForcedRemovalConfidence.Medium,
                            ConfidenceScore = 75,
                            MatchReason = $"Service name '{s.ServiceName}' or display name '{s.DisplayName}' matches keyword"
                        });
                    }
                }
            }
            catch { }
        }

        private static bool IsMatch(string subject, string search)
        {
            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(search)) return false;

            var subClean = Regex.Replace(subject, @"[^a-zA-Z0-9]", "").ToLowerInvariant();
            var searchClean = Regex.Replace(search, @"[^a-zA-Z0-9]", "").ToLowerInvariant();

            if (subClean.Length < 3 || searchClean.Length < 3)
                return string.Equals(subject, search, StringComparison.OrdinalIgnoreCase);

            return subClean.Equals(searchClean) || subClean.Contains(searchClean) || searchClean.Contains(subClean);
        }

        /// <summary>
        /// Executes forced removal of approved items in the plan with pre-removal backup and safety checks.
        /// </summary>
        public static ForcedRemovalExecutionResult ExecutePlan(ForcedRemovalPlan plan, bool createBackup = true)
        {
            var result = new ForcedRemovalExecutionResult();
            if (plan == null || plan.Items.Count == 0) return result;

            var approvedItems = plan.Items.Where(i => i.IsSelected).ToList();
            if (approvedItems.Count == 0) return result;

            StructuredLogger.Info(LogCategory.ForcedRemoval, $"Executing forced removal plan {plan.PlanId} ({approvedItems.Count} items approved)");

            // Step 1: Pre-removal Backup
            if (createBackup)
            {
                var regKeys = approvedItems
                    .Where(i => i.ItemType == ForcedRemovalItemType.RegistryKey)
                    .Select(i => i.PathOrKey);

                var filePaths = approvedItems
                    .Where(i => i.ItemType == ForcedRemovalItemType.File || i.ItemType == ForcedRemovalItemType.Directory || i.ItemType == ForcedRemovalItemType.Shortcut)
                    .Select(i => i.PathOrKey);

                var backup = BackupManager.CreateBackup(
                    plan.SearchQuery,
                    "ForcedRemoval",
                    null,
                    regKeys,
                    filePaths,
                    true,
                    "ForcedRemoval");

                result.BackupId = backup.BackupId;
            }

            // Step 2: Remove Services First (stop then delete)
            foreach (var item in approvedItems.Where(i => i.ItemType == ForcedRemovalItemType.Service))
            {
                try
                {
                    using var sc = new ServiceController(item.PathOrKey);
                    if (sc.Status == ServiceControllerStatus.Running && sc.CanStop)
                    {
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(5));
                    }
                    SafeRegistryEngine.DeleteSubKeyTreeSafe(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\" + item.PathOrKey);
                    result.RemovedItemsCount++;
                    result.RemovedItemDetails.Add($"Service: {item.PathOrKey}");
                }
                catch (Exception ex)
                {
                    result.FailedItemsCount++;
                    result.Errors.Add($"Failed to remove service {item.PathOrKey}: {ex.Message}");
                }
            }

            // Step 3: Remove Files and Shortcuts
            foreach (var item in approvedItems.Where(i => i.ItemType == ForcedRemovalItemType.File || i.ItemType == ForcedRemovalItemType.Shortcut))
            {
                if (SafeFileSystemEngine.DeleteFileSafe(item.PathOrKey, DeletionMode.SendToRecycleBin))
                {
                    result.RemovedItemsCount++;
                    result.RemovedItemDetails.Add($"File: {item.PathOrKey}");
                }
                else
                {
                    result.FailedItemsCount++;
                    result.Errors.Add($"Failed to delete file: {item.PathOrKey}");
                }
            }

            // Step 4: Remove Directories (deepest first)
            foreach (var item in approvedItems.Where(i => i.ItemType == ForcedRemovalItemType.Directory).OrderByDescending(i => i.PathOrKey.Length))
            {
                if (SafeFileSystemEngine.DeleteDirectorySafe(item.PathOrKey, DeletionMode.SendToRecycleBin))
                {
                    result.RemovedItemsCount++;
                    result.RemovedItemDetails.Add($"Directory: {item.PathOrKey}");
                }
                else
                {
                    result.FailedItemsCount++;
                    result.Errors.Add($"Failed to delete directory: {item.PathOrKey}");
                }
            }

            // Step 5: Remove Registry Keys
            foreach (var item in approvedItems.Where(i => i.ItemType == ForcedRemovalItemType.RegistryKey))
            {
                if (SafeRegistryEngine.DeleteSubKeyTreeSafe(item.PathOrKey))
                {
                    result.RemovedItemsCount++;
                    result.RemovedItemDetails.Add($"RegistryKey: {item.PathOrKey}");
                }
                else
                {
                    result.FailedItemsCount++;
                    result.Errors.Add($"Failed to delete registry key: {item.PathOrKey}");
                }
            }

            StructuredLogger.Info(LogCategory.ForcedRemoval,
                $"Forced removal finished: {result.RemovedItemsCount} removed, {result.FailedItemsCount} failed.");

            return result;
        }
    }
}
