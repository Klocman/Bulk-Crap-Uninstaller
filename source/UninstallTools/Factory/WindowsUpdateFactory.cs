/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Klocman.IO;
using Klocman.Tools;
using UninstallTools.Properties;

namespace UninstallTools.Factory
{
    public class WindowsUpdateFactory : IIndependantUninstallerFactory
    {
        private static string HelperPath { get; } = Path.Combine(UninstallToolsGlobalConfig.AssemblyLocation, @"WinUpdateHelper.exe");
        private static bool IsHelperAvailable() => File.Exists(HelperPath);

        public IList<ApplicationUninstallerEntry> GetUninstallerEntries(ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            var results = new List<ApplicationUninstallerEntry>();
            if (!IsHelperAvailable()) return results;

            var output = FactoryTools.StartHelperAndReadOutput(HelperPath, "list");
            if (string.IsNullOrEmpty(output) || output.Trim().StartsWith("Error", StringComparison.OrdinalIgnoreCase)) return results;

            foreach (var group in FactoryTools.ExtractAppDataSetsFromHelperOutput(output))
            {
                var entry = new ApplicationUninstallerEntry
                {
                    UninstallerKind = UninstallerType.WindowsUpdate,
                    IsUpdate = true,
                    Publisher = "Microsoft Corporation"
                };
                foreach (var valuePair in group)
                {
                    switch (valuePair.Key)
                    {
                        case "UpdateID":
                            entry.RatingId = valuePair.Value;
                            if (GuidTools.TryExtractGuid(valuePair.Value, out var result))
                                entry.BundleProviderKey = result;
                            break;
                        case "RevisionNumber":
                            entry.DisplayVersion = ApplicationEntryTools.CleanupDisplayVersion(valuePair.Value);
                            break;
                        case "Title":
                            entry.RawDisplayName = valuePair.Value;
                            break;
                        case "IsUninstallable":
                            if (bool.TryParse(valuePair.Value, out var isUnins))
                                entry.IsProtected = !isUnins;
                            break;
                        case "SupportUrl":
                            entry.AboutUrl = valuePair.Value;
                            break;
                        case "MinDownloadSize":
                            if (long.TryParse(valuePair.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var size))
                                entry.EstimatedSize = FileSize.FromBytes(size);
                            break;
                        case "MaxDownloadSize":
                            break;
                        case "LastDeploymentChangeTime":
                            if (DateTime.TryParse(valuePair.Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) &&
                                !DateTime.MinValue.Equals(date))
                                entry.InstallDate = date;
                            break;
                        default:
                            Debug.Fail("Unknown label");
                            break;
                    }
                }

                entry.UninstallString = $"\"{HelperPath}\" uninstall {entry.RatingId}";
                entry.QuietUninstallString = entry.UninstallString;

                results.Add(entry);
            }

            return results;
        }

        public bool IsEnabled() => UninstallToolsGlobalConfig.ScanWinUpdates;
        public string DisplayName => Localisation.Progress_AppStores_WinUpdates;
    }
}
