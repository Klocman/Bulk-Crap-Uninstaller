﻿/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BrightIdeasSoftware;
using BulkCrapUninstaller.Functions;
using BulkCrapUninstaller.Functions.Tracking;
using BulkCrapUninstaller.Properties;
using Klocman.Extensions;
using Klocman.Forms;
using Klocman.Forms.Tools;
using Klocman.IO;
using Klocman.Native;
using Klocman.Subsystems;
using Klocman.Tools;
using UninstallTools;
using UninstallTools.Dialogs;
using UninstallTools.Lists;
using UninstallTools.Uninstaller;

namespace BulkCrapUninstaller.Forms
{
    internal sealed partial class MainWindow : Form
    {
        private string MainTitleBarText { get; }

        private readonly UninstallerListViewTools _listView;
        private readonly SettingTools _setMan;
        private readonly WindowStyleController _styleController;
        private readonly Uninstaller _uninstaller;

        private readonly ListLegendWindow _listLegendWindow = new ListLegendWindow();
        private DebugWindow _debugWindow;

        private bool _previousListLegendState = true;
        private bool _anyStoreApps;
        private bool _anyWinFeatures;
        private bool _anyOrphans;
        private bool _anyProtected;
        private bool _anySysComponents;
        private bool _anyUpdates;
        private bool _anyInvalid;

        /// <summary>
        ///     Set to false in the list view clicked event. Prevents firing of extra CellEditStarting events.
        ///     Used to fix buggy ObjectListView.
        /// </summary>
        private bool _ignoreCellEdit;

