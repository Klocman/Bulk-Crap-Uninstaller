using System;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Klocman.Extensions;
using Timer = System.Timers.Timer;

namespace Klocman.Forms
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "<Pending>")]
    public sealed class LoadingDialogInterface
    {
        private readonly Timer _updateTimer;

        private Action _lastProgressUpdate;
        private Action _lastSubProgressUpdate;

        internal LoadingDialogInterface(LoadingDialog dialog)
        {
            Dialog = dialog;
            _updateTimer = new Timer
            {
                AutoReset = true,
                Interval = 35,
                SynchronizingObject = Dialog
            };
            _updateTimer.Elapsed += UpdateTimerOnElapsed;
            _updateTimer.Start();
            dialog.Disposed += (sender, args) => _updateTimer.Dispose();
        }

        private void UpdateTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var p = Interlocked.Exchange(ref _lastProgressUpdate, null);
            p?.Invoke();
            p = Interlocked.Exchange(ref _lastSubProgressUpdate, null);
            p?.Invoke();
        }

        public bool Abort { get; internal set; }

        private LoadingDialog Dialog { get; }

        public void CloseDialog()
        {
            _updateTimer.Dispose();
            Dialog.SafeInvoke(() => { if (!Dialog.IsDisposed) Dialog.Close(); });
        }

        public void SetMaximum(int value)
        {
            SetMaximumInt(value, Dialog.ProgressBar);
        }

        private void SetMaximumInt(int value, ProgressBar targetBar)
        {
            Dialog.SafeInvoke(() =>
            {
                if (targetBar.Maximum == value) return;

                if (value < targetBar.Minimum)
                    targetBar.Style = ProgressBarStyle.Marquee;
                else
                {
                    try
                    {
                        targetBar.Maximum = value;
                    }
                    catch
                    {
                        targetBar.Style = ProgressBarStyle.Marquee;
                    }
                }
            });
        }

        public void SetMinimum(int value)
        {
            SetMinimumInt(value, Dialog.ProgressBar);
        }

        private void SetMinimumInt(int value, ProgressBar targetBar)
        {
            Dialog.SafeInvoke(() =>
            {
                try
                {
                    targetBar.Minimum = value;
                }
                catch
                {
                    targetBar.Style = ProgressBarStyle.Marquee;
                }
            });
        }

        /// <summary>
        ///     Setting progress automatically changes the style from "Unknown time" (Marquee) to a normal progressbar.
        ///     If the value is invalid the style goes back to Marquee
        /// </summary>
        public void SetProgress(int value, string description = null, bool forceNoAnimation = false)
        {
            _lastProgressUpdate = () => SetProgressInt(value, description, Dialog.ProgressBar, Dialog.StatusLabel, forceNoAnimation);
        }

        private void SetProgressInt(int value, string description, ProgressBar targetBar, Label targetLabel, bool forceNoAnimation)
        {
            if (description != null && targetLabel.Text != description)
                targetLabel.Text = description;

            try
            {
                if (targetBar.Value == value)
                    return;

                var min = targetBar.Minimum;
                var max = targetBar.Maximum;
                if (min >= max || min > value || value > max)
                {
                    targetBar.Style = ProgressBarStyle.Marquee;
                }
                else
                {
                    targetBar.Style = ProgressBarStyle.Blocks;

                    if (value < min) value = min;
                    else if (value > max) value = max;

                    if (forceNoAnimation)
                    {
                        // Moving progress forward then back skips the animation
                        if (value == max)
                        {
                            targetBar.Maximum = value + 1;
                            targetBar.Value = value + 1;
                            targetBar.Maximum = value;
                        }
                        else
                        {
                            targetBar.Value = value + 1;
                        }
                    }

                    targetBar.Value = value;
                }
            }
            catch
            {
                targetBar.Style = ProgressBarStyle.Marquee;
            }
        }

        public void SetSubMaximum(int value)
        {
            SetMaximumInt(value, Dialog.SubProgressBar);
        }

        public void SetSubMinimum(int value)
        {
            SetMinimumInt(value, Dialog.SubProgressBar);
        }

        public void SetSubProgress(int value, string description, bool forceNoAnimation = false)
        {
            _lastSubProgressUpdate = () => SetProgressInt(value, description, Dialog.SubProgressBar, Dialog.SubStatusLabel, forceNoAnimation);
        }

        public void SetSubProgressVisible(bool visible)
        {
            Dialog.SafeInvoke(() => { Dialog.SubProgressVisible = visible; });
        }

        public void SetTitle(string newTitle)
        {
            Dialog.SafeInvoke(() => { Dialog.Text = newTitle; });
        }

        /// <summary>
        ///     Block current thread until dialog is visible
        /// </summary>
        internal void WaitTillDialogIsReady()
        {
            var notReady = true;
            while (notReady)
            {
                Dialog.SafeInvoke(() => notReady = !Dialog.Visible);
                Thread.Sleep(10);
            }
        }
    }
}