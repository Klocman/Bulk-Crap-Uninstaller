/*
    Copyright (c) 2018 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;
using Klocman.Extensions;

namespace UninstallTools.Factory
{
    /// <summary>
    /// Runs DirectoryFactory scan on multiple threads based physical disk layout
    /// </summary>
    internal static class DirectoryFactoryThreadedHelper
    {
        public static int MaxThreadsPerDrive = 2;

        public static IEnumerable<ApplicationUninstallerEntry> ThreadedApplicationScan(
            ListGenerationProgress.ListGenerationCallback progressCallback,
            List<string> dirsToSkip,
            List<KeyValuePair<DirectoryInfo, bool?>> itemsToScan)
        {
            var dividedItems = DivideDirectoriesBetweenDisks(itemsToScan);

            var progress = 0;

            void OnItemDone(string itemName)
            {
                progressCallback(new ListGenerationProgress(progress++, itemsToScan.Count, itemName));
            }

            var workerDatas = new List<WorkerData>();
            foreach (var itemBucket in dividedItems)
            {
                if (itemBucket.Count < 1) continue;

                var threadCount = Math.Min(MaxThreadsPerDrive, itemBucket.Count / 10 + 1);

                var threadWorkItemCount = itemBucket.Count / threadCount + 1;

                for (var i = 0; i < threadCount; i++)
                {
                    var firstUnique = i * threadWorkItemCount;
                    var workerItems = itemBucket.Skip(firstUnique).Take(threadWorkItemCount).ToList();

                    var worker = new Thread(GetUninstallerEntriesThread)
                    {
                        Name = "GetUninstallerEntries_worker",
                        IsBackground = false
                    };
                    var workerData = new WorkerData(workerItems, dirsToSkip, worker, OnItemDone);
                    workerDatas.Add(workerData);
                    worker.Start(workerData);
                }
            }

            foreach (var workerData in workerDatas)
            {
                try { workerData.Worker.Join(); }
                catch (Exception ex) { Console.WriteLine(ex); }
            }

            var results = workerDatas.Aggregate(new List<ApplicationUninstallerEntry>(),
                (entries, data) => ApplicationUninstallerFactory.MergeResults(entries, data.Results, null));

            return results;
        }

        private static void GetUninstallerEntriesThread(object obj)
        {
            var workerInterface = obj as WorkerData;
            if (workerInterface == null) throw new ArgumentNullException(nameof(workerInterface));

            workerInterface.Results = new List<ApplicationUninstallerEntry>();
            foreach (var directory in workerInterface.Input)
            {
                try { workerInterface.OnInputItemDone(directory.Key.FullName); }
                catch (OperationCanceledException) { return; }

                if (UninstallToolsGlobalConfig.IsSystemDirectory(directory.Key) ||
                    directory.Key.Name.StartsWith("Windows", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                var detectedEntries = DirectoryFactory.TryCreateFromDirectory(
                    directory.Key, directory.Value, workerInterface.DirsToSkip).ToList();

                workerInterface.Results =
                    ApplicationUninstallerFactory.MergeResults(workerInterface.Results, detectedEntries, null);
            }
        }

        private static List<List<KeyValuePair<DirectoryInfo, bool?>>> DivideDirectoriesBetweenDisks(
            List<KeyValuePair<DirectoryInfo, bool?>> itemsToScan)
        {
            var output = new List<List<KeyValuePair<DirectoryInfo, bool?>>>();
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

                    var inputList = itemsToScan.Select(x => new { x.Key.Root.Name, x }).ToList();
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
            catch (ManagementException ex)
            {
                Console.WriteLine(@"Failed to get logical disk to physical drive relationships - " + ex);
                output.Clear();
                output.Add(itemsToScan);
            }
            return output;
        }

        private sealed class WorkerData
        {
            public WorkerData(List<KeyValuePair<DirectoryInfo, bool?>> input, List<string> dirsToSkip, Thread worker,
                Action<string> onInputItemDone)
            {
                Input = input;
                DirsToSkip = dirsToSkip;
                Worker = worker;
                OnInputItemDone = onInputItemDone;
            }

            public List<KeyValuePair<DirectoryInfo, bool?>> Input { get; }
            public List<ApplicationUninstallerEntry> Results { get; set; }
            public List<string> DirsToSkip { get; }
            public Thread Worker { get; }
            public Action<string> OnInputItemDone { get; }
        }
    }
}