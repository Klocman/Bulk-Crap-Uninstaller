namespace BulkCrapUninstaller.Forms
{
    partial class AdvancedClipboardCopyWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedClipboardCopyWindow));
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonCopyAll = new System.Windows.Forms.Button();
            this.advancedClipboardCopy1 = new BulkCrapUninstaller.Controls.AdvancedClipboardCopy();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.buttonClose);
            this.panel1.Controls.Add(this.buttonCopyAll);
            this.panel1.Name = "panel1";
            // 
            // buttonClose
            // 
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.button2_Click);
            // 
            // buttonCopyAll
            // 
            resources.ApplyResources(this.buttonCopyAll, "buttonCopyAll");
            this.buttonCopyAll.Name = "buttonCopyAll";
            this.buttonCopyAll.UseVisualStyleBackColor = true;
            this.buttonCopyAll.Click += new System.EventHandler(this.button1_Click);
            // 
            // advancedClipboardCopy1
            // 
            resources.ApplyResources(this.advancedClipboardCopy1, "advancedClipboardCopy1");
            this.advancedClipboardCopy1.Name = "advancedClipboardCopy1";
            this.advancedClipboardCopy1.PatternText = "";
            this.advancedClipboardCopy1.Targets = null;
            // 
            // AdvancedClipboardCopyWindow
            // 
            this.AcceptButton = this.buttonCopyAll;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.Controls.Add(this.advancedClipboardCopy1);
            this.Controls.Add(this.panel1);
            this.Name = "AdvancedClipboardCopyWindow";
            this.Shown += new System.EventHandler(this.AdvancedClipboardCopyWindow_Shown);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.AdvancedClipboardCopy advancedClipboardCopy1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonCopyAll;
    }
}