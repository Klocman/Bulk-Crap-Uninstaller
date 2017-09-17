namespace UniversalUninstaller
{
    partial class TargetList
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TargetList));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonSelAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonExpand = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCollapse = new System.Windows.Forms.ToolStripButton();
            this.treeListView1 = new BrightIdeasSoftware.TreeListView();
            this.olvColumnName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnSize = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeListView1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(22, 22);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonSelAll,
            this.toolStripSeparator1,
            this.toolStripButtonExpand,
            this.toolStripButtonCollapse});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(431, 29);
            this.toolStrip1.TabIndex = 0;
            // 
            // toolStripButtonSelAll
            // 
            this.toolStripButtonSelAll.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSelAll.Image")));
            this.toolStripButtonSelAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSelAll.Name = "toolStripButtonSelAll";
            this.toolStripButtonSelAll.Size = new System.Drawing.Size(79, 26);
            this.toolStripButtonSelAll.Text = "Select all";
            this.toolStripButtonSelAll.Click += new System.EventHandler(this.toolStripButtonSelAll_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 29);
            // 
            // toolStripButtonExpand
            // 
            this.toolStripButtonExpand.Image = global::UniversalUninstaller.Properties.Resources.section_expand_all;
            this.toolStripButtonExpand.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExpand.Name = "toolStripButtonExpand";
            this.toolStripButtonExpand.Size = new System.Drawing.Size(86, 26);
            this.toolStripButtonExpand.Text = "Expand all";
            this.toolStripButtonExpand.Click += new System.EventHandler(this.expand_Click);
            // 
            // toolStripButtonCollapse
            // 
            this.toolStripButtonCollapse.Image = global::UniversalUninstaller.Properties.Resources.section_collapse_all;
            this.toolStripButtonCollapse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCollapse.Name = "toolStripButtonCollapse";
            this.toolStripButtonCollapse.Size = new System.Drawing.Size(93, 26);
            this.toolStripButtonCollapse.Text = "Collapse all";
            this.toolStripButtonCollapse.Click += new System.EventHandler(this.collapse_Click);
            // 
            // treeListView1
            // 
            this.treeListView1.AllColumns.Add(this.olvColumnName);
            this.treeListView1.AllColumns.Add(this.olvColumnSize);
            this.treeListView1.CellEditUseWholeCell = false;
            this.treeListView1.CheckBoxes = true;
            this.treeListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnName,
            this.olvColumnSize});
            this.treeListView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeListView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeListView1.FullRowSelect = true;
            this.treeListView1.GridLines = true;
            this.treeListView1.Location = new System.Drawing.Point(0, 29);
            this.treeListView1.Name = "treeListView1";
            this.treeListView1.ShowGroups = false;
            this.treeListView1.ShowImagesOnSubItems = true;
            this.treeListView1.Size = new System.Drawing.Size(431, 287);
            this.treeListView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.treeListView1.TabIndex = 1;
            this.treeListView1.UseCompatibleStateImageBehavior = false;
            this.treeListView1.View = System.Windows.Forms.View.Details;
            this.treeListView1.VirtualMode = true;
            this.treeListView1.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.treeListView1_ItemChecked);
            this.treeListView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.treeListView1_MouseDoubleClick);
            // 
            // olvColumnName
            // 
            this.olvColumnName.AspectName = "";
            this.olvColumnName.FillsFreeSpace = true;
            this.olvColumnName.Text = "Name";
            // 
            // olvColumnSize
            // 
            this.olvColumnSize.Text = "Size";
            this.olvColumnSize.Width = 72;
            // 
            // TargetList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeListView1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "TargetList";
            this.Size = new System.Drawing.Size(431, 316);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeListView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonSelAll;
        private BrightIdeasSoftware.TreeListView treeListView1;
        private BrightIdeasSoftware.OLVColumn olvColumnName;
        private System.Windows.Forms.ToolStripButton toolStripButtonExpand;
        private System.Windows.Forms.ToolStripButton toolStripButtonCollapse;
        private BrightIdeasSoftware.OLVColumn olvColumnSize;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}
