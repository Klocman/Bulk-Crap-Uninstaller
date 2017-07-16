namespace BulkCrapUninstaller.Forms
{
    partial class UninstallConfirmationWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UninstallConfirmationWindow));
            this.objectListView1 = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnEnabled = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnQuiet = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnInstallLocation = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonAccept = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
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
            this.objectListView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectListView1.GridLines = true;
            this.objectListView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.objectListView1.Location = new System.Drawing.Point(6, 51);
            this.objectListView1.MultiSelect = false;
            this.objectListView1.Name = "objectListView1";
            this.objectListView1.ShowGroups = false;
            this.objectListView1.ShowItemToolTips = true;
            this.objectListView1.Size = new System.Drawing.Size(619, 206);
            this.objectListView1.TabIndex = 0;
            this.objectListView1.UseCompatibleStateImageBehavior = false;
            this.objectListView1.View = System.Windows.Forms.View.Details;
            // 
            // olvColumnName
            // 
            this.olvColumnName.Text = "Uninstaller name";
            this.olvColumnName.Width = 238;
            // 
            // olvColumnEnabled
            // 
            this.olvColumnEnabled.CheckBoxes = true;
            this.olvColumnEnabled.Hideable = false;
            this.olvColumnEnabled.Text = "Uninstall";
            this.olvColumnEnabled.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // olvColumnQuiet
            // 
            this.olvColumnQuiet.CheckBoxes = true;
            this.olvColumnQuiet.Text = "Quiet";
            this.olvColumnQuiet.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // olvColumnInstallLocation
            // 
            this.olvColumnInstallLocation.Text = "Install location";
            this.olvColumnInstallLocation.Width = 246;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonAccept);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(6, 257);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.panel1.Size = new System.Drawing.Size(619, 32);
            this.panel1.TabIndex = 1;
            // 
            // buttonAccept
            // 
            this.buttonAccept.AutoSize = true;
            this.buttonAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonAccept.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonAccept.Location = new System.Drawing.Point(457, 6);
            this.buttonAccept.Name = "buttonAccept";
            this.buttonAccept.Size = new System.Drawing.Size(75, 26);
            this.buttonAccept.TabIndex = 0;
            this.buttonAccept.Text = "&Accept";
            this.buttonAccept.UseVisualStyleBackColor = true;
            this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(532, 6);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(12, 26);
            this.panel2.TabIndex = 1;
            // 
            // buttonCancel
            // 
            this.buttonCancel.AutoSize = true;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonCancel.Location = new System.Drawing.Point(544, 6);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 26);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(6, 6);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(619, 45);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(609, 39);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // UninstallConfirmationWindow
            // 
            this.AcceptButton = this.buttonAccept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(631, 295);
            this.Controls.Add(this.objectListView1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.Name = "UninstallConfirmationWindow";
            this.Padding = new System.Windows.Forms.Padding(6);
            this.Text = "Confirm uninstallation task";
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BrightIdeasSoftware.ObjectListView objectListView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonAccept;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private BrightIdeasSoftware.OLVColumn olvColumnEnabled;
        private BrightIdeasSoftware.OLVColumn olvColumnQuiet;
        private BrightIdeasSoftware.OLVColumn olvColumnName;
        private BrightIdeasSoftware.OLVColumn olvColumnInstallLocation;
    }
}