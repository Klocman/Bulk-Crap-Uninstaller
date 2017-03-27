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
        public delegate void GetUninstallerListCallback(GetUninstallerListProgress progressReport);

        public class GetUninstallerListProgress
        {
            internal GetUninstallerListProgress(int currentCount, int totalCount, string message)
            {
                TotalCount = totalCount;
                Message = message;
                CurrentCount = currentCount;
            }

            public int CurrentCount { get; }
            /// <summary>
            /// -1 if unknown
            /// </summary>
            public int TotalCount { get; }
            public string Message { get; }

            //public GetUninstallerListProgress Clone() => (GetUninstallerListProgress)MemberwiseClone();

            public GetUninstallerListProgress Inner { get; internal set; }
        }
        
        public static IEnumerable<ApplicationUninstallerEntry> GetUninstallerEntries(GetUninstallerListCallback callback)
        {
            const int totalStepCount = 8;
            var currentStep = 1;

            // Find msi products
            var msiProgress = new GetUninstallerListProgress(currentStep++, totalStepCount, Localisation.Progress_MSI);
            callback(msiProgress);
            var msiGuidCount = 0;
            var msiProducts = MsiTools.MsiEnumProducts().DoForEach(x =>
            {
                msiProgress.Inner = new GetUninstallerListProgress(0, -1, string.Format(Localisation.Progress_MSI_sub, ++msiGuidCount));
                callback(msiProgress);
            }).ToList();

            // Find stuff mentioned in registry
            var regProgress = new GetUninstallerListProgress(currentStep++, totalStepCount, Localisation.Progress_Registry);
            callback(regProgress);
            var registryFactory = new RegistryFactory(msiProducts);
            var registryResults = registryFactory.GetUninstallerEntries(report =>
            {
                regProgress.Inner = report;
                callback(regProgress);
            }).ToList();

            // Fill in instal llocations for the drive search
            var installLocAddProgress = new GetUninstallerListProgress(currentStep++, totalStepCount, Localisation.Progress_GatherUninstallerInfo);
            callback(installLocAddProgress);
            var infoAdder = new InfoAdderManager();
            var installLocAddCount = 0;
            foreach (var result in registryResults)
            {
                installLocAddProgress.Inner = new GetUninstallerListProgress(installLocAddCount++, registryResults.Count, result.DisplayName ?? string.Empty);
                callback(installLocAddProgress);

                infoAdder.AddMissingInformation(result, true);
            }

            // Look for entries on drives, based on info in registry. Need to check for duplicates with other entries later
            var driveProgress = new GetUninstallerListProgress(currentStep++, totalStepCount, Localisation.Progress_DriveScan);
            callback(driveProgress);
            var driveFactory = new DirectoryFactory(registryResults);
            var driveResults = driveFactory.GetUninstallerEntries(report => 
            {
                driveProgress.Inner = report;
                callback(driveProgress);
            }).ToList();

            // Get misc entries that use fancy logic
            var miscProgress = new GetUninstallerListProgress(currentStep++, totalStepCount, Localisation.Progress_AppStores);
            callback(miscProgress);
            var otherResults = GetMiscUninstallerEntries(report =>
            {
                miscProgress.Inner = report;
                callback(miscProgress);
            });

            // Handle duplicate entries
            var mergeProgress = new GetUninstallerListProgress(currentStep++, totalStepCount, Localisation.Progress_Merging);
            mergeProgress.Inner = new GetUninstallerListProgress(1, 4, Localisation.Progress_Merging_Stores);
            callback(mergeProgress);
            var mergedResults = registryResults.ToList();
            mergedResults = MergeResults(mergedResults, otherResults, infoAdder);
            // Make sure to merge driveResults last
            mergeProgress.Inner = new GetUninstallerListProgress(3, 4, Localisation.Progress_Merging_Drives);
            callback(mergeProgress);
            mergedResults = MergeResults(mergedResults, driveResults, infoAdder);

            // Fill in any missing information
            var infoAddProgress = new GetUninstallerListProgress(currentStep, totalStepCount, Localisation.Progress_GeneratingInfo);
            callback(infoAddProgress);
            var infoAddCount = 0;
            foreach (var result in mergedResults)
            {
                infoAddProgress.Inner = new GetUninstallerListProgress(infoAddCount++, registryResults.Count, result.DisplayName ?? string.Empty);
                callback(infoAddProgress);

                infoAdder.AddMissingInformation(result);
                result.IsValid = CheckIsValid(result, msiProducts);
            }

            //callback(new GetUninstallerListProgress(currentStep, totalStepCount, "Finished"));
            return mergedResults;
        }

        private static List<ApplicationUninstallerEntry> MergeResults(IEnumerable<ApplicationUninstallerEntry> baseResults,
            IEnumerable<ApplicationUninstallerEntry> newResults, InfoAdderManager infoAdder)
        {
            // Create local copy
            var baseEntries = baseResults.ToList();
            // Add all of the base results straight away
            var results = new List<ApplicationUninstallerEntry>(baseEntries);
            foreach (var entry in newResults)
            {
                try
                {
                    var matchedEntry = baseEntries.SingleOrDefault(x => CheckAreEntriesRelated(x, entry));
                    if (matchedEntry != null)
                    {
                        // Prevent setting incorrect UninstallerType
                        if (matchedEntry.UninstallPossible)
                            entry.UninstallerKind = UninstallerType.Unknown;

                        infoAdder.CopyMissingInformation(matchedEntry, entry);
                        continue;
                    }
                }
                catch (InvalidOperationException) { Debug.Fail("MergeResults matched more than one entry"); }

                // If the entry failed to match to anything, add it to the results
                results.Add(entry);
            }

            return results;
        }

        private static bool CheckIsValid(ApplicationUninstallerEntry target, IEnumerable<Guid> msiProducts)
        {
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

        private static bool CheckAreEntriesRelated(ApplicationUninstallerEntry baseEntry, ApplicationUninstallerEntry otherEntry)
        {
            if (PathTools.PathsEqual(baseEntry.InstallLocation, otherEntry.InstallLocation))
                return true;

            if (!string.IsNullOrEmpty(baseEntry.UninstallerLocation) && (!string.IsNullOrEmpty(otherEntry.InstallLocation)
                 && baseEntry.UninstallerLocation.StartsWith(otherEntry.InstallLocation, StringComparison.InvariantCultureIgnoreCase)))
                return true;

            if (!string.IsNullOrEmpty(baseEntry.UninstallString) && !string.IsNullOrEmpty(otherEntry.InstallLocation)
                && baseEntry.UninstallString.Contains(otherEntry.InstallLocation))
                return true;

            if (CompareStrings(baseEntry.Publisher, otherEntry.Publisher))
            {
                if (CompareStrings(baseEntry.DisplayName, otherEntry.DisplayName))
                    return true;
                if (CompareStrings(baseEntry.DisplayNameTrimmed, otherEntry.DisplayNameTrimmed))
                    return true;
                try
                {
                    if (CompareStrings(baseEntry.DisplayNameTrimmed, Path.GetFileName(otherEntry.InstallLocation)))
                        return true;
                }
                catch (Exception ex)
                {
                    Debug.Fail(ex.Message);
                }
            }

            return false;
        }

        private static bool CompareStrings(string a, string b)
        {
            if (a == null || a.Length < 5 || b == null || b.Length < 5)
                return false;

            var changesRequired = StringTools.CompareSimilarity(a, b);
            return changesRequired < a.Length / 6;
        }
        
        private static List<ApplicationUninstallerEntry> GetMiscUninstallerEntries(GetUninstallerListCallback progressCallback)
        {
            var otherResults = new List<ApplicationUninstallerEntry>();
            var miscFactories = new[]
            {
                new KeyValuePair<IUninstallerFactory,string>(new PredefinedFactory(), Localisation.Progress_AppStores_Templates),
                new KeyValuePair<IUninstallerFactory,string>(new SteamFactory(), Localisation.Progress_AppStores_Steam),
                new KeyValuePair<IUninstallerFactory,string>(new StoreAppFactory(), Localisation.Progress_AppStores_WinStore),
                new KeyValuePair<IUninstallerFactory,string>(new WindowsFeatureFactory(), Localisation.Progress_AppStores_WinFeatures)
            };
            var progress = 0;
            foreach (var kvp in miscFactories)
            {
                progressCallback(new GetUninstallerListProgress(progress++, miscFactories.Length, kvp.Value));
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
    }
}