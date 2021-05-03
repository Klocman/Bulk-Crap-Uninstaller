// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Storer.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using NBug.Core.Util.Exceptions;
using NBug.Core.Util.Logging;

namespace NBug.Core.Util.Storage
{
    /// <summary>
    ///     Initializes a new instance of the Storage class. This class implements <see cref="IDisposable" /> interface
    ///     so it is better used with a using {...} statement.
    /// </summary>
    /// <remarks>This class should be instantiated to work with a single file at once.</remarks>
    internal class Storer : IDisposable
    {
        private bool disposed;
        private IsolatedStorageFile isoStore;
        private Stream stream;
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Storer()
        {
            Dispose(false);
        }

        private static IEnumerable<string> EnumerateFiles(string directory)
        {
            return
                Directory.GetFiles(directory)
                    .Where(x => x.Substring(x.LastIndexOf('\\')).Contains("Exception_") && x.EndsWith(".zip"));
        }

        internal static int GetReportCount()
        {
            if (Settings.StoragePath == Enums.StoragePath.WindowsTemp)
            {
                var path = Path.Combine(Path.GetTempPath(), Settings.EntryAssembly.GetName().Name);
                return Directory.Exists(path) ? EnumerateFiles(path).Count() : 0;
                //.EnumerateFiles(path, "Exception_*.zip").Count() : 0;
            }
            if (Settings.StoragePath == Enums.StoragePath.CurrentDirectory)
            {
                return Directory.Exists(Settings.NBugDirectory) ? EnumerateFiles(Settings.NBugDirectory).Count() : 0;
            }
            if (Settings.StoragePath == Enums.StoragePath.IsolatedStorage)
            {
                using (
                    var isoFile =
                        IsolatedStorageFile.GetStore(
                            IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain,
                            null, null))
                {
                    return isoFile.GetFileNames("Exception_*.zip").Count();
                }
            }
            if (Settings.StoragePath == Enums.StoragePath.Custom)
            {
                var path = Path.GetFullPath(Settings.StoragePath);
                return Directory.Exists(path) ? EnumerateFiles(path).Count() : 0;
            }
            throw NBugConfigurationException.Create(() => Settings.StoragePath,
                "Parameter supplied for settings property is invalid.");
        }

        /// <summary>
        ///     This function will get rid of the oldest files first.
        /// </summary>
        internal static void TruncateReportFiles()
        {
            TruncateReportFiles(Settings.MaxQueuedReports);
        }

        /// <summary>
        ///     This function will get rid of the oldest files first.
        /// </summary>
        /// <param name="maxQueuedReports">
        ///     Maximum number of queued files to be stored. Setting this to 0 deletes all files. Settings this
        ///     to anything less than zero will store infinite number of files.
        /// </param>
        internal static void TruncateReportFiles(int maxQueuedReports)
        {
            if (maxQueuedReports < 0)
            {
            }
            else if (Settings.StoragePath == Enums.StoragePath.WindowsTemp)
            {
                var path = Path.Combine(Path.GetTempPath(), Settings.EntryAssembly.GetName().Name);
                TruncateFiles(path, maxQueuedReports);
            }
            else if (Settings.StoragePath == Enums.StoragePath.CurrentDirectory)
            {
                TruncateFiles(Settings.NBugDirectory, maxQueuedReports);
            }
            else if (Settings.StoragePath == Enums.StoragePath.IsolatedStorage)
            {
                using (
                    var isoFile =
                        IsolatedStorageFile.GetStore(
                            IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain,
                            null, null))
                {
                    var reportCount = isoFile.GetFileNames("Exception_*.zip").Count();

                    if (reportCount > maxQueuedReports)
                    {
                        var extraCount = reportCount - maxQueuedReports;

                        if (maxQueuedReports == 0)
                        {
                            Logger.Trace("Truncating all report files from the isolated storage.");
                        }
                        else
                        {
                            Logger.Trace("Truncating extra " + extraCount + " report files from the isolates storage.");
                        }

                        foreach (var file in isoFile.GetFileNames("Exception_*.zip"))
                        {
                            extraCount--;
                            File.Delete(file);

                            if (extraCount == 0)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            else if (Settings.StoragePath == Enums.StoragePath.Custom)
            {
                var path = Path.GetFullPath(Settings.StoragePath);
                TruncateFiles(path, maxQueuedReports);
            }
            else
            {
                throw NBugConfigurationException.Create(() => Settings.StoragePath,
                    "Parameter supplied for settings property is invalid.");
            }
        }

        internal Stream CreateReportFile(string reportFileName)
        {
            FileName = reportFileName;

            if (Settings.StoragePath == Enums.StoragePath.WindowsTemp)
            {
                var directoryPath = Path.Combine(Path.GetTempPath(), Settings.EntryAssembly.GetName().Name);

                if (Directory.Exists(directoryPath) == false)
                {
                    Directory.CreateDirectory(directoryPath);
                }

                FilePath = Path.Combine(directoryPath, FileName);
                Logger.Trace("Creating report file to Windows temp path: " + FilePath);
                return stream = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None);
            }
            if (Settings.StoragePath == Enums.StoragePath.CurrentDirectory)
            {
                FilePath = Path.Combine(Settings.NBugDirectory, FileName);
                Logger.Trace("Creating report file to entry assembly directory path: " + FilePath);
                return stream = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None);
            }
            if (Settings.StoragePath == Enums.StoragePath.IsolatedStorage)
            {
                FilePath = null;
                Logger.Trace("Creating report file to isolated storage path: [Isolated Storage Directory]\\" + FileName);
                return
                    stream = new IsolatedStorageFileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None);
            }
            if (Settings.StoragePath == Enums.StoragePath.Custom)
            {
                var directoryPath = Path.GetFullPath(Settings.StoragePath); // In case this is a relative path

                if (Directory.Exists(directoryPath) == false)
                {
                    Directory.CreateDirectory(directoryPath);
                }

                FilePath = Path.Combine(directoryPath, FileName);
                Logger.Trace("Creating report file to custom path: " + FilePath);
                return stream = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None);
            }
            throw NBugConfigurationException.Create(() => Settings.StoragePath,
                "Parameter supplied for settings property is invalid.");
        }

