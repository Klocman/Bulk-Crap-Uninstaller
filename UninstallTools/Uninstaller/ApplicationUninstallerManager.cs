using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using Klocman.Extensions;
using Klocman.IO;
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

            var pfDirectories = UninstallToolsGlobalConfig.GetProgramFilesDirectories(true).ToList();
            
            // Get directories which are already used and should be skipped
            var directoriesToSkip = existingUninstallers.SelectMany(x =>
            {
                if (!string.IsNullOrEmpty(x.DisplayIcon))
                {
                    try
                    {
                        var iconFilename = x.DisplayIcon.Contains('.')
                            ? ProcessTools.SeparateArgsFromCommand(x.DisplayIcon).FileName
                            : x.DisplayIcon;

                        return new[] { x.InstallLocation, x.UninstallerLocation, PathTools.GetDirectory(iconFilename) };
                    }
                    catch
                    {
                        // Ignore invalid DisplayIcon paths
                    }
                }
                return new[] { x.InstallLocation, x.UninstallerLocation };
            }).Where(x => x.IsNotEmpty()).Select(PathTools.PathToNormalCase)
            .Where(x =>!pfDirectories.Any(pfd => pfd.Key.FullName.Contains(x, StringComparison.InvariantCultureIgnoreCase)))
            .Distinct().ToList();

            // Get sub directories which could contain user programs
            var directoriesToCheck = pfDirectories.Aggregate(Enumerable.Empty<KeyValuePair<DirectoryInfo, bool?>>(),
                (a, b) => a.Concat(b.Key.GetDirectories().Select(x => new KeyValuePair<DirectoryInfo, bool?>(x, b.Value))));

            // Get directories that can be relatively safely checked
            var inputs = directoriesToCheck.Where(x => !directoriesToSkip.Any(y =>
                x.Key.FullName.Contains(y, StringComparison.InvariantCultureIgnoreCase)
                || y.Contains(x.Key.FullName, StringComparison.InvariantCultureIgnoreCase))).ToList();

            var results = new List<ApplicationUninstallerEntry>();
            var itemId = 0;
            foreach (var directory in inputs)
            {
                itemId++;

                var progress = new GetUninstallerListProgress(inputs.Count) { CurrentCount = itemId };
                callback(progress);

                if (UninstallToolsGlobalConfig.IsSystemDirectory(directory.Key) ||
                    directory.Key.Name.StartsWith("Windows", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                //Try to get the main executable from the filtered folders. If no executables are present check subfolders.
                var detectedEntries = ApplicationUninstallerFactory.TryCreateFromDirectory(directory.Key,
                    directory.Value);

                results.AddRange(detectedEntries.Where(detected => !existingUninstallers.Any(existing =>
                {
                    if (!string.IsNullOrEmpty(existing.DisplayName) && !string.IsNullOrEmpty(detected.DisplayNameTrimmed) 
                    && existing.DisplayName.Contains(detected.DisplayNameTrimmed))
                    {
                        return !existing.IsInstallLocationValid() ||
                               detected.InstallLocation.Contains(existing.InstallLocation,
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
                    var progress = new GetUninstallerListProgress(uninstallersToCreate.Count) { CurrentCount = itemId };
                    callback(progress);
                }
            }

            applicationUninstallers.AddRange(ApplicationUninstallerFactory.GetStoreApps());

            if (ApplicationUninstallerFactory.SteamHelperIsAvailable)
            {
                var steamAppsOnDisk = ApplicationUninstallerFactory.GetSteamApps().ToList();

                foreach (var steamApp in applicationUninstallers.Where(x => x.UninstallerKind == UninstallerType.Steam))
                {
                    var toRemove = steamAppsOnDisk.FindAll(x => x.InstallLocation.Equals(steamApp.InstallLocation, StringComparison.InvariantCultureIgnoreCase));
                    steamAppsOnDisk.RemoveAll(toRemove);
                    ApplicationUninstallerFactory.ChangeSteamAppUninstallStringToHelper(steamApp);

                    if(steamApp.EstimatedSize.IsDefault() && toRemove.Any())
                        steamApp.EstimatedSize = toRemove.First().EstimatedSize;
                }

                foreach (var steamApp in steamAppsOnDisk)
                {
                    ApplicationUninstallerFactory.ChangeSteamAppUninstallStringToHelper(steamApp);
                }

                applicationUninstallers.AddRange(steamAppsOnDisk);
            }
            
            applicationUninstallers.AddRange(ApplicationUninstallerFactory.GetSpecialUninstallers(applicationUninstallers));

            // Fill in missing information
            foreach (var applicationUninstaller in applicationUninstallers)
            {
                if (applicationUninstaller.IconBitmap == null)
                {
                    string iconPath;
                    applicationUninstaller.IconBitmap = ApplicationUninstallerFactory.TryGetIcon(
                        applicationUninstaller, out iconPath);
                    applicationUninstaller.DisplayIcon = iconPath;
                }

                if (applicationUninstaller.InstallDate.IsDefault() &&
                    Directory.Exists(applicationUninstaller.InstallLocation))
                {
                    applicationUninstaller.InstallDate =
                        Directory.GetCreationTime(applicationUninstaller.InstallLocation);
                }
            }

            return applicationUninstallers;
        }

        public static IEnumerable<ApplicationUninstallerEntry> GetWindowsFeaturesList()
        {
            if (Environment.OSVersion.Version < WindowsTools.Windows7)
                return Enumerable.Empty<ApplicationUninstallerEntry>();

            Exception error = null;
            var applicationUninstallers = new List<ApplicationUninstallerEntry>();
            var t = new Thread(() =>
            {
                try
                {
                    applicationUninstallers.AddRange(WmiQueries.GetWindowsFeatures()
                        .Where(x => x.Enabled)
                        .Select(WindowsFeatureToUninstallerEntry));
                }
                catch(Exception ex)
                {
                    error = ex;
                }
            });
            t.Start();

            t.Join(TimeSpan.FromSeconds(30));

            if(error != null)
                throw new IOException("WMI query returned an error, try restarting your computer", error);
            if (t.IsAlive)
            {
                t.Abort();
                throw new TimeoutException("WMI query has hung, try restarting your computer");
            }

            return applicationUninstallers;
        }

        private static ApplicationUninstallerEntry WindowsFeatureToUninstallerEntry(WindowsFeatureInfo info)
        {
            return new ApplicationUninstallerEntry
            {
                RawDisplayName = info.DisplayName,
                Comment = info.Description,
                UninstallString = DismTools.GetDismUninstallString(info.FeatureName, false),
                QuietUninstallString = DismTools.GetDismUninstallString(info.FeatureName, true),
                UninstallerKind = UninstallerType.WindowsFeature,
                Publisher = "Microsoft Corporation",
                IsValid = true,
                Is64Bit = ProcessTools.Is64BitProcess ? MachineType.X64 : MachineType.X86
            };
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
                            // Updates usually get uninstalled by their parent uninstallers
                            item.UninstallerEntry.IsUpdate ascending,
                            // SysCmps and Protected usually get uninstalled by their parent, user-visible uninstallers
                            item.UninstallerEntry.SystemComponent ascending,
                            item.UninstallerEntry.IsProtected ascending,
                            // Calculate number of digits (Floor of Log10 + 1) and divide it by 4 to create buckets of sizes
                            Math.Round(Math.Floor(Math.Log10(item.UninstallerEntry.EstimatedSize.GetRawSize(true)) + 1) / 4) descending,
                            // Prioritize Msi uninstallers because they tend to take the longest
                            item.UninstallerEntry.UninstallerKind == UninstallerType.Msiexec descending,
                            // Final sorting to get things deterministic
                            item.UninstallerEntry.EstimatedSize.GetRawSize(true) descending
                        select item;

            targetList = configuration.IntelligentSort
                ? query.ToList()
                : targetList.OrderBy(x => x.UninstallerEntry.DisplayName).ToList();

            return new BulkUninstallTask(targetList, configuration);
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
                    Thread.Sleep(5000);
                    if (Debugger.IsAttached && new Random().Next(0, 2) == 0)
                        throw new IOException("Random failure for debugging");
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
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, @"Unknown mode");
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