/*
    EBUninstaller Pro - Unit Tests for Exclusions, History, and Forced Removal
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Exclusions;
using UninstallTools.ForcedRemoval;
using UninstallTools.History;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class ExclusionManagerTests
    {
        [TestMethod]
        public void IsExcluded_ApplicationNameRule_CorrectlyMatches()
        {
            var rule = new ExclusionRule
            {
                RuleType = ExclusionRuleType.ApplicationName,
                Value = "Important*",
                IsEnabled = true
            };

            ExclusionManager.AddRule(rule);

            Assert.IsTrue(ExclusionManager.IsExcluded(applicationName: "Important Tool Suite"));
            Assert.IsFalse(ExclusionManager.IsExcluded(applicationName: "Random Adware"));
        }

        [TestMethod]
        public void IsExcluded_FilePathRule_CorrectlyMatches()
        {
            var rule = new ExclusionRule
            {
                RuleType = ExclusionRuleType.FilePath,
                Value = @"C:\CustomData\safe.txt",
                IsEnabled = true
            };

            ExclusionManager.AddRule(rule);

            Assert.IsTrue(ExclusionManager.IsExcluded(filePath: @"C:\CustomData\safe.txt"));
            Assert.IsFalse(ExclusionManager.IsExcluded(filePath: @"C:\CustomData\other.txt"));
        }
    }

    [TestClass]
    public class OperationHistoryTests
    {
        [TestMethod]
        public void RecordOperation_AndQuery_ReturnsEntry()
        {
            var entry = new OperationHistoryEntry
            {
                ApplicationName = "Unit Test Application 2026",
                OperationType = "UnitTestUninstall",
                Status = HistoryOperationStatus.Success,
                DetectedItemsCount = 10,
                DeletedItemsCount = 10,
                BackupId = "BK-12345"
            };

            OperationHistoryManager.RecordOperation(entry);

            var list = OperationHistoryManager.GetHistory("Unit Test Application 2026");
            Assert.IsTrue(list.Count > 0);
            Assert.AreEqual("BK-12345", list[0].BackupId);
        }

        [TestMethod]
        public void ExportHistory_GeneratesValidCsv()
        {
            var csv = OperationHistoryManager.ExportHistoryToCsv();
            Assert.IsNotNull(csv);
            Assert.IsTrue(csv.Contains("HistoryId"));
            Assert.IsTrue(csv.Contains("ApplicationName"));
        }
    }

    [TestClass]
    public class ForcedUninstallManagerTests
    {
        [TestMethod]
        public void BuildPlan_WithMockFolder_CreatesHighConfidenceDirectoryItem()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "EBUninstaller_ForcedTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(Path.Combine(tempDir, "mock_broken_app.exe"), "DATA");

            try
            {
                var plan = ForcedUninstallManager.BuildPlan(tempDir);
                Assert.IsNotNull(plan);
                Assert.IsTrue(plan.Items.Count > 0);

                var dirItem = plan.Items.FirstOrDefault(i => i.ItemType == ForcedRemovalItemType.Directory);
                Assert.IsNotNull(dirItem);
                Assert.AreEqual(ForcedRemovalConfidence.High, dirItem.Confidence);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }
    }
}
