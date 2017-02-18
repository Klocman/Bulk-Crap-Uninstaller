namespace NBug.Core.UI.Developer
{
	using NBug.Core.UI.WinForms.Panels;

	partial class InternalExceptionViewer
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
			this.quitButton = new System.Windows.Forms.Button();
			this.debugButton = new System.Windows.Forms.Button();
			this.bugReportButton = new System.Windows.Forms.Button();
			this.messageLabel = new System.Windows.Forms.Label();
			this.topToolStrip = new System.Windows.Forms.ToolStrip();
			this.documentationToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.forumToolStripLabel = new System.Windows.Forms.ToolStripLabel();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.trackerToolStripLabel = new System.Windows.Forms.ToolStripLabel();
			this.exceptionLabel = new System.Windows.Forms.Label();
			this.exceptionTextBox = new System.Windows.Forms.TextBox();
			this.invalidSettingLabel = new System.Windows.Forms.Label();
			this.invalidSettingTextBox = new System.Windows.Forms.TextBox();
			this.targetSiteTextBox = new System.Windows.Forms.TextBox();
			this.targetSiteLabel = new System.Windows.Forms.Label();
			this.exceptionMessageTextBox = new System.Windows.Forms.TextBox();
			this.exceptionStackGroupBox = new System.Windows.Forms.GroupBox();
			this.exceptionDetails = new NBug.Core.UI.WinForms.Panels.ExceptionDetails();
			this.warningPictureBox = new System.Windows.Forms.PictureBox();
			this.topToolStrip.SuspendLayout();
			this.exceptionStackGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.warningPictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// quitButton
			// 
            this.quitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.quitButton.Location = new System.Drawing.Point(455, 511);
			this.quitButton.Name = "quitButton";
			this.quitButton.Size = new System.Drawing.Size(75, 23);
			this.quitButton.TabIndex = 0;
			this.quitButton.Text = "&Quit";
			this.quitButton.UseVisualStyleBackColor = true;
			this.quitButton.Click += new System.EventHandler(this.QuitButton_Click);
			// 
			// debugButton
			// 
            this.debugButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.debugButton.Image = global::NBug.Properties.Resources.VS2010_16;
			this.debugButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.debugButton.Location = new System.Drawing.Point(364, 511);
			this.debugButton.Name = "debugButton";
			this.debugButton.Size = new System.Drawing.Size(85, 23);
			this.debugButton.TabIndex = 1;
			this.debugButton.Text = "  &Debug";
			this.debugButton.UseVisualStyleBackColor = true;
			this.debugButton.Click += new System.EventHandler(this.DebugButton_Click);
			// 
			// bugReportButton
			// 
            this.bugReportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bugReportButton.Enabled = false;
			this.bugReportButton.Image = global::NBug.Properties.Resources.Send;
			this.bugReportButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.bugReportButton.Location = new System.Drawing.Point(205, 511);
			this.bugReportButton.Name = "bugReportButton";
			this.bugReportButton.Size = new System.Drawing.Size(128, 23);
			this.bugReportButton.TabIndex = 2;
			this.bugReportButton.Text = "    &Send Bug Report";
			this.bugReportButton.UseVisualStyleBackColor = true;
			this.bugReportButton.Click += new System.EventHandler(this.BugReportButton_Click);
			// 
			// messageLabel
			// 
			this.messageLabel.AutoSize = true;
			this.messageLabel.Location = new System.Drawing.Point(56, 38);
			this.messageLabel.MaximumSize = new System.Drawing.Size(465, 39);
			this.messageLabel.MinimumSize = new System.Drawing.Size(465, 39);
			this.messageLabel.Name = "messageLabel";
			this.messageLabel.Size = new System.Drawing.Size(465, 39);
			this.messageLabel.TabIndex = 3;
			this.messageLabel.Text = "A configuration or runtime exception has occured.";
			// 
			// topToolStrip
			// 
			this.topToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.topToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.documentationToolStripButton,
            this.toolStripSeparator1,
            this.forumToolStripLabel,
            this.toolStripSeparator2,
            this.trackerToolStripLabel});
			this.topToolStrip.Location = new System.Drawing.Point(0, 0);
			this.topToolStrip.Name = "topToolStrip";
			this.topToolStrip.Size = new System.Drawing.Size(541, 25);
			this.topToolStrip.TabIndex = 5;
			// 
			// documentationToolStripButton
			// 
			this.documentationToolStripButton.Image = global::NBug.Properties.Resources.Help_16;
			this.documentationToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.documentationToolStripButton.Name = "documentationToolStripButton";
            this.documentationToolStripButton.Size = new System.Drawing.Size(148, 22);
			this.documentationToolStripButton.Tag = "http://www.nbusy.com/projects/nbug/documentation";
			this.documentationToolStripButton.Text = "Online &Documentation";
			this.documentationToolStripButton.Click += new System.EventHandler(this.DocumentationToolStripButton_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// forumToolStripLabel
			// 
			this.forumToolStripLabel.Image = global::NBug.Properties.Resources.Forum_16;
			this.forumToolStripLabel.IsLink = true;
			this.forumToolStripLabel.Name = "forumToolStripLabel";
            this.forumToolStripLabel.Size = new System.Drawing.Size(117, 22);
			this.forumToolStripLabel.Tag = "http://www.nbusy.com/forum/f11/";
			this.forumToolStripLabel.Text = "Discussion Forum";
			this.forumToolStripLabel.Click += new System.EventHandler(this.ForumToolStripLabel_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// trackerToolStripLabel
			// 
			this.trackerToolStripLabel.Image = global::NBug.Properties.Resources.Error_16;
			this.trackerToolStripLabel.IsLink = true;
			this.trackerToolStripLabel.Name = "trackerToolStripLabel";
            this.trackerToolStripLabel.Size = new System.Drawing.Size(86, 22);
			this.trackerToolStripLabel.Tag = "http://www.nbusy.com/tracker/projects/nbug";
			this.trackerToolStripLabel.Text = "Bug Tracker";
			this.trackerToolStripLabel.Click += new System.EventHandler(this.TrackerToolStripLabel_Click);
			// 
			// exceptionLabel
			// 
			this.exceptionLabel.AutoSize = true;
			this.exceptionLabel.Location = new System.Drawing.Point(11, 96);
			this.exceptionLabel.Name = "exceptionLabel";
			this.exceptionLabel.Size = new System.Drawing.Size(57, 13);
			this.exceptionLabel.TabIndex = 6;
			this.exceptionLabel.Text = "Exception:";
			// 
			// exceptionTextBox
			// 
            this.exceptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.exceptionTextBox.Location = new System.Drawing.Point(74, 93);
			this.exceptionTextBox.Name = "exceptionTextBox";
			this.exceptionTextBox.Size = new System.Drawing.Size(226, 20);
			this.exceptionTextBox.TabIndex = 7;
			// 
			// invalidSettingLabel
			// 
            this.invalidSettingLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.invalidSettingLabel.AutoSize = true;
			this.invalidSettingLabel.Enabled = false;
			this.invalidSettingLabel.Location = new System.Drawing.Point(320, 96);
			this.invalidSettingLabel.Name = "invalidSettingLabel";
			this.invalidSettingLabel.Size = new System.Drawing.Size(77, 13);
			this.invalidSettingLabel.TabIndex = 8;
			this.invalidSettingLabel.Text = "Invalid Setting:";
			// 
			// invalidSettingTextBox
			// 
            this.invalidSettingTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.invalidSettingTextBox.Enabled = false;
			this.invalidSettingTextBox.Location = new System.Drawing.Point(403, 93);
			this.invalidSettingTextBox.Name = "invalidSettingTextBox";
			this.invalidSettingTextBox.Size = new System.Drawing.Size(121, 20);
			this.invalidSettingTextBox.TabIndex = 9;
			// 
			// targetSiteTextBox
			// 
            this.targetSiteTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.targetSiteTextBox.Location = new System.Drawing.Point(74, 122);
			this.targetSiteTextBox.Name = "targetSiteTextBox";
			this.targetSiteTextBox.Size = new System.Drawing.Size(450, 20);
			this.targetSiteTextBox.TabIndex = 11;
			// 
			// targetSiteLabel
			// 
			this.targetSiteLabel.AutoSize = true;
			this.targetSiteLabel.Location = new System.Drawing.Point(10, 125);
			this.targetSiteLabel.Name = "targetSiteLabel";
			this.targetSiteLabel.Size = new System.Drawing.Size(62, 13);
			this.targetSiteLabel.TabIndex = 10;
			this.targetSiteLabel.Text = "Target Site:";
			// 
			// exceptionMessageTextBox
			// 
            this.exceptionMessageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.exceptionMessageTextBox.Location = new System.Drawing.Point(14, 151);
			this.exceptionMessageTextBox.Multiline = true;
			this.exceptionMessageTextBox.Name = "exceptionMessageTextBox";
			this.exceptionMessageTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.exceptionMessageTextBox.Size = new System.Drawing.Size(510, 35);
			this.exceptionMessageTextBox.TabIndex = 12;
			// 
			// exceptionStackGroupBox
			// 
            this.exceptionStackGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.exceptionStackGroupBox.Controls.Add(this.exceptionDetails);
			this.exceptionStackGroupBox.Location = new System.Drawing.Point(14, 198);
			this.exceptionStackGroupBox.Name = "exceptionStackGroupBox";
			this.exceptionStackGroupBox.Size = new System.Drawing.Size(516, 304);
			this.exceptionStackGroupBox.TabIndex = 13;
			this.exceptionStackGroupBox.TabStop = false;
			this.exceptionStackGroupBox.Text = "Full Exception Stack";
			// 
			// exceptionDetails
			// 
            this.exceptionDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.exceptionDetails.InformationColumnWidth = 350;
			this.exceptionDetails.Location = new System.Drawing.Point(6, 16);
			this.exceptionDetails.Name = "exceptionDetails";
			this.exceptionDetails.PropertyColumnWidth = 144;
			this.exceptionDetails.Size = new System.Drawing.Size(504, 282);
			this.exceptionDetails.TabIndex = 0;
			// 
			// warningPictureBox
			// 
			this.warningPictureBox.Location = new System.Drawing.Point(13, 41);
			this.warningPictureBox.Name = "warningPictureBox";
			this.warningPictureBox.Size = new System.Drawing.Size(32, 32);
			this.warningPictureBox.TabIndex = 14;
			this.warningPictureBox.TabStop = false;
			// 
			// InternalExceptionViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(541, 542);
			this.Controls.Add(this.quitButton);
			this.Controls.Add(this.bugReportButton);
			this.Controls.Add(this.warningPictureBox);
			this.Controls.Add(this.debugButton);
			this.Controls.Add(this.exceptionStackGroupBox);
			this.Controls.Add(this.exceptionMessageTextBox);
			this.Controls.Add(this.targetSiteTextBox);
			this.Controls.Add(this.messageLabel);
			this.Controls.Add(this.topToolStrip);
			this.Controls.Add(this.targetSiteLabel);
			this.Controls.Add(this.invalidSettingLabel);
			this.Controls.Add(this.exceptionLabel);
			this.Controls.Add(this.exceptionTextBox);
			this.Controls.Add(this.invalidSettingTextBox);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InternalExceptionViewer";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "NBug Internal Exception Viewer";
			this.TopMost = true;
			this.topToolStrip.ResumeLayout(false);
			this.topToolStrip.PerformLayout();
			this.exceptionStackGroupBox.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.warningPictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button quitButton;
		private System.Windows.Forms.Button debugButton;
		private System.Windows.Forms.Button bugReportButton;
		private System.Windows.Forms.Label messageLabel;
		private System.Windows.Forms.ToolStrip topToolStrip;
		private System.Windows.Forms.ToolStripButton documentationToolStripButton;
		private System.Windows.Forms.ToolStripLabel forumToolStripLabel;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripLabel trackerToolStripLabel;
		private System.Windows.Forms.Label exceptionLabel;
		private System.Windows.Forms.TextBox exceptionTextBox;
		private System.Windows.Forms.Label invalidSettingLabel;
		private System.Windows.Forms.TextBox invalidSettingTextBox;
		private System.Windows.Forms.TextBox targetSiteTextBox;
		private System.Windows.Forms.Label targetSiteLabel;
		private System.Windows.Forms.TextBox exceptionMessageTextBox;
		private System.Windows.Forms.GroupBox exceptionStackGroupBox;
		private ExceptionDetails exceptionDetails;
		private System.Windows.Forms.PictureBox warningPictureBox;
	}
}