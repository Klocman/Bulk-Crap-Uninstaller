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

namespace UninstallTools.History
{
    public enum LifecycleStage
    {
        NewlyInstalled,  // < 14 days
        Established,     // 14 - 90 days
        LongTerm,        // 90 - 365 days
        Vintage          // > 365 days
    }

    public class SoftwareLifetimeTimelineEntry
    {
        public string ApplicationName { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public DateTime InstallDateUtc { get; set; } = DateTime.UtcNow;
        public int AgeInDays { get; set; }
        public LifecycleStage Stage { get; set; } = LifecycleStage.Established;
        public string FormattedInstallDate => InstallDateUtc.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// Reconstructs the complete historical software installation and lifecycle timeline of the machine.
    /// </summary>
    public static class SoftwareLifetimeTrackerEngine
    {
        /// <summary>
        /// Compiles a chronological lifecycle timeline from a collection of installed software entries.
        /// </summary>
        public static List<SoftwareLifetimeTimelineEntry> BuildLifecycleTimeline(IEnumerable<ApplicationUninstallerEntry> apps)
        {
            var timeline = new List<SoftwareLifetimeTimelineEntry>();
            if (apps == null) return timeline;

            var now = DateTime.UtcNow;

            foreach (var app in apps)
            {
                if (string.IsNullOrWhiteSpace(app.DisplayName)) continue;

                var installDate = app.InstallDate;
                if (installDate == DateTime.MinValue || installDate > now)
                {
                    installDate = now.AddDays(-30); // Default estimate if not recorded
                }

                var age = Math.Max(0, (int)(now - installDate).TotalDays);

                LifecycleStage stage;
                if (age <= 14) stage = LifecycleStage.NewlyInstalled;
                else if (age <= 90) stage = LifecycleStage.Established;
                else if (age <= 365) stage = LifecycleStage.LongTerm;
                else stage = LifecycleStage.Vintage;

                timeline.Add(new SoftwareLifetimeTimelineEntry
                {
                    ApplicationName = app.DisplayName,
                    Publisher = app.Publisher ?? "Unknown",
                    Version = app.DisplayVersion ?? "Unknown",
                    InstallDateUtc = installDate,
                    AgeInDays = age,
                    Stage = stage
                });
            }

            return timeline.OrderByDescending(t => t.InstallDateUtc).ToList();
        }
    }
}
