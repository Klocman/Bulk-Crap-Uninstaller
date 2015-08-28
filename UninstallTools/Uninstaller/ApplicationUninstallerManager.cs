using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Native;
using Klocman.Tools;
using Microsoft.Win32;
using UninstallTools.Properties;

namespace UninstallTools.Uninstaller
{
    public static class ApplicationUninstallerManager
    {
        public delegate void GetUninstallerListCallback(GetUninstallerListProgress progressReport);

        public static readonly string RegUninstallersKeyDirect = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        public static readonly string RegUninstallersKeyWow =
            @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

        /// <summary>
        ///     Gets populated automatically when running GetUninstallerList. Returns null before first update.
        /// </summary>
        internal static IEnumerable<Guid> WindowsInstallerValidGuids { get; private set; }

        public static IEnumerable<ApplicationUninstallerEntry> GetApplicationsFromDrive(
            IEnumerable<ApplicationUninstallerEntry> existingUninstallerEntries, GetUninstallerListCallback callback)
        {
            var existingUninstallers = existingUninstallerEntries as IList<ApplicationUninstallerEntry> ??
                                       existingUninstallerEntries.ToList();

            // Get the directiories to look inside of for programs
            var pfDirectories = new List<KeyValuePair<DirectoryInfo, bool>>(2);

            var pf64 = new DirectoryInfo(WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_PROGRAM_FILES));
            var pf32 = new DirectoryInfo(WindowsTools.GetProgramFilesX86Path());
            pfDirectories.Add(new KeyValuePair<DirectoryInfo, bool>(pf32, false));
            if (!pf32.FullName.Equals(pf64.FullName))
                pfDirectories.Add(new KeyValuePair<DirectoryInfo, bool>(pf64, true));

            foreach (var dir in UninstallToolsGlobalConfig.CustomProgramFiles.Distinct())
            {
                try
                {
                    var di = new DirectoryInfo(dir);
                    if (di.Exists)
                        pfDirectories.Add(new KeyValuePair<DirectoryInfo, bool>(di, false));
                }
                catch
                {
                    // Ignore missing or inaccessible directories
                }
            }

            // Get sub directories which should contain the programs
            var directoriesToCheck = pfDirectories.Aggregate(Enumerable.Empty<KeyValuePair<DirectoryInfo, bool>>(),
                (a, b) =>
                    a.Concat(b.Key.GetDirectories().Select(x => new KeyValuePair<DirectoryInfo, bool>(x, b.Value))));

            // Get directories which are already used and should be skipped
            var directoriesToSkip = existingUninstallers.SelectMany(x =>
            {
                if (string.IsNullOrEmpty(x.DisplayIcon)) return new[] {x.InstallLocation, x.UninstallerLocation};
                try
                {
                    var iconDir =
                        PathTools.GetDirectory(ProcessTools.SeparateArgsFromCommand(x.DisplayIcon).FileName);
                    return new[] {x.InstallLocation, x.UninstallerLocation, iconDir};
                }
                catch
                {
                    return new[] {x.InstallLocation, x.UninstallerLocation};
                }
            }).Where(x => x.IsNotEmpty()).ToList();

            var inputs = directoriesToCheck.Where(x => !directoriesToSkip.Any(y =>
                x.Key.FullName.Contains(y, StringComparison.InvariantCultureIgnoreCase)
                || y.Contains(x.Key.FullName, StringComparison.InvariantCultureIgnoreCase))).ToList();

            var results = new List<ApplicationUninstallerEntry>();
            var itemId = 0;
            foreach (var directory in inputs)
            {
                itemId++;

                var progress = new GetUninstallerListProgress(inputs.Count) {CurrentCount = itemId};
                callback(progress);

                if (UninstallToolsGlobalConfig.IsSystemDirectory(directory.Key) ||
                    directory.Key.Name.StartsWith("Windows"))
                    continue;

                //Try to get the main executable from the filtered folders. If no executables are present check subfolders.
                var detectedEntries = ApplicationUninstallerFactory.TryCreateFromDirectory(directory.Key,
                    directory.Value);

                results.AddRange(detectedEntries.Where(entry => !existingUninstallers.Any(x =>
                {
                    if (x.DisplayName.Contains(entry.DisplayNameTrimmed))
                    {
                        return string.IsNullOrEmpty(x.InstallLocation)
                               ||
                               entry.InstallLocation.Contains(x.InstallLocation,
                                   StringComparison.CurrentCultureIgnoreCase);
                    }
                    return false;
                })));

                //if (result != null && !existingUninstallers.Any(x => x.DisplayName.Contains(result.DisplayNameTrimmed)))
                //    results.Add(result);
            }

