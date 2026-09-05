/*
    EBUninstaller Pro - Windows Services Optimizer Tests
    Unit tests for service classification, safety filters, and startup modes.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using NUnit.Framework;
using UninstallTools.Startup;

namespace BulkCrapUninstallerTests
{
    [TestFixture]
    public class WindowsServicesOptimizerTests
    {
        [Test]
        public void TestGetServicesStructure()
        {
            var services = WindowsServicesOptimizer.GetServices();
            Assert.That(services, Is.Not.Null);

            foreach (var svc in services)
            {
                Assert.That(svc.ServiceName, Is.Not.Null);
                Assert.That(svc.DisplayName, Is.Not.Null);
                Assert.That(svc.StartupMode, Is.Not.EqualTo(ServiceStartupMode.Unknown));
            }
        }

        [Test]
        public void TestCriticalSystemServiceProtection()
        {
            // Critical services like RpcSs, DcomLaunch, EventLog must be protected
            bool changeResult = WindowsServicesOptimizer.ChangeStartupMode("RpcSs", ServiceStartupMode.Disabled);
            Assert.That(changeResult, Is.False, "Critical system service RpcSs must not be modifiable.");

            bool deleteResult = WindowsServicesOptimizer.DeleteOrphanedService("EventLog");
            Assert.That(deleteResult, Is.False, "Critical system service EventLog must not be deletable.");
        }

        [Test]
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

            Assert.That(item.ServiceName, Is.EqualTo("TestService"));
            Assert.That(item.IsOrphaned, Is.True);
            Assert.That(item.IsMicrosoftService, Is.False);
            Assert.That(item.IsCriticalSystem, Is.False);
        }
    }
}
