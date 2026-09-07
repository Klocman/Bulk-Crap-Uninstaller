// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Handler.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32.Interop;
using NBug.Core.Reporting;
using NBug.Core.UI;
using NBug.Core.Util;
using NBug.Core.Util.Logging;
using Dispatcher = NBug.Core.Submission.Dispatcher;

namespace NBug
{
    public static class Handler
    {
        static Handler()
        {
            // Submit any queued reports on a seperate thread asynchronously, while exceptions handlers are being set);
            if (!Settings.SkipDispatching)
            {
                new Dispatcher(Settings.DispatcherIsAsynchronous);
            }
        }

        // Using delegates to make sure that static constructor gets called on delegate access

        /// <summary>
        ///     Used for handling WinForms exceptions bound to the UI thread.
        ///     Handles the <see cref="Application.ThreadException" /> events in <see cref="System.Windows.Forms" /> namespace.
        /// </summary>
        public static ThreadExceptionEventHandler ThreadException
        {
            get
            {
                {
                    return ThreadExceptionHandler;
                }
            }
        }

        /// <summary>
        ///     Used for handling general exceptions bound to the main thread.
        ///     Handles the <see cref="AppDomain.UnhandledException" /> events in <see cref="System" /> namespace.
        /// </summary>
        public static UnhandledExceptionEventHandler UnhandledException
        {
            get { return UnhandledExceptionHandler; }
        }

        /// <summary>
        ///     Used for handling WinForms exceptions bound to the UI thread.
        ///     Handles the <see cref="Application.ThreadException" /> events in <see cref="System.Windows.Forms" /> namespace.
        /// </summary>
        /// <param name="sender">Exception sender object.</param>
        /// <param name="e">Real exception is in: e.Exception</param>
        private static void ThreadExceptionHandler(object sender, ThreadExceptionEventArgs e)
        {
            if (HandleOutdatedWindowsCrash(e.Exception)) return;
            if (HandleMissingApplicationFiles(e.Exception)) return;

            if (Settings.HandleExceptions)
            {
                Logger.Trace("Starting to handle a System.Windows.Forms.Application.ThreadException.");

                // WinForms UI thread exceptions do not propagate to more general handlers unless: Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
                var executionFlow = new BugReport().Report(e.Exception, ExceptionThread.UI_WinForms);
                if (executionFlow == ExecutionFlow.BreakExecution)
                {
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>
        ///     Used for handling general exceptions bound to the main thread.
        ///     Handles the <see cref="AppDomain.UnhandledException" /> events in <see cref="System" /> namespace.
        /// </summary>
        /// <param name="sender">Exception sender object.</param>
        /// <param name="e">Real exception is in: ((Exception)e.ExceptionObject)</param>
        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;

            if (HandleOutdatedWindowsCrash(exception)) return;
            if (HandleMissingApplicationFiles(exception)) return;

            if (Settings.HandleExceptions)
            {
                Logger.Trace("Starting to handle a System.AppDomain.UnhandledException.");
                var executionFlow = new BugReport().Report(exception, ExceptionThread.Main);
                if (executionFlow == ExecutionFlow.BreakExecution)
                {
                    Environment.Exit(0);
                }
            }
        }

        private static bool HandleOutdatedWindowsCrash(Exception dnfe)
        {
            // DllNotFoundException, EntryPointNotFoundException, possibly others
            if (dnfe != null)
            {
                if (dnfe.Message.Contains(@"'api-ms-win-core-com-l1-1-0.dll'"))
                {
                    MessageBox.Show("It seems like you're running an unsupported version of Windows. Please make sure you have all of the latest Windows service packs and updates installed and try again.\n\n" +
                                    "If updating didn't help you may need to use an older version of BCUninstaller. Check the README.md file for more information.",
                                    "Unsupported Windows Version", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Do not let NBug handle this, it only clogs up the error reports
                    Environment.Exit((int)ResultWin32.ERROR_DLL_NOT_FOUND);
                    return true;
                }
            }

            return false;
        }

        private static bool HandleMissingApplicationFiles(Exception ex)
        {
            if (ex != null)
            {
                // Unwrap first
                if (ex is AggregateException agex)
                {
                    foreach (var inner in agex.Flatten().InnerExceptions)
                    {
                        if (inner is FileNotFoundException or FileLoadException)
                        {
                            ex = inner;
                            break;
                        }
                    }
                }
                else if (ex is InvalidOperationException iopex && iopex.InnerException is not null)
                    ex = iopex.InnerException;

                // If the exception is because of a missing file it's most likely because of an AV or other security software
                if (ex is FileNotFoundException fnfex)
                    return Handle(fnfex.FileName);
                // Usually caused by AVs blocking access
                if (ex is FileLoadException flex)
                    return Handle(flex.FileName);
            }

            return false;


            bool Handle(string filename)
            {
                // Handle missing assemblies which are referred by qualified name
                if (Regex.IsMatch(filename, @"^\w[\w\.]*, Version="))
                {
                    ShowMessage(filename.Split(',')[0]);
                    return true;
                }
                // Handle missing files which are referred by path
                // Ensure the file is inside the app directory to not catch unrelated exceptions
                if (string.Equals(Path.GetDirectoryName(filename), NBug.Settings.NBugDirectory, StringComparison.OrdinalIgnoreCase))
                {
                    ShowMessage(filename);
                    return true;
                }

                return false;
            }

            void ShowMessage(string filename)
            {
                MessageBox.Show("It seems like some of the BCUninstaller files are missing or inaccessible. " +
                                "Please make sure you have all of the BCUninstaller files in the same directory and " +
                                "that your antivirus or other software is not preventing BCU from running.\n\n" +
                                "Could not access file: " + filename,
                    "Missing or inaccessible BCUninstaller files", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Do not let NBug handle this, it only clogs up the error reports
                Environment.Exit((int)ResultWin32.ERROR_DLL_NOT_FOUND);
            }
        }
    }
}