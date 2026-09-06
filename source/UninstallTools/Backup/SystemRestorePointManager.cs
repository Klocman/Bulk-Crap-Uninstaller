/*
    EBUninstaller Pro - Windows System Restore Point & VSS Manager
    Creation, enumeration, and management of Windows System Restore points.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.InteropServices;
using UninstallTools.Core;

namespace UninstallTools.Backup
{
    public enum RestorePointType
    {
        ApplicationInstall = 0,
        ApplicationUninstall = 1,
        DeviceDriverInstall = 10,
        ModifySettings = 12,
        CancelledOperation = 13,
        ManualCheckpoint = 16
    }

    public class SystemRestorePointItem
    {
        public uint SequenceNumber { get; set; }
        public string Description { get; set; } = string.Empty;
        public RestorePointType Type { get; set; }
        public DateTime CreationTime { get; set; }
        public uint EventType { get; set; }
    }

    public static class SystemRestorePointManager
    {
        public static List<SystemRestorePointItem> GetRestorePoints()
        {
            var results = new List<SystemRestorePointItem>();
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return results;

            try
            {
                using var searcher = new ManagementObjectSearcher(@"root\default", "SELECT * FROM SystemRestore");
                foreach (ManagementObject obj in searcher.Get())
                {
                    try
                    {
                        uint seq = Convert.ToUInt32(obj["SequenceNumber"] ?? 0);
                        string desc = (obj["Description"] as string) ?? "(No Description)";
                        uint rpType = Convert.ToUInt32(obj["RestorePointType"] ?? 0);
                        uint eventType = Convert.ToUInt32(obj["EventType"] ?? 0);
                        string creationTimeStr = (obj["CreationTime"] as string) ?? string.Empty;

                        DateTime dt = DateTime.UtcNow;
                        if (!string.IsNullOrEmpty(creationTimeStr))
                        {
                            try { dt = ManagementDateTimeConverter.ToDateTime(creationTimeStr); } catch { }
                        }

                        results.Add(new SystemRestorePointItem
                        {
                            SequenceNumber = seq,
                            Description = desc,
                            Type = (RestorePointType)rpType,
                            EventType = eventType,
                            CreationTime = dt
                        });
                    }
                    catch (Exception ex)
                    {
                        StructuredLogger.Log(LogLevel.Warning, "SystemRestorePointManager", $"Error reading restore point: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Warning, "SystemRestorePointManager", $"WMI query for SystemRestore failed: {ex.Message}");
            }

            return results;
        }

        public static bool CreateRestorePoint(string description, RestorePointType type = RestorePointType.ManualCheckpoint)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return false;

            try
            {
                using var mgmtClass = new ManagementClass(@"root\default:SystemRestore");
                using var inParams = mgmtClass.GetMethodParameters("CreateRestorePoint");

                inParams["Description"] = description;
                inParams["RestorePointType"] = (uint)type;
                inParams["EventType"] = 100; // BEGIN_SYSTEM_CHANGE

                using var outParams = mgmtClass.InvokeMethod("CreateRestorePoint", inParams, null);
                uint returnValue = Convert.ToUInt32(outParams["ReturnValue"] ?? 1);

                bool success = returnValue == 0;
                if (success)
                {
                    StructuredLogger.Log(LogLevel.Info, "SystemRestorePointManager", $"Successfully created system restore point: '{description}'");
                }
                else
                {
                    StructuredLogger.Log(LogLevel.Warning, "SystemRestorePointManager", $"CreateRestorePoint returned code: {returnValue}");
                }
                return success;
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Error, "SystemRestorePointManager", $"Failed to create restore point: {ex.Message}");
                return false;
            }
        }
    }
}
