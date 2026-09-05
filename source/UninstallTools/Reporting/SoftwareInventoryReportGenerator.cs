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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using UninstallTools.Core;

namespace UninstallTools.Reporting
{
    /// <summary>
    /// Simplified software item representation for cross-module reporting.
    /// </summary>
    public class ReportSoftwareItem
    {
        public string DisplayName { get; set; } = string.Empty;
        public string DisplayVersion { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string InstallDate { get; set; } = string.Empty;
        public string InstallLocation { get; set; } = string.Empty;
        public long EstimatedSizeBytes { get; set; }
        public string Architecture { get; set; } = "x64";
        public string UninstallerType { get; set; } = "Registry";
        public bool IsValidSigned { get; set; }
        public string SafetyScore { get; set; } = "Safe";
    }

    /// <summary>
    /// Generates structured software inventory audit reports in HTML, Markdown, CSV, and JSON formats.
    /// </summary>
    public static class SoftwareInventoryReportGenerator
    {
        /// <summary>
        /// Generates a modern HTML audit report with CSS styling and summary badges.
        /// </summary>
        public static string GenerateHtmlReport(IEnumerable<ReportSoftwareItem> items, string title = "EBUninstaller Pro - Software Inventory Audit Report")
        {
            var list = items?.ToList() ?? new List<ReportSoftwareItem>();
            var totalSize = list.Sum(i => i.EstimatedSizeBytes);
            var signedCount = list.Count(i => i.IsValidSigned);
            var safeCount = list.Count(i => string.Equals(i.SafetyScore, "Safe", StringComparison.OrdinalIgnoreCase));

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset=\"UTF-8\">");
            sb.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            sb.AppendLine($"  <title>{title}</title>");
            sb.AppendLine("  <style>");
            sb.AppendLine("    :root { --primary: #0284c7; --bg: #0f172a; --card: #1e293b; --text: #f8fafc; --muted: #94a3b8; --border: #334155; --success: #10b981; --warn: #f59e0b; }");
            sb.AppendLine("    body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; background: var(--bg); color: var(--text); margin: 0; padding: 24px; line-height: 1.5; }");
            sb.AppendLine("    .container { max-width: 1200px; margin: 0 auto; }");
            sb.AppendLine("    .header { display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid var(--border); padding-bottom: 16px; margin-bottom: 24px; }");
            sb.AppendLine("    .header h1 { margin: 0; font-size: 1.75rem; color: #38bdf8; }");
            sb.AppendLine("    .metrics-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 16px; margin-bottom: 24px; }");
            sb.AppendLine("    .metric-card { background: var(--card); border: 1px solid var(--border); border-radius: 8px; padding: 16px; text-align: center; }");
            sb.AppendLine("    .metric-val { font-size: 1.75rem; font-weight: bold; color: #38bdf8; }");
            sb.AppendLine("    .metric-lbl { font-size: 0.85rem; color: var(--muted); text-transform: uppercase; letter-spacing: 0.5px; }");
            sb.AppendLine("    table { width: 100%; border-collapse: collapse; background: var(--card); border-radius: 8px; overflow: hidden; border: 1px solid var(--border); }");
            sb.AppendLine("    th, td { padding: 12px 16px; text-align: left; border-bottom: 1px solid var(--border); }");
            sb.AppendLine("    th { background: #0b1120; color: var(--muted); font-weight: 600; font-size: 0.85rem; text-transform: uppercase; }");
            sb.AppendLine("    tr:hover { background: #243247; }");
            sb.AppendLine("    .badge { display: inline-block; padding: 2px 8px; border-radius: 9999px; font-size: 0.75rem; font-weight: 600; }");
            sb.AppendLine("    .badge-safe { background: rgba(16, 185, 129, 0.2); color: #34d399; }");
            sb.AppendLine("    .badge-warn { background: rgba(245, 158, 11, 0.2); color: #fbbf24; }");
            sb.AppendLine("    .footer { margin-top: 32px; text-align: center; font-size: 0.85rem; color: var(--muted); border-top: 1px solid var(--border); padding-top: 16px; }");
            sb.AppendLine("  </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("  <div class=\"container\">");
            sb.AppendLine("    <div class=\"header\">");
            sb.AppendLine($"      <div><h1>{title}</h1><div style=\"color: var(--muted); font-size: 0.9rem;\">Generated on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC &bull; Host: {Environment.MachineName}</div></div>");
            sb.AppendLine("      <div style=\"font-weight: bold; color: var(--primary); font-size: 1.1rem;\">EBUninstaller Pro 7.0</div>");
            sb.AppendLine("    </div>");

            sb.AppendLine("    <div class=\"metrics-grid\">");
            sb.AppendLine($"      <div class=\"metric-card\"><div class=\"metric-val\">{list.Count}</div><div class=\"metric-lbl\">Installed Applications</div></div>");
            sb.AppendLine($"      <div class=\"metric-card\"><div class=\"metric-val\">{FormatSize(totalSize)}</div><div class=\"metric-lbl\">Total Disk Usage</div></div>");
            sb.AppendLine($"      <div class=\"metric-card\"><div class=\"metric-val\">{signedCount}</div><div class=\"metric-lbl\">Digitally Signed</div></div>");
            sb.AppendLine($"      <div class=\"metric-card\"><div class=\"metric-val\">{safeCount}</div><div class=\"metric-lbl\">Verified Safe</div></div>");
            sb.AppendLine("    </div>");

