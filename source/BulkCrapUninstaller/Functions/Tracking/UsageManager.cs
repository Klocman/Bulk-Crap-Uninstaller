/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Tools;

namespace BulkCrapUninstaller.Functions.Tracking
{
    public static class UsageManager
    {
        private static readonly string StatsFilename = Program.AssemblyLocation + @"\UsageStatistics.xml";
        internal static List<UsageTracker> Trackers = new();
        private static XDocument _currentData;
        private static readonly object OperationLock = new();

        public static bool IsCollectingData { get; private set; }

        private static XDocument CurrentData
        {
            get
            {
                lock (OperationLock)
                {
                    if (_currentData == null)
                    {
                        XDocument doc;
                        try
                        {
                            doc = XDocument.Load(StatsFilename);
                            if (doc.Root == null || (doc.Root.Name != "UsageStatistics" || doc.Root.IsEmpty))
                                throw new XmlException();
                        }
                        catch
                        {
                            doc = CreateNewDataDocument();
                        }
                        _currentData = doc;

                        AppLaunchCount++;
                    }
                    return _currentData;
                }
            }
        }

        public static int AppLaunchCount
        {
            get
            {
                lock (OperationLock)
                {
                    var metadata = CurrentData.Root.GetOrCreateElement("Metadata");
                    var counter = metadata.GetOrCreateElement("LaunchCount");
                    int.TryParse(counter.Value, out var launchCount);
                    return launchCount;
                }
            }
            private set
            {
                lock (OperationLock)
                {
                    var metadata = CurrentData.Root.GetOrCreateElement("Metadata");
                    var counter = metadata.GetOrCreateElement("LaunchCount");
                    counter.SetValue(value);
                }
            }
        }

        /// <summary>
        ///     Object used to send the usage data.
        /// </summary>
        public static DatabaseStatSender DataSender { get; set; }

        /// <summary>
        ///     Finish tracking data and save results to the hard drive
        /// </summary>
        public static void FinishCollectingData()
        {
            lock (Trackers)
            {
                IsCollectingData = false;

                foreach (var tracker in Trackers)
                {
                    SerializeToXml(tracker.Hooks);
                }

                // Save to disk
                try
                {
                    CurrentData.Save(StatsFilename);
                }
                catch
                {
                    /*PremadeDialogs.GenericError(ex); // No need, not critical*/
                }
            }
        }

        public static void RemoveStoredData()
        {
            try
            {
                File.Delete(StatsFilename);
            }
            catch
            {
                //Ignore permission and read errors
            }
        }

        /// <summary>
        ///     Send usage data specified mail address
        /// </summary>
        public static void SendUsageData()
        {
            if (DataSender == null)
                throw new InvalidOperationException("DataSender must be set before sending data");

            lock (OperationLock)
            {
                DataSender.SendData(CurrentData.ToString(SaveOptions.DisableFormatting));
            }
        }

        internal static void UsageTrackerDestructionCallback(UsageTracker tracker)
        {
            lock (Trackers)
            {
                if (Trackers.Contains(tracker))
                    Trackers.Remove(tracker);

                if (IsCollectingData)
                    SerializeToXml(tracker.Hooks);
            }
        }

        private static XDocument CreateNewDataDocument()
        {
            var tempDoc = new XDocument(new XDeclaration("1.0", "utf-8", null), new XElement("UsageStatistics"));

            var metadata = tempDoc.Root.GetOrCreateElement("Metadata");
            metadata.Add(new XElement("ApplicationVersion", Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "Unknown"));
            metadata.Add(new XElement("SystemVersion", Environment.OSVersion.VersionString));
            metadata.Add(new XElement("Is64Bit", ProcessTools.Is64BitProcess));
            metadata.Add(new XElement("Locale", CultureInfo.InstalledUICulture.ToString()));
            //metadata.Add(new XElement("UserId", WindowsTools.GetUserSid()));

            var net = new XElement("Net", string.Join("; ", NetFrameworkTools.GetInstalledFrameworkVersions()));
            metadata.Add(net);

            return tempDoc;
        }

        private static void SerializeToXml(IEnumerable<EventHook> hooks)
        {
            lock (OperationLock)
            {
                var root = CurrentData.Root.GetOrCreateElement("InterfaceStatistics");
                foreach (var mainHook in hooks)
                {
                    var parentElement = root.GetOrCreateElement(mainHook.ParentName);
                    var fieldElement = parentElement.GetOrCreateElement(mainHook.FieldName);

                    foreach (var singleHook in mainHook.Hooks)
                    {
                        if (singleHook.HitCount <= 0)
                            continue;

                        var element = fieldElement.GetOrCreateElement(singleHook.EventName);
                        int.TryParse(element.Value, out var result);
                        element.SetValue(singleHook.HitCount + result);
                    }

                    if (fieldElement.IsEmpty)
                        fieldElement.Remove();
                }
            }
        }
    }
}