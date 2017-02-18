namespace BulkCrapUninstaller.Controls
{
    partial class ListLegend
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListLegend));
            this.labelWinFeature = new System.Windows.Forms.Label();
            this.labelOrphaned = new System.Windows.Forms.Label();
            this.labelInvalid = new System.Windows.Forms.Label();
            this.labelUnverified = new System.Windows.Forms.Label();
            this.labelVerified = new System.Windows.Forms.Label();
            this.labelLegend = new System.Windows.Forms.Label();
            this.labelStoreApp = new System.Windows.Forms.Label();
            this.flowLayoutPanellabelVerified = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanellabelUnverified = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanellabelInvalid = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanellabelOrphaned = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanellabelWinFeature = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanellabelStoreApp = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanellabelVerified.SuspendLayout();
            this.flowLayoutPanellabelUnverified.SuspendLayout();
            this.flowLayoutPanellabelInvalid.SuspendLayout();
            this.flowLayoutPanellabelOrphaned.SuspendLayout();
            this.flowLayoutPanellabelWinFeature.SuspendLayout();
            this.flowLayoutPanellabelStoreApp.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelWinFeature
            // 
            resources.ApplyResources(this.labelWinFeature, "labelWinFeature");
            this.labelWinFeature.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelWinFeature.Name = "labelWinFeature";
            this.labelWinFeature.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // labelOrphaned
            // 
            resources.ApplyResources(this.labelOrphaned, "labelOrphaned");
            this.labelOrphaned.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelOrphaned.Name = "labelOrphaned";
            this.labelOrphaned.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // labelInvalid
            // 
            resources.ApplyResources(this.labelInvalid, "labelInvalid");
            this.labelInvalid.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelInvalid.Name = "labelInvalid";
            this.labelInvalid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // labelUnverified
            // 
            resources.ApplyResources(this.labelUnverified, "labelUnverified");
            this.labelUnverified.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelUnverified.Name = "labelUnverified";
            this.labelUnverified.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // labelVerified
            // 
            resources.ApplyResources(this.labelVerified, "labelVerified");
            this.labelVerified.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelVerified.Name = "labelVerified";
            this.labelVerified.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // labelLegend
            // 
            this.labelLegend.BackColor = System.Drawing.SystemColors.Control;
            this.labelLegend.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.labelLegend, "labelLegend");
            this.labelLegend.Name = "labelLegend";
            this.labelLegend.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // labelStoreApp
            // 
            resources.ApplyResources(this.labelStoreApp, "labelStoreApp");
            this.labelStoreApp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelStoreApp.Name = "labelStoreApp";
            // 
            // flowLayoutPanellabelVerified
            // 
            resources.ApplyResources(this.flowLayoutPanellabelVerified, "flowLayoutPanellabelVerified");
            this.flowLayoutPanellabelVerified.BackColor = System.Drawing.Color.PaleGreen;
            this.flowLayoutPanellabelVerified.Controls.Add(this.labelVerified);
            this.flowLayoutPanellabelVerified.Name = "flowLayoutPanellabelVerified";
            this.flowLayoutPanellabelVerified.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // flowLayoutPanellabelUnverified
            // 
            resources.ApplyResources(this.flowLayoutPanellabelUnverified, "flowLayoutPanellabelUnverified");
            this.flowLayoutPanellabelUnverified.BackColor = System.Drawing.Color.Aquamarine;
            this.flowLayoutPanellabelUnverified.Controls.Add(this.labelUnverified);
            this.flowLayoutPanellabelUnverified.Name = "flowLayoutPanellabelUnverified";
            this.flowLayoutPanellabelUnverified.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // flowLayoutPanellabelInvalid
            // 
            resources.ApplyResources(this.flowLayoutPanellabelInvalid, "flowLayoutPanellabelInvalid");
            this.flowLayoutPanellabelInvalid.BackColor = System.Drawing.Color.LightSteelBlue;
            this.flowLayoutPanellabelInvalid.Controls.Add(this.labelInvalid);
            this.flowLayoutPanellabelInvalid.Name = "flowLayoutPanellabelInvalid";
            this.flowLayoutPanellabelInvalid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // flowLayoutPanellabelOrphaned
            // 
            resources.ApplyResources(this.flowLayoutPanellabelOrphaned, "flowLayoutPanellabelOrphaned");
            this.flowLayoutPanellabelOrphaned.BackColor = System.Drawing.Color.LightPink;
            this.flowLayoutPanellabelOrphaned.Controls.Add(this.labelOrphaned);
            this.flowLayoutPanellabelOrphaned.Name = "flowLayoutPanellabelOrphaned";
            this.flowLayoutPanellabelOrphaned.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // flowLayoutPanellabelWinFeature
            // 
            resources.ApplyResources(this.flowLayoutPanellabelWinFeature, "flowLayoutPanellabelWinFeature");
            this.flowLayoutPanellabelWinFeature.BackColor = System.Drawing.Color.SlateBlue;
            this.flowLayoutPanellabelWinFeature.Controls.Add(this.labelWinFeature);
            this.flowLayoutPanellabelWinFeature.Name = "flowLayoutPanellabelWinFeature";
            this.flowLayoutPanellabelWinFeature.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // flowLayoutPanellabelStoreApp
            // 
            resources.ApplyResources(this.flowLayoutPanellabelStoreApp, "flowLayoutPanellabelStoreApp");
            this.flowLayoutPanellabelStoreApp.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.flowLayoutPanellabelStoreApp.Controls.Add(this.labelStoreApp);
            this.flowLayoutPanellabelStoreApp.Name = "flowLayoutPanellabelStoreApp";
            this.flowLayoutPanellabelStoreApp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // ListLegend
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.flowLayoutPanellabelStoreApp);
            this.Controls.Add(this.flowLayoutPanellabelWinFeature);
            this.Controls.Add(this.flowLayoutPanellabelOrphaned);
            this.Controls.Add(this.flowLayoutPanellabelInvalid);
            this.Controls.Add(this.flowLayoutPanellabelUnverified);
            this.Controls.Add(this.flowLayoutPanellabelVerified);
            this.Controls.Add(this.labelLegend);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.MinimumSize = new System.Drawing.Size(158, 2);
            this.Name = "ListLegend";
            this.EnabledChanged += new System.EventHandler(this.ThisEnabledChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.flowLayoutPanellabelVerified.ResumeLayout(false);
            this.flowLayoutPanellabelVerified.PerformLayout();
            this.flowLayoutPanellabelUnverified.ResumeLayout(false);
            this.flowLayoutPanellabelUnverified.PerformLayout();
            this.flowLayoutPanellabelInvalid.ResumeLayout(false);
            this.flowLayoutPanellabelInvalid.PerformLayout();
            this.flowLayoutPanellabelOrphaned.ResumeLayout(false);
            this.flowLayoutPanellabelOrphaned.PerformLayout();
            this.flowLayoutPanellabelWinFeature.ResumeLayout(false);
            this.flowLayoutPanellabelWinFeature.PerformLayout();
            this.flowLayoutPanellabelStoreApp.ResumeLayout(false);
            this.flowLayoutPanellabelStoreApp.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelWinFeature;
        private System.Windows.Forms.Label labelOrphaned;
        private System.Windows.Forms.Label labelInvalid;
        private System.Windows.Forms.Label labelUnverified;
        private System.Windows.Forms.Label labelVerified;
        private System.Windows.Forms.Label labelLegend;
        private System.Windows.Forms.Label labelStoreApp;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanellabelVerified;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanellabelUnverified;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanellabelInvalid;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanellabelOrphaned;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanellabelWinFeature;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanellabelStoreApp;
    }
}
