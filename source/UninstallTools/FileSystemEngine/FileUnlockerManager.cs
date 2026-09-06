/*
    EBUninstaller Pro - Windows Restart Manager & File Unlocker Engine
    Detects locking processes on stubborn files/folders and performs safe handle release or termination.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UninstallTools.Core;

namespace UninstallTools.FileSystemEngine
{
    public class LockProcessInfo
    {
        public int ProcessId { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = string.Empty;
        public string MainModulePath { get; set; } = string.Empty;
        public bool IsSystemProcess { get; set; }
    }

    public static class FileUnlockerManager
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct RM_UNIQUE_PROCESS
        {
            public int dwProcessId;
            public System.Runtime.InteropServices.ComTypes.FILETIME ProcessStartTime;
        }

        private const int CCH_RM_MAX_APP_NAME = 255;
        private const int CCH_RM_MAX_SVC_NAME = 63;

        private enum RM_APP_TYPE
        {
            RmUnknownApp = 0,
            RmMainWindow = 1,
            RmOtherWindow = 2,
            RmService = 3,
            RmExplorer = 4,
            RmConsole = 5,
            RmCritical = 1000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct RM_PROCESS_INFO
        {
            public RM_UNIQUE_PROCESS Process;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_APP_NAME + 1)]
            public string strAppName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_SVC_NAME + 1)]
            public string strServiceShortName;
            public RM_APP_TYPE ApplicationType;
            public uint AppStatus;
            public uint TSSessionId;
            [MarshalAs(UnmanagedType.Bool)]
            public bool bRestartable;
        }

        [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
        private static extern int RmStartSession(out uint pSessionHandle, int dwSessionFlags, string strSessionKey);

        [DllImport("rstrtmgr.dll")]
        private static extern int RmEndSession(uint pSessionHandle);

        [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
        private static extern int RmRegisterResources(uint pSessionHandle, uint nFiles, string[] rgsFileNames, uint nApplications, RM_UNIQUE_PROCESS[]? rgApplications, uint nServices, string[]? rgsServiceNames);

        [DllImport("rstrtmgr.dll")]
        private static extern int RmGetList(uint dwSessionHandle, out uint pnProcInfoNeeded, ref uint pnProcInfo, [In, Out] RM_PROCESS_INFO[]? rgAffectedApps, ref uint lpdwRebootReasons);

        public static List<LockProcessInfo> FindLockingProcesses(string targetPath)
        {
            var results = new List<LockProcessInfo>();
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || string.IsNullOrWhiteSpace(targetPath))
                return results;

            if (!File.Exists(targetPath) && !Directory.Exists(targetPath))
                return results;

            uint sessionHandle;
            string key = Guid.NewGuid().ToString();
            int res = RmStartSession(out sessionHandle, 0, key);
            if (res != 0) return results;

            try
            {
                string[] resources = new[] { Path.GetFullPath(targetPath) };
                res = RmRegisterResources(sessionHandle, (uint)resources.Length, resources, 0, null, 0, null);
                if (res != 0) return results;

                uint procInfoNeeded = 0;
                uint procInfo = 0;
                uint rebootReasons = 0;

                res = RmGetList(sessionHandle, out procInfoNeeded, ref procInfo, null, ref rebootReasons);
                if (res == 234) // ERROR_MORE_DATA
                {
                    var processInfo = new RM_PROCESS_INFO[procInfoNeeded];
                    procInfo = procInfoNeeded;

                    res = RmGetList(sessionHandle, out procInfoNeeded, ref procInfo, processInfo, ref rebootReasons);
                    if (res == 0)
                    {
                        for (int i = 0; i < procInfo; i++)
                        {
                            try
                            {
                                int pid = processInfo[i].Process.dwProcessId;
                                using var proc = Process.GetProcessById(pid);
                                string procName = proc.ProcessName;
                                string modulePath = string.Empty;
                                try { modulePath = proc.MainModule?.FileName ?? string.Empty; } catch { }

                                bool isSystem = SecurityGuard.IsProtectedPath(modulePath) ||
                                                string.Equals(procName, "explorer", StringComparison.OrdinalIgnoreCase) ||
                                                string.Equals(procName, "svchost", StringComparison.OrdinalIgnoreCase) ||
                                                string.Equals(procName, "services", StringComparison.OrdinalIgnoreCase) ||
                                                string.Equals(procName, "csrss", StringComparison.OrdinalIgnoreCase);

                                results.Add(new LockProcessInfo
                                {
                                    ProcessId = pid,
                                    ProcessName = procName,
                                    ApplicationName = processInfo[i].strAppName,
                                    MainModulePath = modulePath,
                                    IsSystemProcess = isSystem
                                });
                            }
                            catch
                            {
                                // Process might have terminated in between
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Warning, "FileUnlockerManager", $"Error detecting file locks on {targetPath}: {ex.Message}");
            }
            finally
            {
                RmEndSession(sessionHandle);
            }

            return results;
        }

        public static bool TerminateLockingProcess(int processId)
        {
            try
            {
                using var proc = Process.GetProcessById(processId);
                string procName = proc.ProcessName;

                // Never terminate critical Windows processes
                if (string.Equals(procName, "csrss", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(procName, "services", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(procName, "lsass", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(procName, "winlogon", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                proc.Kill();
                proc.WaitForExit(3000);
                StructuredLogger.Log(LogLevel.Info, "FileUnlockerManager", $"Terminated locking process {procName} (PID {processId})");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Error, "FileUnlockerManager", $"Failed to terminate PID {processId}: {ex.Message}");
                return false;
            }
        }

        public static bool UnlockAndDelete(string targetPath)
        {
            if (SecurityGuard.IsProtectedPath(targetPath))
                return false;

            var lockingProcesses = FindLockingProcesses(targetPath);
            foreach (var lockProc in lockingProcesses)
            {
                if (!lockProc.IsSystemProcess)
                {
                    TerminateLockingProcess(lockProc.ProcessId);
                }
            }

            try
            {
                if (File.Exists(targetPath))
                {
                    File.SetAttributes(targetPath, FileAttributes.Normal);
                    File.Delete(targetPath);
                    return true;
                }
                if (Directory.Exists(targetPath))
                {
                    Directory.Delete(targetPath, true);
                    return true;
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Warning, "FileUnlockerManager", $"Could not delete {targetPath} even after unlock: {ex.Message}");
            }

            return false;
        }
    }
}
