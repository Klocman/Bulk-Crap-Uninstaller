// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UISelector.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using NBug.Core.Reporting.Info;
using NBug.Core.UI.Console;
using NBug.Core.UI.Custom;
using NBug.Core.UI.WinForms;
using NBug.Core.UI.WPF;
using NBug.Core.Util;
using NBug.Core.Util.Exceptions;
using NBug.Core.Util.Serialization;
using NBug.Enums;

namespace NBug.Core.UI
{
    /// <summary>
    ///     Initializes a new instance of the UISelector class which displays the user an appropriate user interface in the
    ///     event of unhandled exceptions.
    /// </summary>
    internal static class UISelector
    {
        internal static UIDialogResult DisplayBugReportUI(ExceptionThread exceptionThread,
            SerializableException serializableException, Report report)
        {
            if (exceptionThread == ExceptionThread.Task)
            {
                // Do not interfere with the default behaviour for continuation on background thread exceptions. Just log and send'em (no UI...)
                return new UIDialogResult(ExecutionFlow.ContinueExecution, SendReport.Send);
            }
            if (Settings.UIMode == UIMode.Auto)
            {
                // First of, test to see if the call is from an UI thread and if so, use the same UI type (WinForms, WPF, etc.)
                if (exceptionThread == ExceptionThread.UI_WinForms)
                {
                    return WinFormsUI.ShowDialog(UIMode.Minimal, serializableException, report);
                }
                if (exceptionThread == ExceptionThread.UI_WPF)
                {
                    return WPFUI.ShowDialog(UIMode.Minimal, serializableException, report);
                }
                if (exceptionThread == ExceptionThread.Main)
                {
                    // If the call is not from a non-UI thread like the main app thread, it may be from the current appdomain but
                    // the application may still be using an UI. Or it may be coming from an exception filter where UI type is undefined yet.
                    switch (DiscoverUI())
                    {
                        case UIProvider.WinForms:
                            return WinFormsUI.ShowDialog(UIMode.Minimal, serializableException, report);

                        case UIProvider.WPF:
                            return WPFUI.ShowDialog(UIMode.Minimal, serializableException, report);

                        case UIProvider.Console:
                            return ConsoleUI.ShowDialog(UIMode.Minimal, serializableException, report);

                        case UIProvider.Custom:
                            return CustomUI.ShowDialog(UIMode.Minimal, serializableException, report);

                        default:
                            throw new NBugRuntimeException("UISelector.DiscoverUI() returned an invalid UI type.");
                    }
                }
                throw new NBugRuntimeException(string.Format("Parameter supplied for '{0}' is not valid.",
                    typeof (ExceptionThread).Name));
            }
            if (Settings.UIMode == UIMode.None)
            {
                // Do not display an UI for UIMode.None
                if (Settings.ExitApplicationImmediately)
                {
                    return new UIDialogResult(ExecutionFlow.BreakExecution, SendReport.Send);
                }
                return new UIDialogResult(ExecutionFlow.ContinueExecution, SendReport.Send);
            }
            if (Settings.UIProvider == UIProvider.Console)
            {
                return ConsoleUI.ShowDialog(Settings.UIMode, serializableException, report);
            }
            if (Settings.UIProvider == UIProvider.WinForms)
            {
                return WinFormsUI.ShowDialog(Settings.UIMode, serializableException, report);
            }
            if (Settings.UIProvider == UIProvider.WPF)
            {
                return WPFUI.ShowDialog(Settings.UIMode, serializableException, report);
            }
            if (Settings.UIProvider == UIProvider.Custom)
            {
                return CustomUI.ShowDialog(UIMode.Minimal, serializableException, report);
            }
            if (Settings.UIProvider == UIProvider.Auto)
            {
                // In this case, UIProvider = Auto & UIMode != Auto so just discover the UI provider and use the selected UI mode
                switch (DiscoverUI())
                {
                    case UIProvider.WinForms:
                        return WinFormsUI.ShowDialog(Settings.UIMode, serializableException, report);

                    case UIProvider.WPF:
                        return WPFUI.ShowDialog(Settings.UIMode, serializableException, report);

                    case UIProvider.Console:
                        return ConsoleUI.ShowDialog(Settings.UIMode, serializableException, report);

                    case UIProvider.Custom:
                        return CustomUI.ShowDialog(UIMode.Minimal, serializableException, report);

                    default:
                        throw new NBugRuntimeException("UISelector.DiscoverUI() returned an invalid UI type.");
                }
            }
            throw NBugConfigurationException.Create(() => Settings.UIProvider,
                "Parameter supplied for settings property is invalid.");
        }

        internal static void DisplayFeedbackUI()
        {
        }

        private static UIProvider DiscoverUI()
        {
            // First of search for loaded assemblies in the current domain
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                switch (assembly.GetName().Name)
                {
                    case "System.Windows.Forms":
                        return UIProvider.WinForms;

                    case "PresentationFramework":
                        return UIProvider.WPF;
                }
            }

            // Eventhough UI assemblies may not be loaded, they may still be referenced. Search for them for a second time
            foreach (var assembly in Settings.EntryAssembly.GetReferencedAssemblies())
            {
                switch (assembly.Name)
                {
                    case "System.Windows.Forms":
                        return UIProvider.WinForms;

                    case "PresentationFramework":
                        return UIProvider.WPF;
                }
            }

            // If there is no known UI assembly loaded or referenced, the application is probably a console app
            return UIProvider.Console;
        }
    }
}