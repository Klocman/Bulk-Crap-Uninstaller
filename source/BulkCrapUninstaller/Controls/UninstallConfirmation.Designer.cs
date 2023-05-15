namespace BulkCrapUninstaller.Forms
{
    partial class UninstallConfirmation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UninstallConfirmation));
            this.objectListView1 = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnEnabled = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnQuiet = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnInstallLocation = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonSmartSort = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.labelInfo = new System.Windows.Forms.Label();
            this.labelInfo2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // objectListView1
            // 
            this.objectListView1.AllColumns.Add(this.olvColumnName);
            this.objectListView1.AllColumns.Add(this.olvColumnEnabled);
            this.objectListView1.AllColumns.Add(this.olvColumnQuiet);
            this.objectListView1.AllColumns.Add(this.olvColumnInstallLocation);
            this.objectListView1.CellEditUseWholeCell = false;
            this.objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnName,
            this.olvColumnEnabled,
            this.olvColumnQuiet,
            this.olvColumnInstallLocation});
            this.objectListView1.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.objectListView1, "objectListView1");
            this.objectListView1.FullRowSelect = true;
            this.objectListView1.GridLines = true;
            this.objectListView1.MultiSelect = true;
            this.objectListView1.Name = "objectListView1";
            this.objectListView1.ShowGroups = false;
            this.objectListView1.ShowItemToolTips = true;
            this.objectListView1.UseCompatibleStateImageBehavior = false;
            this.objectListView1.View = System.Windows.Forms.View.Details;
            // 
            // olvColumnName
            // 
            resources.ApplyResources(this.olvColumnName, "olvColumnName");
            // 
            // olvColumnEnabled
            // 
            this.olvColumnEnabled.CheckBoxes = true;
            this.olvColumnEnabled.Hideable = false;
            resources.ApplyResources(this.olvColumnEnabled, "olvColumnEnabled");
            // 
            // olvColumnQuiet
            // 
            this.olvColumnQuiet.CheckBoxes = true;
            resources.ApplyResources(this.olvColumnQuiet, "olvColumnQuiet");
            // 
            // olvColumnInstallLocation
            // 
            this.olvColumnInstallLocation.FillsFreeSpace = true;
            resources.ApplyResources(this.olvColumnInstallLocation, "olvColumnInstallLocation");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonSmartSort);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // buttonSmartSort
            // 
            resources.ApplyResources(this.buttonSmartSort, "buttonSmartSort");
            this.buttonSmartSort.Name = "buttonSmartSort";
            this.buttonSmartSort.UseVisualStyleBackColor = true;
            this.buttonSmartSort.Click += new System.EventHandler(this.buttonSort_Click);
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.labelInfo);
            this.flowLayoutPanel1.Controls.Add(this.labelInfo2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // labelInfo
            // 
            resources.ApplyResources(this.labelInfo, "labelInfo");
            this.labelInfo.Name = "labelInfo";
            // 
            // labelInfo2
            // 
            resources.ApplyResources(this.labelInfo2, "labelInfo2");
            this.labelInfo2.Name = "labelInfo2";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::BulkCrapUninstaller.Properties.Resources.list_reorder;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // UninstallConfirmation
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.objectListView1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.Name = "UninstallConfirmation";
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BrightIdeasSoftware.ObjectListView objectListView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label labelInfo;
        private BrightIdeasSoftware.OLVColumn olvColumnEnabled;
        private BrightIdeasSoftware.OLVColumn olvColumnQuiet;
        private BrightIdeasSoftware.OLVColumn olvColumnName;
        private BrightIdeasSoftware.OLVColumn olvColumnInstallLocation;
        private System.Windows.Forms.Button buttonSmartSort;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelInfo2;
    }
}