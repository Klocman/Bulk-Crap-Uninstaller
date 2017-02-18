// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Exceptions.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using NBug.Core.Reporting;
using NBug.Core.Reporting.MiniDump;
using NBug.Core.Util;

namespace NBug
{
    public static class Exceptions
    {
        /// <summary>
        ///     This function acts as an exception filter for any exception that is raised from within the action body (you can see
        ///     MSDN subject "Exception Filters" to get more info on the subject). As the name implies, exceptions raised from
        ///     within
        ///     the action block is simply filtered to be sent as an error report, and never actually caught or handled. Filters
        ///     all
        ///     the exceptions inside the action body and queues an error report. Note that the exceptions are not actually
        ///     handled,
        ///     but filtered, so if the exception is left unhandled in an upper block, it will crash the application. This is very
        ///     useful for situations where you need to log exceptions inside a code block and get a good minidump of the
        ///     exception.
        ///     Use the <see cref="Handle(bool,Action)" /> method to actually handle the exception and show an exception dialog to
        ///     the
        ///     user and shut down the application gracefully (if set so). You can simply use
        ///     <c>Filter(() => { MyCodeHere(); })</c>
        /// </summary>
        /// <param name="body">Body of code to be executed.</param>
        public static void Filter(Action body)
        {
            ExceptionFilters.Filter(body, ex => new BugReport().Report(ex, ExceptionThread.Main));
        }

        /// <summary>
        ///     Similar to <see cref="Filter(Action)" /> but this time, exceptions are not allowed to escape the action body and
        ///     they are
        ///     simply swallowed after being queued for reporting, with a small UI displayed to the user (if set so). Note that
        ///     NBug can halt the execution with <c>Environment.Exit(0);</c> if you configured it to do so with
        ///     <paramref name="continueExecution" />
        ///     parameter set to <see langword="false" />. You can simply use <c>Handle(true, () => { MyCodeHere(); })</c>
        /// </summary>
        /// <param name="continueExecution">Decides whether to exit application after handling the exception or continue execution.</param>
        /// <param name="body">Body of code to be executed.</param>
        public static void Handle(bool continueExecution, Action body)
        {
            ExceptionFilters.Filter(
                body,
                ex =>
                {
                    // Filtering the exception
                    new BugReport().Report(ex, ExceptionThread.Main);
                    return true; // Yes proceed to handling the exception
                },
                ex =>
                {
                    if (!continueExecution)
                    {
                        Environment.Exit(0);
                    }
                });
        }

        /// <summary>
        ///     Submits a bug report for the given exception. This function useful for submitting bug reports inside a try-catch
        ///     block.
        ///     Note that this function uses the NBug configuration so it will use the pre-configured UI and submission settings.
        /// </summary>
        /// <param name="exception">The exception to submit as the bug report.</param>
        public static void Report(Exception exception)
        {
            // Below never exits application by itself (by design) so execution of the application continues normally
            new BugReport().Report(exception, ExceptionThread.Main);
        }
    }
}