/*
    EBUninstaller Pro - Professional Next-Generation Windows Uninstaller
    Browser Extension Manager Window
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.BrowserExtensions;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class BrowserExtensionsWindow : Form
    {
        private ComboBox _cmbBrowserFilter;
        private FastObjectListView _folvExtensions;
        private TextBox _txtDetails;
        private Button _btnOpenFolder;
        private Button _btnRemove;
        private Button _btnRefresh;
        private Button _btnClose;
        private Label _lblStatus;
        private List<BrowserExtensionEntry> _extensions = new();

        public BrowserExtensionsWindow()
        {
            InitializeComponent();
            RefreshExtensions();
        }

        private void InitializeComponent()
        {
            Text = "EBUninstaller Pro - Browser Extensions Manager";
            Size = new Size(950, 600);
            MinimumSize = new Size(700, 480);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(12)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Filter bar
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Split
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Status
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons

            // Filter bar
            var filterLayout = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, Margin = new Padding(0, 0, 0, 8) };
            var lblFilter = new Label { Text = "Filter Browser:", AutoSize = true, Margin = new Padding(0, 6, 8, 0) };
            _cmbBrowserFilter = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 180 };
            _cmbBrowserFilter.Items.AddRange(new object[] { "All Browsers", "Google Chrome", "Microsoft Edge", "Mozilla Firefox", "Brave Browser", "Opera", "Vivaldi" });
            _cmbBrowserFilter.SelectedIndex = 0;
            _cmbBrowserFilter.SelectedIndexChanged += (s, e) => ApplyFilter();

            filterLayout.Controls.Add(lblFilter);
            filterLayout.Controls.Add(_cmbBrowserFilter);
            mainLayout.Controls.Add(filterLayout, 0, 0);

            // Split Container
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 320
            };

            _folvExtensions = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                ShowGroups = true,
                GridLines = true
            };
            var colBrowser = new OLVColumn("Browser", nameof(BrowserExtensionEntry.BrowserName)) { Width = 130 };
            var colName = new OLVColumn("Extension Name", nameof(BrowserExtensionEntry.Name)) { Width = 260, FillsFreeSpace = true };
            var colVer = new OLVColumn("Version", nameof(BrowserExtensionEntry.Version)) { Width = 90 };
            var colPub = new OLVColumn("Publisher", nameof(BrowserExtensionEntry.Publisher)) { Width = 150 };
            var colId = new OLVColumn("Extension ID", nameof(BrowserExtensionEntry.ExtensionId)) { Width = 220 };

            _folvExtensions.AllColumns.AddRange(new[] { colBrowser, colName, colVer, colPub, colId });
            _folvExtensions.RebuildColumns();
            _folvExtensions.SelectionChanged += (s, e) => OnExtensionSelected();
            split.Panel1.Controls.Add(_folvExtensions);

            _txtDetails = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 8.5F, FontStyle.Regular, GraphicsUnit.Point)
            };
            split.Panel2.Controls.Add(_txtDetails);

            mainLayout.Controls.Add(split, 0, 1);

            // Status label
            _lblStatus = new Label { Text = "Loading extensions...", AutoSize = true, Margin = new Padding(0, 4, 0, 4) };
            mainLayout.Controls.Add(_lblStatus, 0, 2);

            // Action Buttons
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true
            };

            _btnClose = new Button { Text = "Close", DialogResult = DialogResult.Cancel, AutoSize = true };
            _btnRemove = new Button { Text = "Remove Extension", AutoSize = true, Enabled = false, ForeColor = Color.DarkRed, Font = new Font(Font, FontStyle.Bold) };
            _btnRemove.Click += (s, e) => RemoveSelectedExtension();

            _btnOpenFolder = new Button { Text = "Open Folder", AutoSize = true, Enabled = false };
            _btnOpenFolder.Click += (s, e) => OpenExtensionFolder();

            _btnRefresh = new Button { Text = "Refresh", AutoSize = true };
            _btnRefresh.Click += (s, e) => RefreshExtensions();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRemove);
            btnPanel.Controls.Add(_btnOpenFolder);
            btnPanel.Controls.Add(_btnRefresh);
            mainLayout.Controls.Add(btnPanel, 0, 3);

            Controls.Add(mainLayout);
        }

        private async void RefreshExtensions()
        {
            _btnRefresh.Enabled = false;
            _lblStatus.Text = "Scanning browser extensions...";

            try
            {
                _extensions = await BrowserExtensionManager.GetInstalledExtensionsAsync();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed loading extensions: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _btnRefresh.Enabled = true;
            }
        }

        private void ApplyFilter()
        {
            var filterText = _cmbBrowserFilter.SelectedItem?.ToString();
            var filtered = _extensions.AsEnumerable();

            if (!string.IsNullOrEmpty(filterText) && !filterText.StartsWith("All"))
            {
                filtered = filtered.Where(e => e.BrowserName.Contains(filterText.Replace(" ", "").Replace("Browser", ""), StringComparison.OrdinalIgnoreCase) ||
                                              filterText.Contains(e.BrowserName, StringComparison.OrdinalIgnoreCase));
            }

            var list = filtered.ToList();
            _folvExtensions.SetObjects(list);
            _lblStatus.Text = $"Showing {list.Count} browser extensions ({_extensions.Count} total found).";
            _txtDetails.Text = string.Empty;
            UpdateButtons(false);
        }

        private void OnExtensionSelected()
        {
            var selected = _folvExtensions.SelectedObject as BrowserExtensionEntry;
            if (selected == null)
            {
                _txtDetails.Text = string.Empty;
                UpdateButtons(false);
                return;
            }

            UpdateButtons(true);
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"EXTENSION: {selected.Name} (v{selected.Version})");
            sb.AppendLine($"Browser: {selected.BrowserName}");
            sb.AppendLine($"Extension ID: {selected.ExtensionId}");
            sb.AppendLine($"Publisher: {selected.Publisher}");
            sb.AppendLine($"Install Path: {selected.InstallPath}");
            if (!string.IsNullOrEmpty(selected.ManifestPath))
                sb.AppendLine($"Manifest Path: {selected.ManifestPath}");
            sb.AppendLine();
            sb.AppendLine($"DESCRIPTION:");
            sb.AppendLine(selected.Description ?? "No description provided.");
            sb.AppendLine();
            sb.AppendLine($"PERMISSIONS ({selected.Permissions.Count}):");
            foreach (var p in selected.Permissions)
                sb.AppendLine($" - {p}");

            _txtDetails.Text = sb.ToString();
        }

        private void UpdateButtons(bool hasSelection)
        {
            _btnOpenFolder.Enabled = hasSelection;
            _btnRemove.Enabled = hasSelection;
        }

        private void OpenExtensionFolder()
        {
            var selected = _folvExtensions.SelectedObject as BrowserExtensionEntry;
            if (selected == null || string.IsNullOrEmpty(selected.InstallPath) || !Directory.Exists(selected.InstallPath)) return;

            try
            {
                Process.Start(new ProcessStartInfo { FileName = "explorer.exe", Arguments = $"\"{selected.InstallPath}\"", UseShellExecute = true });
            }
            catch { }
        }

        private void RemoveSelectedExtension()
        {
            var selected = _folvExtensions.SelectedObject as BrowserExtensionEntry;
            if (selected == null) return;

            if (MessageBox.Show($"Remove browser extension '{selected.Name}' from {selected.BrowserName}?\n\nThis will delete the extension files.",
                "Confirm Extension Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            if (BrowserExtensionManager.RemoveExtension(selected))
            {
                MessageBox.Show("Extension removed successfully.", "Removal Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshExtensions();
            }
            else
            {
                MessageBox.Show("Failed to remove extension. Please ensure browser is closed.", "Removal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
