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
        //private bool _abortSkipMessageboxThread;
        private Thread _boxThread;
        private BulkUninstallTask _currentTargetStatus;
        //private CustomMessageBox _skipMessageBox;
        //private Thread _skipMessageboxThread;
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
                    if (args.NewValue) ShutdownBlockReasonCreate(Handle, "Bulk uninstallation is in progress.");
                    else ShutdownBlockReasonDestroy(Handle);
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
                    ShutdownBlockReasonDestroy(Handle);
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

        [DllImport("user32.dll")]
        private static extern bool ShutdownBlockReasonCreate(IntPtr hWnd,
            [MarshalAs(UnmanagedType.LPWStr)] string pwszReason);

        [DllImport("user32.dll")]
        private static extern bool ShutdownBlockReasonDestroy(IntPtr hWnd);

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

        /*private void CloseSkipMessagebox()
        {
            if (_skipMessageboxThread != null && _skipMessageboxThread.IsAlive)
            {
                _abortSkipMessageboxThread = true;
                _skipMessageboxThread.Join();
                this.SafeInvoke(() => Enabled = true);
            }
        }*/

        private void currentTargetStatus_OnCurrentTaskChanged(object sender, EventArgs e)
        {
            //CloseSkipMessagebox();

            objectListView1.SafeInvoke(() =>
            {
                //objectListView1.RefreshObjects(_currentTargetStatus.AllUninstallersList.ToList());
                //objectListView1.RebuildColumns();

                objectListView1.SetObjects(_currentTargetStatus.AllUninstallersList, true);

                if (_currentTargetStatus.Finished)
                {
                    OnTaskFinished();
                }
                else
                {
                    OnTaskUpdated();
                }
            });
        }

        private void OnTaskFinished()
        {
            if (_boxThread != null)
            {
                if (_boxThread.IsAlive)
                    _boxThread.Abort();

                _boxThread = null;
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
                // Show the walk away box if there are no running/waiting loud uninstallers and at least one quiet unistaller running/waiting
                // TODO do it with less enumerations / simplify
                if (_walkAwayBox == null &&
                    // There is at least one loud uninstaller
                    _currentTargetStatus.AllUninstallersList.Any(x => !x.IsSilent) &&
                    //_currentTargetStatus.AllUninstallersList.Any(x => x.IsSilent) && // and one quiet uninstaller
                    // There are no loud uninstallers running or waiting
                    !_currentTargetStatus.AllUninstallersList.Any(x => !x.IsSilent &&
                                                                       (x.CurrentStatus == UninstallStatus.Waiting ||
                                                                        x.CurrentStatus == UninstallStatus.Uninstalling)) &&
                    // There is at least one silent uninstaller running or waiting
                    _currentTargetStatus.AllUninstallersList.Any(x => x.IsSilent &&
                                                                        (x.CurrentStatus == UninstallStatus.Waiting ||
                                                                        x.CurrentStatus == UninstallStatus.Uninstalling)))
                {
                    _walkAwayBox = MessageBoxes.CanWalkAwayInfo(this);

                    Enabled = false;
                    _walkAwayBox.FormClosed += (x1, y1) => Enabled = true;
                }

                buttonClose.Enabled = true;

                var progress = _currentTargetStatus.AllUninstallersList
                    .Count(x => x.CurrentStatus != UninstallStatus.Waiting);
                var statusString = string.Join("; ", _currentTargetStatus.AllUninstallersList
                    .Where(x1 => x1.CurrentStatus == UninstallStatus.Uninstalling)
                    .Select(x2 => x2.UninstallerEntry.DisplayName).ToArray());

                label1.Text = $"{Localisable.UninstallProgressWindow_Uninstalling} " +
                              $"{progress}" +
                              $"/{_currentTargetStatus.AllUninstallersList.Count}: {statusString}";

                //objectListView1.EnsureVisible(objectListView1.IndexOf(currentUninstallerStatus));

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

        /*private void OpenSkipMessagebox(object sender, EventArgs e)
        {
            CloseSkipMessagebox();

            var selected = SelectedTaskEntries.Where(x => !x.Finished).ToList();
            if (selected.Count == 0)
                return;

            _skipMessageBox = MessageBoxes.TaskSkipCurrentKillTaskQuestion(this);
            Enabled = false;
            _skipMessageBox.FormClosed += (x, y) => Enabled = true;

            _skipMessageboxThread = new Thread(SkipMesssageboxThread);
            _skipMessageboxThread.Start(selected);
        }

        private void SkipMesssageboxThread(object items)
        {
            var selected = items as IEnumerable<BulkUninstallEntry>;
            if (_skipMessageBox == null || selected == null)
                return;

            try
            {
                while (!_skipMessageBox.IsDisposed) // && box.Visible)
                {
                    if (_abortSkipMessageboxThread)
                    {
                        //box.Close(); No need, dispose is called
                        return;
                    }

                    Thread.Sleep(200);
                }

                switch (_skipMessageBox.Result)
                {
                    case CustomMessageBox.PressedButton.Left:
                        selected.ForEach(x => x.SkipWaiting(true));
                        break;
                    case CustomMessageBox.PressedButton.Middle:
                        selected.ForEach(x => x.SkipWaiting(false));
                        break;
                }
            }
            finally
            {
                _abortSkipMessageboxThread = false;
                this.SafeInvoke(() =>
                {
                    if (!_skipMessageBox.IsDisposed)
                        _skipMessageBox.Dispose();

                    currentTargetStatus_OnCurrentTaskChanged(this, EventArgs.Empty);
                });
            }
        }*/

        private void UninstallProgressWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_currentTargetStatus.Finished || _currentTargetStatus.Aborted ||
                e.CloseReason != CloseReason.UserClosing)
            {
                _currentTargetStatus.OnStatusChanged -= currentTargetStatus_OnCurrentTaskChanged;
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
            var failed =
                SelectedTaskEntries.Where(
                    x =>
                        !_currentTargetStatus.RunSingle(x,
                            _settings.Settings.UninstallConcurrentDisableManualCollisionProtection)).ToList();

            if (failed.Any())
                MessageBoxes.ForceRunUninstallFailedError(this,
                    failed.Select(x => x.UninstallerEntry.DisplayName).OrderBy(x => x));

            currentTargetStatus_OnCurrentTaskChanged(sender, e);
        }

        private void objectListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            toolStripButtonFolderOpen.Enabled = SelectedTaskEntries.Any(x => x.UninstallerEntry.InstallLocation.IsNotEmpty());
            toolStripButtonProperties.Enabled = SelectedTaskEntries.Any();

            toolStripButtonRun.Enabled = SelectedTaskEntries.Any(x => x.CurrentStatus == UninstallStatus.Waiting
                                                                      || x.CurrentStatus == UninstallStatus.Failed
                                                                      || x.CurrentStatus == UninstallStatus.Skipped);

            toolStripButtonSkip.Enabled = SelectedTaskEntries.Any(x => x.CurrentStatus != UninstallStatus.Skipped
                                                                       && x.CurrentStatus != UninstallStatus.Completed
                                                                       && x.CurrentStatus != UninstallStatus.Invalid
                                                                       && x.CurrentStatus != UninstallStatus.Protected
                                                                       && !(x.CurrentStatus == UninstallStatus.Uninstalling
                                                                        && x.UninstallerEntry.UninstallerKind == UninstallerType.Msiexec));

            toolStripButtonTerminate.Enabled = SelectedTaskEntries.Any(x => x.CurrentStatus == UninstallStatus.Uninstalling);
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
    }
}