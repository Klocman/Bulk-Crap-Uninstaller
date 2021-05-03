/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions.Tracking;
using BulkCrapUninstaller.Properties;
using Klocman.Tools;
using NBug;
using NBug.Core.Reporting.Info;
using NBug.Core.Submission;
using NBug.Core.Util.Serialization;
using NBug.Enums;
using Settings = NBug.Settings;

namespace BulkCrapUninstaller
{
    /// <summary>
    ///     Can not be contained inside of a static class as it needs to be serialized
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Used during serialization")]
    public class BugReportExtraInfo
    {
        public bool Is64Bit = ProcessTools.Is64BitProcess;
        public string Locale = CultureInfo.InstalledUICulture.ToString();
        public string SystemVersion = Environment.OSVersion.VersionString;

        internal BugReportExtraInfo()
        {
        }
    }

    public static class NBugConfigurator
    {
        public static void SetupNBug()
        {
            Settings.UIMode = UIMode.Full;
            Settings.UIProvider = UIProvider.WinForms;
            Settings.SleepBeforeSend = 3;
            Settings.MaxQueuedReports = 4;
            Settings.StopReportingAfter = 10;
            Settings.MiniDumpType = MiniDumpType.None;
            Settings.WriteLogToDisk = false;
            Settings.ExitApplicationImmediately = true;
            Settings.HandleProcessCorruptedStateExceptions = false;

            Settings.ReleaseMode = true;

            Settings.Destinations.Add(new NBugDatabaseSenderWrapper());

            AppDomain.CurrentDomain.UnhandledException += Handler.UnhandledException;
            Application.ThreadException += Handler.ThreadException;
        }

        private class NBugDatabaseSenderWrapper : ProtocolBase
        {
            private readonly DatabaseStatSender _sender;

            public NBugDatabaseSenderWrapper()
            {
                _sender = new DatabaseStatSender(Program.DbConnectionString,
                    Resources.DbCommandCrash, Properties.Settings.Default.MiscUserId);
            }

            public override bool Send(string fileName, Stream file, Report report, SerializableException exception)
            {
                report.CustomInfo = new BugReportExtraInfo();

                var data = string.Concat("<BugReport>", report.ToString(), exception.ToString(), "</BugReport>");
                return _sender.SendData(CompressionTools.ZipString(data));
            }
        }
    }
}