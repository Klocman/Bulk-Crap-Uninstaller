using System.IO;
using Klocman.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class ApplicationEntrySerializerTests
    {
        [TestMethod]
        public void SerializeApplicationEntries_InvalidXmlCharactersAreRemoved_ValidUnicodeIsPreserved()
        {
            var path = Path.GetTempFileName();
            try
            {
                var entry = new ApplicationUninstallerEntry
                {
                    DisplayName = "Test app",
                    Publisher = "Pu\u0002blisher",
                    Comment = "Before\u0001after \U0001F600",
                    CacheIdOverride = "Id\u0003value"
                };

                ApplicationEntrySerializer.SerializeApplicationEntries(path, new[] { entry });

                var result = SerializationTools.DeserializeFromXml<ApplicationEntrySerializer>(path);

                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.Items.Count);
                Assert.AreEqual("Publisher", result.Items[0].Publisher);
                Assert.AreEqual("Beforeafter \U0001F600", result.Items[0].Comment);
                Assert.AreEqual("Idvalue", result.Items[0].CacheIdOverride);
                Assert.AreEqual("Pu\u0002blisher", entry.Publisher);
                Assert.AreEqual("Before\u0001after \U0001F600", entry.Comment);
                Assert.AreEqual("Id\u0003value", entry.CacheIdOverride);
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}
