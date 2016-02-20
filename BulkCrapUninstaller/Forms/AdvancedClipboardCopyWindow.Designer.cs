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
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonCopyAll = new System.Windows.Forms.Button();
            this.advancedClipboardCopy1 = new BulkCrapUninstaller.Controls.AdvancedClipboardCopy();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonClose);
            this.panel1.Controls.Add(this.buttonCopyAll);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 305);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(534, 36);
            this.panel1.TabIndex = 1;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(453, 6);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.button2_Click);
            // 
            // buttonCopyAll
            // 
            this.buttonCopyAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCopyAll.Location = new System.Drawing.Point(332, 6);
            this.buttonCopyAll.Name = "buttonCopyAll";
            this.buttonCopyAll.Size = new System.Drawing.Size(115, 23);
            this.buttonCopyAll.TabIndex = 0;
            this.buttonCopyAll.Text = "Copy all";
            this.buttonCopyAll.UseVisualStyleBackColor = true;
            this.buttonCopyAll.Click += new System.EventHandler(this.button1_Click);
            // 
            // advancedClipboardCopy1
            // 
            this.advancedClipboardCopy1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.advancedClipboardCopy1.Location = new System.Drawing.Point(0, 0);
            this.advancedClipboardCopy1.Name = "advancedClipboardCopy1";
            this.advancedClipboardCopy1.Padding = new System.Windows.Forms.Padding(6, 6, 6, 3);
            this.advancedClipboardCopy1.PatternText = "";
            this.advancedClipboardCopy1.Size = new System.Drawing.Size(534, 305);
            this.advancedClipboardCopy1.TabIndex = 0;
            this.advancedClipboardCopy1.Targets = null;
            // 
            // AdvancedClipboardCopyWindow
            // 
            this.AcceptButton = this.buttonCopyAll;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(534, 341);
            this.Controls.Add(this.advancedClipboardCopy1);
            this.Controls.Add(this.panel1);
            this.Name = "AdvancedClipboardCopyWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Copy information to clipboard";
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