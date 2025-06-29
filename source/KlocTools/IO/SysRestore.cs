/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Management;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Klocman.IO
{
    /// <summary>
    ///     Based on http://www.codeproject.com/Articles/38205/Creating-System-Restore-Points-using-PInvoke
    ///     Updated for new Windows versions
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
        ///     Attempts to cancel the creation of a restore point. 
        ///     Note: Due to Windows API limitations, the restore point will still appear in the restore point list,
        ///     but will be marked as canceled for the application. It is not deleted.
        /// </summary>
        /// <param name="lSeqNum">The restore sequence number</param>
        /// <returns>The status of the call</returns>
        public static int CancelRestore(long lSeqNum)
        {
            return EndOrCancelRestore(lSeqNum, RestoreType.CancelledOperation);
        }

        /// <summary>
        ///     Ends system restore call
        /// </summary>
        public static int EndRestore(long lSeqNum)
        {
            return EndOrCancelRestore(lSeqNum, RestoreType.ApplicationUninstall);
        }

        private static int EndOrCancelRestore(long lSeqNum, RestoreType cancelType)
        {
            var rpInfo = new RestorePointInfo
            {
                dwEventType = EndSystemChange,
                dwRestorePtType = (int)cancelType,
                llSequenceNumber = lSeqNum
            };
            STATEMGRSTATUS rpStatus;

            if (!SysRestoreAvailable())
                return -1;

            try
            {
                var result = SRSetRestorePointW(ref rpInfo, out rpStatus);
                if (!result)
                {
                    var error = Marshal.GetLastWin32Error();
                    throw new Exception($"SRSetRestorePointW failed with error code: {error}");
                }
            }
            catch (Exception ex)
            {
                LogError("EndOrCancelRestore failed", ex);
                return -1;
            }

            return rpStatus.nStatus;
        }

        /// <summary>
        ///     Starts system restore
        /// </summary>
        /// <param name="strDescription">The description of the restore</param>
        /// <param name="lSeqNum">Returns the sequence number</param>
        /// <param name="creationFrequency">
        ///     Under Win 8 or newer - Minimal amount of minutes since last restore point for this point to be created. 
        ///     0 to always create, number for amount of minutes, -1 for default behaviour (24 hours).
        /// </param>
        /// <returns>The status of call</returns>
        /// <seealso>
        ///     <cref>Use EndRestore() or CancelRestore() to end the system restore</cref>
        /// </seealso>
        public static int StartRestore(string strDescription, out long lSeqNum, int creationFrequency = -1)
        {
            if (strDescription == null) throw new ArgumentNullException(nameof(strDescription));

            lSeqNum = 0;
            if (!SysRestoreAvailable())
                return -1;

            if (creationFrequency >= 0)
            {
                try
                {
                    using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore", true);
                    if (key == null)
                        throw new InvalidOperationException("SystemRestore registry key not found.");
                    key.SetValue("SystemRestorePointCreationFrequency", creationFrequency, RegistryValueKind.DWord);
                }
                catch (Exception ex)
                {
                    LogError("Failed to set SystemRestorePointCreationFrequency", ex);
                }
            }

            // Ensure description is under the max character limit
            if (strDescription.Length > MaxDescW)
                strDescription = strDescription.Substring(0, MaxDescW);

            var rpInfo = new RestorePointInfo
            {
                dwEventType = BeginSystemChange,
                dwRestorePtType = (int)RestoreType.ApplicationUninstall,
                llSequenceNumber = 0,
                szDescription = strDescription
            };
            STATEMGRSTATUS rpStatus;

            try
            {
                bool result = SRSetRestorePointW(ref rpInfo, out rpStatus);
                if (!result)
                {
                    var error = Marshal.GetLastWin32Error();
                    throw new Exception($"SRSetRestorePointW failed with error code: {error}");
                }
            }
            catch (Exception ex)
            {
                LogError("StartRestore failed", ex);
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
            try
            {
                // Check if System Protection is enabled for the system drive (usually C:)
                using var searcher = new ManagementObjectSearcher("root\\default", "SELECT * FROM SystemRestore");
                using var results = searcher.Get();
                foreach (ManagementObject _ in results)
                {
                    // If we can enumerate, System Restore is available
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogError("System Restore check failed", ex);
            }
            return false;
        }

        private static void LogError(string message, Exception ex = null)
        {
            Console.WriteLine($@"{message}{(ex != null ? ": " + ex : string.Empty)}");
        }

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