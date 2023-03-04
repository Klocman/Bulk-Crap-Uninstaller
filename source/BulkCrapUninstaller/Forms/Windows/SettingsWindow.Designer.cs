using System.ComponentModel;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions.Tracking;

namespace BulkCrapUninstaller.Forms
{
    partial class SettingsWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsWindow));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.textBoxPreUninstall = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxPostUninstall = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBoxMisc = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxColorblind = new System.Windows.Forms.CheckBox();
            this.checkBoxDpiaware = new System.Windows.Forms.CheckBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.comboBoxDoubleClick = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxAutoLoad = new System.Windows.Forms.CheckBox();
            this.checkBoxRatings = new System.Windows.Forms.CheckBox();
            this.checkBoxUpdateSearch = new System.Windows.Forms.CheckBox();
            this.checkBoxSendStats = new System.Windows.Forms.CheckBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.comboBoxLanguage = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBoxMessages = new System.Windows.Forms.GroupBox();
            this.groupBoxBackup = new System.Windows.Forms.GroupBox();
            this.directorySelectBoxBackup = new Klocman.Controls.DirectorySelectBox();
            this.flowLayoutPanel9 = new System.Windows.Forms.FlowLayoutPanel();
            this.radioButtonBackupAsk = new System.Windows.Forms.RadioButton();
            this.radioButtonBackupNever = new System.Windows.Forms.RadioButton();
            this.radioButtonBackupAuto = new System.Windows.Forms.RadioButton();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxShowAllBadJunk = new System.Windows.Forms.CheckBox();
            this.checkBoxLoud = new System.Windows.Forms.CheckBox();
            this.checkBoxNeverFeedback = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.comboBoxJunk = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.comboBoxRestore = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBoxExternal = new System.Windows.Forms.GroupBox();
            this.checkBoxEnableExternal = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label7 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.propertiesSidebar1 = new BulkCrapUninstaller.Controls.PropertiesSidebar();
            this.tabPageInterface = new System.Windows.Forms.TabPage();
            this.groupBoxLanguage = new System.Windows.Forms.GroupBox();
            this.tabPageUninstallation = new System.Windows.Forms.TabPage();
            this.uninstallationSettings1 = new BulkCrapUninstaller.Controls.UninstallationSettings();
            this.tabPageDetection = new System.Windows.Forms.TabPage();
            this.groupBoxAppStores = new System.Windows.Forms.GroupBox();
            this.labelWinUpdateInfo = new System.Windows.Forms.Label();
            this.checkBoxScanWinUpdates = new System.Windows.Forms.CheckBox();
            this.labelWinFeatureInfo = new System.Windows.Forms.Label();
            this.checkBoxScanWinFeatures = new System.Windows.Forms.CheckBox();
            this.checkBoxScanStoreApps = new System.Windows.Forms.CheckBox();
            this.checkBoxScanSteam = new System.Windows.Forms.CheckBox();
            this.checkBoxScoop = new System.Windows.Forms.CheckBox();
            this.checkBoxOculus = new System.Windows.Forms.CheckBox();
            this.checkBoxChoco = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel7 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxScanRegistry = new System.Windows.Forms.CheckBox();
            this.checkBoxScanDrives = new System.Windows.Forms.CheckBox();
            this.checkBoxPreDefined = new System.Windows.Forms.CheckBox();
            this.tabPageExternal = new System.Windows.Forms.TabPage();
            this.tabPageFolders = new System.Windows.Forms.TabPage();
            this.groupBoxProgramFolders = new System.Windows.Forms.GroupBox();
            this.textBoxProgramFolders = new System.Windows.Forms.TextBox();
            this.checkBoxRemovable = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoInstallFolderDetect = new System.Windows.Forms.CheckBox();
            this.labelProgramFolders = new System.Windows.Forms.Label();
            this.tabPageMisc = new System.Windows.Forms.TabPage();
            this.cacheSettings1 = new BulkCrapUninstaller.Controls.Settings.CacheSettings();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel10 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.usageTracker1 = new BulkCrapUninstaller.Functions.Tracking.UsageTracker();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBoxMisc.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBoxMessages.SuspendLayout();
            this.groupBoxBackup.SuspendLayout();
            this.flowLayoutPanel9.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBoxExternal.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.tabPageInterface.SuspendLayout();
            this.groupBoxLanguage.SuspendLayout();
            this.tabPageUninstallation.SuspendLayout();
            this.tabPageDetection.SuspendLayout();
            this.groupBoxAppStores.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel7.SuspendLayout();
            this.tabPageExternal.SuspendLayout();
            this.tabPageFolders.SuspendLayout();
            this.groupBoxProgramFolders.SuspendLayout();
            this.tabPageMisc.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.flowLayoutPanel10.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.textBoxPreUninstall);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.textBoxPostUninstall);
            this.splitContainer1.Panel2.Controls.Add(this.label6);
            // 
            // textBoxPreUninstall
            // 
            resources.ApplyResources(this.textBoxPreUninstall, "textBoxPreUninstall");
            this.textBoxPreUninstall.Name = "textBoxPreUninstall";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // textBoxPostUninstall
            // 
            resources.ApplyResources(this.textBoxPostUninstall, "textBoxPostUninstall");
            this.textBoxPostUninstall.Name = "textBoxPostUninstall";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // groupBoxMisc
            // 
            resources.ApplyResources(this.groupBoxMisc, "groupBoxMisc");
            this.groupBoxMisc.Controls.Add(this.flowLayoutPanel3);
            this.groupBoxMisc.Controls.Add(this.panel5);
            this.groupBoxMisc.Name = "groupBoxMisc";
            this.groupBoxMisc.TabStop = false;
            // 
            // flowLayoutPanel3
            // 
            resources.ApplyResources(this.flowLayoutPanel3, "flowLayoutPanel3");
            this.flowLayoutPanel3.Controls.Add(this.checkBoxColorblind);
            this.flowLayoutPanel3.Controls.Add(this.checkBoxDpiaware);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            // 
            // checkBoxColorblind
            // 
            resources.ApplyResources(this.checkBoxColorblind, "checkBoxColorblind");
            this.checkBoxColorblind.Name = "checkBoxColorblind";
            this.checkBoxColorblind.UseVisualStyleBackColor = true;
            // 
            // checkBoxDpiaware
            // 
            resources.ApplyResources(this.checkBoxDpiaware, "checkBoxDpiaware");
            this.checkBoxDpiaware.Name = "checkBoxDpiaware";
            this.checkBoxDpiaware.UseVisualStyleBackColor = true;
            // 
            // panel5
            // 
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Controls.Add(this.comboBoxDoubleClick);
            this.panel5.Controls.Add(this.label3);
            this.panel5.Name = "panel5";
            // 
            // comboBoxDoubleClick
            // 
            resources.ApplyResources(this.comboBoxDoubleClick, "comboBoxDoubleClick");
            this.comboBoxDoubleClick.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDoubleClick.FormattingEnabled = true;
            this.comboBoxDoubleClick.Name = "comboBoxDoubleClick";
            this.comboBoxDoubleClick.SelectedIndexChanged += new System.EventHandler(this.comboBoxDoubleClick_SelectedIndexChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // checkBoxAutoLoad
            // 
            resources.ApplyResources(this.checkBoxAutoLoad, "checkBoxAutoLoad");
            this.checkBoxAutoLoad.Name = "checkBoxAutoLoad";
            this.checkBoxAutoLoad.UseVisualStyleBackColor = true;
            // 
            // checkBoxRatings
            // 
            resources.ApplyResources(this.checkBoxRatings, "checkBoxRatings");
            this.checkBoxRatings.Name = "checkBoxRatings";
            this.checkBoxRatings.UseVisualStyleBackColor = true;
            // 
            // checkBoxUpdateSearch
            // 
            resources.ApplyResources(this.checkBoxUpdateSearch, "checkBoxUpdateSearch");
            this.checkBoxUpdateSearch.Name = "checkBoxUpdateSearch";
            this.checkBoxUpdateSearch.UseVisualStyleBackColor = true;
            // 
            // checkBoxSendStats
            // 
            resources.ApplyResources(this.checkBoxSendStats, "checkBoxSendStats");
            this.checkBoxSendStats.Name = "checkBoxSendStats";
            this.checkBoxSendStats.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Controls.Add(this.comboBoxLanguage);
            this.panel3.Controls.Add(this.label9);
            this.panel3.Name = "panel3";
            // 
            // comboBoxLanguage
            // 
            resources.ApplyResources(this.comboBoxLanguage, "comboBoxLanguage");
            this.comboBoxLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLanguage.FormattingEnabled = true;
            this.comboBoxLanguage.Name = "comboBoxLanguage";
            this.comboBoxLanguage.SelectedIndexChanged += new System.EventHandler(this.comboBoxLanguage_SelectedIndexChanged);
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // groupBoxMessages
            // 
            resources.ApplyResources(this.groupBoxMessages, "groupBoxMessages");
            this.groupBoxMessages.Controls.Add(this.groupBoxBackup);
            this.groupBoxMessages.Controls.Add(this.flowLayoutPanel1);
            this.groupBoxMessages.Controls.Add(this.panel1);
            this.groupBoxMessages.Controls.Add(this.panel2);
            this.groupBoxMessages.Name = "groupBoxMessages";
            this.groupBoxMessages.TabStop = false;
            // 
            // groupBoxBackup
            // 
            resources.ApplyResources(this.groupBoxBackup, "groupBoxBackup");
            this.groupBoxBackup.Controls.Add(this.directorySelectBoxBackup);
            this.groupBoxBackup.Controls.Add(this.flowLayoutPanel9);
            this.groupBoxBackup.Name = "groupBoxBackup";
            this.groupBoxBackup.TabStop = false;
            // 
            // directorySelectBoxBackup
            // 
            this.directorySelectBoxBackup.DirectoryPath = "";
            resources.ApplyResources(this.directorySelectBoxBackup, "directorySelectBoxBackup");
            this.directorySelectBoxBackup.Name = "directorySelectBoxBackup";
            // 
            // flowLayoutPanel9
            // 
            resources.ApplyResources(this.flowLayoutPanel9, "flowLayoutPanel9");
            this.flowLayoutPanel9.Controls.Add(this.radioButtonBackupAsk);
            this.flowLayoutPanel9.Controls.Add(this.radioButtonBackupNever);
            this.flowLayoutPanel9.Controls.Add(this.radioButtonBackupAuto);
            this.flowLayoutPanel9.Name = "flowLayoutPanel9";
            // 
            // radioButtonBackupAsk
            // 
            resources.ApplyResources(this.radioButtonBackupAsk, "radioButtonBackupAsk");
            this.flowLayoutPanel9.SetFlowBreak(this.radioButtonBackupAsk, true);
            this.radioButtonBackupAsk.Name = "radioButtonBackupAsk";
            this.radioButtonBackupAsk.TabStop = true;
            this.radioButtonBackupAsk.UseVisualStyleBackColor = true;
            this.radioButtonBackupAsk.CheckedChanged += new System.EventHandler(this.radioButtonBackup_CheckedChanged);
            // 
            // radioButtonBackupNever
            // 
            resources.ApplyResources(this.radioButtonBackupNever, "radioButtonBackupNever");
            this.flowLayoutPanel9.SetFlowBreak(this.radioButtonBackupNever, true);
            this.radioButtonBackupNever.Name = "radioButtonBackupNever";
            this.radioButtonBackupNever.TabStop = true;
            this.radioButtonBackupNever.UseVisualStyleBackColor = true;
            this.radioButtonBackupNever.CheckedChanged += new System.EventHandler(this.radioButtonBackup_CheckedChanged);
            // 
            // radioButtonBackupAuto
            // 
            resources.ApplyResources(this.radioButtonBackupAuto, "radioButtonBackupAuto");
            this.flowLayoutPanel9.SetFlowBreak(this.radioButtonBackupAuto, true);
            this.radioButtonBackupAuto.Name = "radioButtonBackupAuto";
            this.radioButtonBackupAuto.TabStop = true;
            this.radioButtonBackupAuto.UseVisualStyleBackColor = true;
            this.radioButtonBackupAuto.CheckedChanged += new System.EventHandler(this.radioButtonBackup_CheckedChanged);
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.checkBoxShowAllBadJunk);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxLoud);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxNeverFeedback);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // checkBoxShowAllBadJunk
            // 
            resources.ApplyResources(this.checkBoxShowAllBadJunk, "checkBoxShowAllBadJunk");
            this.checkBoxShowAllBadJunk.Name = "checkBoxShowAllBadJunk";
            this.checkBoxShowAllBadJunk.UseVisualStyleBackColor = true;
            // 
            // checkBoxLoud
            // 
            resources.ApplyResources(this.checkBoxLoud, "checkBoxLoud");
            this.checkBoxLoud.Name = "checkBoxLoud";
            this.checkBoxLoud.UseVisualStyleBackColor = true;
            // 
            // checkBoxNeverFeedback
            // 
            resources.ApplyResources(this.checkBoxNeverFeedback, "checkBoxNeverFeedback");
            this.checkBoxNeverFeedback.Name = "checkBoxNeverFeedback";
            this.checkBoxNeverFeedback.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.comboBoxJunk);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Name = "panel1";
            // 
            // comboBoxJunk
            // 
            resources.ApplyResources(this.comboBoxJunk, "comboBoxJunk");
            this.comboBoxJunk.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxJunk.FormattingEnabled = true;
            this.comboBoxJunk.Name = "comboBoxJunk";
            this.comboBoxJunk.SelectedIndexChanged += new System.EventHandler(this.comboBoxJunk_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.comboBoxRestore);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Name = "panel2";
            // 
            // comboBoxRestore
            // 
            resources.ApplyResources(this.comboBoxRestore, "comboBoxRestore");
            this.comboBoxRestore.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRestore.FormattingEnabled = true;
            this.comboBoxRestore.Name = "comboBoxRestore";
            this.comboBoxRestore.SelectedIndexChanged += new System.EventHandler(this.comboBoxRestore_SelectedIndexChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // groupBoxExternal
            // 
            resources.ApplyResources(this.groupBoxExternal, "groupBoxExternal");
            this.groupBoxExternal.Controls.Add(this.splitContainer1);
            this.groupBoxExternal.Controls.Add(this.checkBoxEnableExternal);
            this.groupBoxExternal.Controls.Add(this.flowLayoutPanel2);
            this.groupBoxExternal.Name = "groupBoxExternal";
            this.groupBoxExternal.TabStop = false;
            // 
            // checkBoxEnableExternal
            // 
            resources.ApplyResources(this.checkBoxEnableExternal, "checkBoxEnableExternal");
            this.checkBoxEnableExternal.Checked = true;
            this.checkBoxEnableExternal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxEnableExternal.Name = "checkBoxEnableExternal";
            this.checkBoxEnableExternal.UseVisualStyleBackColor = true;
            this.checkBoxEnableExternal.CheckedChanged += new System.EventHandler(this.checkBoxEnableExternal_CheckedChanged);
            // 
            // flowLayoutPanel2
            // 
            resources.ApplyResources(this.flowLayoutPanel2, "flowLayoutPanel2");
            this.flowLayoutPanel2.Controls.Add(this.label7);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageGeneral);
            this.tabControl.Controls.Add(this.tabPageInterface);
            this.tabControl.Controls.Add(this.tabPageUninstallation);
            this.tabControl.Controls.Add(this.tabPageDetection);
            this.tabControl.Controls.Add(this.tabPageExternal);
            this.tabControl.Controls.Add(this.tabPageFolders);
            this.tabControl.Controls.Add(this.tabPageMisc);
            resources.ApplyResources(this.tabControl, "tabControl");
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.propertiesSidebar1);
            resources.ApplyResources(this.tabPageGeneral, "tabPageGeneral");
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // propertiesSidebar1
            // 
            resources.ApplyResources(this.propertiesSidebar1, "propertiesSidebar1");
            this.propertiesSidebar1.InvalidEnabled = true;
            this.propertiesSidebar1.Name = "propertiesSidebar1";
            this.propertiesSidebar1.OrphansEnabled = true;
            this.propertiesSidebar1.ProtectedEnabled = true;
            this.propertiesSidebar1.ShowTweaksEnabled = true;
            this.propertiesSidebar1.StoreAppsEnabled = true;
            this.propertiesSidebar1.SysCompEnabled = true;
            this.propertiesSidebar1.UpdatesEnabled = true;
            this.propertiesSidebar1.WinFeaturesEnabled = true;
            // 
            // tabPageInterface
            // 
            resources.ApplyResources(this.tabPageInterface, "tabPageInterface");
            this.tabPageInterface.Controls.Add(this.groupBoxMisc);
            this.tabPageInterface.Controls.Add(this.groupBoxMessages);
            this.tabPageInterface.Controls.Add(this.groupBoxLanguage);
            this.tabPageInterface.Name = "tabPageInterface";
            this.tabPageInterface.UseVisualStyleBackColor = true;
            // 
            // groupBoxLanguage
            // 
            resources.ApplyResources(this.groupBoxLanguage, "groupBoxLanguage");
            this.groupBoxLanguage.Controls.Add(this.panel3);
            this.groupBoxLanguage.Controls.Add(this.label10);
            this.groupBoxLanguage.Name = "groupBoxLanguage";
            this.groupBoxLanguage.TabStop = false;
            // 
            // tabPageUninstallation
            // 
            resources.ApplyResources(this.tabPageUninstallation, "tabPageUninstallation");
            this.tabPageUninstallation.Controls.Add(this.uninstallationSettings1);
            this.tabPageUninstallation.Name = "tabPageUninstallation";
            this.tabPageUninstallation.UseVisualStyleBackColor = true;
            // 
            // uninstallationSettings1
            // 
            resources.ApplyResources(this.uninstallationSettings1, "uninstallationSettings1");
            this.uninstallationSettings1.Name = "uninstallationSettings1";
            // 
            // tabPageDetection
            // 
            resources.ApplyResources(this.tabPageDetection, "tabPageDetection");
            this.tabPageDetection.Controls.Add(this.groupBoxAppStores);
            this.tabPageDetection.Controls.Add(this.groupBox1);
            this.tabPageDetection.Name = "tabPageDetection";
            this.tabPageDetection.UseVisualStyleBackColor = true;
            // 
            // groupBoxAppStores
            // 
            resources.ApplyResources(this.groupBoxAppStores, "groupBoxAppStores");
            this.groupBoxAppStores.Controls.Add(this.labelWinUpdateInfo);
            this.groupBoxAppStores.Controls.Add(this.checkBoxScanWinUpdates);
            this.groupBoxAppStores.Controls.Add(this.labelWinFeatureInfo);
            this.groupBoxAppStores.Controls.Add(this.checkBoxScanWinFeatures);
            this.groupBoxAppStores.Controls.Add(this.checkBoxScanStoreApps);
            this.groupBoxAppStores.Controls.Add(this.checkBoxScanSteam);
            this.groupBoxAppStores.Controls.Add(this.checkBoxScoop);
            this.groupBoxAppStores.Controls.Add(this.checkBoxOculus);
            this.groupBoxAppStores.Controls.Add(this.checkBoxChoco);
            this.groupBoxAppStores.Name = "groupBoxAppStores";
            this.groupBoxAppStores.TabStop = false;
            // 
            // labelWinUpdateInfo
            // 
            resources.ApplyResources(this.labelWinUpdateInfo, "labelWinUpdateInfo");
            this.labelWinUpdateInfo.Name = "labelWinUpdateInfo";
            // 
            // checkBoxScanWinUpdates
            // 
            resources.ApplyResources(this.checkBoxScanWinUpdates, "checkBoxScanWinUpdates");
            this.checkBoxScanWinUpdates.Name = "checkBoxScanWinUpdates";
            this.checkBoxScanWinUpdates.UseVisualStyleBackColor = true;
            // 
            // labelWinFeatureInfo
            // 
            resources.ApplyResources(this.labelWinFeatureInfo, "labelWinFeatureInfo");
            this.labelWinFeatureInfo.Name = "labelWinFeatureInfo";
            // 
            // checkBoxScanWinFeatures
            // 
            resources.ApplyResources(this.checkBoxScanWinFeatures, "checkBoxScanWinFeatures");
            this.checkBoxScanWinFeatures.Name = "checkBoxScanWinFeatures";
            this.checkBoxScanWinFeatures.UseVisualStyleBackColor = true;
            // 
            // checkBoxScanStoreApps
            // 
            resources.ApplyResources(this.checkBoxScanStoreApps, "checkBoxScanStoreApps");
            this.checkBoxScanStoreApps.Name = "checkBoxScanStoreApps";
            this.checkBoxScanStoreApps.UseVisualStyleBackColor = true;
            // 
            // checkBoxScanSteam
            // 
            resources.ApplyResources(this.checkBoxScanSteam, "checkBoxScanSteam");
            this.checkBoxScanSteam.Name = "checkBoxScanSteam";
            this.checkBoxScanSteam.UseVisualStyleBackColor = true;
            // 
            // checkBoxScoop
            // 
            resources.ApplyResources(this.checkBoxScoop, "checkBoxScoop");
            this.checkBoxScoop.Name = "checkBoxScoop";
            this.checkBoxScoop.UseVisualStyleBackColor = true;
            // 
            // checkBoxOculus
            // 
            resources.ApplyResources(this.checkBoxOculus, "checkBoxOculus");
            this.checkBoxOculus.Name = "checkBoxOculus";
            this.checkBoxOculus.UseVisualStyleBackColor = true;
            // 
            // checkBoxChoco
            // 
            resources.ApplyResources(this.checkBoxChoco, "checkBoxChoco");
            this.checkBoxChoco.Name = "checkBoxChoco";
            this.checkBoxChoco.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.flowLayoutPanel7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // flowLayoutPanel7
            // 
            resources.ApplyResources(this.flowLayoutPanel7, "flowLayoutPanel7");
            this.flowLayoutPanel7.Controls.Add(this.checkBoxScanRegistry);
            this.flowLayoutPanel7.Controls.Add(this.checkBoxScanDrives);
            this.flowLayoutPanel7.Controls.Add(this.checkBoxPreDefined);
            this.flowLayoutPanel7.Name = "flowLayoutPanel7";
            // 
            // checkBoxScanRegistry
            // 
            resources.ApplyResources(this.checkBoxScanRegistry, "checkBoxScanRegistry");
            this.flowLayoutPanel7.SetFlowBreak(this.checkBoxScanRegistry, true);
            this.checkBoxScanRegistry.Name = "checkBoxScanRegistry";
            this.checkBoxScanRegistry.UseVisualStyleBackColor = true;
            // 
            // checkBoxScanDrives
            // 
            resources.ApplyResources(this.checkBoxScanDrives, "checkBoxScanDrives");
            this.flowLayoutPanel7.SetFlowBreak(this.checkBoxScanDrives, true);
            this.checkBoxScanDrives.Name = "checkBoxScanDrives";
            this.checkBoxScanDrives.UseVisualStyleBackColor = true;
            // 
            // checkBoxPreDefined
            // 
            resources.ApplyResources(this.checkBoxPreDefined, "checkBoxPreDefined");
            this.flowLayoutPanel7.SetFlowBreak(this.checkBoxPreDefined, true);
            this.checkBoxPreDefined.Name = "checkBoxPreDefined";
            this.checkBoxPreDefined.UseVisualStyleBackColor = true;
            // 
            // tabPageExternal
            // 
            this.tabPageExternal.Controls.Add(this.groupBoxExternal);
            resources.ApplyResources(this.tabPageExternal, "tabPageExternal");
            this.tabPageExternal.Name = "tabPageExternal";
            this.tabPageExternal.UseVisualStyleBackColor = true;
            // 
            // tabPageFolders
            // 
            this.tabPageFolders.Controls.Add(this.groupBoxProgramFolders);
            resources.ApplyResources(this.tabPageFolders, "tabPageFolders");
            this.tabPageFolders.Name = "tabPageFolders";
            this.tabPageFolders.UseVisualStyleBackColor = true;
            // 
            // groupBoxProgramFolders
            // 
            resources.ApplyResources(this.groupBoxProgramFolders, "groupBoxProgramFolders");
            this.groupBoxProgramFolders.Controls.Add(this.textBoxProgramFolders);
            this.groupBoxProgramFolders.Controls.Add(this.checkBoxRemovable);
            this.groupBoxProgramFolders.Controls.Add(this.checkBoxAutoInstallFolderDetect);
            this.groupBoxProgramFolders.Controls.Add(this.labelProgramFolders);
            this.groupBoxProgramFolders.Name = "groupBoxProgramFolders";
            this.groupBoxProgramFolders.TabStop = false;
            // 
            // textBoxProgramFolders
            // 
            resources.ApplyResources(this.textBoxProgramFolders, "textBoxProgramFolders");
            this.textBoxProgramFolders.Name = "textBoxProgramFolders";
            // 
            // checkBoxRemovable
            // 
            resources.ApplyResources(this.checkBoxRemovable, "checkBoxRemovable");
            this.checkBoxRemovable.Name = "checkBoxRemovable";
            this.checkBoxRemovable.UseVisualStyleBackColor = true;
            // 
            // checkBoxAutoInstallFolderDetect
            // 
            resources.ApplyResources(this.checkBoxAutoInstallFolderDetect, "checkBoxAutoInstallFolderDetect");
            this.checkBoxAutoInstallFolderDetect.Name = "checkBoxAutoInstallFolderDetect";
            this.checkBoxAutoInstallFolderDetect.UseVisualStyleBackColor = true;
            // 
            // labelProgramFolders
            // 
            resources.ApplyResources(this.labelProgramFolders, "labelProgramFolders");
            this.labelProgramFolders.Name = "labelProgramFolders";
            // 
            // tabPageMisc
            // 
            this.tabPageMisc.Controls.Add(this.cacheSettings1);
            this.tabPageMisc.Controls.Add(this.groupBox2);
            this.tabPageMisc.Controls.Add(this.groupBox3);
            resources.ApplyResources(this.tabPageMisc, "tabPageMisc");
            this.tabPageMisc.Name = "tabPageMisc";
            this.tabPageMisc.UseVisualStyleBackColor = true;
            // 
            // cacheSettings1
            // 
            resources.ApplyResources(this.cacheSettings1, "cacheSettings1");
            this.cacheSettings1.Name = "cacheSettings1";
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.flowLayoutPanel4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // flowLayoutPanel4
            // 
            resources.ApplyResources(this.flowLayoutPanel4, "flowLayoutPanel4");
            this.flowLayoutPanel4.Controls.Add(this.checkBoxAutoLoad);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.flowLayoutPanel10);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // flowLayoutPanel10
            // 
            resources.ApplyResources(this.flowLayoutPanel10, "flowLayoutPanel10");
            this.flowLayoutPanel10.Controls.Add(this.checkBoxUpdateSearch);
            this.flowLayoutPanel10.Controls.Add(this.checkBoxSendStats);
            this.flowLayoutPanel10.Controls.Add(this.checkBoxRatings);
            this.flowLayoutPanel10.Name = "flowLayoutPanel10";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.button2);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // usageTracker1
            // 
            this.usageTracker1.ContainerControl = this;
            // 
            // SettingsWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button2;
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.panel4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsWindow_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBoxMisc.ResumeLayout(false);
            this.groupBoxMisc.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.groupBoxMessages.ResumeLayout(false);
            this.groupBoxMessages.PerformLayout();
            this.groupBoxBackup.ResumeLayout(false);
            this.groupBoxBackup.PerformLayout();
            this.flowLayoutPanel9.ResumeLayout(false);
            this.flowLayoutPanel9.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBoxExternal.ResumeLayout(false);
            this.groupBoxExternal.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageGeneral.PerformLayout();
            this.tabPageInterface.ResumeLayout(false);
            this.tabPageInterface.PerformLayout();
            this.groupBoxLanguage.ResumeLayout(false);
            this.groupBoxLanguage.PerformLayout();
            this.tabPageUninstallation.ResumeLayout(false);
            this.tabPageUninstallation.PerformLayout();
            this.tabPageDetection.ResumeLayout(false);
            this.tabPageDetection.PerformLayout();
            this.groupBoxAppStores.ResumeLayout(false);
            this.groupBoxAppStores.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.flowLayoutPanel7.ResumeLayout(false);
            this.flowLayoutPanel7.PerformLayout();
            this.tabPageExternal.ResumeLayout(false);
            this.tabPageExternal.PerformLayout();
            this.tabPageFolders.ResumeLayout(false);
            this.tabPageFolders.PerformLayout();
            this.groupBoxProgramFolders.ResumeLayout(false);
            this.groupBoxProgramFolders.PerformLayout();
            this.tabPageMisc.ResumeLayout(false);
            this.tabPageMisc.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.flowLayoutPanel10.ResumeLayout(false);
            this.flowLayoutPanel10.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox groupBoxMessages;
        private CheckBox checkBoxLoud;
        private ComboBox comboBoxRestore;
        private ComboBox comboBoxJunk;
        private Label label2;
        private Label label1;
        private Button button2;
        private GroupBox groupBoxMisc;
        private CheckBox checkBoxUpdateSearch;
        private UsageTracker usageTracker1;
        private GroupBox groupBoxExternal;
        private FlowLayoutPanel flowLayoutPanel1;
        private Panel panel1;
        private Panel panel2;
        private FlowLayoutPanel flowLayoutPanel3;
        private CheckBox checkBoxSendStats;
        private FlowLayoutPanel flowLayoutPanel2;
        private Label label5;
        private TextBox textBoxPreUninstall;
        private Label label6;
        private TextBox textBoxPostUninstall;
        private Label label7;
        private CheckBox checkBoxEnableExternal;
        private Panel panel3;
        private ComboBox comboBoxLanguage;
        private Label label9;
        private Label label10;
        private SplitContainer splitContainer1;
        private TabControl tabControl;
        private TabPage tabPageMisc;
        private TabPage tabPageExternal;
        private Panel panel4;
        private TabPage tabPageGeneral;
        private Controls.PropertiesSidebar propertiesSidebar1;
        private TabPage tabPageFolders;
        private GroupBox groupBoxProgramFolders;
        private Label labelProgramFolders;
        private TextBox textBoxProgramFolders;
        private CheckBox checkBoxAutoLoad;
        private CheckBox checkBoxRatings;
        private TabPage tabPageUninstallation;
        private Controls.UninstallationSettings uninstallationSettings1;
        private CheckBox checkBoxShowAllBadJunk;
        private CheckBox checkBoxNeverFeedback;
        private TabPage tabPageDetection;
        private GroupBox groupBoxAppStores;
        private CheckBox checkBoxScanSteam;
        private CheckBox checkBoxScanStoreApps;
        private CheckBox checkBoxScanWinFeatures;
        private CheckBox checkBoxScanWinUpdates;
        private Label labelWinFeatureInfo;
        private Label labelWinUpdateInfo;
        private CheckBox checkBoxAutoInstallFolderDetect;
        private GroupBox groupBox1;
        private FlowLayoutPanel flowLayoutPanel7;
        private CheckBox checkBoxScanRegistry;
        private CheckBox checkBoxScanDrives;
        private CheckBox checkBoxPreDefined;
        private TabPage tabPageInterface;
        private GroupBox groupBoxLanguage;
        private GroupBox groupBoxBackup;
        private FlowLayoutPanel flowLayoutPanel9;
        private RadioButton radioButtonBackupAsk;
        private RadioButton radioButtonBackupNever;
        private RadioButton radioButtonBackupAuto;
        private Klocman.Controls.DirectorySelectBox directorySelectBoxBackup;
        private Controls.Settings.CacheSettings cacheSettings1;
        private GroupBox groupBox3;
        private FlowLayoutPanel flowLayoutPanel10;
        private CheckBox checkBoxChoco;
        private CheckBox checkBoxOculus;
        private CheckBox checkBoxRemovable;
        private CheckBox checkBoxColorblind;
        private CheckBox checkBoxScoop;
        private CheckBox checkBoxDpiaware;
        private GroupBox groupBox2;
        private FlowLayoutPanel flowLayoutPanel4;
        private Panel panel5;
        private ComboBox comboBoxDoubleClick;
        private Label label3;
    }
}