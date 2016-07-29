using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions;
using BulkCrapUninstaller.Properties;
using Klocman.Binding.Settings;
using Klocman.Extensions;
using Klocman.Forms;
using Klocman.Forms.Tools;
using UninstallTools.Uninstaller;

namespace BulkCrapUninstaller.Forms
{
    internal sealed partial class UninstallProgressWindow : Form
    {
        private readonly SettingBinder<Settings> _settings = Settings.Default.SettingBinder;

        private BulkUninstallTask _currentTargetStatus;
        private CustomMessageBox _walkAwayBox;

        public UninstallProgressWindow()
        {
            InitializeComponent();

            Icon = Resources.Icon_Logo;

            toolStrip1.Renderer = new ToolStripProfessionalRenderer(new StandardSystemColorTable());

            // Shutdown blocking not available below Windows Vista
            if (Environment.OSVersion.Version >= new Version(6, 0))
            {
                _settings.Subscribe((sender, args) =>
                {
                    if (args.NewValue)
                        NativeMethods.ShutdownBlockReasonCreate(Handle, "Bulk uninstallation is in progress.");
                    else NativeMethods.ShutdownBlockReasonDestroy(Handle);
                }, settings => settings.UninstallPreventShutdown, this);
            }

            _settings.SendUpdates(this);

            FormClosing += (sender, args) =>
            {
                if (args.CloseReason == CloseReason.WindowsShutDown && _settings.Settings.UninstallPreventShutdown)
                    args.Cancel = true;
            };

            FormClosed += (o, eventArgs) =>
            {
                _settings.RemoveHandlers(this);

                // Shutdown blocking not available below Windows Vista
                if (_settings.Settings.UninstallPreventShutdown && Environment.OSVersion.Version >= new Version(6, 0))
                    NativeMethods.ShutdownBlockReasonDestroy(Handle);
            };

            olvColumnName.AspectGetter = BulkUninstallTask.DisplayNameAspectGetter;
            olvColumnStatus.AspectGetter = BulkUninstallTask.StatusAspectGetter;
            olvColumnIsSilent.AspectGetter = BulkUninstallTask.IsSilentAspectGetter;
            olvColumnId.AspectName = nameof(BulkUninstallEntry.Id);

            olvColumnStatus.GroupKeyGetter = rowObject => (rowObject as BulkUninstallEntry)?.CurrentStatus;
            objectListView1.PrimarySortColumn = olvColumnStatus;
            objectListView1.SecondarySortColumn = olvColumnId;
        }

        private IEnumerable<BulkUninstallEntry> SelectedTaskEntries
            => objectListView1.SelectedObjects.Cast<BulkUninstallEntry>();

        private IEnumerable<ApplicationUninstallerEntry> SelectedUninstallerEntries
            => SelectedTaskEntries.Select(x => x.UninstallerEntry);

        public void SetTargetStatus(BulkUninstallTask targetStatus)
        {
            if (targetStatus == null)
                throw new ArgumentNullException(nameof(targetStatus));

            _currentTargetStatus = targetStatus;

            progressBar1.Maximum = _currentTargetStatus.AllUninstallersList.Count;

            objectListView1.SetObjects(_currentTargetStatus.AllUninstallersList);
            _currentTargetStatus.OnStatusChanged += currentTargetStatus_OnCurrentTaskChanged;
            currentTargetStatus_OnCurrentTaskChanged(this, EventArgs.Empty);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void currentTargetStatus_OnCurrentTaskChanged(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                objectListView1.SafeInvoke(() =>
                {
                    objectListView1.SetObjects(_currentTargetStatus.AllUninstallersList, true);

                    if (_currentTargetStatus.Finished)
                        OnTaskFinished();
                    else
                        OnTaskUpdated();
                });
            }).Start();
        }

        private void OnTaskFinished()
        {
            if (_walkAwayBox != null && _walkAwayBox.Visible)
                _walkAwayBox.Close();

            if (!_currentTargetStatus.Aborted && _currentTargetStatus.Configuration.PreferQuiet)
            {
                var failedSilent = _currentTargetStatus.AllUninstallersList
                    .Where(x => x.CurrentStatus == UninstallStatus.Failed && x.IsSilent).ToList();
                if (failedSilent.Count > 0 && MessageBoxes.AskToRetryFailedQuietAsLoud(this, failedSilent.Select(x => x.UninstallerEntry.DisplayName)))
                {
                    foreach (var uninstallEntry in failedSilent)
                    {
                        uninstallEntry.Reset();
                        uninstallEntry.IsSilent = false;
                    }
                    objectListView1.UpdateObjects(failedSilent);
                    objectListView1.BuildGroups();
                    _currentTargetStatus.Start();
                    return;
                }
            }

            label1.Text = Localisable.UninstallProgressWindow_TaskDone;
            progressBar1.Value = progressBar1.Maximum;
            buttonClose.Text = Buttons.ButtonClose;
            buttonClose.Enabled = true;
        }

