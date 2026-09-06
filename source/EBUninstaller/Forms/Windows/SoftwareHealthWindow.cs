/*
    EBUninstaller Pro - Software Health & System Hygiene Advisor Window
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions;
using UninstallTools;
using UninstallTools.Core;
using UninstallTools.Detection;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class SoftwareHealthWindow : Form
    {
        private readonly IEnumerable<ApplicationUninstallerEntry> _installedApps;
        private Label _lblScore;
        private Label _lblSummary;
        private ListView _lvRecommendations;
        private Button _btnAnalyze;
        private Button _btnExportReport;
        private Button _btnClose;
        private SystemHygieneReport _currentReport;

        public SoftwareHealthWindow(IEnumerable<ApplicationUninstallerEntry> installedApps = null)
        {
            _installedApps = installedApps;
            InitializeComponent();
            ThemeEngine.ApplyThemeToForm(this);
            RunHealthAnalysis();
        }

        private void InitializeComponent()
        {
            Text = "EBUninstaller Pro - Software Health & System Hygiene Advisor";
            Size = new Size(820, 560);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            MinimumSize = new Size(650, 450);

            var mainTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(12)
            };
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 90)); // Score Header Panel
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 35)); // Section label
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Recommendations List
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 45)); // Action Buttons

            // 1. Score Header Panel
            var headerPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(35, 35, 38), Padding = new Padding(10) };
            _lblScore = new Label
            {
                Text = "Hygiene Score: 100/100 (Optimal)",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.LightGreen,
                AutoSize = true,
                Location = new Point(10, 10)
            };
            _lblSummary = new Label
            {
                Text = "Analyzing software health, duplicate runtimes, and storage clutter...",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.LightGray,
                AutoSize = true,
                Location = new Point(12, 45)
            };
            headerPanel.Controls.Add(_lblScore);
            headerPanel.Controls.Add(_lblSummary);
            mainTable.Controls.Add(headerPanel, 0, 0);

            // 2. Section Label
            var lblSection = new Label
            {
                Text = "System Recommendations & Optimization Opportunities:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft
            };
            mainTable.Controls.Add(lblSection, 0, 1);

            // 3. ListView
            _lvRecommendations = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            _lvRecommendations.Columns.Add("Severity", 90);
            _lvRecommendations.Columns.Add("Category", 110);
            _lvRecommendations.Columns.Add("Recommendation", 260);
            _lvRecommendations.Columns.Add("Details", 300);
            mainTable.Controls.Add(_lvRecommendations, 0, 2);

            // 4. Action Buttons
            var btnFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 4, 0, 0)
            };

            _btnClose = new Button { Text = "Close", Size = new Size(100, 32), DialogResult = DialogResult.OK };
            _btnExportReport = new Button { Text = "Export Report (JSON)", Size = new Size(160, 32) };
            _btnExportReport.Click += BtnExportReport_Click;

            _btnAnalyze = new Button { Text = "Re-Analyze", Size = new Size(110, 32) };
            _btnAnalyze.Click += (s, e) => RunHealthAnalysis();

            btnFlow.Controls.Add(_btnClose);
            btnFlow.Controls.Add(_btnExportReport);
            btnFlow.Controls.Add(_btnAnalyze);
            mainTable.Controls.Add(btnFlow, 0, 3);

            Controls.Add(mainTable);
        }

        private void RunHealthAnalysis()
        {
            _lvRecommendations.Items.Clear();
            _currentReport = SoftwareHealthEngine.AnalyzeSystemHealth(_installedApps);

            _lblScore.Text = $"Hygiene Score: {_currentReport.HygieneScore}/100 ({GetScoreGrade(_currentReport.HygieneScore)})";
            _lblScore.ForeColor = _currentReport.HygieneScore >= 85 ? Color.LightGreen : (_currentReport.HygieneScore >= 65 ? Color.Gold : Color.LightCoral);

            _lblSummary.Text = $"Analyzed {_currentReport.TotalAppsAnalyzed} apps | {_currentReport.DuplicateRuntimesCount} redundant runtimes | {_currentReport.OrphanedFoldersCount} abandoned folders | {_currentReport.Recommendations.Count} recommendations";

            foreach (var rec in _currentReport.Recommendations)
            {
                var lvi = new ListViewItem(rec.Severity.ToString());
                lvi.SubItems.Add(rec.Category);
                lvi.SubItems.Add(rec.Title);
                lvi.SubItems.Add(rec.Description);
                lvi.Tag = rec;

                if (rec.Severity == HealthIssueSeverity.Medium || rec.Severity == HealthIssueSeverity.High)
                    lvi.ForeColor = Color.OrangeRed;

                _lvRecommendations.Items.Add(lvi);
            }
        }

        private static string GetScoreGrade(int score)
        {
            if (score >= 90) return "Excellent";
            if (score >= 80) return "Good";
            if (score >= 65) return "Fair";
            return "Attention Recommended";
        }

        private void BtnExportReport_Click(object sender, EventArgs e)
        {
            if (_currentReport == null) return;
            using var sfd = new SaveFileDialog { FileName = "EBUninstaller_HealthReport.json", Filter = "JSON File (*.json)|*.json" };
            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(_currentReport, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                    System.IO.File.WriteAllText(sfd.FileName, json);
                    MessageBox.Show("Health report exported successfully!", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to export report: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
