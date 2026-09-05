/*
    EBUninstaller Pro - Windows Firewall Manager Tests
    Unit tests for firewall rule parsing, orphan detection, and data models.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class WindowsFirewallManagerTests
    {
        [TestMethod]
        public void TestParseRuleStringValid()
        {
            string raw = "v2.10|Action=Allow|Active=TRUE|Dir=In|Protocol=6|LPort=8080|App=C:\\NonExistentApp\\app.exe|Name=Test App Rule|Desc=Test description|";
            var rule = WindowsFirewallManager.ParseRuleString("Rule-001", raw);

            Assert.IsNotNull(rule);
            Assert.AreEqual("Rule-001", rule.RuleId);
            Assert.AreEqual("Test App Rule", rule.Name);
            Assert.AreEqual("Test description", rule.Description);
            Assert.AreEqual(@"C:\NonExistentApp\app.exe", rule.ApplicationPath);
            Assert.AreEqual(FirewallRuleDirection.Inbound, rule.Direction);
            Assert.AreEqual(FirewallRuleAction.Allow, rule.Action);
            Assert.AreEqual("6", rule.Protocol);
            Assert.AreEqual("8080", rule.Ports);
            Assert.IsTrue(rule.IsEnabled);
            Assert.IsTrue(rule.IsOrphaned); // File does not exist on disk
        }

        [TestMethod]
        public void TestParseRuleStringEmpty()
        {
            var rule = WindowsFirewallManager.ParseRuleString("EmptyRule", "");
            Assert.IsNull(rule);
        }
    }
}
