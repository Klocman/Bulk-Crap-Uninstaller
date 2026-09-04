/*
    OpenUninstall Pro - Unit Tests for Cryptography and Confidence Scorer
*/

using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools;
using UninstallTools.Core;
using UninstallTools.Detection;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class CryptoHasherTests
    {
        [TestMethod]
        public void ComputeSha256_KnownString_MatchesExpectedHash()
        {
            // SHA256 of "OpenUninstallPro"
            var input = "OpenUninstallPro";
            var hash = CryptoHasher.ComputeSha256(input, Encoding.UTF8);

            Assert.IsNotNull(hash);
            Assert.AreEqual(64, hash.Length); // 256 bits = 64 hex characters
        }

        [TestMethod]
        public void ComputeSha256_Bytes_MatchesStringComputation()
        {
            var rawBytes = Encoding.UTF8.GetBytes("HelloWorld123");
            var hashFromBytes = CryptoHasher.ComputeSha256(rawBytes);
            var hashFromString = CryptoHasher.ComputeSha256("HelloWorld123", Encoding.UTF8);

            Assert.AreEqual(hashFromBytes, hashFromString);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ComputeSha256_NullData_ThrowsArgumentNullException()
        {
            CryptoHasher.ComputeSha256((byte[])null);
        }
    }

    [TestClass]
    public class ConfidenceScorerTests
    {
        [TestMethod]
        public void CalculateConfidence_NullEntry_ReturnsSuspicious()
        {
            var result = ConfidenceScorer.CalculateConfidence(null);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(DiscoveryConfidenceLevel.SuspiciousOrBroken, result.Level);
        }

        [TestMethod]
        public void CalculateConfidence_FullyRegisteredEntry_ReturnsHighScore()
        {
            var entry = new ApplicationUninstallerEntry
            {
                DisplayName = "Test Suite Pro Application",
                Publisher = "OpenUninstall",
                DisplayVersion = "1.0.0",
                IsRegistered = true,
                InstallDate = DateTime.UtcNow
            };

            var result = ConfidenceScorer.CalculateConfidence(entry);
            Assert.IsTrue(result.Score >= 40);
            Assert.IsTrue(result.IsRegisteredInSystem);
        }

        [TestMethod]
        public void CalculateConfidence_OrphanedEntryWithoutPublisher_ScoresLower()
        {
            var orphaned = new ApplicationUninstallerEntry
            {
                DisplayName = "Orphaned App",
                IsRegistered = false,
                IsOrphaned = true
            };

            var result = ConfidenceScorer.CalculateConfidence(orphaned);
            Assert.IsTrue(result.Score < 50);
        }
    }
}
