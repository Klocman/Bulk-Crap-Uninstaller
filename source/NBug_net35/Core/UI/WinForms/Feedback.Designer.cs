namespace NBug.Core.UI.WinForms
{
	partial class Feedback
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
			this.suggestionTypeGroupBox = new System.Windows.Forms.GroupBox();
			this.featureRequestRadioButton = new System.Windows.Forms.RadioButton();
			this.commentRadioButton = new System.Windows.Forms.RadioButton();
			this.errorReportRadioButton = new System.Windows.Forms.RadioButton();
			this.suggestionRadioButton = new System.Windows.Forms.RadioButton();
			this.feedbackLabel = new System.Windows.Forms.Label();
			this.feedbackPictureBox = new System.Windows.Forms.PictureBox();
			this.emailLabel = new System.Windows.Forms.Label();
			this.remarksLabel = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.sendButton = new System.Windows.Forms.Button();
			this.closeButton = new System.Windows.Forms.Button();
			this.suggestionTypeGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.feedbackPictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// suggestionTypeGroupBox
			// 
			this.suggestionTypeGroupBox.Controls.Add(this.featureRequestRadioButton);
			this.suggestionTypeGroupBox.Controls.Add(this.commentRadioButton);
			this.suggestionTypeGroupBox.Controls.Add(this.errorReportRadioButton);
			this.suggestionTypeGroupBox.Controls.Add(this.suggestionRadioButton);
			this.suggestionTypeGroupBox.Location = new System.Drawing.Point(12, 61);
			this.suggestionTypeGroupBox.Name = "suggestionTypeGroupBox";
			this.suggestionTypeGroupBox.Size = new System.Drawing.Size(382, 42);
			this.suggestionTypeGroupBox.TabIndex = 0;
			this.suggestionTypeGroupBox.TabStop = false;
			this.suggestionTypeGroupBox.Text = "Suggestion Type:";
			// 
			// featureRequestRadioButton
			// 
			this.featureRequestRadioButton.AutoSize = true;
			this.featureRequestRadioButton.Location = new System.Drawing.Point(263, 17);
			this.featureRequestRadioButton.Name = "featureRequestRadioButton";
			this.featureRequestRadioButton.Size = new System.Drawing.Size(104, 17);
			this.featureRequestRadioButton.TabIndex = 3;
			this.featureRequestRadioButton.TabStop = true;
			this.featureRequestRadioButton.Text = "Feature Request";
			this.featureRequestRadioButton.UseVisualStyleBackColor = true;
			// 
			// commentRadioButton
			// 
			this.commentRadioButton.AutoSize = true;
			this.commentRadioButton.Location = new System.Drawing.Point(100, 17);
			this.commentRadioButton.Name = "commentRadioButton";
			this.commentRadioButton.Size = new System.Drawing.Size(69, 17);
			this.commentRadioButton.TabIndex = 2;
			this.commentRadioButton.TabStop = true;
			this.commentRadioButton.Text = "Comment";
			this.commentRadioButton.UseVisualStyleBackColor = true;
			// 
			// errorReportRadioButton
			// 
			this.errorReportRadioButton.AutoSize = true;
			this.errorReportRadioButton.Location = new System.Drawing.Point(175, 17);
			this.errorReportRadioButton.Name = "errorReportRadioButton";
			this.errorReportRadioButton.Size = new System.Drawing.Size(82, 17);
			this.errorReportRadioButton.TabIndex = 1;
			this.errorReportRadioButton.TabStop = true;
			this.errorReportRadioButton.Text = "Error Report";
			this.errorReportRadioButton.UseVisualStyleBackColor = true;
			// 
			// suggestionRadioButton
			// 
			this.suggestionRadioButton.AutoSize = true;
			this.suggestionRadioButton.Location = new System.Drawing.Point(16, 17);
			this.suggestionRadioButton.Name = "suggestionRadioButton";
			this.suggestionRadioButton.Size = new System.Drawing.Size(78, 17);
			this.suggestionRadioButton.TabIndex = 0;
			this.suggestionRadioButton.TabStop = true;
			this.suggestionRadioButton.Text = "Suggestion";
			this.suggestionRadioButton.UseVisualStyleBackColor = true;
			// 
			// feedbackLabel
			// 
			this.feedbackLabel.Location = new System.Drawing.Point(50, 14);
			this.feedbackLabel.Name = "feedbackLabel";
			this.feedbackLabel.Size = new System.Drawing.Size(344, 28);
			this.feedbackLabel.TabIndex = 1;
			this.feedbackLabel.Text = "Please fill in the form below to submit your feedback. Selecting the proper feedb" +
    "ack type will help us better understand your opinion.";
			// 
			// feedbackPictureBox
			// 
			this.feedbackPictureBox.Image = global::NBug.Properties.Resources.Feedback;
			this.feedbackPictureBox.Location = new System.Drawing.Point(12, 12);
			this.feedbackPictureBox.Name = "feedbackPictureBox";
			this.feedbackPictureBox.Size = new System.Drawing.Size(32, 32);
			this.feedbackPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.feedbackPictureBox.TabIndex = 2;
			this.feedbackPictureBox.TabStop = false;
			// 
			// emailLabel
			// 
			this.emailLabel.AutoSize = true;
			this.emailLabel.Location = new System.Drawing.Point(9, 122);
			this.emailLabel.Name = "emailLabel";
			this.emailLabel.Size = new System.Drawing.Size(127, 13);
			this.emailLabel.TabIndex = 3;
			this.emailLabel.Text = "E-mail Address (Optional):";
			// 
			// remarksLabel
			// 
			this.remarksLabel.AutoSize = true;
			this.remarksLabel.Location = new System.Drawing.Point(9, 150);
			this.remarksLabel.Name = "remarksLabel";
			this.remarksLabel.Size = new System.Drawing.Size(52, 13);
			this.remarksLabel.TabIndex = 4;
			this.remarksLabel.Text = "Remarks:";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(142, 119);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(146, 20);
			this.textBox1.TabIndex = 5;
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(12, 166);
			this.textBox2.Multiline = true;
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(382, 110);
			this.textBox2.TabIndex = 6;
			// 
			// sendButton
			// 
			this.sendButton.Image = global::NBug.Properties.Resources.Send;
			this.sendButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.sendButton.Location = new System.Drawing.Point(319, 284);
			this.sendButton.Name = "sendButton";
			this.sendButton.Size = new System.Drawing.Size(75, 23);
			this.sendButton.TabIndex = 7;
			this.sendButton.Text = "    &Send";
			this.sendButton.UseVisualStyleBackColor = true;
			// 
			// closeButton
			// 
			this.closeButton.Location = new System.Drawing.Point(233, 284);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(75, 23);
			this.closeButton.TabIndex = 8;
			this.closeButton.Text = "&Close";
			this.closeButton.UseVisualStyleBackColor = true;
			// 
			// Feedback
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(406, 314);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.sendButton);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.remarksLabel);
			this.Controls.Add(this.emailLabel);
			this.Controls.Add(this.feedbackPictureBox);
			this.Controls.Add(this.feedbackLabel);
			this.Controls.Add(this.suggestionTypeGroupBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Feedback";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Feedback";
			this.suggestionTypeGroupBox.ResumeLayout(false);
			this.suggestionTypeGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.feedbackPictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox suggestionTypeGroupBox;
		private System.Windows.Forms.RadioButton featureRequestRadioButton;
		private System.Windows.Forms.RadioButton commentRadioButton;
		private System.Windows.Forms.RadioButton errorReportRadioButton;
		private System.Windows.Forms.RadioButton suggestionRadioButton;
		private System.Windows.Forms.Label feedbackLabel;
		private System.Windows.Forms.PictureBox feedbackPictureBox;
		private System.Windows.Forms.Label emailLabel;
		private System.Windows.Forms.Label remarksLabel;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Button sendButton;
		private System.Windows.Forms.Button closeButton;

	}
}