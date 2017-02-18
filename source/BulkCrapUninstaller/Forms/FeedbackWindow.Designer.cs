namespace BulkCrapUninstaller.Forms
{
    partial class FeedbackWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FeedbackWindow));
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.loadingLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // webBrowser
            // 
            resources.ApplyResources(this.webBrowser, "webBrowser");
            this.webBrowser.AllowWebBrowserDrop = false;
            this.webBrowser.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser.Name = "webBrowser";
            // 
            // loadingLabel
            // 
            resources.ApplyResources(this.loadingLabel, "loadingLabel");
            this.loadingLabel.Name = "loadingLabel";
            this.loadingLabel.UseWaitCursor = true;
            // 
            // FeedbackWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.loadingLabel);
            this.Controls.Add(this.webBrowser);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FeedbackWindow";
            this.Shown += new System.EventHandler(this.FeedbackWindow_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.Label loadingLabel;
    }
}