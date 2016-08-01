using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private readonly object _operationLock = new object();

        private bool _canRetry = true;

        private SkipCurrentLevel _skipLevel = SkipCurrentLevel.None;

        internal BulkUninstallEntry(ApplicationUninstallerEntry uninstallerEntry, bool isSilent,
            UninstallStatus startingStatus)
        {
            CurrentStatus = startingStatus;
            IsSilent = isSilent;
            UninstallerEntry = uninstallerEntry;
        }

        public int Id { get; internal set; }

        public Exception CurrentError { get; private set; }
        public UninstallStatus CurrentStatus { get; private set; }
        public bool IsSilent { get; set; }
        public ApplicationUninstallerEntry UninstallerEntry { get; }

        public bool IsRunning { get; private set; }
        public bool Finished { get; private set; }

        public void Reset()
        {
            CurrentError = null;
            CurrentStatus = UninstallStatus.Waiting;
            IsRunning = false;
            Finished = false;
        }

        public void SkipWaiting(bool terminate)
        {
            lock (_operationLock)
            {
                if (Finished)
                    return;

                if (!IsRunning && CurrentStatus == UninstallStatus.Waiting)
                    CurrentStatus = UninstallStatus.Skipped;

                // Do not allow skipping of Msiexec uninstallers because they will hold up the rest of Msiexec uninstallers in the task
                if (CurrentStatus == UninstallStatus.Uninstalling &&
                    UninstallerEntry.UninstallerKind == UninstallerType.Msiexec &&
                    !terminate)
                    return;

                _skipLevel = terminate ? SkipCurrentLevel.Terminate : SkipCurrentLevel.Skip;
            }
        }

        /// <summary>
        ///     Run the uninstaller on a new thread.
        /// </summary>
        internal void RunUninstaller(RunUninstallerOptions options)
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

            var worker = new Thread(UninstallThread) { Name = "RunBulkUninstall_Worker" };
            worker.Start(options);
        }

        public sealed class RunUninstallerOptions
        {
            public RunUninstallerOptions(bool autoKillStuckQuiet, bool retryFailedQuiet, bool preferQuiet, bool simulate)
            {
                AutoKillStuckQuiet = autoKillStuckQuiet;
                RetryFailedQuiet = retryFailedQuiet;
                PreferQuiet = preferQuiet;
                Simulate = simulate;
            }

            public bool AutoKillStuckQuiet { get; }
            public bool RetryFailedQuiet { get; }
            public bool PreferQuiet { get; }
            public bool Simulate { get; }
        }

        private void UninstallThread(object parameters)
        {
            var options = parameters as RunUninstallerOptions;
            Debug.Assert(options != null, "options != null");

            Exception error = null;
            var retry = false;
            try
            {
                using (var uninstaller = UninstallerEntry.RunUninstaller(options.PreferQuiet, options.Simulate))
                {
                    // Can be null during simulation
                    if (uninstaller != null)
                    {
                        if (options.PreferQuiet && UninstallerEntry.QuietUninstallPossible)
                            uninstaller.PriorityClass = ProcessPriorityClass.BelowNormal;

                        var checkCounters = options.PreferQuiet && options.AutoKillStuckQuiet && UninstallerEntry.QuietUninstallPossible;
                        List<Process> childProcesses;
                        var idleCounter = 0;

                        do
                        {
                            if (_skipLevel == SkipCurrentLevel.Skip)
                                break;

                            childProcesses = uninstaller.GetChildProcesses().Where(
                                p => !p.ProcessName.Contains("explorer", StringComparison.InvariantCultureIgnoreCase))
                                .ToList();

                            if (!uninstaller.HasExited)
                                childProcesses.Add(uninstaller);

                            if (checkCounters)
                            {
                                if (TestUninstallerForStalls(childProcesses))
                                    idleCounter++;

                                // Kill the uninstaller (and children) if they were idle/stalled for too long
                                if (idleCounter > 40)
                                {
                                    uninstaller.Kill(true);
                                    throw new IOException(Localisation.UninstallError_UninstallerTimedOut);
                                }
                            }
                            else Thread.Sleep(1000);

                            // Kill the uninstaller (and children) if user told us to or if it was idle for too long
                            if (_skipLevel == SkipCurrentLevel.Terminate)
                            {
                                uninstaller.Kill(true);
                                if (UninstallerEntry.UninstallerKind == UninstallerType.Msiexec)
                                {
                                    foreach (var process in Process.GetProcessesByName("Msiexec"))
                                    {
                                        try { process.Kill(); }
                                        catch (InvalidOperationException) { }
                                        catch (Win32Exception) { }
                                        catch (NotSupportedException) { }
                                    }
                                }
                                break;
                            }
                        } while (!uninstaller.HasExited || childProcesses.Any(p => !p.HasExited));

                        if (_skipLevel == SkipCurrentLevel.None)
                        {
                            var exitVar = uninstaller.ExitCode;
                            if (exitVar != 0)
                            {
                                if (UninstallerEntry.UninstallerKind == UninstallerType.Msiexec && exitVar == 1602)
                                {
                                    // 1602 ERROR_INSTALL_USEREXIT - The user has cancelled the installation.
                                    _skipLevel = SkipCurrentLevel.Skip;
                                }
                                else if (exitVar == -1073741510)
                                {
                                    /* 3221225786 / 0xC000013A / -1073741510 
                                    The application terminated as a result of a CTRL+C. 
                                    Indicates that the application has been terminated either by user's 
                                    keyboard input CTRL+C or CTRL+Break or closing command prompt window. */
                                    _skipLevel = SkipCurrentLevel.Terminate;
                                }
                                else
                                {
                                    switch (exitVar)
                                    {
                                        // The system cannot find the file specified. Indicates that the file can not be found in specified location.
                                        case 2:
                                        // The system cannot find the path specified. Indicates that the specified path can not be found.
                                        case 3:
                                        // Access is denied. Indicates that user has no access right to specified resource.
                                        case 5:
                                        // Program is not recognized as an internal or external command, operable program or batch file. 
                                        case 9009:
                                            break;
                                        default:
                                            if (options.RetryFailedQuiet)
                                                retry = true;
                                            break;
                                    }
                                    throw new IOException(Localisation.UninstallError_UninstallerReturnedCode + exitVar);
                                }
                            }
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

        /// <summary>
        /// Returns true if uninstaller appears to be stalled. Blocks for 1000ms to gather data.
        /// </summary>
        private bool TestUninstallerForStalls(List<Process> childProcesses)
        {
            List<KeyValuePair<PerformanceCounter[], CounterSample[]>> counters = null;
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
                                new[] { perfCounters[0].NextSample(), perfCounters[1].NextSample() }
                                // Important to enumerate them now, they will collect data when we sleep
                                )).ToList();
            }
            catch
            {
                // Ignore errors caused by processes ending at bad times 
                // BUG: Will leak objects without disposing if it crashes in middle of work
            }

            Thread.Sleep(1000);

            //Has problems with Msiexec
            if (counters != null && UninstallerEntry.UninstallerKind != UninstallerType.Msiexec)
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

                    return !anyWorking;
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

            return false;
        }

        internal enum SkipCurrentLevel
        {
            None = 0,
            Terminate,
            Skip
        }
    }
}