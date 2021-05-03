namespace Klocman.Forms
{
    partial class OverlaySplashScreen
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
            this.startupSplashPictureBox = new System.Windows.Forms.PictureBox();
            this.timerFadeout = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.startupSplashPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // startupSplashPictureBox
            // 
            this.startupSplashPictureBox.BackColor = System.Drawing.SystemColors.Window;
            this.startupSplashPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.startupSplashPictureBox.Image = global::Klocman.Properties.Resources.centerline;
            this.startupSplashPictureBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.startupSplashPictureBox.Location = new System.Drawing.Point(0, 0);
            this.startupSplashPictureBox.Name = "startupSplashPictureBox";
            this.startupSplashPictureBox.Size = new System.Drawing.Size(284, 261);
            this.startupSplashPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.startupSplashPictureBox.TabIndex = 0;
            this.startupSplashPictureBox.TabStop = false;
            // 
            // timerFadeout
            // 
            this.timerFadeout.Interval = 25;
            this.timerFadeout.Tick += new System.EventHandler(this.timerFadeout_Tick);
            // 
            // OverlaySplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.ControlBox = false;
            this.Controls.Add(this.startupSplashPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OverlaySplashScreen";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            ((System.ComponentModel.ISupportInitialize)(this.startupSplashPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox startupSplashPictureBox;
        private System.Windows.Forms.Timer timerFadeout;
    }
}