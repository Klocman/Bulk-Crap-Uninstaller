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

namespace UninstallTools.Factory
{
    public static class ApplicationUninstallerFactory
    {
        public delegate void GetUninstallerListCallback(GetUninstallerListProgress progressReport);

        public class GetUninstallerListProgress
        {
            internal GetUninstallerListProgress(int currentCount, int totalCount)
            {
                TotalCount = totalCount;
                CurrentCount = currentCount;
            }

            public int CurrentCount { get; }
            public int TotalCount { get; }

            // TODO public GetUninstallerListProgress Inner { get; internal set; }
            //public ApplicationUninstallerEntry FinishedEntry { get; internal set; }
        }


        public static IEnumerable<ApplicationUninstallerEntry> GetUninstallerEntries(GetUninstallerListCallback callback)
        {
            const int totalStepCount = 6;
            // Find msi products
            callback(new GetUninstallerListProgress(0, totalStepCount));
            var msiProducts = MsiTools.MsiEnumProducts().ToList();

            // Find stuff mentioned in registry
            callback(new GetUninstallerListProgress(1, totalStepCount));
            var registryFactory = new RegistryFactory(msiProducts);
            var registryResults = registryFactory.GetUninstallerEntries().ToList();
            //todo fill in install location and uninstaller location for driveFactory
            // find drive stuff (based on directories found in install locations, 2 or more apps installed in one location = scan location for drive stuff)
            //results = results.DoForEach(entry => infoAdder.TryAddFieldInformation(entry, nameof(entry.InstallLocation)));

            // Look for entries on drives, based on info in registry. Need to check for duplicates with other entries later
            callback(new GetUninstallerListProgress(2, totalStepCount));
            var driveFactory = new DirectoryFactory(registryResults);
            var driveResults = driveFactory.GetUninstallerEntries();

            // Get misc entries that use fancy logic
            callback(new GetUninstallerListProgress(3, totalStepCount));
            var otherResults = GetMiscUninstallerEntries();

            // Handle duplicate entries
            callback(new GetUninstallerListProgress(4, totalStepCount));
            var infoAdder = new InfoAdderManager();
            var mergedResults = registryResults.ToList();
            mergedResults = MergeResults(mergedResults, otherResults, infoAdder);
            // Make sure to merge driveResults last
            mergedResults = MergeResults(mergedResults, driveResults, infoAdder);

            // Fill in any missing information
            callback(new GetUninstallerListProgress(5, totalStepCount));
            foreach (var result in mergedResults)
            {
                infoAdder.AddMissingInformation(result);
                result.IsValid = CheckIsValid(result, msiProducts);
            }

            callback(new GetUninstallerListProgress(6, totalStepCount));
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
            var isPathRooted = Path.IsPathRooted(target.UninstallerFullFilename);

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

        // todo move to a second thread?
        private static IEnumerable<ApplicationUninstallerEntry> GetMiscUninstallerEntries()
        {
            var otherResults = new List<ApplicationUninstallerEntry>();
            var miscFactories = new IUninstallerFactory[]
            {
                new PredefinedFactory(),
                new SteamFactory(),
                new StoreAppFactory(),
                new WindowsFeatureFactory()
            };
            foreach (var uninstallerFactory in miscFactories)
            {
                try
                {
                    otherResults.AddRange(uninstallerFactory.GetUninstallerEntries());
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