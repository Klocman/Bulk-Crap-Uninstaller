/*
 * EBUninstaller Pro - Application Uninstaller & System Optimization Suite
 * HelperTools Shared Utilities Subsystem
 * Copyright (C) 2026 EBUninstaller Development Team & Contributors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Klocman
{
    /// <summary>
    /// Quick diagnostic system and memory metrics for helper sub-processes.
    /// </summary>
    internal static class SystemEnvironmentInfo
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public MEMORYSTATUSEX()
            {
                dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        /// <summary>
        /// Gets total installed physical RAM in bytes.
        /// </summary>
        public static ulong GetTotalPhysicalMemoryBytes()
        {
            try
            {
                var stat = new MEMORYSTATUSEX();
                if (GlobalMemoryStatusEx(stat))
                {
                    return stat.ullTotalPhys;
                }
            }
            catch { }

            return 0;
        }

        /// <summary>
        /// Gets available physical RAM in bytes.
        /// </summary>
        public static ulong GetAvailablePhysicalMemoryBytes()
        {
            try
            {
                var stat = new MEMORYSTATUSEX();
                if (GlobalMemoryStatusEx(stat))
                {
                    return stat.ullAvailPhys;
                }
            }
            catch { }

            return 0;
        }

        /// <summary>
        /// Gets current process working set memory in bytes.
        /// </summary>
        public static long GetCurrentProcessMemoryUsageBytes()
        {
            try
            {
                using var proc = Process.GetCurrentProcess();
                proc.Refresh();
                return proc.WorkingSet64;
            }
            catch
            {
                return 0;
            }
        }
    }
}
