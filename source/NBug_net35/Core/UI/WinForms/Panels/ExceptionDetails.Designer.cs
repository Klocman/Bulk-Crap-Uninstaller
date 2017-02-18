namespace NBug.Core.UI.WinForms.Panels
{
	partial class ExceptionDetails
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
			this.components = new System.ComponentModel.Container();
			this.exceptionDetailsListView = new System.Windows.Forms.ListView();
			this.propertyColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.informationColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.exceptionDetailsLabel = new System.Windows.Forms.Label();
			this.exceptionLabel = new System.Windows.Forms.Label();
			this.exceptionTreeView = new System.Windows.Forms.TreeView();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// exceptionDetailsListView
			// 
			this.exceptionDetailsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.exceptionDetailsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.propertyColumnHeader,
            this.informationColumnHeader});
			this.exceptionDetailsListView.FullRowSelect = true;
			this.exceptionDetailsListView.GridLines = true;
			this.exceptionDetailsListView.Location = new System.Drawing.Point(3, 110);
			this.exceptionDetailsListView.MultiSelect = false;
			this.exceptionDetailsListView.Name = "exceptionDetailsListView";
			this.exceptionDetailsListView.Size = new System.Drawing.Size(455, 217);
			this.exceptionDetailsListView.TabIndex = 5;
			this.exceptionDetailsListView.UseCompatibleStateImageBehavior = false;
			this.exceptionDetailsListView.View = System.Windows.Forms.View.Details;
			this.exceptionDetailsListView.ItemMouseHover += new System.Windows.Forms.ListViewItemMouseHoverEventHandler(this.ExceptionDetailsListView_ItemMouseHover);
			this.exceptionDetailsListView.DoubleClick += new System.EventHandler(this.ExceptionDetailsListView_DoubleClick);
			// 
			// propertyColumnHeader
			// 
			this.propertyColumnHeader.Text = "Property";
			this.propertyColumnHeader.Width = 101;
			// 
			// informationColumnHeader
			// 
			this.informationColumnHeader.Text = "Information";
			this.informationColumnHeader.Width = 350;
			// 
			// exceptionDetailsLabel
			// 
			this.exceptionDetailsLabel.AutoSize = true;
			this.exceptionDetailsLabel.Location = new System.Drawing.Point(3, 92);
			this.exceptionDetailsLabel.Name = "exceptionDetailsLabel";
			this.exceptionDetailsLabel.Size = new System.Drawing.Size(265, 13);
			this.exceptionDetailsLabel.TabIndex = 7;
			this.exceptionDetailsLabel.Text = "Exception Details (double click on items to see details):";
			// 
			// exceptionLabel
			// 
			this.exceptionLabel.AutoSize = true;
			this.exceptionLabel.Location = new System.Drawing.Point(3, 4);
			this.exceptionLabel.Name = "exceptionLabel";
			this.exceptionLabel.Size = new System.Drawing.Size(68, 13);
			this.exceptionLabel.TabIndex = 6;
			this.exceptionLabel.Text = "Exception(s):";
			// 
			// exceptionTreeView
			// 
			this.exceptionTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.exceptionTreeView.Location = new System.Drawing.Point(3, 22);
			this.exceptionTreeView.Name = "exceptionTreeView";
			this.exceptionTreeView.Size = new System.Drawing.Size(455, 60);
			this.exceptionTreeView.TabIndex = 4;
			this.exceptionTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ExceptionTreeView_AfterSelect);
			// 
			// ExceptionDetails
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.exceptionDetailsListView);
			this.Controls.Add(this.exceptionDetailsLabel);
			this.Controls.Add(this.exceptionLabel);
			this.Controls.Add(this.exceptionTreeView);
			this.Name = "ExceptionDetails";
			this.Size = new System.Drawing.Size(461, 330);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView exceptionDetailsListView;
		private System.Windows.Forms.ColumnHeader propertyColumnHeader;
		private System.Windows.Forms.ColumnHeader informationColumnHeader;
		private System.Windows.Forms.Label exceptionDetailsLabel;
		private System.Windows.Forms.Label exceptionLabel;
		private System.Windows.Forms.TreeView exceptionTreeView;
		private System.Windows.Forms.ToolTip toolTip;
	}
}
