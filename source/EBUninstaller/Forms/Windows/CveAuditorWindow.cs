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
    public class CveAuditorWindow : Form
    {
        private readonly List<ApplicationUninstallerEntry> _apps;
        private ListView _listView;
        private Label _lblSummary;
        private Button _btnClose;

        public CveAuditorWindow(IEnumerable<ApplicationUninstallerEntry> apps)
        {
            _apps = apps?.ToList() ?? new List<ApplicationUninstallerEntry>();
            InitializeComponent();
            AuditCve();
        }

        private void InitializeComponent()
        {
            Text = "Software Vulnerability (CVE) Intelligence Auditor - EBUninstaller Pro";
            Size = new Size(950, 540);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(780, 420);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(12) };

            _lblSummary = new Label
            {
                Text = "Auditing installed applications against the offline CVE security database...",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular)
            };
            topPanel.Controls.Add(_lblSummary);

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };

            _listView.Columns.Add("CVE Identifier", 140);
            _listView.Columns.Add("Affected Software", 160);
            _listView.Columns.Add("Severity", 100);
            _listView.Columns.Add("CVSS Score", 90);
            _listView.Columns.Add("Vulnerability Summary & Remediation", 420);

            _btnClose = new Button { Text = "Close", Width = 90, Dock = DockStyle.Right, DialogResult = DialogResult.OK };
            bottomPanel.Controls.Add(_btnClose);

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(bottomPanel);
        }

        private void AuditCve()
        {
            _listView.Items.Clear();
            var findings = CveDatabaseAuditor.AuditApplications(_apps);

            foreach (var f in findings)
            {
                var lvi = new ListViewItem(f.CveId);
                lvi.SubItems.Add(f.AffectedSoftware);
                lvi.SubItems.Add(f.Severity);
                lvi.SubItems.Add(f.CvssScore.ToString("F1"));
                lvi.SubItems.Add(f.Summary + " (" + f.Remediation + ")");

                if (f.Severity == "Critical")
                {
                    lvi.BackColor = Color.FromArgb(255, 235, 235);
                    lvi.ForeColor = Color.DarkRed;
                }
                else if (f.Severity == "High")
                {
                    lvi.BackColor = Color.FromArgb(255, 248, 225);
                }

                _listView.Items.Add(lvi);
            }

            _lblSummary.Text = "Security audit complete: " + findings.Count + " potential CVE vulnerability matches identified.";
        }
    }
}
