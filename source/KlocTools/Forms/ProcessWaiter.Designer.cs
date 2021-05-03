namespace Klocman.Forms
{
    partial class ProcessWaiter
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
            processWaiterControl1 = new ProcessWaiterControl();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessWaiter));
            this.SuspendLayout();
            // 
            // processWaiterControl1
            // 
            resources.ApplyResources(this.processWaiterControl1, "processWaiterControl1");
            this.processWaiterControl1.Name = "processWaiterControl1";
            this.processWaiterControl1.ShowIgnoreAndCancel = true;
            // 
            // ProcessWaiter
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.processWaiterControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProcessWaiter";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Shown += new System.EventHandler(this.ProcessWaiter_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private ProcessWaiterControl processWaiterControl1;
    }
}