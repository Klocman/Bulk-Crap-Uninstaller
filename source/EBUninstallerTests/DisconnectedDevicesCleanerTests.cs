/*
    EBUninstaller Pro - Disconnected Devices Cleaner Tests
    Unit tests for device model properties, categories, and safety filters.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class DisconnectedDevicesCleanerTests
    {
        [TestMethod]
        public void TestDisconnectedDeviceItemProperties()
        {
            var item = new DisconnectedDeviceItem
            {
                DeviceInstanceId = @"USBSTOR\Disk&Ven_SanDisk&Prod_Ultra\0123456789",
                FriendlyName = "SanDisk Ultra USB Device",
                DeviceDescription = "USB Mass Storage Device",
                HardwareId = @"USBSTOR\DiskSanDisk_Ultra___________1.00",
                ClassGuid = "{4d36e967-e325-11ce-bfc1-08002be10318}",
                Category = DeviceCategoryClass.UsbStorage,
                IsSystemCritical = false
            };

            Assert.AreEqual("SanDisk Ultra USB Device", item.FriendlyName);
            Assert.AreEqual(DeviceCategoryClass.UsbStorage, item.Category);
            Assert.IsFalse(item.IsSystemCritical);
            Assert.AreEqual(@"USBSTOR\Disk&Ven_SanDisk&Prod_Ultra\0123456789", item.DeviceInstanceId);
        }

        [TestMethod]
        public void TestDeviceCategoryValues()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(DeviceCategoryClass), DeviceCategoryClass.UsbStorage));
            Assert.IsTrue(Enum.IsDefined(typeof(DeviceCategoryClass), DeviceCategoryClass.Bluetooth));
            Assert.IsTrue(Enum.IsDefined(typeof(DeviceCategoryClass), DeviceCategoryClass.AudioEndpoint));
            Assert.IsTrue(Enum.IsDefined(typeof(DeviceCategoryClass), DeviceCategoryClass.Printer));
            Assert.IsTrue(Enum.IsDefined(typeof(DeviceCategoryClass), DeviceCategoryClass.HumanInterfaceDevice));
        }
    }
}
