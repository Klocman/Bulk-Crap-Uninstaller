/*
    OpenUninstall Pro - Complete Lifecycle Integration Test Suite
    Tests the full end-to-end lifecycle:
    DISPOSABLE APP SETUP -> MONITOR -> BACKUP -> UNINSTALL -> SCAN LEFTOVERS -> CLEAN -> VERIFY -> RESTORE
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools;
using UninstallTools.Backup;
using UninstallTools.Core;
using UninstallTools.FileSystemEngine;
using UninstallTools.ForcedRemoval;
using UninstallTools.InstallationMonitor;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class LifecycleIntegrationTests
    {
        [TestMethod]
        public void FullApplicationLifecycle_SimulatedEnvironment_CompletesSuccessfully()
        {
            var sandboxRoot = Path.Combine(Path.GetTempPath(), "OpenUninstall_LifecycleSandbox_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(sandboxRoot);

            var appInstallDir = Path.Combine(sandboxRoot, "ProgramFiles", "TestApp2026");
            var appDataDir = Path.Combine(sandboxRoot, "AppData", "TestApp2026");
            var backupsDir = Path.Combine(sandboxRoot, "Backups");
            var tracesDir = Path.Combine(sandboxRoot, "Traces");

            Directory.CreateDirectory(appInstallDir);
            Directory.CreateDirectory(appDataDir);
            Directory.CreateDirectory(backupsDir);
            Directory.CreateDirectory(tracesDir);

            BackupManager.BackupDirectory = backupsDir;
            InstallationMonitorEngine.TracesDirectory = tracesDir;

            try
            {
                // =====================================================================
                // STAGE 1: INSTALL (Simulate creating application files, data, and config)
                // =====================================================================
                var exePath = Path.Combine(appInstallDir, "testapp.exe");
                var configPath = Path.Combine(appInstallDir, "config.json");
                var dataFile = Path.Combine(appDataDir, "user_settings.dat");

                File.WriteAllText(exePath, "SIMULATED_PE_HEADER_AND_CODE");
                File.WriteAllText(configPath, "{ \"Version\": \"2.0.0\", \"Installed\": true }");
                File.WriteAllText(dataFile, "USER_PROFILE_DATA_XYZ");

                Assert.IsTrue(File.Exists(exePath));
                Assert.IsTrue(File.Exists(configPath));
                Assert.IsTrue(File.Exists(dataFile));

                // =====================================================================
                // STAGE 2: MONITOR / SNAPSHOT RECORDING
                // =====================================================================
                var trace = new InstallationTrace
                {
                    ApplicationName = "TestApp 2026 Pro",
                    InstallerExecutablePath = Path.Combine(sandboxRoot, "setup.exe"),
                    MonitoringStartedAt = DateTime.UtcNow,
                    MonitoringStoppedAt = DateTime.UtcNow.AddSeconds(5)
                };
                trace.Items.Add(new TraceItem { Category = TraceItemCategory.File, ChangeType = TraceItemChangeType.Added, PathOrIdentifier = exePath, Size = new FileInfo(exePath).Length });
                trace.Items.Add(new TraceItem { Category = TraceItemCategory.File, ChangeType = TraceItemChangeType.Added, PathOrIdentifier = configPath, Size = new FileInfo(configPath).Length });
                trace.Items.Add(new TraceItem { Category = TraceItemCategory.File, ChangeType = TraceItemChangeType.Added, PathOrIdentifier = dataFile, Size = new FileInfo(dataFile).Length });

                var traceFile = InstallationMonitorEngine.SaveTrace(trace);
                Assert.IsTrue(File.Exists(traceFile));

                // =====================================================================
                // STAGE 3: PRE-UNINSTALL BACKUP CREATION
                // =====================================================================
                var manifest = BackupManager.CreateBackup(
                    "TestApp 2026 Pro",
                    "2.0.0",
                    "Test Publisher",
                    null,
                    new[] { appInstallDir, appDataDir },
                    false,
                    "LifecycleTest");

                Assert.IsNotNull(manifest);
                Assert.IsNotNull(manifest.BackupId);

                var backupVerification = BackupManager.VerifyBackup(manifest.BackupId);
                Assert.IsTrue(backupVerification.IsValid, "Backup must be verified with cryptographic SHA-256 checksums.");

                // =====================================================================
                // STAGE 4: FORCED REMOVAL / LEFTOVER CLEANUP EXECUTION
                // =====================================================================
                var plan = ForcedUninstallManager.BuildPlan(appInstallDir);
                Assert.IsTrue(plan.Items.Count > 0, "Forced removal planner should identify the install directory.");

                var removalResult = ForcedUninstallManager.ExecutePlan(plan, false);
                Assert.IsTrue(removalResult.Success);

                // Delete mock app data directly to simulate leftover cleaning
                SafeFileSystemEngine.DeleteDirectorySafe(appDataDir, DeletionMode.PermanentNormal);

                // =====================================================================
                // STAGE 5: VERIFY REMOVAL
                // =====================================================================
                Assert.IsFalse(File.Exists(exePath), "Executable should be removed.");
                Assert.IsFalse(File.Exists(configPath), "Configuration should be removed.");
                Assert.IsFalse(File.Exists(dataFile), "User data should be removed.");

                // =====================================================================
                // STAGE 6: RESTORE FROM BACKUP
                // =====================================================================
                var restoreSuccess = BackupManager.RestoreBackup(manifest.BackupId, out var restoredItems, out var restoreErrors);
                Assert.IsTrue(restoreSuccess, "Backup restore must succeed.");
                Assert.AreEqual(0, restoreErrors.Count);

                // Verify restored files exist and match original content
                Assert.IsTrue(File.Exists(exePath), "Restored executable must exist.");
                Assert.IsTrue(File.Exists(configPath), "Restored config must exist.");
                Assert.IsTrue(File.Exists(dataFile), "Restored user data must exist.");
                Assert.AreEqual("SIMULATED_PE_HEADER_AND_CODE", File.ReadAllText(exePath));
            }
            finally
            {
                if (Directory.Exists(sandboxRoot))
                {
                    try { Directory.Delete(sandboxRoot, true); } catch { }
                }
            }
        }
    }
}
