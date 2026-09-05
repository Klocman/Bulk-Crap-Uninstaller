/*
    EBUninstaller Pro - Empty Directory & Duplicate Scanner Tests
    Unit and integration tests for empty directory and duplicate file cleanup.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UninstallTools.FileSystemEngine;

namespace BulkCrapUninstallerTests
{
    [TestFixture]
    public class EmptyDirectoryAndDuplicateTests
    {
        private string _tempTestDir = string.Empty;

        [SetUp]
        public void Setup()
        {
            _tempTestDir = Path.Combine(Path.GetTempPath(), "EBUninstaller_Test_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempTestDir);
        }

        [TearDown]
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

        [Test]
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
            Assert.That(results, Is.Not.Null);
            Assert.That(results.Any(r => r.Path == emptySubdir1), Is.True, "EmptyFolder1 should be identified as empty.");
            Assert.That(results.Any(r => r.Path == emptySubdir2), Is.True, "NestedEmpty should be identified as empty.");
            Assert.That(results.Any(r => r.Path == nonEmptySubdir), Is.False, "NonEmptyFolder should not be identified as empty.");

            int deleted = EmptyDirectoryCleaner.DeleteEmptyDirectories(results);
            Assert.That(deleted, Is.GreaterThanOrEqualTo(2));
            Assert.That(Directory.Exists(emptySubdir1), Is.False);
            Assert.That(Directory.Exists(nonEmptySubdir), Is.True);
        }

        [Test]
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
            Assert.That(duplicateGroups, Is.Not.Null);
            Assert.That(duplicateGroups.Count, Is.EqualTo(1), "Exactly one duplicate group should be found.");

            var group = duplicateGroups[0];
            Assert.That(group.Files.Count, Is.EqualTo(2));
            Assert.That(group.Files.Any(f => f.IsOriginal), Is.True);
            Assert.That(group.Files.Any(f => f.IsSelectedForRemoval), Is.True);

            var duplicateItem = group.Files.First(f => f.IsSelectedForRemoval);
            var (deletedCount, freedBytes) = DuplicateFileScanner.DeleteDuplicates(new[] { duplicateItem });

            Assert.That(deletedCount, Is.EqualTo(1));
            Assert.That(freedBytes, Is.EqualTo(identicalBytes.Length));
            Assert.That(File.Exists(file1), Is.True, "Original file must be preserved.");
            Assert.That(File.Exists(file2), Is.False, "Duplicate file must be deleted.");
        }
    }
}
