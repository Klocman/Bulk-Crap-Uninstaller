// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BugReport.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Xml.Serialization;
using NBug.Core.Reporting.Info;
using NBug.Core.Reporting.MiniDump;
using NBug.Core.UI;
using NBug.Core.Util;
using NBug.Core.Util.Logging;
using NBug.Core.Util.Serialization;
using NBug.Core.Util.Storage;

namespace NBug.Core.Reporting
{
    internal class BugReport
    {
        /// <summary>
        ///     First parameters is the serializable exception object that is about to be processed, second parameter is any custom
        ///     data
        ///     object that the user wants to include in the report.
        /// </summary>
        internal static event Action<Exception, Report> ProcessingException;

        internal ExecutionFlow Report(Exception exception, ExceptionThread exceptionThread)
        {
            try
            {
                Logger.Trace("Starting to generate a bug report for the exception.");
                var serializableException = new SerializableException(exception);
                var report = new Report(serializableException);

                var handler = ProcessingException;
                if (handler != null)
                {
                    Logger.Trace("Notifying the user before handling the exception.");

                    // Allowing user to add any custom information to the report
                    handler(exception, report);
                }

                var uiDialogResult = UISelector.DisplayBugReportUI(exceptionThread, serializableException, report);
                if (uiDialogResult.Report == SendReport.Send)
                {
                    CreateReportZip(serializableException, report);
                }

                return uiDialogResult.Execution;
            }
            catch (Exception ex)
            {
                Logger.Error(
                    "An exception occurred during bug report generation process. See the inner exception for details.",
                    ex);
                return ExecutionFlow.BreakExecution; // Since an internal exception occured
            }
        }
        
        private void AddAdditionalFiles(ZipStorer zipStorer)
        {
            foreach (var mask in Settings.AdditionalReportFiles)
            {
                // Join before spliting because the mask may have some folders inside it
                var fullPath = Path.Combine(Settings.NBugDirectory, mask);
                var dir = Path.GetDirectoryName(fullPath);
                var file = Path.GetFileName(fullPath);

                if (!Directory.Exists(dir))
                {
                    continue;
                }

                if (file.Contains("*") || file.Contains("?"))
                {
                    foreach (var item in Directory.GetFiles(dir!, file))
                    {
                        AddToZip(zipStorer, Settings.NBugDirectory, item);
                    }
                }
                else
                {
                    AddToZip(zipStorer, Settings.NBugDirectory, fullPath);
                }
            }
        }
        
        private void AddToZip(ZipStorer zipStorer, string basePath, string path)
        {
            path = Path.GetFullPath(path);

            // If this is not inside basePath, lets change the basePath so at least some directories are kept
            if (!path.StartsWith(basePath))
            {
                basePath = Path.GetDirectoryName(path);
            }

            if (Directory.Exists(path))
            {
                foreach (var file in Directory.GetFiles(path))
                {
                    AddToZip(zipStorer, basePath, file);
                }

                foreach (var dir in Directory.GetDirectories(path))
                {
                    AddToZip(zipStorer, basePath, dir);
                }
            }
            else if (File.Exists(path))
            {
                var nameInZip = path.Substring(basePath.Length);
                if (nameInZip.StartsWith("\\") || nameInZip.StartsWith("/"))
                {
                    nameInZip = nameInZip.Substring(1);
                }

                nameInZip = Path.Combine("files", nameInZip);

                zipStorer.AddFile(ZipStorer.Compression.Deflate, path, nameInZip, string.Empty);
            }
        }

        private void CreateReportZip(SerializableException serializableException, Report report)
        {
            // Test if it has NOT been more than x many days since entry assembly was last modified)
            if (Settings.StopReportingAfter < 0
                ||
                File.GetLastWriteTime(Settings.EntryAssembly.Location)
                    .AddDays(Settings.StopReportingAfter)
                    .CompareTo(DateTime.Now) > 0)
            {
                // Test if there is already more than enough queued report files
                if (Settings.MaxQueuedReports < 0 || Storer.GetReportCount() < Settings.MaxQueuedReports)
                {
                    var reportFileName = "Exception_" + DateTime.UtcNow.ToFileTime() + ".zip";
                    var minidumpFilePath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        "Exception_MiniDump_" + DateTime.UtcNow.ToFileTime() + ".mdmp");

                    using (var storer = new Storer())
                    using (var zipStorer = ZipStorer.Create(storer.CreateReportFile(reportFileName), string.Empty))
                    using (var stream = new MemoryStream())
                    {
                        // Store the exception
                        var serializer = XmlSerializer.FromTypes(new[] {typeof (SerializableException)})[0];
                        //var serializer = new XmlSerializer(typeof(SerializableException));
                        serializer.Serialize(stream, serializableException);
                        stream.Position = 0;
                        zipStorer.AddStream(ZipStorer.Compression.Deflate, StoredItemFile.Exception, stream,
                            DateTime.UtcNow, string.Empty);

                        // Store the report
                        stream.SetLength(0);

                        try
                        {
                            serializer = report.CustomInfo != null
                                ? new XmlSerializer(typeof (Report), new[] {report.CustomInfo.GetType()})
                                : new XmlSerializer(typeof (Report));

                            serializer.Serialize(stream, report);
                        }
                        catch (Exception exception)
                        {
                            Logger.Error(
                                string.Format(
                                    "The given custom info of type [{0}] cannot be serialized. Make sure that given type and inner types are XML serializable.",
                                    report.CustomInfo.GetType()),
                                exception);
                            report.CustomInfo = null;
                            serializer = new XmlSerializer(typeof (Report));
                            serializer.Serialize(stream, report);
                        }

                        stream.Position = 0;
                        zipStorer.AddStream(ZipStorer.Compression.Deflate, StoredItemFile.Report, stream,
                            DateTime.UtcNow, string.Empty);

                        // Add the memory minidump to the report file (only if configured so)
                        if (DumpWriter.Write(minidumpFilePath))
                        {
                            zipStorer.AddFile(ZipStorer.Compression.Deflate, minidumpFilePath, StoredItemFile.MiniDump,
                                string.Empty);
                            File.Delete(minidumpFilePath);
                        }

                        // Add any user supplied files in the report (if any)
                        if (Settings.AdditionalReportFiles.Count != 0)
                        {
                            AddAdditionalFiles(zipStorer);
                        }
                    }

                    Logger.Trace(
                        "Created a new report file. Currently the number of report files queued to be send is: " +
                        Storer.GetReportCount());
                }
                else
                {
                    Logger.Trace(
                        "Current report count is at its limit as per 'Settings.MaxQueuedReports (" +
                        Settings.MaxQueuedReports
                        + ")' setting: Skipping bug report generation.");
                }
            }
            else
            {
                Logger.Trace(
                    "As per setting 'Settings.StopReportingAfter(" + Settings.StopReportingAfter
                    +
                    ")', bug reporting feature was enabled for a certain amount of time which has now expired: Bug reporting is now disabled.");
                
                if (Storer.GetReportCount() > 0)
                {
                    Logger.Trace(
                        "As per setting 'Settings.StopReportingAfter(" + Settings.StopReportingAfter
                        +
                        ")', bug reporting feature was enabled for a certain amount of time which has now expired: Truncating all expired bug reports.");
                    Storer.TruncateReportFiles(0);
                }
            }
        }
    }
}