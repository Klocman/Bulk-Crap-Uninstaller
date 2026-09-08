using System.Drawing;
using System.IO;
using Klocman.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class DrawingToolsTests
    {
        [TestMethod]
        public void CreateOwnedIconFromHandle_ReturnsUsableClone()
        {
            using var sourceIcon = SystemIcons.Application;
            var handle = sourceIcon.Handle;

            using var ownedIcon = DrawingTools.CreateOwnedIconFromHandle(handle);
            using var stream = new MemoryStream();

            ownedIcon.Save(stream);

            Assert.IsGreaterThan(0, stream.Length);
            Assert.AreEqual(sourceIcon.Size, ownedIcon.Size);
        }
    }
}
