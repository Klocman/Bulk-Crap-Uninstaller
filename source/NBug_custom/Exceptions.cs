// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Exceptions.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using NBug.Core.Reporting;
using NBug.Core.Util;

namespace NBug
{
    public static class Exceptions
    {
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