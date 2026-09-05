/*
    OpenUninstall Pro - Open Source Professional Windows Uninstaller
    Installation Monitor and Snapshot Engine Subsystem
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using UninstallTools.Core;
using UninstallTools.FileSystemEngine;
using UninstallTools.RegistryEngine;

namespace UninstallTools.InstallationMonitor
{
    public static class InstallationMonitorEngine
    {
        private static string _tracesDirectory;
        private static readonly object _monitorLock = new();

        public static string TracesDirectory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_tracesDirectory))
                {
                    try
                    {
                        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        _tracesDirectory = Path.Combine(localAppData, "EBUninstallerPro", "Traces");
                    }
                    catch
                    {
                        _tracesDirectory = Path.Combine(Path.GetTempPath(), "EBUninstallerPro", "Traces");
                    }
                }

                if (!Directory.Exists(_tracesDirectory))
                    Directory.CreateDirectory(_tracesDirectory);

                return _tracesDirectory;
            }
            set
            {
                _tracesDirectory = value;
                if (!string.IsNullOrWhiteSpace(_tracesDirectory) && !Directory.Exists(_tracesDirectory))
                    Directory.CreateDirectory(_tracesDirectory);
            }
        }

        /// <summary>
        /// Captures a point-in-time system snapshot across key registry hives, file systems, services, and startup items.
        /// </summary>
        public static InstallationSnapshot TakeSnapshot(string description = "System Snapshot", IEnumerable<string> additionalFolders = null)
        {
            var snapshot = new InstallationSnapshot
            {
                SnapshotId = Guid.NewGuid().ToString("N"),
                CapturedAt = DateTime.UtcNow,
                Description = description
            };

            StructuredLogger.Info(LogCategory.InstallationMonitor, $"Taking system snapshot: {description}");

            // 1. Snapshot Registry Keys and Values
            var targetRegistryRoots = new[]
            {
                @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Uninstall",
                @"HKEY_LOCAL_MACHINE\Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall",
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Uninstall",
                @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run",
                @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\RunOnce",
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run",
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\RunOnce",
                @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\App Paths",
                @"HKEY_LOCAL_MACHINE\Software",
                @"HKEY_CURRENT_USER\Software"
            };

            foreach (var regRoot in targetRegistryRoots)
            {
                CaptureRegistrySnapshot(regRoot, snapshot, 3);
            }

            // 2. Snapshot Files & Folders
            var targetFolders = new List<string>
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory),
                Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu)
            };

            if (additionalFolders != null)
            {
                targetFolders.AddRange(additionalFolders);
            }

            foreach (var folder in targetFolders.Distinct())
            {
                CaptureFolderSnapshot(folder, snapshot, 2);
            }

            // 3. Snapshot Services
            CaptureServicesSnapshot(snapshot);

            // 4. Snapshot Environment Variables
            try
            {
                var env = Environment.GetEnvironmentVariables();
                foreach (string key in env.Keys)
                {
                    snapshot.EnvironmentVariables[key] = env[key]?.ToString();
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.InstallationMonitor, "Failed to capture environment variables", ex.Message);
            }

            StructuredLogger.Info(LogCategory.InstallationMonitor,
                $"Snapshot {snapshot.SnapshotId} captured. (Files: {snapshot.FileMetadata.Count}, RegKeys: {snapshot.RegistryKeys.Count}, Services: {snapshot.Services.Count})");

            return snapshot;
        }

        private static void CaptureRegistrySnapshot(string rootPath, InstallationSnapshot snapshot, int maxDepth)
        {
            try
            {
                using var key = SafeRegistryEngine.OpenKey(rootPath);
                if (key == null) return;

                snapshot.RegistryKeys.Add(rootPath);

                try
                {
                    foreach (var valName in key.GetValueNames())
                    {
                        var val = key.GetValue(valName);
                        snapshot.RegistryValues[$"{rootPath}|{valName}"] = val?.ToString() ?? string.Empty;
                    }
                }
                catch { }

                if (maxDepth > 0)
                {
                    foreach (var subKey in key.GetSubKeyNames())
                    {
                        CaptureRegistrySnapshot($"{rootPath}\\{subKey}", snapshot, maxDepth - 1);
                    }
                }
            }
            catch
            {
                // Inaccessible keys skipped
            }
        }

        private static void CaptureFolderSnapshot(string folderPath, InstallationSnapshot snapshot, int maxDepth)
        {
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath)) return;

            try
            {
                var di = new DirectoryInfo(folderPath);
                if ((di.Attributes & FileAttributes.ReparsePoint) != 0) return;

                snapshot.Directories.Add(di.FullName);

                foreach (var fi in di.GetFiles())
                {
                    snapshot.FileMetadata[fi.FullName] = fi.Length;
                }

                if (maxDepth > 0)
                {
                    foreach (var sub in di.GetDirectories())
                    {
                        CaptureFolderSnapshot(sub.FullName, snapshot, maxDepth - 1);
                    }
                }
            }
            catch
            {
                // Inaccessible folders skipped
            }
        }

        private static void CaptureServicesSnapshot(InstallationSnapshot snapshot)
        {
            try
            {
                var services = ServiceController.GetServices();
                foreach (var s in services)
                {
                    snapshot.Services.Add(s.ServiceName);
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.InstallationMonitor, "Failed capturing services", ex.Message);
            }
        }

        /// <summary>
        /// Compares two snapshots and produces an exact diff of added, modified, and removed items.
        /// </summary>
        public static SnapshotDiffResult CompareSnapshots(InstallationSnapshot before, InstallationSnapshot after)
        {
            if (before == null) throw new ArgumentNullException(nameof(before));
            if (after == null) throw new ArgumentNullException(nameof(after));

            var diff = new SnapshotDiffResult
            {
                BeforeSnapshotId = before.SnapshotId,
                AfterSnapshotId = after.SnapshotId
            };

            // 1. Files Diff
            foreach (var kvp in after.FileMetadata)
            {
                if (!before.FileMetadata.TryGetValue(kvp.Key, out var beforeLength))
                {
                    diff.AddedItems.Add(new TraceItem
                    {
                        Category = TraceItemCategory.File,
                        ChangeType = TraceItemChangeType.Added,
                        PathOrIdentifier = kvp.Key,
                        Size = kvp.Value
                    });
                }
                else if (beforeLength != kvp.Value)
                {
                    diff.ModifiedItems.Add(new TraceItem
                    {
                        Category = TraceItemCategory.File,
                        ChangeType = TraceItemChangeType.Modified,
                        PathOrIdentifier = kvp.Key,
                        Size = kvp.Value,
                        OldData = beforeLength.ToString(),
                        NewData = kvp.Value.ToString()
                    });
                }
            }

            foreach (var kvp in before.FileMetadata)
            {
                if (!after.FileMetadata.ContainsKey(kvp.Key))
                {
                    diff.RemovedItems.Add(new TraceItem
                    {
                        Category = TraceItemCategory.File,
                        ChangeType = TraceItemChangeType.Removed,
                        PathOrIdentifier = kvp.Key,
                        Size = kvp.Value
                    });
                }
            }

            // 2. Directories Diff
            foreach (var dir in after.Directories)
            {
                if (!before.Directories.Contains(dir))
                {
                    diff.AddedItems.Add(new TraceItem
                    {
                        Category = TraceItemCategory.Directory,
                        ChangeType = TraceItemChangeType.Added,
                        PathOrIdentifier = dir
                    });
                }
            }

            // 3. Registry Keys Diff
            foreach (var key in after.RegistryKeys)
            {
                if (!before.RegistryKeys.Contains(key))
                {
                    diff.AddedItems.Add(new TraceItem
                    {
                        Category = TraceItemCategory.RegistryKey,
                        ChangeType = TraceItemChangeType.Added,
                        PathOrIdentifier = key
                    });
                }
            }

            foreach (var key in before.RegistryKeys)
            {
                if (!after.RegistryKeys.Contains(key))
                {
                    diff.RemovedItems.Add(new TraceItem
                    {
                        Category = TraceItemCategory.RegistryKey,
                        ChangeType = TraceItemChangeType.Removed,
                        PathOrIdentifier = key
                    });
                }
            }

            // 4. Registry Values Diff
            foreach (var kvp in after.RegistryValues)
            {
                var split = kvp.Key.Split('|');
                var keyPath = split[0];
                var valName = split.Length > 1 ? split[1] : string.Empty;

                if (!before.RegistryValues.TryGetValue(kvp.Key, out var oldVal))
                {
                    diff.AddedItems.Add(new TraceItem
                    {
                        Category = TraceItemCategory.RegistryValue,
                        ChangeType = TraceItemChangeType.Added,
                        PathOrIdentifier = keyPath,
                        ValueName = valName,
                        NewData = kvp.Value
                    });
                }
                else if (!string.Equals(oldVal, kvp.Value, StringComparison.Ordinal))
                {
                    diff.ModifiedItems.Add(new TraceItem
                    {
                        Category = TraceItemCategory.RegistryValue,
                        ChangeType = TraceItemChangeType.Modified,
                        PathOrIdentifier = keyPath,
                        ValueName = valName,
                        OldData = oldVal,
                        NewData = kvp.Value
                    });
                }
            }

            // 5. Services Diff
            foreach (var s in after.Services)
            {
                if (!before.Services.Contains(s))
                {
                    diff.AddedItems.Add(new TraceItem
                    {
                        Category = TraceItemCategory.Service,
                        ChangeType = TraceItemChangeType.Added,
                        PathOrIdentifier = s
                    });
                }
            }

            return diff;
        }

        /// <summary>
        /// Starts real-time live monitoring of an installer.
        /// Takes a pre-snapshot, monitors active filesystem events during setup, runs installer, waits, and takes post-snapshot.
        /// </summary>
        public static async Task<InstallationTrace> MonitorInstallerAsync(
            string installerPath,
            string applicationName = null,
            Action<TraceItem> onLiveChangeDetected = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(installerPath) || !File.Exists(installerPath))
                throw new FileNotFoundException("Installer executable not found", installerPath);

            var appName = string.IsNullOrWhiteSpace(applicationName)
                ? Path.GetFileNameWithoutExtension(installerPath)
                : applicationName;

            var trace = new InstallationTrace
            {
                TraceId = Guid.NewGuid().ToString("N"),
                ApplicationName = appName,
                InstallerExecutablePath = installerPath,
                MonitoringStartedAt = DateTime.UtcNow
            };

            StructuredLogger.Info(LogCategory.InstallationMonitor, $"Beginning live monitoring for: {appName} ({installerPath})");

            // Step 1: Pre-install Snapshot
            var beforeSnapshot = TakeSnapshot($"Pre-install snapshot for {appName}");

            // Step 2: Set up live FileSystemWatchers
            var liveChanges = new ConcurrentBag<TraceItem>();
            var watchers = new List<FileSystemWatcher>();

            var watchDirs = new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                Environment.GetFolderPath(Environment.SpecialFolder.StartMenu)
            }.Where(d => !string.IsNullOrEmpty(d) && Directory.Exists(d)).Distinct();

            foreach (var dir in watchDirs)
            {
                try
                {
                    var watcher = new FileSystemWatcher(dir)
                    {
                        IncludeSubdirectories = true,
                        NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.Size
                    };

                    watcher.Created += (s, e) =>
                    {
                        var item = new TraceItem
                        {
                            Category = Directory.Exists(e.FullPath) ? TraceItemCategory.Directory : TraceItemCategory.File,
                            ChangeType = TraceItemChangeType.Added,
                            PathOrIdentifier = e.FullPath
                        };
                        liveChanges.Add(item);
                        onLiveChangeDetected?.Invoke(item);
                    };

                    watcher.EnableRaisingEvents = true;
                    watchers.Add(watcher);
                }
                catch (Exception ex)
                {
                    StructuredLogger.Warning(LogCategory.InstallationMonitor, $"Failed creating watcher for {dir}", ex.Message);
                }
            }

            // Step 3: Launch the Installer
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = installerPath,
                    UseShellExecute = true,
                    WorkingDirectory = Path.GetDirectoryName(installerPath)
                };

                using var proc = Process.Start(psi);
                if (proc != null)
                {
                    while (!proc.HasExited && !cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(500, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.InstallationMonitor, $"Installer execution failed: {installerPath}", ex.Message);
            }
            finally
            {
                // Stop watchers
                foreach (var w in watchers)
                {
                    try
                    {
                        w.EnableRaisingEvents = false;
                        w.Dispose();
                    }
                    catch { }
                }
            }

            // Step 4: Post-install Snapshot
            var afterSnapshot = TakeSnapshot($"Post-install snapshot for {appName}");
            trace.MonitoringStoppedAt = DateTime.UtcNow;

            // Step 5: Merge live changes and diff results
            var diff = CompareSnapshots(beforeSnapshot, afterSnapshot);
            var mergedItems = new Dictionary<string, TraceItem>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in diff.AddedItems.Concat(diff.ModifiedItems))
            {
                mergedItems[item.ToString()] = item;
            }

            foreach (var item in liveChanges)
            {
                mergedItems[item.ToString()] = item;
            }

            trace.Items = mergedItems.Values.ToList();

            // Save trace file
            SaveTrace(trace);
            StructuredLogger.Info(LogCategory.InstallationMonitor,
                $"Monitoring complete for {appName}. Detected {trace.Items.Count} total system changes.");

            return trace;
        }

        /// <summary>
        /// Saves installation trace to file in JSON format.
        /// </summary>
        public static string SaveTrace(InstallationTrace trace, string outputDir = null)
        {
            if (trace == null) throw new ArgumentNullException(nameof(trace));

            var targetDir = string.IsNullOrWhiteSpace(outputDir) ? TracesDirectory : outputDir;
            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            var traceFile = Path.Combine(targetDir, $"Trace_{trace.TraceId}.trace");
            var json = JsonSerializer.Serialize(trace, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(traceFile, json, Encoding.UTF8);

            StructuredLogger.Info(LogCategory.InstallationMonitor, $"Saved installation trace to: {traceFile}");
            return traceFile;
        }

        /// <summary>
        /// Loads an installation trace by ID or file path.
        /// </summary>
        public static InstallationTrace LoadTrace(string traceIdOrPath)
        {
            if (string.IsNullOrWhiteSpace(traceIdOrPath)) return null;

            string filePath = traceIdOrPath;
            if (!File.Exists(filePath))
            {
                filePath = Path.Combine(TracesDirectory, $"Trace_{traceIdOrPath}.trace");
            }

            if (!File.Exists(filePath)) return null;

            try
            {
                var json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<InstallationTrace>(json);
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.InstallationMonitor, $"Failed loading trace from {filePath}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Lists all saved installation traces.
        /// </summary>
        public static IReadOnlyList<InstallationTrace> ListTraces()
        {
            var list = new List<InstallationTrace>();
            if (!Directory.Exists(TracesDirectory)) return list;

            foreach (var file in Directory.GetFiles(TracesDirectory, "*.trace"))
            {
                try
                {
                    var trace = LoadTrace(file);
                    if (trace != null)
                        list.Add(trace);
                }
                catch { }
            }

            return list.OrderByDescending(t => t.MonitoringStartedAt).ToList();
        }

        /// <summary>
        /// Deletes a saved installation trace file.
        /// </summary>
        public static bool DeleteTrace(string traceId)
        {
            var filePath = Path.Combine(TracesDirectory, $"Trace_{traceId}.trace");
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    StructuredLogger.Info(LogCategory.InstallationMonitor, $"Deleted trace file {traceId}");
                    return true;
                }
                catch (Exception ex)
                {
                    StructuredLogger.Error(LogCategory.InstallationMonitor, $"Failed to delete trace {traceId}", ex.Message);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Replays / rolls back changes recorded in an installation trace to cleanly remove the application.
        /// </summary>
        public static bool RollbackTrace(InstallationTrace trace, out List<string> removedItems, out List<string> errors)
        {
            removedItems = new List<string>();
            errors = new List<string>();

            if (trace == null || trace.Items.Count == 0)
            {
                errors.Add("Trace contains no recorded changes to remove.");
                return false;
            }

            StructuredLogger.Info(LogCategory.InstallationMonitor, $"Starting rollback of trace: {trace.ApplicationName} (Changes: {trace.Items.Count})");

            // Process items: Files first, then Directories, then Registry
            var filesToRemove = trace.Items
                .Where(i => i.Category == TraceItemCategory.File && i.ChangeType == TraceItemChangeType.Added)
                .Select(i => i.PathOrIdentifier)
                .Distinct()
                .ToList();

            foreach (var file in filesToRemove)
            {
                if (SafeFileSystemEngine.DeleteFileSafe(file, DeletionMode.SendToRecycleBin))
                {
                    removedItems.Add($"File: {file}");
                }
                else
                {
                    errors.Add($"Failed to delete file: {file}");
                }
            }

            var dirsToRemove = trace.Items
                .Where(i => i.Category == TraceItemCategory.Directory && i.ChangeType == TraceItemChangeType.Added)
                .Select(i => i.PathOrIdentifier)
                .OrderByDescending(p => p.Length) // Deepest dirs first
                .Distinct()
                .ToList();

            foreach (var dir in dirsToRemove)
            {
                if (Directory.Exists(dir))
                {
                    if (SafeFileSystemEngine.DeleteDirectorySafe(dir, DeletionMode.SendToRecycleBin))
                    {
                        removedItems.Add($"Directory: {dir}");
                    }
                    else
                    {
                        errors.Add($"Failed to delete directory: {dir}");
                    }
                }
            }

            var regValuesToRemove = trace.Items
                .Where(i => i.Category == TraceItemCategory.RegistryValue && i.ChangeType == TraceItemChangeType.Added)
                .ToList();

            foreach (var rv in regValuesToRemove)
            {
                if (SafeRegistryEngine.DeleteValueSafe(rv.PathOrIdentifier, rv.ValueName))
                {
                    removedItems.Add($"RegValue: {rv.PathOrIdentifier}\\{rv.ValueName}");
                }
            }

            var regKeysToRemove = trace.Items
                .Where(i => i.Category == TraceItemCategory.RegistryKey && i.ChangeType == TraceItemChangeType.Added)
                .Select(i => i.PathOrIdentifier)
                .OrderByDescending(p => p.Length)
                .Distinct()
                .ToList();

            foreach (var rk in regKeysToRemove)
            {
                if (SafeRegistryEngine.DeleteSubKeyTreeSafe(rk))
                {
                    removedItems.Add($"RegKey: {rk}");
                }
            }

            StructuredLogger.Info(LogCategory.InstallationMonitor, $"Trace rollback finished. Removed {removedItems.Count} items with {errors.Count} errors.");
            return errors.Count == 0;
        }
    }
}
