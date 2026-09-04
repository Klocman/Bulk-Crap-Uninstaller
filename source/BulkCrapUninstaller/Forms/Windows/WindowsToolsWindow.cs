/*
    OpenUninstall Pro - Professional Next-Generation Windows Uninstaller
    Windows Built-in Tools Center Window
*/

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class WindowsToolsWindow : Form
    {
        private FastObjectListView _folvTools;
        private TextBox _txtSearch;
        private Button _btnLaunch;
        private Button _btnClose;
        private Label _lblStatus;

        public WindowsToolsWindow()
        {
            InitializeComponent();
            LoadTools();
        }

        private void InitializeComponent()
        {
            Text = "OpenUninstall Pro - Windows Administrative Tools Hub";
            Size = new Size(850, 520);
            MinimumSize = new Size(600, 400);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(12)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Search bar
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Tools list
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Status
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons

            // Search bar
            var searchLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Margin = new Padding(0, 0, 0, 8) };
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var lblSearch = new Label { Text = "Search Tools:", AutoSize = true, Margin = new Padding(0, 5, 8, 0) };
            _txtSearch = new TextBox { Dock = DockStyle.Fill };
            _txtSearch.TextChanged += (s, e) => FilterTools();

            searchLayout.Controls.Add(lblSearch, 0, 0);
            searchLayout.Controls.Add(_txtSearch, 1, 0);
            mainLayout.Controls.Add(searchLayout, 0, 0);

            // Tools ObjectListView
            _folvTools = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                ShowGroups = true,
                GridLines = true
            };
            var colName = new OLVColumn("Tool Name", nameof(WindowsToolItem.Name)) { Width = 200 };
            var colCat = new OLVColumn("Category", nameof(WindowsToolItem.Category)) { Width = 140 };
            var colDesc = new OLVColumn("Description", nameof(WindowsToolItem.Description)) { Width = 350, FillsFreeSpace = true };
            var colAdmin = new OLVColumn("Admin Required", nameof(WindowsToolItem.RequiresAdmin)) { Width = 110, AspectToStringConverter = v => (bool)v ? "Yes" : "No" };

            _folvTools.AllColumns.AddRange(new[] { colName, colCat, colDesc, colAdmin });
            _folvTools.RebuildColumns();
            _folvTools.ItemActivate += (s, e) => LaunchSelectedTool();
            _folvTools.SelectionChanged += (s, e) => _btnLaunch.Enabled = _folvTools.SelectedObject != null;

            mainLayout.Controls.Add(_folvTools, 0, 1);

            // Status label
            _lblStatus = new Label { Text = "Double-click any tool or select and click 'Launch Tool'.", AutoSize = true, Margin = new Padding(0, 4, 0, 4) };
            mainLayout.Controls.Add(_lblStatus, 0, 2);

            // Buttons
            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, AutoSize = true };
            _btnClose = new Button { Text = "Close", DialogResult = DialogResult.Cancel, AutoSize = true };
            _btnLaunch = new Button { Text = "Launch Tool", AutoSize = true, Enabled = false, Font = new Font(Font, FontStyle.Bold), ForeColor = Color.DarkBlue };
            _btnLaunch.Click += (s, e) => LaunchSelectedTool();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnLaunch);
            mainLayout.Controls.Add(btnPanel, 0, 3);

            Controls.Add(mainLayout);
        }

        private void LoadTools()
        {
            var tools = WindowsToolsLauncher.GetAvailableTools();
            _folvTools.SetObjects(tools);
        }

        private void FilterTools()
        {
            var search = _txtSearch.Text?.Trim();
            var tools = WindowsToolsLauncher.GetAvailableTools();
            if (!string.IsNullOrEmpty(search))
            {
                tools = tools.Where(t => t.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                         t.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                         t.Category.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            _folvTools.SetObjects(tools);
        }

        private void LaunchSelectedTool()
        {
            var selected = _folvTools.SelectedObject as WindowsToolItem;
            if (selected == null) return;

            if (WindowsToolsLauncher.LaunchTool(selected))
            {
                _lblStatus.Text = $"Launched {selected.Name} successfully.";
            }
            else
            {
                _lblStatus.Text = $"Failed to launch {selected.Name}.";
            }
        }
    }
}