            return results;
        }

        /// <summary>
        ///     Search the system for valid uninstallers, parse them into coherent objects and return the resulting list.
        /// </summary>
        /// <exception cref="System.Security.SecurityException">
        ///     The user does not have the permissions required to read the
        ///     registry key.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="T:Microsoft.Win32.RegistryKey" /> is closed (closed keys
        ///     cannot be accessed).
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">The user does not have the necessary registry rights.</exception>
        /// <exception cref="IOException">A system error occurred, for example the current key has been deleted.</exception>
        public static IEnumerable<ApplicationUninstallerEntry> GetUninstallerList(GetUninstallerListCallback callback)
        {
            PopulateWindowsInstallerValidGuids();

            var keysToCheck = GetRegistryKeys();
            var uninstallersToCreate = new List<KeyValuePair<RegistryKey, bool>>();

            foreach (var kvp in keysToCheck.Where(kvp => kvp.Key != null))
            {
                uninstallersToCreate.AddRange(from subkeyName in kvp.Key.GetSubKeyNames()
                    select kvp.Key.OpenSubKey(subkeyName)
                    into subkey
                    where subkey != null
                    select new KeyValuePair<RegistryKey, bool>(subkey, kvp.Value));

                kvp.Key.Close();
            }

            var itemId = 0;
            var applicationUninstallers = new List<ApplicationUninstallerEntry>();
            foreach (var uninstallerToCreate in uninstallersToCreate)
            {
                try
                {
                    var entry = ApplicationUninstallerFactory.TryCreateFromRegistry(uninstallerToCreate.Key,
                        uninstallerToCreate.Value);
                    if (entry != null)
                        applicationUninstallers.Add(entry);
                }
                catch
                {
                    //Uninstaller is invalid or there is no uninstaller in the first place. Skip it to avoid problems.
                }
                finally
                {
                    uninstallerToCreate.Key.Close();

                    itemId++;
                    var progress = new GetUninstallerListProgress(uninstallersToCreate.Count) {CurrentCount = itemId};
                    callback(progress);
                }
            }

            return applicationUninstallers;
        }

        public static IEnumerable<ApplicationUninstallerEntry> GetWindowsFeaturesList(
            GetUninstallerListCallback callback)
        {
            if (!DismTools.DismIsAvailable)
                return Enumerable.Empty<ApplicationUninstallerEntry>();

            var applicationUninstallers = new List<ApplicationUninstallerEntry>();

            var features = DismTools.GetFeatures().Where(x => x.Value).ToList();
            var itemId = 0;
            foreach (var feature in features)
            {
                var info = DismTools.GetFeatureInfo(feature.Key);

                //ApplicationUninstallerEntry entry = null;
                var entry = new ApplicationUninstallerEntry
                {
                    RawDisplayName = info.DisplayName,
                    Comment = info.Description,
                    UninstallString = DismTools.GetDismUninstallString(feature.Key, false),
                    QuietUninstallString = DismTools.GetDismUninstallString(feature.Key, true),
                    UninstallerKind = UninstallerType.Dism,
                    Publisher = Localisation.WindowsFeatureTitle,
                    IsValid = true,
                    Is64Bit = ProcessTools.Is64BitProcess
                };

                applicationUninstallers.Add(entry);

                itemId++;
                callback(new GetUninstallerListProgress(features.Count) {CurrentCount = itemId, FinishedEntry = entry});
            }

            return applicationUninstallers;
        }

        /// <summary>
        ///     Rename the uninstaller entry by changing registry data. The entry is not refreshed in the process.
        /// </summary>
        public static bool Rename(this ApplicationUninstallerEntry entry, string newName)
        {
            if (string.IsNullOrEmpty(newName) || newName.ContainsAny(StringTools.InvalidPathChars))
                return false;

            using (var key = entry.OpenRegKey(true))
            {
                key.SetValue(ApplicationUninstallerEntry.RegistryNameDisplayName, newName);
            }
            return true;
        }

