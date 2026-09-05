/*
    EBUninstaller Pro - Windows Services Optimizer Tests
    Unit tests for service classification, safety filters, and startup modes.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Startup;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class WindowsServicesOptimizerTests
    {
        [TestMethod]
        public void TestGetServicesStructure()
        {
            var services = WindowsServicesOptimizer.GetServices();
            Assert.IsNotNull(services);

            foreach (var svc in services)
            {
                Assert.IsNotNull(svc.ServiceName);
                Assert.IsNotNull(svc.DisplayName);
                Assert.AreNotEqual(ServiceStartupMode.Unknown, svc.StartupMode);
            }
        }

        [TestMethod]
        public void TestCriticalSystemServiceProtection()
        {
            // Critical services like RpcSs, DcomLaunch, EventLog must be protected
            bool changeResult = WindowsServicesOptimizer.ChangeStartupMode("RpcSs", ServiceStartupMode.Disabled);
            Assert.IsFalse(changeResult, "Critical system service RpcSs must not be modifiable.");

            bool deleteResult = WindowsServicesOptimizer.DeleteOrphanedService("EventLog");
            Assert.IsFalse(deleteResult, "Critical system service EventLog must not be deletable.");
        }

        [TestMethod]
        public void TestServiceItemProperties()
        {
            var item = new WindowsServiceItem
            {
                ServiceName = "TestService",
                DisplayName = "Test Service Display",
                Description = "A mock service for unit testing.",
                ImagePath = @"C:\Program Files\TestApp\service.exe",
                Publisher = "Test Publisher",
                StartupMode = ServiceStartupMode.Automatic,
                IsMicrosoftService = false,
                IsOrphaned = true,
                IsCriticalSystem = false
            };

            Assert.AreEqual("TestService", item.ServiceName);
            Assert.IsTrue(item.IsOrphaned);
            Assert.IsFalse(item.IsMicrosoftService);
            Assert.IsFalse(item.IsCriticalSystem);
        }
    }
}
