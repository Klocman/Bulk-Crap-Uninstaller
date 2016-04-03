using System.ComponentModel;
using System.Windows.Forms;
using BrightIdeasSoftware;
using BulkCrapUninstaller.Controls;

namespace BulkCrapUninstaller.Forms
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;
        
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.advancedFilters1 = new BulkCrapUninstaller.Controls.AdvancedFilters();
            this.listLegend1 = new BulkCrapUninstaller.Controls.ListLegend();
            this.listViewPanel = new System.Windows.Forms.Panel();
            this.uninstallerObjectListView = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnDisplayName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnPublisher = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnRating = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnDisplayVersion = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnInstallDate = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnSize = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnStartup = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnIs64 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnUninstallString = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnAbout = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnInstallSource = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnInstallLocation = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnUninstallerKind = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnSystemComponent = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnProtected = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnRegistryKeyName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnGuid = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnQuietUninstallString = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSelAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSelNone = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSelInv = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonWindowSearcher = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonUninstall = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonProperties = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton7 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton8 = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripLabelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripLabelSize = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripLabelTotal = new System.Windows.Forms.ToolStripStatusLabel();
            this.facebookStatusButton1 = new Klocman.Controls.FacebookStatusButton();
            this.twitterStatusButton1 = new Klocman.Controls.TwitterStatusButton();
            this.donateButton = new System.Windows.Forms.ToolStripStatusLabel();
            this.settingsSidebarPanel = new System.Windows.Forms.Panel();
            this.propertiesSidebar = new BulkCrapUninstaller.Controls.PropertiesSidebar();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonAdvFiltering = new System.Windows.Forms.Button();
            this.filterEditor1 = new UninstallTools.Controls.FilterEditor();
            this.uninstallListContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.uninstallContextMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quietUninstallContextMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manualUninstallToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.uninstallUsingMsiExecContextMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.msiInstallContextMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.msiUninstallContextMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.msiQuietUninstallContextMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToClipboardContextMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.fullInformationCopyContextMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.programNameCopyContextMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gUIDProductCodeCopyContextMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullRegistryPathCopyContextMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uninstallStringCopyContextMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteRegistryEntryContextMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameContextMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.openInExplorerContextMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.installLocationOpenInExplorerContextMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uninstallerLocationOpenInExplorerContextMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sourceLocationOpenInExplorerContextMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openWebPageContextMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lookUpOnlineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.propertiesContextMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportDialog = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadUninstallersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.loadUninstallerListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showColorLegendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayToolbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayStatusbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displaySettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.useSystemThemeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.addWindowsFeaturesToTheListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewWindowsStoreAppsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findByWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.basicOperationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uninstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quietUninstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedClipCopyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.copyFullInformationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripMenuItem();
            this.onlineSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rateToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedOperationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manualUninstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.msiUninstalltoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableAutostartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.createBackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openKeyInRegeditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanUpProgramFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openStartupManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.openProgramsAndFeaturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanUpTheSystemCCleanerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateApplicationsNiniteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startSetupWizardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.submitFeedbackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.resetSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uninstallBCUninstallToolstripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createBackupFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.usageTracker = new Klocman.Subsystems.Tracking.UsageTracker();
            this.globalHotkeys1 = new Klocman.Subsystems.GlobalHotkeys();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.listViewPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uninstallerObjectListView)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.settingsSidebarPanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.uninstallListContextMenuStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            resources.ApplyResources(this.splitContainer1.Panel1, "splitContainer1.Panel1");
            this.splitContainer1.Panel1.Controls.Add(this.advancedFilters1);
            this.toolTip1.SetToolTip(this.splitContainer1.Panel1, resources.GetString("splitContainer1.Panel1.ToolTip"));
            // 
            // splitContainer1.Panel2
            // 
            resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
            this.splitContainer1.Panel2.Controls.Add(this.listLegend1);
            this.splitContainer1.Panel2.Controls.Add(this.listViewPanel);
            this.splitContainer1.Panel2.Controls.Add(this.toolStrip);
            this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
            this.toolTip1.SetToolTip(this.splitContainer1.Panel2, resources.GetString("splitContainer1.Panel2.ToolTip"));
            this.toolTip1.SetToolTip(this.splitContainer1, resources.GetString("splitContainer1.ToolTip"));
            // 
            // advancedFilters1
            // 
            resources.ApplyResources(this.advancedFilters1, "advancedFilters1");
            this.advancedFilters1.Name = "advancedFilters1";
            this.toolTip1.SetToolTip(this.advancedFilters1, resources.GetString("advancedFilters1.ToolTip"));
            // 
            // listLegend1
            // 
            resources.ApplyResources(this.listLegend1, "listLegend1");
            this.listLegend1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.listLegend1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listLegend1.CertificatesEnabled = true;
            this.listLegend1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.listLegend1.InvalidEnabled = true;
            this.listLegend1.Name = "listLegend1";
            this.listLegend1.OrphanedEnabled = true;
            this.listLegend1.StoreAppEnabled = true;
            this.toolTip1.SetToolTip(this.listLegend1, resources.GetString("listLegend1.ToolTip"));
            this.listLegend1.WinFeatureEnabled = true;
            // 
            // listViewPanel
            // 
            resources.ApplyResources(this.listViewPanel, "listViewPanel");
            this.listViewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewPanel.Controls.Add(this.uninstallerObjectListView);
            this.listViewPanel.Name = "listViewPanel";
            this.toolTip1.SetToolTip(this.listViewPanel, resources.GetString("listViewPanel.ToolTip"));
            // 
            // uninstallerObjectListView
            // 
            resources.ApplyResources(this.uninstallerObjectListView, "uninstallerObjectListView");
            this.uninstallerObjectListView.AllColumns.Add(this.olvColumnDisplayName);
            this.uninstallerObjectListView.AllColumns.Add(this.olvColumnPublisher);
            this.uninstallerObjectListView.AllColumns.Add(this.olvColumnRating);
            this.uninstallerObjectListView.AllColumns.Add(this.olvColumnDisplayVersion);
            this.uninstallerObjectListView.AllColumns.Add(this.olvColumnInstallDate);
            this.uninstallerObjectListView.AllColumns.Add(this.olvColumnSize);
            this.uninstallerObjectListView.AllColumns.Add(this.olvColumnStartup);
            this.uninstallerObjectListView.AllColumns.Add(this.olvColumnIs64);
            this.uninstallerObjectListView.AllColumns.Add(this.olvColumnUninstallString);
            this.uninstallerObjectListView.AllColumns.Add(this.olvColumnAbout);
            this.uninstallerObjectListView.AllColumns.Add(this.olvColumnInstallSource);
            this.uninstallerObjectListView.AllColumns.Add(this.olvColumnInstallLocation);
            this.uninstallerObjectListView.AllColumns.Add(this.olvColumnUninstallerKind);
            this.uninstallerObjectListView.AllColumns.Add(this.olvColumnSystemComponent);
            this.uninstallerObjectListView.AllColumns.Add(this.olvColumnProtected);
            this.uninstallerObjectListView.AllColumns.Add(this.olvColumnRegistryKeyName);
            this.uninstallerObjectListView.AllColumns.Add(this.olvColumnGuid);
            this.uninstallerObjectListView.AllColumns.Add(this.olvColumnQuietUninstallString);
            this.uninstallerObjectListView.AllowColumnReorder = true;
            this.uninstallerObjectListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.uninstallerObjectListView.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.uninstallerObjectListView.CellEditUseWholeCell = false;
            this.uninstallerObjectListView.CheckBoxes = true;
            this.uninstallerObjectListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnDisplayName,
            this.olvColumnPublisher,
            this.olvColumnRating,
            this.olvColumnDisplayVersion,
            this.olvColumnInstallDate,
            this.olvColumnSize,
            this.olvColumnStartup,
            this.olvColumnIs64,
            this.olvColumnUninstallString,
            this.olvColumnAbout,
            this.olvColumnInstallSource,
            this.olvColumnInstallLocation,
            this.olvColumnUninstallerKind,
            this.olvColumnSystemComponent,
            this.olvColumnProtected,
            this.olvColumnRegistryKeyName,
            this.olvColumnGuid,
            this.olvColumnQuietUninstallString});
            this.uninstallerObjectListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.uninstallerObjectListView.FullRowSelect = true;
            this.uninstallerObjectListView.GridLines = true;
            this.uninstallerObjectListView.HideSelection = false;
            this.uninstallerObjectListView.HighlightBackgroundColor = System.Drawing.Color.Empty;
            this.uninstallerObjectListView.HighlightForegroundColor = System.Drawing.Color.Empty;
            this.uninstallerObjectListView.Name = "uninstallerObjectListView";
            this.uninstallerObjectListView.OverlayText.Text = resources.GetString("resource.Text");
            this.uninstallerObjectListView.ShowGroups = false;
            this.uninstallerObjectListView.ShowImagesOnSubItems = true;
            this.uninstallerObjectListView.ShowItemToolTips = true;
            this.uninstallerObjectListView.SortGroupItemsByPrimaryColumn = false;
            this.toolTip1.SetToolTip(this.uninstallerObjectListView, resources.GetString("uninstallerObjectListView.ToolTip"));
            this.uninstallerObjectListView.UseCompatibleStateImageBehavior = false;
            this.uninstallerObjectListView.UseFilterIndicator = true;
            this.uninstallerObjectListView.UseHyperlinks = true;
            this.uninstallerObjectListView.View = System.Windows.Forms.View.Details;
            this.uninstallerObjectListView.VirtualMode = true;
            this.uninstallerObjectListView.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.uninstallerObjectListView_CellEditStarting);
            this.uninstallerObjectListView.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.uninstallerObjectListView_CellRightClick);
            this.uninstallerObjectListView.ItemsChanged += new System.EventHandler<BrightIdeasSoftware.ItemsChangedEventArgs>(this.RefreshStatusbarTotalLabel);
            this.uninstallerObjectListView.SelectionChanged += new System.EventHandler(this.uninstallerObjectListView_SelectedChanged);
            this.uninstallerObjectListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.uninstallerObjectListView_SelectedChanged);
            this.uninstallerObjectListView.Click += new System.EventHandler(this.uninstallerObjectListView_Click);
            this.uninstallerObjectListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.uninstallerObjectListView_KeyDown);
            this.uninstallerObjectListView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.HandleListViewMenuKeystroke);
            // 
            // olvColumnDisplayName
            // 
            resources.ApplyResources(this.olvColumnDisplayName, "olvColumnDisplayName");
            this.olvColumnDisplayName.Hideable = false;
            // 
            // olvColumnPublisher
            // 
            resources.ApplyResources(this.olvColumnPublisher, "olvColumnPublisher");
            // 
            // olvColumnRating
            // 
            resources.ApplyResources(this.olvColumnRating, "olvColumnRating");
            this.olvColumnRating.IsEditable = false;
            this.olvColumnRating.MaximumWidth = 80;
            this.olvColumnRating.MinimumWidth = 80;
            this.olvColumnRating.Searchable = false;
            this.olvColumnRating.UseFiltering = false;
            // 
            // olvColumnDisplayVersion
            // 
            resources.ApplyResources(this.olvColumnDisplayVersion, "olvColumnDisplayVersion");
            // 
            // olvColumnInstallDate
            // 
            resources.ApplyResources(this.olvColumnInstallDate, "olvColumnInstallDate");
            this.olvColumnInstallDate.IsEditable = false;
            // 
            // olvColumnSize
            // 
            resources.ApplyResources(this.olvColumnSize, "olvColumnSize");
            this.olvColumnSize.Searchable = false;
            // 
            // olvColumnStartup
            // 
            this.olvColumnStartup.AspectName = "";
            resources.ApplyResources(this.olvColumnStartup, "olvColumnStartup");
            // 
            // olvColumnIs64
            // 
            this.olvColumnIs64.AspectName = "Is64Bit";
            resources.ApplyResources(this.olvColumnIs64, "olvColumnIs64");
            // 
            // olvColumnUninstallString
            // 
            resources.ApplyResources(this.olvColumnUninstallString, "olvColumnUninstallString");
            // 
            // olvColumnAbout
            // 
            resources.ApplyResources(this.olvColumnAbout, "olvColumnAbout");
            this.olvColumnAbout.Hyperlink = true;
            this.olvColumnAbout.IsEditable = false;
            // 
            // olvColumnInstallSource
            // 
            resources.ApplyResources(this.olvColumnInstallSource, "olvColumnInstallSource");
            // 
            // olvColumnInstallLocation
            // 
            resources.ApplyResources(this.olvColumnInstallLocation, "olvColumnInstallLocation");
            // 
            // olvColumnUninstallerKind
            // 
            resources.ApplyResources(this.olvColumnUninstallerKind, "olvColumnUninstallerKind");
            // 
            // olvColumnSystemComponent
            // 
            resources.ApplyResources(this.olvColumnSystemComponent, "olvColumnSystemComponent");
            // 
            // olvColumnProtected
            // 
            this.olvColumnProtected.AspectName = "IsProtected";
            resources.ApplyResources(this.olvColumnProtected, "olvColumnProtected");
            // 
            // olvColumnRegistryKeyName
            // 
            resources.ApplyResources(this.olvColumnRegistryKeyName, "olvColumnRegistryKeyName");
            // 
            // olvColumnGuid
            // 
            resources.ApplyResources(this.olvColumnGuid, "olvColumnGuid");
            // 
            // olvColumnQuietUninstallString
            // 
            resources.ApplyResources(this.olvColumnQuietUninstallString, "olvColumnQuietUninstallString");
            // 
            // toolStrip
            // 
            resources.ApplyResources(this.toolStrip, "toolStrip");
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(22, 22);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripSeparator22,
            this.toolStripButtonSelAll,
            this.toolStripButtonSelNone,
            this.toolStripButtonSelInv,
            this.toolStripSeparator23,
            this.toolStripButtonWindowSearcher,
            this.toolStripSeparator21,
            this.toolStripButtonUninstall,
            this.toolStripButton2,
            this.toolStripSeparator4,
            this.toolStripButtonProperties,
            this.toolStripSeparator24,
            this.toolStripButton7,
            this.toolStripButton8});
            this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.TabStop = true;
            this.toolTip1.SetToolTip(this.toolStrip, resources.GetString("toolStrip.ToolTip"));
            // 
            // toolStripButton1
            // 
            resources.ApplyResources(this.toolStripButton1, "toolStripButton1");
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Click += new System.EventHandler(this.ReloadUninstallers);
            // 
            // toolStripSeparator22
            // 
            resources.ApplyResources(this.toolStripSeparator22, "toolStripSeparator22");
            this.toolStripSeparator22.Name = "toolStripSeparator22";
            // 
            // toolStripButtonSelAll
            // 
            resources.ApplyResources(this.toolStripButtonSelAll, "toolStripButtonSelAll");
            this.toolStripButtonSelAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSelAll.Name = "toolStripButtonSelAll";
            // 
            // toolStripButtonSelNone
            // 
            resources.ApplyResources(this.toolStripButtonSelNone, "toolStripButtonSelNone");
            this.toolStripButtonSelNone.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSelNone.Name = "toolStripButtonSelNone";
            // 
            // toolStripButtonSelInv
            // 
            resources.ApplyResources(this.toolStripButtonSelInv, "toolStripButtonSelInv");
            this.toolStripButtonSelInv.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSelInv.Name = "toolStripButtonSelInv";
            // 
            // toolStripSeparator23
            // 
            resources.ApplyResources(this.toolStripSeparator23, "toolStripSeparator23");
            this.toolStripSeparator23.Name = "toolStripSeparator23";
            // 
            // toolStripButtonWindowSearcher
            // 
            resources.ApplyResources(this.toolStripButtonWindowSearcher, "toolStripButtonWindowSearcher");
            this.toolStripButtonWindowSearcher.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonWindowSearcher.Image = global::BulkCrapUninstaller.Properties.Resources.centerline;
            this.toolStripButtonWindowSearcher.Name = "toolStripButtonWindowSearcher";
            this.toolStripButtonWindowSearcher.Click += new System.EventHandler(this.OpenWindowSearcher);
            // 
            // toolStripSeparator21
            // 
            resources.ApplyResources(this.toolStripSeparator21, "toolStripSeparator21");
            this.toolStripSeparator21.Name = "toolStripSeparator21";
            // 
            // toolStripButtonUninstall
            // 
            resources.ApplyResources(this.toolStripButtonUninstall, "toolStripButtonUninstall");
            this.toolStripButtonUninstall.Name = "toolStripButtonUninstall";
            this.toolStripButtonUninstall.Click += new System.EventHandler(this.RunLoudUninstall);
            // 
            // toolStripButton2
            // 
            resources.ApplyResources(this.toolStripButton2, "toolStripButton2");
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Click += new System.EventHandler(this.RunQuietUninstall);
            // 
            // toolStripSeparator4
            // 
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            // 
            // toolStripButtonProperties
            // 
            resources.ApplyResources(this.toolStripButtonProperties, "toolStripButtonProperties");
            this.toolStripButtonProperties.Image = global::BulkCrapUninstaller.Properties.Resources.properties;
            this.toolStripButtonProperties.Name = "toolStripButtonProperties";
            this.toolStripButtonProperties.Click += new System.EventHandler(this.OpenProperties);
            // 
            // toolStripSeparator24
            // 
            resources.ApplyResources(this.toolStripSeparator24, "toolStripSeparator24");
            this.toolStripSeparator24.Name = "toolStripSeparator24";
            // 
            // toolStripButton7
            // 
            resources.ApplyResources(this.toolStripButton7, "toolStripButton7");
            this.toolStripButton7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton7.Image = global::BulkCrapUninstaller.Properties.Resources.settings;
            this.toolStripButton7.Name = "toolStripButton7";
            this.toolStripButton7.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // toolStripButton8
            // 
            resources.ApplyResources(this.toolStripButton8, "toolStripButton8");
            this.toolStripButton8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton8.Image = global::BulkCrapUninstaller.Properties.Resources.information_circle;
            this.toolStripButton8.Name = "toolStripButton8";
            this.toolStripButton8.Click += new System.EventHandler(this.openHelpToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabelStatus,
            this.toolStripLabelSize,
            this.toolStripLabelTotal,
            this.facebookStatusButton1,
            this.twitterStatusButton1,
            this.donateButton});
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.toolTip1.SetToolTip(this.statusStrip1, resources.GetString("statusStrip1.ToolTip"));
            // 
            // toolStripLabelStatus
            // 
            resources.ApplyResources(this.toolStripLabelStatus, "toolStripLabelStatus");
            this.toolStripLabelStatus.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripLabelStatus.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripLabelStatus.Name = "toolStripLabelStatus";
            this.toolStripLabelStatus.Spring = true;
            this.toolStripLabelStatus.TextChanged += new System.EventHandler(this.toolStripLabelStatus_TextChanged);
            // 
            // toolStripLabelSize
            // 
            resources.ApplyResources(this.toolStripLabelSize, "toolStripLabelSize");
            this.toolStripLabelSize.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripLabelSize.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripLabelSize.Name = "toolStripLabelSize";
            // 
            // toolStripLabelTotal
            // 
            resources.ApplyResources(this.toolStripLabelTotal, "toolStripLabelTotal");
            this.toolStripLabelTotal.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripLabelTotal.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripLabelTotal.Name = "toolStripLabelTotal";
            // 
            // facebookStatusButton1
            // 
            resources.ApplyResources(this.facebookStatusButton1, "facebookStatusButton1");
            this.facebookStatusButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.facebookStatusButton1.IsLink = true;
            this.facebookStatusButton1.Name = "facebookStatusButton1";
            this.facebookStatusButton1.Padding = new System.Windows.Forms.Padding(2, 0, 3, 0);
            this.facebookStatusButton1.TargetSite = null;
            // 
            // twitterStatusButton1
            // 
            resources.ApplyResources(this.twitterStatusButton1, "twitterStatusButton1");
            this.twitterStatusButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.twitterStatusButton1.IsLink = true;
            this.twitterStatusButton1.MessageText = null;
            this.twitterStatusButton1.Name = "twitterStatusButton1";
            this.twitterStatusButton1.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.twitterStatusButton1.TargetSite = null;
            // 
            // donateButton
            // 
            resources.ApplyResources(this.donateButton, "donateButton");
            this.donateButton.BackgroundImage = global::BulkCrapUninstaller.Properties.Resources.donate_button;
            this.donateButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.donateButton.IsLink = true;
            this.donateButton.Margin = new System.Windows.Forms.Padding(0, 3, 3, 2);
            this.donateButton.Name = "donateButton";
            this.donateButton.Click += new System.EventHandler(this.donateButton_Click);
            // 
            // settingsSidebarPanel
            // 
            resources.ApplyResources(this.settingsSidebarPanel, "settingsSidebarPanel");
            this.settingsSidebarPanel.Controls.Add(this.propertiesSidebar);
            this.settingsSidebarPanel.Controls.Add(this.label1);
            this.settingsSidebarPanel.Controls.Add(this.groupBox1);
            this.settingsSidebarPanel.Name = "settingsSidebarPanel";
            this.toolTip1.SetToolTip(this.settingsSidebarPanel, resources.GetString("settingsSidebarPanel.ToolTip"));
            // 
            // propertiesSidebar
            // 
            resources.ApplyResources(this.propertiesSidebar, "propertiesSidebar");
            this.propertiesSidebar.InvalidEnabled = true;
            this.propertiesSidebar.Name = "propertiesSidebar";
            this.propertiesSidebar.OrphansEnabled = true;
            this.propertiesSidebar.ProtectedEnabled = true;
            this.propertiesSidebar.StoreAppsEnabled = true;
            this.propertiesSidebar.SysCompEnabled = true;
            this.toolTip1.SetToolTip(this.propertiesSidebar, resources.GetString("propertiesSidebar.ToolTip"));
            this.propertiesSidebar.UpdatesEnabled = true;
            this.propertiesSidebar.WinFeaturesEnabled = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.buttonAdvFiltering);
            this.groupBox1.Controls.Add(this.filterEditor1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // buttonAdvFiltering
            // 
            resources.ApplyResources(this.buttonAdvFiltering, "buttonAdvFiltering");
            this.buttonAdvFiltering.Name = "buttonAdvFiltering";
            this.toolTip1.SetToolTip(this.buttonAdvFiltering, resources.GetString("buttonAdvFiltering.ToolTip"));
            this.buttonAdvFiltering.UseVisualStyleBackColor = true;
            this.buttonAdvFiltering.Click += new System.EventHandler(this.buttonAdvFiltering_Click);
            // 
            // filterEditor1
            // 
            resources.ApplyResources(this.filterEditor1, "filterEditor1");
            this.filterEditor1.Name = "filterEditor1";
            this.filterEditor1.ShowAsSearch = true;
            this.toolTip1.SetToolTip(this.filterEditor1, resources.GetString("filterEditor1.ToolTip"));
            // 
            // uninstallListContextMenuStrip
            // 
            resources.ApplyResources(this.uninstallListContextMenuStrip, "uninstallListContextMenuStrip");
            this.uninstallListContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uninstallContextMenuStripItem,
            this.quietUninstallContextMenuStripItem,
            this.manualUninstallToolStripMenuItem1,
            this.uninstallUsingMsiExecContextMenuStripItem,
            this.toolStripSeparator8,
            this.copyToClipboardContextMenuStripItem,
            this.deleteRegistryEntryContextMenuStripItem,
            this.renameContextMenuStripItem,
            this.toolStripSeparator6,
            this.openInExplorerContextMenuStripItem,
            this.openWebPageContextMenuStripItem,
            this.lookUpOnlineToolStripMenuItem,
            this.rateToolStripMenuItem,
            this.toolStripSeparator7,
            this.propertiesContextMenuStripItem});
            this.uninstallListContextMenuStrip.Name = "uninstallListContextMenuStrip";
            this.toolTip1.SetToolTip(this.uninstallListContextMenuStrip, resources.GetString("uninstallListContextMenuStrip.ToolTip"));
            this.uninstallListContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.UpdateUninstallListContextMenuStrip);
            // 
            // uninstallContextMenuStripItem
            // 
            resources.ApplyResources(this.uninstallContextMenuStripItem, "uninstallContextMenuStripItem");
            this.uninstallContextMenuStripItem.Name = "uninstallContextMenuStripItem";
            this.uninstallContextMenuStripItem.Click += new System.EventHandler(this.RunLoudUninstall);
            // 
            // quietUninstallContextMenuStripItem
            // 
            resources.ApplyResources(this.quietUninstallContextMenuStripItem, "quietUninstallContextMenuStripItem");
            this.quietUninstallContextMenuStripItem.Name = "quietUninstallContextMenuStripItem";
            this.quietUninstallContextMenuStripItem.Click += new System.EventHandler(this.RunQuietUninstall);
            // 
            // manualUninstallToolStripMenuItem1
            // 
            resources.ApplyResources(this.manualUninstallToolStripMenuItem1, "manualUninstallToolStripMenuItem1");
            this.manualUninstallToolStripMenuItem1.Name = "manualUninstallToolStripMenuItem1";
            this.manualUninstallToolStripMenuItem1.Click += new System.EventHandler(this.RunAdvancedUninstall);
            // 
            // uninstallUsingMsiExecContextMenuStripItem
            // 
            resources.ApplyResources(this.uninstallUsingMsiExecContextMenuStripItem, "uninstallUsingMsiExecContextMenuStripItem");
            this.uninstallUsingMsiExecContextMenuStripItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.msiInstallContextMenuStripItem,
            this.msiUninstallContextMenuStripItem,
            this.msiQuietUninstallContextMenuStripItem});
            this.uninstallUsingMsiExecContextMenuStripItem.Name = "uninstallUsingMsiExecContextMenuStripItem";
            // 
            // msiInstallContextMenuStripItem
            // 
            resources.ApplyResources(this.msiInstallContextMenuStripItem, "msiInstallContextMenuStripItem");
            this.msiInstallContextMenuStripItem.Name = "msiInstallContextMenuStripItem";
            this.msiInstallContextMenuStripItem.Click += new System.EventHandler(this.msiInstallContextMenuStripItem_Click);
            // 
            // msiUninstallContextMenuStripItem
            // 
            resources.ApplyResources(this.msiUninstallContextMenuStripItem, "msiUninstallContextMenuStripItem");
            this.msiUninstallContextMenuStripItem.Name = "msiUninstallContextMenuStripItem";
            this.msiUninstallContextMenuStripItem.Click += new System.EventHandler(this.msiUninstallContextMenuStripItem_Click);
            // 
            // msiQuietUninstallContextMenuStripItem
            // 
            resources.ApplyResources(this.msiQuietUninstallContextMenuStripItem, "msiQuietUninstallContextMenuStripItem");
            this.msiQuietUninstallContextMenuStripItem.Name = "msiQuietUninstallContextMenuStripItem";
            this.msiQuietUninstallContextMenuStripItem.Click += new System.EventHandler(this.msiQuietUninstallContextMenuStripItem_Click);
            // 
            // toolStripSeparator8
            // 
            resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            // 
            // copyToClipboardContextMenuStripItem
            // 
            resources.ApplyResources(this.copyToClipboardContextMenuStripItem, "copyToClipboardContextMenuStripItem");
            this.copyToClipboardContextMenuStripItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem9,
            this.toolStripSeparator9,
            this.fullInformationCopyContextMenuStripItem,
            this.programNameCopyContextMenuStripItem,
            this.gUIDProductCodeCopyContextMenuStripItem,
            this.fullRegistryPathCopyContextMenuStripItem,
            this.uninstallStringCopyContextMenuStripItem});
            this.copyToClipboardContextMenuStripItem.Name = "copyToClipboardContextMenuStripItem";
            // 
            // toolStripMenuItem9
            // 
            resources.ApplyResources(this.toolStripMenuItem9, "toolStripMenuItem9");
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Click += new System.EventHandler(this.OpenAdvancedClipboardCopy);
            // 
            // toolStripSeparator9
            // 
            resources.ApplyResources(this.toolStripSeparator9, "toolStripSeparator9");
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            // 
            // fullInformationCopyContextMenuStripItem
            // 
            resources.ApplyResources(this.fullInformationCopyContextMenuStripItem, "fullInformationCopyContextMenuStripItem");
            this.fullInformationCopyContextMenuStripItem.Name = "fullInformationCopyContextMenuStripItem";
            this.fullInformationCopyContextMenuStripItem.Click += new System.EventHandler(this.ClipboardCopyFullInformation);
            // 
            // programNameCopyContextMenuStripItem
            // 
            resources.ApplyResources(this.programNameCopyContextMenuStripItem, "programNameCopyContextMenuStripItem");
            this.programNameCopyContextMenuStripItem.Name = "programNameCopyContextMenuStripItem";
            this.programNameCopyContextMenuStripItem.Click += new System.EventHandler(this.ClipboardCopyProgramName);
            // 
            // gUIDProductCodeCopyContextMenuStripItem
            // 
            resources.ApplyResources(this.gUIDProductCodeCopyContextMenuStripItem, "gUIDProductCodeCopyContextMenuStripItem");
            this.gUIDProductCodeCopyContextMenuStripItem.Name = "gUIDProductCodeCopyContextMenuStripItem";
            this.gUIDProductCodeCopyContextMenuStripItem.Click += new System.EventHandler(this.ClipboardCopyGuids);
            // 
            // fullRegistryPathCopyContextMenuStripItem
            // 
            resources.ApplyResources(this.fullRegistryPathCopyContextMenuStripItem, "fullRegistryPathCopyContextMenuStripItem");
            this.fullRegistryPathCopyContextMenuStripItem.Name = "fullRegistryPathCopyContextMenuStripItem";
            this.fullRegistryPathCopyContextMenuStripItem.Click += new System.EventHandler(this.ClipboardCopyRegistryPath);
            // 
            // uninstallStringCopyContextMenuStripItem
            // 
            resources.ApplyResources(this.uninstallStringCopyContextMenuStripItem, "uninstallStringCopyContextMenuStripItem");
            this.uninstallStringCopyContextMenuStripItem.Name = "uninstallStringCopyContextMenuStripItem";
            this.uninstallStringCopyContextMenuStripItem.Click += new System.EventHandler(this.ClipboardCopyUninstallString);
            // 
            // deleteRegistryEntryContextMenuStripItem
            // 
            resources.ApplyResources(this.deleteRegistryEntryContextMenuStripItem, "deleteRegistryEntryContextMenuStripItem");
            this.deleteRegistryEntryContextMenuStripItem.Name = "deleteRegistryEntryContextMenuStripItem";
            this.deleteRegistryEntryContextMenuStripItem.Click += new System.EventHandler(this.DeleteRegistryEntries);
            // 
            // renameContextMenuStripItem
            // 
            resources.ApplyResources(this.renameContextMenuStripItem, "renameContextMenuStripItem");
            this.renameContextMenuStripItem.Name = "renameContextMenuStripItem";
            this.renameContextMenuStripItem.Click += new System.EventHandler(this.RenameEntries);
            // 
            // toolStripSeparator6
            // 
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            // 
            // openInExplorerContextMenuStripItem
            // 
            resources.ApplyResources(this.openInExplorerContextMenuStripItem, "openInExplorerContextMenuStripItem");
            this.openInExplorerContextMenuStripItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.installLocationOpenInExplorerContextMenuStripItem,
            this.uninstallerLocationOpenInExplorerContextMenuStripItem,
            this.sourceLocationOpenInExplorerContextMenuStripItem});
            this.openInExplorerContextMenuStripItem.Name = "openInExplorerContextMenuStripItem";
            // 
            // installLocationOpenInExplorerContextMenuStripItem
            // 
            resources.ApplyResources(this.installLocationOpenInExplorerContextMenuStripItem, "installLocationOpenInExplorerContextMenuStripItem");
            this.installLocationOpenInExplorerContextMenuStripItem.Name = "installLocationOpenInExplorerContextMenuStripItem";
            this.installLocationOpenInExplorerContextMenuStripItem.Click += new System.EventHandler(this.OpenInstallLocation);
            // 
            // uninstallerLocationOpenInExplorerContextMenuStripItem
            // 
            resources.ApplyResources(this.uninstallerLocationOpenInExplorerContextMenuStripItem, "uninstallerLocationOpenInExplorerContextMenuStripItem");
            this.uninstallerLocationOpenInExplorerContextMenuStripItem.Name = "uninstallerLocationOpenInExplorerContextMenuStripItem";
            this.uninstallerLocationOpenInExplorerContextMenuStripItem.Click += new System.EventHandler(this.OpenUninstallerLocation);
            // 
            // sourceLocationOpenInExplorerContextMenuStripItem
            // 
            resources.ApplyResources(this.sourceLocationOpenInExplorerContextMenuStripItem, "sourceLocationOpenInExplorerContextMenuStripItem");
            this.sourceLocationOpenInExplorerContextMenuStripItem.Name = "sourceLocationOpenInExplorerContextMenuStripItem";
            this.sourceLocationOpenInExplorerContextMenuStripItem.Click += new System.EventHandler(this.OpenInstallationSource);
            // 
            // openWebPageContextMenuStripItem
            // 
            resources.ApplyResources(this.openWebPageContextMenuStripItem, "openWebPageContextMenuStripItem");
            this.openWebPageContextMenuStripItem.Name = "openWebPageContextMenuStripItem";
            this.openWebPageContextMenuStripItem.Click += new System.EventHandler(this.OpenAssociatedWebPage);
            // 
            // lookUpOnlineToolStripMenuItem
            // 
            resources.ApplyResources(this.lookUpOnlineToolStripMenuItem, "lookUpOnlineToolStripMenuItem");
            this.lookUpOnlineToolStripMenuItem.Name = "lookUpOnlineToolStripMenuItem";
            this.lookUpOnlineToolStripMenuItem.Click += new System.EventHandler(this.SearchOnlineForSelection);
            // 
            // rateToolStripMenuItem
            // 
            resources.ApplyResources(this.rateToolStripMenuItem, "rateToolStripMenuItem");
            this.rateToolStripMenuItem.Image = global::BulkCrapUninstaller.Properties.Resources.star;
            this.rateToolStripMenuItem.Name = "rateToolStripMenuItem";
            this.rateToolStripMenuItem.Click += new System.EventHandler(this.rateToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            // 
            // propertiesContextMenuStripItem
            // 
            resources.ApplyResources(this.propertiesContextMenuStripItem, "propertiesContextMenuStripItem");
            this.propertiesContextMenuStripItem.Image = global::BulkCrapUninstaller.Properties.Resources.magnifybrowse;
            this.propertiesContextMenuStripItem.Name = "propertiesContextMenuStripItem";
            this.propertiesContextMenuStripItem.Click += new System.EventHandler(this.OpenProperties);
            // 
            // exportDialog
            // 
            this.exportDialog.DefaultExt = "txt";
            this.exportDialog.FileName = "New BCUninstaller Export";
            resources.ApplyResources(this.exportDialog, "exportDialog");
            this.exportDialog.RestoreDirectory = true;
            this.exportDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.exportDialog_FileOk);
            // 
            // menuStrip
            // 
            resources.ApplyResources(this.menuStrip, "menuStrip");
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.basicOperationsToolStripMenuItem,
            this.advancedOperationsToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.debugToolStripMenuItem});
            this.menuStrip.Name = "menuStrip";
            this.toolTip1.SetToolTip(this.menuStrip, resources.GetString("menuStrip.ToolTip"));
            // 
            // fileToolStripMenuItem
            // 
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reloadUninstallersToolStripMenuItem,
            this.toolStripSeparator1,
            this.loadUninstallerListToolStripMenuItem,
            this.exportSelectedToolStripMenuItem,
            this.toolStripSeparator10,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.DropDownOpening += new System.EventHandler(this.fileToolStripMenuItem_DropDownOpening);
            // 
            // reloadUninstallersToolStripMenuItem
            // 
            resources.ApplyResources(this.reloadUninstallersToolStripMenuItem, "reloadUninstallersToolStripMenuItem");
            this.reloadUninstallersToolStripMenuItem.Name = "reloadUninstallersToolStripMenuItem";
            this.reloadUninstallersToolStripMenuItem.Click += new System.EventHandler(this.ReloadUninstallers);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // loadUninstallerListToolStripMenuItem
            // 
            resources.ApplyResources(this.loadUninstallerListToolStripMenuItem, "loadUninstallerListToolStripMenuItem");
            this.loadUninstallerListToolStripMenuItem.Name = "loadUninstallerListToolStripMenuItem";
            this.loadUninstallerListToolStripMenuItem.Click += new System.EventHandler(this.OpenUninstallLists);
            // 
            // exportSelectedToolStripMenuItem
            // 
            resources.ApplyResources(this.exportSelectedToolStripMenuItem, "exportSelectedToolStripMenuItem");
            this.exportSelectedToolStripMenuItem.Name = "exportSelectedToolStripMenuItem";
            this.exportSelectedToolStripMenuItem.Click += new System.EventHandler(this.exportSelectedToolStripMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            resources.ApplyResources(this.toolStripSeparator10, "toolStripSeparator10");
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            // 
            // exitToolStripMenuItem
            // 
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showColorLegendToolStripMenuItem,
            this.displayToolbarToolStripMenuItem,
            this.displayStatusbarToolStripMenuItem,
            this.displaySettingsToolStripMenuItem,
            this.toolStripSeparator12,
            this.useSystemThemeToolStripMenuItem,
            this.toolStripSeparator20,
            this.addWindowsFeaturesToTheListToolStripMenuItem,
            this.viewWindowsStoreAppsToolStripMenuItem,
            this.findByWindowToolStripMenuItem,
            this.searchToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            // 
            // showColorLegendToolStripMenuItem
            // 
            resources.ApplyResources(this.showColorLegendToolStripMenuItem, "showColorLegendToolStripMenuItem");
            this.showColorLegendToolStripMenuItem.Name = "showColorLegendToolStripMenuItem";
            // 
            // displayToolbarToolStripMenuItem
            // 
            resources.ApplyResources(this.displayToolbarToolStripMenuItem, "displayToolbarToolStripMenuItem");
            this.displayToolbarToolStripMenuItem.Checked = true;
            this.displayToolbarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.displayToolbarToolStripMenuItem.Name = "displayToolbarToolStripMenuItem";
            // 
            // displayStatusbarToolStripMenuItem
            // 
            resources.ApplyResources(this.displayStatusbarToolStripMenuItem, "displayStatusbarToolStripMenuItem");
            this.displayStatusbarToolStripMenuItem.Checked = true;
            this.displayStatusbarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.displayStatusbarToolStripMenuItem.Name = "displayStatusbarToolStripMenuItem";
            // 
            // displaySettingsToolStripMenuItem
            // 
            resources.ApplyResources(this.displaySettingsToolStripMenuItem, "displaySettingsToolStripMenuItem");
            this.displaySettingsToolStripMenuItem.Checked = true;
            this.displaySettingsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.displaySettingsToolStripMenuItem.Name = "displaySettingsToolStripMenuItem";
            // 
            // toolStripSeparator12
            // 
            resources.ApplyResources(this.toolStripSeparator12, "toolStripSeparator12");
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            // 
            // useSystemThemeToolStripMenuItem
            // 
            resources.ApplyResources(this.useSystemThemeToolStripMenuItem, "useSystemThemeToolStripMenuItem");
            this.useSystemThemeToolStripMenuItem.Checked = true;
            this.useSystemThemeToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useSystemThemeToolStripMenuItem.Name = "useSystemThemeToolStripMenuItem";
            // 
            // toolStripSeparator20
            // 
            resources.ApplyResources(this.toolStripSeparator20, "toolStripSeparator20");
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            // 
            // addWindowsFeaturesToTheListToolStripMenuItem
            // 
            resources.ApplyResources(this.addWindowsFeaturesToTheListToolStripMenuItem, "addWindowsFeaturesToTheListToolStripMenuItem");
            this.addWindowsFeaturesToTheListToolStripMenuItem.Name = "addWindowsFeaturesToTheListToolStripMenuItem";
            this.addWindowsFeaturesToTheListToolStripMenuItem.Click += new System.EventHandler(this.addWindowsFeaturesToTheListToolStripMenuItem_Click);
            // 
            // viewWindowsStoreAppsToolStripMenuItem
            // 
            resources.ApplyResources(this.viewWindowsStoreAppsToolStripMenuItem, "viewWindowsStoreAppsToolStripMenuItem");
            this.viewWindowsStoreAppsToolStripMenuItem.Name = "viewWindowsStoreAppsToolStripMenuItem";
            this.viewWindowsStoreAppsToolStripMenuItem.Click += new System.EventHandler(this.viewWindowsStoreAppsToolStripMenuItem_Click);
            // 
            // findByWindowToolStripMenuItem
            // 
            resources.ApplyResources(this.findByWindowToolStripMenuItem, "findByWindowToolStripMenuItem");
            this.findByWindowToolStripMenuItem.Image = global::BulkCrapUninstaller.Properties.Resources.centerline;
            this.findByWindowToolStripMenuItem.Name = "findByWindowToolStripMenuItem";
            this.findByWindowToolStripMenuItem.Click += new System.EventHandler(this.OpenWindowSearcher);
            // 
            // searchToolStripMenuItem
            // 
            resources.ApplyResources(this.searchToolStripMenuItem, "searchToolStripMenuItem");
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Click += new System.EventHandler(this.searchToolStripMenuItem_Click);
            // 
            // basicOperationsToolStripMenuItem
            // 
            resources.ApplyResources(this.basicOperationsToolStripMenuItem, "basicOperationsToolStripMenuItem");
            this.basicOperationsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uninstallToolStripMenuItem,
            this.quietUninstallToolStripMenuItem,
            this.toolStripSeparator2,
            this.toolStripMenuItem8,
            this.toolStripMenuItem1,
            this.toolStripMenuItem14,
            this.onlineSearchToolStripMenuItem,
            this.rateToolStripMenuItem1,
            this.toolStripSeparator15,
            this.propertiesToolStripMenuItem});
            this.basicOperationsToolStripMenuItem.Name = "basicOperationsToolStripMenuItem";
            this.basicOperationsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.basicOperationsToolStripMenuItem_DropDownOpening);
            // 
            // uninstallToolStripMenuItem
            // 
            resources.ApplyResources(this.uninstallToolStripMenuItem, "uninstallToolStripMenuItem");
            this.uninstallToolStripMenuItem.Name = "uninstallToolStripMenuItem";
            this.uninstallToolStripMenuItem.Click += new System.EventHandler(this.RunLoudUninstall);
            // 
            // quietUninstallToolStripMenuItem
            // 
            resources.ApplyResources(this.quietUninstallToolStripMenuItem, "quietUninstallToolStripMenuItem");
            this.quietUninstallToolStripMenuItem.Name = "quietUninstallToolStripMenuItem";
            this.quietUninstallToolStripMenuItem.Click += new System.EventHandler(this.RunQuietUninstall);
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // toolStripMenuItem8
            // 
            resources.ApplyResources(this.toolStripMenuItem8, "toolStripMenuItem8");
            this.toolStripMenuItem8.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.advancedClipCopyToolStripMenuItem,
            this.toolStripSeparator11,
            this.copyFullInformationToolStripMenuItem,
            this.toolStripMenuItem10,
            this.toolStripMenuItem11,
            this.toolStripMenuItem12,
            this.toolStripMenuItem13});
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            // 
            // advancedClipCopyToolStripMenuItem
            // 
            resources.ApplyResources(this.advancedClipCopyToolStripMenuItem, "advancedClipCopyToolStripMenuItem");
            this.advancedClipCopyToolStripMenuItem.Name = "advancedClipCopyToolStripMenuItem";
            this.advancedClipCopyToolStripMenuItem.Click += new System.EventHandler(this.OpenAdvancedClipboardCopy);
            // 
            // toolStripSeparator11
            // 
            resources.ApplyResources(this.toolStripSeparator11, "toolStripSeparator11");
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            // 
            // copyFullInformationToolStripMenuItem
            // 
            resources.ApplyResources(this.copyFullInformationToolStripMenuItem, "copyFullInformationToolStripMenuItem");
            this.copyFullInformationToolStripMenuItem.Name = "copyFullInformationToolStripMenuItem";
            this.copyFullInformationToolStripMenuItem.Click += new System.EventHandler(this.ClipboardCopyFullInformation);
            // 
            // toolStripMenuItem10
            // 
            resources.ApplyResources(this.toolStripMenuItem10, "toolStripMenuItem10");
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Click += new System.EventHandler(this.ClipboardCopyProgramName);
            // 
            // toolStripMenuItem11
            // 
            resources.ApplyResources(this.toolStripMenuItem11, "toolStripMenuItem11");
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Click += new System.EventHandler(this.ClipboardCopyGuids);
            // 
            // toolStripMenuItem12
            // 
            resources.ApplyResources(this.toolStripMenuItem12, "toolStripMenuItem12");
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            this.toolStripMenuItem12.Click += new System.EventHandler(this.ClipboardCopyRegistryPath);
            // 
            // toolStripMenuItem13
            // 
            resources.ApplyResources(this.toolStripMenuItem13, "toolStripMenuItem13");
            this.toolStripMenuItem13.Name = "toolStripMenuItem13";
            this.toolStripMenuItem13.Click += new System.EventHandler(this.ClipboardCopyUninstallString);
            // 
            // toolStripMenuItem1
            // 
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem5,
            this.toolStripMenuItem6,
            this.toolStripMenuItem7});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            // 
            // toolStripMenuItem5
            // 
            resources.ApplyResources(this.toolStripMenuItem5, "toolStripMenuItem5");
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.OpenInstallLocation);
            // 
            // toolStripMenuItem6
            // 
            resources.ApplyResources(this.toolStripMenuItem6, "toolStripMenuItem6");
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Click += new System.EventHandler(this.OpenUninstallerLocation);
            // 
            // toolStripMenuItem7
            // 
            resources.ApplyResources(this.toolStripMenuItem7, "toolStripMenuItem7");
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.OpenInstallationSource);
            // 
            // toolStripMenuItem14
            // 
            resources.ApplyResources(this.toolStripMenuItem14, "toolStripMenuItem14");
            this.toolStripMenuItem14.Name = "toolStripMenuItem14";
            this.toolStripMenuItem14.Click += new System.EventHandler(this.OpenAssociatedWebPage);
            // 
            // onlineSearchToolStripMenuItem
            // 
            resources.ApplyResources(this.onlineSearchToolStripMenuItem, "onlineSearchToolStripMenuItem");
            this.onlineSearchToolStripMenuItem.Name = "onlineSearchToolStripMenuItem";
            this.onlineSearchToolStripMenuItem.Click += new System.EventHandler(this.SearchOnlineForSelection);
            // 
            // rateToolStripMenuItem1
            // 
            resources.ApplyResources(this.rateToolStripMenuItem1, "rateToolStripMenuItem1");
            this.rateToolStripMenuItem1.Image = global::BulkCrapUninstaller.Properties.Resources.star;
            this.rateToolStripMenuItem1.Name = "rateToolStripMenuItem1";
            this.rateToolStripMenuItem1.Click += new System.EventHandler(this.rateToolStripMenuItem_Click);
            // 
            // toolStripSeparator15
            // 
            resources.ApplyResources(this.toolStripSeparator15, "toolStripSeparator15");
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            // 
            // propertiesToolStripMenuItem
            // 
            resources.ApplyResources(this.propertiesToolStripMenuItem, "propertiesToolStripMenuItem");
            this.propertiesToolStripMenuItem.Image = global::BulkCrapUninstaller.Properties.Resources.properties;
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.OpenProperties);
            // 
            // advancedOperationsToolStripMenuItem
            // 
            resources.ApplyResources(this.advancedOperationsToolStripMenuItem, "advancedOperationsToolStripMenuItem");
            this.advancedOperationsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manualUninstallToolStripMenuItem,
            this.msiUninstalltoolStripMenuItem,
            this.toolStripSeparator14,
            this.renameToolStripMenuItem,
            this.disableAutostartToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator5,
            this.createBackupToolStripMenuItem,
            this.openKeyInRegeditToolStripMenuItem});
            this.advancedOperationsToolStripMenuItem.Name = "advancedOperationsToolStripMenuItem";
            this.advancedOperationsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.advancedOperationsToolStripMenuItem_DropDownOpening);
            // 
            // manualUninstallToolStripMenuItem
            // 
            resources.ApplyResources(this.manualUninstallToolStripMenuItem, "manualUninstallToolStripMenuItem");
            this.manualUninstallToolStripMenuItem.Name = "manualUninstallToolStripMenuItem";
            this.manualUninstallToolStripMenuItem.Click += new System.EventHandler(this.RunAdvancedUninstall);
            // 
            // msiUninstalltoolStripMenuItem
            // 
            resources.ApplyResources(this.msiUninstalltoolStripMenuItem, "msiUninstalltoolStripMenuItem");
            this.msiUninstalltoolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4});
            this.msiUninstalltoolStripMenuItem.Name = "msiUninstalltoolStripMenuItem";
            // 
            // toolStripMenuItem2
            // 
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.msiInstallContextMenuStripItem_Click);
            // 
            // toolStripMenuItem3
            // 
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.msiUninstallContextMenuStripItem_Click);
            // 
            // toolStripMenuItem4
            // 
            resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.msiQuietUninstallContextMenuStripItem_Click);
            // 
            // toolStripSeparator14
            // 
            resources.ApplyResources(this.toolStripSeparator14, "toolStripSeparator14");
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            // 
            // renameToolStripMenuItem
            // 
            resources.ApplyResources(this.renameToolStripMenuItem, "renameToolStripMenuItem");
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.RenameEntries);
            // 
            // disableAutostartToolStripMenuItem
            // 
            resources.ApplyResources(this.disableAutostartToolStripMenuItem, "disableAutostartToolStripMenuItem");
            this.disableAutostartToolStripMenuItem.Name = "disableAutostartToolStripMenuItem";
            this.disableAutostartToolStripMenuItem.Click += new System.EventHandler(this.disableAutostartToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteRegistryEntries);
            // 
            // toolStripSeparator5
            // 
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            // 
            // createBackupToolStripMenuItem
            // 
            resources.ApplyResources(this.createBackupToolStripMenuItem, "createBackupToolStripMenuItem");
            this.createBackupToolStripMenuItem.Name = "createBackupToolStripMenuItem";
            this.createBackupToolStripMenuItem.Click += new System.EventHandler(this.CreateRegistryBackup);
            // 
            // openKeyInRegeditToolStripMenuItem
            // 
            resources.ApplyResources(this.openKeyInRegeditToolStripMenuItem, "openKeyInRegeditToolStripMenuItem");
            this.openKeyInRegeditToolStripMenuItem.Name = "openKeyInRegeditToolStripMenuItem";
            this.openKeyInRegeditToolStripMenuItem.Click += new System.EventHandler(this.OpenInRegedit);
            // 
            // toolsToolStripMenuItem
            // 
            resources.ApplyResources(this.toolsToolStripMenuItem, "toolsToolStripMenuItem");
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cleanUpProgramFilesToolStripMenuItem,
            this.openStartupManagerToolStripMenuItem,
            this.toolStripSeparator13,
            this.openProgramsAndFeaturesToolStripMenuItem,
            this.cleanUpTheSystemCCleanerToolStripMenuItem,
            this.updateApplicationsNiniteToolStripMenuItem,
            this.toolStripSeparator19,
            this.settingsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.toolsToolStripMenuItem_DropDownOpening);
            // 
            // cleanUpProgramFilesToolStripMenuItem
            // 
            resources.ApplyResources(this.cleanUpProgramFilesToolStripMenuItem, "cleanUpProgramFilesToolStripMenuItem");
            this.cleanUpProgramFilesToolStripMenuItem.Name = "cleanUpProgramFilesToolStripMenuItem";
            this.cleanUpProgramFilesToolStripMenuItem.Click += new System.EventHandler(this.cleanUpProgramFilesToolStripMenuItem_Click);
            // 
            // openStartupManagerToolStripMenuItem
            // 
            resources.ApplyResources(this.openStartupManagerToolStripMenuItem, "openStartupManagerToolStripMenuItem");
            this.openStartupManagerToolStripMenuItem.Name = "openStartupManagerToolStripMenuItem";
            this.openStartupManagerToolStripMenuItem.Click += new System.EventHandler(this.openStartupManagerToolStripMenuItem_Click);
            // 
            // toolStripSeparator13
            // 
            resources.ApplyResources(this.toolStripSeparator13, "toolStripSeparator13");
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            // 
            // openProgramsAndFeaturesToolStripMenuItem
            // 
            resources.ApplyResources(this.openProgramsAndFeaturesToolStripMenuItem, "openProgramsAndFeaturesToolStripMenuItem");
            this.openProgramsAndFeaturesToolStripMenuItem.Name = "openProgramsAndFeaturesToolStripMenuItem";
            this.openProgramsAndFeaturesToolStripMenuItem.Click += new System.EventHandler(this.openProgramsAndFeaturesToolStripMenuItem_Click);
            // 
            // cleanUpTheSystemCCleanerToolStripMenuItem
            // 
            resources.ApplyResources(this.cleanUpTheSystemCCleanerToolStripMenuItem, "cleanUpTheSystemCCleanerToolStripMenuItem");
            this.cleanUpTheSystemCCleanerToolStripMenuItem.Name = "cleanUpTheSystemCCleanerToolStripMenuItem";
            this.cleanUpTheSystemCCleanerToolStripMenuItem.Click += new System.EventHandler(this.cleanUpTheSystemCCleanerToolStripMenuItem_Click);
            // 
            // updateApplicationsNiniteToolStripMenuItem
            // 
            resources.ApplyResources(this.updateApplicationsNiniteToolStripMenuItem, "updateApplicationsNiniteToolStripMenuItem");
            this.updateApplicationsNiniteToolStripMenuItem.Name = "updateApplicationsNiniteToolStripMenuItem";
            this.updateApplicationsNiniteToolStripMenuItem.Click += new System.EventHandler(this.updateApplicationsNiniteToolStripMenuItem_Click);
            // 
            // toolStripSeparator19
            // 
            resources.ApplyResources(this.toolStripSeparator19, "toolStripSeparator19");
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            // 
            // settingsToolStripMenuItem
            // 
            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openHelpToolStripMenuItem,
            this.startSetupWizardToolStripMenuItem,
            this.toolStripSeparator16,
            this.checkForUpdatesToolStripMenuItem,
            this.submitFeedbackToolStripMenuItem,
            this.toolStripSeparator18,
            this.resetSettingsToolStripMenuItem,
            this.uninstallBCUninstallToolstripMenuItem,
            this.toolStripSeparator17,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.DropDownOpening += new System.EventHandler(this.helpToolStripMenuItem_DropDownOpening);
            // 
            // openHelpToolStripMenuItem
            // 
            resources.ApplyResources(this.openHelpToolStripMenuItem, "openHelpToolStripMenuItem");
            this.openHelpToolStripMenuItem.Image = global::BulkCrapUninstaller.Properties.Resources.information_circle;
            this.openHelpToolStripMenuItem.Name = "openHelpToolStripMenuItem";
            this.openHelpToolStripMenuItem.Click += new System.EventHandler(this.openHelpToolStripMenuItem_Click);
            // 
            // startSetupWizardToolStripMenuItem
            // 
            resources.ApplyResources(this.startSetupWizardToolStripMenuItem, "startSetupWizardToolStripMenuItem");
            this.startSetupWizardToolStripMenuItem.Name = "startSetupWizardToolStripMenuItem";
            this.startSetupWizardToolStripMenuItem.Click += new System.EventHandler(this.StartSetupWizard);
            // 
            // toolStripSeparator16
            // 
            resources.ApplyResources(this.toolStripSeparator16, "toolStripSeparator16");
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            resources.ApplyResources(this.checkForUpdatesToolStripMenuItem, "checkForUpdatesToolStripMenuItem");
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdatesToolStripMenuItem_Click);
            // 
            // submitFeedbackToolStripMenuItem
            // 
            resources.ApplyResources(this.submitFeedbackToolStripMenuItem, "submitFeedbackToolStripMenuItem");
            this.submitFeedbackToolStripMenuItem.Name = "submitFeedbackToolStripMenuItem";
            this.submitFeedbackToolStripMenuItem.Click += new System.EventHandler(this.OpenSubmitFeedbackWindow);
            // 
            // toolStripSeparator18
            // 
            resources.ApplyResources(this.toolStripSeparator18, "toolStripSeparator18");
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            // 
            // resetSettingsToolStripMenuItem
            // 
            resources.ApplyResources(this.resetSettingsToolStripMenuItem, "resetSettingsToolStripMenuItem");
            this.resetSettingsToolStripMenuItem.Name = "resetSettingsToolStripMenuItem";
            this.resetSettingsToolStripMenuItem.Click += new System.EventHandler(this.ResetSettingsDialog);
            // 
            // uninstallBCUninstallToolstripMenuItem
            // 
            resources.ApplyResources(this.uninstallBCUninstallToolstripMenuItem, "uninstallBCUninstallToolstripMenuItem");
            this.uninstallBCUninstallToolstripMenuItem.Name = "uninstallBCUninstallToolstripMenuItem";
            this.uninstallBCUninstallToolstripMenuItem.Click += new System.EventHandler(this.uninstallBCUninstallToolstripMenuItem_Click);
            // 
            // toolStripSeparator17
            // 
            resources.ApplyResources(this.toolStripSeparator17, "toolStripSeparator17");
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            // 
            // aboutToolStripMenuItem
            // 
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            resources.ApplyResources(this.debugToolStripMenuItem, "debugToolStripMenuItem");
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Click += new System.EventHandler(this.OpenDebugWindow);
            // 
            // createBackupFileDialog
            // 
            this.createBackupFileDialog.DefaultExt = "reg";
            this.createBackupFileDialog.FileName = "New Uninstaller Backup";
            resources.ApplyResources(this.createBackupFileDialog, "createBackupFileDialog");
            this.createBackupFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.createBackupFileDialog_FileOk);
            // 
            // usageTracker
            // 
            this.usageTracker.ContainerControl = this;
            // 
            // globalHotkeys1
            // 
            this.globalHotkeys1.ContainerControl = this;
            this.globalHotkeys1.StopWhenFormIsDisabled = true;
            this.globalHotkeys1.SuppressKeyPresses = true;
            // 
            // MainWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.settingsSidebarPanel);
            this.Controls.Add(this.menuStrip);
            this.KeyPreview = true;
            this.Name = "MainWindow";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
            this.Shown += new System.EventHandler(this.MainWindow_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.listViewPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uninstallerObjectListView)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.settingsSidebarPanel.ResumeLayout(false);
            this.settingsSidebarPanel.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.uninstallListContextMenuStrip.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SplitContainer splitContainer1;
        internal ObjectListView uninstallerObjectListView;
        internal OLVColumn olvColumnDisplayName;
        internal OLVColumn olvColumnPublisher;
        internal OLVColumn olvColumnDisplayVersion;
        internal OLVColumn olvColumnUninstallString;
        internal OLVColumn olvColumnInstallDate;
        internal OLVColumn olvColumnIs64;
        internal OLVColumn olvColumnGuid;
        internal OLVColumn olvColumnSize;
        internal OLVColumn olvColumnInstallSource;
        internal OLVColumn olvColumnInstallLocation;
        internal OLVColumn olvColumnUninstallerKind;
        internal OLVColumn olvColumnAbout;
        internal OLVColumn olvColumnRegistryKeyName;
        internal OLVColumn olvColumnSystemComponent;
        internal OLVColumn olvColumnQuietUninstallString;
        internal OLVColumn olvColumnProtected;
        private SaveFileDialog exportDialog;
        private ContextMenuStrip uninstallListContextMenuStrip;
        private ToolStripMenuItem uninstallContextMenuStripItem;
        private ToolStripMenuItem propertiesContextMenuStripItem;
        private Panel settingsSidebarPanel;
        private MenuStrip menuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem reloadUninstallersToolStripMenuItem;
        private ToolStripMenuItem exportSelectedToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem displayToolbarToolStripMenuItem;
        private ToolStripMenuItem displaySettingsToolStripMenuItem;
        private ToolStripMenuItem basicOperationsToolStripMenuItem;
        private ToolStripMenuItem quietUninstallToolStripMenuItem;
        private ToolStripMenuItem propertiesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem openHelpToolStripMenuItem;
        private ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private ToolStripMenuItem resetSettingsToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStrip toolStrip;
        private ToolStripButton toolStripButton1;
        private ToolStripButton toolStripButtonUninstall;
        private ToolStripButton toolStripButtonProperties;
        private Label label1;
        private SaveFileDialog createBackupFileDialog;
        private ToolStripMenuItem quietUninstallContextMenuStripItem;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem copyToClipboardContextMenuStripItem;
        private ToolStripMenuItem fullInformationCopyContextMenuStripItem;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripMenuItem programNameCopyContextMenuStripItem;
        private ToolStripMenuItem gUIDProductCodeCopyContextMenuStripItem;
        private ToolStripMenuItem fullRegistryPathCopyContextMenuStripItem;
        private ToolStripMenuItem uninstallStringCopyContextMenuStripItem;
        private ToolStripMenuItem deleteRegistryEntryContextMenuStripItem;
        private ToolStripMenuItem renameContextMenuStripItem;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem openInExplorerContextMenuStripItem;
        private ToolStripMenuItem installLocationOpenInExplorerContextMenuStripItem;
        private ToolStripMenuItem uninstallerLocationOpenInExplorerContextMenuStripItem;
        private ToolStripMenuItem sourceLocationOpenInExplorerContextMenuStripItem;
        private ToolStripMenuItem openWebPageContextMenuStripItem;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripButton toolStripButton2;
        private ToolStripMenuItem uninstallUsingMsiExecContextMenuStripItem;
        private ToolStripMenuItem msiInstallContextMenuStripItem;
        private ToolStripMenuItem msiUninstallContextMenuStripItem;
        private ToolStripMenuItem msiQuietUninstallContextMenuStripItem;
        private ToolStripMenuItem uninstallToolStripMenuItem;
        private ToolStripMenuItem useSystemThemeToolStripMenuItem;
        private ToolStripMenuItem submitFeedbackToolStripMenuItem;
        private ToolTip toolTip1;
        private ToolStripMenuItem loadUninstallerListToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripMenuItem toolStripMenuItem8;
        private ToolStripMenuItem copyFullInformationToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripMenuItem toolStripMenuItem10;
        private ToolStripMenuItem toolStripMenuItem11;
        private ToolStripMenuItem toolStripMenuItem12;
        private ToolStripMenuItem toolStripMenuItem13;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem5;
        private ToolStripMenuItem toolStripMenuItem6;
        private ToolStripMenuItem toolStripMenuItem7;
        private ToolStripMenuItem toolStripMenuItem14;
        private ToolStripMenuItem advancedOperationsToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem renameToolStripMenuItem;
        private ToolStripMenuItem msiUninstalltoolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem toolStripMenuItem4;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem openKeyInRegeditToolStripMenuItem;
        private ToolStripMenuItem createBackupToolStripMenuItem;
        private ToolStripMenuItem lookUpOnlineToolStripMenuItem;
        private ToolStripMenuItem onlineSearchToolStripMenuItem;
        private PropertiesSidebar propertiesSidebar;
        private ToolStripMenuItem debugToolStripMenuItem;
        private ToolStripMenuItem uninstallBCUninstallToolstripMenuItem;
        private Klocman.Subsystems.Tracking.UsageTracker usageTracker;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripLabelStatus;
        private ToolStripStatusLabel toolStripLabelSize;
        private ToolStripStatusLabel toolStripLabelTotal;
        private ToolStripMenuItem displayStatusbarToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem openProgramsAndFeaturesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator13;
        private ToolStripMenuItem cleanUpTheSystemCCleanerToolStripMenuItem;
        private ToolStripMenuItem updateApplicationsNiniteToolStripMenuItem;
        private Klocman.Controls.FacebookStatusButton facebookStatusButton1;
        private Klocman.Controls.TwitterStatusButton twitterStatusButton1;
        private ToolStripMenuItem manualUninstallToolStripMenuItem1;
        private ToolStripMenuItem manualUninstallToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator14;
        private ToolStripSeparator toolStripSeparator15;
        private ToolStripSeparator toolStripSeparator12;
        private ToolStripSeparator toolStripSeparator16;
        private ToolStripSeparator toolStripSeparator17;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem startSetupWizardToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator18;
        private ToolStripStatusLabel donateButton;
        private ToolStripSeparator toolStripSeparator19;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem cleanUpProgramFilesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator20;
        private ToolStripMenuItem searchToolStripMenuItem;
        internal Klocman.Subsystems.GlobalHotkeys globalHotkeys1;
        private ListLegend listLegend1;
        private ToolStripMenuItem showColorLegendToolStripMenuItem;
        private GroupBox groupBox1;
        private ToolStripSeparator toolStripSeparator21;
        private ToolStripButton toolStripButtonSelAll;
        private ToolStripButton toolStripButtonSelInv;
        private ToolStripButton toolStripButtonSelNone;
        private ToolStripSeparator toolStripSeparator22;
        private ToolStripMenuItem openStartupManagerToolStripMenuItem;
        internal OLVColumn olvColumnStartup;
        private Panel listViewPanel;
        private ToolStripMenuItem disableAutostartToolStripMenuItem;
        internal OLVColumn olvColumnRating;
        private ToolStripMenuItem rateToolStripMenuItem;
        private ToolStripMenuItem rateToolStripMenuItem1;
        private ToolStripButton toolStripButtonWindowSearcher;
        private ToolStripSeparator toolStripSeparator23;
        private ToolStripMenuItem findByWindowToolStripMenuItem;
        private ToolStripMenuItem viewWindowsStoreAppsToolStripMenuItem;
        internal UninstallTools.Controls.FilterEditor filterEditor1;
        private Button buttonAdvFiltering;
        private ToolStripSeparator toolStripSeparator24;
        private ToolStripButton toolStripButton7;
        private ToolStripButton toolStripButton8;
        private AdvancedFilters advancedFilters1;
        private ToolStripMenuItem advancedClipCopyToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem9;
        private ToolStripMenuItem addWindowsFeaturesToTheListToolStripMenuItem;
    }
}