            sb.AppendLine("    <table>");
            sb.AppendLine("      <thead>");
            sb.AppendLine("        <tr>");
            sb.AppendLine("          <th>Application Name</th>");
            sb.AppendLine("          <th>Version</th>");
            sb.AppendLine("          <th>Publisher</th>");
            sb.AppendLine("          <th>Install Date</th>");
            sb.AppendLine("          <th>Est. Size</th>");
            sb.AppendLine("          <th>Arch</th>");
            sb.AppendLine("          <th>Safety</th>");
            sb.AppendLine("        </tr>");
            sb.AppendLine("      </thead>");
            sb.AppendLine("      <tbody>");

            foreach (var item in list)
            {
                var badgeClass = item.SafetyScore == "Safe" ? "badge-safe" : "badge-warn";
                sb.AppendLine("        <tr>");
                sb.AppendLine($"          <td style=\"font-weight: 500;\">{Escape(item.DisplayName)}</td>");
                sb.AppendLine($"          <td>{Escape(item.DisplayVersion)}</td>");
                sb.AppendLine($"          <td>{Escape(item.Publisher)}</td>");
                sb.AppendLine($"          <td>{Escape(item.InstallDate)}</td>");
                sb.AppendLine($"          <td>{FormatSize(item.EstimatedSizeBytes)}</td>");
                sb.AppendLine($"          <td>{Escape(item.Architecture)}</td>");
                sb.AppendLine($"          <td><span class=\"badge {badgeClass}\">{Escape(item.SafetyScore)}</span></td>");
                sb.AppendLine("        </tr>");
            }

            sb.AppendLine("      </tbody>");
            sb.AppendLine("    </table>");
            sb.AppendLine("    <div class=\"footer\">Report generated by EBUninstaller Pro &bull; Privacy-first &bull; Fully Offline</div>");
            sb.AppendLine("  </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        /// <summary>
        /// Generates a Markdown audit report.
        /// </summary>
        public static string GenerateMarkdownReport(IEnumerable<ReportSoftwareItem> items)
        {
            var list = items?.ToList() ?? new List<ReportSoftwareItem>();
            var totalSize = list.Sum(i => i.EstimatedSizeBytes);

            var sb = new StringBuilder();
            sb.AppendLine("# EBUninstaller Pro - Software Inventory Audit Report");
            sb.AppendLine($"*Generated on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC on machine `{Environment.MachineName}`*");
            sb.AppendLine();
            sb.AppendLine("## Summary");
            sb.AppendLine($"- **Total Applications:** {list.Count}");
            sb.AppendLine($"- **Total Estimated Disk Usage:** {FormatSize(totalSize)}");
            sb.AppendLine($"- **Digitally Signed Count:** {list.Count(i => i.IsValidSigned)}");
            sb.AppendLine();
            sb.AppendLine("## Installed Applications");
            sb.AppendLine("| Name | Version | Publisher | Install Date | Size | Arch | Safety |");
            sb.AppendLine("|---|---|---|---|---|---|---|");

            foreach (var item in list)
            {
                sb.AppendLine($"| {EscapeMd(item.DisplayName)} | {EscapeMd(item.DisplayVersion)} | {EscapeMd(item.Publisher)} | {EscapeMd(item.InstallDate)} | {FormatSize(item.EstimatedSizeBytes)} | {item.Architecture} | {item.SafetyScore} |");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generates a standard CSV report.
        /// </summary>
        public static string GenerateCsvReport(IEnumerable<ReportSoftwareItem> items)
        {
            var list = items?.ToList() ?? new List<ReportSoftwareItem>();
            var sb = new StringBuilder();
            sb.AppendLine("DisplayName,DisplayVersion,Publisher,InstallDate,EstimatedSizeBytes,Architecture,UninstallerType,IsValidSigned,SafetyScore");

            foreach (var item in list)
            {
                sb.AppendLine($"\"{EscapeCsv(item.DisplayName)}\",\"{EscapeCsv(item.DisplayVersion)}\",\"{EscapeCsv(item.Publisher)}\",\"{EscapeCsv(item.InstallDate)}\",{item.EstimatedSizeBytes},\"{EscapeCsv(item.Architecture)}\",\"{EscapeCsv(item.UninstallerType)}\",{item.IsValidSigned},\"{EscapeCsv(item.SafetyScore)}\"");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generates a JSON inventory report.
        /// </summary>
        public static string GenerateJsonReport(IEnumerable<ReportSoftwareItem> items)
        {
            var list = items?.ToList() ?? new List<ReportSoftwareItem>();
            var reportObj = new
            {
                Application = "EBUninstaller Pro",
                GeneratedUtc = DateTime.UtcNow,
                MachineName = Environment.MachineName,
                OSVersion = Environment.OSVersion.ToString(),
                TotalCount = list.Count,
                TotalEstimatedSizeBytes = list.Sum(i => i.EstimatedSizeBytes),
                Applications = list
            };

            return JsonSerializer.Serialize(reportObj, new JsonSerializerOptions { WriteIndented = true });
        }

        private static string Escape(string val) => System.Net.WebUtility.HtmlEncode(val ?? string.Empty);
        private static string EscapeMd(string val) => (val ?? string.Empty).Replace("|", "\\|");
        private static string EscapeCsv(string val) => (val ?? string.Empty).Replace("\"", "\"\"");

        private static string FormatSize(long bytes)
        {
            if (bytes <= 0) return "0 MB";
            if (bytes >= 1024L * 1024 * 1024)
                return $"{(bytes / (1024.0 * 1024 * 1024)):F2} GB";
            return $"{(bytes / (1024.0 * 1024)):F1} MB";
        }
    }
}
