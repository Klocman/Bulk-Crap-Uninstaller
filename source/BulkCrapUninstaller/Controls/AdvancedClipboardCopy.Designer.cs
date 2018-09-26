namespace BulkCrapUninstaller.Controls
{
    partial class AdvancedClipboardCopy
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedClipboardCopy));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.comboBoxInsert = new System.Windows.Forms.ComboBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.checkBoxUnescape = new System.Windows.Forms.CheckBox();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.textBoxPatternInput = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxResults = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Controls.Add(this.textBoxPatternInput);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.comboBoxInsert);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.checkBoxUnescape);
            this.panel2.Controls.Add(this.buttonHelp);
            this.panel2.Name = "panel2";
            // 
            // comboBoxInsert
            // 
            resources.ApplyResources(this.comboBoxInsert, "comboBoxInsert");
            this.comboBoxInsert.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxInsert.FormattingEnabled = true;
            this.comboBoxInsert.Items.AddRange(new object[] {
            resources.GetString("comboBoxInsert.Items")});
            this.comboBoxInsert.Name = "comboBoxInsert";
            this.comboBoxInsert.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // checkBoxUnescape
            // 
            resources.ApplyResources(this.checkBoxUnescape, "checkBoxUnescape");
            this.checkBoxUnescape.Name = "checkBoxUnescape";
            this.checkBoxUnescape.UseVisualStyleBackColor = true;
            this.checkBoxUnescape.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // buttonHelp
            // 
            resources.ApplyResources(this.buttonHelp, "buttonHelp");
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // textBoxPatternInput
            // 
            resources.ApplyResources(this.textBoxPatternInput, "textBoxPatternInput");
            this.textBoxPatternInput.Name = "textBoxPatternInput";
            this.textBoxPatternInput.TextChanged += new System.EventHandler(this.RefreshResult);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxResults);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // textBoxResults
            // 
            this.textBoxResults.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.textBoxResults, "textBoxResults");
            this.textBoxResults.Name = "textBoxResults";
            this.textBoxResults.ReadOnly = true;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // AdvancedClipboardCopy
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Name = "AdvancedClipboardCopy";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxPatternInput;
        private System.Windows.Forms.TextBox textBoxResults;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.ComboBox comboBoxInsert;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox checkBoxUnescape;
    }
}
