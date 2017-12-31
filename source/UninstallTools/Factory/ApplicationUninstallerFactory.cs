/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Klocman.Extensions;
using Klocman.Forms.Tools;
using Klocman.IO;
using Klocman.Tools;
using UninstallTools.Factory.InfoAdders;
using UninstallTools.Properties;

namespace UninstallTools.Factory
{
    public static class ApplicationUninstallerFactory
    {
        public static ApplicationUninstallerFactoryCache Cache { get; }
        private static readonly string CachePath = Path.Combine(UninstallToolsGlobalConfig.AssemblyLocation, "InfoCache.xml");

        static ApplicationUninstallerFactory()
        {
            try
            {
                if (File.Exists(CachePath))
                    Cache = ApplicationUninstallerFactoryCache.Load(CachePath);
                else
                    Cache = new ApplicationUninstallerFactoryCache(CachePath);
            }
            catch (SystemException e)
            {
                Cache = new ApplicationUninstallerFactoryCache(CachePath);
                Console.WriteLine(e);
            }
        }

        public static IList<ApplicationUninstallerEntry> GetUninstallerEntries(ListGenerationProgress.ListGenerationCallback callback)
        {
            const int totalStepCount = 7;
            var currentStep = 1;

            var infoAdder = new InfoAdderManager();

            // Find msi products
            var msiProgress = new ListGenerationProgress(currentStep++, totalStepCount, Localisation.Progress_MSI);
            callback(msiProgress);
            var msiGuidCount = 0;
            var msiProducts = MsiTools.MsiEnumProducts().DoForEach(x =>
            {
                msiProgress.Inner = new ListGenerationProgress(0, -1, string.Format(Localisation.Progress_MSI_sub, ++msiGuidCount));
                callback(msiProgress);
            }).ToList();

            // Find stuff mentioned in registry
            List<ApplicationUninstallerEntry> registryResults;
            if (UninstallToolsGlobalConfig.ScanRegistry)
            {
                var regProgress = new ListGenerationProgress(currentStep++, totalStepCount,
                    Localisation.Progress_Registry);
                callback(regProgress);
                var registryFactory = new RegistryFactory(msiProducts);
                registryResults = registryFactory.GetUninstallerEntries(report =>
                {
                    regProgress.Inner = report;
                    callback(regProgress);
                }).ToList();

                // Fill in instal llocations for the drive search
                ApplyCache(registryResults, Cache, infoAdder);

                var installLocAddProgress = new ListGenerationProgress(currentStep++, totalStepCount, Localisation.Progress_GatherUninstallerInfo);
                callback(installLocAddProgress);
                var installLocAddCount = 0;
                foreach (var result in registryResults)
                {
                    installLocAddProgress.Inner = new ListGenerationProgress(installLocAddCount++, registryResults.Count, result.DisplayName ?? string.Empty);
                    callback(installLocAddProgress);

                    infoAdder.AddMissingInformation(result, true);
                }
            }
            else
            {
                registryResults = new List<ApplicationUninstallerEntry>();
            }

            // Look for entries on drives, based on info in registry. Need to check for duplicates with other entries later
            List<ApplicationUninstallerEntry> driveResults;
            if (UninstallToolsGlobalConfig.ScanDrives)
            {
                var driveProgress = new ListGenerationProgress(currentStep++, totalStepCount, Localisation.Progress_DriveScan);
                callback(driveProgress);
                var driveFactory = new DirectoryFactory(registryResults);
                driveResults = driveFactory.GetUninstallerEntries(report =>
                {
                    driveProgress.Inner = report;
                    callback(driveProgress);
                }).ToList();
            }
            else
            {
                driveResults = new List<ApplicationUninstallerEntry>();
            }

            // Get misc entries that use fancy logic
            var miscProgress = new ListGenerationProgress(currentStep++, totalStepCount, Localisation.Progress_AppStores);
            callback(miscProgress);
            var otherResults = GetMiscUninstallerEntries(report =>
            {
                miscProgress.Inner = report;
                callback(miscProgress);
            });

            // Handle duplicate entries
            var mergeProgress = new ListGenerationProgress(currentStep++, totalStepCount, Localisation.Progress_Merging);
            callback(mergeProgress);
            var mergedResults = registryResults.ToList();
            mergedResults = MergeResults(mergedResults, otherResults, infoAdder, report =>
            {
                mergeProgress.Inner = report;
                report.TotalCount *= 2;
                report.Message = Localisation.Progress_Merging_Stores;
                callback(mergeProgress);
            });
            // Make sure to merge driveResults last
            mergedResults = MergeResults(mergedResults, driveResults, infoAdder, report =>
            {
                mergeProgress.Inner = report;
                report.CurrentCount += report.TotalCount;
                report.TotalCount *= 2;
                report.Message = Localisation.Progress_Merging_Drives;
                callback(mergeProgress);
            });

            // Fill in any missing information
            ApplyCache(mergedResults, Cache, infoAdder);

            var infoAddProgress = new ListGenerationProgress(currentStep, totalStepCount, Localisation.Progress_GeneratingInfo);
            callback(infoAddProgress);
            var infoAddCount = 0;
            foreach (var result in mergedResults)
            {
                infoAddProgress.Inner = new ListGenerationProgress(infoAddCount++, registryResults.Count, result.DisplayName ?? string.Empty);
                callback(infoAddProgress);

                infoAdder.AddMissingInformation(result);
                result.IsValid = CheckIsValid(result, msiProducts);
            }

            //callback(new GetUninstallerListProgress(currentStep, totalStepCount, "Finished"));

            foreach (var entry in mergedResults.Where(x => !string.IsNullOrEmpty(x.RatingId)))
                Cache.Cache[entry.RatingId] = entry;
            try
            {
                Cache.Save();
            }
            catch (SystemException e)
            {
                //todo disable cache?
                Console.WriteLine(@"Failed to save cache: " + e);
            }

            return mergedResults;
        }

