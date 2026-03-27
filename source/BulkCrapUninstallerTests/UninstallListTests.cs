using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Lists;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class UninstallListTests
    {
        [TestMethod]
        public void ReadFromFile_EmptyFile_ThrowsInvalidDataException()
        {
            var path = Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, string.Empty);

                var ex = Assert.ThrowsException<InvalidDataException>(() => UninstallList.ReadFromFile(path));

                StringAssert.Contains(ex.Message, "empty");
            }
            finally
            {
                File.Delete(path);
            }
        }

        [TestMethod]
        public void ReadFromFile_LeadingBomAndWhitespace_LoadsList()
        {
            var path = Path.GetTempFileName();
            try
            {
                var input = new UninstallList(new[] { new Filter("Test", "Example App") });
                input.SaveToFile(path);

                var originalContents = File.ReadAllText(path);
                File.WriteAllText(path, "\uFEFF\r\n\t  " + originalContents, Encoding.UTF8);

                var result = UninstallList.ReadFromFile(path);

                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.Filters.Count);
                Assert.AreEqual("Test", result.Filters[0].Name);
                Assert.AreEqual("Example App", result.Filters[0].ComparisonEntries[0].FilterText);
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}
