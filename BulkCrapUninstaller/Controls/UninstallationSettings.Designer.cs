namespace BulkCrapUninstaller.Controls
{
    partial class UninstallationSettings
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBoxManualNoCollisionProtection = new System.Windows.Forms.CheckBox();
            this.checkBoxConcurrentOneLoud = new System.Windows.Forms.CheckBox();
            this.checkBoxConcurrent = new System.Windows.Forms.CheckBox();
            this.numericUpDownMaxConcurrent = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxShutdown = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxBatchSortQuiet = new System.Windows.Forms.CheckBox();
            this.checkBoxDiisableProtection = new System.Windows.Forms.CheckBox();
            this.checkBoxSimulate = new System.Windows.Forms.CheckBox();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxConcurrent)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBoxManualNoCollisionProtection);
            this.groupBox3.Controls.Add(this.checkBoxConcurrentOneLoud);
            this.groupBox3.Controls.Add(this.checkBoxConcurrent);
            this.groupBox3.Controls.Add(this.numericUpDownMaxConcurrent);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Location = new System.Drawing.Point(0, 111);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(298, 118);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Concurrent uninstallation";
            // 
            // checkBoxManualNoCollisionProtection
            // 
            this.checkBoxManualNoCollisionProtection.AutoSize = true;
            this.checkBoxManualNoCollisionProtection.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.checkBoxManualNoCollisionProtection.Location = new System.Drawing.Point(6, 82);
            this.checkBoxManualNoCollisionProtection.Name = "checkBoxManualNoCollisionProtection";
            this.checkBoxManualNoCollisionProtection.Size = new System.Drawing.Size(224, 30);
            this.checkBoxManualNoCollisionProtection.TabIndex = 7;
            this.checkBoxManualNoCollisionProtection.Text = "Disable collision prevention when running \r\nuninstallers manually";
            this.checkBoxManualNoCollisionProtection.UseVisualStyleBackColor = true;
            // 
            // checkBoxConcurrentOneLoud
            // 
            this.checkBoxConcurrentOneLoud.AutoSize = true;
            this.checkBoxConcurrentOneLoud.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.checkBoxConcurrentOneLoud.Location = new System.Drawing.Point(19, 60);
            this.checkBoxConcurrentOneLoud.Name = "checkBoxConcurrentOneLoud";
            this.checkBoxConcurrentOneLoud.Size = new System.Drawing.Size(184, 17);
            this.checkBoxConcurrentOneLoud.TabIndex = 5;
            this.checkBoxConcurrentOneLoud.Text = "Only one loud uninstaller at a time";
            this.checkBoxConcurrentOneLoud.UseVisualStyleBackColor = true;
            // 
            // checkBoxConcurrent
            // 
            this.checkBoxConcurrent.AutoSize = true;
            this.checkBoxConcurrent.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.checkBoxConcurrent.Location = new System.Drawing.Point(6, 19);
            this.checkBoxConcurrent.Name = "checkBoxConcurrent";
            this.checkBoxConcurrent.Size = new System.Drawing.Size(277, 17);
            this.checkBoxConcurrent.TabIndex = 0;
            this.checkBoxConcurrent.Text = "Automatically run uninstallers concurrently (if possible)";
            this.checkBoxConcurrent.UseVisualStyleBackColor = true;
            // 
            // numericUpDownMaxConcurrent
            // 
            this.numericUpDownMaxConcurrent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownMaxConcurrent.Location = new System.Drawing.Point(223, 39);
            this.numericUpDownMaxConcurrent.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownMaxConcurrent.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownMaxConcurrent.Name = "numericUpDownMaxConcurrent";
            this.numericUpDownMaxConcurrent.Size = new System.Drawing.Size(69, 20);
            this.numericUpDownMaxConcurrent.TabIndex = 4;
            this.numericUpDownMaxConcurrent.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(16, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(173, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Max number of running uninstallers:";
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSize = true;
            this.groupBox2.Controls.Add(this.flowLayoutPanel4);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(298, 111);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General settings";
            // 
            // checkBoxShutdown
            // 
            this.checkBoxShutdown.AutoSize = true;
            this.checkBoxShutdown.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.checkBoxShutdown.Location = new System.Drawing.Point(3, 3);
            this.checkBoxShutdown.Name = "checkBoxShutdown";
            this.checkBoxShutdown.Size = new System.Drawing.Size(181, 17);
            this.checkBoxShutdown.TabIndex = 0;
            this.checkBoxShutdown.Text = "Prevent system shutdown/restart";
            this.checkBoxShutdown.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.AutoSize = true;
            this.flowLayoutPanel4.Controls.Add(this.checkBoxShutdown);
            this.flowLayoutPanel4.Controls.Add(this.checkBoxBatchSortQuiet);
            this.flowLayoutPanel4.Controls.Add(this.checkBoxDiisableProtection);
            this.flowLayoutPanel4.Controls.Add(this.checkBoxSimulate);
            this.flowLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel4.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel4.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(292, 92);
            this.flowLayoutPanel4.TabIndex = 1;
            this.flowLayoutPanel4.WrapContents = false;
            // 
            // checkBoxBatchSortQuiet
            // 
            this.checkBoxBatchSortQuiet.AutoSize = true;
            this.checkBoxBatchSortQuiet.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.checkBoxBatchSortQuiet.Location = new System.Drawing.Point(3, 26);
            this.checkBoxBatchSortQuiet.Name = "checkBoxBatchSortQuiet";
            this.checkBoxBatchSortQuiet.Size = new System.Drawing.Size(155, 17);
            this.checkBoxBatchSortQuiet.TabIndex = 0;
            this.checkBoxBatchSortQuiet.Text = "Intelligent uninstaller sorting";
            this.checkBoxBatchSortQuiet.UseVisualStyleBackColor = true;
            // 
            // checkBoxDiisableProtection
            // 
            this.checkBoxDiisableProtection.AutoSize = true;
            this.checkBoxDiisableProtection.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.checkBoxDiisableProtection.Location = new System.Drawing.Point(3, 49);
            this.checkBoxDiisableProtection.Name = "checkBoxDiisableProtection";
            this.checkBoxDiisableProtection.Size = new System.Drawing.Size(111, 17);
            this.checkBoxDiisableProtection.TabIndex = 3;
            this.checkBoxDiisableProtection.Text = "Disable protection";
            this.checkBoxDiisableProtection.UseVisualStyleBackColor = true;
            // 
            // checkBoxSimulate
            // 
            this.checkBoxSimulate.AutoSize = true;
            this.checkBoxSimulate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.checkBoxSimulate.Location = new System.Drawing.Point(3, 72);
            this.checkBoxSimulate.Name = "checkBoxSimulate";
            this.checkBoxSimulate.Size = new System.Drawing.Size(130, 17);
            this.checkBoxSimulate.TabIndex = 4;
            this.checkBoxSimulate.Text = "Simulate uninstallation";
            this.checkBoxSimulate.UseVisualStyleBackColor = true;
            // 
            // UninstallationSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Name = "UninstallationSettings";
            this.Size = new System.Drawing.Size(298, 414);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxConcurrent)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBoxConcurrentOneLoud;
        private System.Windows.Forms.CheckBox checkBoxConcurrent;
        private System.Windows.Forms.NumericUpDown numericUpDownMaxConcurrent;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBoxShutdown;
        private System.Windows.Forms.CheckBox checkBoxManualNoCollisionProtection;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.CheckBox checkBoxBatchSortQuiet;
        private System.Windows.Forms.CheckBox checkBoxDiisableProtection;
        private System.Windows.Forms.CheckBox checkBoxSimulate;
    }
}
