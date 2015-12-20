using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Klocman.Extensions;
using UninstallTools.Properties;

namespace UninstallTools.Uninstaller
{
    public class BulkUninstallEntry
    {
        internal BulkUninstallEntry(ApplicationUninstallerEntry uninstallerEntry, bool isSilent,
            UninstallStatus startingStatus)
        {
            CurrentStatus = startingStatus;
            IsSilent = isSilent;
            UninstallerEntry = uninstallerEntry;
        }

        public int Id { get; internal set; }

        public Exception CurrentError { get; set; }
        public UninstallStatus CurrentStatus { get; set; }
        public bool IsSilent { get; }
        public ApplicationUninstallerEntry UninstallerEntry { get; }

        public bool IsRunning { get; private set; }
        public bool Finished { get; private set; }

        private SkipCurrentLevel _skipLevel = SkipCurrentLevel.None;
        private readonly object _operationLock = new object();

        public void SkipWaiting(bool terminate)
        {
            lock (_operationLock)
            {
                if (Finished)
                    return;

                if (!IsRunning && CurrentStatus == UninstallStatus.Waiting)
                    CurrentStatus = UninstallStatus.Skipped;

                _skipLevel = terminate ? SkipCurrentLevel.Terminate : SkipCurrentLevel.Skip;
            }
        }

        /// <summary>
        ///     Run the uninstaller on a new thread.
        /// </summary>
        internal void RunUninstaller(bool preferQuiet, bool simulate)
        {
            lock (_operationLock)
            {
                if (Finished || IsRunning || CurrentStatus != UninstallStatus.Waiting)
                    return;

                if (UninstallerEntry.IsRegistered && !UninstallerEntry.RegKeyStillExists())
                {
                    CurrentStatus = UninstallStatus.Completed;
                    Finished = true;
                    return;
                }

                CurrentStatus = UninstallStatus.Uninstalling;
                IsRunning = true;
            }

            var worker = new Thread(UninstallThread) {Name = "RunBulkUninstall_Worker"};
            worker.Start(new KeyValuePair<bool, bool>(preferQuiet, simulate));
        }

        private bool _canRetry = true;

        private void UninstallThread(object parameters)
        {
            var kvp = (KeyValuePair<bool, bool>) parameters;
            var preferQuiet = kvp.Key;
            var simulate = kvp.Value;
            Exception error = null;
            var retry = false;
            try
            {
                var uninstaller = UninstallerEntry.RunUninstaller(preferQuiet, simulate);

                // Can be null during simulation
                if (uninstaller != null)
                {
                    var checkCounters = preferQuiet && UninstallerEntry.QuietUninstallPossible;
                    List<Process> childProcesses;
                    var idleCounter = 0;

                    do
                    {
                        if (_skipLevel == SkipCurrentLevel.Skip)
                            break;

                        childProcesses = uninstaller.GetChildProcesses().ToList();
                        if (!uninstaller.HasExited)
                            childProcesses.Add(uninstaller);

                        List<KeyValuePair<PerformanceCounter[], CounterSample[]>> counters = null;
                        if (checkCounters)
                        {
                            try
                            {
                                counters = (from process in childProcesses
                                    let processName = process.ProcessName
                                    let perfCounters = new[]
                                    {
                                        new PerformanceCounter("Process", "% Processor Time", processName, true),
                                        new PerformanceCounter("Process", "IO Data Bytes/sec", processName, true)
                                    }
                                    select new KeyValuePair<PerformanceCounter[], CounterSample[]>(
                                        perfCounters,
                                        new[] {perfCounters[0].NextSample(), perfCounters[1].NextSample()}
                                        // Important to enumerate them now, they will collect data when we sleep
                                        )).ToList();
                            }
                            catch
                            {
                                // Ignore errors caused by processes ending at bad times 
                                // BUG: Will leak objects without disposing if it crashes in middle of work
                            }
                        }

                        Thread.Sleep(1000);

                        if (counters != null && UninstallerEntry.UninstallerKind != UninstallerType.Msiexec) //Has problems with Msiexec
                        {
                            try
                            {
                                var anyWorking = false;
                                foreach (var c in counters)
                                {
                                    var c0 = CounterSample.Calculate(c.Value[0], c.Key[0].NextSample());
                                    var c1 = CounterSample.Calculate(c.Value[1], c.Key[1].NextSample());

                                    Debug.WriteLine("CPU " + c0 + "%, IO " + c1 + "B");

                                    // Check if process seems to be doing anything. Use 1% for CPU and 10KB for I/O
                                    if (c0 <= 1 && c1 <= 10240) continue;

                                    anyWorking = true;
                                    break;
                                }

                                idleCounter = anyWorking ? 0 : idleCounter + 1;
                            }
                            catch
                            {
                                // Ignore errors caused by processes ending at bad times
                            }
                            finally
                            {
                                // Remember to dispose of the counters
                                counters.ForEach(x =>
                                {
                                    x.Key[0].Dispose();
                                    x.Key[1].Dispose();
                                });
                            }
                        }

                        // Kill the uninstaller (and children) if user told us to or if it was idle for too long
                        if (_skipLevel == SkipCurrentLevel.Terminate || idleCounter > 40)
                        {
                            uninstaller.Kill(true);
                            if (idleCounter > 40)
                                throw new IOException(Localisation.UninstallError_UninstallerTimedOut);
                            break;
                        }
                    } while (!uninstaller.HasExited || childProcesses.Any());

                    if (_skipLevel == SkipCurrentLevel.None)
                    {
                        var exitVar = uninstaller.ExitCode;
                        if (exitVar != 0)
                        {
                            retry = true;
                            throw new IOException(Localisation.UninstallError_UninstallerReturnedCode + exitVar);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            // Take care of the aftermath
            if (_skipLevel != SkipCurrentLevel.None)
            {
                _skipLevel = SkipCurrentLevel.None;

                CurrentStatus = UninstallStatus.Skipped;
                CurrentError = new OperationCanceledException(Localisation.ManagerError_Skipped);
            }
            else if (error != null)
            {
                //Localisation.ManagerError_PrematureWorkerStop is unused
                CurrentStatus = UninstallStatus.Failed;
                CurrentError = error;
            }
            else
            {
                CurrentStatus = UninstallStatus.Completed;
            }

            if (retry && _canRetry)
            {
                CurrentStatus = UninstallStatus.Waiting;
                _canRetry = false;
            }
            else
            {
                Finished = true;
            }

            IsRunning = false;
        }

        internal enum SkipCurrentLevel
        {
            None = 0,
            Terminate,
            Skip
        }
    }
}