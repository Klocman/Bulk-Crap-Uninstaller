﻿namespace BulkCrapUninstaller.Forms
{
    partial class RelatedUninstallerAdder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RelatedUninstallerAdder));
            this.uninstallerObjectListView = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnEnabled = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnRelatedApps = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.labelInfo = new System.Windows.Forms.Label();
            this.labelInfo2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.uninstallerObjectListView)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // uninstallerObjectListView
            // 
            this.uninstallerObjectListView.AllColumns.Add(this.olvColumnName);
            this.uninstallerObjectListView.AllColumns.Add(this.olvColumnEnabled);
            this.uninstallerObjectListView.AllColumns.Add(this.olvColumnRelatedApps);
            this.uninstallerObjectListView.CellEditUseWholeCell = false;
            this.uninstallerObjectListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnName,
            this.olvColumnEnabled,
            this.olvColumnRelatedApps});
            this.uninstallerObjectListView.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.uninstallerObjectListView, "uninstallerObjectListView");
            this.uninstallerObjectListView.FullRowSelect = true;
            this.uninstallerObjectListView.GridLines = true;
            this.uninstallerObjectListView.MultiSelect = false;
            this.uninstallerObjectListView.Name = "uninstallerObjectListView";
            this.uninstallerObjectListView.ShowGroups = false;
            this.uninstallerObjectListView.ShowItemToolTips = true;
            this.uninstallerObjectListView.UseCompatibleStateImageBehavior = false;
            this.uninstallerObjectListView.View = System.Windows.Forms.View.Details;
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
            // olvColumnRelatedApps
            // 
            this.olvColumnRelatedApps.FillsFreeSpace = true;
            resources.ApplyResources(this.olvColumnRelatedApps, "olvColumnRelatedApps");
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
            this.pictureBox1.Image = global::BulkCrapUninstaller.Properties.Resources.warning;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // RelatedUninstallerAdder
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.uninstallerObjectListView);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "RelatedUninstallerAdder";
            ((System.ComponentModel.ISupportInitialize)(this.uninstallerObjectListView)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BrightIdeasSoftware.ObjectListView uninstallerObjectListView;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label labelInfo;
        private BrightIdeasSoftware.OLVColumn olvColumnEnabled;
        private BrightIdeasSoftware.OLVColumn olvColumnName;
        private BrightIdeasSoftware.OLVColumn olvColumnRelatedApps;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelInfo2;
    }
}