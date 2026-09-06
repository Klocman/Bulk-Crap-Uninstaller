/*
    EBUninstaller Pro - Windows Services Health & Optimization Engine
    Detection, auditing, startup optimization, and orphaned service cleanup.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.Startup
{
    public enum ServiceStartupMode
    {
        Automatic = 2,
        Manual = 3,
        Disabled = 4,
        AutomaticDelayed = 5,
        Unknown = 0
    }

    public class WindowsServiceItem
    {
        public string ServiceName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public ServiceStartupMode StartupMode { get; set; }
        public ServiceControllerStatus Status { get; set; }
        public bool IsMicrosoftService { get; set; }
        public bool IsOrphaned { get; set; }
        public bool IsCriticalSystem { get; set; }
    }

    public static class WindowsServicesOptimizer
    {
        private static readonly HashSet<string> CriticalSystemServices = new(StringComparer.OrdinalIgnoreCase)
        {
            "RpcSs", "DcomLaunch", "RpcEptMapper", "EventLog", "PlugPlay", "CryptSvc",
            "Winmgmt", "Themes", "AudioSrv", "ProfSvc", "gpsvc", "MpsSvc", "WdNisSvc",
            "WinDefend", "Schedule", "BrokerInfrastructure", "LSM", "CoreMessagingRegistrar"
        };

        public static List<WindowsServiceItem> GetServices()
        {
            var results = new List<WindowsServiceItem>();
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return results;

            try
            {
                using var servicesKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services");
                if (servicesKey == null) return results;

                var scArray = ServiceController.GetServices();
                var scDict = scArray.ToDictionary(s => s.ServiceName, StringComparer.OrdinalIgnoreCase);

                foreach (var serviceName in servicesKey.GetSubKeyNames())
                {
                    try
                    {
                        using var svcKey = servicesKey.OpenSubKey(serviceName);
                        if (svcKey == null) continue;

                        int type = (int)(svcKey.GetValue("Type", 0) ?? 0);
                        // Filter out kernel drivers (Type 1 or 2) unless they are Win32 service (Type 16, 32, 256, etc.)
                        if ((type & 0x10) == 0 && (type & 0x20) == 0 && (type & 0x100) == 0 && type != 0)
                            continue;

                        string displayName = (svcKey.GetValue("DisplayName") as string) ?? serviceName;
                        string description = (svcKey.GetValue("Description") as string) ?? string.Empty;
                        string rawImagePath = (svcKey.GetValue("ImagePath") as string) ?? string.Empty;
                        int startVal = (int)(svcKey.GetValue("Start", 3) ?? 3);
                        int delayedAuto = (int)(svcKey.GetValue("DelayedAutoStart", 0) ?? 0);

                        var (cleanExePath, publisher) = ResolveServiceExe(rawImagePath);
                        bool isOrphaned = !string.IsNullOrEmpty(cleanExePath) && !File.Exists(cleanExePath);
                        bool isCritical = CriticalSystemServices.Contains(serviceName);
                        bool isMicrosoft = IsMicrosoftPublisher(publisher, cleanExePath);

                        ServiceControllerStatus status = ServiceControllerStatus.Stopped;
                        if (scDict.TryGetValue(serviceName, out var sc))
                        {
                            try { status = sc.Status; } catch { }
                        }

                        ServiceStartupMode mode = startVal switch
                        {
                            2 => (delayedAuto == 1 ? ServiceStartupMode.AutomaticDelayed : ServiceStartupMode.Automatic),
                            3 => ServiceStartupMode.Manual,
                            4 => ServiceStartupMode.Disabled,
                            _ => ServiceStartupMode.Unknown
                        };

                        results.Add(new WindowsServiceItem
                        {
                            ServiceName = serviceName,
                            DisplayName = displayName,
                            Description = description,
                            ImagePath = cleanExePath ?? rawImagePath,
                            Publisher = publisher ?? (isMicrosoft ? "Microsoft Corporation" : "(Unknown)"),
                            StartupMode = mode,
                            Status = status,
                            IsMicrosoftService = isMicrosoft,
                            IsOrphaned = isOrphaned,
                            IsCriticalSystem = isCritical
                        });
                    }
                    catch (Exception ex)
                    {
                        StructuredLogger.Log(LogLevel.Warning, "WindowsServicesOptimizer", $"Error inspecting service {serviceName}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Error, "WindowsServicesOptimizer", $"Failed to enumerate services: {ex.Message}");
            }

            return results;
        }

        private static (string? ExePath, string? Publisher) ResolveServiceExe(string rawPath)
        {
            if (string.IsNullOrWhiteSpace(rawPath))
                return (null, null);

            string expanded = Environment.ExpandEnvironmentVariables(rawPath.Trim());
            string exePath = expanded;

            if (expanded.StartsWith("\""))
            {
                int endQuote = expanded.IndexOf('"', 1);
                if (endQuote > 1)
                    exePath = expanded.Substring(1, endQuote - 1);
            }
            else
            {
                int spaceIndex = expanded.IndexOf(".exe", StringComparison.OrdinalIgnoreCase);
                if (spaceIndex > 0)
                    exePath = expanded.Substring(0, spaceIndex + 4);
            }

            string? publisher = null;
            if (File.Exists(exePath))
            {
                try
                {
                    var vi = System.Diagnostics.FileVersionInfo.GetVersionInfo(exePath);
                    publisher = vi.CompanyName;
                }
                catch { }
            }

            return (exePath, publisher);
        }

        private static bool IsMicrosoftPublisher(string? publisher, string? exePath)
        {
            if (!string.IsNullOrEmpty(publisher) && publisher.IndexOf("Microsoft", StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            if (!string.IsNullOrEmpty(exePath))
            {
                string windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                if (exePath.StartsWith(windir, StringComparison.OrdinalIgnoreCase) &&
                    (exePath.IndexOf("system32\\svchost.exe", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     exePath.IndexOf("system32\\services.exe", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     exePath.IndexOf("system32\\lsass.exe", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ChangeStartupMode(string serviceName, ServiceStartupMode newMode)
        {
            if (CriticalSystemServices.Contains(serviceName))
                return false;

            try
            {
                using var svcKey = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\{serviceName}", true);
                if (svcKey == null) return false;

                switch (newMode)
                {
                    case ServiceStartupMode.Automatic:
                        svcKey.SetValue("Start", 2, RegistryValueKind.DWord);
                        svcKey.SetValue("DelayedAutoStart", 0, RegistryValueKind.DWord);
                        break;
                    case ServiceStartupMode.AutomaticDelayed:
                        svcKey.SetValue("Start", 2, RegistryValueKind.DWord);
                        svcKey.SetValue("DelayedAutoStart", 1, RegistryValueKind.DWord);
                        break;
                    case ServiceStartupMode.Manual:
                        svcKey.SetValue("Start", 3, RegistryValueKind.DWord);
                        break;
                    case ServiceStartupMode.Disabled:
                        svcKey.SetValue("Start", 4, RegistryValueKind.DWord);
                        break;
                }

                StructuredLogger.Log(LogLevel.Info, "WindowsServicesOptimizer", $"Changed startup mode for {serviceName} to {newMode}");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Error, "WindowsServicesOptimizer", $"Failed to change startup mode for {serviceName}: {ex.Message}");
                return false;
            }
        }

        public static bool DeleteOrphanedService(string serviceName)
        {
            if (CriticalSystemServices.Contains(serviceName))
                return false;

            try
            {
                using var servicesKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services", true);
                if (servicesKey != null)
                {
                    servicesKey.DeleteSubKeyTree(serviceName, false);
                    StructuredLogger.Log(LogLevel.Info, "WindowsServicesOptimizer", $"Deleted orphaned service {serviceName}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Error, "WindowsServicesOptimizer", $"Failed to delete orphaned service {serviceName}: {ex.Message}");
            }

            return false;
        }
    }
}
