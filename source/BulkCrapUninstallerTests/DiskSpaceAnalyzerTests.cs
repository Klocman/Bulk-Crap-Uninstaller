/*
    EBUninstaller Pro - Disk Space Analyzer Tests
    Unit tests for extension categorization, file ranking, and storage reports.
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
    public class DiskSpaceAnalyzerTests
    {
        [TestMethod]
        public void TestExtensionCategorization()
        {
            Assert.AreEqual(FileTypeCategory.ApplicationsAndExecutables, DiskSpaceAnalyzer.ClassifyExtension(".exe"));
            Assert.AreEqual(FileTypeCategory.ApplicationsAndExecutables, DiskSpaceAnalyzer.ClassifyExtension(".msi"));
            Assert.AreEqual(FileTypeCategory.DiskImagesAndIsos, DiskSpaceAnalyzer.ClassifyExtension(".iso"));
            Assert.AreEqual(FileTypeCategory.DiskImagesAndIsos, DiskSpaceAnalyzer.ClassifyExtension(".vmdk"));
            Assert.AreEqual(FileTypeCategory.ArchivesAndZips, DiskSpaceAnalyzer.ClassifyExtension(".zip"));
            Assert.AreEqual(FileTypeCategory.ArchivesAndZips, DiskSpaceAnalyzer.ClassifyExtension(".7z"));
            Assert.AreEqual(FileTypeCategory.MediaAndVideos, DiskSpaceAnalyzer.ClassifyExtension(".mp4"));
            Assert.AreEqual(FileTypeCategory.Documents, DiskSpaceAnalyzer.ClassifyExtension(".pdf"));
            Assert.AreEqual(FileTypeCategory.LogsAndDumps, DiskSpaceAnalyzer.ClassifyExtension(".log"));
            Assert.AreEqual(FileTypeCategory.Other, DiskSpaceAnalyzer.ClassifyExtension(".unknownxyz"));
        }

        [TestMethod]
        public void TestLargeFileItemProperties()
        {
            var item = new LargeFileItem
            {
                FileName = "large_installer.iso",
                FilePath = @"C:\Downloads\large_installer.iso",
                Extension = ".iso",
                SizeBytes = 4294967296, // 4 GB
                Category = FileTypeCategory.DiskImagesAndIsos,
                IsProtected = false
            };

            Assert.AreEqual("large_installer.iso", item.FileName);
            Assert.AreEqual(".iso", item.Extension);
            Assert.AreEqual(4294967296, item.SizeBytes);
            Assert.AreEqual(FileTypeCategory.DiskImagesAndIsos, item.Category);
            Assert.IsFalse(item.IsProtected);
        }
    }
}
