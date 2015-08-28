using System;

namespace UninstallTools.Controls
{
    partial class FilterEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterEditor));
            this.labelText = new System.Windows.Forms.Label();
            this.textBoxFilterText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxCompareMethod = new System.Windows.Forms.ComboBox();
            this.searchBox1 = new Klocman.Controls.SearchBox();
            this.SuspendLayout();
            // 
            // labelText
            // 
            resources.ApplyResources(this.labelText, "labelText");
            this.labelText.Name = "labelText";
            // 
            // textBoxFilterText
            // 
            resources.ApplyResources(this.textBoxFilterText, "textBoxFilterText");
            this.textBoxFilterText.Name = "textBoxFilterText";
            this.textBoxFilterText.TextChanged += new System.EventHandler(this.textBoxFilterText_TextChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // comboBoxCompareMethod
            // 
            resources.ApplyResources(this.comboBoxCompareMethod, "comboBoxCompareMethod");
            this.comboBoxCompareMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCompareMethod.FormattingEnabled = true;
            this.comboBoxCompareMethod.Name = "comboBoxCompareMethod";
            this.comboBoxCompareMethod.SelectedIndexChanged += new System.EventHandler(this.comboBoxCompareMethod_SelectedIndexChanged);
            // 
            // searchBox1
            // 
            this.searchBox1.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.searchBox1, "searchBox1");
            this.searchBox1.Name = "searchBox1";
            this.searchBox1.SearchTextChanged += new System.Action<Klocman.Controls.SearchBox, System.EventArgs>(this.searchBox1_SearchTextChanged);
            // 
            // FilterEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.comboBoxCompareMethod);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxFilterText);
            this.Controls.Add(this.labelText);
            this.Controls.Add(this.searchBox1);
            this.Name = "FilterEditor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelText;
        private System.Windows.Forms.TextBox textBoxFilterText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxCompareMethod;
        private Klocman.Controls.SearchBox searchBox1;
    }
}
