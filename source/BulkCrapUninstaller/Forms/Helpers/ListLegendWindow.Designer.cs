namespace BulkCrapUninstaller.Forms
{
    partial class ListLegendWindow
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
            this.components = new System.ComponentModel.Container();
            this.opacityResetTimer = new System.Windows.Forms.Timer(this.components);
            this.listLegend1 = new BulkCrapUninstaller.Controls.ListLegend();
            this.SuspendLayout();
            // 
            // opacityResetTimer
            // 
            this.opacityResetTimer.Interval = 25;
            this.opacityResetTimer.Tick += new System.EventHandler(this.opacityResetTimer_Tick);
            // 
            // listLegend1
            // 
            this.listLegend1.AutoSize = true;
            this.listLegend1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.listLegend1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listLegend1.CertificatesEnabled = true;
            this.listLegend1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.listLegend1.InvalidEnabled = true;
            this.listLegend1.Location = new System.Drawing.Point(0, 0);
            this.listLegend1.MinimumSize = new System.Drawing.Size(158, 2);
            this.listLegend1.Name = "listLegend1";
            this.listLegend1.OrphanedEnabled = true;
            this.listLegend1.Size = new System.Drawing.Size(158, 148);
            this.listLegend1.StoreAppEnabled = true;
            this.listLegend1.TabIndex = 0;
            this.listLegend1.WinFeatureEnabled = true;
            // 
            // ListLegendWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Fuchsia;
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.ControlBox = false;
            this.Controls.Add(this.listLegend1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ListLegendWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ListLegendWindow";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.EnabledChanged += new System.EventHandler(this.ListLegendWindow_EnabledChanged);
            this.VisibleChanged += new System.EventHandler(this.ListLegendWindow_VisibleChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.ListLegend listLegend1;
        private System.Windows.Forms.Timer opacityResetTimer;
    }
}