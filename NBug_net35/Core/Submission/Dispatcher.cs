// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Dispatcher.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using NBug.Core.Reporting.Info;
using NBug.Core.Util.Logging;
using NBug.Core.Util.Serialization;
using NBug.Core.Util.Storage;

namespace NBug.Core.Submission
{
    //using System.Threading.Tasks;

    internal class Dispatcher
    {
        /// <summary>
        ///     Initializes a new instance of the Dispatcher class to send queued reports.
        /// </summary>
        /// <param name="isAsynchronous">
        ///     Decides whether to start the dispatching process asynchronously on a background thread.
        /// </param>
        internal Dispatcher(bool isAsynchronous)
        {
            // Test if it has NOT been more than x many days since entry assembly was last modified)
            // This is the exact verifier code in the BugReport.cs of CreateReportZip() function
            if (Settings.StopReportingAfter < 0
                ||
                File.GetLastWriteTime(Settings.EntryAssembly.Location)
                    .AddDays(Settings.StopReportingAfter)
                    .CompareTo(DateTime.Now) > 0)
            {
                if (isAsynchronous)
                {
                    // Log and swallow NBug's internal exceptions by default
                    new Thread(Dispatch).Start();
                    /*
					Task.Factory.StartNew(this.Dispatch)
					    .ContinueWith(
						    t => Logger.Error("An exception occurred while dispatching bug report. Check the inner exception for details", t.Exception), 
						    TaskContinuationOptions.OnlyOnFaulted);*/
                }
                else
                {
                    try
                    {
                        Dispatch();
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(
                            "An exception occurred while dispatching bug report. Check the inner exception for details.",
                            exception);
                    }
                }
            }
        }

        private void Dispatch()
        {
            // Make sure that we are not interfering with the crucial startup work);
            if (!Settings.RemoveThreadSleep)
            {
                Thread.Sleep(Settings.SleepBeforeSend*1000);
            }

            // Truncate extra report files and try to send the first one in the queue
            Storer.TruncateReportFiles();

            // Now go through configured destinations and submit to all automatically
            for (var hasReport = true; hasReport;)
            {
                using (var storer = new Storer())
                using (var stream = storer.GetFirstReportFile())
                {
                    if (stream != null)
                    {
                        // Extract crash/exception report data from the zip file. Delete the zip file if no data can be retrieved (i.e. corrupt file)
                        ExceptionData exceptionData;
                        try
                        {
                            exceptionData = GetDataFromZip(stream);
                        }
                        catch (Exception exception)
                        {
                            storer.DeleteCurrentReportFile();
                            Logger.Error(
                                "An exception occurred while extraction report data from zip file. Check the inner exception for details.",
                                exception);
                            return;
                        }

                        // Now submit the report file to all configured bug report submission targets
                        if (EnumerateDestinations(stream, exceptionData) == false)
                        {
                            break;
                        }

                        // Delete the file after it was sent
                        storer.DeleteCurrentReportFile();
                    }
                    else
                    {
                        hasReport = false;
                    }
                }
            }
        }

        /// <summary>
        ///     Enumerate all protocols to see if they are properly configured and send using the ones that are configured
        ///     as many times as necessary.
        /// </summary>
        /// <param name="reportFile">The file to read the report from.</param>
        /// <returns>
        ///     Returns <see langword="true" /> if the sending was successful.
        ///     Returns <see langword="true" /> if the report was submitted to at least one destination.
        /// </returns>
        private bool EnumerateDestinations(Stream reportFile, ExceptionData exceptionData)
        {
            var sentSuccessfullyAtLeastOnce = false;
            var fileName = Path.GetFileName(((FileStream) reportFile).Name);
            foreach (var destination in Settings.Destinations)
            {
                try
                {
                    Logger.Trace(string.Format("Submitting bug report via {0}.", destination.GetType().Name));
                    if (destination.Send(fileName, reportFile, exceptionData.Report, exceptionData.Exception))
                    {
                        sentSuccessfullyAtLeastOnce = true;
                    }
                }
                catch (Exception exception)
                {
                    Logger.Error(
                        string.Format(
                            "An exception occurred while submitting bug report with {0}. Check the inner exception for details.",
                            destination.GetType().Name),
                        exception);
                }
            }

            return sentSuccessfullyAtLeastOnce;
        }

        private ExceptionData GetDataFromZip(Stream stream)
        {
            var results = new ExceptionData();
            var zipStorer = ZipStorer.Open(stream, FileAccess.Read);
            using (Stream zipItemStream = new MemoryStream())
            {
                var zipDirectory = zipStorer.ReadCentralDir();
                foreach (var entry in zipDirectory)
                {
                    if (Path.GetFileName(entry.FilenameInZip) == StoredItemFile.Exception)
                    {
                        zipItemStream.SetLength(0);
                        zipStorer.ExtractFile(entry, zipItemStream);
                        zipItemStream.Position = 0;
                        var deserializer = XmlSerializer.FromTypes(new[] {typeof (SerializableException)})[0];
                        //var deserializer = new XmlSerializer(typeof(SerializableException));
                        results.Exception = (SerializableException) deserializer.Deserialize(zipItemStream);
                        zipItemStream.Position = 0;
                    }
                    else if (Path.GetFileName(entry.FilenameInZip) == StoredItemFile.Report)
                    {
                        zipItemStream.SetLength(0);
                        zipStorer.ExtractFile(entry, zipItemStream);
                        zipItemStream.Position = 0;
                        var deserializer = XmlSerializer.FromTypes(new[] {typeof (Report)})[0];
                        //var deserializer = new XmlSerializer(typeof(Report));
                        results.Report = (Report) deserializer.Deserialize(zipItemStream);
                        zipItemStream.Position = 0;
                    }
                }
            }

            return results;
        }

        private class ExceptionData
        {
            public SerializableException Exception { get; set; }
            public Report Report { get; set; }
        }
    }
}