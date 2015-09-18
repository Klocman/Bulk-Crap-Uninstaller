using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using BulkCrapUninstaller.Functions;
using BulkCrapUninstaller.Properties;
using Klocman.Extensions;
using Klocman.Forms;
using Klocman.Forms.Tools;
using Klocman.IO;
using Klocman.Native;
using Klocman.Subsystems;
using Klocman.Subsystems.Tracking;
using Klocman.Subsystems.Update;
using Klocman.Tools;
using UninstallTools;
using UninstallTools.Dialogs;
using UninstallTools.Lists;
using UninstallTools.Uninstaller;

namespace BulkCrapUninstaller.Forms
{
    sealed partial class MainWindow : Form
    {
        private readonly UninstallerListViewTools _listView;
        private readonly SettingTools _setMan;
        private readonly WindowStyleController _styleController;
        private readonly Uninstaller _uninstaller;
        private DebugWindow _debugWindow;

        /// <summary>
        ///     Set to false in the list view clicked event. Prevents firing of extra CellEditStarting events.
        ///     Used to fix buggy ObjectListView.
        /// </summary>
        private bool _ignoreCellEdit;

        public MainWindow()
        {
            InitializeComponent();
            
            // Setup settings
            _setMan = new SettingTools(Settings.Default.SettingBinder, this);
            _setMan.LoadSettings();
            BindControlsToSettings();

            // Finish up setting controls and window, suspend after settings have loaded
            SuspendLayout();
            Opacity = 0;
            ToolStripManager.Renderer = new ToolStripProfessionalRenderer(new StandardSystemColorTable())
            {
                RoundedEdges = true
            };

            // Disable until the first list refresh finishes
            Enabled = false;

            // Other bindings
            _setMan.Selected.Subscribe((x, y) =>
                UninstallToolsGlobalConfig.CustomProgramFiles =
                    y.NewValue.SplitNewlines(StringSplitOptions.RemoveEmptyEntries),
                x => x.FoldersCustomProgramDirs, this);

            // Setup list view
            _listView = new UninstallerListViewTools(this);

            toolStripButtonSelAll.Click += _listView.SelectAllItems;
            toolStripButtonSelNone.Click += _listView.DeselectAllItems;
            toolStripButtonSelInv.Click += _listView.InvertSelectedItems;

            _listView.AfterFiltering += RefreshStatusbarTotalLabel;
            _listView.UninstallerPostprocessingProgressUpdate += (x, y) =>
            {
                string result = null;

                if (y.Value == y.Maximum)
                    result = string.Empty;
                else if ((y.Value - 1) % 7 == 0)
                    result = string.Format(Localisable.MainWindow_Statusbar_ProcessingUninstallers,
                        y.Value, y.Maximum);

                if (result != null)
                    this.SafeInvoke(() => toolStripLabelStatus.Text = result);
            };

            // Setup update manager, skip at first boot to let user change the setting
            UpdateGrabber.Setup();
            if (!_setMan.Selected.Settings.MiscFirstRun)
            {
                BackgroundSearchForUpdates();
            }

            // Setup the main window
            Icon = Resources.Icon_Logo;
            Text = Text.Append(" v", Program.AssemblyVersion.ToString(Program.AssemblyVersion.Build != 0 ? 3 : 2))
                .AppendIf(!Program.IsInstalled, Localisable.StrIsPortable)
                .AppendIf(ProcessTools.Is64BitProcess, Localisable.Str64Bit)
                .AppendIf(Program.EnableDebug, Localisable.StrDebug);

            _styleController = new WindowStyleController(this);

            // Initialize the status bar
            toolStripLabelStatus_TextChanged(this, EventArgs.Empty);

            // Debug stuff
            debugToolStripMenuItem.Enabled = Program.EnableDebug;
            debugToolStripMenuItem.Visible = Program.EnableDebug;
            _setMan.Selected.Settings.AdvancedSimulate = Program.EnableDebug;

            // External links
            facebookStatusButton1.TargetSite = Resources.HomepageUrl;
            twitterStatusButton1.TargetSite = Resources.HomepageUrl;

            // Tracking
            UsageManager.DataSender = new DatabaseStatSender(Resources.DbConnectionString,
                Resources.DbCommandStats, _setMan.Selected.Settings.MiscUserId);
            FormClosed += (x, y) => UsageTrackerSendData(); //new Thread(UsageTrackerSendData) { IsBackground = false, Name = "UsageManager" }.Start();

            // Misc
            _uninstaller = new Uninstaller(_listView.InitiateListRefresh, LockApplication);
            _listView.UninstallerFileLock = _uninstaller.PublicUninstallLock;
            _listView.ListRefreshIsRunningChanged += _listView_ListRefreshIsRunningChanged;

            filterEditor1.FilterTextChanged += SearchCriteriaChanged;
            filterEditor1.ComparisonMethodChanged += SearchCriteriaChanged;
            filterEditor1.ComparisonMethod = FilterComparisonMethod.Any;

            MessageBoxes.DefaultOwner = this;
            LoadingDialog.DefaultOwner = this;
            PremadeDialogs.DefaultOwner = this;

            SetupHotkeys();
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

                _listView.Dispose();
            }
            base.Dispose(disposing);
        }

