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
            this.toolStripButtonAddSelectedAsFilters = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonOpenUl = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSaveUl = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSaveUlDef = new System.Windows.Forms.ToolStripButton();
            this.saveUlDialog = new System.Windows.Forms.SaveFileDialog();
            this.openUlDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripUninstallerList.SuspendLayout();
            this.SuspendLayout();
            // 
            // uninstallListEditor1
            // 
            resources.ApplyResources(this.uninstallListEditor1, "uninstallListEditor1");
            this.uninstallListEditor1.Name = "uninstallListEditor1";
            // 
            // toolStripUninstallerList
            // 
            this.toolStripUninstallerList.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripUninstallerList.ImageScalingSize = new System.Drawing.Size(22, 22);
            this.toolStripUninstallerList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonToBasicFilters,
            this.toolStripSeparator3,
            this.toolStripButtonAddSelectedAsFilters,
            this.toolStripSeparator1,
            this.toolStripButtonOpenUl,
            this.toolStripButtonSaveUl,
            this.toolStripButtonSaveUlDef,
            this.toolStripButtonDelete});
            resources.ApplyResources(this.toolStripUninstallerList, "toolStripUninstallerList");
            this.toolStripUninstallerList.Name = "toolStripUninstallerList";
            // 
            // toolStripButtonToBasicFilters
            // 
            this.toolStripButtonToBasicFilters.Image = global::BulkCrapUninstaller.Properties.Resources.magnifyforward;
            resources.ApplyResources(this.toolStripButtonToBasicFilters, "toolStripButtonToBasicFilters");
            this.toolStripButtonToBasicFilters.Name = "toolStripButtonToBasicFilters";
            this.toolStripButtonToBasicFilters.Click += new System.EventHandler(this.toolStripButtonToBasicFilters_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // toolStripButtonAddSelectedAsFilters
            // 
            this.toolStripButtonAddSelectedAsFilters.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAddSelectedAsFilters.Image = global::BulkCrapUninstaller.Properties.Resources.add_multiple;
            resources.ApplyResources(this.toolStripButtonAddSelectedAsFilters, "toolStripButtonAddSelectedAsFilters");
            this.toolStripButtonAddSelectedAsFilters.Name = "toolStripButtonAddSelectedAsFilters";
            this.toolStripButtonAddSelectedAsFilters.Click += new System.EventHandler(this.toolStripButtonAddSelectedAsFilters_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripButtonOpenUl
            // 
            this.toolStripButtonOpenUl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonOpenUl, "toolStripButtonOpenUl");
            this.toolStripButtonOpenUl.Name = "toolStripButtonOpenUl";
            this.toolStripButtonOpenUl.Click += new System.EventHandler(this.toolStripButtonOpenUl_Click);
            // 
            // toolStripButtonSaveUl
            // 
            this.toolStripButtonSaveUl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonSaveUl, "toolStripButtonSaveUl");
            this.toolStripButtonSaveUl.Name = "toolStripButtonSaveUl";
            this.toolStripButtonSaveUl.Click += new System.EventHandler(this.ShowSaveDialog);
            // 
            // toolStripButtonSaveUlDef
            // 
            this.toolStripButtonSaveUlDef.Image = global::BulkCrapUninstaller.Properties.Resources.weather_sun_set;
            resources.ApplyResources(this.toolStripButtonSaveUlDef, "toolStripButtonSaveUlDef");
            this.toolStripButtonSaveUlDef.Name = "toolStripButtonSaveUlDef";
            this.toolStripButtonSaveUlDef.Click += new System.EventHandler(this.toolStripButtonSaveUlDef_Click);
            // 
            // saveUlDialog
            // 
            this.saveUlDialog.DefaultExt = "bcul";
            resources.ApplyResources(this.saveUlDialog, "saveUlDialog");
            this.saveUlDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveUlDialog_FileOk);
            // 
            // openUlDialog
            // 
            this.openUlDialog.DefaultExt = "bcul";
            resources.ApplyResources(this.openUlDialog, "openUlDialog");
            this.openUlDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openUlDialog_FileOk);
            // 
            // toolStripButtonDelete
            // 
            this.toolStripButtonDelete.Image = global::BulkCrapUninstaller.Properties.Resources.delete;
            resources.ApplyResources(this.toolStripButtonDelete, "toolStripButtonDelete");
            this.toolStripButtonDelete.Name = "toolStripButtonDelete";
            this.toolStripButtonDelete.Click += new System.EventHandler(this.toolStripButtonDelete_Click);
            // 
            // AdvancedFilters
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uninstallListEditor1);
            this.Controls.Add(this.toolStripUninstallerList);
            this.Name = "AdvancedFilters";
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
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveUlDef;
        private System.Windows.Forms.SaveFileDialog saveUlDialog;
        private System.Windows.Forms.OpenFileDialog openUlDialog;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddSelectedAsFilters;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonDelete;
    }
}
