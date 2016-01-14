namespace BulkCrapUninstaller.Controls
{
    partial class AdvancedFilters
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedFilters));
            this.uninstallListEditor1 = new UninstallTools.Controls.UninstallListEditor();
            this.toolStripUninstallerList = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonToBasicFilters = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonOpenUl = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSaveUl = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSaveUlDef = new System.Windows.Forms.ToolStripButton();
            this.saveUlDialog = new System.Windows.Forms.SaveFileDialog();
            this.openUlDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolStripUninstallerList.SuspendLayout();
            this.SuspendLayout();
            // 
            // uninstallListEditor1
            // 
            this.uninstallListEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uninstallListEditor1.Location = new System.Drawing.Point(0, 29);
            this.uninstallListEditor1.MinimumSize = new System.Drawing.Size(310, 0);
            this.uninstallListEditor1.Name = "uninstallListEditor1";
            this.uninstallListEditor1.Size = new System.Drawing.Size(319, 483);
            this.uninstallListEditor1.TabIndex = 2;
            // 
            // toolStripUninstallerList
            // 
            this.toolStripUninstallerList.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripUninstallerList.ImageScalingSize = new System.Drawing.Size(22, 22);
            this.toolStripUninstallerList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonToBasicFilters,
            this.toolStripSeparator3,
            this.toolStripButtonOpenUl,
            this.toolStripButtonSaveUl,
            this.toolStripSeparator25,
            this.toolStripButtonSaveUlDef});
            this.toolStripUninstallerList.Location = new System.Drawing.Point(0, 0);
            this.toolStripUninstallerList.Name = "toolStripUninstallerList";
            this.toolStripUninstallerList.Size = new System.Drawing.Size(319, 29);
            this.toolStripUninstallerList.TabIndex = 3;
            // 
            // toolStripButtonToBasicFilters
            // 
            this.toolStripButtonToBasicFilters.Image = global::BulkCrapUninstaller.Properties.Resources.magnifyforward;
            this.toolStripButtonToBasicFilters.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonToBasicFilters.Name = "toolStripButtonToBasicFilters";
            this.toolStripButtonToBasicFilters.Size = new System.Drawing.Size(104, 26);
            this.toolStripButtonToBasicFilters.Text = "Basic filtering";
            this.toolStripButtonToBasicFilters.Click += new System.EventHandler(this.toolStripButtonToBasicFilters_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 29);
            // 
            // toolStripButtonOpenUl
            // 
            this.toolStripButtonOpenUl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonOpenUl.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonOpenUl.Image")));
            this.toolStripButtonOpenUl.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOpenUl.Name = "toolStripButtonOpenUl";
            this.toolStripButtonOpenUl.Size = new System.Drawing.Size(26, 26);
            this.toolStripButtonOpenUl.Text = "Open...";
            this.toolStripButtonOpenUl.Click += new System.EventHandler(this.toolStripButtonOpenUl_Click);
            // 
            // toolStripButtonSaveUl
            // 
            this.toolStripButtonSaveUl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSaveUl.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSaveUl.Image")));
            this.toolStripButtonSaveUl.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSaveUl.Name = "toolStripButtonSaveUl";
            this.toolStripButtonSaveUl.Size = new System.Drawing.Size(26, 26);
            this.toolStripButtonSaveUl.Text = "Save as...";
            this.toolStripButtonSaveUl.Click += new System.EventHandler(this.toolStripButtonSaveUl_Click);
            // 
            // toolStripSeparator25
            // 
            this.toolStripSeparator25.Name = "toolStripSeparator25";
            this.toolStripSeparator25.Size = new System.Drawing.Size(6, 29);
            // 
            // toolStripButtonSaveUlDef
            // 
            this.toolStripButtonSaveUlDef.Image = global::BulkCrapUninstaller.Properties.Resources.save;
            this.toolStripButtonSaveUlDef.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSaveUlDef.Name = "toolStripButtonSaveUlDef";
            this.toolStripButtonSaveUlDef.Size = new System.Drawing.Size(111, 26);
            this.toolStripButtonSaveUlDef.Text = "Save as default";
            this.toolStripButtonSaveUlDef.Click += new System.EventHandler(this.toolStripButtonSaveUlDef_Click);
            // 
            // saveUlDialog
            // 
            this.saveUlDialog.DefaultExt = "bcul";
            this.saveUlDialog.Filter = "Uninstall lists|*.bcul";
            this.saveUlDialog.Title = "Save an Uninstall List...";
            this.saveUlDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveUlDialog_FileOk);
            // 
            // openUlDialog
            // 
            this.openUlDialog.DefaultExt = "bcul";
            this.openUlDialog.Filter = "Uninstall lists|*.bcul";
            this.openUlDialog.Title = "Open an Uninstall List...";
            this.openUlDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openUlDialog_FileOk);
            // 
            // AdvancedFilters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uninstallListEditor1);
            this.Controls.Add(this.toolStripUninstallerList);
            this.Name = "AdvancedFilters";
            this.Size = new System.Drawing.Size(319, 512);
            this.toolStripUninstallerList.ResumeLayout(false);
            this.toolStripUninstallerList.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UninstallTools.Controls.UninstallListEditor uninstallListEditor1;
        private System.Windows.Forms.ToolStrip toolStripUninstallerList;
        private System.Windows.Forms.ToolStripButton toolStripButtonToBasicFilters;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButtonOpenUl;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveUl;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator25;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveUlDef;
        private System.Windows.Forms.SaveFileDialog saveUlDialog;
        private System.Windows.Forms.OpenFileDialog openUlDialog;
    }
}
