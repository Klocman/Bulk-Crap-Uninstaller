namespace SimpleTreeMapTests
{
    partial class Form1
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
            this.treeMap1 = new SimpleTreeMap.TreeMap();
            this.SuspendLayout();
            // 
            // treeMap1
            // 
            this.treeMap1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeMap1.Location = new System.Drawing.Point(33, 33);
            this.treeMap1.Name = "treeMap1";
            this.treeMap1.Size = new System.Drawing.Size(403, 239);
            this.treeMap1.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 305);
            this.Controls.Add(this.treeMap1);
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(33);
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion
        private SimpleTreeMap.TreeMap treeMap1;
    }
}

