/*
    EBUninstaller Pro - Software Safety & Bloatware Advisor Window
    Modern GUI for identifying bloatware, trialware, and reviewing uninstallation recommendations.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using UninstallTools;
using UninstallTools.Core;
using UninstallTools.Localization;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class SoftwareAdvisorWindow : Form
    {
        private ListView _listView = null!;
        private ToolStrip _toolStrip = null!;
        private ToolStripButton _selectBloatwareBtn = null!;
        private ToolStripComboBox _filterBox = null!;
        private Panel _summaryPanel = null!;
        private Label _bloatwareCountLabel = null!;
        private Label _avgScoreLabel = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        private readonly List<ApplicationUninstallerEntry> _allApps;
        private List<SoftwareAdviceReport> _reports = new();

        public SoftwareAdvisorWindow(IEnumerable<ApplicationUninstallerEntry> applications)
        {
            _allApps = applications.ToList();
            InitializeComponent();
            ApplyTheme();
            LoadData();
        }

        private void InitializeComponent()
        {
            Text = LanguageManager.GetString("SoftwareAdvisor_Title") ?? "Software Safety & Bloatware Advisor - EBUninstaller Pro";
            Size = new Size(1050, 650);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(850, 500);

            // Summary Panel
            _summaryPanel = new Panel { Dock = DockStyle.Top, Height = 65, Padding = new Padding(15, 10, 15, 10) };
            _bloatwareCountLabel = new Label
            {
                Text = "Bloatware Detected: 0",
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                Location = new Point(15, 12),
                AutoSize = true,
                ForeColor = Color.FromArgb(190, 40, 40)
            };
            _avgScoreLabel = new Label
            {
                Text = "Average Software Safety Score: 100%",
                Font = new Font("Segoe UI", 11f),
                Location = new Point(15, 36),
                AutoSize = true
            };
            _summaryPanel.Controls.Add(_bloatwareCountLabel);
            _summaryPanel.Controls.Add(_avgScoreLabel);

            // ToolStrip
            _toolStrip = new ToolStrip { ImageList = null, RenderMode = ToolStripRenderMode.System, GripStyle = ToolStripGripStyle.Hidden };
            _selectBloatwareBtn = new ToolStripButton("🎯 Select All Bloatware", null, (s, e) => SelectBloatwareItems());

            _filterBox = new ToolStripComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            _filterBox.Items.AddRange(new object[] { "All Applications", "Bloatware & PUPs Only", "Damaged / Orphaned Only", "Large Footprint", "Verified Clean" });
            _filterBox.SelectedIndex = 0;
            _filterBox.SelectedIndexChanged += (s, e) => ApplyFilter();

            _toolStrip.Items.Add(_selectBloatwareBtn);
            _toolStrip.Items.Add(new ToolStripSeparator());
            _toolStrip.Items.Add(new ToolStripLabel("View: "));
            _toolStrip.Items.Add(_filterBox);

            // ListView
            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                CheckBoxes = true,
                FullRowSelect = true,
                GridLines = true
            };
            _listView.Columns.Add("Application Name", 260);
            _listView.Columns.Add("Category Rating", 140);
            _listView.Columns.Add("Safety Score", 100);
            _listView.Columns.Add("Recommendation", 160);
            _listView.Columns.Add("Publisher", 180);
            _listView.Columns.Add("Analysis & Reason", 380);

            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
            _statusStrip.Items.Add(_statusLabel);

            Controls.Add(_listView);
            Controls.Add(_toolStrip);
            Controls.Add(_summaryPanel);
            Controls.Add(_statusStrip);
        }

        private void LoadData()
        {
            _reports = SoftwareSafetyAdvisor.AnalyzeAllApplications(_allApps);

            int bloatwareCount = _reports.Count(r => r.IsBloatware);
            double avgScore = _reports.Count > 0 ? _reports.Average(r => r.SafetyScore) : 100;

            _bloatwareCountLabel.Text = $"Bloatware / PUPs Detected: {bloatwareCount}";
            _avgScoreLabel.Text = $"Average Software Hygiene & Safety Score: {avgScore:F1}%";

            ApplyFilter();
            _statusLabel.Text = $"Analyzed {_reports.Count} applications. Found {bloatwareCount} bloatware/trialware items.";
        }

        private void ApplyFilter()
        {
            _listView.Items.Clear();
            string selectedFilter = _filterBox.SelectedItem?.ToString() ?? "All Applications";

            var filtered = _reports.Where(r =>
            {
                if (selectedFilter == "Bloatware & PUPs Only") return r.IsBloatware;
                if (selectedFilter == "Damaged / Orphaned Only") return r.Category == SoftwareCategoryRating.StubbornOrDamaged;
                if (selectedFilter == "Large Footprint") return r.Category == SoftwareCategoryRating.LargeFootprint;
                if (selectedFilter == "Verified Clean") return r.Category == SoftwareCategoryRating.VerifiedClean;
                return true;
            }).ToList();

            foreach (var r in filtered)
            {
                var lvi = new ListViewItem(r.ApplicationName) { Tag = r, Checked = r.IsBloatware };
                lvi.SubItems.Add(FormatCategory(r.Category));
                lvi.SubItems.Add($"{r.SafetyScore}/100");
                lvi.SubItems.Add(r.Recommendation.ToString());
                lvi.SubItems.Add(r.Publisher);
                lvi.SubItems.Add(r.Reason);

                if (r.IsBloatware)
                {
                    lvi.ForeColor = Color.FromArgb(190, 40, 40);
                }
                else if (r.Category == SoftwareCategoryRating.VerifiedClean)
                {
                    lvi.ForeColor = Color.FromArgb(0, 120, 60);
                }
                else if (r.Category == SoftwareCategoryRating.StubbornOrDamaged)
                {
                    lvi.ForeColor = Color.FromArgb(180, 100, 0);
                }

                _listView.Items.Add(lvi);
            }
        }

        private void SelectBloatwareItems()
        {
            foreach (ListViewItem lvi in _listView.Items)
            {
                if (lvi.Tag is SoftwareAdviceReport report)
                {
                    lvi.Checked = report.IsBloatware;
                }
            }
        }

        private static string FormatCategory(SoftwareCategoryRating cat)
        {
            return cat switch
            {
                SoftwareCategoryRating.OEMBloatware => "OEM Bloatware",
                SoftwareCategoryRating.AdwareOrPup => "Adware / PUP",
                SoftwareCategoryRating.StubbornOrDamaged => "Damaged Uninstaller",
                SoftwareCategoryRating.LargeFootprint => "Large Footprint",
                SoftwareCategoryRating.VerifiedClean => "Verified Clean",
                _ => "Standard"
            };
        }

        private void ApplyTheme()
        {
            bool isDark = ThemeManager.IsDarkModeEnabled;
            BackColor = isDark ? Color.FromArgb(32, 32, 32) : Color.FromArgb(245, 245, 245);
            ForeColor = isDark ? Color.White : Color.Black;
            _summaryPanel.BackColor = isDark ? Color.FromArgb(40, 40, 40) : Color.FromArgb(235, 238, 245);

            if (LanguageManager.IsRightToLeft)
            {
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
            }
        }
    }
}