        /// <summary>
        ///     Uninstall multiple items in sequence. Items are uninstalled in order specified by the configuration.
        ///     This is a non-blocking method, a controller object is returned for monitoring of the task.
        ///     The task waits until an uninstaller fully exits before running the next one.
        /// </summary>
        /// <param name="targets">Uninstallers to run.</param>
        /// <param name="configuration">How the uninstallers should be ran.</param>
        public static BulkUninstallTask RunBulkUninstall(IEnumerable<ApplicationUninstallerEntry> targets,
            BulkUninstallConfiguration configuration)
        {
            var targetList = new List<BulkUninstallEntry>();

            foreach (var target in targets)
            {
                var tempStatus = UninstallStatus.Waiting;
                if (!target.IsValid)
                    tempStatus = UninstallStatus.Invalid;
                else if (!configuration.IgnoreProtection && target.IsProtected)
                    tempStatus = UninstallStatus.Protected;

                var silentPossible = configuration.PreferQuiet && target.QuietUninstallPossible;

                targetList.Add(new BulkUninstallEntry(target, silentPossible, tempStatus));
            }

            var query = from item in targetList
                orderby item.IsSilent ascending,
                    item.UninstallerEntry.IsUpdate ascending,
                    item.UninstallerEntry.SystemComponent ascending,
                    item.UninstallerEntry.IsProtected ascending,
                    item.UninstallerEntry.EstimatedSize.GetRawSize(true) descending
                select item;

            targetList = configuration.IntelligentSort
                ? query.ToList()
                : targetList.OrderBy(x => x.UninstallerEntry.DisplayName).ToList();

            return StartNewUninstallerThread(targetList, configuration);
        }

        /// <summary>
        ///     Start the default uninstaller with normal UI
        /// </summary>
        /// <exception cref="IOException">Uninstaller returned error code.</exception>
        /// <exception cref="InvalidOperationException">There are no usable ways of uninstalling this entry </exception>
        /// <exception cref="FormatException">Exception while decoding or attempting to run the uninstaller command. </exception>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static Process RunUninstaller(this ApplicationUninstallerEntry entry)
        {
            return RunUninstaller(entry, false, false);
        }

