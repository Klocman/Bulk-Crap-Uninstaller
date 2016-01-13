namespace UninstallTools.Controls
{
    partial class UninstallListEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UninstallListEditor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBoxFilterList = new System.Windows.Forms.GroupBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderMethod = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderProperty = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonAddFilter = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRemoveFilter = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAddFiltersFromList = new System.Windows.Forms.ToolStripButton();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBoxConditions = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonAddCondition = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRemoveCondition = new System.Windows.Forms.ToolStripButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.filterEditor = new UninstallTools.Controls.FilterEditor();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBoxFilterList.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBoxConditions.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBoxFilterList);
            resources.ApplyResources(this.splitContainer1.Panel1, "splitContainer1.Panel1");
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            // 
            // groupBoxFilterList
            // 
            this.groupBoxFilterList.Controls.Add(this.listView1);
            this.groupBoxFilterList.Controls.Add(this.toolStrip1);
            resources.ApplyResources(this.groupBoxFilterList, "groupBoxFilterList");
            this.groupBoxFilterList.Name = "groupBoxFilterList";
            this.groupBoxFilterList.TabStop = false;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderMethod,
            this.columnHeaderProperty});
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.HideSelection = false;
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.ShowGroups = false;
            this.listView1.ShowItemToolTips = true;
            this.listView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // columnHeaderName
            // 
            resources.ApplyResources(this.columnHeaderName, "columnHeaderName");
            // 
            // columnHeaderMethod
            // 
            resources.ApplyResources(this.columnHeaderMethod, "columnHeaderMethod");
            // 
            // columnHeaderProperty
            // 
            resources.ApplyResources(this.columnHeaderProperty, "columnHeaderProperty");
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAddFilter,
            this.toolStripButtonRemoveFilter,
            this.toolStripButtonAddFiltersFromList});
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Name = "toolStrip1";
            // 
            // toolStripButtonAddFilter
            // 
            resources.ApplyResources(this.toolStripButtonAddFilter, "toolStripButtonAddFilter");
            this.toolStripButtonAddFilter.Name = "toolStripButtonAddFilter";
            this.toolStripButtonAddFilter.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // toolStripButtonRemoveFilter
            // 
            resources.ApplyResources(this.toolStripButtonRemoveFilter, "toolStripButtonRemoveFilter");
            this.toolStripButtonRemoveFilter.Name = "toolStripButtonRemoveFilter";
            this.toolStripButtonRemoveFilter.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // toolStripButtonAddFiltersFromList
            // 
            resources.ApplyResources(this.toolStripButtonAddFiltersFromList, "toolStripButtonAddFiltersFromList");
            this.toolStripButtonAddFiltersFromList.Name = "toolStripButtonAddFiltersFromList";
            this.toolStripButtonAddFiltersFromList.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // splitContainer2
            // 
            resources.ApplyResources(this.splitContainer2, "splitContainer2");
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBoxConditions);
            resources.ApplyResources(this.splitContainer2.Panel1, "splitContainer2.Panel1");
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox1);
            resources.ApplyResources(this.splitContainer2.Panel2, "splitContainer2.Panel2");
            // 
            // groupBoxConditions
            // 
            this.groupBoxConditions.Controls.Add(this.panel1);
            resources.ApplyResources(this.groupBoxConditions, "groupBoxConditions");
            this.groupBoxConditions.Name = "groupBoxConditions";
            this.groupBoxConditions.TabStop = false;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.listBox1);
            this.panel1.Controls.Add(this.toolStrip2);
            this.panel1.Name = "panel1";
            // 
            // listBox1
            // 
            resources.ApplyResources(this.listBox1, "listBox1");
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // toolStrip2
            // 
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAddCondition,
            this.toolStripButtonRemoveCondition});
            resources.ApplyResources(this.toolStrip2, "toolStrip2");
            this.toolStrip2.Name = "toolStrip2";
            // 
            // toolStripButtonAddCondition
            // 
            resources.ApplyResources(this.toolStripButtonAddCondition, "toolStripButtonAddCondition");
            this.toolStripButtonAddCondition.Name = "toolStripButtonAddCondition";
            this.toolStripButtonAddCondition.Click += new System.EventHandler(this.toolStripButtonAddCondition_Click);
            // 
            // toolStripButtonRemoveCondition
            // 
            resources.ApplyResources(this.toolStripButtonRemoveCondition, "toolStripButtonRemoveCondition");
            this.toolStripButtonRemoveCondition.Name = "toolStripButtonRemoveCondition";
            this.toolStripButtonRemoveCondition.Click += new System.EventHandler(this.toolStripButtonRemoveCondition_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.filterEditor);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // filterEditor
            // 
            resources.ApplyResources(this.filterEditor, "filterEditor");
            this.filterEditor.Name = "filterEditor";
            // 
            // openFileDialog
            // 
            resources.ApplyResources(this.openFileDialog, "openFileDialog");
            this.openFileDialog.Multiselect = true;
            this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog_FileOk);
            // 
            // UninstallListEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "UninstallListEditor";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.groupBoxFilterList.ResumeLayout(false);
            this.groupBoxFilterList.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.groupBoxConditions.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxFilterList;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderMethod;
        private System.Windows.Forms.GroupBox groupBoxConditions;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ColumnHeader columnHeaderProperty;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddFilter;
        private System.Windows.Forms.ToolStripButton toolStripButtonRemoveFilter;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddFiltersFromList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddCondition;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ToolStripButton toolStripButtonRemoveCondition;
        private System.Windows.Forms.GroupBox groupBox1;
        private FilterEditor filterEditor;
    }
}
