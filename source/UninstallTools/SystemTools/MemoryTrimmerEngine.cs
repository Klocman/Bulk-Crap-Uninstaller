/*
    EBUninstaller Pro - Memory & Standby Working Set Trim Engine
*/

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public sealed class MemoryTrimResult
    {
        public int TotalProcessesInspected { get; set; }
        public int TotalProcessesTrimmed { get; set; }
        public long EstimatedMemoryReclaimedBytes { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public static class MemoryTrimmerEngine
    {
        [DllImport("psapi.dll", SetLastError = true)]
        private static extern bool EmptyWorkingSet(IntPtr hProcess);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetProcessWorkingSetSize(IntPtr proc, IntPtr min, IntPtr max);

        public static MemoryTrimResult TrimSystemWorkingSet()
        {
            var result = new MemoryTrimResult();
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return result;

            long beforeTotal = GC.GetTotalMemory(false);

            try
            {
                var processes = Process.GetProcesses();
                result.TotalProcessesInspected = processes.Length;

                foreach (var proc in processes)
                {
                    try
                    {
                        if (proc.Id == 0 || proc.Id == 4) continue; // Idle & System
                        long wsBefore = proc.WorkingSet64;

                        if (EmptyWorkingSet(proc.Handle))
                        {
                            result.TotalProcessesTrimmed++;
                            long wsAfter = proc.WorkingSet64;
                            if (wsBefore > wsAfter)
                            {
                                result.EstimatedMemoryReclaimedBytes += (wsBefore - wsAfter);
                            }
                        }
                    }
                    catch { }
                    finally
                    {
                        proc.Dispose();
                    }
                }

                // Force local garbage collection
                GC.Collect(2, GCCollectionMode.Forced, true, true);
                GC.WaitForPendingFinalizers();
                GC.Collect(2, GCCollectionMode.Forced, true, true);

                long afterTotal = GC.GetTotalMemory(false);
                if (beforeTotal > afterTotal)
                {
                    result.EstimatedMemoryReclaimedBytes += (beforeTotal - afterTotal);
                }

                StructuredLogger.Info(LogCategory.SystemTools, $"Memory trim complete: {result.TotalProcessesTrimmed} processes trimmed, ~{result.EstimatedMemoryReclaimedBytes / (1024 * 1024.0):F1} MB reclaimed.");
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.SystemTools, "Error during memory trim operation", ex.Message);
            }

            return result;
        }
    }
}
