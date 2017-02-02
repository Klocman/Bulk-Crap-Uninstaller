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
using Klocman.Native;
using Klocman.Tools;
using UninstallTools.Factory.InfoAdders;

namespace UninstallTools.Factory
{
    public static class ApplicationUninstallerFactory
    {
        /*
        public delegate void GetUninstallerListCallback(GetUninstallerListProgress progressReport);


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
        }

        /// <summary>
        ///     Gets populated automatically when running GetUninstallerList. Returns null before first update.
        /// </summary>
        private static IEnumerable<Guid> WindowsInstallerValidGuids { get;  set; }
        
        private static void PopulateWindowsInstallerValidGuids()
        {
            WindowsInstallerValidGuids = MsiTools.MsiEnumProducts();
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
            var infoAdder = new InfoAdderManager();
            var registryFactory = new RegistryFactory();

            var registryResults = registryFactory.GetUninstallerEntries().ToList();
            //todo fill in install location and uninstaller location
            // find drive stuff (based on directories found in install locations, 2 or more apps installed in one location = scan location for drive stuff)
            //results = results.DoForEach(entry => infoAdder.TryAddFieldInformation(entry, nameof(entry.InstallLocation)));
            // todo cleanup the paths
            var driveFactory = new DirectoryFactory(registryResults);

            var driveResults = driveFactory.GetUninstallerEntries().ToList();
            var otherResults = GetMiscUninstallerEntries();

            //todo combine  driveResults otherResults with existing results
            foreach (var entry in driveResults.Concat(otherResults))
            {
                try
                {
                    var matchedEntry = registryResults.SingleOrDefault(x => CheckAreEntriesRelated(x, entry));
                    if (matchedEntry != null)
                    {
                        // todo copy values using reflection, or manually based on what the drive factory does
                        continue;
                    }
                }
                catch(InvalidOperationException) { }

                registryResults.Add(entry);
            }

            foreach (var result in registryResults)
            {
                infoAdder.AddMissingInformation(result);
            }

            return registryResults;
        }

        private static bool CheckAreEntriesRelated(ApplicationUninstallerEntry baseEntry, ApplicationUninstallerEntry otherEntry)
        {
            if (!string.IsNullOrEmpty(baseEntry.InstallLocation) && !string.IsNullOrEmpty(otherEntry.InstallLocation) 
                && string.Equals(baseEntry.InstallLocation, otherEntry.InstallLocation, StringComparison.InvariantCultureIgnoreCase))
                return true;

            return !string.IsNullOrEmpty(baseEntry.UninstallerLocation) && !string.IsNullOrEmpty(otherEntry.UninstallerLocation) 
                && baseEntry.UninstallerLocation.StartsWith(otherEntry.InstallLocation, StringComparison.InvariantCultureIgnoreCase);
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

        public static string GetUninstallerFilename(string uninstallString, UninstallerType type, Guid bundleKey)
        {
            if (!string.IsNullOrEmpty(uninstallString) && !PathPointsToMsiExec(uninstallString))
            {
                try
                {
                    var fileName = ProcessTools.SeparateArgsFromCommand(uninstallString).FileName;

                    Debug.Assert(!fileName.Contains(' ') || File.Exists(fileName));

                    return fileName;
                }
                catch (ArgumentException) { }
                catch (FormatException) { }
            }

            return type == UninstallerType.Msiexec ? MsiTools.MsiGetProductInfo(bundleKey, MsiWrapper.INSTALLPROPERTY.LOCALPACKAGE) : string.Empty;
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