        public MainWindow()
        {
            Opacity = 0;
            Application.DoEvents();

            InitializeComponent();

            // Setup settings
            _setMan = new SettingTools(Settings.Default.SettingBinder, this);
            _setMan.LoadSettings();
            BindControlsToSettings();

            // Finish up setting controls and window, suspend after settings have loaded
            SuspendLayout();
            ToolStripManager.Renderer = new ToolStripProfessionalRenderer(new StandardSystemColorTable())
            {
                RoundedEdges = true
            };

            // Disable until the first list refresh finishes
            LockApplication(true);

            // Other bindings
            _setMan.Selected.Subscribe((x, y) =>
                UninstallToolsGlobalConfig.CustomProgramFiles =
                    y.NewValue.SplitNewlines(StringSplitOptions.RemoveEmptyEntries)
                    .Select(path => path.Trim().Trim('"').Trim()).ToArray(),
                x => x.FoldersCustomProgramDirs, this);

            _setMan.Selected.Subscribe(RefreshListLegend, x => x.AdvancedTestCertificates, this);
            _setMan.Selected.Subscribe(RefreshListLegend, x => x.AdvancedTestInvalid, this);
            _setMan.Selected.Subscribe(RefreshListLegend, x => x.FilterShowStoreApps, this);
            _setMan.Selected.Subscribe(RefreshListLegend, x => x.FilterShowWinFeatures, this);
            _setMan.Selected.Subscribe(RefreshListLegend, x => x.AdvancedDisplayOrphans, this);

            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.ScanSteam = y.NewValue, x => x.ScanSteam, this);
            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.ScanStoreApps = y.NewValue, x => x.ScanStoreApps, this);
            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.ScanWinFeatures = y.NewValue, x => x.ScanWinFeatures, this);
            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.ScanWinUpdates = y.NewValue, x => x.ScanWinUpdates, this);

            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.ScanDrives = y.NewValue, x => x.ScanDrives, this);
            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.ScanRegistry = y.NewValue, x => x.ScanRegistry, this);
            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.ScanPreDefined = y.NewValue, x => x.ScanPreDefined, this);

            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.AutoDetectCustomProgramFiles = y.NewValue, x => x.FoldersAutoDetect, this);

            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.QuietAutomatization = y.NewValue,
                x => x.QuietAutomatization, this);
            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.QuietAutomatizationKillStuck = y.NewValue,
                x => x.QuietAutomatizationKillStuck, this);

            // Setup list view
            _listView = new UninstallerListViewTools(this);
            _uninstaller = new Uninstaller(_listView.InitiateListRefresh, LockApplication, SetVisible);

            toolStripButtonSelAll.Click += _listView.SelectAllItems;
            toolStripButtonSelNone.Click += _listView.DeselectAllItems;
            toolStripButtonSelInv.Click += _listView.InvertSelectedItems;
            _listView.AfterFiltering += RefreshStatusbarTotalLabel;
            _listView.UninstallerPostprocessingProgressUpdate += (x, y) =>
            {
                string result = null;

                if (y.Value == y.Maximum)
                    result = string.Empty;
                else if ((y.Value - 1) % 15 == 0)
                    result = string.Format(CultureInfo.CurrentCulture, Localisable.MainWindow_Statusbar_ProcessingUninstallers,
                        y.Value, y.Maximum);

                if (result != null)
                    this.SafeInvoke(() => toolStripLabelStatus.Text = result);
            };
            _listView.UninstallerFileLock = _uninstaller.PublicUninstallLock;
            _listView.ListRefreshIsRunningChanged += _listView_ListRefreshIsRunningChanged;

            advancedFilters1.CurrentListChanged += RefreshSidebarVisibility;
            advancedFilters1.CurrentListChanged +=
                (sender, args) => _listView.FilteringOverride = advancedFilters1.CurrentList;
            advancedFilters1.FiltersChanged += (sender, args) =>
            {
                if (_listView.FilteringOverride != null)
                    _listView.UpdateColumnFiltering();
            };
            advancedFilters1.CurrentListFileNameChanged += RefreshTitleBar;
            advancedFilters1.UnsavedChangesChanged += RefreshTitleBar;
            advancedFilters1.SelectedEntryGetter = () => _listView.SelectedUninstallers;

            // Setup update manager, skip at first boot to let user change the setting
            UpdateGrabber.Setup();
            if (!_setMan.Selected.Settings.MiscFirstRun)
            {
                BackgroundSearchForUpdates();
            }

            // Setup the main window
            Icon = Resources.Icon_Logo;
            MainTitleBarText = Text.Append(" v", Program.AssemblyVersion.ToString(Program.AssemblyVersion.Build != 0 ? 3 : 2))
                .AppendIf(!Program.IsInstalled, " ", Localisable.StrIsPortable)
                .AppendIf(ProcessTools.Is64BitProcess, " ", Localisable.Str64Bit)
                .AppendIf(Program.EnableDebug, " ", Localisable.StrDebug);
            Text = MainTitleBarText;

            _styleController = new WindowStyleController(this);

            // Initialize the status bar
            toolStripLabelStatus_TextChanged(this, EventArgs.Empty);

            // Debug stuff
            debugToolStripMenuItem.Enabled = Program.EnableDebug;
            debugToolStripMenuItem.Visible = Program.EnableDebug;
            _setMan.Selected.Settings.AdvancedSimulate = Program.EnableDebug;

            // Tracking
            UsageManager.DataSender = new DatabaseStatSender(Program.DbConnectionString,
                Resources.DbCommandStats, _setMan.Selected.Settings.MiscUserId);
            FormClosed += (x, y) => new Thread(UsageTrackerSendData).Start();

            // Misc
            filterEditor1.ComparisonMethodChanged += SearchCriteriaChanged;

            MessageBoxes.DefaultOwner = this;
            LoadingDialog.DefaultOwner = this;
            PremadeDialogs.DefaultOwner = this;
            PremadeDialogs.SendErrorAction = NBug.Exceptions.Report;

            SetupHotkeys();
        }

        private void RefreshListLegend(object sender, EventArgs e)
        {
            var force = advancedFilters1.CurrentList != null;
            _listLegendWindow.ListLegend.CertificatesEnabled = force || _setMan.Selected.Settings.AdvancedTestCertificates;
            _listLegendWindow.ListLegend.InvalidEnabled = force || _setMan.Selected.Settings.AdvancedTestInvalid && _anyInvalid;
            _listLegendWindow.ListLegend.StoreAppEnabled = force || _setMan.Selected.Settings.FilterShowStoreApps && _anyStoreApps;
            _listLegendWindow.ListLegend.OrphanedEnabled = force || _setMan.Selected.Settings.AdvancedDisplayOrphans && _anyOrphans;
            _listLegendWindow.ListLegend.WinFeatureEnabled = force || _setMan.Selected.Settings.FilterShowWinFeatures && _anyWinFeatures;
            _listLegendWindow.UpdatePosition(uninstallerObjectListView);
        }

        private void RefreshTitleBar(object sender, EventArgs e)
        {
            var result = MainTitleBarText;
            if (!string.IsNullOrEmpty(advancedFilters1.CurrentListFileName) || advancedFilters1.UnsavedChanges)
            {
                var changedDot = advancedFilters1.UnsavedChanges ? "*" : string.Empty;
                result = string.Format(CultureInfo.CurrentCulture, "{0} [{1}{2}]",
                    result, advancedFilters1.CurrentListFileName ?? string.Empty, changedDot);
            }
            Text = result;
        }

        /// <summary>
        ///     Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    components?.Dispose();
                }
                catch (NullReferenceException)
                {
                    // ObjectListView sometimes throws it at exit
                }

                _listView?.Dispose();
            }
            try { base.Dispose(disposing); }
            catch (InvalidOperationException) { }
        }

        private void SearchCriteriaChanged(object sender, EventArgs e)
        {
            _listView.UpdateColumnFiltering();
        }

        public void LockApplication(bool value)
        {
            this.SafeInvoke(() =>
            {
                UseWaitCursor = value;

                foreach (Control control in Controls)
                    control.Enabled = !value;

                Refresh();
            });
        }

        private void SetVisible(bool val)
        {
            this.SafeInvoke(() =>
            {
                Visible = val;
                if (_listLegendWindow != null)
                {
                    if (val)
                    {
                        _setMan.Selected.Settings.UninstallerListShowLegend = _previousListLegendState;
                        //_listLegendWindow.Visible = _previousListLegendState;
                    }
                    else
                    {
                        _previousListLegendState = _setMan.Selected.Settings.UninstallerListShowLegend;
                        _listLegendWindow.Visible = false;
                    }
                }
            });
        }

        internal static void OpenUrls(IEnumerable<Uri> urls)
        {
            if (WindowsTools.IsNetworkAvailable())
            {
                var urlList = urls as IList<Uri> ?? urls.ToList();
                if (MessageBoxes.OpenUrlsMessageBox(urlList.Count))
                {
                    try
                    {
                        urlList.ForEach(x => Process.Start(x.AbsoluteUri));
                    }
                    catch (Exception e)
                    {
                        MessageBoxes.OpenUrlError(e);
                    }
                }
            }
            else
                MessageBoxes.NoNetworkConnected();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var abox = new AboutBox())
            {
                abox.ShowDialog();
            }
        }

        private void advancedOperationsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            var selectionCount = _listView.SelectedUninstallerCount;
            openKeyInRegeditToolStripMenuItem.Enabled = selectionCount == 1;
            deleteToolStripMenuItem.Enabled = selectionCount > 0;
            createBackupToolStripMenuItem.Enabled = selectionCount > 0;
            msiUninstalltoolStripMenuItem.Enabled = selectionCount == 1;

            var autostart = _listView.SelectedUninstallers.Any(
                u => u.StartupEntries != null && u.StartupEntries.Any(se => !se.Disabled));
            disableAutostartToolStripMenuItem.Enabled = autostart;
        }

        private void BackgroundSearchForUpdates()
        {
            UpdateGrabber.AutoUpdate(() => _listView.FirstRefreshCompleted,
                () => this.SafeInvoke(() =>
                {
                    UpdateGrabber.AskAndBeginUpdate();
                }));
        }

        private void basicOperationsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            var selectionCount = _listView.SelectedUninstallerCount;
            uninstallToolStripMenuItem.Enabled = selectionCount > 0;
            quietUninstallToolStripMenuItem.Enabled = selectionCount > 0;
            propertiesToolStripMenuItem.Enabled = selectionCount > 0;
            modifyToolStripMenuItem.Enabled = selectionCount == 1 &&
                !string.IsNullOrEmpty(_listView.SelectedUninstallers.FirstOrDefault()?.ModifyPath);
        }

        private void BindControlsToSettings()
        {
            var settings = _setMan.Selected;

            // Bind controls to their respective settings
            settings.BindControl(displayToolbarToolStripMenuItem, x => x.ToolbarsShowToolbar, this);
            settings.BindControl(displaySettingsToolStripMenuItem, x => x.ToolbarsShowSettings, this);
            settings.BindControl(useSystemThemeToolStripMenuItem, x => x.WindowUseSystemTheme, this);
            settings.BindControl(displayStatusbarToolStripMenuItem, x => x.ToolbarsShowStatusbar, this);

            settings.BindControl(showColorLegendToolStripMenuItem, x => x.UninstallerListShowLegend, this);

            settings.Subscribe(RefreshSidebarVisibility,
                x => x.ToolbarsShowSettings, this);
            settings.Subscribe((x, y) => toolStrip.Visible = y.NewValue,
                x => x.ToolbarsShowToolbar, this);
            settings.Subscribe((x, y) => _styleController.SetStyles(y.NewValue),
                x => x.WindowUseSystemTheme, this);
            settings.Subscribe((x, y) => statusStrip1.Visible = y.NewValue,
                x => x.ToolbarsShowStatusbar, this);

            settings.Subscribe((x, y) =>
            {
                if (_listView.CheckIsAppDisposed()) return;

                try
                {
                    uninstallerObjectListView.CheckBoxes = y.NewValue;
                }
                catch (InvalidOperationException)
                {
                    // Setting CheckBoxes value throws this exception (even though it works fine). Bug in objectlistview?
                }
                _listView.RefreshList();
                uninstallerObjectListView_SelectedChanged(this, EventArgs.Empty);
            }, x => x.UninstallerListUseCheckboxes, this);

            settings.Subscribe((x, y) =>
            {
                if (_listView.CheckIsAppDisposed()) return;

                uninstallerObjectListView.ShowGroups = y.NewValue;
                _listView.RefreshList();
            }, x => x.UninstallerListUseGroups, this);

            settings.Subscribe(RefreshList, x => x.FilterHideMicrosoft, this);
            settings.Subscribe(RefreshList, x => x.FilterShowUpdates, this);
            settings.Subscribe(RefreshList, x => x.FilterShowSystemComponents, this);
            settings.Subscribe(RefreshList, x => x.FilterShowProtected, this);
            settings.Subscribe(RefreshList, x => x.FilterShowStoreApps, this);
            settings.Subscribe(RefreshList, x => x.FilterShowWinFeatures, this);

            settings.Subscribe((sender, args) =>
            {
                if (_listView.CheckIsAppDisposed()) return;

                olvColumnRating.IsVisible = args.NewValue;
                uninstallerObjectListView.RebuildColumns();
            }, x => x.MiscUserRatings, this);
        }

        private void RefreshSidebarVisibility(object sender, EventArgs e)
        {
            this.BeginControlUpdate();
            SuspendLayout();

            var ulistOpen = advancedFilters1.CurrentList != null;
            splitContainer1.Panel1Collapsed = !ulistOpen;
            splitContainer1.Panel1.Enabled = ulistOpen;

            var sidebarOpen = _setMan.Selected.Settings.ToolbarsShowSettings && !ulistOpen;
            settingsSidebarPanel.Visible = sidebarOpen;
            settingsSidebarPanel.Enabled = sidebarOpen;

            RefreshListLegend(sender, e);

            ResumeLayout();
            this.EndControlUpdate();
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (WindowsTools.IsNetworkAvailable())
            {
                LockApplication(true);
                UpdateGrabber.LookForUpdates();
                LockApplication(false);
            }
            else
                MessageBoxes.NoNetworkConnected();
        }

        private void cleanUpProgramFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _uninstaller.SearchForAndRemoveProgramFilesJunk(_listView.AllUninstallers);
        }

        private void ClipboardCopyFullInformation(object x, EventArgs y)
        {
            ImportExport.CopyFullInformationToClipboard(_listView.SelectedUninstallers);
        }

        private void ClipboardCopyGuids(object x, EventArgs y)
        {
            ImportExport.CopyGuidsToClipboard(_listView.SelectedUninstallers);
        }

        private void ClipboardCopyProgramName(object x, EventArgs y)
        {
            ImportExport.CopyNamesToClipboard(_listView.SelectedUninstallers);
        }

        private void ClipboardCopyRegistryPath(object x, EventArgs y)
        {
            ImportExport.CopyRegKeysToClipboard(_listView.SelectedUninstallers);
        }

        private void ClipboardCopyUninstallString(object x, EventArgs y)
        {
            ImportExport.CopyUninstallStringsToClipboard(_listView.SelectedUninstallers);
        }

        private void createBackupFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            if (!_listView.SelectedUninstallers.Any())
            {
                e.Cancel = true;
                return;
            }

            try
            {
                RegistryTools.ExportRegistry(createBackupFileDialog.FileName,
                    _listView.SelectedUninstallers.Select(x => x.RegistryPath));
            }
            catch (Exception ex)
            {
                MessageBoxes.ExportFailed(ex.Message, this);
                e.Cancel = true;
            }
        }

        private void CreateRegistryBackup(object sender, EventArgs e)
        {
            createBackupFileDialog.ShowDialog();
        }

        private void DeleteRegistryEntries(object sender, EventArgs eventArgs)
        {
            if (_listView.SelectedUninstallerCount == 0)
                return;

            var items = _listView.SelectedUninstallers.ToArray();
            var protectedItems = items.Where(x => x.IsProtected).ToArray();

            if (!_setMan.Selected.Settings.AdvancedDisableProtection && protectedItems.Any())
            {
                var affectedKeyNames = protectedItems.Select(x => x.DisplayName).ToArray();
                if (MessageBoxes.ProtectedItemsWarningQuestion(affectedKeyNames) == MessageBoxes.PressedButton.Cancel)
                    return;

                items = _listView.SelectedUninstallers.Where(x => !x.IsProtected).ToArray();
            }

            if (!items.Any() || !MessageBoxes.DeleteRegKeysConfirmation(items.Select(x => x.DisplayName).ToArray()))
                return;

            foreach (var item in items)
            {
                try
                {
                    if (item.IsRegistered) RegistryTools.RemoveRegistryKey(item.RegistryPath);
                }
                catch (Exception ex)
                {
                    PremadeDialogs.GenericError(ex);
                }
            }

            _listView.InitiateListRefresh();
        }

        private void donateButton_Click(object sender, EventArgs e)
        {
            OpenUrls(new[] { new Uri(Resources.DonateLink) });
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void exportDialog_FileOk(object sender, CancelEventArgs e)
        {
            if (!Uninstaller.ExportUninstallers(_listView.SelectedUninstallers, exportDialog.FileName))
                e.Cancel = true;
        }

        private void exportSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportDialog.ShowDialog();
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            var selectionCount = _listView.SelectedUninstallerCount;
            exportSelectedToolStripMenuItem.Enabled = selectionCount > 0;
        }

        private void HandleListViewMenuKeystroke(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Apps)
            {
                if (_listView.SelectedUninstallerCount > 0)
                    uninstallListContextMenuStrip.Show(uninstallerObjectListView.PointToScreen(Point.Empty));
                e.SuppressKeyPress = true;
            }
        }

        private void helpToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            //uninstallBCUninstallToolstripMenuItem.Visible = Program.IsInstalled;
            uninstallBCUninstallToolstripMenuItem.Enabled = Program.IsInstalled;
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            _setMan.SaveSettings();
            SystemRestore.CancelSysRestore();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing || _setMan.Selected.Settings.MiscFirstRun ||
                !WindowsTools.IsNetworkAvailable())
                return;

            if (!_setMan.Selected.Settings.MiscFeedbackNagNeverShow)
            {
                if (!_setMan.Selected.Settings.MiscFeedbackNagShown &&
                    DateTime.Now - Process.GetCurrentProcess().StartTime > TimeSpan.FromMinutes(3))
                {
                    FeedbackBox.ShowFeedbackBox(this, true);
                }

                // Show the nag every other time
                _setMan.Selected.Settings.MiscFeedbackNagShown = !_setMan.Selected.Settings.MiscFeedbackNagShown;
            }
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            _setMan.Selected.SendUpdates();

            // Work around a bug in Object list view
            try
            {
                ResumeLayout();
            }
            catch (ObjectDisposedException)
            {
                Application.DoEvents();
                ResumeLayout();
            }

            _listView.InitiateListRefresh();

            settingsSidebarPanel.Width = propertiesSidebar.GetSuggestedWidth() +
                settingsSidebarPanel.Padding.Left +
                settingsSidebarPanel.Padding.Right;
        }

        private void SetupAndShowLegendWindow()
        {
            if (IsDisposed || Disposing)
                return;

            _listLegendWindow.Show(this);
            AddOwnedForm(_listLegendWindow);

            _listLegendWindow.UpdatePosition(uninstallerObjectListView);
            Resize += (o, args) => _listLegendWindow.UpdatePosition(uninstallerObjectListView);
            Move += (o, args) => _listLegendWindow.UpdatePosition(uninstallerObjectListView);
            Controls[0].EnabledChanged += (o, args) => _listLegendWindow.Enabled = Controls[0].Enabled;

            var settings = _setMan.Selected;
            settings.Subscribe((x, y) => _listLegendWindow.Visible = y.NewValue, x => x.UninstallerListShowLegend, this);
            _listLegendWindow.VisibleChanged += (x, y) =>
            {
                if (!_listLegendWindow.Visible && settings.Settings.UninstallerListShowLegend)
                    settings.Settings.UninstallerListShowLegend = false;
            };
        }

        private void msiInstallContextMenuStripItem_Click(object sender, EventArgs e)
        {
            _uninstaller.UninstallUsingMsi(MsiUninstallModes.InstallModify, _listView.SelectedUninstallers);
        }

        private void msiQuietUninstallContextMenuStripItem_Click(object sender, EventArgs e)
        {
            _uninstaller.UninstallUsingMsi(MsiUninstallModes.QuietUninstall, _listView.SelectedUninstallers);
        }

        private void msiUninstallContextMenuStripItem_Click(object sender, EventArgs e)
        {
            _uninstaller.UninstallUsingMsi(MsiUninstallModes.Uninstall, _listView.SelectedUninstallers);
        }

        private void OnFirstApplicationStart()
        {
            StartSetupWizard(this, EventArgs.Empty);

            // On first start the updates are not searched from constructor to give user a chance to disable them.
            BackgroundSearchForUpdates();
        }

        private void OpenAssociatedWebPage(object sender, EventArgs eventArgs)
        {
            var urls = _listView.SelectedUninstallers.Select(y => y.GetAboutUri()).Where(x => x != null).ToList();

            OpenUrls(urls);
        }

        private void OpenDebugWindow(object sender, EventArgs e)
        {
            if (_debugWindow == null || _debugWindow.IsDisposed)
            {
                _debugWindow = new DebugWindow(this, _listView, _uninstaller);
            }

            _debugWindow.Show();
        }

        private void OpenInRegedit(object sender, EventArgs e)
        {
            if (_listView.SelectedUninstallerCount == 1 && _listView.SelectedUninstallers.First().IsRegistered)
            {
                //MessageBox.Show("To properly open the uninstaller key Regedit will be restarted if it's already running.",
                //    "Open key in regedit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                try
                {
                    RegistryTools.OpenRegKeyInRegedit(_listView.SelectedUninstallers.First().RegistryPath);
                }
                catch (IOException ex)
                {
                    PremadeDialogs.GenericError(ex);
                }
            }
        }

        private void OpenInstallationSource(object sender, EventArgs eventArgs)
        {
            var sourceDirs =
                _listView.SelectedUninstallers.Where(x => x.InstallSource.IsNotEmpty())
                    .Select(y => y.InstallSource)
                    .ToList();

            if (MessageBoxes.OpenDirectoriesMessageBox(sourceDirs.Count))
            {
                try
                {
                    sourceDirs.ForEach(x => Process.Start(x));
                }
                catch (Exception e)
                {
                    MessageBoxes.OpenDirectoryError(e);
                }
            }
        }

        private void OpenInstallLocation(object sender, EventArgs eventArgs)
        {
            var sourceDirs =
                _listView.SelectedUninstallers.Where(x => x.InstallLocation.IsNotEmpty())
                    .Select(y => y.InstallLocation)
                    .ToList();

            if (MessageBoxes.OpenDirectoriesMessageBox(sourceDirs.Count))
            {
                try
                {
                    sourceDirs.ForEach(x => Process.Start(x));
                }
                catch (Exception e)
                {
                    MessageBoxes.OpenDirectoryError(e);
                }
            }
        }

        private void openProgramsAndFeaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                WindowsTools.OpenControlPanelApplet(ControlPanelCanonicalNames.ProgramsAndFeatures);
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
            }
        }

        private void OpenProperties(object sender, EventArgs eventArgs)
        {
            if (_listView.SelectedUninstallerCount == 0) return;

            using (var propertiesWindow = new PropertiesWindow())
            {
                propertiesWindow.ShowPropertiesDialog(_listView.SelectedUninstallers);
            }
        }

        private void OpenSubmitFeedbackWindow(object sender, EventArgs e)
        {
            FeedbackBox.ShowFeedbackBox(this, false);
        }

        private void OpenUninstallerLocation(object sender, EventArgs eventArgs)
        {
            var sourceDirs = _listView.SelectedUninstallers.Where(x => x.UninstallerFullFilename.IsNotEmpty()).ToList();

            if (MessageBoxes.OpenDirectoriesMessageBox(sourceDirs.Count))
            {
                try
                {
                    sourceDirs.ForEach(x =>
                    {
                        if (File.Exists(x.UninstallerFullFilename))
                            WindowsTools.OpenExplorerFocusedOnObject(x.UninstallerFullFilename);
                        else
                            Process.Start(x.UninstallerLocation);
                    });
                }
                catch (Exception e)
                {
                    MessageBoxes.OpenDirectoryError(e);
                }
            }
        }

        private void OpenUninstallLists(object sender, EventArgs e)
        {
            advancedFilters1.LoadUninstallList();
        }

        private void RefreshList(object sender, EventArgs e)
        {
            _listView.RefreshList();
        }

        private void RefreshStatusbarTotalLabel(object sender, EventArgs e)
        {
            toolStripLabelTotal.Text = string.Format(CultureInfo.CurrentCulture, Localisable.MainWindow_Statusbar_Total,
                _listView.FilteredUninstallers.Count(), _listView.GetFilteredSize());
        }

        private void ReloadUninstallers(object sender, EventArgs e)
        {
            _listView.InitiateListRefresh();
        }

        private void RenameEntries(object sender, EventArgs eventArgs)
        {
            if (_listView.SelectedUninstallerCount != 1)
            {
                MessageBoxes.CanSelectOnlyOneItemInfo();
                return;
            }

            var selected = _listView.SelectedUninstallers.First();

            if (!_setMan.Selected.Settings.AdvancedDisableProtection && selected.IsProtected)
            {
                MessageBoxes.ProtectedItemError(selected.DisplayName);
                return;
            }

            if (!selected.IsRegistered)
                return;

            string output;
            if (StringEditBox.ShowDialog(string.Format(CultureInfo.InvariantCulture, Localisable.MainWindow_Rename_Description, selected.DisplayName),
                Localisable.MainWindow_Rename_Title, selected.DisplayName, Buttons.ButtonOk, Buttons.ButtonCancel,
                out output))
            {
                try
                {
                    if (selected.Rename(output))
                        _listView.InitiateListRefresh();
                    else
                        MessageBoxes.InvalidNewEntryName();
                }
                catch (Exception exception)
                {
                    PremadeDialogs.GenericError(exception);
                }
            }
        }

        private void ResetSettingsDialog(object sender, EventArgs e)
        {
            _setMan.ResetSettingsDialog();
        }

        private void RunAdvancedUninstall(object sender, EventArgs e)
        {
            var items = _listView.SelectedUninstallers.ToArray();
            var protectedItems = items.Where(x => x.IsProtected).ToArray();

            if (!_setMan.Selected.Settings.AdvancedDisableProtection && protectedItems.Any())
            {
                var affectedKeyNames = protectedItems.Select(x => x.DisplayName).ToArray();
                if (MessageBoxes.ProtectedItemsWarningQuestion(affectedKeyNames) == MessageBoxes.PressedButton.Cancel)
                    return;

                items = _listView.SelectedUninstallers.Where(x => !x.IsProtected).ToArray();
            }

            if (!items.Any())
            {
                MessageBoxes.NoUninstallersSelectedInfo();
                return;
            }

            _uninstaller.AdvancedUninstall(items, _listView.AllUninstallers);
        }

        private void RunLoudUninstall(object x, EventArgs y)
        {
            _uninstaller.RunUninstall(_listView.SelectedUninstallers, _listView.AllUninstallers, false);
        }

        private void RunQuietUninstall(object x, EventArgs y)
        {
            _uninstaller.RunUninstall(_listView.SelectedUninstallers, _listView.AllUninstallers, true);

            /*var nonQuiet =
                _listView.SelectedUninstallers.Where(o => !o.QuietUninstallPossible)
                    .Select(p => p.DisplayName)
                    .ToArray();

            if (!nonQuiet.Any())
                _uninstaller.RunUninstall(_listView.SelectedUninstallers, _listView.AllUninstallers, true);
            else
            {
                switch (MessageBoxes.QuietUninstallersNotAvailableQuestion(nonQuiet))
                {
                    case MessageBoxes.PressedButton.Yes:
                        _uninstaller.RunUninstall(_listView.SelectedUninstallers,
                            _listView.AllUninstallers, true);
                        break;
                    case MessageBoxes.PressedButton.No:
                        _uninstaller.RunUninstall(_listView.SelectedUninstallers.Where(p => p.QuietUninstallPossible),
                            _listView.AllUninstallers, true);
                        break;
                    default:
                        return;
                }
            }*/
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_setMan.Selected.Settings.ToolbarsShowSettings)
                _setMan.Selected.Settings.ToolbarsShowSettings = true;
            filterEditor1.FocusSearchbox();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var sw = new SettingsWindow())
                sw.ShowDialog();
        }

        private void SetupHotkeys()
        {
            // File
            globalHotkeys1.Add(new HotkeyEntry(Keys.F5, reloadUninstallersToolStripMenuItem));
            globalHotkeys1.Add(new HotkeyEntry(Keys.O, false, true, false, loadUninstallerListToolStripMenuItem));
            globalHotkeys1.Add(new HotkeyEntry(Keys.F4, true, false, false, exitToolStripMenuItem));

            // View
            globalHotkeys1.Add(new HotkeyEntry(Keys.F, false, true, false, searchToolStripMenuItem_Click, null));
            globalHotkeys1.Add(new HotkeyEntry(Keys.F3, searchToolStripMenuItem));

            // Basic operations
            globalHotkeys1.Add(new HotkeyEntry(Keys.Delete, uninstallToolStripMenuItem));
            globalHotkeys1.Add(new HotkeyEntry(Keys.Delete, false, false, true, quietUninstallToolStripMenuItem,
                () => !_listView.CheckIsAppDisposed() && uninstallerObjectListView.ContainsFocus));
            globalHotkeys1.Add(new HotkeyEntry(Keys.C, false, true, false, copyFullInformationToolStripMenuItem,
                () => !_listView.CheckIsAppDisposed() && uninstallerObjectListView.ContainsFocus));
            globalHotkeys1.Add(new HotkeyEntry(Keys.Enter, true, false, false, propertiesToolStripMenuItem,
                () => !_listView.CheckIsAppDisposed() && uninstallerObjectListView.ContainsFocus));

            // Advanced operations
            globalHotkeys1.Add(new HotkeyEntry(Keys.Delete, false, true, true, manualUninstallToolStripMenuItem,
                () => !_listView.CheckIsAppDisposed() && uninstallerObjectListView.ContainsFocus));
            globalHotkeys1.Add(new HotkeyEntry(Keys.B, false, true, false, createBackupToolStripMenuItem,
                () => !_listView.CheckIsAppDisposed() && uninstallerObjectListView.ContainsFocus));
            globalHotkeys1.Add(new HotkeyEntry(Keys.R, false, true, false, openKeyInRegeditToolStripMenuItem,
                () => !_listView.CheckIsAppDisposed() && uninstallerObjectListView.ContainsFocus));

            // Tools
            globalHotkeys1.Add(new HotkeyEntry(Keys.P, false, true, false, settingsToolStripMenuItem_Click,
                settingsToolStripMenuItem));
        }

        private void StartSetupWizard(object sender, EventArgs e)
        {
            using (var wizard = new FirstStartBox())
            {
                wizard.StartPosition = FormStartPosition.CenterParent;
                wizard.ShowDialog(this);
            }
        }

        private void toolsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            addWindowsFeaturesToTheListToolStripMenuItem.Enabled = DismTools.DismIsAvailable;
        }

        private void toolStripLabelStatus_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(toolStripLabelStatus.Text))
            {
                toolStripLabelStatus.TextChanged -= toolStripLabelStatus_TextChanged;
                toolStripLabelStatus.Text = Localisable.MainWindow_Statusbar_StatusReady;
                toolStripLabelStatus.TextChanged += toolStripLabelStatus_TextChanged;
            }
        }

        private void uninstallBCUninstallToolstripMenuItem_Click(object sender, EventArgs e)
        {
            _uninstaller.AskToSelfUninstall();
        }

        private void uninstallerObjectListView_CellEditStarting(object sender, CellEditEventArgs e)
        {
            e.Cancel = true;

            if (_ignoreCellEdit || e.RowObject == null)
                return;

            _ignoreCellEdit = true;

            if (uninstallerObjectListView.CheckBoxes && !uninstallerObjectListView.IsChecked(e.RowObject))
            {
                uninstallerObjectListView.UncheckAll();
                uninstallerObjectListView.CheckObject(e.RowObject);
            }

            OpenProperties(sender, e);

            //uninstallerObjectListView.CancelCellEdit();
        }

        private void uninstallerObjectListView_CellRightClick(object sender, CellRightClickEventArgs e)
        {
            if (e.Model == null)
                return;

            if (uninstallerObjectListView.CheckBoxes && !uninstallerObjectListView.IsChecked(e.Model))
            {
                uninstallerObjectListView.UncheckAll();
                uninstallerObjectListView.CheckObject(e.Model);
            }

            e.MenuStrip = uninstallListContextMenuStrip;
        }

        private void uninstallerObjectListView_Click(object sender, EventArgs e)
        {
            _ignoreCellEdit = false;
        }

        private void uninstallerObjectListView_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = _listView.SelectItemFromKeystroke(e.KeyCode);
        }

        private void uninstallerObjectListView_SelectedChanged(object sender, EventArgs e)
        {
            toolStripLabelStatus.Text = _listView.SelectedUninstallerCount > 0
                ? string.Format(CultureInfo.CurrentCulture, Localisable.MainWindow_Statusbar_StatusSelection, _listView.SelectedUninstallerCount)
                : string.Empty;

            toolStripLabelSize.Text = _listView.GetSelectedSize().ToString();

            // Disable/enable edit menus
            var anySelected = _listView.SelectedUninstallerCount > 0;
            basicOperationsToolStripMenuItem.Enabled = anySelected;
            advancedOperationsToolStripMenuItem.Enabled = anySelected;

            toolStripButtonModify.Enabled = _listView.SelectedUninstallerCount == 1 &&
                _listView.SelectedUninstallers.Count(x => !string.IsNullOrEmpty(x.ModifyPath)) == 1;
        }

        private void UpdateUninstallListContextMenuStrip(object sender, CancelEventArgs e)
        {
            if (_listView.SelectedUninstallerCount == 0)
            {
                e.Cancel = true;
                return;
            }

            var advancedFiltering = advancedFilters1.CurrentList != null;
            toolStripSeparatorFiltering.Visible = advancedFiltering;
            excludeToolStripMenuItem.Visible = advancedFiltering;
            includeToolStripMenuItem.Visible = advancedFiltering;

            var singleItem = _listView.SelectedUninstallerCount == 1;

            uninstallUsingMsiExecContextMenuStripItem.Enabled =
                singleItem && !_listView.SelectedUninstallers.First().BundleProviderKey.IsEmpty();

            foreach (var itemToDisable in new[]
            {
                uninstallContextMenuStripItem,
                quietUninstallContextMenuStripItem,
                gUIDProductCodeCopyContextMenuStripItem,
                uninstallStringCopyContextMenuStripItem,
                installLocationOpenInExplorerContextMenuStripItem,
                uninstallerLocationOpenInExplorerContextMenuStripItem,
                sourceLocationOpenInExplorerContextMenuStripItem,
                openWebPageContextMenuStripItem,
                runToolStripMenuItem,
                manualUninstallToolStripMenuItem1
            })
                itemToDisable.Enabled = false;

            runToolStripMenuItem.DropDownItems.Clear();

            foreach (var item in _listView.SelectedUninstallers)
            {
                if (item.IsValid)
                {
                    if (item.UninstallPossible) uninstallContextMenuStripItem.Enabled = true;
                    if (item.QuietUninstallPossible) quietUninstallContextMenuStripItem.Enabled = true;
                    manualUninstallToolStripMenuItem1.Enabled = true;
                }

                if (singleItem)
                {
                    foreach (var executable in item.GetSortedExecutables())
                    {
                        if (!runToolStripMenuItem.Enabled) runToolStripMenuItem.Enabled = true;

                        runToolStripMenuItem.DropDownItems.Add(executable);
                    }
                }

                if (item.IsRegistered)
                    manualUninstallToolStripMenuItem1.Enabled = true;

                if (!item.BundleProviderKey.IsEmpty()) gUIDProductCodeCopyContextMenuStripItem.Enabled = true;
                if (item.UninstallPossible)
                {
                    uninstallStringCopyContextMenuStripItem.Enabled = true;
                    manualUninstallToolStripMenuItem1.Enabled = true;
                }

                if (item.InstallLocation.IsNotEmpty())
                {
                    installLocationOpenInExplorerContextMenuStripItem.Enabled = true;
                    manualUninstallToolStripMenuItem1.Enabled = true;
                }
                if (item.UninstallerLocation.IsNotEmpty())
                {
                    uninstallerLocationOpenInExplorerContextMenuStripItem.Enabled = true;
                    manualUninstallToolStripMenuItem1.Enabled = true;
                }
                if (item.InstallSource.IsNotEmpty()) sourceLocationOpenInExplorerContextMenuStripItem.Enabled = true;

                if (item.AboutUrl.IsNotEmpty()) openWebPageContextMenuStripItem.Enabled = true;
            }

            openInExplorerContextMenuStripItem.Enabled = installLocationOpenInExplorerContextMenuStripItem.Enabled ||
                                                         uninstallerLocationOpenInExplorerContextMenuStripItem.Enabled ||
                                                         sourceLocationOpenInExplorerContextMenuStripItem.Enabled;
        }

        private void UsageTrackerSendData()
        {
            if (_setMan.Selected.Settings.MiscSendStatistics)
            {
                UsageManager.FinishCollectingData();

                if (Program.EnableDebug || !WindowsTools.IsNetworkAvailable()) return;

                var count = UsageManager.AppLaunchCount;

                //Reduce frequency of the uploads
                if (count != 2 && (count <= 0 || count % 5 != 0)) return;

                try
                {
                    UsageManager.SendUsageData();
                }
                catch
                {
                    // Ignore, will try again next time
                }
            }
            else
            {
                UsageManager.RemoveStoredData();
            }
        }

        private void _listView_ListRefreshIsRunningChanged(object sender,
            UninstallerListViewTools.ListRefreshEventArgs e)
        {
            // Skip notifications about starting the refresh
            if (e.NewValue) return;

            // If refresh has finished update the interface
            _anyStoreApps = _listView.AllUninstallers.Any(x => x.UninstallerKind == UninstallerType.StoreApp);
            _anyWinFeatures = _listView.AllUninstallers.Any(x => x.UninstallerKind == UninstallerType.WindowsFeature);

            _anyOrphans = _listView.AllUninstallers.Any(x => x.IsOrphaned);
            _anyProtected = _listView.AllUninstallers.Any(x => x.IsProtected);
            _anySysComponents = _listView.AllUninstallers.Any(x => x.SystemComponent);
            _anyUpdates = _listView.AllUninstallers.Any(x => x.IsUpdate);
            _anyInvalid = _listView.AllUninstallers.Any(x => !x.IsValid);

            propertiesSidebar.StoreAppsEnabled = _anyStoreApps;
            propertiesSidebar.WinFeaturesEnabled = _anyWinFeatures;

            propertiesSidebar.OrphansEnabled = _anyOrphans;
            propertiesSidebar.ProtectedEnabled = _anyProtected;
            propertiesSidebar.SysCompEnabled = _anySysComponents;
            propertiesSidebar.UpdatesEnabled = _anyUpdates;
            propertiesSidebar.InvalidEnabled = _anyInvalid;

            if (e.FirstRefresh)
            {
                _setMan.LoadSorting();

                var args = Environment.GetCommandLineArgs();
                var dir = args.Skip(1).FirstOrDefault();
                if (!string.IsNullOrEmpty(dir))
                {
                    try
                    {
                        advancedFilters1.LoadUninstallList(dir);
                    }
                    catch (Exception ex)
                    { PremadeDialogs.GenericError(ex); }
                }
                if (advancedFilters1.CurrentList == null && _setMan.Selected.Settings.MiscAutoLoadDefaultList)
                {
                    try
                    {
                        var defaultUninstallListPath = Path.Combine(Program.AssemblyLocation.FullName, Resources.DefaultUninstallListFilename);
                        if (File.Exists(defaultUninstallListPath))
                            advancedFilters1.LoadUninstallList(defaultUninstallListPath);
                    }
                    catch (Exception ex)
                    { PremadeDialogs.GenericError(ex); }
                }

                splashScreen1.CloseSplashScreen();

                // Display the legend first so it is hidden under the splash
                _listLegendWindow.Opacity = 0;
                SetupAndShowLegendWindow();
                // Needed in case main window starts maximized
                _listLegendWindow.UpdatePosition(uninstallerObjectListView);
                _listLegendWindow.Visible = _setMan.Selected.Settings.UninstallerListShowLegend;

                new Thread(() =>
                {
                    this.SafeInvoke(() =>
                    {
                        if (_setMan.Selected.Settings.MiscFirstRun)
                        {
                            // Run the welcome wizard at first start of the application
                            OnFirstApplicationStart();
                        }

                        if (!_setMan.Selected.Settings.MiscNet4NagShown && !Program.Net4IsAvailable)
                        {
                            _setMan.Selected.Settings.MiscNet4NagShown = true;
                            MessageBoxes.Net4MissingInfo();
                        }
                    });
                }).Start();
            }

            RefreshListLegend(sender, e);
        }

        private void openStartupManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var results = StartupManagerWindow.ShowManagerDialog(this);
            toolStripLabelStatus.Text = Localisable.MainWindow_Statusbar_RefreshingStartup;

            //Application.DoEvents();
            //if (_listView.CheckIsAppDisposed())
            //    return;

            Cursor = Cursors.WaitCursor;
            statusStrip1.Refresh();

            _listView.ReassignStartupEntries(true, results);
            toolStripLabelStatus.Text = string.Empty;
            Cursor = DefaultCursor;
        }

        private void disableAutostartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var uninstaller in _listView.SelectedUninstallers)
            {
                if (uninstaller.StartupEntries == null)
                    continue;

                foreach (var entry in uninstaller.StartupEntries)
                {
                    try
                    {
                        entry.Disabled = true;
                    }
                    catch (Exception ex)
                    {
                        PremadeDialogs.GenericError(ex);
                    }
                }

                uninstallerObjectListView.RefreshObject(uninstaller);
            }
        }

        private void rateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _listView.RateEntries(_listView.SelectedUninstallers.ToArray(), Point.Empty);
        }

        private void OpenNukeWindow(object sender, EventArgs e)
        {
            var results = NukeWindow.ShowDialog(this);

            if (results == null) return;

            var apps = Uninstaller.GetApplicationsFromDirectories(_listView.AllUninstallers, results);

            if (apps.Count == 0)
            {
                MessageBoxes.UninstallFromDirectoryNothingFound();
                return;
            }

            _uninstaller.RunUninstall(apps, _listView.AllUninstallers, true);
        }

        private void addWindowsFeaturesToTheListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _setMan.Selected.Settings.FilterShowWinFeatures = true;
            filterEditor1.Search(nameof(UninstallerType.WindowsFeature), ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.UninstallerKind));
        }

        private void viewWindowsStoreAppsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _setMan.Selected.Settings.FilterShowStoreApps = true;
            filterEditor1.Search(nameof(UninstallerType.StoreApp), ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.UninstallerKind));
        }

        private void buttonAdvFiltering_Click(object sender, EventArgs e)
        {
            advancedFilters1.LoadUninstallList(new UninstallList(_listView.GenerateEquivalentFilter()));
        }

        private void OpenAdvancedClipboardCopy(object sender, EventArgs e)
        {
            AdvancedClipboardCopyWindow.ShowDialog(this, _listView.SelectedUninstallers);
        }

        private void openHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBoxes.DisplayHelp();
        }

        private void uninstallFromDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _uninstaller.UninstallFromDirectory(_listView.AllUninstallers);
        }

        private void openSystemRestoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                WindowsTools.OpenControlPanelApplet(ControlPanelCanonicalNames.Recovery);
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
            }
        }

        private void runToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            PremadeDialogs.StartProcessSafely(e.ClickedItem.Text);
        }

        private void viewUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _setMan.Selected.Settings.FilterShowUpdates = true;
            filterEditor1.Search(true.ToString(), ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.IsUpdate));
        }

        private void filterEditor1_FocusSearchTarget(object sender, EventArgs e)
        {
            uninstallerObjectListView.Focus();
        }

        private void googleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnlineSearchTools.SearchGoogle(_listView.SelectedUninstallers);
        }

        private void alternativeToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnlineSearchTools.SearchAlternativeTo(_listView.SelectedUninstallers);
        }

        private void fossHubcomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnlineSearchTools.SearchFosshub(_listView.SelectedUninstallers);
        }

        private void sourceForgecomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnlineSearchTools.SearchSourceforge(_listView.SelectedUninstallers);
        }

        private void fileHippocomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnlineSearchTools.SearchFilehippo(_listView.SelectedUninstallers);
        }

        private void gitHubcomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnlineSearchTools.SearchGithub(_listView.SelectedUninstallers);
        }

        private void modifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _uninstaller.Modify(_listView.SelectedUninstallers);
        }

        private void excludeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSelectedAsAdvancedFilters(true);
        }

        private void includeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSelectedAsAdvancedFilters(false);
        }

        private void AddSelectedAsAdvancedFilters(bool exclude)
        {
            var selectedUninstallers = _listView.SelectedUninstallers;
            var filters = advancedFilters1.CurrentList.Filters;

            var existingFilters = filters.Where(x => selectedUninstallers.Any(y => x.Name == y.DisplayName));
            filters.RemoveAll(existingFilters.ToList());

            filters.AddRange(selectedUninstallers.Select(x => new Filter(x.DisplayName, exclude, new FilterCondition(x.DisplayName, ComparisonMethod.Equals,
                    nameof(ApplicationUninstallerEntry.DisplayName)))));
            advancedFilters1.RepopulateList();
        }
    }
}