/*
    OpenUninstall Pro - Unit Tests for Installation Monitor and Backup Subsystems
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Backup;
using UninstallTools.InstallationMonitor;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class InstallationMonitorTests
    {
        [TestMethod]
        public void CompareSnapshots_DetectsAddedFilesAndRegistryKeys()
        {
            var before = new InstallationSnapshot
            {
                SnapshotId = "Before-001"
            };
            before.FileMetadata["C:\\App\\old_file.dll"] = 1024;
            before.RegistryKeys.Add(@"HKEY_LOCAL_MACHINE\Software\ExistingApp");

            var after = new InstallationSnapshot
            {
                SnapshotId = "After-001"
            };
            after.FileMetadata["C:\\App\\old_file.dll"] = 2048; // Modified
            after.FileMetadata["C:\\App\\new_program.exe"] = 4096; // Added
            after.RegistryKeys.Add(@"HKEY_LOCAL_MACHINE\Software\ExistingApp");
            after.RegistryKeys.Add(@"HKEY_LOCAL_MACHINE\Software\NewAppKey"); // Added

            var diff = InstallationMonitorEngine.CompareSnapshots(before, after);

            Assert.IsNotNull(diff);
            Assert.AreEqual(2, diff.AddedItems.Count); // 1 file + 1 reg key
            Assert.AreEqual(1, diff.ModifiedItems.Count); // 1 modified file
            Assert.AreEqual(0, diff.RemovedItems.Count);

            var addedFile = diff.AddedItems.FirstOrDefault(i => i.Category == TraceItemCategory.File);
            Assert.IsNotNull(addedFile);
            Assert.AreEqual("C:\\App\\new_program.exe", addedFile.PathOrIdentifier);

            var addedKey = diff.AddedItems.FirstOrDefault(i => i.Category == TraceItemCategory.RegistryKey);
            Assert.IsNotNull(addedKey);
            Assert.AreEqual(@"HKEY_LOCAL_MACHINE\Software\NewAppKey", addedKey.PathOrIdentifier);
        }

        [TestMethod]
        public void TraceSerialization_RoundTrip_PreservesProperties()
        {
            var trace = new InstallationTrace
            {
                TraceId = "Trace-Test-123",
                ApplicationName = "Test Game 2026",
                InstallerExecutablePath = "C:\\Downloads\\setup.exe",
                MonitoringStartedAt = DateTime.UtcNow,
                MonitoringStoppedAt = DateTime.UtcNow.AddMinutes(2)
            };

            trace.Items.Add(new TraceItem
            {
                Category = TraceItemCategory.File,
                ChangeType = TraceItemChangeType.Added,
                PathOrIdentifier = "C:\\Games\\TestGame\\game.exe",
                Size = 10485760
            });

            var tempDir = Path.Combine(Path.GetTempPath(), "OpenUninstall_Test_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            try
            {
                var filePath = InstallationMonitorEngine.SaveTrace(trace, tempDir);
                Assert.IsTrue(File.Exists(filePath));

                var loaded = InstallationMonitorEngine.LoadTrace(filePath);
                Assert.IsNotNull(loaded);
                Assert.AreEqual(trace.TraceId, loaded.TraceId);
                Assert.AreEqual(trace.ApplicationName, loaded.ApplicationName);
                Assert.AreEqual(1, loaded.Items.Count);
                Assert.AreEqual(trace.Items[0].PathOrIdentifier, loaded.Items[0].PathOrIdentifier);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }
    }

    [TestClass]
    public class BackupManagerTests
    {
        [TestMethod]
        public void CreateBackup_WithMockFiles_GeneratesValidManifestAndArchive()
        {
            var tempBase = Path.Combine(Path.GetTempPath(), "OpenUninstall_BackupTest_" + Guid.NewGuid().ToString("N"));
            var mockAppDir = Path.Combine(tempBase, "MockApp");
            Directory.CreateDirectory(mockAppDir);

            var file1 = Path.Combine(mockAppDir, "app.exe");
            var file2 = Path.Combine(mockAppDir, "config.ini");
            File.WriteAllText(file1, "MOCK_EXECUTABLE_CONTENT_12345");
            File.WriteAllText(file2, "Setting=Value");

            var backupDir = Path.Combine(tempBase, "Backups");
            BackupManager.BackupDirectory = backupDir;

            try
            {
                var manifest = BackupManager.CreateBackup(
                    "MockApp",
                    "1.0",
                    "Mock Publisher",
                    null,
                    new[] { mockAppDir },
                    false,
                    "UnitTest");

                Assert.IsNotNull(manifest);
                Assert.IsNotNull(manifest.BackupId);
                Assert.AreEqual("MockApp", manifest.ApplicationName);
                Assert.AreEqual(2, manifest.FileEntries.Count);

                // Verify backup integrity
                var verifyResult = BackupManager.VerifyBackup(manifest.BackupId);
                Assert.IsTrue(verifyResult.IsValid);
                Assert.AreEqual(0, verifyResult.CorruptedItemsCount);
                Assert.AreEqual(0, verifyResult.MissingItemsCount);

                // List backups
                var summaries = BackupManager.ListBackups();
                Assert.IsTrue(summaries.Any(s => s.BackupId == manifest.BackupId));
            }
            finally
            {
                if (Directory.Exists(tempBase))
                    Directory.Delete(tempBase, true);
            }
        }
    }
}
