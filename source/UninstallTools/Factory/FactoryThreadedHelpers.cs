/*
    Copyright (c) 2018 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using Klocman.Extensions;

namespace UninstallTools.Factory
{
    internal static class FactoryThreadedHelpers
    {
        public static int MaxThreadsPerDrive = 2;

        public static IEnumerable<ApplicationUninstallerEntry> DriveApplicationScan(
            ListGenerationProgress.ListGenerationCallback progressCallback,
            List<string> dirsToSkip,
            List<KeyValuePair<DirectoryInfo, bool?>> itemsToScan)
        {
            var dividedItems = SplitByPhysicalDrives(itemsToScan, pair => pair.Key);

            void GetUninstallerEntriesThread(KeyValuePair<DirectoryInfo, bool?> data, List<ApplicationUninstallerEntry> state)
            {
                if (UninstallToolsGlobalConfig.IsSystemDirectory(data.Key) ||
                    data.Key.Name.StartsWith("Windows", StringComparison.InvariantCultureIgnoreCase))
                    return;

                var detectedEntries = DirectoryFactory.TryCreateFromDirectory(data.Key, data.Value, dirsToSkip).ToList();

                ApplicationUninstallerFactory.MergeResults(state, detectedEntries, null);
            }

            var workSpreader = new ThreadedWorkSpreader<KeyValuePair<DirectoryInfo, bool?>, List<ApplicationUninstallerEntry>>
                (MaxThreadsPerDrive, GetUninstallerEntriesThread, list => new List<ApplicationUninstallerEntry>(list.Count), data => data.Key.FullName);

            workSpreader.Start(dividedItems, progressCallback);
            
            var results = new List<ApplicationUninstallerEntry>();

            foreach (var workerResults in workSpreader.Join())
                ApplicationUninstallerFactory.MergeResults(results, workerResults, null);

            return results;
        }

        public static void GenerateMisingInformation(IReadOnlyCollection<ApplicationUninstallerEntry> entries,
            ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            throw new NotImplementedException();
        }
        
        private static List<List<TData>> SplitByPhysicalDrives<TData>(List<TData> itemsToScan, Func<TData, DirectoryInfo> locationGetter)
        {
            var output = new List<List<TData>>();
            try
            {
                using (var searcherDtp = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskDriveToDiskPartition"))
                using (var searcherLtp = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_LogicalDiskToPartition"))
                {
                    var dtp = searcherDtp.Get().Cast<ManagementObject>().Select(queryObj => new
                    {
                        Drive = queryObj["Antecedent"] as string,
                        Partition = queryObj["Dependent"] as string
                    });

                    var ltp = searcherLtp.Get().Cast<ManagementObject>().Select(queryObj => new
                    {
                        Partition = queryObj["Antecedent"] as string,
                        LogicalDrive = queryObj["Dependent"] as string
                    });

                    var correlatedDriveList = ltp.Join(dtp, arg => arg.Partition, arg => arg.Partition, (x, y) => new
                    {
                        LogicalName = x.LogicalDrive.Split(new[] { '"' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault()?.Append(@"\"),
                        y.Drive
                    }).Where(x => !string.IsNullOrEmpty(x.LogicalName)).GroupBy(x => x.Drive);

                    var inputList = itemsToScan.Select(x => new { locationGetter(x).Root.Name, x }).ToList();
                    foreach (var logicalDriveGroup in correlatedDriveList)
                    {
                        var filteredByPhysicalDrive = inputList.Where(x =>
                            logicalDriveGroup.Any(y =>
                                y.LogicalName.Equals(x.Name, StringComparison.OrdinalIgnoreCase))).ToList();

                        inputList.RemoveAll(filteredByPhysicalDrive);
                        output.Add(filteredByPhysicalDrive.Select(x => x.x).ToList());
                    }
                    // Bundle leftovers as a single drive
                    output.Add(inputList.Select(x => x.x).ToList());
                }
            }
            catch (SystemException ex)
            {
                Console.WriteLine(@"Failed to get logical disk to physical drive relationships - " + ex);
                output.Clear();
                output.Add(itemsToScan);
            }
            return output;
        }
    }
}