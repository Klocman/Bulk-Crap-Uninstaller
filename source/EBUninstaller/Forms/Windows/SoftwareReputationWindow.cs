/*
 * EBUninstaller Pro - Application Uninstaller & System Optimization Suite
 * Copyright (C) 2026 EBUninstaller Development Team & Contributors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using UninstallTools;
using UninstallTools.Detection;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class SoftwareReputationWindow : Form
    {
        private readonly List<ApplicationUninstallerEntry> _installedApps;
        private ListView _listView;
        private Label _lblSummary;
        private Button _btnRefresh;
        private Button _btnClose;
        private ComboBox _cboFilter;

        public SoftwareReputationWindow(IEnumerable<ApplicationUninstallerEntry> apps)
        {
            _installedApps = apps?.ToList() ?? new List<ApplicationUninstallerEntry>();
            InitializeComponent();
            LoadReputationData();
        }

        private void InitializeComponent()
        {
            Text = "Software Reputation & Safety Advisor - EBUninstaller Pro";
            Size = new Size(950, 580);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 480);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(12) };

            _lblSummary = new Label
            {
                Text = "Analyzing application provenance, vendor signatures, and bloatware risks...",
                Dock = DockStyle.Left,
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            _cboFilter = new ComboBox
            {
                Dock = DockStyle.Right,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 200
            };
            _cboFilter.Items.AddRange(new object[] { "All Applications", "Caution / High Risk Only", "Verified Trusted Only", "Bloatware / PUPs Only" });
            _cboFilter.SelectedIndex = 0;
            _cboFilter.SelectedIndexChanged += (s, e) => LoadReputationData();

            topPanel.Controls.Add(_lblSummary);
            topPanel.Controls.Add(_cboFilter);

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };

            _listView.Columns.Add("Application Name", 240);
            _listView.Columns.Add("Publisher", 180);
            _listView.Columns.Add("Reputation Tier", 130);
            _listView.Columns.Add("Score", 70);
            _listView.Columns.Add("Assessment & Recommendation", 300);

            _btnRefresh = new Button { Text = "Re-evaluate", Width = 110, Dock = DockStyle.Left };
            _btnRefresh.Click += (s, e) => LoadReputationData();

            _btnClose = new Button { Text = "Close", Width = 90, Dock = DockStyle.Right, DialogResult = DialogResult.OK };

            bottomPanel.Controls.Add(_btnRefresh);
            bottomPanel.Controls.Add(_btnClose);

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(bottomPanel);
        }

        private void LoadReputationData()
        {
            _listView.Items.Clear();

            var records = SoftwareReputationEngine.EvaluateBatch(_installedApps);

            var filter = _cboFilter.SelectedItem?.ToString() ?? "All Applications";
            if (filter == "Caution / High Risk Only")
                records = records.Where(r => r.Tier == ReputationTier.CautionAdvised || r.Tier == ReputationTier.HighRisk).ToList();
            else if (filter == "Verified Trusted Only")
                records = records.Where(r => r.Tier == ReputationTier.VerifiedTrusted).ToList();
            else if (filter == "Bloatware / PUPs Only")
                records = records.Where(r => r.IsKnownBloatware || r.IsBundledInstaller).ToList();

            foreach (var r in records)
            {
                var item = new ListViewItem(r.ApplicationName);
                item.SubItems.Add(r.Publisher);
                item.SubItems.Add(r.Tier.ToString());
                item.SubItems.Add($"{r.ReputationScore}/100");
                item.SubItems.Add($"{r.SafetyExplanation} - {r.Recommendation}");

                if (r.Tier == ReputationTier.HighRisk)
                {
                    item.BackColor = Color.FromArgb(255, 235, 235);
                    item.ForeColor = Color.DarkRed;
                }
                else if (r.Tier == ReputationTier.CautionAdvised)
                {
                    item.BackColor = Color.FromArgb(255, 248, 220);
                }
                else if (r.Tier == ReputationTier.VerifiedTrusted)
                {
                    item.BackColor = Color.FromArgb(240, 255, 240);
                }

                _listView.Items.Add(item);
            }

            int highRiskCount = records.Count(r => r.Tier == ReputationTier.HighRisk);
            int cautionCount = records.Count(r => r.Tier == ReputationTier.CautionAdvised);
            _lblSummary.Text = $"Evaluated {records.Count} apps: {highRiskCount} High Risk, {cautionCount} Caution Advised.";
        }
    }
}
