using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BulkCrapUninstaller.Forms;
using BulkCrapUninstaller.Properties;
using Klocman.Extensions;
using Klocman.Forms;
using Klocman.Forms.Tools;
using Klocman.Native;
using Klocman.Tools;
using UninstallTools.Junk;
using UninstallTools.Uninstaller;

namespace BulkCrapUninstaller.Functions
{
    internal class Uninstaller
    {
        private readonly Action _initiateListRefresh;
        private readonly Action<bool> _lockApplication;
        private readonly Settings _settings = Settings.Default;
        private readonly object _uninstallLock = new object();

        /// <summary>
        ///     Uninstall tasks will wait until this is released to continue. Keep it short to prevent ui unresponsiveness.
        /// </summary>
        public readonly object PublicUninstallLock = new object();

        /// <exception cref="ArgumentNullException"> One of arguments is <see langword="null" />.</exception>
        internal Uninstaller(Action listRefreshCallback, Action<bool> applicationLockCallback)
        {
            if (listRefreshCallback == null) throw new ArgumentNullException(nameof(listRefreshCallback));
            if (applicationLockCallback == null) throw new ArgumentNullException(nameof(applicationLockCallback));

            _initiateListRefresh = listRefreshCallback;
            _lockApplication = applicationLockCallback;
        }

        /// <summary>
        ///     Returns false if export failed, else true.
        /// </summary>
        /// <param name="itemsToExport">What to export</param>
        /// <param name="filename">Full path with filename and extension to write the export result to.</param>
        /// <returns></returns>
        public bool ExportUninstallers(IEnumerable<ApplicationUninstallerEntry> itemsToExport, string filename)
        {
            var applicationUninstallerEntries = itemsToExport as List<ApplicationUninstallerEntry> ??
                                                itemsToExport.ToList();
            if (applicationUninstallerEntries.Count <= 0)
                return false;

            /*
            var result = new StringBuilder();
            result.AppendLine(Localisable.StrExportHeader);
            result.AppendLine();

            foreach (var uninstaller in applicationUninstallerEntries)
                result.AppendLine(uninstaller.ToLongString());
                */
            try
            {
                UninstallTools.Uninstaller.ApplicationInfoExport.SerializeApplicationInfo(filename, applicationUninstallerEntries);
                //File.WriteAllText(filename, result.ToString());
            }
            catch (Exception ex)
            {
                MessageBoxes.ExportFailed(ex.Message);
                return false;
            }
            return true;
        }

        private static bool CheckForRunningProcessesBeforeUninstall(IEnumerable<ApplicationUninstallerEntry> entries)
        {
            var filters = entries.SelectMany(e => new[] {e.InstallLocation, e.UninstallerLocation})
                .Where(s => !string.IsNullOrEmpty(s)).Distinct().ToArray();

            return CheckForRunningProcesses(filters);
        }

        private static bool CheckForRunningProcessesBeforeCleanup(IEnumerable<JunkNode> entries)
        {
            var filters = entries
                .Where(e => !(e is RegistryJunkNode))
                .Select(x => x.FullName)
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct().ToArray();

            return CheckForRunningProcesses(filters);
        }

        public static IEnumerable<ApplicationUninstallerEntry> GetApplicationsFromProcess(
            IEnumerable<ApplicationUninstallerEntry> allApplications, Process targetProcess)
        {
            var mainFilename = targetProcess.MainModule.FileName;
            return from app in allApplications
                where
                    (app.IsInstallLocationValid() &&
                     mainFilename.Contains(app.InstallLocation, StringComparison.InvariantCultureIgnoreCase))
                    ||
                    (!string.IsNullOrEmpty(app.UninstallerLocation) &&
                     mainFilename.Contains(app.UninstallerLocation, StringComparison.InvariantCultureIgnoreCase))
                select app;
        }

