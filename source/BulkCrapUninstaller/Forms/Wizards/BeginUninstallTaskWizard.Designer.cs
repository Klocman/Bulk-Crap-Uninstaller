using BulkCrapUninstaller.Controls;

namespace BulkCrapUninstaller.Forms
{
    partial class BeginUninstallTaskWizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BeginUninstallTaskWizard));
            this.panelNavigation = new System.Windows.Forms.Panel();
            this.labelProgress = new System.Windows.Forms.Label();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonPrev = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.buttonNext = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1 = new BulkCrapUninstaller.Controls.TabControlWithoutHeader();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.relatedUninstallerAdder1 = new BulkCrapUninstaller.Forms.RelatedUninstallerAdder();
            this.p1Heading = new System.Windows.Forms.Label();
            this.uninstallConfirmation1 = new BulkCrapUninstaller.Forms.UninstallConfirmation();
            this.label1 = new System.Windows.Forms.Label();
            this.panelNavigation.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelNavigation
            // 
            this.panelNavigation.BackColor = System.Drawing.SystemColors.Control;
            this.panelNavigation.Controls.Add(this.labelProgress);
            this.panelNavigation.Controls.Add(this.buttonExit);
            this.panelNavigation.Controls.Add(this.buttonPrev);
            this.panelNavigation.Controls.Add(this.panel4);
            this.panelNavigation.Controls.Add(this.buttonNext);
            resources.ApplyResources(this.panelNavigation, "panelNavigation");
            this.panelNavigation.Name = "panelNavigation";
            // 
            // labelProgress
            // 
            resources.ApplyResources(this.labelProgress, "labelProgress");
            this.labelProgress.ForeColor = System.Drawing.SystemColors.GrayText;
            this.labelProgress.Name = "labelProgress";
            // 
            // buttonExit
            // 
            resources.ApplyResources(this.buttonExit, "buttonExit");
            this.buttonExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.UseVisualStyleBackColor = true;
            // 
            // buttonPrev
            // 
            resources.ApplyResources(this.buttonPrev, "buttonPrev");
            this.buttonPrev.Name = "buttonPrev";
            this.buttonPrev.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // buttonNext
            // 
            resources.ApplyResources(this.buttonNext, "buttonNext");
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tabControl1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.TabStop = false;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.relatedUninstallerAdder1);
            this.tabPage1.Controls.Add(this.p1Heading);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.uninstallConfirmation1);
            this.tabPage2.Controls.Add(this.label1);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // relatedUninstallerAdder1
            // 
            resources.ApplyResources(this.relatedUninstallerAdder1, "relatedUninstallerAdder1");
            this.relatedUninstallerAdder1.Name = "relatedUninstallerAdder1";
            // 
            // p1Heading
            // 
            resources.ApplyResources(this.p1Heading, "p1Heading");
            this.p1Heading.Name = "p1Heading";
            // 
            // uninstallConfirmation1
            // 
            resources.ApplyResources(this.uninstallConfirmation1, "uninstallConfirmation1");
            this.uninstallConfirmation1.Name = "uninstallConfirmation1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // BeginUninstallTaskWizard
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelNavigation);
            this.Name = "BeginUninstallTaskWizard";
            this.panelNavigation.ResumeLayout(false);
            this.panelNavigation.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panelNavigation;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonPrev;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel1;
        private TabControlWithoutHeader tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private RelatedUninstallerAdder relatedUninstallerAdder1;
        private System.Windows.Forms.Label p1Heading;
        private UninstallConfirmation uninstallConfirmation1;
        private System.Windows.Forms.Label label1;
    }
}