        private static List<ApplicationUninstallerEntry> MergeResults(ICollection<ApplicationUninstallerEntry> baseEntries,
            ICollection<ApplicationUninstallerEntry> newResults, InfoAdderManager infoAdder, ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            // Add all of the base results straight away
            var results = new List<ApplicationUninstallerEntry>(baseEntries);
            var progress = 0;
            foreach (var entry in newResults)
            {
                progressCallback(new ListGenerationProgress(progress++, newResults.Count, null));

                var matchedEntries = baseEntries.Where(x => CheckAreEntriesRelated(x, entry)).Take(2).ToList();
                if (matchedEntries.Count == 1)
                {
                    // Prevent setting incorrect UninstallerType
                    if (matchedEntries[0].UninstallPossible)
                        entry.UninstallerKind = UninstallerType.Unknown;

                    infoAdder.CopyMissingInformation(matchedEntries[0], entry);
                    continue;
                }

                // If the entry failed to match to anything, add it to the results
                results.Add(entry);
            }

            return results;
        }

        private static void ApplyCache(List<ApplicationUninstallerEntry> baseEntries, ApplicationUninstallerFactoryCache cache, InfoAdderManager infoAdder)
        {
            foreach (var entry in baseEntries.Where(x=>!string.IsNullOrEmpty(x.RatingId)))
            {
                ApplicationUninstallerEntry matchedEntry;
                if (cache.Cache.TryGetValue(entry.RatingId, out matchedEntry))
                    infoAdder.CopyMissingInformation(entry, matchedEntry);
            }
        }

