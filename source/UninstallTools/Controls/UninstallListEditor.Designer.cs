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
            this.columnHeaderName = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderType = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderConditions = new System.Windows.Forms.ColumnHeader();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonAddFilter = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRemoveFilter = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAddFiltersFromList = new System.Windows.Forms.ToolStripButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBoxFilterSettings = new System.Windows.Forms.GroupBox();
            this.textBoxFilterName = new System.Windows.Forms.TextBox();
            this.labelFilterType = new System.Windows.Forms.Label();
            this.comboBoxFilterType = new System.Windows.Forms.ComboBox();
            this.labelFilterName = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBoxConditions = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.listBoxConditions = new System.Windows.Forms.ListBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonAddCondition = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRemoveCondition = new System.Windows.Forms.ToolStripButton();
            this.groupBoxConditionEditor = new System.Windows.Forms.GroupBox();
            this.filterEditor = new UninstallTools.Controls.FilterEditor();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBoxFilterList.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBoxFilterSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBoxConditions.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.groupBoxConditionEditor.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBoxFilterList);
            this.splitContainer1.Panel1.Controls.Add(this.panel2);
            this.splitContainer1.Panel1.Controls.Add(this.groupBoxFilterSettings);
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
            this.columnHeaderType,
            this.columnHeaderConditions});
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
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.OnSelectedFilterChanged);
            // 
            // columnHeaderName
            // 
            resources.ApplyResources(this.columnHeaderName, "columnHeaderName");
            // 
            // columnHeaderType
            // 
            resources.ApplyResources(this.columnHeaderType, "columnHeaderType");
            // 
            // columnHeaderConditions
            // 
            resources.ApplyResources(this.columnHeaderConditions, "columnHeaderConditions");
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAddFilter,
            this.toolStripButtonRemoveFilter,
            this.toolStripButtonAddFiltersFromList});
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Name = "toolStrip1";
            // 
            // toolStripButtonAddFilter
            // 
            this.toolStripButtonAddFilter.Image = global::UninstallTools.Properties.Resources.add;
            resources.ApplyResources(this.toolStripButtonAddFilter, "toolStripButtonAddFilter");
            this.toolStripButtonAddFilter.Name = "toolStripButtonAddFilter";
            this.toolStripButtonAddFilter.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // toolStripButtonRemoveFilter
            // 
            this.toolStripButtonRemoveFilter.Image = global::UninstallTools.Properties.Resources.minus;
            resources.ApplyResources(this.toolStripButtonRemoveFilter, "toolStripButtonRemoveFilter");
            this.toolStripButtonRemoveFilter.Name = "toolStripButtonRemoveFilter";
            this.toolStripButtonRemoveFilter.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // toolStripButtonAddFiltersFromList
            // 
            this.toolStripButtonAddFiltersFromList.Image = global::UninstallTools.Properties.Resources.folder_open;
            resources.ApplyResources(this.toolStripButtonAddFiltersFromList, "toolStripButtonAddFiltersFromList");
            this.toolStripButtonAddFiltersFromList.Name = "toolStripButtonAddFiltersFromList";
            this.toolStripButtonAddFiltersFromList.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // groupBoxFilterSettings
            // 
            this.groupBoxFilterSettings.Controls.Add(this.textBoxFilterName);
            this.groupBoxFilterSettings.Controls.Add(this.labelFilterType);
            this.groupBoxFilterSettings.Controls.Add(this.comboBoxFilterType);
            this.groupBoxFilterSettings.Controls.Add(this.labelFilterName);
            resources.ApplyResources(this.groupBoxFilterSettings, "groupBoxFilterSettings");
            this.groupBoxFilterSettings.Name = "groupBoxFilterSettings";
            this.groupBoxFilterSettings.TabStop = false;
            // 
            // textBoxFilterName
            // 
            resources.ApplyResources(this.textBoxFilterName, "textBoxFilterName");
            this.textBoxFilterName.Name = "textBoxFilterName";
            this.textBoxFilterName.TextChanged += new System.EventHandler(this.textBoxFilterName_TextChanged);
            // 
            // labelFilterType
            // 
            resources.ApplyResources(this.labelFilterType, "labelFilterType");
            this.labelFilterType.Name = "labelFilterType";
            // 
            // comboBoxFilterType
            // 
            resources.ApplyResources(this.comboBoxFilterType, "comboBoxFilterType");
            this.comboBoxFilterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFilterType.FormattingEnabled = true;
            this.comboBoxFilterType.Items.AddRange(new object[] {
            resources.GetString("comboBoxFilterType.Items"),
            resources.GetString("comboBoxFilterType.Items1")});
            this.comboBoxFilterType.Name = "comboBoxFilterType";
            this.comboBoxFilterType.SelectedIndexChanged += new System.EventHandler(this.comboBoxFilterType_SelectedIndexChanged);
            // 
            // labelFilterName
            // 
            resources.ApplyResources(this.labelFilterName, "labelFilterName");
            this.labelFilterName.Name = "labelFilterName";
            // 
            // splitContainer2
            // 
            resources.ApplyResources(this.splitContainer2, "splitContainer2");
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBoxConditions);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBoxConditionEditor);
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
            this.panel1.Controls.Add(this.listBoxConditions);
            this.panel1.Controls.Add(this.toolStrip2);
            this.panel1.Name = "panel1";
            // 
            // listBoxConditions
            // 
            resources.ApplyResources(this.listBoxConditions, "listBoxConditions");
            this.listBoxConditions.FormattingEnabled = true;
            this.listBoxConditions.Name = "listBoxConditions";
            this.listBoxConditions.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // toolStrip2
            // 
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAddCondition,
            this.toolStripButtonRemoveCondition});
            resources.ApplyResources(this.toolStrip2, "toolStrip2");
            this.toolStrip2.Name = "toolStrip2";
            // 
            // toolStripButtonAddCondition
            // 
            this.toolStripButtonAddCondition.Image = global::UninstallTools.Properties.Resources.add;
            resources.ApplyResources(this.toolStripButtonAddCondition, "toolStripButtonAddCondition");
            this.toolStripButtonAddCondition.Name = "toolStripButtonAddCondition";
            this.toolStripButtonAddCondition.Click += new System.EventHandler(this.toolStripButtonAddCondition_Click);
            // 
            // toolStripButtonRemoveCondition
            // 
            this.toolStripButtonRemoveCondition.Image = global::UninstallTools.Properties.Resources.minus;
            resources.ApplyResources(this.toolStripButtonRemoveCondition, "toolStripButtonRemoveCondition");
            this.toolStripButtonRemoveCondition.Name = "toolStripButtonRemoveCondition";
            this.toolStripButtonRemoveCondition.Click += new System.EventHandler(this.toolStripButtonRemoveCondition_Click);
            // 
            // groupBoxConditionEditor
            // 
            this.groupBoxConditionEditor.Controls.Add(this.filterEditor);
            resources.ApplyResources(this.groupBoxConditionEditor, "groupBoxConditionEditor");
            this.groupBoxConditionEditor.Name = "groupBoxConditionEditor";
            this.groupBoxConditionEditor.TabStop = false;
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
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBoxFilterList.ResumeLayout(false);
            this.groupBoxFilterList.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBoxFilterSettings.ResumeLayout(false);
            this.groupBoxFilterSettings.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBoxConditions.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.groupBoxConditionEditor.ResumeLayout(false);
            this.groupBoxConditionEditor.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxFilterList;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderType;
        private System.Windows.Forms.GroupBox groupBoxConditions;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ColumnHeader columnHeaderConditions;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddFilter;
        private System.Windows.Forms.ToolStripButton toolStripButtonRemoveFilter;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddFiltersFromList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddCondition;
        private System.Windows.Forms.ListBox listBoxConditions;
        private System.Windows.Forms.ToolStripButton toolStripButtonRemoveCondition;
        private System.Windows.Forms.GroupBox groupBoxConditionEditor;
        private FilterEditor filterEditor;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBoxFilterSettings;
        private System.Windows.Forms.TextBox textBoxFilterName;
        private System.Windows.Forms.Label labelFilterType;
        private System.Windows.Forms.ComboBox comboBoxFilterType;
        private System.Windows.Forms.Label labelFilterName;
    }
}
