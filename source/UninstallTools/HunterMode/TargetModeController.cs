/*
    OpenUninstall Pro - Open Source Professional Windows Uninstaller
    Hunter / Target Mode Controller Subsystem
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Klocman.Subsystems;
using Klocman.Tools;
using UninstallTools.Core;

namespace UninstallTools.HunterMode
{
    public sealed class TargetInspectionResult
    {
        public IntPtr WindowHandle { get; set; }
        public int ProcessId { get; set; }
        public string ProcessName { get; set; }
        public string ExecutablePath { get; set; }
        public string WindowTitle { get; set; }
        public ApplicationUninstallerEntry MatchedApplication { get; set; }
        public bool IsIdentified => MatchedApplication != null || !string.IsNullOrEmpty(ExecutablePath);
        public string StatusMessage { get; set; }
    }

    public static class TargetModeController
    {
        /// <summary>
        /// Inspects a target window from WindowHoverSearcher and correlates it with installed applications.
        /// </summary>
        public static TargetInspectionResult InspectWindow(WindowHoverSearcher.WindowInfo windowInfo, IEnumerable<ApplicationUninstallerEntry> installedApps)
        {
            var result = new TargetInspectionResult();
            if (windowInfo == null)
            {
                result.StatusMessage = "No window targeted";
                return result;
            }

            result.WindowHandle = windowInfo.Handle;
            result.ProcessId = windowInfo.ProcessId;
            result.WindowTitle = windowInfo.WindowText;

            try
            {
                var proc = windowInfo.GetRunningProcess();
                if (proc != null)
                {
                    result.ProcessName = proc.ProcessName;
                    try
                    {
                        result.ExecutablePath = proc.MainModule?.FileName;
                    }
                    catch (Exception ex)
                    {
                        StructuredLogger.Warning(LogCategory.General, "Could not get MainModule for process", ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Error inspecting process ID", ex.Message);
            }

            if (!string.IsNullOrWhiteSpace(result.ExecutablePath) && installedApps != null)
            {
                result.MatchedApplication = MatchApplication(result.ExecutablePath, installedApps);
            }

            result.StatusMessage = result.MatchedApplication != null
                ? $"Matched application: {result.MatchedApplication.DisplayName}"
                : (result.ExecutablePath != null ? $"Found process: {Path.GetFileName(result.ExecutablePath)}" : "Unknown target");

            return result;
        }

        /// <summary>
        /// Inspects a target executable file or shortcut on disk and correlates it with installed applications.
        /// </summary>
        public static TargetInspectionResult InspectFile(string filePath, IEnumerable<ApplicationUninstallerEntry> installedApps)
        {
            var result = new TargetInspectionResult();
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                result.StatusMessage = "File does not exist";
                return result;
            }

            var normPath = SecurityGuard.NormalizePath(filePath);
            result.ExecutablePath = normPath;
            result.ProcessName = Path.GetFileNameWithoutExtension(normPath);

            if (installedApps != null)
            {
                result.MatchedApplication = MatchApplication(normPath, installedApps);
            }

            result.StatusMessage = result.MatchedApplication != null
                ? $"Matched application: {result.MatchedApplication.DisplayName}"
                : $"Identified file: {Path.GetFileName(normPath)}";

            return result;
        }

        private static ApplicationUninstallerEntry MatchApplication(string exeOrFilePath, IEnumerable<ApplicationUninstallerEntry> apps)
        {
            if (string.IsNullOrWhiteSpace(exeOrFilePath) || apps == null) return null;

            var targetNorm = SecurityGuard.NormalizePath(exeOrFilePath);
            var targetDir = Path.GetDirectoryName(targetNorm);

            // 1. Direct match with UninstallerFullFilename or InstallLocation
            foreach (var app in apps)
            {
                if (!string.IsNullOrWhiteSpace(app.UninstallerFullFilename))
                {
                    var uNorm = SecurityGuard.NormalizePath(app.UninstallerFullFilename);
                    if (string.Equals(uNorm, targetNorm, StringComparison.OrdinalIgnoreCase))
                        return app;
                }

                if (!string.IsNullOrWhiteSpace(app.InstallLocation))
                {
                    var iNorm = SecurityGuard.NormalizePath(app.InstallLocation);
                    if (targetNorm.StartsWith(iNorm, StringComparison.OrdinalIgnoreCase))
                        return app;
                }
            }

            // 2. Name resemblance in directory path
            if (!string.IsNullOrEmpty(targetDir))
            {
                var dirName = Path.GetFileName(targetDir);
                foreach (var app in apps)
                {
                    if (string.Equals(app.DisplayNameTrimmed, dirName, StringComparison.OrdinalIgnoreCase))
                        return app;
                }
            }

            return null;
        }
    }
}
