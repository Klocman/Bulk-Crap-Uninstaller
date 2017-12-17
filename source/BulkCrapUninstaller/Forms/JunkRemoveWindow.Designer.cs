using System.ComponentModel;
using System.Windows.Forms;
using BrightIdeasSoftware;
using BulkCrapUninstaller.Functions.Tracking;

namespace BulkCrapUninstaller.Forms
{
    partial class JunkRemoveWindow
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JunkRemoveWindow));
            this.exportDialog = new System.Windows.Forms.SaveFileDialog();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonAccept = new System.Windows.Forms.Button();
            this.buttonExport = new System.Windows.Forms.Button();
            this.comboBoxChecker = new System.Windows.Forms.ComboBox();
            this.checkBoxHideLowConfidence = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.objectListViewMain = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnPath = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnSafety = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnUninstallerName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.headerIntro = new System.Windows.Forms.Label();
            this.headerConfTitle = new System.Windows.Forms.Label();
            this.headerConfInfo = new System.Windows.Forms.Label();
            this.usageTracker1 = new BulkCrapUninstaller.Functions.Tracking.UsageTracker();
            this.listViewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objectListViewMain)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.listViewContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // exportDialog
            // 
            this.exportDialog.DefaultExt = "txt";
            this.exportDialog.FileName = "New BCUninstaller Junk Export";
            resources.ApplyResources(this.exportDialog, "exportDialog");
            this.exportDialog.RestoreDirectory = true;
            this.exportDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.exportDialog_FileOk);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonAccept
            // 
            resources.ApplyResources(this.buttonAccept, "buttonAccept");
            this.buttonAccept.Name = "buttonAccept";
            this.buttonAccept.UseVisualStyleBackColor = true;
            this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
            // 
            // buttonExport
            // 
            resources.ApplyResources(this.buttonExport, "buttonExport");
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // comboBoxChecker
            // 
            this.comboBoxChecker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxChecker.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxChecker, "comboBoxChecker");
            this.comboBoxChecker.Name = "comboBoxChecker";
            this.comboBoxChecker.DropDown += new System.EventHandler(this.comboBoxChecker_DropDown);
            this.comboBoxChecker.SelectedIndexChanged += new System.EventHandler(this.comboBoxChecker_SelectedIndexChanged);
            this.comboBoxChecker.DropDownClosed += new System.EventHandler(this.comboBoxChecker_DropDownClosed);
            // 
            // checkBoxHideLowConfidence
            // 
            resources.ApplyResources(this.checkBoxHideLowConfidence, "checkBoxHideLowConfidence");
            this.checkBoxHideLowConfidence.Checked = true;
            this.checkBoxHideLowConfidence.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxHideLowConfidence.Name = "checkBoxHideLowConfidence";
            this.checkBoxHideLowConfidence.UseVisualStyleBackColor = true;
            this.checkBoxHideLowConfidence.CheckedChanged += new System.EventHandler(this.checkBoxHideLowConfidence_CheckedChanged);
            this.checkBoxHideLowConfidence.Click += new System.EventHandler(this.checkBoxHideLowConfidence_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonAccept);
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Controls.Add(this.checkBoxHideLowConfidence);
            this.panel1.Controls.Add(this.comboBoxChecker);
            this.panel1.Controls.Add(this.buttonExport);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.objectListViewMain);
            this.groupBox1.Controls.Add(this.flowLayoutPanel1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // objectListViewMain
            // 
            this.objectListViewMain.AllColumns.Add(this.olvColumnPath);
            this.objectListViewMain.AllColumns.Add(this.olvColumnSafety);
            this.objectListViewMain.AllColumns.Add(this.olvColumnUninstallerName);
            this.objectListViewMain.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.objectListViewMain.CellEditUseWholeCell = false;
            this.objectListViewMain.CheckBoxes = true;
            this.objectListViewMain.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnPath,
            this.olvColumnSafety,
            this.olvColumnUninstallerName});
            this.objectListViewMain.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.objectListViewMain, "objectListViewMain");
            this.objectListViewMain.FullRowSelect = true;
            this.objectListViewMain.GridLines = true;
            this.objectListViewMain.HideSelection = false;
            this.objectListViewMain.Name = "objectListViewMain";
            this.objectListViewMain.ShowItemToolTips = true;
            this.objectListViewMain.UseCompatibleStateImageBehavior = false;
            this.objectListViewMain.View = System.Windows.Forms.View.Details;
            this.objectListViewMain.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.objectListViewMain_CellEditStarting);
            this.objectListViewMain.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.objectListViewMain_CellRightClick);
            // 
            // olvColumnPath
            // 
            this.olvColumnPath.AspectName = "";
            resources.ApplyResources(this.olvColumnPath, "olvColumnPath");
            // 
            // olvColumnSafety
            // 
            this.olvColumnSafety.AspectName = "";
            resources.ApplyResources(this.olvColumnSafety, "olvColumnSafety");
            // 
            // olvColumnUninstallerName
            // 
            this.olvColumnUninstallerName.AspectName = "";
            resources.ApplyResources(this.olvColumnUninstallerName, "olvColumnUninstallerName");
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.headerIntro);
            this.flowLayoutPanel1.Controls.Add(this.headerConfTitle);
            this.flowLayoutPanel1.Controls.Add(this.headerConfInfo);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // headerIntro
            // 
            resources.ApplyResources(this.headerIntro, "headerIntro");
            this.headerIntro.Name = "headerIntro";
            // 
            // headerConfTitle
            // 
            resources.ApplyResources(this.headerConfTitle, "headerConfTitle");
            this.headerConfTitle.Name = "headerConfTitle";
            // 
            // headerConfInfo
            // 
            resources.ApplyResources(this.headerConfInfo, "headerConfInfo");
            this.headerConfInfo.Name = "headerConfInfo";
            // 
            // usageTracker1
            // 
            this.usageTracker1.ContainerControl = this;
            // 
            // listViewContextMenuStrip
            // 
            this.listViewContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.copyToClipboardToolStripMenuItem,
            this.detailsToolStripMenuItem});
            this.listViewContextMenuStrip.Name = "listViewContextMenuStrip";
            resources.ApplyResources(this.listViewContextMenuStrip, "listViewContextMenuStrip");
            // 
            // openToolStripMenuItem
            // 
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            this.openToolStripMenuItem.Image = global::BulkCrapUninstaller.Properties.Resources.folderopen;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // copyToClipboardToolStripMenuItem
            // 
            this.copyToClipboardToolStripMenuItem.Image = global::BulkCrapUninstaller.Properties.Resources.pagecopy;
            this.copyToClipboardToolStripMenuItem.Name = "copyToClipboardToolStripMenuItem";
            resources.ApplyResources(this.copyToClipboardToolStripMenuItem, "copyToClipboardToolStripMenuItem");
            this.copyToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyToClipboardToolStripMenuItem_Click);
            // 
            // detailsToolStripMenuItem
            // 
            this.detailsToolStripMenuItem.Image = global::BulkCrapUninstaller.Properties.Resources.magnifybrowse;
            this.detailsToolStripMenuItem.Name = "detailsToolStripMenuItem";
            resources.ApplyResources(this.detailsToolStripMenuItem, "detailsToolStripMenuItem");
            this.detailsToolStripMenuItem.Click += new System.EventHandler(this.detailsToolStripMenuItem_Click);
            // 
            // JunkRemoveWindow
            // 
            this.AcceptButton = this.buttonAccept;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Name = "JunkRemoveWindow";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objectListViewMain)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.listViewContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SaveFileDialog exportDialog;
        private Button buttonCancel;
        private Button buttonAccept;
        private Button buttonExport;
        private ComboBox comboBoxChecker;
        private CheckBox checkBoxHideLowConfidence;
        private Panel panel1;
        private GroupBox groupBox1;
        private ObjectListView objectListViewMain;
        private OLVColumn olvColumnPath;
        private OLVColumn olvColumnSafety;
        private OLVColumn olvColumnUninstallerName;
        private UsageTracker usageTracker1;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label headerConfInfo;
        private Label headerIntro;
        private Label headerConfTitle;
        private ContextMenuStrip listViewContextMenuStrip;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem copyToClipboardToolStripMenuItem;
        private ToolStripMenuItem detailsToolStripMenuItem;
    }
}