using System;
using System.IO;
using System.Linq;
using BulkCrapUninstaller;
using BulkCrapUninstaller.Functions.Ratings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BulkCrapUninstallerTests.Functions
{
    [TestClass]
    public class UninstallerRatingManagerTests
    {
        private static readonly string[] TestEntryNames = {"Test_1", "Test_2", "Test_3", "Test_4"};
        private UninstallerRatingManager _manager;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Program.DbConnectionString = @"server=192.168.100.200;uid=application;pwd=sy9jSnUEae3XJXPV;database=bcu;";
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _manager = new UninstallerRatingManager(1);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _manager?.Dispose();
        }

        [TestMethod]
        public void RefreshStatsTest()
        {
            _manager.FetchRatings();
            if (!_manager.Items.Any())
                Assert.Fail();
        }

        [TestMethod]
        public void GetRatingTest()
        {
            _manager.FetchRatings();
            var rating = _manager.GetRating(TestEntryNames[0]);

            if (rating.IsEmpty)
                Assert.Fail();
        }

        [TestMethod]
        public void SetMyRatingTest()
        {
            _manager.FetchRatings();

            try
            {
                _manager.SetMyRating(null, UninstallerRating.Bad);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
            }
            try
            {
                _manager.SetMyRating(TestEntryNames[0], UninstallerRating.Unknown);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }

            _manager.SetMyRating(TestEntryNames[0], UninstallerRating.Good);
            Assert.AreEqual((int) UninstallerRating.Good, _manager.GetRating(TestEntryNames[0]).MyRating);

            var rating = _manager.GetRating("Test_SetMyRatingTest");
            var newRating = rating.MyRating == (int) UninstallerRating.Bad
                ? UninstallerRating.Good
                : UninstallerRating.Bad;
            _manager.SetMyRating("Test_SetMyRatingTest", newRating);
            Assert.AreEqual((int) newRating, _manager.GetRating("Test_SetMyRatingTest").MyRating);
        }

        [TestMethod]
        public void SerializeDeserializeCasheTest()
        {
            _manager.FetchRatings();
            var count = _manager.Items.Count();
            if (count == 0)
                Assert.Fail("No items received");

            var filename = Path.Combine(Program.AssemblyLocation.FullName, "RatingCasheTest.xml");

            _manager.SerializeCashe(filename);

            TestCleanup();
            TestInitialize();

            _manager.DeserializeCashe(filename);
            Assert.AreEqual(count, _manager.Items.Count());
        }
    }
}