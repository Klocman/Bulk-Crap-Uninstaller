/*
    Copyright (c) 2026 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions.ApplicationList;
using Klocman.Tools;
using UninstallTools;

namespace BulkCrapUninstaller.Forms
{
    internal sealed partial class MainWindow
    {
        internal void ConfigureShortcutUninstall(string shortcutPath)
        {
            if (string.IsNullOrWhiteSpace(shortcutPath))
                throw new ArgumentException("Shortcut path must not be empty.", nameof(shortcutPath));

            EventHandler<UninstallerListViewUpdater.ListRefreshEventArgs> handler = null;
            handler = (sender, args) =>
            {
                if (args.RefreshIsRunning || !args.FirstRefresh)
                    return;

                _listView.ListRefreshIsRunningChanged -= handler;
                BeginInvoke(new Action(() => ProcessShortcutUninstall(shortcutPath)));
            };

            _listView.ListRefreshIsRunningChanged += handler;
        }

        private void ProcessShortcutUninstall(string shortcutPath)
        {
            string executablePath;
            try
            {
                executablePath = WindowsTools.ResolveShortcut(shortcutPath);
            }
            catch (Exception)
            {
                ShowShortcutUninstallError("The shortcut could not be resolved to an existing executable file.");
                return;
            }

            if (string.IsNullOrWhiteSpace(executablePath) ||
                !executablePath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ||
                !File.Exists(executablePath))
            {
                ShowShortcutUninstallError("The shortcut could not be resolved to an existing executable file.");
                return;
            }

            var match = ShortcutUninstallMatcher.MatchExecutablePath(_listView.AllUninstallers, executablePath);
            switch (match.Status)
            {
                case ShortcutUninstallMatchStatus.NotFound:
                    ShowShortcutUninstallError("No installed application could be identified from this shortcut target.");
                    return;
                case ShortcutUninstallMatchStatus.Ambiguous:
                    ShowShortcutUninstallError("More than one installed application matches this shortcut target. No application was uninstalled.");
                    return;
                case ShortcutUninstallMatchStatus.Unique:
                    _appUninstaller.RunUninstall(new[] {match.Entry}, _listView.AllUninstallers, false);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ShowShortcutUninstallError(string message)
        {
            MessageBox.Show(this, message, "BCUninstaller", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