        private static bool CheckIsValid(ApplicationUninstallerEntry target, IEnumerable<Guid> msiProducts)
        {
            if (string.IsNullOrEmpty(target.UninstallerFullFilename))
                return false;

            bool isPathRooted;
            try
            {
                isPathRooted = Path.IsPathRooted(target.UninstallerFullFilename);
            }
            catch (ArgumentException)
            {
                isPathRooted = false;
            }

            if (isPathRooted && File.Exists(target.UninstallerFullFilename))
                return true;

            if (target.UninstallerKind == UninstallerType.Msiexec)
                return msiProducts.Contains(target.BundleProviderKey);

            return !isPathRooted;
        }

        /// <summary>
        /// Try to figure out if base uninstaller entry and other entry are pointing to the same application.
        /// </summary>
        private static bool CheckAreEntriesRelated(ApplicationUninstallerEntry baseEntry, ApplicationUninstallerEntry otherEntry)
        {
            //Debug.Assert(!(otherEntry.DisplayName.Contains("vnc", StringComparison.OrdinalIgnoreCase) && 
            //    baseEntry.DisplayName.Contains("vnc", StringComparison.OrdinalIgnoreCase)));

            if (PathTools.PathsEqual(baseEntry.InstallLocation, otherEntry.InstallLocation))
                return true;

            var score = -1;
            if (!string.IsNullOrEmpty(baseEntry.InstallLocation) && !string.IsNullOrEmpty(otherEntry.InstallLocation))
                AddScore(ref score, -8, 0, -3, baseEntry.InstallLocation.Contains(otherEntry.InstallLocation,
                    StringComparison.InvariantCultureIgnoreCase));

            if (!string.IsNullOrEmpty(baseEntry.UninstallerLocation) && !string.IsNullOrEmpty(otherEntry.InstallLocation) && baseEntry.UninstallerLocation.StartsWith(otherEntry.InstallLocation, StringComparison.InvariantCultureIgnoreCase))
                return true;

            if (!string.IsNullOrEmpty(baseEntry.UninstallString) && !string.IsNullOrEmpty(otherEntry.InstallLocation) &&
                baseEntry.UninstallString.Contains(otherEntry.InstallLocation, StringComparison.InvariantCultureIgnoreCase))
                return true;

            AddScore(ref score, -5, 0, 3, baseEntry.Is64Bit != MachineType.Unknown && otherEntry.Is64Bit != MachineType.Unknown ?
                baseEntry.Is64Bit == otherEntry.Is64Bit : (bool?)null);
            AddScore(ref score, -3, -1, 5, CompareDates(baseEntry.InstallDate, otherEntry.InstallDate));

            AddScore(ref score, -2, 0, 5, CompareStrings(baseEntry.DisplayVersion, otherEntry.DisplayVersion, true));
            AddScore(ref score, -5, 0, 5, CompareStrings(baseEntry.Publisher, otherEntry.Publisher));

            // Check if base entry was installed from inside other entry's install directory
            if (string.IsNullOrEmpty(baseEntry.InstallLocation) && !string.IsNullOrEmpty(baseEntry.InstallSource) &&
                !string.IsNullOrEmpty(otherEntry.InstallLocation) && otherEntry.InstallLocation.Length >= 5)
            {
                AddScore(ref score, 0, 0, 5, baseEntry.InstallSource.Contains(
                    otherEntry.InstallLocation, StringComparison.InvariantCultureIgnoreCase));
            }

            if (score <= -14) return false;

            var nameSimilarity = CompareStrings(baseEntry.DisplayName, otherEntry.DisplayName);
            AddScore(ref score, -5, -2, 8, nameSimilarity);
            if (!nameSimilarity.HasValue || nameSimilarity == false)
            {
                var trimmedSimilarity = CompareStrings(baseEntry.DisplayNameTrimmed, otherEntry.DisplayNameTrimmed);
                // Don't risk it if names can't be compared at all
                //if (!trimmedSimilarity.HasValue && !nameSimilarity.HasValue) return false;
                AddScore(ref score, -5, -2, 8, trimmedSimilarity);
            }

            if (score <= -4) return false;

            try
            {
                AddScore(ref score, -2, -2, 5,
                    CompareStrings(baseEntry.DisplayNameTrimmed.Length < 5 ? baseEntry.DisplayName : baseEntry.DisplayNameTrimmed, Path.GetFileName(otherEntry.InstallLocation)));
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
            }
            //Debug.Assert(score <= 0);
            return score > 0;
        }

