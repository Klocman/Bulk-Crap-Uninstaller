// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleUI.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using NBug.Core.Reporting.Info;
using NBug.Core.Util.Exceptions;
using NBug.Core.Util.Serialization;
using NBug.Enums;

namespace NBug.Core.UI.Console
{
    internal static class ConsoleUI
    {
        internal static UIDialogResult ShowDialog(UIMode uiMode, SerializableException exception, Report report)
        {
            if (uiMode == UIMode.Minimal)
            {
                // Do not interact with the user
                System.Console.WriteLine(Environment.NewLine + Settings.Resources.UI_Console_Minimal_Message);
                return new UIDialogResult(ExecutionFlow.BreakExecution, SendReport.Send);
            }
            if (uiMode == UIMode.Normal)
            {
                System.Console.WriteLine(Environment.NewLine + Settings.Resources.UI_Console_Normal_Message);
                return new UIDialogResult(ExecutionFlow.BreakExecution, SendReport.Send);
            }
            if (uiMode == UIMode.Full)
            {
                System.Console.WriteLine(Environment.NewLine + Settings.Resources.UI_Console_Full_Message);
                return new UIDialogResult(ExecutionFlow.BreakExecution, SendReport.Send);
            }
            throw NBugConfigurationException.Create(() => Settings.UIMode,
                "Parameter supplied for settings property is invalid.");
        }
    }
}