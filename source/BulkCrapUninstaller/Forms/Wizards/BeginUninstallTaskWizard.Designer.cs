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
            this.relatedUninstallerAdder1 = new BulkCrapUninstaller.Forms.RelatedUninstallerAdder();
            this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            this.p1Heading = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.uninstallConfirmation1 = new BulkCrapUninstaller.Forms.UninstallConfirmation();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.processWaiterControl1 = new Klocman.Forms.ProcessWaiterControl();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.uninstallationSettings1 = new BulkCrapUninstaller.Controls.UninstallationSettings();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelOther = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.labelConcurrentEnabled = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.labelWillBeSilent = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.labelRestorePointCreated = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.labelFilesStillUsed = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.labelTotalSize = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.labelApps = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.panelNavigation.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.flowLayoutPanel5.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
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
            this.buttonPrev.Click += new System.EventHandler(this.buttonPrev_Click);
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
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.TabStop = false;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.relatedUninstallerAdder1);
            this.tabPage1.Controls.Add(this.flowLayoutPanel5);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // relatedUninstallerAdder1
            // 
            resources.ApplyResources(this.relatedUninstallerAdder1, "relatedUninstallerAdder1");
            this.relatedUninstallerAdder1.Name = "relatedUninstallerAdder1";
            // 
            // flowLayoutPanel5
            // 
            resources.ApplyResources(this.flowLayoutPanel5, "flowLayoutPanel5");
            this.flowLayoutPanel5.Controls.Add(this.p1Heading);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            // 
            // p1Heading
            // 
            resources.ApplyResources(this.p1Heading, "p1Heading");
            this.p1Heading.Name = "p1Heading";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.uninstallConfirmation1);
            this.tabPage2.Controls.Add(this.flowLayoutPanel4);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // uninstallConfirmation1
            // 
            resources.ApplyResources(this.uninstallConfirmation1, "uninstallConfirmation1");
            this.uninstallConfirmation1.Name = "uninstallConfirmation1";
            // 
            // flowLayoutPanel4
            // 
            resources.ApplyResources(this.flowLayoutPanel4, "flowLayoutPanel4");
            this.flowLayoutPanel4.Controls.Add(this.label1);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.processWaiterControl1);
            this.tabPage3.Controls.Add(this.flowLayoutPanel3);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // processWaiterControl1
            // 
            resources.ApplyResources(this.processWaiterControl1, "processWaiterControl1");
            this.processWaiterControl1.Name = "processWaiterControl1";
            this.processWaiterControl1.ShowIgnoreAndCancel = false;
            this.processWaiterControl1.AllProcessesClosed += new System.EventHandler(this.processWaiterControl1_AllProcessesClosed);
            // 
            // flowLayoutPanel3
            // 
            resources.ApplyResources(this.flowLayoutPanel3, "flowLayoutPanel3");
            this.flowLayoutPanel3.Controls.Add(this.label2);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // tabPage4
            // 
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Controls.Add(this.uninstallationSettings1);
            this.tabPage4.Controls.Add(this.flowLayoutPanel1);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // uninstallationSettings1
            // 
            resources.ApplyResources(this.uninstallationSettings1, "uninstallationSettings1");
            this.uninstallationSettings1.Name = "uninstallationSettings1";
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.label5);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.flowLayoutPanel1.SetFlowBreak(this.label5, true);
            this.label5.Name = "label5";
            // 
            // tabPage5
            // 
            resources.ApplyResources(this.tabPage5, "tabPage5");
            this.tabPage5.Controls.Add(this.tableLayoutPanel1);
            this.tabPage5.Controls.Add(this.button2);
            this.tabPage5.Controls.Add(this.flowLayoutPanel2);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelOther, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.label19, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.labelConcurrentEnabled, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.label17, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.labelWillBeSilent, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label15, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelRestorePointCreated, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label13, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelFilesStillUsed, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label11, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelTotalSize, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label9, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelApps, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelOther
            // 
            resources.ApplyResources(this.labelOther, "labelOther");
            this.labelOther.Name = "labelOther";
            // 
            // label19
            // 
            resources.ApplyResources(this.label19, "label19");
            this.label19.Name = "label19";
            // 
            // labelConcurrentEnabled
            // 
            resources.ApplyResources(this.labelConcurrentEnabled, "labelConcurrentEnabled");
            this.labelConcurrentEnabled.Name = "labelConcurrentEnabled";
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // labelWillBeSilent
            // 
            resources.ApplyResources(this.labelWillBeSilent, "labelWillBeSilent");
            this.labelWillBeSilent.Name = "labelWillBeSilent";
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // labelRestorePointCreated
            // 
            resources.ApplyResources(this.labelRestorePointCreated, "labelRestorePointCreated");
            this.labelRestorePointCreated.Name = "labelRestorePointCreated";
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // labelFilesStillUsed
            // 
            resources.ApplyResources(this.labelFilesStillUsed, "labelFilesStillUsed");
            this.labelFilesStillUsed.Name = "labelFilesStillUsed";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // labelTotalSize
            // 
            resources.ApplyResources(this.labelTotalSize, "labelTotalSize");
            this.labelTotalSize.Name = "labelTotalSize";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // labelApps
            // 
            this.labelApps.AutoEllipsis = true;
            resources.ApplyResources(this.labelApps, "labelApps");
            this.labelApps.Name = "labelApps";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // flowLayoutPanel2
            // 
            resources.ApplyResources(this.flowLayoutPanel2, "flowLayoutPanel2");
            this.flowLayoutPanel2.Controls.Add(this.label6);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.flowLayoutPanel2.SetFlowBreak(this.label6, true);
            this.label6.Name = "label6";
            // 
            // BeginUninstallTaskWizard
            // 
            this.AcceptButton = this.button2;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.CancelButton = this.buttonExit;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelNavigation);
            this.Name = "BeginUninstallTaskWizard";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BeginUninstallTaskWizard_FormClosed);
            this.panelNavigation.ResumeLayout(false);
            this.panelNavigation.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.flowLayoutPanel5.ResumeLayout(false);
            this.flowLayoutPanel5.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
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
        private UninstallConfirmation uninstallConfirmation1;
        private System.Windows.Forms.TabPage tabPage3;
        private Klocman.Forms.ProcessWaiterControl processWaiterControl1;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private UninstallationSettings uninstallationSettings1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelOther;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label labelConcurrentEnabled;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label labelWillBeSilent;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label labelRestorePointCreated;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label labelFilesStillUsed;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label labelTotalSize;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label labelApps;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel5;
        private System.Windows.Forms.Label p1Heading;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.Label label1;
    }
}