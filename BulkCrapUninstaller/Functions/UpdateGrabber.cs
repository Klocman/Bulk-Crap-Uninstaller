using System;
using System.Reflection;
using System.Threading;
using BulkCrapUninstaller.Properties;
using Klocman.Forms;
using Klocman.Subsystems.Update;
using Klocman.Tools;

namespace BulkCrapUninstaller.Functions
{
    internal static class UpdateGrabber
    {
        private static readonly Uri DebugUpdateFeedUri =
            new Uri(@"https://dl.dropboxusercontent.com/u/21871088/Update/BCUninstaller_test.xml");

        private static readonly Uri UpdateFeedUri =
            new Uri(@"https://dl.dropboxusercontent.com/u/21871088/Update/BCUninstaller.xml");

        /// <summary>
        ///     Look for updates while displaying a progress bar. At the end display a message box with the result.
        /// </summary>
        public static void LookForUpdates()
        {
            var result = UpdateSystem.UpdateStatus.CheckFailed;
            var error = LoadingDialog.ShowDialog(Localisable.LoadingDialogTitleSearchingForUpdates,
                x => { result = UpdateSystem.CheckForUpdates(); });

            if (error == null)
            {
                switch (result)
                {
                    case UpdateSystem.UpdateStatus.CheckFailed:
                        MessageBoxes.UpdateFailed(UpdateSystem.LastError != null
                            ? UpdateSystem.LastError.Message
                            : "Unknown error");
                        break;

                    case UpdateSystem.UpdateStatus.NewAvailable:
                        if (MessageBoxes.UpdateAskToDownload())
                            UpdateSystem.BeginUpdate();
                        break;

                    case UpdateSystem.UpdateStatus.UpToDate:
                        MessageBoxes.UpdateUptodate();
                        break;
                }
            }
            else
            {
                MessageBoxes.UpdateFailed(error.Message);
            }
        }

        /// <summary>
        ///     Setup update system and automatically search for them if auto update is enabled.
        ///     Doesn't block, use delegates to interface.
        /// </summary>
        public static void Setup()
        {
            UpdateSystem.UpdateFeedUri = Program.EnableDebug ? DebugUpdateFeedUri : UpdateFeedUri;
            UpdateSystem.CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version;
        }

        /// <summary>
        ///     Automatically search for updates if auto update is enabled.
        ///     Doesn't block, use delegates to interface.
        /// </summary>
        /// <param name="canDisplayMessage">updateFoundCallback will be called after this returns true</param>
        /// <param name="updateFoundCallback">Launched only if a new update was found. It's launched from a background thread.</param>
        public static void AutoUpdate(Func<bool> canDisplayMessage, Action updateFoundCallback)
        {
            if (Settings.Default.MiscCheckForUpdates && WindowsTools.IsNetworkAvailable())
            {
                new Thread(() =>
                {
                    switch (UpdateSystem.CheckForUpdates())
                    {
                        case UpdateSystem.UpdateStatus.NewAvailable:
                        {
                            while (!canDisplayMessage())
                                Thread.Sleep(100);
                            try
                            {
                                updateFoundCallback();
                            }
                            catch
                            {
                                // Ignore background error, not necessary
                            }
                        }
                            break;
                    }
                }) {Name = "UpdateCheck_Thread", IsBackground = true}.Start();
            }
        }
    }
}