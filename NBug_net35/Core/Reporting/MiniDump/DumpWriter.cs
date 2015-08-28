// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DumpWriter.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using NBug.Core.Util.Exceptions;
using NBug.Core.Util.Logging;
using NBug.Enums;

namespace NBug.Core.Reporting.MiniDump
{
    /// <summary>
    ///     Sample usage:
    ///     <code>
    ///  using (FileStream fs = new FileStream("minidump.mdmp", FileMode.Create, FileAccess.ReadWrite, FileShare.Write))
    ///  {
    /// 		DumpWriter.Write(fs.SafeFileHandle, DumpTypeFlag.WithDataSegs | DumpTypeFlag.WithHandleData);
    ///  }
    ///  </code>
    /// </summary>
    /// <remarks>Code snippet is from http://blogs.msdn.com/b/dondu/archive/2010/10/24/writing-minidumps-in-c.aspx </remarks>
    internal static class DumpWriter
    {
        /// <summary>
        ///     Creates a new memory dump and writes it to the specified file (only if Settings.MiniDumpType != MiniDumpType.None).
        /// </summary>
        /// <param name="minidumpFilePath">The minidump file path. Overwritten if exists.</param>
        /// <returns>True if Settings.MiniDumpType settings is set to anything else then MiniDumpType.None.</returns>
        internal static bool Write(string minidumpFilePath)
        {
            if (Settings.MiniDumpType != MiniDumpType.None)
            {
                bool created;

                using (var fileStream = new FileStream(minidumpFilePath, FileMode.Create, FileAccess.Write))
                {
                    // ToDo: Create the minidump at a seperate process! Use this to deal with access errors: http://social.msdn.microsoft.com/Forums/en/csharpgeneral/thread/c314e6ca-4892-41e7-ae19-b3a36ad640e9
                    // Bug: In process minidumps causes all sorts of access problems (i.e. one of them is explained below, debugger prevents accessing private memory)
                    created = Write(fileStream.SafeFileHandle, Settings.MiniDumpType.ToString());
                }

                if (created)
                {
                    return true;
                }
                File.Delete(minidumpFilePath);
                return false;
            }
            return false;
        }

        [DllImport("kernel32.dll", EntryPoint = "GetCurrentThreadId", ExactSpelling = true, SetLastError = true)]
        private static extern uint GetCurrentThreadId();

        [DllImport("dbghelp.dll", EntryPoint = "MiniDumpWriteDump", CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Unicode,
            ExactSpelling = true, SetLastError = true)]
        private static extern bool MiniDumpWriteDump(
            IntPtr hProcess,
            uint processId,
            SafeHandle hFile,
            uint dumpType,
            ref MiniDumpExceptionInformation expParam,
            IntPtr userStreamParam,
            IntPtr callbackParam);

        // Overload supporting MiniDumpExceptionInformation == NULL
        [DllImport("dbghelp.dll", EntryPoint = "MiniDumpWriteDump", CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Unicode,
            ExactSpelling = true, SetLastError = true)]
        private static extern bool MiniDumpWriteDump(
            IntPtr hProcess, uint processId, SafeHandle hFile, uint dumpType, IntPtr expParam, IntPtr userStreamParam,
            IntPtr callbackParam);

        private static bool Write(SafeHandle fileHandle, string dumpType)
        {
            if (dumpType.ToLower() == MiniDumpType.Tiny.ToString().ToLower())
            {
                return Write(fileHandle, DumpTypeFlag.WithIndirectlyReferencedMemory | DumpTypeFlag.ScanMemory);
            }
            if (dumpType.ToLower() == MiniDumpType.Normal.ToString().ToLower())
            {
                //// If the debugger is attached, it is not possible to access private read-write memory
                //if (Debugger.IsAttached)
                //{
                //    return Write(fileHandle, DumpTypeFlag.WithDataSegs | DumpTypeFlag.WithHandleData | DumpTypeFlag.WithUnloadedModules);
                //}
                //else
                //{
                //    // Bug: Combination of WithPrivateReadWriteMemory + WithDataSegs hangs Visual Studio 2010 SP1 on some cases while loading the minidump for debugging in mixed mode which was created in by a release build application
                //    return Write(
                //        fileHandle, DumpTypeFlag.WithPrivateReadWriteMemory | DumpTypeFlag.WithDataSegs | DumpTypeFlag.WithHandleData | DumpTypeFlag.WithUnloadedModules);
                //}
                return Write(fileHandle, DumpTypeFlag.Normal);
            }
            if (dumpType.ToLower() == MiniDumpType.Full.ToString().ToLower())
            {
                return Write(fileHandle, DumpTypeFlag.WithFullMemory);
            }
            throw NBugConfigurationException.Create(() => Settings.MiniDumpType,
                "Parameter supplied for settings property is invalid.");
        }

        private static bool Write(SafeHandle fileHandle, DumpTypeFlag dumpTypeFlag)
        {
            var currentProcess = Process.GetCurrentProcess();
            var currentProcessHandle = currentProcess.Handle;
            var currentProcessId = (uint) currentProcess.Id;
            MiniDumpExceptionInformation exp;
            exp.ThreadId = GetCurrentThreadId();
            exp.ClientPointers = false;
            exp.ExceptionPointers = IntPtr.Zero;
            exp.ExceptionPointers = Marshal.GetExceptionPointers();

            var bRet = false;

            try
            {
                if (exp.ExceptionPointers == IntPtr.Zero)
                {
                    bRet = MiniDumpWriteDump(currentProcessHandle, currentProcessId, fileHandle, (uint) dumpTypeFlag,
                        IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                }
                else
                {
                    bRet = MiniDumpWriteDump(currentProcessHandle, currentProcessId, fileHandle, (uint) dumpTypeFlag,
                        ref exp, IntPtr.Zero, IntPtr.Zero);
                }
            }
            catch (DllNotFoundException)
            {
                Logger.Warning(
                    "dbghelp.dll was not found inside the application folder, the system path or SDK folder. Minidump was not generated. If you are not planning on using the minidump feature, you can disable it with the Configurator tool.");
                return false;
            }

            if (!bRet)
            {
                Logger.Error(
                    "Cannot write the minidump. MiniDumpWriteDump (dbghelp.dll) function returned error code: " +
                    Marshal.GetLastWin32Error());
                return false;
            }
            return true;
        }

        /* typedef struct _MINIDUMP_EXCEPTION_INFORMATION {
		 *    DWORD ThreadId;
		 *    PEXCEPTION_POINTERS ExceptionPointers;
		 *    BOOL ClientPointers;
		 * } MINIDUMP_EXCEPTION_INFORMATION, *PMINIDUMP_EXCEPTION_INFORMATION;
		 */

        [StructLayout(LayoutKind.Sequential, Pack = 4)] // Pack=4 is important! So it works also for x64!
        private struct MiniDumpExceptionInformation
        {
            public uint ThreadId;

            public IntPtr ExceptionPointers;

            [MarshalAs(UnmanagedType.Bool)] public bool ClientPointers;
        }
    }
}