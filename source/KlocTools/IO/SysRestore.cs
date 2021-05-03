/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Klocman.Extensions;
using Klocman.Tools;
using Microsoft.Win32;

namespace Klocman.IO
{
    /// <summary>
    ///     Code from http://www.codeproject.com/Articles/38205/Creating-System-Restore-Points-using-PInvoke
    /// </summary>
    public static class SysRestore
    {
        // Type of restorations
        public enum RestoreType
        {
            ApplicationInstall = 0, // Installing a new application
            ApplicationUninstall = 1, // An application has been uninstalled
            ModifySettings = 12, // An application has had features added or removed
            CancelledOperation = 13, // An application needs to delete the restore point it created
            Restore = 6, // System Restore
            Checkpoint = 7, // Checkpoint
            DeviceDriverInstall = 10, // Device driver has been installed
            FirstRun = 11, // Program used for 1st time
            BackupRecovery = 14 // Restoring a backup
        }

        private const short AccessibilitySetting = 3; /* not implemented */
        private const short ApplicationRun = 5; /* not implemented */
        // Windows XP only - used to prevent the restore points intertwined
        private const short BeginNestedSystemChange = 102;
        // Constants
        private const short BeginSystemChange = 100; // Start of operation
        private const short DesktopSetting = 2; /* not implemented */
        private const short EndNestedSystemChange = 103;
        private const short EndSystemChange = 101; // End of operation
        private const short MaxDesc = 64;
        private const short MaxDescW = 256;
        private const short OeSetting = 4; /* not implemented */
        private const short WindowsBoot = 9; /* not implemented */
        private const short WindowsShutdown = 8; /* not implemented */

        /// <summary>
        ///     Cancels restore call
        /// </summary>
        /// <param name="lSeqNum">The restore sequence number</param>
        /// <returns>The status of call</returns>
        public static int CancelRestore(long lSeqNum)
        {
            var rpInfo = new RestorePointInfo();
            STATEMGRSTATUS rpStatus;

            if (!SysRestoreAvailable())
                return -1;

            try
            {
                rpInfo.dwEventType = EndSystemChange;
                rpInfo.dwRestorePtType = (int)RestoreType.CancelledOperation;
                rpInfo.llSequenceNumber = lSeqNum;

                SRSetRestorePointW(ref rpInfo, out rpStatus);
            }
            catch (DllNotFoundException)
            {
                return -1;
            }

            return rpStatus.nStatus;
        }

        /// <summary>
        ///     Ends system restore call
        /// </summary>
        /// <param name="lSeqNum">The restore sequence number</param>
        /// <returns>The status of call</returns>
        public static int EndRestore(long lSeqNum)
        {
            var rpInfo = new RestorePointInfo();
            STATEMGRSTATUS rpStatus;

            if (!SysRestoreAvailable())
                return -1;

            try
            {
                rpInfo.dwEventType = EndSystemChange;
                rpInfo.llSequenceNumber = lSeqNum;

                SRSetRestorePointW(ref rpInfo, out rpStatus);
            }
            catch (DllNotFoundException)
            {
                return -1;
            }

            return rpStatus.nStatus;
        }

        /// <summary>
        ///     Starts system restore
        /// </summary>
        /// <param name="strDescription">The description of the restore</param>
        /// <param name="rt">The type of restore point</param>
        /// <param name="lSeqNum">Returns the sequence number</param>
        /// <param name="creationFrequency">
        ///     Under Win 8 or newer - Minimal amount of minutes since last restore point for this point to be created. 
        ///     0 to always create, number for amount of minutes, -1 for default behaviour (24 hours).
        /// </param>
        /// <returns>The status of call</returns>
        /// <seealso>
        ///     <cref>Use EndRestore() or CancelRestore() to end the system restore</cref>
        /// </seealso>
        public static int StartRestore(string strDescription, RestoreType rt, out long lSeqNum, int creationFrequency = -1)
        {
            var rpInfo = new RestorePointInfo();
            STATEMGRSTATUS rpStatus;

            if (!SysRestoreAvailable())
            {
                lSeqNum = 0;
                return -1;
            }

            if (creationFrequency >= 0)
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore", true))
                {
                    Debug.Assert(key != null, "SystemRestore key must exist after SysRestoreAvailable");
                    key.SetValue("SystemRestorePointCreationFrequency", creationFrequency, RegistryValueKind.DWord);
                }
            }

            try
            {
                // Prepare Restore Point
                rpInfo.dwEventType = BeginSystemChange;
                // By default we create a verification system
                rpInfo.dwRestorePtType = (int)rt;
                rpInfo.llSequenceNumber = 0;
                rpInfo.szDescription = strDescription;

                SRSetRestorePointW(ref rpInfo, out rpStatus);
            }
            catch (DllNotFoundException)
            {
                lSeqNum = 0;
                return -1;
            }

            lSeqNum = rpStatus.llSequenceNumber;

            return rpStatus.nStatus;
        }

        /// <summary>
        /// Check if system restore is supported and enabled
        /// </summary>
        public static bool SysRestoreAvailable()
        {
            // See if sys restore is enabled
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore", false))
            {
                if (key == null) return false;
                if (key.GetStringSafe("SRInitDone") != "1" && key.GetStringSafe("RPSessionInterval") != "1") return false;
            }

            // See if DLL exists
            var sbPath = new StringBuilder(260);
            if (SearchPath(null, "srclient.dll", null, 260, sbPath, null) != 0)
                return true;

            // Fall back to checking by system version
            var majorVersion = Environment.OSVersion.Version.Major;
            var minorVersion = Environment.OSVersion.Version.Minor;

            // Windows ME
            if (majorVersion == 4 && minorVersion == 90) return true;

            // Windows XP
            if (majorVersion == 5 && minorVersion == 1) return true;

            // Windows Vista
            if (majorVersion == 6 && minorVersion == 0) return true;

            // Windows Se7en
            if (majorVersion == 6 && minorVersion == 1) return true;

            if (Environment.OSVersion.Version >= WindowsTools.Windows8) return true;

            // All others : Win 95, 98, 2000, Server
            return false;
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern uint SearchPath(string lpPath,
            string lpFileName,
            string lpExtension,
            int nBufferLength,
            [MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpBuffer,
            string lpFilePart);

        [DllImport("srclient.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SRSetRestorePointW(ref RestorePointInfo pRestorePtSpec,
            out STATEMGRSTATUS pSMgrStatus);

        /// <summary>
        ///     Contains information used by the SRSetRestorePoint function
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct RestorePointInfo
        {
            public int dwEventType; // The type of event
            public int dwRestorePtType; // The type of restore point
            public long llSequenceNumber; // The sequence number of the restore point
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxDescW + 1)] public string szDescription;

            // The description to be displayed so the user can easily identify a restore point
        }

        /// <summary>
        ///     Contains status information used by the SRSetRestorePoint function
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct STATEMGRSTATUS
        {
            public int nStatus; // The status code
            public long llSequenceNumber; // The sequence number of the restore point
        }
    }
}