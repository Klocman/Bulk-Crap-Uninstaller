using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class ApplicationUninstallerEntryTests
    {
        [DataTestMethod]
        [DataRow("Tweak-TestEntry", true)]
        [DataRow("tweak-TestEntry", true)]
        [DataRow("Steam App 42", false)]
        [DataRow(null, false)]
        public void IsScriptTweak_MatchesCurrentTweakIdFormat(string ratingId, bool expected)
        {
            var entry = new ApplicationUninstallerEntry
            {
                RatingId = ratingId
            };

            Assert.AreEqual(expected, entry.IsScriptTweak);
        }
    }
}