        private void SearchCriteriaChanged(object sender, EventArgs e)
        {
            _listView.UpdateColumnFiltering(filterEditor1.FilterText, filterEditor1.ComparisonMethod);
        }

        public void LockApplication(bool value)
        {
            this.SafeInvoke(() =>
            {
                UseWaitCursor = value;
                Enabled = !value;
                Refresh();
            });
        }

        private static void OpenUrls(IEnumerable<Uri> urls)
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

        private void addWindowsFeaturesToTheListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_listView.DisplayWindowsFeatures())
                filterEditor1.Search("Dism.exe", FilterComparisonMethod.StartsWith);
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
                    if (MessageBoxes.UpdateAskToDownload())
                        UpdateSystem.BeginUpdate();
                }));
        }

        private void basicOperationsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            var selectionCount = _listView.SelectedUninstallerCount;
            uninstallToolStripMenuItem.Enabled = selectionCount > 0;
            quietUninstallToolStripMenuItem.Enabled = selectionCount > 0;
            propertiesToolStripMenuItem.Enabled = selectionCount > 0;
        }

        private void BindControlsToSettings()
        {
            var settings = _setMan.Selected;

            // Bind controls to their respective settings
            settings.BindControl(displayToolbarToolStripMenuItem, x => x.ToolbarsShowToolbar, this);
            settings.BindControl(displaySettingsToolStripMenuItem, x => x.ToolbarsShowSettings, this);
            settings.BindControl(useSystemThemeToolStripMenuItem, x => x.WindowUseSystemTheme, this);
            settings.BindControl(displayStatusbarToolStripMenuItem, x => x.ToolbarsShowStatusbar, this);

            // Show the legend control
            settings.BindControl(showColorLegendToolStripMenuItem, x => x.UninstallerListShowLegend, this);
            settings.Subscribe((x, y) => listLegend1.Visible = y.NewValue, x => x.UninstallerListShowLegend, this);
            listLegend1.VisibleChanged += (x, y) =>
            {
                if (!listLegend1.Visible && settings.Settings.UninstallerListShowLegend)
                    settings.Settings.UninstallerListShowLegend = false;
            };

            settings.Subscribe((x, y) => splitContainer1.Panel1Collapsed = !y.NewValue,
                x => x.ToolbarsShowSettings, this);
            settings.Subscribe((x, y) => toolStrip.Visible = y.NewValue,
                x => x.ToolbarsShowToolbar, this);
            settings.Subscribe((x, y) => _styleController.SetStyles(y.NewValue),
                x => x.WindowUseSystemTheme, this);
            settings.Subscribe((x, y) => statusStrip1.Visible = y.NewValue,
                x => x.ToolbarsShowStatusbar, this);

            settings.Subscribe((x, y) =>
            {
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
                uninstallerObjectListView.ShowGroups = y.NewValue;
                _listView.RefreshList();
            }, x => x.UninstallerListUseGroups, this);

            settings.Subscribe(RefreshList, x => x.FilterHideMicrosoft, this);
            settings.Subscribe(RefreshList, x => x.FilterShowUpdates, this);
            settings.Subscribe(RefreshList, x => x.FilterShowSystemComponents, this);
            settings.Subscribe(RefreshList, x => x.FilterShowProtected, this);
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

        private void cleanUpTheSystemCCleanerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenUrls(new[] { new Uri(@"https://www.piriform.com/ccleaner", UriKind.Absolute) });
        }

        private void ClipboardCopyFullInformation(object x, EventArgs y)
        {
            ImportExport.CopyToClipboard(_listView.SelectedUninstallers.Select(z => z.ToLongString()));
        }

        private void ClipboardCopyGuids(object x, EventArgs y)
        {
            ImportExport.CopyToClipboard(_listView.SelectedUninstallers.Select(z =>
                string.Format("{0} - {1}", z.DisplayName, z.BundleProviderKey.ToString("B").ToUpper())));
        }

        private void ClipboardCopyProgramName(object x, EventArgs y)
        {
            ImportExport.CopyToClipboard(_listView.SelectedUninstallers.Select(z => z.DisplayName));
        }

        private void ClipboardCopyRegistryPath(object x, EventArgs y)
        {
            ImportExport.CopyToClipboard(
                _listView.SelectedUninstallers.Select(z => string.Format("{0} - {1}", z.DisplayName, z.RegistryPath)));
        }

        private void ClipboardCopyUninstallString(object x, EventArgs y)
        {
            ImportExport.CopyToClipboard(
                _listView.SelectedUninstallers.Select(z => string.Format("{0} - {1}", z.DisplayName, z.UninstallString)));
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
                MessageBoxes.ExportFailed(ex.Message);
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
            OpenUrls(new[] { new Uri(@"https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=TB9DA2P8KQX52") });
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void exportDialog_FileOk(object sender, CancelEventArgs e)
        {
            if (!_uninstaller.ExportUninstallers(_listView.SelectedUninstallers, exportDialog.FileName))
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
                _setMan.Selected.Settings.MiscFeedbackNagShown || !WindowsTools.IsNetworkAvailable())
                return;

            _setMan.Selected.Settings.MiscFeedbackNagShown = true;

            //TODO better feedback submit window
            switch (MessageBoxes.AskToSubmitFeedback())
            {
                    case CustomMessageBox.PressedButton.Left:
                    PremadeDialogs.ProcessStartSafe(@"https://sourceforge.net/projects/bulk-crap-uninstaller/reviews/new");
                    break;

                case CustomMessageBox.PressedButton.Middle:
                    OpenSubmitFeedbackWindow(sender, e);
                    break;
            }
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            /*
            if (Width < 400 || Height < 370)
            {
                splitContainer1.Panel1Collapsed = true;
                displaySettingsToolStripMenuItem.Enabled = false;
            }
            else
            {
                splitContainer1.Panel1Collapsed = !displaySettingsToolStripMenuItem.Checked;
                displaySettingsToolStripMenuItem.Enabled = true;
            }

            if (Height < 300)
            {
                menuStrip.Visible = false;
                toolStrip.Visible = false;
                displayToolbarToolStripMenuItem.Enabled = false;
            }
            else
            {
                menuStrip.Visible = true;
                toolStrip.Visible = displayToolbarToolStripMenuItem.Checked;
                displayToolbarToolStripMenuItem.Enabled = true;
            }*/
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            _setMan.Selected.SendUpdates();

            try
            {
                ResumeLayout();
            }
            // BUG Can throw on some systems, not sure what is causing it
            catch (ObjectDisposedException)
            {
                Application.DoEvents();
                // BUG Still throws? Object list view is the culprit
                ResumeLayout();
            }

            listLegend1.Top = listViewPanel.Height + listViewPanel.Top - listLegend1.Height - 30;
            listLegend1.Left = listViewPanel.Width + listViewPanel.Left - listLegend1.Width - 30;

            Opacity = 1;

            _listView.InitiateListRefresh();

            if (_setMan.Selected.Settings.MiscFirstRun)
            {
                // Run the welcome wizard at first start of the application
                OnFirstApplicationStart();
            }
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
            using (var wizard = new FirstStartBox())
            {
                wizard.ShowDialog();
            }

            BackgroundSearchForUpdates();
        }

        private void OpenAssociatedWebPage(object sender, EventArgs eventArgs)
        {
            var urls = _listView.SelectedUninstallers.Select(y => y.GetUri()).Where(x => x != null).ToList();

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
                    WindowsTools.OpenRegKeyInRegedit(_listView.SelectedUninstallers.First().RegistryPath);
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
            PremadeDialogs.ProcessStartSafe(@"http://klocmansoftware.weebly.com/feedback--contact.html");

            /*if (WindowsTools.IsNetworkAvailable())
            {
                FeedbackWindow.ShowFeedbackDialog();
                _setMan.Selected.Settings.MiscFeedbackNagShown = true;
            }
            else
                MessageBoxes.NoNetworkConnected();*/
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
            _listView.OpenUninstallLists();
        }

        private void RefreshList(object sender, EventArgs e)
        {
            _listView.RefreshList();
        }

        private void RefreshStatusbarTotalLabel(object sender, EventArgs e)
        {
            toolStripLabelTotal.Text = string.Format(Localisable.MainWindow_Statusbar_Total,
                _listView.FilteredUninstallers.Count(), _listView.GetFilteredSize());
        }

        private void ReloadUninstallers(object sender, EventArgs e)
        {
            _listView.InitiateListRefresh();
        }

        private void removeMalwareSpyBotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenUrls(new[] { new Uri(@"https://www.safer-networking.org/", UriKind.Absolute) });
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
            if (StringEditBox.ShowDialog(string.Format(Localisable.MainWindow_Rename_Description, selected.DisplayName),
                Localisable.MainWindow_Rename_Title, selected.DisplayName, Buttons.ButtonOk, Buttons.ButtonCancel,
                out output))
            {
                if (selected.Rename(output))
                {
                    _listView.InitiateListRefresh();
                }
                else
                {
                    MessageBoxes.InvalidNewEntryName();
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
            var nonQuiet =
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
            }
        }

        private void SaveUninstallList(object sender, EventArgs e)
        {
            _listView.SaveUninstallList();
        }

        private void SearchOnlineForSelection(object sender, EventArgs e)
        {
            var items = _listView.SelectedUninstallers.Where(x => x.DisplayName.IsNotEmpty())
                .Select(y => string.Concat(@"https://www.google.com/search?q=", y.DisplayName.Replace(' ', '+')))
                .ToList();

            if (WindowsTools.IsNetworkAvailable())
            {
                if (MessageBoxes.SearchOnlineMessageBox(items.Count) == MessageBoxes.PressedButton.Yes)
                {
                    try
                    {
                        items.ForEach(x => Process.Start(x));
                    }
                    catch (Exception ex)
                    {
                        MessageBoxes.SearchOnlineError(ex);
                    }
                }
            }
            else
                MessageBoxes.NoNetworkConnected();
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
            globalHotkeys1.Add(new HotkeyEntry(Keys.S, false, true, false, saveUninstallerListToolStripMenuItem));
            globalHotkeys1.Add(new HotkeyEntry(Keys.F4, true, false, false, exitToolStripMenuItem));

            // View
            globalHotkeys1.Add(new HotkeyEntry(Keys.F, false, true, false, searchToolStripMenuItem_Click, null));
            globalHotkeys1.Add(new HotkeyEntry(Keys.F3, searchToolStripMenuItem));

            // Basic operations
            globalHotkeys1.Add(new HotkeyEntry(Keys.Delete, uninstallToolStripMenuItem));
            globalHotkeys1.Add(new HotkeyEntry(Keys.Delete, false, false, true, quietUninstallToolStripMenuItem,
                () => uninstallerObjectListView.ContainsFocus));
            globalHotkeys1.Add(new HotkeyEntry(Keys.C, false, true, false, copyFullInformationToolStripMenuItem,
                () => uninstallerObjectListView.ContainsFocus));
            globalHotkeys1.Add(new HotkeyEntry(Keys.Enter, true, false, false, propertiesToolStripMenuItem,
                () => uninstallerObjectListView.ContainsFocus));

            // Advanced operations
            globalHotkeys1.Add(new HotkeyEntry(Keys.Delete, false, true, true, manualUninstallToolStripMenuItem,
                () => uninstallerObjectListView.ContainsFocus));
            globalHotkeys1.Add(new HotkeyEntry(Keys.B, false, true, false, createBackupToolStripMenuItem,
                () => uninstallerObjectListView.ContainsFocus));
            globalHotkeys1.Add(new HotkeyEntry(Keys.R, false, true, false, openKeyInRegeditToolStripMenuItem,
                () => uninstallerObjectListView.ContainsFocus));

            // Tools
            globalHotkeys1.Add(new HotkeyEntry(Keys.P, false, true, false, settingsToolStripMenuItem_Click,
                settingsToolStripMenuItem));
        }

        private void StartSetupWizard(object sender, EventArgs e)
        {
            using (var wizard = new FirstStartBox())
            {
                wizard.ShowDialog();
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
                ? string.Format(Localisable.MainWindow_Statusbar_StatusSelection, _listView.SelectedUninstallerCount)
                : string.Empty;

            toolStripLabelSize.Text = _listView.GetSelectedSize().ToString();

            // Disable/enable edit menus
            var anySelected = _listView.SelectedUninstallerCount > 0;
            basicOperationsToolStripMenuItem.Enabled = anySelected;
            advancedOperationsToolStripMenuItem.Enabled = anySelected;
        }

        private void updateApplicationsNiniteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenUrls(new[] { new Uri(@"https://ninite.com/", UriKind.Absolute) });
        }

        private void UpdateUninstallListContextMenuStrip(object sender, CancelEventArgs e)
        {
            if (_listView.SelectedUninstallerCount == 0)
            {
                e.Cancel = true;
                return;
            }

            var singleItem = _listView.SelectedUninstallerCount == 1;

            uninstallUsingMsiExecContextMenuStripItem.Enabled =
                singleItem && !_listView.SelectedUninstallers.First().BundleProviderKey.IsEmpty();

            var toolStripItemsToDisable = new[]
            {
                uninstallContextMenuStripItem,
                quietUninstallContextMenuStripItem,
                gUIDProductCodeCopyContextMenuStripItem,
                uninstallStringCopyContextMenuStripItem,
                installLocationOpenInExplorerContextMenuStripItem,
                uninstallerLocationOpenInExplorerContextMenuStripItem,
                sourceLocationOpenInExplorerContextMenuStripItem,
                openWebPageContextMenuStripItem
            };
            toolStripItemsToDisable.ForEach(x => x.Enabled = false);

            foreach (var item in _listView.SelectedUninstallers)
            {
                if (item.IsValid)
                {
                    if (item.UninstallPossible) uninstallContextMenuStripItem.Enabled = true;
                    if (item.QuietUninstallPossible) quietUninstallContextMenuStripItem.Enabled = true;
                }

                //if (item.UninstallPossible) copyToClipboardContextMenuStripItem.Enabled = true;
                //if (item.UninstallPossible) fullInformationCopyContextMenuStripItem.Enabled = true;
                //if (item.UninstallPossible) programNameCopyContextMenuStripItem.Enabled = true;
                if (!item.BundleProviderKey.IsEmpty()) gUIDProductCodeCopyContextMenuStripItem.Enabled = true;
                //if (item.UninstallPossible) fullRegistryPathCopyContextMenuStripItem.Enabled = true;
                if (item.UninstallPossible) uninstallStringCopyContextMenuStripItem.Enabled = true;

                //if () deleteRegistryEntryContextMenuStripItem.Enabled = true;
                //if (item.UninstallPossible) renameContextMenuStripItem.Enabled = true;

                if (item.InstallLocation.IsNotEmpty()) installLocationOpenInExplorerContextMenuStripItem.Enabled = true;
                if (item.UninstallerLocation.IsNotEmpty())
                    uninstallerLocationOpenInExplorerContextMenuStripItem.Enabled = true;
                if (item.InstallSource.IsNotEmpty()) sourceLocationOpenInExplorerContextMenuStripItem.Enabled = true;

                if (item.AboutUrl.IsNotEmpty()) openWebPageContextMenuStripItem.Enabled = true;
                //if (item.UninstallPossible) propertiesContextMenuStripItem.Enabled = true;
            }

            openInExplorerContextMenuStripItem.Enabled = (installLocationOpenInExplorerContextMenuStripItem.Enabled ||
                                                          uninstallerLocationOpenInExplorerContextMenuStripItem.Enabled ||
                                                          sourceLocationOpenInExplorerContextMenuStripItem.Enabled);
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
            if (e.FirstRefresh && !e.NewValue)
            {
                _setMan.LoadSorting();
                _listView.ListRefreshIsRunningChanged -= _listView_ListRefreshIsRunningChanged;
            }
        }

        private void openStartupManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var results = StartupManagerWindow.ShowManagerDialog(this);
            toolStripLabelStatus.Text = Localisable.MainWindow_Statusbar_RefreshingStartup;
            Application.DoEvents();
            _listView.ReassignStartupEntries(true, results);
            toolStripLabelStatus.Text = string.Empty;
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
    }
}