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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UninstallTools.Reporting;

namespace BulkCrapUninstaller.Forms
{
    public class SoftwareInventoryReportWindow : Form
    {
        private List<ReportSoftwareItem> _items;
        private WebBrowser _webBrowser;
        private Button _btnExportHtml;
        private Button _btnExportMd;
        private Button _btnExportCsv;
        private Button _btnExportJson;
        private Button _btnClose;
        private Label _lblSummary;

        public SoftwareInventoryReportWindow(IEnumerable<ReportSoftwareItem> items = null)
        {
            _items = items?.ToList() ?? new List<ReportSoftwareItem>();
            InitializeComponents();
            RenderPreview();
        }

        private void InitializeComponents()
        {
            Text = "EBUninstaller Pro - Software Inventory & Audit Report";
            Size = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 500);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(12, 10, 12, 10) };
            _lblSummary = new Label { Dock = DockStyle.Left, AutoSize = true, Text = $"Total Applications: {_items.Count}", Location = new Point(12, 14) };

            var rightActions = new FlowLayoutPanel { Dock = DockStyle.Right, AutoSize = true, FlowDirection = FlowDirection.LeftToRight };
            _btnExportHtml = new Button { Text = "Export HTML...", Width = 110, Height = 28 };
            _btnExportHtml.Click += (s, e) => ExportReport("HTML Files (*.html)|*.html", "html");

            _btnExportMd = new Button { Text = "Export Markdown...", Width = 125, Height = 28 };
            _btnExportMd.Click += (s, e) => ExportReport("Markdown Files (*.md)|*.md", "md");

            _btnExportCsv = new Button { Text = "Export CSV...", Width = 100, Height = 28 };
            _btnExportCsv.Click += (s, e) => ExportReport("CSV Files (*.csv)|*.csv", "csv");

            _btnExportJson = new Button { Text = "Export JSON...", Width = 105, Height = 28 };
            _btnExportJson.Click += (s, e) => ExportReport("JSON Files (*.json)|*.json", "json");

            rightActions.Controls.AddRange(new Control[] { _btnExportHtml, _btnExportMd, _btnExportCsv, _btnExportJson });
            topPanel.Controls.AddRange(new Control[] { _lblSummary, rightActions });

            _webBrowser = new WebBrowser
            {
                Dock = DockStyle.Fill,
                AllowWebBrowserDrop = false,
                IsWebBrowserContextMenuEnabled = false
            };

            var btmPanel = new Panel { Dock = DockStyle.Bottom, Height = 45, Padding = new Padding(12, 8, 12, 8) };
            _btnClose = new Button { Text = "Close", Dock = DockStyle.Right, Width = 90, DialogResult = DialogResult.OK };
            btmPanel.Controls.Add(_btnClose);

            Controls.Add(_webBrowser);
            Controls.Add(topPanel);
            Controls.Add(btmPanel);
        }

        private void RenderPreview()
        {
            var html = SoftwareInventoryReportGenerator.GenerateHtmlReport(_items);
            _webBrowser.DocumentText = html;
        }

        private void ExportReport(string filter, string format)
        {
            using var sfd = new SaveFileDialog
            {
                Filter = filter,
                FileName = $"Software_Audit_Report_{DateTime.Now:yyyyMMdd}.{format}"
            };

            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    string content = format switch
                    {
                        "html" => SoftwareInventoryReportGenerator.GenerateHtmlReport(_items),
                        "md" => SoftwareInventoryReportGenerator.GenerateMarkdownReport(_items),
                        "csv" => SoftwareInventoryReportGenerator.GenerateCsvReport(_items),
                        "json" => SoftwareInventoryReportGenerator.GenerateJsonReport(_items),
                        _ => string.Empty
                    };

                    File.WriteAllText(sfd.FileName, content);
                    MessageBox.Show(this, "Audit report exported successfully.", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Export failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
