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
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.Detection
{
    public class CveRecord
    {
        public string CveId { get; set; } = string.Empty;
        public string AffectedSoftware { get; set; } = string.Empty;
        public double CvssScore { get; set; } = 7.5;
        public string Severity { get; set; } = "High";
        public string Summary { get; set; } = string.Empty;
        public string Remediation { get; set; } = string.Empty;
    }

    /// <summary>
    /// Offline CVE security vulnerability intelligence database and compliance auditor.
    /// </summary>
    public static class CveDatabaseAuditor
    {
        private static readonly List<CveRecord> CveDatabase = new List<CveRecord>
        {
            new CveRecord
            {
                CveId = "CVE-2023-38831",
                AffectedSoftware = "WinRAR",
                CvssScore = 9.8,
                Severity = "Critical",
                Summary = "Remote Code Execution via spoofed file extensions in ZIP archives.",
                Remediation = "Upgrade WinRAR to 6.23 or newer."
            },
            new CveRecord
            {
                CveId = "CVE-2024-31497",
                AffectedSoftware = "PuTTY",
                CvssScore = 9.1,
                Severity = "Critical",
                Summary = "Biased ECDSA nonce generation allows recovery of private SSH keys.",
                Remediation = "Upgrade PuTTY to 0.81 or newer."
            },
            new CveRecord
            {
                CveId = "CVE-2024-32002",
                AffectedSoftware = "Git",
                CvssScore = 9.8,
                Severity = "Critical",
                Summary = "Remote Code Execution during git clone on case-insensitive filesystems.",
                Remediation = "Upgrade Git for Windows to 2.45.1 or newer."
            },
            new CveRecord
            {
                CveId = "CVE-2023-40481",
                AffectedSoftware = "7-Zip",
                CvssScore = 7.8,
                Severity = "High",
                Summary = "Heap buffer overflow in SquashFS image parser.",
                Remediation = "Upgrade 7-Zip to 23.01 or newer."
            }
        };

        /// <summary>
        /// Audits installed applications against the offline CVE intelligence base.
        /// </summary>
        public static List<CveRecord> AuditApplications(IEnumerable<ApplicationUninstallerEntry> apps)
        {
            var findings = new List<CveRecord>();
            if (apps == null) return findings;

            foreach (var app in apps)
            {
                if (string.IsNullOrWhiteSpace(app.DisplayName)) continue;

                foreach (var cve in CveDatabase)
                {
                    if (app.DisplayName.IndexOf(cve.AffectedSoftware, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        findings.Add(cve);
                    }
                }
            }

            return findings.OrderByDescending(f => f.CvssScore).ToList();
        }
    }
}