        /// <summary>
        ///     Start selected uninstaller type. If selected type is not available, fall back to the default.
        /// </summary>
        /// <param name="entry">Application to uninstall</param>
        /// <param name="silentIfAvailable">Choose quiet uninstaller if it's available.</param>
        /// <param name="simulate">If true, nothing will actually be uninstalled</param>
        /// <exception cref="IOException">Uninstaller returned error code.</exception>
        /// <exception cref="InvalidOperationException">There are no usable ways of uninstalling this entry </exception>
        /// <exception cref="FormatException">Exception while decoding or attempting to run the uninstaller command. </exception>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static Process RunUninstaller(this ApplicationUninstallerEntry entry, bool silentIfAvailable,
            bool simulate)
        {
            try
            {
                ProcessStartInfo startInfo = null;
                string fallBack = null;

                if (silentIfAvailable && entry.QuietUninstallPossible)
                {
                    // Use supplied quiet uninstaller if any
                    try
                    {
                        startInfo =
                            ProcessTools.SeparateArgsFromCommand(entry.QuietUninstallString).ToProcessStartInfo();
                    }
                    catch (FormatException)
                    {
                        fallBack = entry.QuietUninstallString;
                    }
                }
                else if (entry.UninstallPossible)
                {
                    // Fall back to the non-quiet uninstaller
                    try
                    {
                        startInfo = ProcessTools.SeparateArgsFromCommand(entry.UninstallString).ToProcessStartInfo();
                    }
                    catch (FormatException)
                    {
                        fallBack = entry.UninstallString;
                    }
                }
                else
                {
                    // Cant do shit, capt'n
                    throw new InvalidOperationException(Localisation.UninstallError_Nowaytouninstall);
                }

                if (simulate)
                {
                    Thread.Sleep(1000);
                    return null;
                }

                if (fallBack != null)
                    return Process.Start(fallBack);

                if (startInfo != null)
                {
                    startInfo.UseShellExecute = true;
                    return Process.Start(startInfo);
                }

                // Cant do shit, capt'n
                throw new InvalidOperationException(Localisation.UninstallError_Nowaytouninstall);
            }
            catch (IOException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FormatException(ex.Message, ex);
            }
        }

        /// <summary>
        ///     Uninstall using msiexec in selected mode. If no guid is present nothing is done and -1 is returned.
        /// </summary>
        /// <param name="entry">Application to uninstall</param>
        /// <param name="mode">Mode of the MsiExec run.</param>
        /// <param name="simulate">If true, nothing will be actually uninstalled</param>
        /// <exception cref="IOException">Uninstaller returned error code.</exception>
        /// <exception cref="InvalidOperationException">There are no usable ways of uninstalling this entry </exception>
        /// <exception cref="FormatException">Exception while decoding or attempting to run the uninstaller command. </exception>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static int UninstallUsingMsi(this ApplicationUninstallerEntry entry, MsiUninstallModes mode,
            bool simulate)
        {
            try
            {
                var uninstallPath = GetMsiString(entry.BundleProviderKey, mode);
                if (string.IsNullOrEmpty(uninstallPath))
                    return -1;

                var startInfo = ProcessTools.SeparateArgsFromCommand(uninstallPath).ToProcessStartInfo();
                startInfo.UseShellExecute = false;
                if (simulate)
                {
                    Thread.Sleep(1000);
                    return 0;
                }
                return startInfo.StartAndWait();
            }
            catch (IOException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FormatException(ex.Message, ex);
            }
        }

        internal static string GetMsiString(Guid bundleProviderKey, MsiUninstallModes mode)
        {
            if (bundleProviderKey == Guid.Empty) return string.Empty;

            switch (mode)
            {
                case MsiUninstallModes.InstallModify:
                    return $@"MsiExec.exe /I{bundleProviderKey:B}";

                case MsiUninstallModes.QuietUninstall:
                    return $@"MsiExec.exe /qb /X{bundleProviderKey:B} REBOOT=ReallySuppress /norestart";

                case MsiUninstallModes.Uninstall:
                    return $@"MsiExec.exe /X{bundleProviderKey:B}";

                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unknown mode");
            }
        }

        internal static void PopulateWindowsInstallerValidGuids()
        {
            WindowsInstallerValidGuids = MsiTools.MsiEnumProducts();
        }

        private static List<KeyValuePair<RegistryKey, bool>> GetRegistryKeys()
        {
            var keysToCheck = new List<KeyValuePair<RegistryKey, bool>>();

            var hklm = Registry.LocalMachine;
            var hkcu = Registry.CurrentUser;

            if (ProcessTools.Is64BitProcess)
            {
                keysToCheck.Add(new KeyValuePair<RegistryKey, bool>(hklm.OpenSubKey(RegUninstallersKeyDirect), true));
                keysToCheck.Add(new KeyValuePair<RegistryKey, bool>(hkcu.OpenSubKey(RegUninstallersKeyDirect), true));

                keysToCheck.Add(new KeyValuePair<RegistryKey, bool>(hklm.OpenSubKey(RegUninstallersKeyWow), false));
                keysToCheck.Add(new KeyValuePair<RegistryKey, bool>(hkcu.OpenSubKey(RegUninstallersKeyWow), false));
            }
            else
            {
                keysToCheck.Add(new KeyValuePair<RegistryKey, bool>(hklm.OpenSubKey(RegUninstallersKeyDirect), false));
                keysToCheck.Add(new KeyValuePair<RegistryKey, bool>(hkcu.OpenSubKey(RegUninstallersKeyDirect), false));
            }
            return keysToCheck;
        }

        private static BulkUninstallTask StartNewUninstallerThread(IList<BulkUninstallEntry> targetList,
            BulkUninstallConfiguration configuration)
        {
            var returnStatus = new BulkUninstallTask(targetList, configuration);
            var worker = new Thread(UninstallWorkerThread) {Name = "RunBulkUninstall_Worker"};
            worker.Start(returnStatus);
            return returnStatus;
        }

        private static void UninstallWorkerThread(object status)
        {
            var returnStatus = status as BulkUninstallTask;
            if (returnStatus == null)
                throw new ArgumentNullException(nameof(status));

            var targetList = returnStatus.AllUninstallersList;
            var configuration = returnStatus.Configuration;
            if (targetList == null || configuration == null)
                throw new ArgumentException("BulkUninstallTask is incomplete, this should not have happened.");

            for (var i = 0; i < targetList.Count; i++)
            {
                //returnStatus.FireOnStatusChanged();

                var currentUninstaller = targetList[i];
                returnStatus.CurrentUninstallerStatus = currentUninstaller;
                returnStatus.CurrentTask = i + 1;

                if (currentUninstaller.CurrentStatus == UninstallStatus.Invalid
                    || currentUninstaller.CurrentStatus == UninstallStatus.Protected)
                {
                    continue;
                }

                if (currentUninstaller.UninstallerEntry.IsRegistered &&
                    !currentUninstaller.UninstallerEntry.RegKeyStillExists())
                {
                    currentUninstaller.CurrentStatus = UninstallStatus.Completed;
                    continue;
                }

                if (returnStatus.Aborted)
                {
                    currentUninstaller.CurrentStatus = UninstallStatus.Skipped;
                    continue;
                }

                currentUninstaller.CurrentStatus = UninstallStatus.Uninstalling;
                // Fire the event now so the interface can be updated to show the "Uninstalling" tag
                returnStatus.FireOnStatusChanged();

                var result = ProcessUninstaller(currentUninstaller, configuration, ref returnStatus.SkipCurrent);

                // Take care of the aftermath
                if (returnStatus.SkipCurrent != BulkUninstallTask.SkipCurrentLevel.None)
                {
                    returnStatus.SkipCurrent = BulkUninstallTask.SkipCurrentLevel.None;

                    currentUninstaller.CurrentStatus = UninstallStatus.Skipped;
                    currentUninstaller.CurrentError = new OperationCanceledException(Localisation.ManagerError_Skipped);
                }
                else if (result != null)
                {
                    //Localisation.ManagerError_PrematureWorkerStop is unused
                    currentUninstaller.CurrentStatus = UninstallStatus.Failed;
                    currentUninstaller.CurrentError = result;
                }
                else
                {
                    currentUninstaller.CurrentStatus = UninstallStatus.Completed;
                }
            }
            returnStatus.Finished = true;
            returnStatus.Dispose();
        }

        /// <summary>
        ///     Run the uninstaller and wait for it to finish. Optionally terminate it or skip waiting for it. Returns
        ///     exception/error if any.
        /// </summary>
        /// <param name="currentUninstaller">Uninstaller to uninstall</param>
        /// <param name="configuration">Config to use</param>
        /// <param name="skipLevel">Reference to variable changed when user wants to terminate or skip</param>
        private static Exception ProcessUninstaller(BulkUninstallEntry currentUninstaller,
            BulkUninstallConfiguration configuration, ref BulkUninstallTask.SkipCurrentLevel skipLevel)
        {
            try
            {
                var uninstaller = currentUninstaller.UninstallerEntry.RunUninstaller(
                    configuration.PreferQuiet, configuration.Simulate);

                // Can be null during simulation
                if (uninstaller == null) return null;

                var checkCounters = configuration.PreferQuiet &&
                                    currentUninstaller.UninstallerEntry.QuietUninstallPossible;
                List<Process> childProcesses;
                var idleCounter = 0;

                do
                {
                    if (skipLevel == BulkUninstallTask.SkipCurrentLevel.Skip)
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

                    if (counters != null)
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
                    if (skipLevel == BulkUninstallTask.SkipCurrentLevel.Terminate || idleCounter > 15)
                    {
                        uninstaller.Kill(true);
                        if (idleCounter > 15)
                            throw new IOException(Localisation.UninstallError_UninstallerTimedOut);
                        break;
                    }
                } while (!uninstaller.HasExited || childProcesses.Any());

                if (skipLevel == BulkUninstallTask.SkipCurrentLevel.None)
                {
                    var exitVar = uninstaller.ExitCode;
                    if (exitVar != 0)
                        throw new IOException(Localisation.UninstallError_UninstallerReturnedCode + exitVar);
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
            return null;
        }

        public class GetUninstallerListProgress
        {
            internal GetUninstallerListProgress(int totalCount)
            {
                TotalCount = totalCount;
                CurrentCount = 0;
            }

            public int CurrentCount { get; internal set; }
            public ApplicationUninstallerEntry FinishedEntry { get; internal set; }
            public int TotalCount { get; internal set; }
        }
    }
}