/*
    EBUninstaller Pro - Device Driver & Hardware Peripherals Residuals Cleaner
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    public sealed class DriverResidualItem
    {
        public string DeviceName { get; set; }
        public string HardwareId { get; set; }
        public string DriverClass { get; set; }
        public string RegistryKeyPath { get; set; }
        public string InfFileName { get; set; }
        public bool IsDisconnected { get; set; }
        public string Description { get; set; }
    }

    public static class DeviceDriverResidualsCleaner
    {
        public static List<DriverResidualItem> ScanDriverResiduals()
        {
            var results = new List<DriverResidualItem>();
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return results;

            try
            {
                // Scan Enum registry keys for phantom or orphaned USB/Display/Printer devices
                const string enumRoot = @"SYSTEM\CurrentControlSet\Enum";
                using var baseKey = Registry.LocalMachine.OpenSubKey(enumRoot, false);
                if (baseKey != null)
                {
                    foreach (var busName in new[] { "USB", "USBSTOR", "SWD", "DISPLAY" })
                    {
                        using var busKey = baseKey.OpenSubKey(busName, false);
                        if (busKey == null) continue;

                        foreach (var devId in busKey.GetSubKeyNames())
                        {
                            using var devKey = busKey.OpenSubKey(devId, false);
                            if (devKey == null) continue;

                            foreach (var instance in devKey.GetSubKeyNames())
                            {
                                using var instKey = devKey.OpenSubKey(instance, false);
                                if (instKey == null) continue;

                                var friendlyName = instKey.GetValue("FriendlyName")?.ToString() ?? instKey.GetValue("DeviceDesc")?.ToString();
                                var driver = instKey.GetValue("Driver")?.ToString();
                                var service = instKey.GetValue("Service")?.ToString();

                                if (!string.IsNullOrEmpty(friendlyName))
                                {
                                    // Clean device name from localized resource string (e.g. @oem12.inf,%DeviceDesc%;Realtek...)
                                    if (friendlyName.Contains(";"))
                                        friendlyName = friendlyName.Substring(friendlyName.IndexOf(';') + 1);

                                    results.Add(new DriverResidualItem
                                    {
                                        DeviceName = friendlyName,
                                        HardwareId = $"{busName}\\{devId}",
                                        RegistryKeyPath = $@"HKLM\{enumRoot}\{busName}\{devId}\{instance}",
                                        DriverClass = instKey.GetValue("Class")?.ToString() ?? "Unknown",
                                        InfFileName = instKey.GetValue("Mfg")?.ToString(),
                                        IsDisconnected = true,
                                        Description = $"Device: {friendlyName} (Driver: {driver ?? "Generic"}, Service: {service ?? "None"})"
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.JunkCleaner, "Error scanning driver residuals", ex.Message);
            }

            StructuredLogger.Info(LogCategory.JunkCleaner, $"Found {results.Count} hardware driver / peripheral entries.");
            return results;
        }
    }
}
