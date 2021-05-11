using System.ComponentModel;
using System.Windows.Forms;
using BrightIdeasSoftware;
using BulkCrapUninstaller.Functions.Tracking;

namespace BulkCrapUninstaller.Forms
{
    partial class UninstallProgressWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UninstallProgressWindow));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.olvColumnStatus = new BrightIdeasSoftware.OLVColumn();
            this.olvColumnIsSilent = new BrightIdeasSoftware.OLVColumn();
            this.olvColumnName = new BrightIdeasSoftware.OLVColumn();
            this.objectListView1 = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnId = new BrightIdeasSoftware.OLVColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.skipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.terminateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.runNowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manualUninstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.openInstallDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonRun = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSkip = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTerminate = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonManualUninstall = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonFolderOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonProperties = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonHelp = new System.Windows.Forms.ToolStripButton();
            this.label2 = new System.Windows.Forms.Label();
            this.usageTracker1 = new BulkCrapUninstaller.Functions.Tracking.UsageTracker();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.forceUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxFinishSleep = new System.Windows.Forms.CheckBox();
            this.sleepTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // label1
            // 
            this.label1.AutoEllipsis = true;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // buttonClose
            // 
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // olvColumnStatus
            // 
            resources.ApplyResources(this.olvColumnStatus, "olvColumnStatus");
            // 
            // olvColumnIsSilent
            // 
            resources.ApplyResources(this.olvColumnIsSilent, "olvColumnIsSilent");
            // 
            // olvColumnName
            // 
            this.olvColumnName.FillsFreeSpace = true;
            resources.ApplyResources(this.olvColumnName, "olvColumnName");
            // 
            // objectListView1
            // 
            this.objectListView1.AllColumns.Add(this.olvColumnId);
            this.objectListView1.AllColumns.Add(this.olvColumnStatus);
            this.objectListView1.AllColumns.Add(this.olvColumnIsSilent);
            this.objectListView1.AllColumns.Add(this.olvColumnName);
            this.objectListView1.CellEditUseWholeCell = false;
            this.objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnId,
            this.olvColumnStatus,
            this.olvColumnIsSilent,
            this.olvColumnName});
            this.objectListView1.ContextMenuStrip = this.contextMenuStrip1;
            this.objectListView1.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.objectListView1, "objectListView1");
            this.objectListView1.FullRowSelect = true;
            this.objectListView1.HideSelection = false;
            this.objectListView1.MultiSelect = false;
            this.objectListView1.Name = "objectListView1";
            this.objectListView1.ShowItemToolTips = true;
            this.objectListView1.UseCompatibleStateImageBehavior = false;
            this.objectListView1.View = System.Windows.Forms.View.Details;
            this.objectListView1.SelectedIndexChanged += new System.EventHandler(this.objectListView1_SelectedIndexChanged);
            // 
            // olvColumnId
            // 
            resources.ApplyResources(this.olvColumnId, "olvColumnId");
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.skipToolStripMenuItem,
            this.terminateToolStripMenuItem,
            this.toolStripSeparator3,
            this.runNowToolStripMenuItem,
            this.manualUninstallToolStripMenuItem,
            this.toolStripSeparator4,
            this.openInstallDirectoryToolStripMenuItem,
            this.propertiesToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // skipToolStripMenuItem
            // 
            this.skipToolStripMenuItem.Name = "skipToolStripMenuItem";
            resources.ApplyResources(this.skipToolStripMenuItem, "skipToolStripMenuItem");
            this.skipToolStripMenuItem.Click += new System.EventHandler(this.toolStripButtonSkip_Click);
            // 
            // terminateToolStripMenuItem
            // 
            this.terminateToolStripMenuItem.Name = "terminateToolStripMenuItem";
            resources.ApplyResources(this.terminateToolStripMenuItem, "terminateToolStripMenuItem");
            this.terminateToolStripMenuItem.Click += new System.EventHandler(this.toolStripButtonTerminate_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // runNowToolStripMenuItem
            // 
            this.runNowToolStripMenuItem.Name = "runNowToolStripMenuItem";
            resources.ApplyResources(this.runNowToolStripMenuItem, "runNowToolStripMenuItem");
            this.runNowToolStripMenuItem.Click += new System.EventHandler(this.toolStripButtonRun_Click);
            // 
            // manualUninstallToolStripMenuItem
            // 
            this.manualUninstallToolStripMenuItem.Name = "manualUninstallToolStripMenuItem";
            resources.ApplyResources(this.manualUninstallToolStripMenuItem, "manualUninstallToolStripMenuItem");
            this.manualUninstallToolStripMenuItem.Click += new System.EventHandler(this.toolStripButtonManualUninstall_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // openInstallDirectoryToolStripMenuItem
            // 
            this.openInstallDirectoryToolStripMenuItem.Name = "openInstallDirectoryToolStripMenuItem";
            resources.ApplyResources(this.openInstallDirectoryToolStripMenuItem, "openInstallDirectoryToolStripMenuItem");
            this.openInstallDirectoryToolStripMenuItem.Click += new System.EventHandler(this.toolStripButtonFolderOpen_Click);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            resources.ApplyResources(this.propertiesToolStripMenuItem, "propertiesToolStripMenuItem");
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.toolStripButtonProperties_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.objectListView1);
            this.groupBox1.Controls.Add(this.toolStrip1);
            this.groupBox1.Controls.Add(this.label2);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonRun,
            this.toolStripButtonSkip,
            this.toolStripButtonTerminate,
            this.toolStripButtonManualUninstall,
            this.toolStripSeparator2,
            this.toolStripButtonFolderOpen,
            this.toolStripButtonProperties,
            this.toolStripSeparator1,
            this.toolStripButtonSettings,
            this.toolStripButtonHelp});
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Name = "toolStrip1";
            // 
            // toolStripButtonRun
            // 
            resources.ApplyResources(this.toolStripButtonRun, "toolStripButtonRun");
            this.toolStripButtonRun.Image = global::BulkCrapUninstaller.Properties.Resources.layerdown;
            this.toolStripButtonRun.Name = "toolStripButtonRun";
            this.toolStripButtonRun.Click += new System.EventHandler(this.toolStripButtonRun_Click);
            // 
            // toolStripButtonSkip
            // 
            resources.ApplyResources(this.toolStripButtonSkip, "toolStripButtonSkip");
            this.toolStripButtonSkip.Image = global::BulkCrapUninstaller.Properties.Resources.control_fastforward_variant;
            this.toolStripButtonSkip.Name = "toolStripButtonSkip";
            this.toolStripButtonSkip.Click += new System.EventHandler(this.toolStripButtonSkip_Click);
            // 
            // toolStripButtonTerminate
            // 
            resources.ApplyResources(this.toolStripButtonTerminate, "toolStripButtonTerminate");
            this.toolStripButtonTerminate.Image = global::BulkCrapUninstaller.Properties.Resources.stop;
            this.toolStripButtonTerminate.Name = "toolStripButtonTerminate";
            this.toolStripButtonTerminate.Click += new System.EventHandler(this.toolStripButtonTerminate_Click);
            // 
            // toolStripButtonManualUninstall
            // 
            this.toolStripButtonManualUninstall.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonManualUninstall.Image = global::BulkCrapUninstaller.Properties.Resources.list;
            resources.ApplyResources(this.toolStripButtonManualUninstall, "toolStripButtonManualUninstall");
            this.toolStripButtonManualUninstall.Name = "toolStripButtonManualUninstall";
            this.toolStripButtonManualUninstall.Click += new System.EventHandler(this.toolStripButtonManualUninstall_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // toolStripButtonFolderOpen
            // 
            this.toolStripButtonFolderOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonFolderOpen, "toolStripButtonFolderOpen");
            this.toolStripButtonFolderOpen.Image = global::BulkCrapUninstaller.Properties.Resources.fullscreen;
            this.toolStripButtonFolderOpen.Name = "toolStripButtonFolderOpen";
            this.toolStripButtonFolderOpen.Click += new System.EventHandler(this.toolStripButtonFolderOpen_Click);
            // 
            // toolStripButtonProperties
            // 
            this.toolStripButtonProperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonProperties, "toolStripButtonProperties");
            this.toolStripButtonProperties.Image = global::BulkCrapUninstaller.Properties.Resources.properties;
            this.toolStripButtonProperties.Name = "toolStripButtonProperties";
            this.toolStripButtonProperties.Click += new System.EventHandler(this.toolStripButtonProperties_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripButtonSettings
            // 
            this.toolStripButtonSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSettings.Image = global::BulkCrapUninstaller.Properties.Resources.settings;
            resources.ApplyResources(this.toolStripButtonSettings, "toolStripButtonSettings");
            this.toolStripButtonSettings.Name = "toolStripButtonSettings";
            this.toolStripButtonSettings.Click += new System.EventHandler(this.toolStripButtonSettings_Click);
            // 
            // toolStripButtonHelp
            // 
            this.toolStripButtonHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonHelp.Image = global::BulkCrapUninstaller.Properties.Resources.information_circle;
            resources.ApplyResources(this.toolStripButtonHelp, "toolStripButtonHelp");
            this.toolStripButtonHelp.Name = "toolStripButtonHelp";
            this.toolStripButtonHelp.Click += new System.EventHandler(this.toolStripButtonHelp_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // usageTracker1
            // 
            this.usageTracker1.ContainerControl = this;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.panel1);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.progressBar1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::BulkCrapUninstaller.Properties.Resources.layerdelete;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // forceUpdateTimer
            // 
            this.forceUpdateTimer.Interval = 3500;
            // 
            // flowLayoutPanel2
            // 
            resources.ApplyResources(this.flowLayoutPanel2, "flowLayoutPanel2");
            this.flowLayoutPanel2.Controls.Add(this.buttonClose);
            this.flowLayoutPanel2.Controls.Add(this.checkBoxFinishSleep);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            // 
            // checkBoxFinishSleep
            // 
            resources.ApplyResources(this.checkBoxFinishSleep, "checkBoxFinishSleep");
            this.checkBoxFinishSleep.Name = "checkBoxFinishSleep";
            this.checkBoxFinishSleep.UseVisualStyleBackColor = true;
            // 
            // UninstallProgressWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "UninstallProgressWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UninstallProgressWindow_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ProgressBar progressBar1;
        private Label label1;
        private Button buttonClose;
        private OLVColumn olvColumnStatus;
        private OLVColumn olvColumnIsSilent;
        private OLVColumn olvColumnName;
        private ObjectListView objectListView1;
        private GroupBox groupBox1;
        private Label label2;
        private Label label3;
        private UsageTracker usageTracker1;
        private Panel panel3;
        private PictureBox pictureBox1;
        private FlowLayoutPanel flowLayoutPanel1;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButtonRun;
        private ToolStripButton toolStripButtonFolderOpen;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripButtonSettings;
        private ToolStripButton toolStripButtonSkip;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton toolStripButtonProperties;
        private OLVColumn olvColumnId;
        private ToolStripButton toolStripButtonTerminate;
        private ToolStripButton toolStripButtonHelp;
        private Timer forceUpdateTimer;
        private ToolStripButton toolStripButtonManualUninstall;
        private Panel panel1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem skipToolStripMenuItem;
        private ToolStripMenuItem terminateToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem runNowToolStripMenuItem;
        private ToolStripMenuItem manualUninstallToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem openInstallDirectoryToolStripMenuItem;
        private ToolStripMenuItem propertiesToolStripMenuItem;
        private FlowLayoutPanel flowLayoutPanel2;
        private CheckBox checkBoxFinishSleep;
        private Timer sleepTimer;
    }
}