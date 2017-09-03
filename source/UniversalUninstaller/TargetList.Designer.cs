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
            this.treeListView1 = new BrightIdeasSoftware.TreeListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeListView1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonSelAll});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(431, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonSelAll
            // 
            this.toolStripButtonSelAll.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSelAll.Image")));
            this.toolStripButtonSelAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSelAll.Name = "toolStripButtonSelAll";
            this.toolStripButtonSelAll.Size = new System.Drawing.Size(73, 22);
            this.toolStripButtonSelAll.Text = "Select all";
            this.toolStripButtonSelAll.Click += new System.EventHandler(this.toolStripButtonSelAll_Click);
            // 
            // treeListView1
            // 
            this.treeListView1.AllColumns.Add(this.olvColumn1);
            this.treeListView1.CellEditUseWholeCell = false;
            this.treeListView1.CheckBoxes = true;
            this.treeListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1});
            this.treeListView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeListView1.FullRowSelect = true;
            this.treeListView1.GridLines = true;
            this.treeListView1.Location = new System.Drawing.Point(0, 25);
            this.treeListView1.Name = "treeListView1";
            this.treeListView1.ShowGroups = false;
            this.treeListView1.ShowImagesOnSubItems = true;
            this.treeListView1.Size = new System.Drawing.Size(431, 291);
            this.treeListView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.treeListView1.TabIndex = 1;
            this.treeListView1.UseCompatibleStateImageBehavior = false;
            this.treeListView1.View = System.Windows.Forms.View.Details;
            this.treeListView1.VirtualMode = true;
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "";
            this.olvColumn1.FillsFreeSpace = true;
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
        private BrightIdeasSoftware.OLVColumn olvColumn1;
    }
}
