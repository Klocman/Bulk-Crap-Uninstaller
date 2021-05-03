/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Klocman.IO;
using Klocman.Tools;
using UninstallTools.Properties;

namespace UninstallTools.Factory
{
    public class WindowsFeatureFactory : IIndependantUninstallerFactory
    {
        public IList<ApplicationUninstallerEntry> GetUninstallerEntries(
            ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            var results = new List<ApplicationUninstallerEntry>();
            if (Environment.OSVersion.Version < WindowsTools.Windows7) return results;

            Exception error = null;
            var t = new Thread(() =>
            {
                try
                {
                    results.AddRange(WmiQueries.GetWindowsFeatures()
                        .Where(x => x.Enabled)
                        .Select(WindowsFeatureToUninstallerEntry));
                }
                catch (Exception ex)
                {
                    error = ex;
                }
            });
            t.Start();

            t.Join(TimeSpan.FromSeconds(40));

            if (error != null)
                throw new IOException("Error while collecting Windows Features. If Windows Update is running wait until it finishes and try again. If the error persists try restarting your computer. In case nothing helps, read the KB957310 article.", error);
            if (t.IsAlive)
            {
                //t.Abort();
                throw new TimeoutException("WMI query has hung while collecting Windows Features, try restarting your computer. If the error persists read the KB957310 article.");
            }

            return results;
        }

        private static ApplicationUninstallerEntry WindowsFeatureToUninstallerEntry(WindowsFeatureInfo info)
        {
            var displayName = !string.IsNullOrEmpty(info.DisplayName) ? info.DisplayName : info.FeatureName;

            return new ApplicationUninstallerEntry
            {
                RawDisplayName = displayName,
                Comment = info.Description,
                UninstallString = DismTools.GetDismUninstallString(info.FeatureName, false),
                QuietUninstallString = DismTools.GetDismUninstallString(info.FeatureName, true),
                UninstallerKind = UninstallerType.WindowsFeature,
                Publisher = "Microsoft Corporation",
                IsValid = true,
                Is64Bit = ProcessTools.Is64BitProcess ? MachineType.X64 : MachineType.X86,
                RatingId = "WindowsFeature_" + info.FeatureName
            };
        }

        public bool IsEnabled() => UninstallToolsGlobalConfig.ScanWinFeatures;
        public string DisplayName => Localisation.Progress_AppStores_WinFeatures;
    }
}