/*
    EBUninstaller Pro - Disconnected & Ghost Devices Residuals Cleaner
    Auditing and safe cleanup of non-present/disconnected USB, Bluetooth, audio, and printer device registry nodes.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    public enum DeviceCategoryClass
    {
        UsbStorage,
        Bluetooth,
        AudioEndpoint,
        Printer,
        NetworkAdapter,
        HumanInterfaceDevice,
        Other
    }

    public class DisconnectedDeviceItem
    {
        public string DeviceInstanceId { get; set; } = string.Empty;
        public string FriendlyName { get; set; } = string.Empty;
        public string DeviceDescription { get; set; } = string.Empty;
        public string HardwareId { get; set; } = string.Empty;
        public string ClassGuid { get; set; } = string.Empty;
        public DeviceCategoryClass Category { get; set; }
        public string RegistryPath { get; set; } = string.Empty;
        public bool IsSystemCritical { get; set; }
    }

    public static class DisconnectedDevicesCleaner
    {
        private const string EnumKeyPath = @"SYSTEM\CurrentControlSet\Enum";

        public static List<DisconnectedDeviceItem> ScanDisconnectedDevices()
        {
            var results = new List<DisconnectedDeviceItem>();

            var scanRoots = new (string subKey, DeviceCategoryClass cat)[]
            {
                ("USBSTOR", DeviceCategoryClass.UsbStorage),
                ("BTHENUM", DeviceCategoryClass.Bluetooth),
                ("BTH", DeviceCategoryClass.Bluetooth),
                ("SWD\\MMDEVAPI", DeviceCategoryClass.AudioEndpoint),
                ("SWD\\PRINTENUM", DeviceCategoryClass.Printer),
                ("HID", DeviceCategoryClass.HumanInterfaceDevice)
            };

            foreach (var (rootSubKey, cat) in scanRoots)
            {
                try
                {
                    using var parentKey = Registry.LocalMachine.OpenSubKey($@"{EnumKeyPath}\{rootSubKey}", false);
                    if (parentKey == null) continue;

                    foreach (var deviceGroup in parentKey.GetSubKeyNames())
                    {
                        try
                        {
                            using var groupKey = parentKey.OpenSubKey(deviceGroup, false);
                            if (groupKey == null) continue;

                            foreach (var instanceName in groupKey.GetSubKeyNames())
                            {
                                try
                                {
                                    using var instanceKey = groupKey.OpenSubKey(instanceName, false);
                                    if (instanceKey == null) continue;

                                    string friendlyName = instanceKey.GetValue("FriendlyName") as string ?? string.Empty;
                                    string devDesc = instanceKey.GetValue("DeviceDesc") as string ?? string.Empty;
                                    string hardwareId = instanceKey.GetValue("HardwareID") as string[]?.FirstOrDefault() ?? (instanceKey.GetValue("HardwareID") as string ?? string.Empty);
                                    string classGuid = instanceKey.GetValue("ClassGUID") as string ?? string.Empty;

                                    // Resolve display string
                                    string displayName;
                                    if (!string.IsNullOrEmpty(friendlyName))
                                    {
                                        displayName = friendlyName;
                                    }
                                    else if (!string.IsNullOrEmpty(devDesc))
                                    {
                                        displayName = devDesc;
                                    }
                                    else
                                    {
                                        displayName = deviceGroup + @"\" + instanceName;
                                    }

                                    if (displayName.Contains(";"))
                                        displayName = displayName.Substring(displayName.LastIndexOf(';') + 1);

                                    string fullInstanceId = rootSubKey + @"\" + deviceGroup + @"\" + instanceName;
                                    bool isSys = IsSystemDevice(displayName, fullInstanceId);

                                    results.Add(new DisconnectedDeviceItem
                                    {
                                        DeviceInstanceId = fullInstanceId,
                                        FriendlyName = displayName,
                                        DeviceDescription = devDesc,
                                        HardwareId = hardwareId,
                                        ClassGuid = classGuid,
                                        Category = cat,
                                        RegistryPath = $@"{parentKey.Name}\{deviceGroup}\{instanceName}",
                                        IsSystemCritical = isSys
                                    });
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Log(LogLevel.Warning, "DisconnectedDevicesCleaner", $"Failed to scan {rootSubKey}: {ex.Message}");
                }
            }

            return results.OrderBy(d => d.FriendlyName).ToList();
        }

        private static bool IsSystemDevice(string name, string instanceId)
        {
            if (string.IsNullOrEmpty(name)) return false;
            string lower = name.ToLowerInvariant();
            string lowerId = instanceId.ToLowerInvariant();

            if (lower.Contains("microsoft") || lower.Contains("standard") || lower.Contains("system") ||
                lower.Contains("root") || lowerId.Contains("root") || lower.Contains("generic volume"))
                return true;

            return false;
        }

        public static bool RemoveDeviceNode(DisconnectedDeviceItem item)
        {
            if (item == null || item.IsSystemCritical || string.IsNullOrWhiteSpace(item.DeviceInstanceId))
                return false;

            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(EnumKeyPath, true);
                if (key == null) return false;

                key.DeleteSubKeyTree(item.DeviceInstanceId, false);
                StructuredLogger.Log(LogLevel.Info, "DisconnectedDevicesCleaner", $"Deleted disconnected device node: {item.DeviceInstanceId}");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Error, "DisconnectedDevicesCleaner", $"Failed to delete device node '{item.DeviceInstanceId}': {ex.Message}");
                return false;
            }
        }
    }
}
