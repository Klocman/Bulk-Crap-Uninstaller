/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BulkCrapUninstaller.Forms;
using BulkCrapUninstaller.Properties;
using Klocman.Binding.Settings;
using Klocman.Forms.Tools;

namespace BulkCrapUninstaller.Functions.Tools
{
    internal sealed class SettingTools
    {
        private readonly MainWindow _mainWindow;
        private bool _resetSettings;

        public SettingTools(SettingBinder<Settings> settingSet, MainWindow mainWindow)
        {
            Selected = settingSet;
            _mainWindow = mainWindow;
        }

        public SettingBinder<Settings> Selected { get; }

        public void LoadSettings()
        {
#if DEBUG
            Selected.Settings.Debug = true;
#endif
            if (Selected.Settings.MiscFirstRun) return;

            if (Selected.Settings.WindowSize.Width >= _mainWindow.MinimumSize.Width &&
                Selected.Settings.WindowSize.Height >= _mainWindow.MinimumSize.Height &&
                !Selected.Settings.WindowPosition.IsEmpty)
            {
                var targetRect = new Rectangle(Selected.Settings.WindowPosition, Selected.Settings.WindowSize);
                if (Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(targetRect)))
                {
                    _mainWindow.Size = Selected.Settings.WindowSize;
                    _mainWindow.Location = Selected.Settings.WindowPosition;

                    _mainWindow.StartPosition = FormStartPosition.Manual;

                    if (Selected.Settings.WindowState != FormWindowState.Minimized)
                        _mainWindow.WindowState = Selected.Settings.WindowState;
                }
            }

            if (!string.IsNullOrEmpty(Selected.Settings.UninstallerListViewState))
                _mainWindow.uninstallerObjectListView.RestoreState(
                    Convert.FromBase64String(Selected.Settings.UninstallerListViewState));
        }

        public void LoadSorting()
        {
            try
            {
                _mainWindow.uninstallerObjectListView.Sorting = Selected.Settings.UninstallerListSortOrder;
                _mainWindow.uninstallerObjectListView.Sort(Selected.Settings.UninstallerListSortColumn);
            }
            catch
            {
                // OLV can throw a nullref
            }
        }

        /// <summary>
        ///     Ask user to reset settings. If user selects yes the settings are reset and application is restarted.
        /// </summary>
        public void ResetSettingsDialog()
        {
            if (MessageBoxes.ResetSettingsConfirmation() == MessageBoxes.PressedButton.Yes)
            {
                _resetSettings = true;
                EntryPoint.Restart();
            }
        }

        public void SaveSettings()
        {
            if (_resetSettings)
            {
                try
                {
                    File.Delete(Program.ConfigFileFullname);
                }
                catch (Exception ex)
                {
                    /*Failed to save settings, probably read only drive*/
                    PremadeDialogs.GenericError(new IOException(Localisable.Error_SaveSettingsFailed, ex));
                }
            }
            else
            {
                Selected.Settings.WindowState = _mainWindow.WindowState;
                if (_mainWindow.WindowState == FormWindowState.Normal)
                {
                    Selected.Settings.WindowSize = _mainWindow.Size;
                    Selected.Settings.WindowPosition = _mainWindow.Location;
                }

                Selected.Settings.UninstallerListViewState =
                    Convert.ToBase64String(_mainWindow.uninstallerObjectListView.SaveState());

                Selected.Settings.UninstallerListSortOrder = _mainWindow.uninstallerObjectListView.PrimarySortOrder;
                Selected.Settings.UninstallerListSortColumn = _mainWindow.uninstallerObjectListView.Columns.IndexOf(
                    _mainWindow.uninstallerObjectListView.PrimarySortColumn);
                Selected.Settings.MiscVersion = Program.AssemblyVersion.ToString();

                try
                {
                    Selected.Settings.Save();
                }
                catch (Exception ex)
                {
                    /*Failed to save settings, probably read only drive*/
                    PremadeDialogs.GenericError(new IOException(Localisable.Error_SaveSettingsFailed, ex));
                }
            }
        }
    }
}