        internal void DeleteCurrentReportFile()
        {
            stream.Close();

            if (stream is IsolatedStorageFileStream)
            {
                Logger.Trace("Deleting report file from isolated storage: " + FileName);
                isoStore.DeleteFile(FileName);
            }
            else
            {
                Logger.Trace("Deleting report file from path: " + FilePath);
                File.Delete(FilePath);
            }
        }

        /// <summary>
        ///     Returns the first-in-queue report file. If there are no files queued, returns <see langword="null" />.
        /// </summary>
        /// <returns>Report file stream.</returns>
        internal Stream GetFirstReportFile()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(
                    "NBug.Utils.Storage was already destroyed and out of scope when accessed.");
            }

            if (Settings.StoragePath == Enums.StoragePath.WindowsTemp)
            {
                var path = Path.Combine(Path.GetTempPath(), Settings.EntryAssembly.GetName().Name);

                if (Directory.Exists(path) && EnumerateFiles(path).Any())
                {
                    try
                    {
                        FilePath = EnumerateFiles(path).First();
                        FileName = Path.GetFileName(FilePath);
                        return stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.None);
                    }
                    catch (IOException exception)
                    {
                        // If the file is locked (as per IOException), then probably another instance of the library is already accessing
                        // the file so let the other instance handle the file
                        Logger.Error(
                            "Cannot access the report file at Windows temp directory (it is probably locked, see the inner exception): " +
                            FilePath, exception);
                        return null;
                    }
                }
                return null;
            }
            if (Settings.StoragePath == Enums.StoragePath.CurrentDirectory)
            {
                var path = Settings.NBugDirectory;

                if (path != null && Directory.Exists(path) && EnumerateFiles(path).Any())
                {
                    try
                    {
                        FilePath = EnumerateFiles(path).First();
                        FileName = Path.GetFileName(FilePath);
                        return stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.None);
                    }
                    catch (IOException exception)
                    {
                        // If the file is locked (as per IOException), then probably another instance of the library is already accessing
                        // the file so let the other instance handle the file
                        Logger.Error(
                            "Cannot access the report file at entry assembly directory (it is probably locked, see the inner exception): " +
                            FilePath, exception);
                        return null;
                    }
                }
                return null;
            }
            if (Settings.StoragePath == Enums.StoragePath.IsolatedStorage)
            {
                isoStore =
                    IsolatedStorageFile.GetStore(
                        IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, null,
                        null);

                if (isoStore.GetFileNames("Exception_*.zip").Any())
                {
                    try
                    {
                        FilePath = null;
                        FileName = isoStore.GetFileNames("Exception_*.zip").First();
                        stream = new IsolatedStorageFileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.None);
                        return stream;
                    }
                    catch (IOException exception)
                    {
                        // If the file is locked (as per IOException), then probably another instance of the library is already accessing
                        // the file so let the other instance handle the file
                        Logger.Error(
                            "Cannot access the report file at isolated storage (it is probably locked, see the inner exception): [Isolated Storage Directory]\\"
                            + FileName,
                            exception);
                        return null;
                    }
                }
                return null;
            }
            if (Settings.StoragePath == Enums.StoragePath.Custom)
            {
                var path = Path.GetFullPath(Settings.StoragePath);

                if (Directory.Exists(path) && EnumerateFiles(path).Any())
                {
                    try
                    {
                        FilePath = EnumerateFiles(path).First();
                        FileName = Path.GetFileName(FilePath);
                        return stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.None);
                    }
                    catch (IOException exception)
                    {
                        // If the file is locked (as per IOException), then probably another instance of the library is already accessing
                        // the file so let the other instance handle the file
                        Logger.Error(
                            "Cannot access the report file from the given custom path (it is probably locked, see the inner exception): " +
                            FilePath, exception);
                        return null;
                    }
                }
                return null;
            }
            throw NBugConfigurationException.Create(() => Settings.StoragePath,
                "Parameter supplied for settings property is invalid.");
        }

        private static void TruncateFiles(string path, int maxQueuedReports)
        {
            if (Directory.Exists(path))
            {
                var reportCount = EnumerateFiles(path).Count();

                if (reportCount > maxQueuedReports)
                {
                    var extraCount = reportCount - maxQueuedReports;

                    if (maxQueuedReports == 0)
                    {
                        Logger.Trace("Truncating all report files from: " + path);
                    }
                    else
                    {
                        Logger.Trace("Truncating extra " + extraCount + " report files from: " + path);
                    }

                    foreach (var file in EnumerateFiles(path))
                    {
                        extraCount--;
                        File.Delete(file);

                        if (extraCount == 0)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    if (stream != null)
                    {
                        stream.Close();
                    }

                    if (isoStore != null)
                    {
                        isoStore.Close();
                    }
                }

                // Clean up unmanaged resources
            }

            disposed = true;
        }
    }
}