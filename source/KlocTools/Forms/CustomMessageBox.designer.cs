namespace Klocman.Forms
{
    using System.ComponentModel;
    using System.Windows.Forms;

    sealed partial class CustomMessageBox
    {
        #region Fields

        private Button buttonLeft;
        private Button buttonMiddle;
        private Button buttonRight;
        private CheckBox checkBox1;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;
        private Label label1;
        private Label label2;
        private PictureBox pictureBox1;

        #endregion Fields

        #region Methods

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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonRight = new System.Windows.Forms.Button();
            this.buttonLeft = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonMiddle = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.panelMiddle = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Window;
            this.pictureBox1.Location = new System.Drawing.Point(25, 18);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 11, 8, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.label1, true);
            this.label1.Location = new System.Drawing.Point(68, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 12, 0, 12);
            this.label1.Size = new System.Drawing.Size(85, 37);
            this.label1.TabIndex = 2;
            this.label1.Text = "Dialog message.";
            // 
            // buttonRight
            // 
            this.buttonRight.AutoSize = true;
            this.buttonRight.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonRight.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonRight.Location = new System.Drawing.Point(338, 11);
            this.buttonRight.MinimumSize = new System.Drawing.Size(75, 0);
            this.buttonRight.Name = "buttonRight";
            this.buttonRight.Size = new System.Drawing.Size(75, 24);
            this.buttonRight.TabIndex = 2;
            this.buttonRight.Text = "Right";
            this.buttonRight.UseVisualStyleBackColor = true;
            this.buttonRight.Click += new System.EventHandler(this.buttonRight_Click);
            // 
            // buttonLeft
            // 
            this.buttonLeft.AutoSize = true;
            this.buttonLeft.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonLeft.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonLeft.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonLeft.Location = new System.Drawing.Point(168, 11);
            this.buttonLeft.MinimumSize = new System.Drawing.Size(75, 0);
            this.buttonLeft.Name = "buttonLeft";
            this.buttonLeft.Size = new System.Drawing.Size(75, 24);
            this.buttonLeft.TabIndex = 0;
            this.buttonLeft.Text = "Left";
            this.buttonLeft.UseVisualStyleBackColor = true;
            this.buttonLeft.Click += new System.EventHandler(this.buttonLeft_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.label2, true);
            this.label2.Location = new System.Drawing.Point(68, 37);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(5, 5, 5, 14);
            this.label2.Size = new System.Drawing.Size(53, 32);
            this.label2.TabIndex = 0;
            this.label2.Text = "Caption";
            // 
            // buttonMiddle
            // 
            this.buttonMiddle.AutoSize = true;
            this.buttonMiddle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonMiddle.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonMiddle.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonMiddle.Location = new System.Drawing.Point(253, 11);
            this.buttonMiddle.MinimumSize = new System.Drawing.Size(75, 0);
            this.buttonMiddle.Name = "buttonMiddle";
            this.buttonMiddle.Size = new System.Drawing.Size(75, 24);
            this.buttonMiddle.TabIndex = 1;
            this.buttonMiddle.Text = "Middle";
            this.buttonMiddle.UseVisualStyleBackColor = true;
            this.buttonMiddle.Click += new System.EventHandler(this.buttonMiddle_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.checkBox1.Location = new System.Drawing.Point(11, 11);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(112, 24);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "Remember choice";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.Window;
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.MinimumSize = new System.Drawing.Size(0, 65);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(65, 0, 0, 0);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(424, 69);
            this.flowLayoutPanel1.TabIndex = 3;
            this.flowLayoutPanel1.SizeChanged += new System.EventHandler(this.flowLayoutPanel1_SizeChanged);
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.panel1);
            this.panelButtons.Controls.Add(this.buttonLeft);
            this.panelButtons.Controls.Add(this.panelLeft);
            this.panelButtons.Controls.Add(this.buttonMiddle);
            this.panelButtons.Controls.Add(this.panelMiddle);
            this.panelButtons.Controls.Add(this.buttonRight);
            this.panelButtons.Controls.Add(this.checkBox1);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelButtons.Location = new System.Drawing.Point(0, 69);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Padding = new System.Windows.Forms.Padding(11);
            this.panelButtons.Size = new System.Drawing.Size(424, 46);
            this.panelButtons.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(158, 11);
            this.panel1.MaximumSize = new System.Drawing.Size(10, 11111111);
            this.panel1.MinimumSize = new System.Drawing.Size(10, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(10, 24);
            this.panel1.TabIndex = 6;
            // 
            // panelLeft
            // 
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelLeft.Location = new System.Drawing.Point(243, 11);
            this.panelLeft.MaximumSize = new System.Drawing.Size(10, 11111111);
            this.panelLeft.MinimumSize = new System.Drawing.Size(10, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(10, 24);
            this.panelLeft.TabIndex = 4;
            // 
            // panelMiddle
            // 
            this.panelMiddle.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelMiddle.Location = new System.Drawing.Point(328, 11);
            this.panelMiddle.MaximumSize = new System.Drawing.Size(10, 11111111);
            this.panelMiddle.MinimumSize = new System.Drawing.Size(10, 0);
            this.panelMiddle.Name = "panelMiddle";
            this.panelMiddle.Size = new System.Drawing.Size(10, 24);
            this.panelMiddle.TabIndex = 5;
            // 
            // CustomMessageBox
            // 
            this.AcceptButton = this.buttonLeft;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonRight;
            this.ClientSize = new System.Drawing.Size(424, 347);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this.flowLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(375, 171);
            this.Name = "CustomMessageBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Special Dialog";
            this.SizeChanged += new System.EventHandler(this.CustomMessageBox_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.panelButtons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion Methods
        private FlowLayoutPanel flowLayoutPanel1;
        private Panel panelButtons;
        private Panel panelLeft;
        private Panel panelMiddle;
        private Panel panel1;
    }
}