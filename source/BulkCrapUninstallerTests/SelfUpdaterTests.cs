using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Klocman.UpdateSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class SelfUpdaterTests
    {
        [TestMethod]
        public void VersionTest()
        {
            var ver = SelfUpdater.CheckLatestVersion();
            Assert.AreNotEqual(ver, default);
        }
    }
}
