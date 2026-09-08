/*
    Copyright (c) 2026 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            var match = ShortcutUninstallMatcher.MatchExecutablePath(_listView.AllUninstallers, executablePath,
                out var ambiguous);
            if (ambiguous)
            {
                ShowShortcutUninstallError("More than one installed application matches this shortcut target. No application was uninstalled.");
                return;
            }

            if (match == null)
            {
                ShowShortcutUninstallError("No installed application could be identified from this shortcut target.");
                return;
            }

            _appUninstaller.RunUninstall(new[] {match}, _listView.AllUninstallers, false);
        }

        private void ShowShortcutUninstallError(string message)
        {
            MessageBox.Show(this, message, "BCUninstaller", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    internal static class ShortcutUninstallMatcher
    {
        internal static ApplicationUninstallerEntry MatchExecutablePath(
            IEnumerable<ApplicationUninstallerEntry> entries, string executablePath, out bool ambiguous)
        {
            ambiguous = false;
            if (entries == null || string.IsNullOrWhiteSpace(executablePath))
                return null;

            var candidates = entries as IList<ApplicationUninstallerEntry> ?? new List<ApplicationUninstallerEntry>(entries);

            var match = FindUnique(candidates,
                entry => PathTools.PathsEqual(entry?.UninstallerFullFilename, executablePath), out ambiguous);
            if (match != null || ambiguous)
                return match;

            match = FindUnique(candidates,
                entry => entry?.GetSortedExecutables()
                    .Any(path => PathTools.PathsEqual(path, executablePath)) == true, out ambiguous);
            if (match != null || ambiguous)
                return match;

            return FindUnique(candidates,
                entry => PathTools.SubPathIsInsideBasePath(entry?.InstallLocation, executablePath, true, false),
                out ambiguous);
        }

        private static ApplicationUninstallerEntry FindUnique(IEnumerable<ApplicationUninstallerEntry> entries,
            Func<ApplicationUninstallerEntry, bool> predicate, out bool ambiguous)
        {
            ambiguous = false;
            ApplicationUninstallerEntry match = null;

            foreach (var entry in entries)
            {
                if (!predicate(entry))
                    continue;

                if (match != null)
                {
                    ambiguous = true;
                    return null;
                }

                match = entry;
            }

            return match;
        }
    }
}