        private void OnTaskUpdated()
        {
            SuspendLayout();

            try
            {
                var uninstList = _currentTargetStatus.AllUninstallersList;
                // Show the walk away box if there are no running/waiting loud uninstallers and at least one quiet unistaller running/waiting
                if (_walkAwayBox == null &&
                    // There is at least one loud uninstaller
                    uninstList.Any(x => !x.IsSilent) &&
                    // There are no loud uninstallers running or waiting
                    !uninstList.Any(x => !x.IsSilent &&
                        (x.CurrentStatus == UninstallStatus.Waiting || x.CurrentStatus == UninstallStatus.Uninstalling)) &&
                    // There is at least one silent uninstaller running or waiting
                    uninstList.Any(x => x.IsSilent &&
                        (x.CurrentStatus == UninstallStatus.Waiting || x.CurrentStatus == UninstallStatus.Uninstalling)))
                {
                    _walkAwayBox = MessageBoxes.CanWalkAwayInfo(this);

                    Enabled = false;
                    _walkAwayBox.FormClosed += (x1, y1) => Enabled = true;
                }

                buttonClose.Enabled = true;

                var progress = uninstList.Count(x => x.CurrentStatus != UninstallStatus.Waiting);
                var statusString = string.Join("; ",
                    uninstList.Where(x1 => x1.CurrentStatus == UninstallStatus.Uninstalling)
                        .Select(x2 => x2.UninstallerEntry.DisplayName)
                        .ToArray());

                label1.Text = $"{Localisable.UninstallProgressWindow_Uninstalling} {progress}/{uninstList.Count}: {statusString}";

                buttonClose.Text = Buttons.ButtonCancel;
                progressBar1.Value = Math.Max(0, progress - 1);
                progressBar1.Style = ProgressBarStyle.Continuous;
            }
            catch
            {
                progressBar1.Style = ProgressBarStyle.Marquee;
            }

            ResumeLayout();
        }

        private void UninstallProgressWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_currentTargetStatus.Finished || _currentTargetStatus.Aborted ||
                e.CloseReason != CloseReason.UserClosing)
            {
                _currentTargetStatus.Dispose();
                return;
            }

            if (MessageBoxes.TaskStopConfirmation(this) == MessageBoxes.PressedButton.Yes)
            {
                _currentTargetStatus.Aborted = true;
                buttonClose.Enabled = false;
            }

            e.Cancel = true;
        }

        private void toolStripButtonSettings_Click(object sender, EventArgs e)
        {
            using (var sw = new SettingsWindow())
            {
                sw.OpenedTab = 1;
                sw.ShowDialog();
            }

            _currentTargetStatus.OneLoudLimit = _settings.Settings.UninstallConcurrentOneLoud;
            _currentTargetStatus.ConcurrentUninstallerCount = _settings.Settings.UninstallConcurrency
                ? _settings.Settings.UninstallConcurrentMaxCount
                : 1;
        }

        private void toolStripButtonProperties_Click(object sender, EventArgs e)
        {
            using (var propertiesWindow = new PropertiesWindow())
            {
                propertiesWindow.ShowPropertiesDialog(SelectedUninstallerEntries);
            }
        }

        private void toolStripButtonFolderOpen_Click(object sender, EventArgs e)
        {
            var sourceDirs = SelectedUninstallerEntries.Where(x => x.InstallLocation.IsNotEmpty())
                .Select(y => y.InstallLocation).ToList();

            if (MessageBoxes.OpenDirectoriesMessageBox(sourceDirs.Count))
            {
                try
                {
                    sourceDirs.ForEach(x => Process.Start(x));
                }
                catch (Exception ex)
                {
                    MessageBoxes.OpenDirectoryError(ex);
                }
            }
        }

        private void toolStripButtonRun_Click(object sender, EventArgs e)
        {
            var targetGroups = SelectedTaskEntries.GroupBy(x => x.IsRunning).ToList();
            var running = targetGroups.SingleOrDefault(x => x.Key);
            var notRunning = targetGroups.SingleOrDefault(x => !x.Key);

            if (running != null && running.Any())
                MessageBoxes.ForceRunUninstallFailedError(this,
                    running.Select(x => x.UninstallerEntry.DisplayName).OrderBy(x => x));

            if (notRunning == null || !notRunning.Any()) return;

            foreach (var target in notRunning)
            {
                target.Reset();
            }

            OnTaskUpdated();

            if (_currentTargetStatus.Finished)
                _currentTargetStatus.Start();
        }

        private void objectListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ste = SelectedTaskEntries.ToList();

            toolStripButtonFolderOpen.Enabled = ste.Any(x => x.UninstallerEntry.InstallLocation.IsNotEmpty());
            toolStripButtonProperties.Enabled = ste.Any();

            toolStripButtonRun.Enabled = ste.Any(x =>
                x.CurrentStatus == UninstallStatus.Waiting || x.CurrentStatus == UninstallStatus.Failed ||
                x.CurrentStatus == UninstallStatus.Skipped);

            toolStripButtonSkip.Enabled = ste.Any(x =>
                x.CurrentStatus != UninstallStatus.Skipped && x.CurrentStatus != UninstallStatus.Completed &&
                x.CurrentStatus != UninstallStatus.Invalid && x.CurrentStatus != UninstallStatus.Protected &&
                !(x.CurrentStatus == UninstallStatus.Uninstalling &&
                  x.UninstallerEntry.UninstallerKind == UninstallerType.Msiexec));

            toolStripButtonTerminate.Enabled = ste.Any(x => x.CurrentStatus == UninstallStatus.Uninstalling);
        }

        private void toolStripButtonTerminate_Click(object sender, EventArgs e)
        {
            SelectedTaskEntries.ForEach(x => x.SkipWaiting(true));
        }

        private void toolStripButtonSkip_Click(object sender, EventArgs e)
        {
            SelectedTaskEntries.ForEach(x => x.SkipWaiting(false));
        }

        private void toolStripButtonHelp_Click(object sender, EventArgs e)
        {
            MessageBoxes.DisplayHelp(this);
        }

        private static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern bool ShutdownBlockReasonCreate(IntPtr hWnd,
                [MarshalAs(UnmanagedType.LPWStr)] string pwszReason);

            [DllImport("user32.dll")]
            public static extern bool ShutdownBlockReasonDestroy(IntPtr hWnd);
        }
    }
}