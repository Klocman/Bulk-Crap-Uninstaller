using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using BulkCrapUninstaller.Properties;
using Klocman;
using Klocman.Subsystems.Tracking;
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
            private readonly DatabaseStatSender sender;

            public NBugDatabaseSenderWrapper()
            {
                sender = new DatabaseStatSender(Resources.DbConnectionString,
                    Resources.DbCommandCrash, Properties.Settings.Default.MiscUserId);
            }

            public override bool Send(string fileName, Stream file, Report report, SerializableException exception)
            {
                report.CustomInfo = new BugReportExtraInfo();

                var data = string.Concat("<BugReport>", report.ToString(), exception.ToString(), "</BugReport>");
                return sender.SendData(CompressionTools.ZipString(data));
            }
        }
    }
}