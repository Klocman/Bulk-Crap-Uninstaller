namespace NBug.Core.UI.Developer
{
	partial class InternalLogViewer
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
			this.components = new System.ComponentModel.Container();
			this.detailsTextBox = new System.Windows.Forms.TextBox();
			this.detailsLabel = new System.Windows.Forms.Label();
			this.loggerListView = new System.Windows.Forms.ListView();
			this.categoryColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.timeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.messageColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.entriesLabel = new System.Windows.Forms.Label();
			this.quitButton = new System.Windows.Forms.Button();
			this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.hideButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// detailsTextBox
			// 
			this.detailsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.detailsTextBox.Location = new System.Drawing.Point(5, 261);
			this.detailsTextBox.Multiline = true;
			this.detailsTextBox.Name = "detailsTextBox";
			this.detailsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.detailsTextBox.Size = new System.Drawing.Size(526, 80);
			this.detailsTextBox.TabIndex = 7;
			// 
			// detailsLabel
			// 
			this.detailsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.detailsLabel.AutoSize = true;
			this.detailsLabel.Location = new System.Drawing.Point(5, 245);
			this.detailsLabel.Name = "detailsLabel";
			this.detailsLabel.Size = new System.Drawing.Size(42, 13);
			this.detailsLabel.TabIndex = 6;
			this.detailsLabel.Text = "Details:";
			// 
			// loggerListView
			// 
			this.loggerListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.loggerListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.categoryColumnHeader,
			this.timeColumnHeader,
			this.messageColumnHeader});
			this.loggerListView.FullRowSelect = true;
			this.loggerListView.Location = new System.Drawing.Point(5, 21);
			this.loggerListView.MultiSelect = false;
			this.loggerListView.Name = "loggerListView";
			this.loggerListView.ShowItemToolTips = true;
			this.loggerListView.Size = new System.Drawing.Size(526, 217);
			this.loggerListView.TabIndex = 4;
			this.loggerListView.UseCompatibleStateImageBehavior = false;
			this.loggerListView.View = System.Windows.Forms.View.Details;
			this.loggerListView.Click += new System.EventHandler(this.LoggerListView_Click);
			// 
			// categoryColumnHeader
			// 
			this.categoryColumnHeader.Text = "Category";
			// 
			// timeColumnHeader
			// 
			this.timeColumnHeader.Text = "Time";
			// 
			// messageColumnHeader
			// 
			this.messageColumnHeader.Text = "Message";
			this.messageColumnHeader.Width = 402;
			// 
			// entriesLabel
			// 
			this.entriesLabel.AutoSize = true;
			this.entriesLabel.Location = new System.Drawing.Point(5, 5);
			this.entriesLabel.Name = "entriesLabel";
			this.entriesLabel.Size = new System.Drawing.Size(42, 13);
			this.entriesLabel.TabIndex = 5;
			this.entriesLabel.Text = "Entries:";
			// 
			// quitButton
			// 
			this.quitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.quitButton.Location = new System.Drawing.Point(366, 347);
			this.quitButton.Name = "quitButton";
			this.quitButton.Size = new System.Drawing.Size(75, 23);
			this.quitButton.TabIndex = 8;
			this.quitButton.Text = "&Quit";
			this.quitButton.UseVisualStyleBackColor = true;
			this.quitButton.Click += new System.EventHandler(this.QuitButton_Click);
			// 
			// notifyIcon
			// 
			this.notifyIcon.BalloonTipText = "NBug Developer Interface";
			this.notifyIcon.BalloonTipTitle = "NBug (Debug Mode)";
			this.notifyIcon.Text = "NBug Developer Interface";
			this.notifyIcon.Visible = true;
			this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseDoubleClick);
			// 
			// hideButton
			// 
			this.hideButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.hideButton.Location = new System.Drawing.Point(456, 347);
			this.hideButton.Name = "hideButton";
			this.hideButton.Size = new System.Drawing.Size(75, 23);
			this.hideButton.TabIndex = 9;
			this.hideButton.Text = "&Hide";
			this.hideButton.UseVisualStyleBackColor = true;
			this.hideButton.Click += new System.EventHandler(this.HideButton_Click);
			// 
			// InternalLogViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(536, 373);
			this.Controls.Add(this.hideButton);
			this.Controls.Add(this.quitButton);
			this.Controls.Add(this.detailsTextBox);
			this.Controls.Add(this.detailsLabel);
			this.Controls.Add(this.loggerListView);
			this.Controls.Add(this.entriesLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "InternalLogViewer";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "NBug Internal Log Viewer";
			this.Resize += new System.EventHandler(this.InternalLogViewer_Resize);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox detailsTextBox;
		private System.Windows.Forms.Label detailsLabel;
		private System.Windows.Forms.ListView loggerListView;
		private System.Windows.Forms.ColumnHeader categoryColumnHeader;
		private System.Windows.Forms.ColumnHeader timeColumnHeader;
		private System.Windows.Forms.ColumnHeader messageColumnHeader;
		private System.Windows.Forms.Label entriesLabel;
		private System.Windows.Forms.Button quitButton;
		private System.Windows.Forms.NotifyIcon notifyIcon;
		private System.Windows.Forms.Button hideButton;
	}
}