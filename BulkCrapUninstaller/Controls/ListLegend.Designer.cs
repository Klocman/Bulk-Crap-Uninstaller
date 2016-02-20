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
            this.SuspendLayout();
            // 
            // labelWinFeature
            // 
            this.labelWinFeature.BackColor = System.Drawing.Color.SlateBlue;
            this.labelWinFeature.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.labelWinFeature, "labelWinFeature");
            this.labelWinFeature.Name = "labelWinFeature";
            this.labelWinFeature.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // labelOrphaned
            // 
            this.labelOrphaned.BackColor = System.Drawing.Color.LightPink;
            this.labelOrphaned.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.labelOrphaned, "labelOrphaned");
            this.labelOrphaned.Name = "labelOrphaned";
            this.labelOrphaned.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // labelInvalid
            // 
            this.labelInvalid.BackColor = System.Drawing.Color.LightSteelBlue;
            this.labelInvalid.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.labelInvalid, "labelInvalid");
            this.labelInvalid.Name = "labelInvalid";
            this.labelInvalid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // labelUnverified
            // 
            this.labelUnverified.BackColor = System.Drawing.Color.Aquamarine;
            this.labelUnverified.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.labelUnverified, "labelUnverified");
            this.labelUnverified.Name = "labelUnverified";
            this.labelUnverified.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // labelVerified
            // 
            this.labelVerified.BackColor = System.Drawing.Color.PaleGreen;
            this.labelVerified.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.labelVerified, "labelVerified");
            this.labelVerified.Name = "labelVerified";
            this.labelVerified.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // labelLegend
            // 
            this.labelLegend.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.labelLegend, "labelLegend");
            this.labelLegend.Name = "labelLegend";
            this.labelLegend.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // labelStoreApp
            // 
            this.labelStoreApp.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.labelStoreApp.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.labelStoreApp, "labelStoreApp");
            this.labelStoreApp.Name = "labelStoreApp";
            // 
            // ListLegend
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.labelStoreApp);
            this.Controls.Add(this.labelWinFeature);
            this.Controls.Add(this.labelOrphaned);
            this.Controls.Add(this.labelInvalid);
            this.Controls.Add(this.labelUnverified);
            this.Controls.Add(this.labelVerified);
            this.Controls.Add(this.labelLegend);
            this.MinimumSize = new System.Drawing.Size(130, 0);
            this.Name = "ListLegend";
            this.EnabledChanged += new System.EventHandler(this.ThisEnabledChanged);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label labelWinFeature;
        private System.Windows.Forms.Label labelOrphaned;
        private System.Windows.Forms.Label labelInvalid;
        private System.Windows.Forms.Label labelUnverified;
        private System.Windows.Forms.Label labelVerified;
        private System.Windows.Forms.Label labelLegend;
        private System.Windows.Forms.Label labelStoreApp;
    }
}
