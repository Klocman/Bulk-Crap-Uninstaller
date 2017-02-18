// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIDialogResult.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NBug.Core.UI
{
    public enum SendReport
    {
        Send,

        DoNotSend
    }

    public enum ExecutionFlow
    {
        /// <summary>
        ///     This will handle all unhandled exceptions to be able to continue execution.
        /// </summary>
        ContinueExecution,

        /// <summary>
        ///     This will handle all unhandled exceptions and exit the application.
        /// </summary>
        BreakExecution
    }

    public struct UIDialogResult
    {
        internal ExecutionFlow Execution;
        internal SendReport Report;

        public UIDialogResult(ExecutionFlow execution, SendReport report)
        {
            Execution = execution;
            Report = report;
        }
    }
}