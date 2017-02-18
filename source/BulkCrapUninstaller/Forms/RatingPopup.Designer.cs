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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RatingPopup));
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
            resources.ApplyResources(this.buttonGood, "buttonGood");
            this.buttonGood.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.buttonGood.ForeColor = System.Drawing.Color.Black;
            this.buttonGood.Name = "buttonGood";
            this.buttonGood.UseVisualStyleBackColor = false;
            this.buttonGood.Click += new System.EventHandler(this.buttonGood_Click);
            // 
            // buttonBad
            // 
            resources.ApplyResources(this.buttonBad, "buttonBad");
            this.buttonBad.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.buttonBad.ForeColor = System.Drawing.Color.Black;
            this.buttonBad.Name = "buttonBad";
            this.buttonBad.UseVisualStyleBackColor = false;
            this.buttonBad.Click += new System.EventHandler(this.buttonBad_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.buttonBad);
            this.groupBox1.Controls.Add(this.buttonNormal);
            this.groupBox1.Controls.Add(this.buttonGood);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // buttonNormal
            // 
            resources.ApplyResources(this.buttonNormal, "buttonNormal");
            this.buttonNormal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.buttonNormal.ForeColor = System.Drawing.Color.Black;
            this.buttonNormal.Name = "buttonNormal";
            this.buttonNormal.UseVisualStyleBackColor = false;
            this.buttonNormal.Click += new System.EventHandler(this.buttonNormal_Click);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Name = "panel1";
            // 
            // RatingPopup
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "RatingPopup";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
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