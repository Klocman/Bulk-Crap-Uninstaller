namespace BulkCrapUninstaller.Forms
{
    partial class RatingPopup
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
            this.buttonGood = new System.Windows.Forms.Button();
            this.buttonBad = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonNormal = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonGood
            // 
            this.buttonGood.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.buttonGood.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonGood.ForeColor = System.Drawing.Color.Black;
            this.buttonGood.Location = new System.Drawing.Point(8, 21);
            this.buttonGood.Name = "buttonGood";
            this.buttonGood.Size = new System.Drawing.Size(232, 26);
            this.buttonGood.TabIndex = 0;
            this.buttonGood.Text = "Useful | Good quality";
            this.buttonGood.UseVisualStyleBackColor = false;
            this.buttonGood.Click += new System.EventHandler(this.buttonGood_Click);
            // 
            // buttonBad
            // 
            this.buttonBad.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.buttonBad.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonBad.ForeColor = System.Drawing.Color.Black;
            this.buttonBad.Location = new System.Drawing.Point(8, 73);
            this.buttonBad.Name = "buttonBad";
            this.buttonBad.Size = new System.Drawing.Size(232, 26);
            this.buttonBad.TabIndex = 2;
            this.buttonBad.Text = "Junk | Potentially dangerous";
            this.buttonBad.UseVisualStyleBackColor = false;
            this.buttonBad.Click += new System.EventHandler(this.buttonBad_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(173, 6);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonBad);
            this.groupBox1.Controls.Add(this.buttonNormal);
            this.groupBox1.Controls.Add(this.buttonGood);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(8);
            this.groupBox1.Size = new System.Drawing.Size(248, 107);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "How do you rate this application?";
            // 
            // buttonNormal
            // 
            this.buttonNormal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.buttonNormal.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonNormal.ForeColor = System.Drawing.Color.Black;
            this.buttonNormal.Location = new System.Drawing.Point(8, 47);
            this.buttonNormal.Name = "buttonNormal";
            this.buttonNormal.Size = new System.Drawing.Size(232, 26);
            this.buttonNormal.TabIndex = 1;
            this.buttonNormal.Text = "Questionable |  Bad quality";
            this.buttonNormal.UseVisualStyleBackColor = false;
            this.buttonNormal.Click += new System.EventHandler(this.buttonNormal_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(8, 115);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(248, 29);
            this.panel1.TabIndex = 1;
            // 
            // RatingPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(264, 152);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "RatingPopup";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Rate";
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonGood;
        private System.Windows.Forms.Button buttonBad;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonNormal;
        private System.Windows.Forms.Panel panel1;
    }
}