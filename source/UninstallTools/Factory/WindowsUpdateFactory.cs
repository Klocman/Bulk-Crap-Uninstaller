/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Tools;

namespace UninstallTools.Factory
{
    public class WindowsUpdateFactory : IUninstallerFactory
    {
        public IEnumerable<ApplicationUninstallerEntry> GetUninstallerEntries(ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            return GetUpdates();
        }
        private static bool? _helperIsAvailable;

        private static bool HelperIsAvailable
        {
            get
            {
                if (!_helperIsAvailable.HasValue)
                    _helperIsAvailable = File.Exists(HelperPath) && WindowsTools.CheckNetFramework4Installed(true);
                return _helperIsAvailable.Value;
            }
        }

        private static string HelperPath
            => Path.Combine(UninstallToolsGlobalConfig.AssemblyLocation, @"WinUpdateHelper.exe");

        private static IEnumerable<ApplicationUninstallerEntry> GetUpdates()
        {
            if (!HelperIsAvailable)
                yield break;

            var output = FactoryTools.StartProcessAndReadOutput(HelperPath, "list");
            if (string.IsNullOrEmpty(output) || output.Contains("error", StringComparison.OrdinalIgnoreCase))
                yield break;

            foreach (var group in ProcessInput(output))
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
                            Guid result;
                            if (GuidTools.TryExtractGuid(valuePair.Value, out result))
                                entry.BundleProviderKey = result;
                            break;
                        case "RevisionNumber":
                            entry.DisplayVersion =  ApplicationEntryTools.CleanupDisplayVersion(valuePair.Value);
                            break;
                        case "Title":
                            entry.RawDisplayName = valuePair.Value;
                            break;
                        case "IsUninstallable":
                            bool isUnins;
                            if (bool.TryParse(valuePair.Value, out isUnins))
                                entry.IsValid = isUnins;
                            break;
                        case "SupportUrl":
                            entry.AboutUrl = valuePair.Value;
                            break;
                        case "MinDownloadSize":
                            long size;
                            if (long.TryParse(valuePair.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out size))
                                entry.EstimatedSize = FileSize.FromBytes(size);
                            break;
                        case "LastDeploymentChangeTime":
                            DateTime date;
                            if (DateTime.TryParse(valuePair.Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out date) &&
                                !DateTime.MinValue.Equals(date))
                                entry.InstallDate = date;
                            break;
                    }
                }

                if (entry.IsValid)
                {
                    entry.UninstallString = $"\"{HelperPath}\" uninstall {entry.RatingId}";
                    entry.QuietUninstallString = entry.UninstallString;
                }

                yield return entry;
            }
        }

        private static IEnumerable<List<KeyValuePair<string, string>>> ProcessInput(string input)
        {
            var res = new List<List<KeyValuePair<string, string>>> { new List<KeyValuePair<string, string>>() };

            foreach (var line in input.Trim().Trim('\n', '\r').Trim().SplitNewlines(StringSplitOptions.None))
            {
                if (string.IsNullOrEmpty(line))
                {
                    if (res.Last().Any())
                        res.Add(new List<KeyValuePair<string, string>>());
                }
                else
                {
                    var o = line.Split(new[] { " - " }, StringSplitOptions.None);
                    res.Last().Add(new KeyValuePair<string, string>(o[0], o[1]));
                }
            }

            if (!res.Last().Any())
                res.RemoveAt(res.Count - 1);

            return res;
        }
    }
}
