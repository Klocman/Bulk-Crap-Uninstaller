namespace BulkCrapUninstaller.Forms.Windows
{
    partial class Form1
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
            listViewPanel = new System.Windows.Forms.Panel();
            uninstallerObjectListView = new BrightIdeasSoftware.ObjectListView();
            listViewPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)uninstallerObjectListView).BeginInit();
            SuspendLayout();
            // 
            // listViewPanel
            // 
            listViewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            listViewPanel.Controls.Add(uninstallerObjectListView);
            listViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            listViewPanel.Location = new System.Drawing.Point(0, 0);
            listViewPanel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            listViewPanel.Name = "listViewPanel";
            listViewPanel.Size = new System.Drawing.Size(800, 450);
            listViewPanel.TabIndex = 7;
            // 
            // uninstallerObjectListView
            // 
            uninstallerObjectListView.AllowColumnReorder = true;
            uninstallerObjectListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            uninstallerObjectListView.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            uninstallerObjectListView.CellEditUseWholeCell = false;
            uninstallerObjectListView.CheckBoxes = true;
            uninstallerObjectListView.Dock = System.Windows.Forms.DockStyle.Fill;
            uninstallerObjectListView.FullRowSelect = true;
            uninstallerObjectListView.GridLines = true;
            uninstallerObjectListView.Location = new System.Drawing.Point(0, 0);
            uninstallerObjectListView.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            uninstallerObjectListView.Name = "uninstallerObjectListView";
            uninstallerObjectListView.ShowGroups = false;
            uninstallerObjectListView.ShowImagesOnSubItems = true;
            uninstallerObjectListView.ShowItemToolTips = true;
            uninstallerObjectListView.Size = new System.Drawing.Size(798, 448);
            uninstallerObjectListView.SortGroupItemsByPrimaryColumn = false;
            uninstallerObjectListView.TabIndex = 0;
            uninstallerObjectListView.UseCompatibleStateImageBehavior = false;
            uninstallerObjectListView.UseFilterIndicator = true;
            uninstallerObjectListView.UseHyperlinks = true;
            uninstallerObjectListView.View = System.Windows.Forms.View.Details;
            uninstallerObjectListView.VirtualMode = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(listViewPanel);
            Name = "Form1";
            Text = "Form1";
            listViewPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)uninstallerObjectListView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel listViewPanel;
        internal BrightIdeasSoftware.ObjectListView uninstallerObjectListView;
    }
}