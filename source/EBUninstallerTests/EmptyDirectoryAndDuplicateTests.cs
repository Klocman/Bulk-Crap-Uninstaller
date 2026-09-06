/*
    EBUninstaller Pro - Empty Directory & Duplicate Scanner Tests
    Unit and integration tests for empty directory and duplicate file cleanup.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.FileSystemEngine;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class EmptyDirectoryAndDuplicateTests
    {
        private string _tempTestDir = string.Empty;

        [TestInitialize]
        public void Setup()
        {
            _tempTestDir = Path.Combine(Path.GetTempPath(), "EBUninstaller_Test_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempTestDir);
        }

        [TestCleanup]
        public void TearDown()
        {
            try
            {
                if (Directory.Exists(_tempTestDir))
                    Directory.Delete(_tempTestDir, true);
            }
            catch
            {
                // Ignore cleanup errors in tests
            }
        }

        [TestMethod]
        public void TestScanAndCleanEmptyDirectories()
        {
            var emptySubdir1 = Path.Combine(_tempTestDir, "EmptyFolder1");
            var emptySubdir2 = Path.Combine(_tempTestDir, "EmptyFolder2", "NestedEmpty");
            var nonEmptySubdir = Path.Combine(_tempTestDir, "NonEmptyFolder");

            Directory.CreateDirectory(emptySubdir1);
            Directory.CreateDirectory(emptySubdir2);
            Directory.CreateDirectory(nonEmptySubdir);
            File.WriteAllText(Path.Combine(nonEmptySubdir, "test.txt"), "EBUninstaller Pro Unit Test Data");

            var results = EmptyDirectoryCleaner.ScanForEmptyDirectories(new[] { _tempTestDir });
            Assert.IsNotNull(results);
            Assert.IsTrue(results.Any(r => r.Path == emptySubdir1), "EmptyFolder1 should be identified as empty.");
            Assert.IsTrue(results.Any(r => r.Path == emptySubdir2), "NestedEmpty should be identified as empty.");
            Assert.IsFalse(results.Any(r => r.Path == nonEmptySubdir), "NonEmptyFolder should not be identified as empty.");

            int deleted = EmptyDirectoryCleaner.DeleteEmptyDirectories(results);
            Assert.IsTrue(deleted >= 2);
            Assert.IsFalse(Directory.Exists(emptySubdir1));
            Assert.IsTrue(Directory.Exists(nonEmptySubdir));
        }

        [TestMethod]
        public void TestScanAndCleanDuplicateFiles()
        {
            var file1 = Path.Combine(_tempTestDir, "original.bin");
            var file2 = Path.Combine(_tempTestDir, "duplicate_copy.bin");
            var file3 = Path.Combine(_tempTestDir, "different.bin");

            byte[] identicalBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            byte[] differentBytes = new byte[] { 99, 98, 97, 96, 95, 94, 93, 92, 91, 90, 89, 88, 87, 86, 85, 84 };

            File.WriteAllBytes(file1, identicalBytes);
            File.WriteAllBytes(file2, identicalBytes);
            File.WriteAllBytes(file3, differentBytes);

            var duplicateGroups = DuplicateFileScanner.ScanForDuplicates(new[] { _tempTestDir }, minFileSizeBytes: 1);
            Assert.IsNotNull(duplicateGroups);
            Assert.AreEqual(1, duplicateGroups.Count, "Exactly one duplicate group should be found.");

            var group = duplicateGroups[0];
            Assert.AreEqual(2, group.Files.Count);
            Assert.IsTrue(group.Files.Any(f => f.IsOriginal));
            Assert.IsTrue(group.Files.Any(f => f.IsSelectedForRemoval));

            var duplicateItem = group.Files.First(f => f.IsSelectedForRemoval);
            var (deletedCount, freedBytes) = DuplicateFileScanner.DeleteDuplicates(new[] { duplicateItem });

            Assert.AreEqual(1, deletedCount);
            Assert.AreEqual(identicalBytes.Length, freedBytes);
            Assert.IsTrue(File.Exists(file1), "Original file must be preserved.");
            Assert.IsFalse(File.Exists(file2), "Duplicate file must be deleted.");
        }
    }
}
