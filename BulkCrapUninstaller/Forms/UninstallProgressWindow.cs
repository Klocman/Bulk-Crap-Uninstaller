using System;
using System.Threading;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions;
using BulkCrapUninstaller.Properties;
using Klocman.Extensions;
using Klocman.Forms;
using Klocman.Forms.Tools;
using UninstallTools;
using UninstallTools.Uninstaller;

namespace BulkCrapUninstaller.Forms
{
    sealed partial class UninstallProgressWindow : Form
    {
        private Thread _boxThread;
        private BulkUninstallTask _currentTargetStatus;
        private bool _lastWasQuiet;
        private bool _abortSkipMessageboxThread;
        private CustomMessageBox _skipMessageBox;
        private Thread _skipMessageboxThread;
        private CustomMessageBox _walkAwayBox;

        public UninstallProgressWindow()
        {
            InitializeComponent();

            Icon = Resources.Icon_Logo;

            olvColumnName.AspectGetter = BulkUninstallTask.DisplayNameAspectGetter;
            olvColumnStatus.AspectGetter = BulkUninstallTask.StatusAspectGetter;
            olvColumnIsSilent.AspectGetter = BulkUninstallTask.IsSilentAspectGetter;
        }

        /// <exception cref="ArgumentNullException">The value of 'targetStatus' cannot be null. </exception>
        public void SetTargetStatus(BulkUninstallTask targetStatus)
        {
            if (targetStatus == null)
                throw new ArgumentNullException(nameof(targetStatus));

            _currentTargetStatus = targetStatus;

            _lastWasQuiet = _currentTargetStatus.CurrentUninstallerStatus.IsSilent;
            progressBar1.Maximum = _currentTargetStatus.TaskCount;

            objectListView1.SetObjects(_currentTargetStatus.AllUninstallEntries);
            _currentTargetStatus.OnStatusChanged += currentTargetStatus_OnCurrentTaskChanged;
            currentTargetStatus_OnCurrentTaskChanged(this, EventArgs.Empty);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CloseSkipMessagebox()
        {
            if (_skipMessageboxThread != null && _skipMessageboxThread.IsAlive)
            {
                _abortSkipMessageboxThread = true;
                while (_abortSkipMessageboxThread)
                    Thread.Sleep(200);
            }
        }

        private void currentTargetStatus_OnCurrentTaskChanged(object sender, EventArgs e)
        {
            CloseSkipMessagebox();

            objectListView1.SafeInvoke(() =>
            {
                objectListView1.SetObjects(_currentTargetStatus.AllUninstallEntries, true);
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

            buttonSkip.Enabled = false;
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
                if (!_lastWasQuiet && _currentTargetStatus.Configuration.QuietAreSorted &&
                    (_currentTargetStatus.CurrentUninstallerStatus.IsSilent))
                {
                    _lastWasQuiet = true;
                    OpenWalkAwayMessagebox();
                }

                buttonSkip.Enabled = true;
                buttonClose.Enabled = true;

                var currentUninstallerStatus = _currentTargetStatus.CurrentUninstallerStatus;
                label1.Text = string.Format("{0} {1}/{2}: {3}", Localisable.UninstallProgressWindow_Uninstalling,
                    _currentTargetStatus.CurrentTask, _currentTargetStatus.TaskCount,
                    currentUninstallerStatus.UninstallerEntry.DisplayName);

                objectListView1.EnsureVisible(objectListView1.IndexOf(currentUninstallerStatus));

                buttonClose.Text = Buttons.ButtonCancel;
                progressBar1.Value = _currentTargetStatus.CurrentTask - 1;
                progressBar1.Style = ProgressBarStyle.Continuous;
            }
            catch
            {
                progressBar1.Style = ProgressBarStyle.Marquee;
            }

            ResumeLayout();
        }

        private void OpenSkipMessagebox(object sender, EventArgs e)
        {
            CloseSkipMessagebox();

            _skipMessageBox = MessageBoxes.TaskSkipCurrentKillTaskQuestion(this);
            Enabled = false;
            _skipMessageBox.FormClosed += (x, y) => Enabled = true;

            _skipMessageboxThread = new Thread(SkipMesssageboxThread);
            _skipMessageboxThread.Start();
        }

        private void OpenWalkAwayMessagebox()
        {
            _walkAwayBox = MessageBoxes.CanWalkAwayInfo(this);

            Enabled = false;
            _walkAwayBox.FormClosed += (x, y) => Enabled = true;
        }

        private void SkipMesssageboxThread()
        {
            if (_skipMessageBox == null)
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
                        _currentTargetStatus.SkipWaitingForCurrent(true);
                        break;
                    case CustomMessageBox.PressedButton.Middle:
                        _currentTargetStatus.SkipWaitingForCurrent(false);
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
                });
            }
        }

        private void UninstallProgressWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_currentTargetStatus.Finished || _currentTargetStatus.Aborted ||
                e.CloseReason != CloseReason.UserClosing)
            {
                _currentTargetStatus.OnStatusChanged -= currentTargetStatus_OnCurrentTaskChanged;
                return;
            }

            if (MessageBoxes.TaskStopConfirmation() == MessageBoxes.PressedButton.Yes)
            {
                _currentTargetStatus.Aborted = true;
                buttonClose.Enabled = false;
            }

            e.Cancel = true;
        }

        private void UninstallProgressWindow_Resize(object sender, EventArgs e)
        {
            //linkLabel1.Text = Height > MinimumSize.Height ? "Show less information" : "Show more information";
        }

        private void UninstallProgressWindow_Shown(object sender, EventArgs e)
        {
            //Height = MinimumSize.Height;
        }
    }
}