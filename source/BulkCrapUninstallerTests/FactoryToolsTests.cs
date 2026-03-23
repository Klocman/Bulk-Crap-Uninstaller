using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Factory;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class FactoryToolsTests
    {
        [TestMethod]
        public void ExtractAppDataSetsFromHelperOutput_ParsesMultipleBlocks()
        {
            const string input = "Name: App1\r\nPath: C:\\App1\r\n\r\nName: App2\r\nPath: C:\\App2\r\n";

            var blocks = FactoryTools.ExtractAppDataSetsFromHelperOutput(input).ToList();

            Assert.AreEqual(2, blocks.Count);
            Assert.AreEqual("App1", blocks[0]["Name"]);
            Assert.AreEqual(@"C:\App1", blocks[0]["Path"]);
            Assert.AreEqual("App2", blocks[1]["Name"]);
            Assert.AreEqual(@"C:\App2", blocks[1]["Path"]);
        }

        [TestMethod]
        public void ExtractAppDataSetsFromHelperOutput_DuplicateKeys_LastValueWins()
        {
            const string input = "Name: First\r\nName: Second\r\n";

            var block = FactoryTools.ExtractAppDataSetsFromHelperOutput(input).Single();

            Assert.AreEqual("Second", block["Name"]);
        }

        [TestMethod]
        public void ExtractAppDataSetsFromHelperOutput_InvalidLines_AreIgnored()
        {
            const string input = "NoSeparator\r\n:MissingKey\r\nValid: Value\r\n";

            var block = FactoryTools.ExtractAppDataSetsFromHelperOutput(input).Single();

            Assert.AreEqual(1, block.Count);
            Assert.AreEqual("Value", block["Valid"]);
        }

        [TestMethod]
        public void ExtractAppDataSetsFromHelperOutput_ValueCanContainAdditionalColons()
        {
            const string input = "Url: https://example.com:8080/path\r\n";

            var block = FactoryTools.ExtractAppDataSetsFromHelperOutput(input).Single();

            Assert.AreEqual("https://example.com:8080/path", block["Url"]);
        }
    }
}