        /// <summary>
        /// Check if dates are very close together, or if they differ by a few hours.
        /// Result is null if the length of the difference can't be compared confidently.
        /// </summary>
        private static bool? CompareDates(DateTime a, DateTime b)
        {
            if (a.IsDefault() || b.IsDefault())
                return null;

            var totalHours = Math.Abs((a - b).TotalHours);

            if (totalHours > 40) return null;

            if (totalHours <= 1)
                return true;

            // One of the dates is lacking time part, so can't be compared
            if (a.TimeOfDay.TotalSeconds < 1 || b.TimeOfDay.TotalSeconds < 1)
                return null;

            return totalHours <= 1;
        }

        private static bool? CompareStrings(string a, string b, bool relaxMatchRequirement = false)
        {
            var lengthRequirement = !relaxMatchRequirement ? 5 : 4;
            if (a == null || (a.Length < lengthRequirement) || b == null || b.Length < lengthRequirement)
                return null;

            if (relaxMatchRequirement)
            {
                if (a.StartsWith(b, StringComparison.Ordinal) || b.StartsWith(a, StringComparison.Ordinal))
                    return true;
            }

            var changesRequired = Sift4.SimplestDistance(a, b, 3);
            return changesRequired == 0 || changesRequired < a.Length / 6;
        }

        private static void AddScore(ref int score, int failScore, int unsureScore, int successScore, bool? testResult)
        {
            if (!testResult.HasValue) score = score + unsureScore;
            else score = testResult.Value ? score + successScore : score + failScore;
        }

        private static List<ApplicationUninstallerEntry> GetMiscUninstallerEntries(ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            var otherResults = new List<ApplicationUninstallerEntry>();

            var miscFactories = new Dictionary<IUninstallerFactory, string>();
            if (UninstallToolsGlobalConfig.ScanPreDefined)
                miscFactories.Add(new PredefinedFactory(), Localisation.Progress_AppStores_Templates);
            if (UninstallToolsGlobalConfig.ScanSteam)
                miscFactories.Add(new SteamFactory(), Localisation.Progress_AppStores_Steam);
            if (UninstallToolsGlobalConfig.ScanStoreApps)
                miscFactories.Add(new StoreAppFactory(), Localisation.Progress_AppStores_WinStore);
            if (UninstallToolsGlobalConfig.ScanWinFeatures)
                miscFactories.Add(new WindowsFeatureFactory(), Localisation.Progress_AppStores_WinFeatures);
            if (UninstallToolsGlobalConfig.ScanWinUpdates)
                miscFactories.Add(new WindowsUpdateFactory(), Localisation.Progress_AppStores_WinUpdates);

            var progress = 0;
            foreach (var kvp in miscFactories)
            {
                progressCallback(new ListGenerationProgress(progress++, miscFactories.Count, kvp.Value));
                try
                {
                    otherResults.AddRange(kvp.Key.GetUninstallerEntries(null));
                }
                catch (Exception ex)
                {
                    PremadeDialogs.GenericError(ex);
                }
            }

            return otherResults;
        }

        /// <summary>
        ///     Check if path points to the windows installer program or to a .msi package
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool PathPointsToMsiExec(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            return path.ContainsAny(new[] { "msiexec ", "msiexec.exe" }, StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".msi", StringComparison.OrdinalIgnoreCase);
        }

        public static string CleanupDisplayVersion(string version)
        {
            return version?.Replace(", ", ".").Replace(". ", ".").Replace(",", ".").Replace(". ", ".").Trim();
        }
    }
}