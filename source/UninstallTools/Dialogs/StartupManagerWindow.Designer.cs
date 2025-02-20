using BrightIdeasSoftware;

namespace UninstallTools.Dialogs
{
    partial class StartupManagerWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartupManagerWindow));
            panel2 = new System.Windows.Forms.Panel();
            panel5 = new System.Windows.Forms.Panel();
            comboBoxFilter = new System.Windows.Forms.ComboBox();
            panel4 = new System.Windows.Forms.Panel();
            panel3 = new System.Windows.Forms.Panel();
            buttonRefresh = new System.Windows.Forms.Button();
            panel1 = new System.Windows.Forms.Panel();
            buttonCancel = new System.Windows.Forms.Button();
            buttonExport = new System.Windows.Forms.Button();
            groupBox1 = new System.Windows.Forms.GroupBox();
            listView1 = new ObjectListView();
            columnHeader1 = new OLVColumn();
            columnHeader5 = new OLVColumn();
            columnHeader2 = new OLVColumn();
            columnHeader3 = new OLVColumn();
            columnHeader4 = new OLVColumn();
            contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(components);
            openFileLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openLinkLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            runCommandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            copyToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            createBackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            moveToRegistryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            runForAllUsersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            enableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exportDialog = new System.Windows.Forms.SaveFileDialog();
            folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            panel2.SuspendLayout();
            panel5.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)listView1).BeginInit();
            contextMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // panel2
            // 
            panel2.Controls.Add(panel5);
            panel2.Controls.Add(panel4);
            panel2.Controls.Add(panel3);
            panel2.Controls.Add(buttonRefresh);
            panel2.Controls.Add(panel1);
            panel2.Controls.Add(buttonCancel);
            panel2.Controls.Add(buttonExport);
            resources.ApplyResources(panel2, "panel2");
            panel2.Name = "panel2";
            // 
            // panel5
            // 
            panel5.Controls.Add(comboBoxFilter);
            resources.ApplyResources(panel5, "panel5");
            panel5.Name = "panel5";
            // 
            // comboBoxFilter
            // 
            resources.ApplyResources(comboBoxFilter, "comboBoxFilter");
            comboBoxFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxFilter.FormattingEnabled = true;
            comboBoxFilter.Items.AddRange(new object[] { resources.GetString("comboBoxFilter.Items"), resources.GetString("comboBoxFilter.Items1"), resources.GetString("comboBoxFilter.Items2"), resources.GetString("comboBoxFilter.Items3"), resources.GetString("comboBoxFilter.Items4") });
            comboBoxFilter.Name = "comboBoxFilter";
            comboBoxFilter.SelectedIndexChanged += comboBoxFilter_SelectedIndexChanged;
            // 
            // panel4
            // 
            resources.ApplyResources(panel4, "panel4");
            panel4.Name = "panel4";
            // 
            // panel3
            // 
            resources.ApplyResources(panel3, "panel3");
            panel3.Name = "panel3";
            // 
            // buttonRefresh
            // 
            resources.ApplyResources(buttonRefresh, "buttonRefresh");
            buttonRefresh.Name = "buttonRefresh";
            buttonRefresh.UseVisualStyleBackColor = true;
            buttonRefresh.Click += buttonRefresh_Click;
            // 
            // panel1
            // 
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // buttonCancel
            // 
            resources.ApplyResources(buttonCancel, "buttonCancel");
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonCancel.Name = "buttonCancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // buttonExport
            // 
            resources.ApplyResources(buttonExport, "buttonExport");
            buttonExport.Name = "buttonExport";
            buttonExport.UseVisualStyleBackColor = true;
            buttonExport.Click += buttonExport_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(listView1);
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // listView1
            // 
            listView1.AutoArrange = false;
            listView1.CellEditUseWholeCell = false;
            listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1, columnHeader5, columnHeader2, columnHeader3, columnHeader4 });
            listView1.ContextMenuStrip = contextMenuStrip;
            resources.ApplyResources(listView1, "listView1");
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Name = "listView1";
            listView1.ShowGroups = false;
            listView1.ShowItemToolTips = true;
            listView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = System.Windows.Forms.View.Details;
            listView1.SelectedIndexChanged += SelectionChanged;
            listView1.MouseClick += listView1_MouseClick;
            listView1.MouseDoubleClick += listView1_MouseDoubleClick;
            // 
            // columnHeader1
            // 
            columnHeader1.AspectName = "ProgramName";
            resources.ApplyResources(columnHeader1, "columnHeader1");
            // 
            // columnHeader5
            // 
            columnHeader5.AspectName = "Disabled";
            resources.ApplyResources(columnHeader5, "columnHeader5");
            // 
            // columnHeader2
            // 
            columnHeader2.AspectName = "Company";
            resources.ApplyResources(columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            columnHeader3.AspectName = "ParentShortName";
            resources.ApplyResources(columnHeader3, "columnHeader3");
            // 
            // columnHeader4
            // 
            columnHeader4.AspectName = "Command";
            resources.ApplyResources(columnHeader4, "columnHeader4");
            // 
            // contextMenuStrip
            // 
            contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { openFileLocationToolStripMenuItem, openLinkLocationToolStripMenuItem, runCommandToolStripMenuItem, toolStripSeparator2, copyToClipboardToolStripMenuItem, createBackupToolStripMenuItem, toolStripSeparator1, moveToRegistryToolStripMenuItem, runForAllUsersToolStripMenuItem, toolStripSeparator3, enableToolStripMenuItem, deleteToolStripMenuItem });
            contextMenuStrip.Name = "contextMenuStrip1";
            resources.ApplyResources(contextMenuStrip, "contextMenuStrip");
            contextMenuStrip.Opening += contextMenuStrip1_Opening;
            // 
            // openFileLocationToolStripMenuItem
            // 
            resources.ApplyResources(openFileLocationToolStripMenuItem, "openFileLocationToolStripMenuItem");
            openFileLocationToolStripMenuItem.Image = Properties.Resources.folder_open;
            openFileLocationToolStripMenuItem.Name = "openFileLocationToolStripMenuItem";
            openFileLocationToolStripMenuItem.Click += openFileLocationToolStripMenuItem_Click;
            // 
            // openLinkLocationToolStripMenuItem
            // 
            openLinkLocationToolStripMenuItem.Image = Properties.Resources.link;
            openLinkLocationToolStripMenuItem.Name = "openLinkLocationToolStripMenuItem";
            resources.ApplyResources(openLinkLocationToolStripMenuItem, "openLinkLocationToolStripMenuItem");
            openLinkLocationToolStripMenuItem.Click += openLinkLocationToolStripMenuItem_Click;
            // 
            // runCommandToolStripMenuItem
            // 
            runCommandToolStripMenuItem.Image = Properties.Resources.app;
            runCommandToolStripMenuItem.Name = "runCommandToolStripMenuItem";
            resources.ApplyResources(runCommandToolStripMenuItem, "runCommandToolStripMenuItem");
            runCommandToolStripMenuItem.Click += runCommandToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(toolStripSeparator2, "toolStripSeparator2");
            // 
            // copyToClipboardToolStripMenuItem
            // 
            copyToClipboardToolStripMenuItem.Image = Properties.Resources.page_copy;
            copyToClipboardToolStripMenuItem.Name = "copyToClipboardToolStripMenuItem";
            resources.ApplyResources(copyToClipboardToolStripMenuItem, "copyToClipboardToolStripMenuItem");
            copyToClipboardToolStripMenuItem.Click += copyToClipboardToolStripMenuItem_Click;
            // 
            // createBackupToolStripMenuItem
            // 
            createBackupToolStripMenuItem.Image = Properties.Resources.page_duplicate;
            createBackupToolStripMenuItem.Name = "createBackupToolStripMenuItem";
            resources.ApplyResources(createBackupToolStripMenuItem, "createBackupToolStripMenuItem");
            createBackupToolStripMenuItem.Click += createBackupToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(toolStripSeparator1, "toolStripSeparator1");
            // 
            // moveToRegistryToolStripMenuItem
            // 
            moveToRegistryToolStripMenuItem.Image = Properties.Resources.arrow_right;
            moveToRegistryToolStripMenuItem.Name = "moveToRegistryToolStripMenuItem";
            resources.ApplyResources(moveToRegistryToolStripMenuItem, "moveToRegistryToolStripMenuItem");
            moveToRegistryToolStripMenuItem.Click += moveToRegistryToolStripMenuItem_Click;
            // 
            // runForAllUsersToolStripMenuItem
            // 
            runForAllUsersToolStripMenuItem.Checked = true;
            runForAllUsersToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            runForAllUsersToolStripMenuItem.Name = "runForAllUsersToolStripMenuItem";
            resources.ApplyResources(runForAllUsersToolStripMenuItem, "runForAllUsersToolStripMenuItem");
            runForAllUsersToolStripMenuItem.Click += runForAllUsersToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(toolStripSeparator3, "toolStripSeparator3");
            // 
            // enableToolStripMenuItem
            // 
            enableToolStripMenuItem.Checked = true;
            enableToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            enableToolStripMenuItem.Name = "enableToolStripMenuItem";
            resources.ApplyResources(enableToolStripMenuItem, "enableToolStripMenuItem");
            enableToolStripMenuItem.Click += enableToolStripMenuItem_Click;
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Image = Properties.Resources.delete;
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            resources.ApplyResources(deleteToolStripMenuItem, "deleteToolStripMenuItem");
            deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            // 
            // exportDialog
            // 
            exportDialog.DefaultExt = "txt";
            resources.ApplyResources(exportDialog, "exportDialog");
            exportDialog.FileOk += saveFileDialog1_FileOk;
            // 
            // folderBrowserDialog
            // 
            resources.ApplyResources(folderBrowserDialog, "folderBrowserDialog");
            // 
            // StartupManagerWindow
            // 
            AcceptButton = buttonCancel;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonCancel;
            Controls.Add(groupBox1);
            Controls.Add(panel2);
            Name = "StartupManagerWindow";
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            Shown += StartupManagerWindow_Shown;
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel5.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)listView1).EndInit();
            contextMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private ObjectListView listView1;
        private OLVColumn columnHeader1;
        private OLVColumn columnHeader2;
        private OLVColumn columnHeader3;
        private OLVColumn columnHeader4;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem copyToClipboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem openLinkLocationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFileLocationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runCommandToolStripMenuItem;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.SaveFileDialog exportDialog;
        private OLVColumn columnHeader5;
        private System.Windows.Forms.ToolStripMenuItem enableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createBackupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveToRegistryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runForAllUsersToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ComboBox comboBoxFilter;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel5;
    }
}