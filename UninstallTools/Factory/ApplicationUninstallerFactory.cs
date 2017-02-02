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
using Klocman.IO;
using Klocman.Tools;
using UninstallTools.Factory.InfoAdders;

namespace UninstallTools.Factory
{
    public static class ApplicationUninstallerFactory
    {
        /*public delegate void GetUninstallerListCallback(GetUninstallerListProgress progressReport);


        public class GetUninstallerListProgress
        {
            internal GetUninstallerListProgress(int totalCount)
            {
                TotalCount = totalCount;
                CurrentCount = 0;
            }

            public int CurrentCount { get; internal set; }
            public ApplicationUninstallerEntry FinishedEntry { get; internal set; }
            public int TotalCount { get; internal set; }
        }*/

        //todo global factory
        /*
         find and instantiate all factories
         find reg stuff
         fill in install location and uninstaller location
         find drive stuff (based on directories found in install locations, 2 or more apps installed in one location = scan location for drive stuff)
         merge/skip based on install/uninstaller location
         
        */

        public static IEnumerable<ApplicationUninstallerEntry> GetUninstallerEntries()
        {
            var msiProducts = MsiTools.MsiEnumProducts().ToList();

            var registryFactory = new RegistryFactory(msiProducts);

            var registryResults = registryFactory.GetUninstallerEntries().ToList();
            //todo fill in install location and uninstaller location
            // find drive stuff (based on directories found in install locations, 2 or more apps installed in one location = scan location for drive stuff)
            //results = results.DoForEach(entry => infoAdder.TryAddFieldInformation(entry, nameof(entry.InstallLocation)));
            // todo cleanup the paths
            var driveFactory = new DirectoryFactory(registryResults);

            var driveResults = driveFactory.GetUninstallerEntries();
            var otherResults = GetMiscUninstallerEntries();

            var infoAdder = new InfoAdderManager();
            var results = registryResults.ToList();
            foreach (var entry in driveResults.Concat(otherResults))
            {
                try
                {
#if DEBUG
                    var entries = registryResults.Where(x => CheckAreEntriesRelated(x, entry)).ToList();
                    Debug.Assert(entries.Count < 2);
                    var matchedEntry = entries.SingleOrDefault();
#else
                    var matchedEntry = registryResults.SingleOrDefault(x => CheckAreEntriesRelated(x, entry));
#endif
                    if (matchedEntry != null)
                    {
                        infoAdder.CopyMissingInformation(matchedEntry, entry);
                        continue;
                    }
                }
                catch (InvalidOperationException) { }

                // If not matched to anything, add the entry to the results
                results.Add(entry);
            }

            foreach (var result in results)
            {
                infoAdder.AddMissingInformation(result);
                result.IsValid = CheckIsValid(result, msiProducts);
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

            // Check if publisher and display name are very similar
            if (baseEntry.Publisher != null && baseEntry.DisplayName != null
                && baseEntry.DisplayName.Length >= 5 && baseEntry.Publisher.Length >= 5
                && otherEntry.Publisher != null && otherEntry.DisplayName != null)
            {
                var dispSim = StringTools.CompareSimilarity(baseEntry.DisplayName, otherEntry.DisplayName);
                var pubSim = StringTools.CompareSimilarity(baseEntry.Publisher, otherEntry.Publisher);

                if (dispSim < baseEntry.DisplayName.Length / 6 && pubSim < baseEntry.Publisher.Length / 6)
                    return true;
            }

            return false;
        }

        // todo move to a second thread
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
                otherResults.AddRange(uninstallerFactory.GetUninstallerEntries());
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