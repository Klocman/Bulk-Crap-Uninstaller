// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomUIEventArgs.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using NBug.Core.Reporting.Info;
using NBug.Core.UI;
using NBug.Core.Util.Serialization;
using NBug.Enums;

namespace NBug.Events
{
    public class CustomUIEventArgs : EventArgs
    {
        internal CustomUIEventArgs(UIMode uiMode, SerializableException exception, Report report)
        {
            UIMode = uiMode;
            Report = report;
            Exception = exception;
            Result = new UIDialogResult(ExecutionFlow.BreakExecution, SendReport.DoNotSend);
        }

        public SerializableException Exception { get; private set; }
        public Report Report { get; private set; }
        public UIDialogResult Result { get; set; }
        public UIMode UIMode { get; private set; }
    }
}