/*
    EBUninstaller Pro - Professional Next-Generation Windows Uninstaller
    Unified CLI Automation Engine
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Klocman.Extensions;
using UninstallTools;
using UninstallTools.Backup;
using UninstallTools.BrowserExtensions;
using UninstallTools.Core;
using UninstallTools.Detection;
using UninstallTools.Factory;
using UninstallTools.ForcedRemoval;
using UninstallTools.History;
using UninstallTools.InstallationMonitor;
using UninstallTools.Junk;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;
using UninstallTools.JunkCleaner;
using UninstallTools.Lists;
using UninstallTools.PrivacyCleaner;
using UninstallTools.Startup;
using UninstallTools.SystemTools;
using UninstallTools.Uninstaller;

namespace BCU_console
{
    internal static class Program
    {
        private static void ShowHelp()
        {
            Console.WriteLine(@"================================================================================
EBUninstaller Pro - Professional Command-Line Automation Tool
================================================================================

USAGE:
  EBUninstaller.exe <command> [arguments] [switches]

COMMANDS:
  list                          List all discovered installed applications
  uninstall <name_or_list>      Uninstall specified application(s) or .bcul list
  forced-uninstall <name_or_path> Deep forced removal of corrupted/missing uninstaller app
  scan <name>                   Scan and list leftovers for specified application
  leftovers <name>              Scan and automatically remove leftovers
  backup <name>                 Create complete registry and file backup of application
  restore <backup_id>           Restore application files and registry from backup
  monitor <installer_path>      Live monitor installer execution and record changes trace
  rollback-trace <trace_id>     Remove all items recorded by an installation trace
  clean-junk                    Scan and clean system temporary and cache junk files
  clean-privacy                 Scan and clean browser and Windows privacy tracks
  health                        Analyze software health, duplicate runtimes & hygiene score
  optimize-registry             Scan and repair invalid registry references & dead paths
  update                        Check for latest EBUninstaller Pro release updates
  trim-memory                   Trim process working sets and reclaim standby RAM
  clean-drivers                 Scan and list disconnected device driver residuals
  startup-impact                Analyze Windows boot performance and startup impact
  schedule-maintenance          Configure automatic background cleanup tasks
  startup                       Inspect and manage Windows startup applications
  extensions                    Inspect and manage installed browser extensions
  tools                         List and launch built-in Windows administrative tools
  export <output_file>          Export installed application catalog to XML or JSON
  history                       Inspect uninstallation and cleanup operation history
  runtimes                      Inspect installed Visual C++, .NET, DirectX & Java runtimes
  bloat                         Scan and clean registry bloat (orphan CLSIDs, invalid SharedDLLs)
  orphaned-services             Scan and clean broken Windows services with missing binaries
  export-drivers <dir>          Export and backup all OEM third-party drivers
  audit-report <output_file>    Generate comprehensive HTML/MD/CSV/JSON software audit report
  help | /?                     Display this help screen

GLOBAL SWITCHES:
  --json                        Format output as machine-readable JSON
  /Q, --quiet                   Execute in quiet / silent mode
  /U, --unattended              Unattended mode (skip confirmation prompts)
  /V, --verbose                 Verbose diagnostic logging
  /J=<Level>, --junk=<Level>    Leftover cleaning confidence (VeryGood, Good, Questionable)
  --output <path>               Custom output directory or file path

EXIT CODES:
  0     Operation completed successfully
  1     General error / Execution failure
  87    Invalid command line syntax or arguments
  1223  Operation canceled by user
");
        }

        private static int Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            try { Console.OutputEncoding = Encoding.UTF8; }
            catch { }

            StructuredLogger.Initialize();

            if (args.Length == 0 || args.Any(x => x.Equals("help", StringComparison.OrdinalIgnoreCase) || x.Equals("/?", StringComparison.OrdinalIgnoreCase) || x.Equals("--help", StringComparison.OrdinalIgnoreCase)))
            {
                ShowHelp();
                WaitForKeyIfStandalone();
                return 0;
            }

            var isJson = args.Any(x => x.Equals("--json", StringComparison.OrdinalIgnoreCase));
            var command = args[0].ToLowerInvariant().TrimStart('-', '/');

            try
            {
                switch (command)
                {
                    case "list":
                        return ProcessListCommand(args.Skip(1).ToArray(), isJson);

                    case "uninstall":
                        return ProcessUninstallCommand(args.Skip(1).ToArray(), isJson);

                    case "forced-uninstall":
                    case "forceduninstall":
                    case "force-remove":
                        return ProcessForcedUninstallCommand(args.Skip(1).ToArray(), isJson);

                    case "scan":
                        return ProcessScanCommand(args.Skip(1).ToArray(), isJson);

                    case "leftovers":
                    case "clean-leftovers":
                        return ProcessLeftoversCommand(args.Skip(1).ToArray(), isJson);

                    case "backup":
                        return ProcessBackupCommand(args.Skip(1).ToArray(), isJson);

                    case "restore":
                        return ProcessRestoreCommand(args.Skip(1).ToArray(), isJson);

                    case "monitor":
                        return ProcessMonitorCommand(args.Skip(1).ToArray(), isJson);

                    case "rollback-trace":
                    case "rollbacktrace":
                        return ProcessRollbackTraceCommand(args.Skip(1).ToArray(), isJson);

                    case "clean-junk":
                    case "junk":
                        return ProcessCleanJunkCommand(args.Skip(1).ToArray(), isJson);

                    case "clean-privacy":
                    case "privacy":
                        return ProcessCleanPrivacyCommand(args.Skip(1).ToArray(), isJson);

                    case "health":
                    case "system-health":
                        return ProcessHealthCommand(args.Skip(1).ToArray(), isJson);

                    case "optimize-registry":
                    case "optreg":
                    case "repair-registry":
                        return ProcessOptimizeRegistryCommand(args.Skip(1).ToArray(), isJson);

                    case "update":
                    case "check-update":
                    case "check-updates":
                        return ProcessUpdateCommand(args.Skip(1).ToArray(), isJson);

                    case "trim-memory":
                    case "trim-ram":
                    case "free-ram":
                        return ProcessTrimMemoryCommand(args.Skip(1).ToArray(), isJson);

                    case "clean-drivers":
                    case "drivers":
                        return ProcessCleanDriversCommand(args.Skip(1).ToArray(), isJson);

                    case "startup-impact":
                    case "boot-impact":
                        return ProcessStartupImpactCommand(args.Skip(1).ToArray(), isJson);

                    case "schedule-maintenance":
                    case "autoclean":
                        return ProcessScheduleMaintenanceCommand(args.Skip(1).ToArray(), isJson);

                    case "startup":
                        return ProcessStartupCommand(args.Skip(1).ToArray(), isJson);

                    case "extensions":
                    case "browser-extensions":
                        return ProcessExtensionsCommand(args.Skip(1).ToArray(), isJson);

                    case "tools":
                        return ProcessToolsCommand(args.Skip(1).ToArray(), isJson);

                    case "export":
                        return ProcessExportCommand(args.Skip(1).ToArray(), isJson);

                    case "history":
                        return ProcessHistoryCommand(args.Skip(1).ToArray(), isJson);

                    case "runtimes":
                    case "redist":
                    case "redistributables":
                        return ProcessRuntimesCommand(args.Skip(1).ToArray(), isJson);

                    case "bloat":
                    case "clean-bloat":
                    case "registry-bloat":
                        return ProcessRegistryBloatCommand(args.Skip(1).ToArray(), isJson);

                    case "orphaned-services":
                    case "orphan-services":
                    case "clean-services":
                        return ProcessOrphanedServicesCommand(args.Skip(1).ToArray(), isJson);

                    case "export-drivers":
                    case "backup-drivers":
                        return ProcessExportDriversCommand(args.Skip(1).ToArray(), isJson);

                    case "audit-report":
                    case "report":
                        return ProcessAuditReportCommand(args.Skip(1).ToArray(), isJson);

                    default:
                        Console.WriteLine($"Unknown command '{args[0]}'. Use 'help' for usage.");
                        return 87;
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Critical(LogCategory.Cli, "Unexpected error in CLI", ex.ToString());
                if (isJson)
                {
                    Console.WriteLine(JsonSerializer.Serialize(new { error = ex.Message, stackTrace = ex.StackTrace }));
                }
                else
                {
                    Console.WriteLine($"[FATAL ERROR] {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                }
                return 1;
            }
        }

        #region Command Implementations

        private static int ProcessListCommand(string[] args, bool isJson)
        {
            var isVerbose = args.Any(x => x.Equals("/V", StringComparison.OrdinalIgnoreCase) || x.Equals("--verbose", StringComparison.OrdinalIgnoreCase));
            var filter = GetArgValue(args, "--filter");

            var apps = QueryApps(!isJson, isVerbose);

            if (!string.IsNullOrWhiteSpace(filter))
            {
                apps = apps.Where(a => a.DisplayName != null && a.DisplayName.Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (isJson)
            {
                var summaryList = apps.Select(a => new
                {
                    name = a.DisplayName,
                    version = a.DisplayVersion,
                    publisher = a.Publisher,
                    installDate = a.InstallDate.ToString("yyyy-MM-dd"),
                    installLocation = a.InstallLocation,
                    uninstallString = a.UninstallString,
                    quietUninstallPossible = a.QuietUninstallPossible,
                    uninstallerType = a.UninstallerKind.ToString(),
                    is64Bit = a.Is64Bit.ToString(),
                    ratingId = a.RatingId
                });

                Console.WriteLine(JsonSerializer.Serialize(summaryList, new JsonSerializerOptions { WriteIndented = true }));
                return 0;
            }

            Console.WriteLine($@"{"Display Name",-42}  {"Version",-16}  {"Publisher",-26}  {"Type",-12}");
            Console.WriteLine(new string('-', 102));

            foreach (var a in apps.OrderBy(x => x.DisplayName))
            {
                var name = Truncate(a.DisplayName ?? string.Empty, 42);
                var ver = Truncate(a.DisplayVersion ?? string.Empty, 16);
                var pub = Truncate(a.Publisher ?? string.Empty, 26);
                var type = Truncate(a.UninstallerKind.ToString(), 12);

                Console.WriteLine($@"{name,-42}  {ver,-16}  {pub,-26}  {type,-12}");
            }

            Console.WriteLine();
            Console.WriteLine($"Total Applications: {apps.Count}");
            return 0;
        }

        private static int ProcessUninstallCommand(string[] args, bool isJson)
        {
            var cleanArgs = args.Where(x => !x.StartsWith("-") && !x.StartsWith("/")).ToArray();
            if (cleanArgs.Length < 1)
            {
                Console.WriteLine("Error: Missing application name or .bcul list file.");
                return 87;
            }

            var target = cleanArgs[0];
            var isQuiet = args.Any(x => x.Equals("/Q", StringComparison.OrdinalIgnoreCase) || x.Equals("--quiet", StringComparison.OrdinalIgnoreCase));
            var isUnattended = args.Any(x => x.Equals("/U", StringComparison.OrdinalIgnoreCase) || x.Equals("--unattended", StringComparison.OrdinalIgnoreCase));
            var isVerbose = args.Any(x => x.Equals("/V", StringComparison.OrdinalIgnoreCase) || x.Equals("--verbose", StringComparison.OrdinalIgnoreCase));

            var junkConfidenceLevel = GetJunkConfidenceLevel(args);

            var apps = QueryApps(!isJson, isVerbose);
            List<ApplicationUninstallerEntry> matchedApps;

            if (File.Exists(target) && target.EndsWith(".bcul", StringComparison.OrdinalIgnoreCase))
            {
                var list = UninstallList.ReadFromFile(target);
                matchedApps = apps.Where(a => list?.TestEntry(a) == true).ToList();
            }
            else
            {
                matchedApps = apps.Where(a => a.DisplayName != null && a.DisplayName.Contains(target, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (matchedApps.Count == 0)
            {
                Console.WriteLine($"No installed applications matched '{target}'.");
                return 1;
            }

            if (!isUnattended)
            {
                Console.WriteLine($"Found {matchedApps.Count} matching application(s):");
                foreach (var a in matchedApps)
                    Console.WriteLine($" - {a.DisplayName} ({a.DisplayVersion})");

                Console.Write("Are you sure you want to proceed with uninstallation? [y/N]: ");
                var key = Console.ReadKey().Key;
                Console.WriteLine();
                if (key != ConsoleKey.Y)
                {
                    Console.WriteLine("Uninstallation canceled.");
                    return 1223;
                }
            }

            var results = new List<PipelineResult>();
            foreach (var app in matchedApps)
            {
                Console.WriteLine($"Executing uninstallation pipeline for {app.DisplayName}...");
                var opt = new PipelineOptions
                {
                    SilentMode = isQuiet,
                    CreateBackup = true,
                    CreateSystemRestorePoint = true,
                    AutoRemoveHighConfidenceLeftovers = junkConfidenceLevel.HasValue,
                    MinimumJunkConfidence = junkConfidenceLevel ?? ConfidenceLevel.Good
                };

                var pipelineTask = UninstallPipeline.ExecuteAsync(app, apps, opt, (s, e) =>
                {
                    if (isVerbose) Console.WriteLine($" -> [{e.Percentage}%] {e.StatusMessage}");
                });
                pipelineTask.Wait();
                results.Add(pipelineTask.Result);
            }

            if (isJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true }));
            }
            else
            {
                Console.WriteLine("Uninstallation process completed.");
                foreach (var r in results)
                {
                    Console.WriteLine($"[{r.ApplicationName}] Success: {r.Success}, Leftovers Removed: {r.RemovedLeftoversCount}");
                }
            }

            return results.All(r => r.Success) ? 0 : 1;
        }

        private static int ProcessForcedUninstallCommand(string[] args, bool isJson)
        {
            var cleanArgs = args.Where(x => !x.StartsWith("-") && !x.StartsWith("/")).ToArray();
            if (cleanArgs.Length < 1)
            {
                Console.WriteLine("Error: Missing target application name or directory path.");
                return 87;
            }

            var target = cleanArgs[0];
            var isUnattended = args.Any(x => x.Equals("/U", StringComparison.OrdinalIgnoreCase) || x.Equals("--unattended", StringComparison.OrdinalIgnoreCase));
            var noBackup = args.Any(x => x.Equals("--no-backup", StringComparison.OrdinalIgnoreCase));

            Console.WriteLine($"Building forced removal plan for: {target}...");
            var plan = ForcedUninstallManager.BuildPlan(target, null, msg => Console.WriteLine($" -> {msg}"));

            if (plan.Items.Count == 0)
            {
                Console.WriteLine("No related files, folders, or registry entries found.");
                return 0;
            }

            if (isJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(plan, new JsonSerializerOptions { WriteIndented = true }));
            }
            else
            {
                Console.WriteLine($"\nDiscovered {plan.Items.Count} items to remove (High: {plan.HighConfidenceCount}, Medium: {plan.MediumConfidenceCount}, Low: {plan.LowConfidenceCount}):");
                foreach (var item in plan.Items)
                {
                    Console.WriteLine($" [{item.Confidence}] [{item.ItemType}] {item.PathOrKey} - {item.MatchReason}");
                }
            }

            if (!isUnattended)
            {
                Console.Write("\nProceed with permanent forced removal? [y/N]: ");
                var key = Console.ReadKey().Key;
                Console.WriteLine();
                if (key != ConsoleKey.Y)
                {
                    Console.WriteLine("Forced removal canceled.");
                    return 1223;
                }
            }

            var result = ForcedUninstallManager.ExecutePlan(plan, !noBackup);
            if (isJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
            }
            else
            {
                Console.WriteLine($"Forced removal finished: {result.RemovedItemsCount} items removed, {result.FailedItemsCount} failed.");
                if (!string.IsNullOrEmpty(result.BackupId))
                    Console.WriteLine($"Pre-removal backup created with ID: {result.BackupId}");
            }

            return result.Success ? 0 : 1;
        }

        private static int ProcessScanCommand(string[] args, bool isJson)
        {
            var cleanArgs = args.Where(x => !x.StartsWith("-") && !x.StartsWith("/")).ToArray();
            if (cleanArgs.Length < 1)
            {
                Console.WriteLine("Error: Missing target application name.");
                return 87;
            }

            var appName = cleanArgs[0];
            var apps = QueryApps(!isJson, false);
            var target = apps.FirstOrDefault(a => a.DisplayName != null && a.DisplayName.Contains(appName, StringComparison.OrdinalIgnoreCase));

            if (target == null)
            {
                Console.WriteLine($"Application '{appName}' not found.");
                return 1;
            }

            var junk = JunkManager.FindJunk(new[] { target }, apps, _ => { }).ToList();

            if (isJson)
            {
                var list = junk.Select(j => new
                {
                    name = j.GetDisplayName(),
                    confidence = j.Confidence.GetConfidence().ToString(),
                    source = j.Source?.CategoryName
                });
                Console.WriteLine(JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));
            }
            else
            {
                Console.WriteLine($"Found {junk.Count} leftover items for '{target.DisplayName}':");
                foreach (var j in junk)
                {
                    Console.WriteLine($" [{j.Confidence.GetConfidence()}] {j.GetDisplayName()}");
                }
            }

            return 0;
        }

        private static int ProcessLeftoversCommand(string[] args, bool isJson)
        {
            var cleanArgs = args.Where(x => !x.StartsWith("-") && !x.StartsWith("/")).ToArray();
            if (cleanArgs.Length < 1)
            {
                Console.WriteLine("Error: Missing target application name.");
                return 87;
            }

            var appName = cleanArgs[0];
            var isUnattended = args.Any(x => x.Equals("/U", StringComparison.OrdinalIgnoreCase) || x.Equals("--unattended", StringComparison.OrdinalIgnoreCase));
            var minConfidence = GetJunkConfidenceLevel(args) ?? ConfidenceLevel.Good;

            var apps = QueryApps(!isJson, false);
            var target = apps.FirstOrDefault(a => a.DisplayName != null && a.DisplayName.Contains(appName, StringComparison.OrdinalIgnoreCase));

            if (target == null)
            {
                Console.WriteLine($"Application '{appName}' not found.");
                return 1;
            }

            var junk = JunkManager.FindJunk(new[] { target }, apps, _ => { })
                .Where(j => j.Confidence.GetConfidence() >= minConfidence)
                .ToList();

            if (junk.Count == 0)
            {
                Console.WriteLine("No leftovers found matching the minimum confidence threshold.");
                return 0;
            }

            Console.WriteLine($"Found {junk.Count} leftover items:");
            foreach (var j in junk) Console.WriteLine($" - [{j.Confidence.GetConfidence()}] {j.GetDisplayName()}");

            if (!isUnattended)
            {
                Console.Write("Delete these leftover items? [y/N]: ");
                var key = Console.ReadKey().Key;
                Console.WriteLine();
                if (key != ConsoleKey.Y) return 1223;
            }

            var deleted = 0;
            foreach (var j in junk)
            {
                try { j.Delete(); deleted++; }
                catch (Exception ex) { Console.WriteLine($"Failed to delete {j.GetDisplayName()}: {ex.Message}"); }
            }

            Console.WriteLine($"Leftovers cleaning finished: {deleted}/{junk.Count} items deleted.");
            return 0;
        }

        private static int ProcessBackupCommand(string[] args, bool isJson)
        {
            var cleanArgs = args.Where(x => !x.StartsWith("-") && !x.StartsWith("/")).ToArray();
            if (cleanArgs.Length < 1)
            {
                Console.WriteLine("Error: Missing target application name.");
                return 87;
            }

            var appName = cleanArgs[0];
            var apps = QueryApps(!isJson, false);
            var target = apps.FirstOrDefault(a => a.DisplayName != null && a.DisplayName.Contains(appName, StringComparison.OrdinalIgnoreCase));

            if (target == null)
            {
                Console.WriteLine($"Application '{appName}' not found.");
                return 1;
            }

            var regKeys = new List<string>();
            if (!string.IsNullOrEmpty(target.RegistryPath)) regKeys.Add(target.RegistryPath);

            var files = new List<string>();
            if (!string.IsNullOrEmpty(target.InstallLocation) && Directory.Exists(target.InstallLocation))
                files.Add(target.InstallLocation);

            var customOut = GetArgValue(args, "--output");
            if (!string.IsNullOrEmpty(customOut)) BackupManager.BackupDirectory = customOut;

            var manifest = BackupManager.CreateBackup(target.DisplayName, target.DisplayVersion, target.Publisher, regKeys, files, true);

            if (isJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }));
            }
            else
            {
                Console.WriteLine($"Backup created successfully!");
                Console.WriteLine($"Backup ID: {manifest.BackupId}");
                Console.WriteLine($"Registry Entries: {manifest.RegistryEntries.Count}, Files: {manifest.FileEntries.Count}");
            }

            return 0;
        }

        private static int ProcessRestoreCommand(string[] args, bool isJson)
        {
            var cleanArgs = args.Where(x => !x.StartsWith("-") && !x.StartsWith("/")).ToArray();
            if (cleanArgs.Length < 1)
            {
                Console.WriteLine("Error: Missing backup ID.");
                return 87;
            }

            var backupId = cleanArgs[0];
            var success = BackupManager.RestoreBackup(backupId, out var restored, out var errors);

            if (isJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(new { success, restored, errors }, new JsonSerializerOptions { WriteIndented = true }));
            }
            else
            {
                if (success)
                {
                    Console.WriteLine($"Backup {backupId} restored successfully ({restored.Count} items restored).");
                }
                else
                {
                    Console.WriteLine($"Backup restore encountered errors:");
                    foreach (var err in errors) Console.WriteLine($" - {err}");
                }
            }

            return success ? 0 : 1;
        }

        private static int ProcessMonitorCommand(string[] args, bool isJson)
        {
            var cleanArgs = args.Where(x => !x.StartsWith("-") && !x.StartsWith("/")).ToArray();
            if (cleanArgs.Length < 1)
            {
                Console.WriteLine("Error: Missing installer executable path.");
                return 87;
            }

            var installerPath = cleanArgs[0];
            var customName = GetArgValue(args, "--name");

            Console.WriteLine($"Monitoring installer: {installerPath}...");
            var monitorTask = InstallationMonitorEngine.MonitorInstallerAsync(installerPath, customName,
                item => Console.WriteLine($" [Live Change] {item}"));

            monitorTask.Wait();
            var trace = monitorTask.Result;

            if (isJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(trace, new JsonSerializerOptions { WriteIndented = true }));
            }
            else
            {
                Console.WriteLine($"\nInstallation monitoring complete. Trace ID: {trace.TraceId}");
                Console.WriteLine($"Recorded {trace.Items.Count} total system changes.");
            }

            return 0;
        }

        private static int ProcessRollbackTraceCommand(string[] args, bool isJson)
        {
            var cleanArgs = args.Where(x => !x.StartsWith("-") && !x.StartsWith("/")).ToArray();
            if (cleanArgs.Length < 1)
            {
                Console.WriteLine("Error: Missing trace ID or file path.");
                return 87;
            }

            var traceId = cleanArgs[0];
            var trace = InstallationMonitorEngine.LoadTrace(traceId);
            if (trace == null)
            {
                Console.WriteLine($"Trace '{traceId}' not found.");
                return 1;
            }

            Console.WriteLine($"Rolling back trace for {trace.ApplicationName} ({trace.Items.Count} changes)...");
            var success = InstallationMonitorEngine.RollbackTrace(trace, out var removed, out var errors);

            if (isJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(new { success, removed, errors }, new JsonSerializerOptions { WriteIndented = true }));
            }
            else
            {
                Console.WriteLine($"Rollback finished: {removed.Count} items removed with {errors.Count} errors.");
            }

            return success ? 0 : 1;
        }

        private static int ProcessCleanJunkCommand(string[] args, bool isJson)
        {
            var doClean = args.Any(x => x.Equals("--clean", StringComparison.OrdinalIgnoreCase) || x.Equals("/C", StringComparison.OrdinalIgnoreCase));

            Console.WriteLine("Scanning junk files...");
            var scanTask = JunkCleanerEngine.ScanJunkAsync(null, msg => Console.WriteLine($" -> {msg}"));
            scanTask.Wait();
            var categories = scanTask.Result;

            if (isJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(categories, new JsonSerializerOptions { WriteIndented = true }));
            }
            else
            {
                Console.WriteLine("\nJunk Scan Results:");
                foreach (var c in categories)
                {
                    Console.WriteLine($" - {c.Name}: {c.ItemCount} items ({c.TotalSizeBytes / (1024.0 * 1024.0):F2} MB)");
                }
                Console.WriteLine($"\nTotal Junk: {categories.Sum(c => c.ItemCount)} items ({categories.Sum(c => c.TotalSizeBytes) / (1024.0 * 1024.0):F2} MB)");
            }

            if (doClean)
            {
                Console.WriteLine("\nCleaning junk files...");
                var cleanTask = JunkCleanerEngine.CleanJunkAsync(categories);
                cleanTask.Wait();
                var res = cleanTask.Result;
                Console.WriteLine($"Cleaned {res.DeletedFilesCount} files ({res.BytesFreed / (1024.0 * 1024.0):F2} MB freed).");
            }

            return 0;
        }

        private static int ProcessCleanPrivacyCommand(string[] args, bool isJson)
        {
            var doClean = args.Any(x => x.Equals("--clean", StringComparison.OrdinalIgnoreCase) || x.Equals("/C", StringComparison.OrdinalIgnoreCase));

            Console.WriteLine("Scanning privacy tracks...");
            var scanTask = PrivacyCleanerEngine.ScanPrivacyTracksAsync(msg => Console.WriteLine($" -> {msg}"));
            scanTask.Wait();
            var categories = scanTask.Result;

            if (isJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(categories, new JsonSerializerOptions { WriteIndented = true }));
            }
            else
            {
                Console.WriteLine("\nPrivacy Tracks Results:");
                foreach (var c in categories)
                {
                    Console.WriteLine($" - [{c.GroupName}] {c.ItemName}: {c.ItemCount} items");
                }
            }

            if (doClean)
            {
                Console.WriteLine("\nCleaning privacy tracks...");
                var cleanTask = PrivacyCleanerEngine.CleanPrivacyTracksAsync(categories);
                cleanTask.Wait();
                var res = cleanTask.Result;
                Console.WriteLine($"Cleaned {res.CleanedItemsCount} privacy items.");
            }

            return 0;
        }

        private static int ProcessStartupCommand(string[] args, bool isJson)
        {
            var entries = StartupManager.GetAllStartupEntries().ToList();

            if (isJson)
            {
                var list = entries.Select(e => new
                {
                    name = e.ProgramName,
                    command = e.FullCommand,
                    filepath = e.CommandFilePath,
                    location = e.ParentLongPath,
                    disabled = e.Disabled
                });
                Console.WriteLine(JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));
                return 0;
            }

            Console.WriteLine($@"{"Startup Name",-35}  {"Status",-10}  {"Location",-30}");
            Console.WriteLine(new string('-', 85));

            foreach (var e in entries)
            {
                var name = Truncate(e.ProgramName ?? string.Empty, 35);
                var status = e.Disabled ? "Disabled" : "Enabled";
                var loc = Truncate(e.ParentLongPath ?? string.Empty, 30);
                Console.WriteLine($@"{name,-35}  {status,-10}  {loc,-30}");
            }

            Console.WriteLine($"\nTotal Startup Entries: {entries.Count}");
            return 0;
        }

        private static int ProcessExtensionsCommand(string[] args, bool isJson)
        {
            var extTask = BrowserExtensionManager.GetInstalledExtensionsAsync();
            extTask.Wait();
            var exts = extTask.Result;

            if (isJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(exts, new JsonSerializerOptions { WriteIndented = true }));
                return 0;
            }

            Console.WriteLine($@"{"Browser",-15}  {"Extension Name",-35}  {"Version",-12}  {"ID",-32}");
            Console.WriteLine(new string('-', 100));

            foreach (var e in exts)
            {
                var br = Truncate(e.BrowserName, 15);
                var name = Truncate(e.Name, 35);
                var ver = Truncate(e.Version, 12);
                var id = Truncate(e.ExtensionId, 32);
                Console.WriteLine($@"{br,-15}  {name,-35}  {ver,-12}  {id,-32}");
            }

            Console.WriteLine($"\nTotal Browser Extensions: {exts.Count}");
            return 0;
        }

        private static int ProcessToolsCommand(string[] args, bool isJson)
        {
            var tools = WindowsToolsLauncher.GetAvailableTools();

            var launchTarget = GetArgValue(args, "--launch");
            if (!string.IsNullOrEmpty(launchTarget))
            {
                var tool = tools.FirstOrDefault(t => t.Name.Contains(launchTarget, StringComparison.OrdinalIgnoreCase));
                if (tool != null)
                {
                    Console.WriteLine($"Launching {tool.Name}...");
                    WindowsToolsLauncher.LaunchTool(tool);
                    return 0;
                }
                Console.WriteLine($"Tool '{launchTarget}' not found.");
                return 1;
            }

            if (isJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(tools, new JsonSerializerOptions { WriteIndented = true }));
                return 0;
            }

            Console.WriteLine("Built-in Windows Administrative Tools:");
            foreach (var t in tools)
            {
                Console.WriteLine($" - {t.Name,-30} [{t.Category}] {t.Description}");
            }

            return 0;
        }

        private static int ProcessExportCommand(string[] args, bool isJson)
        {
            var cleanArgs = args.Where(x => !x.StartsWith("-") && !x.StartsWith("/")).ToArray();
            if (cleanArgs.Length < 1)
            {
                Console.WriteLine("Error: Missing export file path.");
                return 87;
            }

            var outPath = cleanArgs[0];
            var apps = QueryApps(true, false);

            if (outPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                var json = JsonSerializer.Serialize(apps, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(outPath, json, Encoding.UTF8);
            }
            else
            {
                ApplicationEntrySerializer.SerializeApplicationEntries(outPath, apps);
            }

            Console.WriteLine($"Exported {apps.Count} applications to {outPath}");
            return 0;
        }

        private static int ProcessHistoryCommand(string[] args, bool isJson)
        {
            var history = OperationHistoryManager.GetHistory();

            if (isJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(history, new JsonSerializerOptions { WriteIndented = true }));
                return 0;
            }

            Console.WriteLine($@"{"Timestamp",-20}  {"Operation",-18}  {"Status",-10}  {"Application",-30}");
            Console.WriteLine(new string('-', 85));

            foreach (var h in history)
            {
                var time = h.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                var op = Truncate(h.OperationType, 18);
                var status = h.Status.ToString();
                var app = Truncate(h.ApplicationName ?? string.Empty, 30);
                Console.WriteLine($@"{time,-20}  {op,-18}  {status,-10}  {app,-30}");
            }

            Console.WriteLine($"\nTotal Recorded Operations: {history.Count}");
            return 0;
        }

        private static int ProcessHealthCommand(string[] args, bool isJson)
        {
            var apps = QueryApps(!isJson, false);
            var report = UninstallTools.Detection.SoftwareHealthEngine.AnalyzeSystemHealth(apps);

            if (isJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));
                return 0;
            }

            Console.WriteLine("\n================================================================================");
            Console.WriteLine($"SYSTEM HYGIENE REPORT - SCORE: {report.HygieneScore}/100");
            Console.WriteLine("================================================================================");
            Console.WriteLine($"Total Applications Analyzed : {report.TotalAppsAnalyzed}");
            Console.WriteLine($"Duplicate/Redundant Runtimes : {report.DuplicateRuntimesCount}");
            Console.WriteLine($"Abandoned AppData Folders   : {report.OrphanedFoldersCount}");
            Console.WriteLine($"Large Space Hogs (> 5 GB)   : {report.LargeAppsCount}");
            Console.WriteLine($"Potential Space Savings     : {report.TotalPotentialSavingsBytes / (1024 * 1024.0):F1} MB\n");

            Console.WriteLine("RECOMMENDATIONS:");
            foreach (var rec in report.Recommendations)
            {
                Console.WriteLine($" [{rec.Severity}] {rec.Title}");
                Console.WriteLine($"   -> {rec.Description}");
            }

            return 0;
        }

        private static int ProcessOptimizeRegistryCommand(string[] args, bool isJson)
        {
            var scan = UninstallTools.RegistryEngine.RegistryOptimizerEngine.ScanRegistryIssues();
            var isUnattended = args.Any(x => x.Equals("/U", StringComparison.OrdinalIgnoreCase) || x.Equals("--unattended", StringComparison.OrdinalIgnoreCase));
            var shouldFix = args.Any(x => x.Equals("--fix", StringComparison.OrdinalIgnoreCase) || x.Equals("-f", StringComparison.OrdinalIgnoreCase) || isUnattended);

            if (isJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(scan, new JsonSerializerOptions { WriteIndented = true }));
                return 0;
            }

            Console.WriteLine($"Found {scan.Issues.Count} registry issues across {scan.TotalKeysScanned} keys:");
            foreach (var iss in scan.Issues)
            {
                Console.WriteLine($" - [{iss.IssueType}] {iss.KeyPath} -> {iss.Description}");
            }

            if (scan.Issues.Count == 0)
            {
                Console.WriteLine("No registry integrity issues detected. Registry is healthy.");
                return 0;
            }

            if (!shouldFix)
            {
                Console.WriteLine("\nPass '--fix' or '/U' to automatically repair these registry issues.");
                return 0;
            }

            var fixedCount = UninstallTools.RegistryEngine.RegistryOptimizerEngine.FixRegistryIssues(scan.Issues, true);
            Console.WriteLine($"Registry optimization complete: {fixedCount} issues repaired.");
            return 0;
        }

        private static int ProcessUpdateCommand(string[] args, bool isJson)
        {
            var updateTask = UninstallTools.Core.UpdateManager.CheckForUpdatesAsync(true);
            updateTask.Wait();
            var update = updateTask.Result;

            if (isJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(update, new JsonSerializerOptions { WriteIndented = true }));
                return 0;
            }

            Console.WriteLine($"Current Version : {update.CurrentVersion}");
            Console.WriteLine($"Latest Version  : {update.LatestVersion ?? "Unknown"}");
            Console.WriteLine($"Update Available: {(update.IsUpdateAvailable ? "YES" : "No (Up to date)")}");

            if (update.IsUpdateAvailable)
            {
                Console.WriteLine($"\nDownload URL: {update.DownloadUrl}");
                if (!string.IsNullOrEmpty(update.ReleaseNotes))
                    Console.WriteLine($"\nRelease Notes:\n{update.ReleaseNotes}");
            }

            return 0;
        }

        private static int ProcessTrimMemoryCommand(string[] args, bool isJson)
        {
            var res = UninstallTools.SystemTools.MemoryTrimmerEngine.TrimSystemWorkingSet();
            if (isJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(res, new JsonSerializerOptions { WriteIndented = true }));
                return 0;
            }

            Console.WriteLine($"Trimmed {res.TotalProcessesTrimmed}/{res.TotalProcessesInspected} running process working sets.");
            Console.WriteLine($"Estimated RAM reclaimed: ~{res.EstimatedMemoryReclaimedBytes / (1024 * 1024.0):F1} MB.");
            return 0;
        }

        private static int ProcessCleanDriversCommand(string[] args, bool isJson)
        {
            var drivers = UninstallTools.JunkCleaner.DeviceDriverResidualsCleaner.ScanDriverResiduals();
            if (isJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(drivers, new JsonSerializerOptions { WriteIndented = true }));
                return 0;
            }

            Console.WriteLine($"Found {drivers.Count} hardware / peripheral driver entries:");
            foreach (var d in drivers)
            {
                Console.WriteLine($" - [{d.DriverClass}] {d.DeviceName} ({d.HardwareId})");
            }
            return 0;
        }

        private static int ProcessStartupImpactCommand(string[] args, bool isJson)
        {
            var startups = StartupManager.GetAllStartupEntries().ToList();
            var report = UninstallTools.Startup.StartupImpactAnalyzer.AnalyzeStartupItems(startups);

            if (isJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));
                return 0;
            }

            Console.WriteLine($"Total Startup Entries: {report.TotalStartupEntries} ({report.HighImpactCount} high impact, {report.DisabledCount} disabled)\n");
            foreach (var r in report.Recommendations)
            {
                Console.WriteLine($" [{r.Impact}] {r.Entry.ProgramName} (Disabled: {r.Entry.Disabled})");
                Console.WriteLine($"   -> {r.RecommendationReason}");
            }
            return 0;
        }

        private static int ProcessRuntimesCommand(string[] args, bool isJson)
        {
            var runtimes = WindowsRuntimesManager.ScanInstalledRuntimes();

            if (isJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(runtimes, new JsonSerializerOptions { WriteIndented = true }));
                return 0;
            }

            Console.WriteLine($@"{"Runtime Name",-45}  {"Category",-12}  {"Version",-15}  {"Arch",-6}  {"Status",-12}");
            Console.WriteLine(new string('-', 98));

            foreach (var r in runtimes)
            {
                var name = Truncate(r.Name, 45);
                var cat = r.Category.ToString();
                var ver = Truncate(r.Version, 15);
                var arch = r.Architecture;
                var status = r.IsSuperseded ? "Superseded" : "Active";
                Console.WriteLine($@"{name,-45}  {cat,-12}  {ver,-15}  {arch,-6}  {status,-12}");
            }

            Console.WriteLine($"\nTotal Runtimes Detected: {runtimes.Count}");
            return 0;
        }

        private static int ProcessRegistryBloatCommand(string[] args, bool isJson)
        {
            var doClean = args.Any(x => x.Equals("--clean", StringComparison.OrdinalIgnoreCase) || x.Equals("/C", StringComparison.OrdinalIgnoreCase));
            var scan = RegistryBloatAnalyzer.ScanAllBloat();

            if (isJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(scan, new JsonSerializerOptions { WriteIndented = true }));
                return 0;
            }

            Console.WriteLine($"Registry Bloat Scan Results ({scan.TotalCount} items detected in {scan.Duration.TotalSeconds:F2}s):");
            Console.WriteLine($" - Orphaned CLSIDs       : {scan.OrphanedClsidsCount}");
            Console.WriteLine($" - Stale App Paths        : {scan.StaleAppPathsCount}");
            Console.WriteLine($" - Invalid SharedDLLs     : {scan.InvalidSharedDllsCount}");
            Console.WriteLine($" - Obsolete MUICache      : {scan.ObsoleteMuiCacheCount}\n");

            if (doClean && scan.TotalCount > 0)
            {
                var backupDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EBUninstaller", "Backups");
                var cleaned = RegistryBloatAnalyzer.CleanBloatItems(scan.Items, backupDir);
                Console.WriteLine($"Cleaned {cleaned} registry bloat entries. Backup saved to {backupDir}.");
            }
            else if (scan.TotalCount > 0)
            {
                Console.WriteLine("Pass '--clean' to remove these detected bloat entries.");
            }

            return 0;
        }

        private static int ProcessOrphanedServicesCommand(string[] args, bool isJson)
        {
            var doClean = args.Any(x => x.Equals("--clean", StringComparison.OrdinalIgnoreCase) || x.Equals("/C", StringComparison.OrdinalIgnoreCase));
            var services = OrphanedServicesCleaner.ScanOrphanedServices();

            if (isJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(services, new JsonSerializerOptions { WriteIndented = true }));
                return 0;
            }

            Console.WriteLine($"Detected {services.Count} orphaned Windows service(s):");
            foreach (var s in services)
            {
                Console.WriteLine($" - {s.ServiceName,-25} [{s.StartTypeName}] {s.ParsedExecutablePath}");
            }

            if (doClean && services.Count > 0)
            {
                var backupDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EBUninstaller", "Backups");
                var res = OrphanedServicesCleaner.RemoveOrphanedServices(services, backupDir);
                Console.WriteLine($"\nRemoved {res.CleanedCount} orphaned services. Backup: {res.BackupRegPath}");
            }

            return 0;
        }

        private static int ProcessExportDriversCommand(string[] args, bool isJson)
        {
            var cleanArgs = args.Where(x => !x.StartsWith("-") && !x.StartsWith("/")).ToArray();
            var targetDir = cleanArgs.Length > 0 ? cleanArgs[0] : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DriversBackup");

            Console.WriteLine($"Exporting OEM third-party drivers to {targetDir}...");
            var res = WindowsDriverBackupEngine.ExportDrivers(targetDir);

            if (isJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(res, new JsonSerializerOptions { WriteIndented = true }));
                return res.Success ? 0 : 1;
            }

            if (res.Success)
            {
                Console.WriteLine($"Driver export complete! {res.ExportedCount} drivers backed up in {res.Duration.TotalSeconds:F1}s.");
                Console.WriteLine($"Manifest created: {res.ManifestPath}");
            }
            else
            {
                Console.WriteLine($"Driver export failed: {res.ErrorMessage}");
            }

            return res.Success ? 0 : 1;
        }

        private static int ProcessAuditReportCommand(string[] args, bool isJson)
        {
            var cleanArgs = args.Where(x => !x.StartsWith("-") && !x.StartsWith("/")).ToArray();
            var outPath = cleanArgs.Length > 0 ? cleanArgs[0] : "Software_Audit_Report.html";

            var apps = QueryApps(true, false);
            var reportItems = apps.Select(a => new UninstallTools.Reporting.ReportSoftwareItem
            {
                DisplayName = a.DisplayName ?? string.Empty,
                DisplayVersion = a.DisplayVersion ?? string.Empty,
                Publisher = a.Publisher ?? string.Empty,
                InstallDate = a.InstallDate.ToString("yyyy-MM-dd"),
                InstallLocation = a.InstallLocation ?? string.Empty,
                EstimatedSizeBytes = a.EstimatedSize.GetSizeBytes(),
                Architecture = a.Is64Bit.ToString(),
                UninstallerType = a.UninstallerKind.ToString(),
                IsValidSigned = a.IsRegistered,
                SafetyScore = "Safe"
            }).ToList();

            string content;
            if (outPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                content = UninstallTools.Reporting.SoftwareInventoryReportGenerator.GenerateJsonReport(reportItems);
            }
            else if (outPath.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            {
                content = UninstallTools.Reporting.SoftwareInventoryReportGenerator.GenerateMarkdownReport(reportItems);
            }
            else if (outPath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                content = UninstallTools.Reporting.SoftwareInventoryReportGenerator.GenerateCsvReport(reportItems);
            }
            else
            {
                content = UninstallTools.Reporting.SoftwareInventoryReportGenerator.GenerateHtmlReport(reportItems);
            }

            File.WriteAllText(outPath, content, Encoding.UTF8);
            Console.WriteLine($"Software audit report generated successfully: {outPath}");
            return 0;
        }

        #endregion

        #region Helpers

        private static IList<ApplicationUninstallerEntry> QueryApps(bool showProgress, bool isVerbose)
        {
            UninstallToolsGlobalConfig.ScanWinUpdates = false;
            UninstallToolsGlobalConfig.QuietAutomatization = true;
            UninstallToolsGlobalConfig.EnableAppInfoCache = false;

            if (!showProgress)
                return ApplicationUninstallerFactory.GetUninstallerEntries(_ => { });

            Console.WriteLine("Scanning system for installed applications...");
            string lastMsg = null;
            var results = ApplicationUninstallerFactory.GetUninstallerEntries(report =>
            {
                if (report.Message != lastMsg)
                {
                    lastMsg = report.Message;
                    Console.WriteLine($" -> {report.Message}");
                }
            });

            Console.WriteLine($"Scan completed. Discovered {results.Count} installed applications.");
            return results;
        }

        private static string GetArgValue(string[] args, string flag)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].Equals(flag, StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
                    return args[i + 1];
                if (args[i].StartsWith(flag + "=", StringComparison.OrdinalIgnoreCase))
                    return args[i].Substring(flag.Length + 1);
            }
            return null;
        }

        private static ConfidenceLevel? GetJunkConfidenceLevel(string[] args)
        {
            var junkArg = args.FirstOrDefault(a => a.StartsWith("/J=", StringComparison.OrdinalIgnoreCase) ||
                                                   a.StartsWith("--junk=", StringComparison.OrdinalIgnoreCase) ||
                                                   a.Equals("/J", StringComparison.OrdinalIgnoreCase) ||
                                                   a.Equals("--junk", StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(junkArg)) return null;

            if (junkArg.Contains("="))
            {
                var val = junkArg.Split('=')[1];
                if (Enum.TryParse<ConfidenceLevel>(val, true, out var parsed))
                    return parsed;
            }

            return ConfidenceLevel.VeryGood;
        }

        private static string Truncate(string val, int maxLen)
        {
            if (string.IsNullOrEmpty(val)) return string.Empty;
            return val.Length <= maxLen ? val : val.Substring(0, maxLen - 3) + "...";
        }

        private static void WaitForKeyIfStandalone()
        {
            if (Console.IsInputRedirected || !LaunchedStandalone()) return;
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            try { Console.ReadKey(true); } catch { }
        }

        private static bool LaunchedStandalone()
        {
            try
            {
                var processIds = new uint[2];
                var count = GetConsoleProcessList(processIds, (uint)processIds.Length);
                return count == 1;
            }
            catch { return false; }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint GetConsoleProcessList(uint[] lpdwProcessList, uint dwProcessCount);

        #endregion
    }
}
