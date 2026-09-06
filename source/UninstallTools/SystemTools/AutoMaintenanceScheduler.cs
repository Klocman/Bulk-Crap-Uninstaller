/*
    EBUninstaller Pro - Automated Maintenance Task Scheduler
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public enum MaintenanceFrequency
    {
        Weekly,
        Monthly,
        AtLogon
    }

    public static class AutoMaintenanceScheduler
    {
        private const string TaskName = "EBUninstaller_Pro_AutoCleanup";

        public static bool IsMaintenanceScheduled()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return false;

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = $"/Query /TN \"{TaskName}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi);
                proc?.WaitForExit(3000);
                return proc != null && proc.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        public static bool ScheduleMaintenance(MaintenanceFrequency frequency, bool cleanJunk = true, bool cleanPrivacy = false)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return false;

            try
            {
                var consoleExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EBU-console.exe");
                if (!File.Exists(consoleExe))
                {
                    consoleExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EBUninstaller.exe");
                }

                var scheduleArg = frequency switch
                {
                    MaintenanceFrequency.Weekly => "/SC WEEKLY /D SUN /ST 03:00",
                    MaintenanceFrequency.Monthly => "/SC MONTHLY /D 1 /ST 03:00",
                    MaintenanceFrequency.AtLogon => "/SC ONLOGON",
                    _ => "/SC WEEKLY /D SUN /ST 03:00"
                };

                var cleanArg = cleanJunk && cleanPrivacy ? "clean-junk --clean && " + consoleExe + " clean-privacy --clean" : (cleanJunk ? "clean-junk --clean" : "clean-privacy --clean");

                var psi = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = $"/Create /TN \"{TaskName}\" /TR \"\\\"{consoleExe}\\\" {cleanArg} /U\" {scheduleArg} /RL HIGHEST /F",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi);
                proc?.WaitForExit(5000);

                var success = proc != null && proc.ExitCode == 0;
                if (success)
                    StructuredLogger.Info(LogCategory.SystemTools, $"Scheduled maintenance task created successfully ({frequency}).");
                else
                    StructuredLogger.Warning(LogCategory.SystemTools, "Failed to create scheduled maintenance task via schtasks.");

                return success;
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.SystemTools, "Error scheduling maintenance task", ex.Message);
                return false;
            }
        }

        public static bool DeleteScheduledMaintenance()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return false;

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = $"/Delete /TN \"{TaskName}\" /F",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi);
                proc?.WaitForExit(3000);

                var success = proc != null && proc.ExitCode == 0;
                StructuredLogger.Info(LogCategory.SystemTools, "Scheduled maintenance task deleted.");
                return success;
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.SystemTools, "Error deleting maintenance task", ex.Message);
                return false;
            }
        }
    }
}
