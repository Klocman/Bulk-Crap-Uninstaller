/*
    OpenUninstall Pro - Open Source Professional Windows Uninstaller
    Unified Uninstallation Pipeline Models & Engine
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UninstallTools.Backup;
using UninstallTools.Core;
using UninstallTools.Detection;
using UninstallTools.ForcedRemoval;
using UninstallTools.History;
using UninstallTools.Junk;
using UninstallTools.Junk.Containers;

namespace UninstallTools.Uninstaller
{
    public enum PipelinePhase
    {
        DiscoverAndValidate,
        CreateBackup,
        CreateRestorePoint,
        RunOfficialUninstaller,
        WaitForCompletion,
        ReScanSystem,
        DetectLeftovers,
        ClassifyAndPreview,
        RemoveApprovedItems,
        VerifyRemoval,
        GenerateReport
    }

    public sealed class PipelineProgressEventArgs : EventArgs
    {
        public PipelinePhase Phase { get; set; }
        public int Percentage { get; set; }
        public string StatusMessage { get; set; }
    }

    public sealed class PipelineResult
    {
        public string PipelineExecutionId { get; set; } = Guid.NewGuid().ToString("N");
        public string ApplicationName { get; set; }
        public bool OfficialUninstallerSucceeded { get; set; }
        public string BackupId { get; set; }
        public List<IJunkResult> DetectedLeftovers { get; } = new();
        public List<IJunkResult> ApprovedLeftovers { get; } = new();
        public int RemovedLeftoversCount { get; set; }
        public int FailedLeftoversCount { get; set; }
        public bool RebootRequired { get; set; }
        public List<string> Warnings { get; } = new();
        public List<string> Errors { get; } = new();
        public TimeSpan Duration { get; set; }
        public bool Success => Errors.Count == 0 && (OfficialUninstallerSucceeded || RemovedLeftoversCount > 0);
    }

    public sealed class PipelineOptions
    {
        public bool SilentMode { get; set; } = true;
        public bool CreateBackup { get; set; } = true;
        public bool CreateSystemRestorePoint { get; set; } = true;
        public bool AutoRemoveHighConfidenceLeftovers { get; set; } = true;
        public Junk.Confidence.ConfidenceLevel MinimumJunkConfidence { get; set; } = Junk.Confidence.ConfidenceLevel.Good;
        public int UninstallerTimeoutSeconds { get; set; } = 300;
    }

    public static class UninstallPipeline
    {
        /// <summary>
        /// Executes the unified multi-phase uninstallation pipeline for a target application.
        /// </summary>
        public static async Task<PipelineResult> ExecuteAsync(
            ApplicationUninstallerEntry targetApp,
            ICollection<ApplicationUninstallerEntry> allApps,
            PipelineOptions options = null,
            EventHandler<PipelineProgressEventArgs> onProgress = null,
            Func<List<IJunkResult>, Task<List<IJunkResult>>> userApprovalCallback = null,
            CancellationToken cancellationToken = default)
        {
            if (targetApp == null) throw new ArgumentNullException(nameof(targetApp));
            options ??= new PipelineOptions();

            var result = new PipelineResult { ApplicationName = targetApp.DisplayName };
            var stopwatch = Stopwatch.StartNew();

            void Report(PipelinePhase phase, int pct, string msg)
            {
                StructuredLogger.Info(LogCategory.Uninstaller, $"[Pipeline: {phase}] {msg}");
                onProgress?.Invoke(null, new PipelineProgressEventArgs { Phase = phase, Percentage = pct, StatusMessage = msg });
            }

            try
            {
                // Phase 1: Discover & Validate
                Report(PipelinePhase.DiscoverAndValidate, 5, $"Validating uninstallation parameters for {targetApp.DisplayName}...");
                var confidence = ConfidenceScorer.CalculateConfidence(targetApp);
                if (targetApp.IsProtected)
                {
                    result.Errors.Add("This application is protected by policy and cannot be uninstalled.");
                    return result;
                }

                // Phase 2: Create Backup
                if (options.CreateBackup)
                {
                    Report(PipelinePhase.CreateBackup, 15, "Creating pre-uninstallation backup...");
                    try
                    {
                        var regKeys = new List<string>();
                        if (!string.IsNullOrEmpty(targetApp.RegistryPath))
                            regKeys.Add(targetApp.RegistryPath);

                        var filePaths = new List<string>();
                        if (!string.IsNullOrEmpty(targetApp.InstallLocation) && Directory.Exists(targetApp.InstallLocation))
                            filePaths.Add(targetApp.InstallLocation);

                        var backup = BackupManager.CreateBackup(
                            targetApp.DisplayName,
                            targetApp.DisplayVersion,
                            targetApp.Publisher,
                            regKeys,
                            filePaths,
                            options.CreateSystemRestorePoint,
                            "PipelineUninstall");

                        result.BackupId = backup.BackupId;
                    }
                    catch (Exception ex)
                    {
                        result.Warnings.Add($"Pre-backup warning: {ex.Message}");
                    }
                }

                // Phase 3 & 4: Run Official Uninstaller
                Report(PipelinePhase.RunOfficialUninstaller, 30, $"Executing uninstaller for {targetApp.DisplayName}...");
                var uninstalledSuccessfully = false;

                if (targetApp.UninstallPossible)
                {
                    try
                    {
                        var uninstallCmd = (options.SilentMode && targetApp.QuietUninstallPossible)
                            ? targetApp.QuietUninstallString
                            : targetApp.UninstallString;

                        if (string.IsNullOrEmpty(uninstallCmd))
                            uninstallCmd = targetApp.UninstallString;

                        if (!string.IsNullOrEmpty(uninstallCmd))
                        {
                            var proc = targetApp.RunUninstaller(options.SilentMode && targetApp.QuietUninstallPossible);
                            if (proc != null)
                            {
                                Report(PipelinePhase.WaitForCompletion, 45, "Waiting for uninstaller process to complete...");
                                var exited = await Task.Run(() => proc.WaitForExit(options.UninstallerTimeoutSeconds * 1000), cancellationToken).ConfigureAwait(false);
                                if (!exited)
                                {
                                    result.Warnings.Add("Uninstaller timed out.");
                                }
                                else
                                {
                                    uninstalledSuccessfully = (proc.ExitCode == 0 || proc.ExitCode == 3010);
                                    if (proc.ExitCode == 3010)
                                    {
                                        result.RebootRequired = true;
                                        Report(PipelinePhase.WaitForCompletion, 50, "Uninstaller completed (Reboot required).");
                                    }
                                }
                            }
                            else
                            {
                                uninstalledSuccessfully = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Warnings.Add($"Official uninstaller encountered error: {ex.Message}");
                    }
                }

                result.OfficialUninstallerSucceeded = uninstalledSuccessfully;

                // Phase 5 & 6: Re-Scan System & Detect Leftovers
                Report(PipelinePhase.DetectLeftovers, 65, "Scanning for residual files, folders, and registry entries...");
                var junkItems = new List<IJunkResult>();
                try
                {
                    junkItems = JunkManager.FindJunk(
                        new[] { targetApp },
                        allApps ?? new[] { targetApp },
                        p => { }).ToList();
                }
                catch (Exception ex)
                {
                    result.Warnings.Add($"Leftover scan warning: {ex.Message}");
                }

                result.DetectedLeftovers.AddRange(junkItems);

                // Phase 7: Classify & Preview
                Report(PipelinePhase.ClassifyAndPreview, 80, $"Classified {junkItems.Count} leftover items.");
                var approvedJunk = new List<IJunkResult>();

                if (userApprovalCallback != null)
                {
                    approvedJunk = await userApprovalCallback(junkItems).ConfigureAwait(false);
                }
                else if (options.AutoRemoveHighConfidenceLeftovers)
                {
                    approvedJunk = junkItems.Where(j => j.Confidence.GetConfidence() >= options.MinimumJunkConfidence).ToList();
                }

                result.ApprovedLeftovers.AddRange(approvedJunk);

                // Phase 8: Remove Approved Items
                if (approvedJunk.Count > 0)
                {
                    Report(PipelinePhase.RemoveApprovedItems, 90, $"Removing {approvedJunk.Count} approved leftover items...");
                    foreach (var junk in approvedJunk)
                    {
                        if (cancellationToken.IsCancellationRequested) break;

                        try
                        {
                            junk.Delete();
                            result.RemovedLeftoversCount++;
                        }
                        catch (Exception ex)
                        {
                            result.FailedLeftoversCount++;
                            result.Errors.Add($"Failed to delete leftover '{junk.GetDisplayName()}': {ex.Message}");
                        }
                    }
                }

                // Phase 9: Verify & Generate Report
                Report(PipelinePhase.GenerateReport, 100, "Uninstallation pipeline finished.");
                stopwatch.Stop();
                result.Duration = stopwatch.Elapsed;

                // Record in history
                OperationHistoryManager.RecordOperation(new OperationHistoryEntry
                {
                    ApplicationName = targetApp.DisplayName,
                    Publisher = targetApp.Publisher,
                    OperationType = "PipelineUninstall",
                    Status = result.Success ? HistoryOperationStatus.Success : HistoryOperationStatus.Partial,
                    DetectedItemsCount = result.DetectedLeftovers.Count,
                    DeletedItemsCount = result.RemovedLeftoversCount + (result.OfficialUninstallerSucceeded ? 1 : 0),
                    FailedItemsCount = result.FailedLeftoversCount,
                    BackupId = result.BackupId,
                    Warnings = result.Warnings,
                    Errors = result.Errors
                });

                return result;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Pipeline error: {ex.Message}");
                StructuredLogger.Error(LogCategory.Uninstaller, $"Pipeline execution failed for {targetApp.DisplayName}", ex.Message);
                return result;
            }
        }
    }
}
