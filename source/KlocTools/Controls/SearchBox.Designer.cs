using System.ComponentModel;
using System.Windows.Forms;

namespace Klocman.Controls
{
    partial class SearchBox
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchBox));
            this.filteringTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // filteringTextBox
            // 
            resources.ApplyResources(this.filteringTextBox, "filteringTextBox");
            this.filteringTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.filteringTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.filteringTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.filteringTextBox.ForeColor = System.Drawing.SystemColors.GrayText;
            this.filteringTextBox.Name = "filteringTextBox";
            this.filteringTextBox.TextChanged += new System.EventHandler(this.filteringTextBox_TextChanged);
            this.filteringTextBox.Enter += new System.EventHandler(this.filteringTextBox_Enter);
            this.filteringTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.filteringTextBox_KeyDown);
            this.filteringTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.filteringTextBox_KeyUp);
            this.filteringTextBox.Leave += new System.EventHandler(this.filteringTextBox_Leave);
            // 
            // SearchBox
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.filteringTextBox);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.MinimumSize = new System.Drawing.Size(50, 0);
            this.Name = "SearchBox";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox filteringTextBox;
    }
}
