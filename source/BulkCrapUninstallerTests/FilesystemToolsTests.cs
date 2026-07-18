using System;
using System.IO;
using Klocman.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class FilesystemToolsTests
    {
        [TestMethod]
        public void GetDirectorySize_IncludesHiddenFiles()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                var normalFilePath = Path.Combine(tempDir, "normal.txt");
                var hiddenFilePath = Path.Combine(tempDir, "hidden.txt");

                File.WriteAllBytes(normalFilePath, new byte[10]);
                File.WriteAllBytes(hiddenFilePath, new byte[20]);

                File.SetAttributes(hiddenFilePath, FileAttributes.Hidden | FileAttributes.System);

                // Act
                var totalSize = FilesystemTools.GetDirectorySize(tempDir);

                // Assert
                Assert.AreEqual(30, totalSize, "Directory size should include both normal and hidden/system files.");
            }
            finally
            {
                // Cleanup
                Directory.Delete(tempDir, true);
            }
        }
    }
}
