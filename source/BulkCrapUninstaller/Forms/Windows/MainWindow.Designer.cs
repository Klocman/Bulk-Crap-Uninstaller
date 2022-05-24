using System.ComponentModel;
using System.Windows.Forms;
using BrightIdeasSoftware;
using BulkCrapUninstaller.Controls;
using BulkCrapUninstaller.Functions.Tracking;

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
            this.splitContainerListAndMap = new System.Windows.Forms.SplitContainer();
            this.listViewPanel = new System.Windows.Forms.Panel();
            this.uninstallerObjectListView = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnDisplayName = new BrightIdeasSoftware.OLVColumn();
            this.olvColumnPublisher = new BrightIdeasSoftware.OLVColumn();
            this.olvColumnRating = new BrightIdeasSoftware.OLVColumn();
            this.olvColumnDisplayVersion = new BrightIdeasSoftware.OLVColumn();
            this.olvColumnInstallDate = new BrightIdeasSoftware.OLVColumn();
            this.olvColumnSize = new BrightIdeasSoftware.OLVColumn();
            this.olvColumnStartup = new BrightIdeasSoftware.OLVColumn();
            this.olvColumnIs64 = new BrightIdeasSoftware.OLVColumn();
            this.olvColumnUninstallString = new BrightIdeasSoftware.OLVColumn();
            this.olvColumnAbout = new BrightIdeasSoftware.OLVColumn();
            this.olvColumnInstallSource = new BrightIdeasSoftware.OLVColumn();
            this.olvColumnInstallLocation = new BrightIdeasSoftware.OLVColumn();
            this.olvColumnUninstallerKind = new BrightIdeasSoftware.OLVColumn();
            this.olvColumnSystemComponent = new BrightIdeasSoftware.OLVColumn();
            this.olvColumnProtected = new BrightIdeasSoftware.OLVColumn();
            this.olvColumnRegistryKeyName = new BrightIdeasSoftware.OLVColumn();
            this.olvColumnGuid = new BrightIdeasSoftware.OLVColumn();
            this.olvColumnQuietUninstallString = new BrightIdeasSoftware.OLVColumn();
            this.treeMap1 = new SimpleTreeMap.TreeMap();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSelAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSelNone = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSelInv = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonTarget = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonUninstall = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonModify = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonProperties = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton7 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton8 = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripLabelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripLabelSize = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripLabelTotal = new System.Windows.Forms.ToolStripStatusLabel();
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
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.excludeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.includeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorFiltering = new System.Windows.Forms.ToolStripSeparator();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.toolStripMenuItem15 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem16 = new System.Windows.Forms.ToolStripMenuItem();
            this.slantcoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator26 = new System.Windows.Forms.ToolStripSeparator();
            this.fossHubcomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sourceForgecomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gitHubcomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileHippocomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.propertiesContextMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportDialog = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadUninstallersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.loadUninstallerListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator30 = new System.Windows.Forms.ToolStripSeparator();
            this.exportSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToABatchUninstallScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportStoreAppsToPowerShellRemoveScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showColorLegendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayToolbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showTreemapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displaySettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayStatusbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.useSystemThemeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filteringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedApplicationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.basicApplicationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.systemComponentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.everythingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.automaticallyStartedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onlyWebBrowsersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator31 = new System.Windows.Forms.ToolStripSeparator();
            this.viewTweaksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewUnregisteredToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewWindowsFeaturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewWindowsStoreAppsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator28 = new System.Windows.Forms.ToolStripSeparator();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.basicOperationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uninstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quietUninstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modifyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.googleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alternativeToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slantcoToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator27 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem17 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem18 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem20 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem19 = new System.Windows.Forms.ToolStripMenuItem();
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
            this.toolStripSeparator32 = new System.Windows.Forms.ToolStripSeparator();
            this.takeOwnershipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openStartupManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
            this.cleanUpProgramFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.targetMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uninstallFromDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.troubleshootUninstallProblemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startDiskCleanupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tryToInstallNETV35ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createRestorePointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator29 = new System.Windows.Forms.ToolStripSeparator();
            this.openProgramsAndFeaturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSystemRestoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.globalHotkeys1 = new Klocman.Subsystems.GlobalHotkeys();
            this.splashScreen1 = new Klocman.Forms.SplashScreen();
            this.usageTracker = new BulkCrapUninstaller.Functions.Tracking.UsageTracker();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerListAndMap)).BeginInit();
            this.splitContainerListAndMap.Panel1.SuspendLayout();
            this.splitContainerListAndMap.Panel2.SuspendLayout();
            this.splitContainerListAndMap.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.advancedFilters1);
            resources.ApplyResources(this.splitContainer1.Panel1, "splitContainer1.Panel1");
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainerListAndMap);
            this.splitContainer1.Panel2.Controls.Add(this.toolStrip);
            this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
            // 
            // advancedFilters1
            // 
            resources.ApplyResources(this.advancedFilters1, "advancedFilters1");
            this.advancedFilters1.Name = "advancedFilters1";
            this.advancedFilters1.SelectedEntryGetter = null;
            // 
            // splitContainerListAndMap
            // 
            resources.ApplyResources(this.splitContainerListAndMap, "splitContainerListAndMap");
            this.splitContainerListAndMap.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerListAndMap.Name = "splitContainerListAndMap";
            // 
            // splitContainerListAndMap.Panel1
            // 
            this.splitContainerListAndMap.Panel1.Controls.Add(this.listViewPanel);
            // 
            // splitContainerListAndMap.Panel2
            // 
            this.splitContainerListAndMap.Panel2.Controls.Add(this.treeMap1);
            // 
            // listViewPanel
            // 
            this.listViewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewPanel.Controls.Add(this.uninstallerObjectListView);
            resources.ApplyResources(this.listViewPanel, "listViewPanel");
            this.listViewPanel.Name = "listViewPanel";
            // 
            // uninstallerObjectListView
            // 
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
            resources.ApplyResources(this.uninstallerObjectListView, "uninstallerObjectListView");
            this.uninstallerObjectListView.FullRowSelect = true;
            this.uninstallerObjectListView.GridLines = true;
            this.uninstallerObjectListView.HideSelection = false;
            this.uninstallerObjectListView.Name = "uninstallerObjectListView";
            this.uninstallerObjectListView.ShowGroups = false;
            this.uninstallerObjectListView.ShowImagesOnSubItems = true;
            this.uninstallerObjectListView.ShowItemToolTips = true;
            this.uninstallerObjectListView.SortGroupItemsByPrimaryColumn = false;
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
            this.olvColumnDisplayName.Hideable = false;
            resources.ApplyResources(this.olvColumnDisplayName, "olvColumnDisplayName");
            // 
            // olvColumnPublisher
            // 
            resources.ApplyResources(this.olvColumnPublisher, "olvColumnPublisher");
            // 
            // olvColumnRating
            // 
            this.olvColumnRating.IsEditable = false;
            this.olvColumnRating.MaximumWidth = 160;
            this.olvColumnRating.MinimumWidth = 80;
            this.olvColumnRating.Searchable = false;
            resources.ApplyResources(this.olvColumnRating, "olvColumnRating");
            this.olvColumnRating.UseFiltering = false;
            // 
            // olvColumnDisplayVersion
            // 
            resources.ApplyResources(this.olvColumnDisplayVersion, "olvColumnDisplayVersion");
            // 
            // olvColumnInstallDate
            // 
            this.olvColumnInstallDate.IsEditable = false;
            resources.ApplyResources(this.olvColumnInstallDate, "olvColumnInstallDate");
            // 
            // olvColumnSize
            // 
            this.olvColumnSize.Searchable = false;
            resources.ApplyResources(this.olvColumnSize, "olvColumnSize");
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
            this.olvColumnAbout.Hyperlink = true;
            this.olvColumnAbout.IsEditable = false;
            resources.ApplyResources(this.olvColumnAbout, "olvColumnAbout");
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
            // treeMap1
            // 
            resources.ApplyResources(this.treeMap1, "treeMap1");
            this.treeMap1.Name = "treeMap1";
            this.treeMap1.ShowToolTip = false;
            this.treeMap1.UseLogValueScaling = false;
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(22, 22);
            resources.ApplyResources(this.toolStrip, "toolStrip");
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripSeparator22,
            this.toolStripButtonSelAll,
            this.toolStripButtonSelNone,
            this.toolStripButtonSelInv,
            this.toolStripSeparator23,
            this.toolStripButtonTarget,
            this.toolStripSeparator21,
            this.toolStripButtonUninstall,
            this.toolStripButton2,
            this.toolStripButtonModify,
            this.toolStripSeparator4,
            this.toolStripButtonProperties,
            this.toolStripSeparator24,
            this.toolStripButton7,
            this.toolStripButton8});
            this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.TabStop = true;
            // 
            // toolStripButton1
            // 
            resources.ApplyResources(this.toolStripButton1, "toolStripButton1");
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Click += new System.EventHandler(this.ReloadUninstallers);
            // 
            // toolStripSeparator22
            // 
            this.toolStripSeparator22.Name = "toolStripSeparator22";
            resources.ApplyResources(this.toolStripSeparator22, "toolStripSeparator22");
            // 
            // toolStripButtonSelAll
            // 
            this.toolStripButtonSelAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonSelAll, "toolStripButtonSelAll");
            this.toolStripButtonSelAll.Name = "toolStripButtonSelAll";
            // 
            // toolStripButtonSelNone
            // 
            this.toolStripButtonSelNone.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonSelNone, "toolStripButtonSelNone");
            this.toolStripButtonSelNone.Name = "toolStripButtonSelNone";
            // 
            // toolStripButtonSelInv
            // 
            this.toolStripButtonSelInv.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonSelInv, "toolStripButtonSelInv");
            this.toolStripButtonSelInv.Name = "toolStripButtonSelInv";
            // 
            // toolStripSeparator23
            // 
            this.toolStripSeparator23.Name = "toolStripSeparator23";
            resources.ApplyResources(this.toolStripSeparator23, "toolStripSeparator23");
            // 
            // toolStripButtonTarget
            // 
            this.toolStripButtonTarget.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonTarget.Image = global::BulkCrapUninstaller.Properties.Resources.target;
            resources.ApplyResources(this.toolStripButtonTarget, "toolStripButtonTarget");
            this.toolStripButtonTarget.Name = "toolStripButtonTarget";
            this.toolStripButtonTarget.Click += new System.EventHandler(this.OpenTargetWindow);
            // 
            // toolStripSeparator21
            // 
            this.toolStripSeparator21.Name = "toolStripSeparator21";
            resources.ApplyResources(this.toolStripSeparator21, "toolStripSeparator21");
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
            // toolStripButtonModify
            // 
            this.toolStripButtonModify.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonModify.Image = global::BulkCrapUninstaller.Properties.Resources.edit_box;
            resources.ApplyResources(this.toolStripButtonModify, "toolStripButtonModify");
            this.toolStripButtonModify.Name = "toolStripButtonModify";
            this.toolStripButtonModify.Click += new System.EventHandler(this.modifyToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // toolStripButtonProperties
            // 
            this.toolStripButtonProperties.Image = global::BulkCrapUninstaller.Properties.Resources.properties;
            resources.ApplyResources(this.toolStripButtonProperties, "toolStripButtonProperties");
            this.toolStripButtonProperties.Name = "toolStripButtonProperties";
            this.toolStripButtonProperties.Click += new System.EventHandler(this.OpenProperties);
            // 
            // toolStripSeparator24
            // 
            this.toolStripSeparator24.Name = "toolStripSeparator24";
            resources.ApplyResources(this.toolStripSeparator24, "toolStripSeparator24");
            // 
            // toolStripButton7
            // 
            this.toolStripButton7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton7.Image = global::BulkCrapUninstaller.Properties.Resources.settings;
            resources.ApplyResources(this.toolStripButton7, "toolStripButton7");
            this.toolStripButton7.Name = "toolStripButton7";
            this.toolStripButton7.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // toolStripButton8
            // 
            this.toolStripButton8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton8.Image = global::BulkCrapUninstaller.Properties.Resources.information_circle;
            resources.ApplyResources(this.toolStripButton8, "toolStripButton8");
            this.toolStripButton8.Name = "toolStripButton8";
            this.toolStripButton8.Click += new System.EventHandler(this.openHelpToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabelStatus,
            this.toolStripLabelSize,
            this.toolStripLabelTotal});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            // 
            // toolStripLabelStatus
            // 
            this.toolStripLabelStatus.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripLabelStatus.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripLabelStatus.Name = "toolStripLabelStatus";
            resources.ApplyResources(this.toolStripLabelStatus, "toolStripLabelStatus");
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
            // settingsSidebarPanel
            // 
            resources.ApplyResources(this.settingsSidebarPanel, "settingsSidebarPanel");
            this.settingsSidebarPanel.Controls.Add(this.propertiesSidebar);
            this.settingsSidebarPanel.Controls.Add(this.label1);
            this.settingsSidebarPanel.Controls.Add(this.groupBox1);
            this.settingsSidebarPanel.Name = "settingsSidebarPanel";
            // 
            // propertiesSidebar
            // 
            resources.ApplyResources(this.propertiesSidebar, "propertiesSidebar");
            this.propertiesSidebar.InvalidEnabled = true;
            this.propertiesSidebar.Name = "propertiesSidebar";
            this.propertiesSidebar.OrphansEnabled = true;
            this.propertiesSidebar.ProtectedEnabled = true;
            this.propertiesSidebar.ShowTweaksEnabled = true;
            this.propertiesSidebar.StoreAppsEnabled = true;
            this.propertiesSidebar.SysCompEnabled = true;
            this.propertiesSidebar.UpdatesEnabled = true;
            this.propertiesSidebar.WinFeaturesEnabled = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.buttonAdvFiltering);
            this.groupBox1.Controls.Add(this.filterEditor1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // buttonAdvFiltering
            // 
            resources.ApplyResources(this.buttonAdvFiltering, "buttonAdvFiltering");
            this.buttonAdvFiltering.Name = "buttonAdvFiltering";
            this.buttonAdvFiltering.UseVisualStyleBackColor = true;
            this.buttonAdvFiltering.Click += new System.EventHandler(this.buttonAdvFiltering_Click);
            // 
            // filterEditor1
            // 
            resources.ApplyResources(this.filterEditor1, "filterEditor1");
            this.filterEditor1.Name = "filterEditor1";
            this.filterEditor1.ShowAsSearch = true;
            this.filterEditor1.FocusSearchTarget += new System.EventHandler(this.filterEditor1_FocusSearchTarget);
            // 
            // uninstallListContextMenuStrip
            // 
            this.uninstallListContextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.uninstallListContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uninstallContextMenuStripItem,
            this.quietUninstallContextMenuStripItem,
            this.manualUninstallToolStripMenuItem1,
            this.uninstallUsingMsiExecContextMenuStripItem,
            this.toolStripSeparator3,
            this.excludeToolStripMenuItem,
            this.includeToolStripMenuItem,
            this.toolStripSeparatorFiltering,
            this.runToolStripMenuItem,
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
            resources.ApplyResources(this.uninstallListContextMenuStrip, "uninstallListContextMenuStrip");
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
            this.manualUninstallToolStripMenuItem1.Name = "manualUninstallToolStripMenuItem1";
            resources.ApplyResources(this.manualUninstallToolStripMenuItem1, "manualUninstallToolStripMenuItem1");
            this.manualUninstallToolStripMenuItem1.Click += new System.EventHandler(this.RunAdvancedUninstall);
            // 
            // uninstallUsingMsiExecContextMenuStripItem
            // 
            this.uninstallUsingMsiExecContextMenuStripItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.msiInstallContextMenuStripItem,
            this.msiUninstallContextMenuStripItem,
            this.msiQuietUninstallContextMenuStripItem});
            this.uninstallUsingMsiExecContextMenuStripItem.Name = "uninstallUsingMsiExecContextMenuStripItem";
            resources.ApplyResources(this.uninstallUsingMsiExecContextMenuStripItem, "uninstallUsingMsiExecContextMenuStripItem");
            // 
            // msiInstallContextMenuStripItem
            // 
            this.msiInstallContextMenuStripItem.Name = "msiInstallContextMenuStripItem";
            resources.ApplyResources(this.msiInstallContextMenuStripItem, "msiInstallContextMenuStripItem");
            this.msiInstallContextMenuStripItem.Click += new System.EventHandler(this.msiInstallContextMenuStripItem_Click);
            // 
            // msiUninstallContextMenuStripItem
            // 
            this.msiUninstallContextMenuStripItem.Name = "msiUninstallContextMenuStripItem";
            resources.ApplyResources(this.msiUninstallContextMenuStripItem, "msiUninstallContextMenuStripItem");
            this.msiUninstallContextMenuStripItem.Click += new System.EventHandler(this.msiUninstallContextMenuStripItem_Click);
            // 
            // msiQuietUninstallContextMenuStripItem
            // 
            this.msiQuietUninstallContextMenuStripItem.Name = "msiQuietUninstallContextMenuStripItem";
            resources.ApplyResources(this.msiQuietUninstallContextMenuStripItem, "msiQuietUninstallContextMenuStripItem");
            this.msiQuietUninstallContextMenuStripItem.Click += new System.EventHandler(this.msiQuietUninstallContextMenuStripItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // excludeToolStripMenuItem
            // 
            this.excludeToolStripMenuItem.Name = "excludeToolStripMenuItem";
            resources.ApplyResources(this.excludeToolStripMenuItem, "excludeToolStripMenuItem");
            this.excludeToolStripMenuItem.Click += new System.EventHandler(this.excludeToolStripMenuItem_Click);
            // 
            // includeToolStripMenuItem
            // 
            this.includeToolStripMenuItem.Name = "includeToolStripMenuItem";
            resources.ApplyResources(this.includeToolStripMenuItem, "includeToolStripMenuItem");
            this.includeToolStripMenuItem.Click += new System.EventHandler(this.includeToolStripMenuItem_Click);
            // 
            // toolStripSeparatorFiltering
            // 
            this.toolStripSeparatorFiltering.Name = "toolStripSeparatorFiltering";
            resources.ApplyResources(this.toolStripSeparatorFiltering, "toolStripSeparatorFiltering");
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            resources.ApplyResources(this.runToolStripMenuItem, "runToolStripMenuItem");
            this.runToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.runToolStripMenuItem_DropDownItemClicked);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
            // 
            // copyToClipboardContextMenuStripItem
            // 
            this.copyToClipboardContextMenuStripItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem9,
            this.toolStripSeparator9,
            this.fullInformationCopyContextMenuStripItem,
            this.programNameCopyContextMenuStripItem,
            this.gUIDProductCodeCopyContextMenuStripItem,
            this.fullRegistryPathCopyContextMenuStripItem,
            this.uninstallStringCopyContextMenuStripItem});
            resources.ApplyResources(this.copyToClipboardContextMenuStripItem, "copyToClipboardContextMenuStripItem");
            this.copyToClipboardContextMenuStripItem.Name = "copyToClipboardContextMenuStripItem";
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            resources.ApplyResources(this.toolStripMenuItem9, "toolStripMenuItem9");
            this.toolStripMenuItem9.Click += new System.EventHandler(this.OpenAdvancedClipboardCopy);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            resources.ApplyResources(this.toolStripSeparator9, "toolStripSeparator9");
            // 
            // fullInformationCopyContextMenuStripItem
            // 
            this.fullInformationCopyContextMenuStripItem.Name = "fullInformationCopyContextMenuStripItem";
            resources.ApplyResources(this.fullInformationCopyContextMenuStripItem, "fullInformationCopyContextMenuStripItem");
            this.fullInformationCopyContextMenuStripItem.Click += new System.EventHandler(this.ClipboardCopyFullInformation);
            // 
            // programNameCopyContextMenuStripItem
            // 
            this.programNameCopyContextMenuStripItem.Name = "programNameCopyContextMenuStripItem";
            resources.ApplyResources(this.programNameCopyContextMenuStripItem, "programNameCopyContextMenuStripItem");
            this.programNameCopyContextMenuStripItem.Click += new System.EventHandler(this.ClipboardCopyProgramName);
            // 
            // gUIDProductCodeCopyContextMenuStripItem
            // 
            this.gUIDProductCodeCopyContextMenuStripItem.Name = "gUIDProductCodeCopyContextMenuStripItem";
            resources.ApplyResources(this.gUIDProductCodeCopyContextMenuStripItem, "gUIDProductCodeCopyContextMenuStripItem");
            this.gUIDProductCodeCopyContextMenuStripItem.Click += new System.EventHandler(this.ClipboardCopyGuids);
            // 
            // fullRegistryPathCopyContextMenuStripItem
            // 
            this.fullRegistryPathCopyContextMenuStripItem.Name = "fullRegistryPathCopyContextMenuStripItem";
            resources.ApplyResources(this.fullRegistryPathCopyContextMenuStripItem, "fullRegistryPathCopyContextMenuStripItem");
            this.fullRegistryPathCopyContextMenuStripItem.Click += new System.EventHandler(this.ClipboardCopyRegistryPath);
            // 
            // uninstallStringCopyContextMenuStripItem
            // 
            this.uninstallStringCopyContextMenuStripItem.Name = "uninstallStringCopyContextMenuStripItem";
            resources.ApplyResources(this.uninstallStringCopyContextMenuStripItem, "uninstallStringCopyContextMenuStripItem");
            this.uninstallStringCopyContextMenuStripItem.Click += new System.EventHandler(this.ClipboardCopyUninstallString);
            // 
            // deleteRegistryEntryContextMenuStripItem
            // 
            this.deleteRegistryEntryContextMenuStripItem.Name = "deleteRegistryEntryContextMenuStripItem";
            resources.ApplyResources(this.deleteRegistryEntryContextMenuStripItem, "deleteRegistryEntryContextMenuStripItem");
            this.deleteRegistryEntryContextMenuStripItem.Click += new System.EventHandler(this.DeleteRegistryEntries);
            // 
            // renameContextMenuStripItem
            // 
            this.renameContextMenuStripItem.Name = "renameContextMenuStripItem";
            resources.ApplyResources(this.renameContextMenuStripItem, "renameContextMenuStripItem");
            this.renameContextMenuStripItem.Click += new System.EventHandler(this.RenameEntries);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // openInExplorerContextMenuStripItem
            // 
            this.openInExplorerContextMenuStripItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.installLocationOpenInExplorerContextMenuStripItem,
            this.uninstallerLocationOpenInExplorerContextMenuStripItem,
            this.sourceLocationOpenInExplorerContextMenuStripItem});
            resources.ApplyResources(this.openInExplorerContextMenuStripItem, "openInExplorerContextMenuStripItem");
            this.openInExplorerContextMenuStripItem.Name = "openInExplorerContextMenuStripItem";
            // 
            // installLocationOpenInExplorerContextMenuStripItem
            // 
            this.installLocationOpenInExplorerContextMenuStripItem.Name = "installLocationOpenInExplorerContextMenuStripItem";
            resources.ApplyResources(this.installLocationOpenInExplorerContextMenuStripItem, "installLocationOpenInExplorerContextMenuStripItem");
            this.installLocationOpenInExplorerContextMenuStripItem.Click += new System.EventHandler(this.OpenInstallLocation);
            // 
            // uninstallerLocationOpenInExplorerContextMenuStripItem
            // 
            this.uninstallerLocationOpenInExplorerContextMenuStripItem.Name = "uninstallerLocationOpenInExplorerContextMenuStripItem";
            resources.ApplyResources(this.uninstallerLocationOpenInExplorerContextMenuStripItem, "uninstallerLocationOpenInExplorerContextMenuStripItem");
            this.uninstallerLocationOpenInExplorerContextMenuStripItem.Click += new System.EventHandler(this.OpenUninstallerLocation);
            // 
            // sourceLocationOpenInExplorerContextMenuStripItem
            // 
            this.sourceLocationOpenInExplorerContextMenuStripItem.Name = "sourceLocationOpenInExplorerContextMenuStripItem";
            resources.ApplyResources(this.sourceLocationOpenInExplorerContextMenuStripItem, "sourceLocationOpenInExplorerContextMenuStripItem");
            this.sourceLocationOpenInExplorerContextMenuStripItem.Click += new System.EventHandler(this.OpenInstallationSource);
            // 
            // openWebPageContextMenuStripItem
            // 
            this.openWebPageContextMenuStripItem.Name = "openWebPageContextMenuStripItem";
            resources.ApplyResources(this.openWebPageContextMenuStripItem, "openWebPageContextMenuStripItem");
            this.openWebPageContextMenuStripItem.Click += new System.EventHandler(this.OpenAssociatedWebPage);
            // 
            // lookUpOnlineToolStripMenuItem
            // 
            this.lookUpOnlineToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem15,
            this.toolStripMenuItem16,
            this.slantcoToolStripMenuItem,
            this.toolStripSeparator26,
            this.fossHubcomToolStripMenuItem,
            this.sourceForgecomToolStripMenuItem,
            this.gitHubcomToolStripMenuItem,
            this.fileHippocomToolStripMenuItem});
            resources.ApplyResources(this.lookUpOnlineToolStripMenuItem, "lookUpOnlineToolStripMenuItem");
            this.lookUpOnlineToolStripMenuItem.Name = "lookUpOnlineToolStripMenuItem";
            // 
            // toolStripMenuItem15
            // 
            this.toolStripMenuItem15.Name = "toolStripMenuItem15";
            resources.ApplyResources(this.toolStripMenuItem15, "toolStripMenuItem15");
            this.toolStripMenuItem15.Click += new System.EventHandler(this.googleToolStripMenuItem_Click);
            // 
            // toolStripMenuItem16
            // 
            this.toolStripMenuItem16.Name = "toolStripMenuItem16";
            resources.ApplyResources(this.toolStripMenuItem16, "toolStripMenuItem16");
            this.toolStripMenuItem16.Click += new System.EventHandler(this.alternativeToToolStripMenuItem_Click);
            // 
            // slantcoToolStripMenuItem
            // 
            this.slantcoToolStripMenuItem.Name = "slantcoToolStripMenuItem";
            resources.ApplyResources(this.slantcoToolStripMenuItem, "slantcoToolStripMenuItem");
            this.slantcoToolStripMenuItem.Click += new System.EventHandler(this.slantcoToolStripMenuItem_Click);
            // 
            // toolStripSeparator26
            // 
            this.toolStripSeparator26.Name = "toolStripSeparator26";
            resources.ApplyResources(this.toolStripSeparator26, "toolStripSeparator26");
            // 
            // fossHubcomToolStripMenuItem
            // 
            this.fossHubcomToolStripMenuItem.Name = "fossHubcomToolStripMenuItem";
            resources.ApplyResources(this.fossHubcomToolStripMenuItem, "fossHubcomToolStripMenuItem");
            this.fossHubcomToolStripMenuItem.Click += new System.EventHandler(this.fossHubcomToolStripMenuItem_Click);
            // 
            // sourceForgecomToolStripMenuItem
            // 
            this.sourceForgecomToolStripMenuItem.Name = "sourceForgecomToolStripMenuItem";
            resources.ApplyResources(this.sourceForgecomToolStripMenuItem, "sourceForgecomToolStripMenuItem");
            this.sourceForgecomToolStripMenuItem.Click += new System.EventHandler(this.sourceForgecomToolStripMenuItem_Click);
            // 
            // gitHubcomToolStripMenuItem
            // 
            this.gitHubcomToolStripMenuItem.Name = "gitHubcomToolStripMenuItem";
            resources.ApplyResources(this.gitHubcomToolStripMenuItem, "gitHubcomToolStripMenuItem");
            this.gitHubcomToolStripMenuItem.Click += new System.EventHandler(this.gitHubcomToolStripMenuItem_Click);
            // 
            // fileHippocomToolStripMenuItem
            // 
            this.fileHippocomToolStripMenuItem.Name = "fileHippocomToolStripMenuItem";
            resources.ApplyResources(this.fileHippocomToolStripMenuItem, "fileHippocomToolStripMenuItem");
            this.fileHippocomToolStripMenuItem.Click += new System.EventHandler(this.fileHippocomToolStripMenuItem_Click);
            // 
            // rateToolStripMenuItem
            // 
            this.rateToolStripMenuItem.Image = global::BulkCrapUninstaller.Properties.Resources.star;
            this.rateToolStripMenuItem.Name = "rateToolStripMenuItem";
            resources.ApplyResources(this.rateToolStripMenuItem, "rateToolStripMenuItem");
            this.rateToolStripMenuItem.Click += new System.EventHandler(this.rateToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
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
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.filteringToolStripMenuItem,
            this.basicOperationsToolStripMenuItem,
            this.advancedOperationsToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.debugToolStripMenuItem});
            resources.ApplyResources(this.menuStrip, "menuStrip");
            this.menuStrip.Name = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reloadUninstallersToolStripMenuItem,
            this.toolStripSeparator1,
            this.loadUninstallerListToolStripMenuItem,
            this.toolStripSeparator30,
            this.exportSelectedToolStripMenuItem,
            this.exportToABatchUninstallScriptToolStripMenuItem,
            this.exportStoreAppsToPowerShellRemoveScriptToolStripMenuItem,
            this.toolStripSeparator10,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
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
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // loadUninstallerListToolStripMenuItem
            // 
            resources.ApplyResources(this.loadUninstallerListToolStripMenuItem, "loadUninstallerListToolStripMenuItem");
            this.loadUninstallerListToolStripMenuItem.Name = "loadUninstallerListToolStripMenuItem";
            this.loadUninstallerListToolStripMenuItem.Click += new System.EventHandler(this.OpenUninstallLists);
            // 
            // toolStripSeparator30
            // 
            this.toolStripSeparator30.Name = "toolStripSeparator30";
            resources.ApplyResources(this.toolStripSeparator30, "toolStripSeparator30");
            // 
            // exportSelectedToolStripMenuItem
            // 
            this.exportSelectedToolStripMenuItem.Name = "exportSelectedToolStripMenuItem";
            resources.ApplyResources(this.exportSelectedToolStripMenuItem, "exportSelectedToolStripMenuItem");
            this.exportSelectedToolStripMenuItem.Click += new System.EventHandler(this.exportSelectedToolStripMenuItem_Click);
            // 
            // exportToABatchUninstallScriptToolStripMenuItem
            // 
            this.exportToABatchUninstallScriptToolStripMenuItem.Name = "exportToABatchUninstallScriptToolStripMenuItem";
            resources.ApplyResources(this.exportToABatchUninstallScriptToolStripMenuItem, "exportToABatchUninstallScriptToolStripMenuItem");
            this.exportToABatchUninstallScriptToolStripMenuItem.Click += new System.EventHandler(this.exportToABatchUninstallScriptToolStripMenuItem_Click);
            // 
            // exportStoreAppsToPowerShellRemoveScriptToolStripMenuItem
            // 
            this.exportStoreAppsToPowerShellRemoveScriptToolStripMenuItem.Name = "exportStoreAppsToPowerShellRemoveScriptToolStripMenuItem";
            resources.ApplyResources(this.exportStoreAppsToPowerShellRemoveScriptToolStripMenuItem, "exportStoreAppsToPowerShellRemoveScriptToolStripMenuItem");
            this.exportStoreAppsToPowerShellRemoveScriptToolStripMenuItem.Click += new System.EventHandler(this.exportStoreAppsToPowerShellRemoveScriptToolStripMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            resources.ApplyResources(this.toolStripSeparator10, "toolStripSeparator10");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showColorLegendToolStripMenuItem,
            this.displayToolbarToolStripMenuItem,
            this.showTreemapToolStripMenuItem,
            this.displaySettingsToolStripMenuItem,
            this.displayStatusbarToolStripMenuItem,
            this.toolStripSeparator12,
            this.useSystemThemeToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            this.viewToolStripMenuItem.DropDownOpening += new System.EventHandler(this.viewToolStripMenuItem_DropDownOpening);
            // 
            // showColorLegendToolStripMenuItem
            // 
            this.showColorLegendToolStripMenuItem.Name = "showColorLegendToolStripMenuItem";
            resources.ApplyResources(this.showColorLegendToolStripMenuItem, "showColorLegendToolStripMenuItem");
            // 
            // displayToolbarToolStripMenuItem
            // 
            this.displayToolbarToolStripMenuItem.Checked = true;
            this.displayToolbarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.displayToolbarToolStripMenuItem.Name = "displayToolbarToolStripMenuItem";
            resources.ApplyResources(this.displayToolbarToolStripMenuItem, "displayToolbarToolStripMenuItem");
            // 
            // showTreemapToolStripMenuItem
            // 
            this.showTreemapToolStripMenuItem.Name = "showTreemapToolStripMenuItem";
            resources.ApplyResources(this.showTreemapToolStripMenuItem, "showTreemapToolStripMenuItem");
            // 
            // displaySettingsToolStripMenuItem
            // 
            this.displaySettingsToolStripMenuItem.Checked = true;
            this.displaySettingsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.displaySettingsToolStripMenuItem.Name = "displaySettingsToolStripMenuItem";
            resources.ApplyResources(this.displaySettingsToolStripMenuItem, "displaySettingsToolStripMenuItem");
            // 
            // displayStatusbarToolStripMenuItem
            // 
            this.displayStatusbarToolStripMenuItem.Checked = true;
            this.displayStatusbarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.displayStatusbarToolStripMenuItem.Name = "displayStatusbarToolStripMenuItem";
            resources.ApplyResources(this.displayStatusbarToolStripMenuItem, "displayStatusbarToolStripMenuItem");
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            resources.ApplyResources(this.toolStripSeparator12, "toolStripSeparator12");
            // 
            // useSystemThemeToolStripMenuItem
            // 
            this.useSystemThemeToolStripMenuItem.Checked = true;
            this.useSystemThemeToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useSystemThemeToolStripMenuItem.Name = "useSystemThemeToolStripMenuItem";
            resources.ApplyResources(this.useSystemThemeToolStripMenuItem, "useSystemThemeToolStripMenuItem");
            // 
            // filteringToolStripMenuItem
            // 
            this.filteringToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.advancedApplicationsToolStripMenuItem,
            this.basicApplicationsToolStripMenuItem,
            this.systemComponentsToolStripMenuItem,
            this.everythingToolStripMenuItem,
            this.toolStripSeparator20,
            this.automaticallyStartedToolStripMenuItem,
            this.onlyWebBrowsersToolStripMenuItem,
            this.toolStripSeparator31,
            this.viewTweaksToolStripMenuItem,
            this.viewUnregisteredToolStripMenuItem,
            this.viewUpdatesToolStripMenuItem,
            this.viewWindowsFeaturesToolStripMenuItem,
            this.viewWindowsStoreAppsToolStripMenuItem,
            this.toolStripSeparator28,
            this.searchToolStripMenuItem});
            this.filteringToolStripMenuItem.Name = "filteringToolStripMenuItem";
            resources.ApplyResources(this.filteringToolStripMenuItem, "filteringToolStripMenuItem");
            this.filteringToolStripMenuItem.DropDownOpening += new System.EventHandler(this.filteringToolStripMenuItem_DropDownOpening);
            // 
            // advancedApplicationsToolStripMenuItem
            // 
            this.advancedApplicationsToolStripMenuItem.Image = global::BulkCrapUninstaller.Properties.Resources.filter;
            this.advancedApplicationsToolStripMenuItem.Name = "advancedApplicationsToolStripMenuItem";
            resources.ApplyResources(this.advancedApplicationsToolStripMenuItem, "advancedApplicationsToolStripMenuItem");
            this.advancedApplicationsToolStripMenuItem.Click += new System.EventHandler(this.advancedApplicationsToolStripMenuItem_Click);
            // 
            // basicApplicationsToolStripMenuItem
            // 
            this.basicApplicationsToolStripMenuItem.Name = "basicApplicationsToolStripMenuItem";
            resources.ApplyResources(this.basicApplicationsToolStripMenuItem, "basicApplicationsToolStripMenuItem");
            this.basicApplicationsToolStripMenuItem.Click += new System.EventHandler(this.basicApplicationsToolStripMenuItem_Click);
            // 
            // systemComponentsToolStripMenuItem
            // 
            this.systemComponentsToolStripMenuItem.Name = "systemComponentsToolStripMenuItem";
            resources.ApplyResources(this.systemComponentsToolStripMenuItem, "systemComponentsToolStripMenuItem");
            this.systemComponentsToolStripMenuItem.Click += new System.EventHandler(this.systemComponentsToolStripMenuItem_Click);
            // 
            // everythingToolStripMenuItem
            // 
            this.everythingToolStripMenuItem.Name = "everythingToolStripMenuItem";
            resources.ApplyResources(this.everythingToolStripMenuItem, "everythingToolStripMenuItem");
            this.everythingToolStripMenuItem.Click += new System.EventHandler(this.everythingToolStripMenuItem_Click);
            // 
            // toolStripSeparator20
            // 
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            resources.ApplyResources(this.toolStripSeparator20, "toolStripSeparator20");
            // 
            // automaticallyStartedToolStripMenuItem
            // 
            this.automaticallyStartedToolStripMenuItem.Image = global::BulkCrapUninstaller.Properties.Resources.timer;
            this.automaticallyStartedToolStripMenuItem.Name = "automaticallyStartedToolStripMenuItem";
            resources.ApplyResources(this.automaticallyStartedToolStripMenuItem, "automaticallyStartedToolStripMenuItem");
            this.automaticallyStartedToolStripMenuItem.Click += new System.EventHandler(this.automaticallyStartedToolStripMenuItem_Click);
            // 
            // onlyWebBrowsersToolStripMenuItem
            // 
            this.onlyWebBrowsersToolStripMenuItem.Name = "onlyWebBrowsersToolStripMenuItem";
            resources.ApplyResources(this.onlyWebBrowsersToolStripMenuItem, "onlyWebBrowsersToolStripMenuItem");
            this.onlyWebBrowsersToolStripMenuItem.Click += new System.EventHandler(this.onlyWebBrowsersToolStripMenuItem_Click);
            // 
            // toolStripSeparator31
            // 
            this.toolStripSeparator31.Name = "toolStripSeparator31";
            resources.ApplyResources(this.toolStripSeparator31, "toolStripSeparator31");
            // 
            // viewTweaksToolStripMenuItem
            // 
            this.viewTweaksToolStripMenuItem.Name = "viewTweaksToolStripMenuItem";
            resources.ApplyResources(this.viewTweaksToolStripMenuItem, "viewTweaksToolStripMenuItem");
            this.viewTweaksToolStripMenuItem.Click += new System.EventHandler(this.viewTweaksToolStripMenuItem_Click);
            // 
            // viewUnregisteredToolStripMenuItem
            // 
            this.viewUnregisteredToolStripMenuItem.Name = "viewUnregisteredToolStripMenuItem";
            resources.ApplyResources(this.viewUnregisteredToolStripMenuItem, "viewUnregisteredToolStripMenuItem");
            this.viewUnregisteredToolStripMenuItem.Click += new System.EventHandler(this.viewUnregisteredToolStripMenuItem_Click);
            // 
            // viewUpdatesToolStripMenuItem
            // 
            this.viewUpdatesToolStripMenuItem.Name = "viewUpdatesToolStripMenuItem";
            resources.ApplyResources(this.viewUpdatesToolStripMenuItem, "viewUpdatesToolStripMenuItem");
            this.viewUpdatesToolStripMenuItem.Click += new System.EventHandler(this.viewUpdatesToolStripMenuItem_Click);
            // 
            // viewWindowsFeaturesToolStripMenuItem
            // 
            this.viewWindowsFeaturesToolStripMenuItem.Name = "viewWindowsFeaturesToolStripMenuItem";
            resources.ApplyResources(this.viewWindowsFeaturesToolStripMenuItem, "viewWindowsFeaturesToolStripMenuItem");
            this.viewWindowsFeaturesToolStripMenuItem.Click += new System.EventHandler(this.viewWindowsFeaturesToolStripMenuItem_Click);
            // 
            // viewWindowsStoreAppsToolStripMenuItem
            // 
            this.viewWindowsStoreAppsToolStripMenuItem.Name = "viewWindowsStoreAppsToolStripMenuItem";
            resources.ApplyResources(this.viewWindowsStoreAppsToolStripMenuItem, "viewWindowsStoreAppsToolStripMenuItem");
            this.viewWindowsStoreAppsToolStripMenuItem.Click += new System.EventHandler(this.viewWindowsStoreAppsToolStripMenuItem_Click);
            // 
            // toolStripSeparator28
            // 
            this.toolStripSeparator28.Name = "toolStripSeparator28";
            resources.ApplyResources(this.toolStripSeparator28, "toolStripSeparator28");
            // 
            // searchToolStripMenuItem
            // 
            resources.ApplyResources(this.searchToolStripMenuItem, "searchToolStripMenuItem");
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Click += new System.EventHandler(this.searchToolStripMenuItem_Click);
            // 
            // basicOperationsToolStripMenuItem
            // 
            this.basicOperationsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uninstallToolStripMenuItem,
            this.quietUninstallToolStripMenuItem,
            this.modifyToolStripMenuItem,
            this.toolStripSeparator2,
            this.toolStripMenuItem8,
            this.toolStripMenuItem1,
            this.toolStripMenuItem14,
            this.onlineSearchToolStripMenuItem,
            this.rateToolStripMenuItem1,
            this.toolStripSeparator15,
            this.propertiesToolStripMenuItem});
            resources.ApplyResources(this.basicOperationsToolStripMenuItem, "basicOperationsToolStripMenuItem");
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
            // modifyToolStripMenuItem
            // 
            this.modifyToolStripMenuItem.Image = global::BulkCrapUninstaller.Properties.Resources.edit_box;
            this.modifyToolStripMenuItem.Name = "modifyToolStripMenuItem";
            resources.ApplyResources(this.modifyToolStripMenuItem, "modifyToolStripMenuItem");
            this.modifyToolStripMenuItem.Click += new System.EventHandler(this.modifyToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.advancedClipCopyToolStripMenuItem,
            this.toolStripSeparator11,
            this.copyFullInformationToolStripMenuItem,
            this.toolStripMenuItem10,
            this.toolStripMenuItem11,
            this.toolStripMenuItem12,
            this.toolStripMenuItem13});
            resources.ApplyResources(this.toolStripMenuItem8, "toolStripMenuItem8");
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            // 
            // advancedClipCopyToolStripMenuItem
            // 
            this.advancedClipCopyToolStripMenuItem.Name = "advancedClipCopyToolStripMenuItem";
            resources.ApplyResources(this.advancedClipCopyToolStripMenuItem, "advancedClipCopyToolStripMenuItem");
            this.advancedClipCopyToolStripMenuItem.Click += new System.EventHandler(this.OpenAdvancedClipboardCopy);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            resources.ApplyResources(this.toolStripSeparator11, "toolStripSeparator11");
            // 
            // copyFullInformationToolStripMenuItem
            // 
            this.copyFullInformationToolStripMenuItem.Name = "copyFullInformationToolStripMenuItem";
            resources.ApplyResources(this.copyFullInformationToolStripMenuItem, "copyFullInformationToolStripMenuItem");
            this.copyFullInformationToolStripMenuItem.Click += new System.EventHandler(this.ClipboardCopyFullInformation);
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            resources.ApplyResources(this.toolStripMenuItem10, "toolStripMenuItem10");
            this.toolStripMenuItem10.Click += new System.EventHandler(this.ClipboardCopyProgramName);
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            resources.ApplyResources(this.toolStripMenuItem11, "toolStripMenuItem11");
            this.toolStripMenuItem11.Click += new System.EventHandler(this.ClipboardCopyGuids);
            // 
            // toolStripMenuItem12
            // 
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            resources.ApplyResources(this.toolStripMenuItem12, "toolStripMenuItem12");
            this.toolStripMenuItem12.Click += new System.EventHandler(this.ClipboardCopyRegistryPath);
            // 
            // toolStripMenuItem13
            // 
            this.toolStripMenuItem13.Name = "toolStripMenuItem13";
            resources.ApplyResources(this.toolStripMenuItem13, "toolStripMenuItem13");
            this.toolStripMenuItem13.Click += new System.EventHandler(this.ClipboardCopyUninstallString);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem5,
            this.toolStripMenuItem6,
            this.toolStripMenuItem7});
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            resources.ApplyResources(this.toolStripMenuItem5, "toolStripMenuItem5");
            this.toolStripMenuItem5.Click += new System.EventHandler(this.OpenInstallLocation);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            resources.ApplyResources(this.toolStripMenuItem6, "toolStripMenuItem6");
            this.toolStripMenuItem6.Click += new System.EventHandler(this.OpenUninstallerLocation);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            resources.ApplyResources(this.toolStripMenuItem7, "toolStripMenuItem7");
            this.toolStripMenuItem7.Click += new System.EventHandler(this.OpenInstallationSource);
            // 
            // toolStripMenuItem14
            // 
            this.toolStripMenuItem14.Name = "toolStripMenuItem14";
            resources.ApplyResources(this.toolStripMenuItem14, "toolStripMenuItem14");
            this.toolStripMenuItem14.Click += new System.EventHandler(this.OpenAssociatedWebPage);
            // 
            // onlineSearchToolStripMenuItem
            // 
            this.onlineSearchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.googleToolStripMenuItem,
            this.alternativeToToolStripMenuItem,
            this.slantcoToolStripMenuItem1,
            this.toolStripSeparator27,
            this.toolStripMenuItem17,
            this.toolStripMenuItem18,
            this.toolStripMenuItem20,
            this.toolStripMenuItem19});
            resources.ApplyResources(this.onlineSearchToolStripMenuItem, "onlineSearchToolStripMenuItem");
            this.onlineSearchToolStripMenuItem.Name = "onlineSearchToolStripMenuItem";
            // 
            // googleToolStripMenuItem
            // 
            this.googleToolStripMenuItem.Name = "googleToolStripMenuItem";
            resources.ApplyResources(this.googleToolStripMenuItem, "googleToolStripMenuItem");
            this.googleToolStripMenuItem.Click += new System.EventHandler(this.googleToolStripMenuItem_Click);
            // 
            // alternativeToToolStripMenuItem
            // 
            this.alternativeToToolStripMenuItem.Name = "alternativeToToolStripMenuItem";
            resources.ApplyResources(this.alternativeToToolStripMenuItem, "alternativeToToolStripMenuItem");
            this.alternativeToToolStripMenuItem.Click += new System.EventHandler(this.alternativeToToolStripMenuItem_Click);
            // 
            // slantcoToolStripMenuItem1
            // 
            this.slantcoToolStripMenuItem1.Name = "slantcoToolStripMenuItem1";
            resources.ApplyResources(this.slantcoToolStripMenuItem1, "slantcoToolStripMenuItem1");
            this.slantcoToolStripMenuItem1.Click += new System.EventHandler(this.slantcoToolStripMenuItem_Click);
            // 
            // toolStripSeparator27
            // 
            this.toolStripSeparator27.Name = "toolStripSeparator27";
            resources.ApplyResources(this.toolStripSeparator27, "toolStripSeparator27");
            // 
            // toolStripMenuItem17
            // 
            this.toolStripMenuItem17.Name = "toolStripMenuItem17";
            resources.ApplyResources(this.toolStripMenuItem17, "toolStripMenuItem17");
            this.toolStripMenuItem17.Click += new System.EventHandler(this.fossHubcomToolStripMenuItem_Click);
            // 
            // toolStripMenuItem18
            // 
            this.toolStripMenuItem18.Name = "toolStripMenuItem18";
            resources.ApplyResources(this.toolStripMenuItem18, "toolStripMenuItem18");
            this.toolStripMenuItem18.Click += new System.EventHandler(this.sourceForgecomToolStripMenuItem_Click);
            // 
            // toolStripMenuItem20
            // 
            this.toolStripMenuItem20.Name = "toolStripMenuItem20";
            resources.ApplyResources(this.toolStripMenuItem20, "toolStripMenuItem20");
            this.toolStripMenuItem20.Click += new System.EventHandler(this.gitHubcomToolStripMenuItem_Click);
            // 
            // toolStripMenuItem19
            // 
            this.toolStripMenuItem19.Name = "toolStripMenuItem19";
            resources.ApplyResources(this.toolStripMenuItem19, "toolStripMenuItem19");
            this.toolStripMenuItem19.Click += new System.EventHandler(this.gitHubcomToolStripMenuItem_Click);
            // 
            // rateToolStripMenuItem1
            // 
            this.rateToolStripMenuItem1.Image = global::BulkCrapUninstaller.Properties.Resources.star;
            this.rateToolStripMenuItem1.Name = "rateToolStripMenuItem1";
            resources.ApplyResources(this.rateToolStripMenuItem1, "rateToolStripMenuItem1");
            this.rateToolStripMenuItem1.Click += new System.EventHandler(this.rateToolStripMenuItem_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            resources.ApplyResources(this.toolStripSeparator15, "toolStripSeparator15");
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Image = global::BulkCrapUninstaller.Properties.Resources.properties;
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            resources.ApplyResources(this.propertiesToolStripMenuItem, "propertiesToolStripMenuItem");
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.OpenProperties);
            // 
            // advancedOperationsToolStripMenuItem
            // 
            this.advancedOperationsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manualUninstallToolStripMenuItem,
            this.msiUninstalltoolStripMenuItem,
            this.toolStripSeparator14,
            this.renameToolStripMenuItem,
            this.disableAutostartToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator5,
            this.createBackupToolStripMenuItem,
            this.openKeyInRegeditToolStripMenuItem,
            this.toolStripSeparator32,
            this.takeOwnershipToolStripMenuItem});
            resources.ApplyResources(this.advancedOperationsToolStripMenuItem, "advancedOperationsToolStripMenuItem");
            this.advancedOperationsToolStripMenuItem.Name = "advancedOperationsToolStripMenuItem";
            this.advancedOperationsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.advancedOperationsToolStripMenuItem_DropDownOpening);
            // 
            // manualUninstallToolStripMenuItem
            // 
            this.manualUninstallToolStripMenuItem.Image = global::BulkCrapUninstaller.Properties.Resources.list;
            this.manualUninstallToolStripMenuItem.Name = "manualUninstallToolStripMenuItem";
            resources.ApplyResources(this.manualUninstallToolStripMenuItem, "manualUninstallToolStripMenuItem");
            this.manualUninstallToolStripMenuItem.Click += new System.EventHandler(this.RunAdvancedUninstall);
            // 
            // msiUninstalltoolStripMenuItem
            // 
            this.msiUninstalltoolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4});
            this.msiUninstalltoolStripMenuItem.Name = "msiUninstalltoolStripMenuItem";
            resources.ApplyResources(this.msiUninstalltoolStripMenuItem, "msiUninstalltoolStripMenuItem");
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            this.toolStripMenuItem2.Click += new System.EventHandler(this.msiInstallContextMenuStripItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            this.toolStripMenuItem3.Click += new System.EventHandler(this.msiUninstallContextMenuStripItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
            this.toolStripMenuItem4.Click += new System.EventHandler(this.msiQuietUninstallContextMenuStripItem_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            resources.ApplyResources(this.toolStripSeparator14, "toolStripSeparator14");
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            resources.ApplyResources(this.renameToolStripMenuItem, "renameToolStripMenuItem");
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.RenameEntries);
            // 
            // disableAutostartToolStripMenuItem
            // 
            this.disableAutostartToolStripMenuItem.Name = "disableAutostartToolStripMenuItem";
            resources.ApplyResources(this.disableAutostartToolStripMenuItem, "disableAutostartToolStripMenuItem");
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
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // createBackupToolStripMenuItem
            // 
            this.createBackupToolStripMenuItem.Name = "createBackupToolStripMenuItem";
            resources.ApplyResources(this.createBackupToolStripMenuItem, "createBackupToolStripMenuItem");
            this.createBackupToolStripMenuItem.Click += new System.EventHandler(this.CreateRegistryBackup);
            // 
            // openKeyInRegeditToolStripMenuItem
            // 
            this.openKeyInRegeditToolStripMenuItem.Name = "openKeyInRegeditToolStripMenuItem";
            resources.ApplyResources(this.openKeyInRegeditToolStripMenuItem, "openKeyInRegeditToolStripMenuItem");
            this.openKeyInRegeditToolStripMenuItem.Click += new System.EventHandler(this.OpenInRegedit);
            // 
            // toolStripSeparator32
            // 
            this.toolStripSeparator32.Name = "toolStripSeparator32";
            resources.ApplyResources(this.toolStripSeparator32, "toolStripSeparator32");
            // 
            // takeOwnershipToolStripMenuItem
            // 
            this.takeOwnershipToolStripMenuItem.Name = "takeOwnershipToolStripMenuItem";
            resources.ApplyResources(this.takeOwnershipToolStripMenuItem, "takeOwnershipToolStripMenuItem");
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openStartupManagerToolStripMenuItem,
            this.toolStripSeparator25,
            this.cleanUpProgramFilesToolStripMenuItem,
            this.targetMenuItem,
            this.uninstallFromDirectoryToolStripMenuItem,
            this.toolStripSeparator13,
            this.troubleshootUninstallProblemsToolStripMenuItem,
            this.startDiskCleanupToolStripMenuItem,
            this.tryToInstallNETV35ToolStripMenuItem,
            this.createRestorePointToolStripMenuItem,
            this.toolStripSeparator29,
            this.openProgramsAndFeaturesToolStripMenuItem,
            this.openSystemRestoreToolStripMenuItem,
            this.toolStripSeparator19,
            this.settingsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            resources.ApplyResources(this.toolsToolStripMenuItem, "toolsToolStripMenuItem");
            this.toolsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.toolsToolStripMenuItem_DropDownOpening);
            // 
            // openStartupManagerToolStripMenuItem
            // 
            resources.ApplyResources(this.openStartupManagerToolStripMenuItem, "openStartupManagerToolStripMenuItem");
            this.openStartupManagerToolStripMenuItem.Name = "openStartupManagerToolStripMenuItem";
            this.openStartupManagerToolStripMenuItem.Click += new System.EventHandler(this.openStartupManagerToolStripMenuItem_Click);
            // 
            // toolStripSeparator25
            // 
            this.toolStripSeparator25.Name = "toolStripSeparator25";
            resources.ApplyResources(this.toolStripSeparator25, "toolStripSeparator25");
            // 
            // cleanUpProgramFilesToolStripMenuItem
            // 
            resources.ApplyResources(this.cleanUpProgramFilesToolStripMenuItem, "cleanUpProgramFilesToolStripMenuItem");
            this.cleanUpProgramFilesToolStripMenuItem.Name = "cleanUpProgramFilesToolStripMenuItem";
            this.cleanUpProgramFilesToolStripMenuItem.Click += new System.EventHandler(this.cleanUpProgramFilesToolStripMenuItem_Click);
            // 
            // targetMenuItem
            // 
            this.targetMenuItem.Image = global::BulkCrapUninstaller.Properties.Resources.target;
            resources.ApplyResources(this.targetMenuItem, "targetMenuItem");
            this.targetMenuItem.Name = "targetMenuItem";
            this.targetMenuItem.Click += new System.EventHandler(this.OpenTargetWindow);
            // 
            // uninstallFromDirectoryToolStripMenuItem
            // 
            this.uninstallFromDirectoryToolStripMenuItem.Name = "uninstallFromDirectoryToolStripMenuItem";
            resources.ApplyResources(this.uninstallFromDirectoryToolStripMenuItem, "uninstallFromDirectoryToolStripMenuItem");
            this.uninstallFromDirectoryToolStripMenuItem.Click += new System.EventHandler(this.uninstallFromDirectoryToolStripMenuItem_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            resources.ApplyResources(this.toolStripSeparator13, "toolStripSeparator13");
            // 
            // troubleshootUninstallProblemsToolStripMenuItem
            // 
            this.troubleshootUninstallProblemsToolStripMenuItem.Name = "troubleshootUninstallProblemsToolStripMenuItem";
            resources.ApplyResources(this.troubleshootUninstallProblemsToolStripMenuItem, "troubleshootUninstallProblemsToolStripMenuItem");
            this.troubleshootUninstallProblemsToolStripMenuItem.Click += new System.EventHandler(this.troubleshootUninstallProblemsToolStripMenuItem_Click);
            // 
            // startDiskCleanupToolStripMenuItem
            // 
            this.startDiskCleanupToolStripMenuItem.Name = "startDiskCleanupToolStripMenuItem";
            resources.ApplyResources(this.startDiskCleanupToolStripMenuItem, "startDiskCleanupToolStripMenuItem");
            this.startDiskCleanupToolStripMenuItem.Click += new System.EventHandler(this.startDiskCleanupToolStripMenuItem_Click);
            // 
            // tryToInstallNETV35ToolStripMenuItem
            // 
            this.tryToInstallNETV35ToolStripMenuItem.Name = "tryToInstallNETV35ToolStripMenuItem";
            resources.ApplyResources(this.tryToInstallNETV35ToolStripMenuItem, "tryToInstallNETV35ToolStripMenuItem");
            this.tryToInstallNETV35ToolStripMenuItem.Click += new System.EventHandler(this.tryToInstallNETV35ToolStripMenuItem_Click);
            // 
            // createRestorePointToolStripMenuItem
            // 
            this.createRestorePointToolStripMenuItem.Name = "createRestorePointToolStripMenuItem";
            resources.ApplyResources(this.createRestorePointToolStripMenuItem, "createRestorePointToolStripMenuItem");
            this.createRestorePointToolStripMenuItem.Click += new System.EventHandler(this.createRestorePointToolStripMenuItem_Click);
            // 
            // toolStripSeparator29
            // 
            this.toolStripSeparator29.Name = "toolStripSeparator29";
            resources.ApplyResources(this.toolStripSeparator29, "toolStripSeparator29");
            // 
            // openProgramsAndFeaturesToolStripMenuItem
            // 
            this.openProgramsAndFeaturesToolStripMenuItem.Name = "openProgramsAndFeaturesToolStripMenuItem";
            resources.ApplyResources(this.openProgramsAndFeaturesToolStripMenuItem, "openProgramsAndFeaturesToolStripMenuItem");
            this.openProgramsAndFeaturesToolStripMenuItem.Click += new System.EventHandler(this.openProgramsAndFeaturesToolStripMenuItem_Click);
            // 
            // openSystemRestoreToolStripMenuItem
            // 
            this.openSystemRestoreToolStripMenuItem.Name = "openSystemRestoreToolStripMenuItem";
            resources.ApplyResources(this.openSystemRestoreToolStripMenuItem, "openSystemRestoreToolStripMenuItem");
            this.openSystemRestoreToolStripMenuItem.Click += new System.EventHandler(this.openSystemRestoreToolStripMenuItem_Click);
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            resources.ApplyResources(this.toolStripSeparator19, "toolStripSeparator19");
            // 
            // settingsToolStripMenuItem
            // 
            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
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
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            this.helpToolStripMenuItem.DropDownOpening += new System.EventHandler(this.helpToolStripMenuItem_DropDownOpening);
            // 
            // openHelpToolStripMenuItem
            // 
            this.openHelpToolStripMenuItem.Image = global::BulkCrapUninstaller.Properties.Resources.information_circle;
            this.openHelpToolStripMenuItem.Name = "openHelpToolStripMenuItem";
            resources.ApplyResources(this.openHelpToolStripMenuItem, "openHelpToolStripMenuItem");
            this.openHelpToolStripMenuItem.Click += new System.EventHandler(this.openHelpToolStripMenuItem_Click);
            // 
            // startSetupWizardToolStripMenuItem
            // 
            this.startSetupWizardToolStripMenuItem.Name = "startSetupWizardToolStripMenuItem";
            resources.ApplyResources(this.startSetupWizardToolStripMenuItem, "startSetupWizardToolStripMenuItem");
            this.startSetupWizardToolStripMenuItem.Click += new System.EventHandler(this.OnClickStartSetupWizard);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            resources.ApplyResources(this.toolStripSeparator16, "toolStripSeparator16");
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            resources.ApplyResources(this.checkForUpdatesToolStripMenuItem, "checkForUpdatesToolStripMenuItem");
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
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            resources.ApplyResources(this.toolStripSeparator18, "toolStripSeparator18");
            // 
            // resetSettingsToolStripMenuItem
            // 
            this.resetSettingsToolStripMenuItem.Name = "resetSettingsToolStripMenuItem";
            resources.ApplyResources(this.resetSettingsToolStripMenuItem, "resetSettingsToolStripMenuItem");
            this.resetSettingsToolStripMenuItem.Click += new System.EventHandler(this.ResetSettingsDialog);
            // 
            // uninstallBCUninstallToolstripMenuItem
            // 
            this.uninstallBCUninstallToolstripMenuItem.Name = "uninstallBCUninstallToolstripMenuItem";
            resources.ApplyResources(this.uninstallBCUninstallToolstripMenuItem, "uninstallBCUninstallToolstripMenuItem");
            this.uninstallBCUninstallToolstripMenuItem.Click += new System.EventHandler(this.uninstallBCUninstallToolstripMenuItem_Click);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            resources.ApplyResources(this.toolStripSeparator17, "toolStripSeparator17");
            // 
            // aboutToolStripMenuItem
            // 
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            resources.ApplyResources(this.debugToolStripMenuItem, "debugToolStripMenuItem");
            this.debugToolStripMenuItem.Click += new System.EventHandler(this.OpenDebugWindow);
            // 
            // createBackupFileDialog
            // 
            this.createBackupFileDialog.DefaultExt = "reg";
            this.createBackupFileDialog.FileName = "New Uninstaller Backup";
            resources.ApplyResources(this.createBackupFileDialog, "createBackupFileDialog");
            this.createBackupFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.createBackupFileDialog_FileOk);
            // 
            // globalHotkeys1
            // 
            this.globalHotkeys1.ContainerControl = this;
            this.globalHotkeys1.StopWhenFormIsDisabled = true;
            this.globalHotkeys1.SuppressKeyPresses = true;
            // 
            // splashScreen1
            // 
            this.splashScreen1.AutomaticallyClose = false;
            this.splashScreen1.ContainerControl = this;
            this.splashScreen1.SplashScreenImage = global::BulkCrapUninstaller.Properties.Resources._bcu_logo;
            // 
            // usageTracker
            // 
            this.usageTracker.ContainerControl = this;
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
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
            this.Shown += new System.EventHandler(this.MainWindow_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainerListAndMap.Panel1.ResumeLayout(false);
            this.splitContainerListAndMap.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerListAndMap)).EndInit();
            this.splitContainerListAndMap.ResumeLayout(false);
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
        private UsageTracker usageTracker;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripLabelStatus;
        private ToolStripStatusLabel toolStripLabelSize;
        private ToolStripStatusLabel toolStripLabelTotal;
        private ToolStripMenuItem displayStatusbarToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem openProgramsAndFeaturesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator13;
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
        private ToolStripSeparator toolStripSeparator19;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem cleanUpProgramFilesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator20;
        private ToolStripMenuItem searchToolStripMenuItem;
        internal Klocman.Subsystems.GlobalHotkeys globalHotkeys1;
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
        private ToolStripButton toolStripButtonTarget;
        private ToolStripSeparator toolStripSeparator23;
        private ToolStripMenuItem viewWindowsStoreAppsToolStripMenuItem;
        internal UninstallTools.Controls.FilterEditor filterEditor1;
        private Button buttonAdvFiltering;
        private ToolStripSeparator toolStripSeparator24;
        private ToolStripButton toolStripButton7;
        private ToolStripButton toolStripButton8;
        private AdvancedFilters advancedFilters1;
        private ToolStripMenuItem advancedClipCopyToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem9;
        private ToolStripMenuItem viewWindowsFeaturesToolStripMenuItem;
        private ToolStripMenuItem uninstallFromDirectoryToolStripMenuItem;
        private ToolStripMenuItem googleToolStripMenuItem;
        private ToolStripMenuItem alternativeToToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem15;
        private ToolStripMenuItem toolStripMenuItem16;
        private ToolStripMenuItem openSystemRestoreToolStripMenuItem;
        private Klocman.Forms.SplashScreen splashScreen1;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem runToolStripMenuItem;
        private ToolStripMenuItem viewUpdatesToolStripMenuItem;
        private ToolStripMenuItem targetMenuItem;
        private ToolStripSeparator toolStripSeparator25;
        private ToolStripSeparator toolStripSeparator26;
        private ToolStripMenuItem fossHubcomToolStripMenuItem;
        private ToolStripMenuItem sourceForgecomToolStripMenuItem;
        private ToolStripMenuItem fileHippocomToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator27;
        private ToolStripMenuItem toolStripMenuItem17;
        private ToolStripMenuItem toolStripMenuItem18;
        private ToolStripMenuItem toolStripMenuItem19;
        private ToolStripMenuItem gitHubcomToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem20;
        private ToolStripButton toolStripButtonModify;
        private ToolStripMenuItem modifyToolStripMenuItem;
        private ToolStripMenuItem excludeToolStripMenuItem;
        private ToolStripMenuItem includeToolStripMenuItem;
        private ToolStripSeparator toolStripSeparatorFiltering;
        private SplitContainer splitContainerListAndMap;
        private SimpleTreeMap.TreeMap treeMap1;
        private ToolStripMenuItem showTreemapToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator28;
        private ToolStripMenuItem viewUnregisteredToolStripMenuItem;
        private ToolStripMenuItem tryToInstallNETV35ToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator29;
        private ToolStripMenuItem startDiskCleanupToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator30;
        private ToolStripMenuItem exportStoreAppsToPowerShellRemoveScriptToolStripMenuItem;
        private ToolStripMenuItem exportToABatchUninstallScriptToolStripMenuItem;
        private ToolStripMenuItem troubleshootUninstallProblemsToolStripMenuItem;
        private ToolStripMenuItem filteringToolStripMenuItem;
        private ToolStripMenuItem basicApplicationsToolStripMenuItem;
        private ToolStripMenuItem advancedApplicationsToolStripMenuItem;
        private ToolStripMenuItem systemComponentsToolStripMenuItem;
        private ToolStripMenuItem automaticallyStartedToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator31;
        private ToolStripMenuItem everythingToolStripMenuItem;
        private ToolStripMenuItem onlyWebBrowsersToolStripMenuItem;
        private ToolStripMenuItem viewTweaksToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator32;
        private ToolStripMenuItem takeOwnershipToolStripMenuItem;
        private ToolStripMenuItem slantcoToolStripMenuItem;
        private ToolStripMenuItem slantcoToolStripMenuItem1;
        private ToolStripMenuItem createRestorePointToolStripMenuItem;
    }
}

