using Microsoft.VisualStudio.TestTools.UnitTesting;
using BulkCrapUninstaller.Functions.Ratings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BulkCrapUninstaller.Functions.Ratings.Tests
{
    [TestClass()]
    public class UninstallerRatingManagerTests
    {
        [ClassInitialize()]
        public static void ClassInitialize(TestContext context)
        {
            Program.DbConnectionString = @"server=192.168.100.200;uid=application;pwd=sy9jSnUEae3XJXPV;database=bcu;";
        }

        UninstallerRatingManager manager;

        [TestInitialize()]
        public void TestInitialize()
        {
            manager = new UninstallerRatingManager(1);
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            manager?.Dispose();
        }

        [TestMethod()]
        public void RefreshStatsTest()
        {
            manager.RefreshStats();
            manager.Items.Any();
        }

        static readonly string[] TestEntryNames = { "Test_1", "Test_2", "Test_3", "Test_4" };

        [TestMethod()]
        public void GetRatingTest()
        {
            manager.RefreshStats();
            var rating = manager.GetRating(TestEntryNames[0]);

            if (rating.IsEmpty)
                Assert.Fail();
        }

        [TestMethod()]
        public void SetMyRatingTest()
        {
            manager.RefreshStats();

            try
            {
                manager.SetMyRating(null, UninstallerRating.Bad);
                Assert.Fail();
            }
            catch (ArgumentNullException) { }
            try
            {
                manager.SetMyRating(TestEntryNames[0], UninstallerRating.Unknown);
                Assert.Fail();
            }
            catch (ArgumentException) { }

            manager.SetMyRating(TestEntryNames[0], UninstallerRating.Good);
            Assert.AreEqual((int)UninstallerRating.Good, manager.GetRating(TestEntryNames[0]).MyRating);

            var rating = manager.GetRating("Test_SetMyRatingTest");
            var newRating = rating.MyRating == (int)UninstallerRating.Bad ? UninstallerRating.Good : UninstallerRating.Bad;
            manager.SetMyRating("Test_SetMyRatingTest", newRating);
            Assert.AreEqual((int)newRating, manager.GetRating("Test_SetMyRatingTest").MyRating);
        }

        [TestMethod()]
        public void SerializeDeserializeCasheTest()
        {
            manager.RefreshStats();
            var count = manager.Items.Count();
            if (count == 0)
                Assert.Fail("No items received");

            var filename = Path.Combine(Program.AssemblyLocation.FullName, "RatingCasheTest.xml");

            manager.SerializeCashe(filename);

            TestCleanup();
            TestInitialize();

            manager.DeserializeCashe(filename);
            Assert.AreEqual(count, manager.Items.Count());
        }
    }
}