        private static bool CheckForRunningProcesses(string[] filters)
        {
            var myId = Process.GetCurrentProcess().Id;
            var idsToCheck = new List<int>();
            foreach (var pr in Process.GetProcesses())
            {
                try
                {
                    if (pr.Id == myId)
                        continue;

                    if (string.IsNullOrEmpty(pr.MainModule.FileName) ||
                        pr.MainModule.FileName.StartsWith(WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_SYSTEM)))
                        continue;

                    var filenames = pr.Modules.Cast<ProcessModule>()
                        .Select(x => x.FileName)
                        .Where(s => !string.IsNullOrEmpty(s))
                        .Distinct();

                    if (filenames.Any(filename => filters.Any(filter =>
                    {
                        if (string.IsNullOrEmpty(filename))
                            return false;

                        if (!Path.IsPathRooted(filename))
                            return false;

                        return filename.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase);
                    })))
                    {
                        idsToCheck.Add(pr.Id);
                    }
                }
                catch
                {
                    // Ignore invalid processes
                }
            }

            if (idsToCheck.Count > 0)
            {
                if (!ProcessWaiter.ShowDialog(MessageBoxes.DefaultOwner, idsToCheck.ToArray(), false))
                    return false;
            }

            return true;
        }

        public void RunUninstall(IEnumerable<ApplicationUninstallerEntry> selectedUninstallers,
            IEnumerable<ApplicationUninstallerEntry> allUninstallers, bool quiet)
        {
            if (!TryGetUninstallLock()) return;
            var listRefreshNeeded = false;

            try
            {
                var targets = new List<ApplicationUninstallerEntry>(selectedUninstallers);

                if (!_settings.AdvancedDisableProtection)
                {
                    var protectedTargets = targets.Where(x => x.IsProtected).ToList();
                    if (
                        MessageBoxes.ProtectedItemsWarningQuestion(protectedTargets.Select(x => x.DisplayName).ToArray()) ==
                        MessageBoxes.PressedButton.Cancel)
                        return;

                    targets.RemoveAll(protectedTargets);
                }

                if (targets.Any())
                {
                    _lockApplication(true);

                    if (!CheckForRunningProcessesBeforeUninstall(targets))
                        return;

                    if (!SystemRestore.BeginSysRestore(targets.Count))
                        return;

                    // No turning back at this point (kind of)
                    listRefreshNeeded = true;

                    if (_settings.ExternalEnable && _settings.ExternalPreCommands.IsNotEmpty())
                    {
                        LoadingDialog.ShowDialog(Localisable.LoadingDialogTitlePreUninstallCommands,
                            controller => { RunExternalCommands(_settings.ExternalPreCommands, controller); });
                    }

                    var status = ApplicationUninstallerManager.RunBulkUninstall(targets, GetConfiguration(quiet));
                    status.OneLoudLimit = _settings.UninstallConcurrentOneLoud;
                    status.ConcurrentUninstallerCount = _settings.UninstallConcurrency
                        ? _settings.UninstallConcurrentMaxCount
                        : 1;
                    status.Start();

                    using (var uninstallWindow = new UninstallProgressWindow())
                    {
                        uninstallWindow.SetTargetStatus(status);
                        uninstallWindow.ShowDialog();
                    }

                    var junkRemoveTargetsQuery = from bulkUninstallEntry in status.AllUninstallersList
                        where bulkUninstallEntry.CurrentStatus == UninstallStatus.Completed
                              || bulkUninstallEntry.CurrentStatus == UninstallStatus.Invalid
                              || (bulkUninstallEntry.CurrentStatus == UninstallStatus.Skipped
                                  && !bulkUninstallEntry.UninstallerEntry.RegKeyStillExists())
                        select bulkUninstallEntry.UninstallerEntry;

                    SearchForAndRemoveJunk(junkRemoveTargetsQuery, allUninstallers);

                    if (_settings.ExternalEnable && _settings.ExternalPostCommands.IsNotEmpty())
                    {
                        LoadingDialog.ShowDialog(Localisable.LoadingDialogTitlePostUninstallCommands,
                            controller => { RunExternalCommands(_settings.ExternalPostCommands, controller); });
                    }

                    SystemRestore.EndSysRestore();
                }
                else
                {
                    MessageBoxes.NoUninstallersSelectedInfo();
                }
            }
            finally
            {
                ReleaseUninstallLock();
                _lockApplication(false);
                if (listRefreshNeeded)
                    _initiateListRefresh();
            }
        }

        private void SearchForAndRemoveJunk(IEnumerable<ApplicationUninstallerEntry> selectedUninstallers,
            IEnumerable<ApplicationUninstallerEntry> allUninstallers)
        {
            //if (!TryGetUninstallLock()) return;
            //var listRefreshNeeded = false;

            if (MessageBoxes.LookForJunkQuestion())
            {
                SearchForAndRemoveJunk(
                    () => JunkManager.FindJunk(selectedUninstallers, allUninstallers.Where(y => y.RegKeyStillExists())));
            }
        }

        public void AdvancedUninstall(IEnumerable<ApplicationUninstallerEntry> selectedUninstallers,
            IEnumerable<ApplicationUninstallerEntry> allUninstallers)
        {
            if (!TryGetUninstallLock()) return;
            var listRefreshNeeded = false;

            try
            {
                _lockApplication(true);

                listRefreshNeeded = SearchForAndRemoveJunk(
                    () => JunkManager.FindJunk(selectedUninstallers, allUninstallers.Where(y => y.RegKeyStillExists())));
            }
            finally
            {
                ReleaseUninstallLock();
                _lockApplication(false);
                if (listRefreshNeeded)
                    _initiateListRefresh();
            }
        }

        /// <summary>
        ///     Returns true if things were actually removed, false if user cancelled the operation.
        /// </summary>
        /// <param name="junkGetter">
        ///     Delegate that returns junk items to remove.
        ///     It will be ran on a separate thread with a progress bar.
        /// </param>
        /// <returns></returns>
        private bool SearchForAndRemoveJunk(Func<IEnumerable<JunkNode>> junkGetter)
        {
            var junk = new List<JunkNode>();
            var error = LoadingDialog.ShowDialog(Localisable.LoadingDialogTitleLookingForJunk,
                x => { junk.AddRange(junkGetter()); });

            if (error != null)
            {
                PremadeDialogs.GenericError(error);
            }
            else if (junk.Any(x => x.Confidence.GetRawConfidence() >= 0))
            {
                using (var junkWindow = new JunkRemoveWindow(junk))
                {
                    if (junkWindow.ShowDialog() != DialogResult.OK) return false;

                    var selectedJunk = junkWindow.SelectedJunk.ToList();

                    if (!CheckForRunningProcessesBeforeCleanup(selectedJunk)) return false;

                    //Removing the junk
                    LoadingDialog.ShowDialog(Localisable.LoadingDialogTitleRemovingJunk, controller =>
                    {
                        var top = selectedJunk.Count;
                        controller.SetMaximum(top);
                        var itemsRemoved = 0; // current value
                        foreach (var junkNode in selectedJunk)
                        {
                            controller.SetProgress(itemsRemoved++);

                            if (_settings.AdvancedSimulate)
                            {
                                Thread.Sleep(100);
                            }
                            else
                            {
                                try
                                {
                                    junkNode.Delete();
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine("Exception while removing junk: " + ex.ToString());
                                }
                            }
                        }
                    });

                    return true;
                }
            }
            else
            {
                MessageBoxes.NoJunkFoundInfo();
            }
            return false;
        }

        public void SearchForAndRemoveProgramFilesJunk(IEnumerable<ApplicationUninstallerEntry> allUninstallers)
        {
            if (!TryGetUninstallLock()) return;

            try
            {
                _lockApplication(true);

                SearchForAndRemoveJunk(() => JunkManager.FindProgramFilesJunk(allUninstallers
                    .Where(y => y.RegKeyStillExists())));
            }
            finally
            {
                ReleaseUninstallLock();
                _lockApplication(false);
            }
        }

        public void UninstallUsingMsi(MsiUninstallModes mode,
            IEnumerable<ApplicationUninstallerEntry> selectedUninstallers)
        {
            if (!TryGetUninstallLock()) return;
            var listRefreshNeeded = false;

            try
            {
                _lockApplication(true);

                var results = selectedUninstallers.Take(2).ToList();

                if (results.Count != 1)
                {
                    MessageBoxes.CanSelectOnlyOneItemInfo();
                    return;
                }

                var selected = results.First();

                if (!_settings.AdvancedDisableProtection && selected.IsProtected)
                {
                    MessageBoxes.ProtectedItemError(selected.DisplayName);
                    return;
                }

                if (selected.BundleProviderKey.IsEmpty())
                {
                    MessageBoxes.UninstallMsiGuidMissing();
                    return;
                }

                if (!CheckForRunningProcessesBeforeUninstall(new[] {selected}))
                    return;

                try
                {
                    selected.UninstallUsingMsi(mode, _settings.AdvancedSimulate);
                    listRefreshNeeded = true;
                }
                catch (Exception ex)
                {
                    PremadeDialogs.GenericError(ex);
                }
            }
            finally
            {
                ReleaseUninstallLock();
                _lockApplication(false);
                if (listRefreshNeeded)
                    _initiateListRefresh();
            }
        }

        /// <summary>
        ///     Ask to self uninstall and do so if user agrees, else return and do nothing.
        /// </summary>
        internal void AskToSelfUninstall()
        {
            if (MessageBoxes.SelfUninstallQuestion())
            {
                if (!TryGetUninstallLock()) return;

                try
                {
                    Process.Start("unins000.exe");
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    PremadeDialogs.GenericError(ex);
                }
                finally
                {
                    ReleaseUninstallLock();
                }
            }
        }

        private BulkUninstallConfiguration GetConfiguration(bool quiet)
        {
            return new BulkUninstallConfiguration(_settings.AdvancedDisableProtection, quiet,
                _settings.AdvancedIntelligentUninstallerSorting, _settings.AdvancedSimulate,
                _settings.QuietAutoKillStuck, _settings.QuietRetryFailedOnce);
        }

        private void ReleaseUninstallLock()
        {
            Monitor.Exit(PublicUninstallLock);
            Monitor.Exit(_uninstallLock);
        }

        private static void RunExternalCommands(string commands, LoadingDialog.LoadingDialogInterface controller)
        {
            var lines = commands.SplitNewlines(StringSplitOptions.RemoveEmptyEntries);
            controller.SetMaximum(lines.Length);

            for (var i = 0; i < lines.Length; i++)
            {
                controller.SetProgress(i);

                var line = lines[i];
                try
                {
                    var filename = ProcessTools.SeparateArgsFromCommand(line);
                    filename.FileName = Path.GetFullPath(filename.FileName);
                    if (!File.Exists(filename.FileName))
                        throw new IOException(Localisable.Error_FileNotFound);
                    filename.ToProcessStartInfo().StartAndWait();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(Localisable.MessageBoxes_ExternalCommandFailed_Message, line)
                                    + Localisable.MessageBoxes_Error_details + ex.Message,
                        Localisable.MessageBoxes_ExternalCommandFailed_Title,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        ///     Attempt to get the lock and display error popup if the process fails.
        /// </summary>
        /// <returns>True if lock was succesfully acquired, otherwise false.</returns>
        private bool TryGetUninstallLock()
        {
            if (Monitor.TryEnter(_uninstallLock))
            {
                Monitor.Enter(PublicUninstallLock);
                return true;
            }

            MessageBoxes.UninstallAlreadyRunning();
            return false;
        }
    }
}