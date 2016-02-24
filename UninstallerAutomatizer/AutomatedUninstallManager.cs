using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Klocman.Tools;
using TestStack.White;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.WindowItems;

namespace UninstallerAutomatizer
{
    public static class AutomatedUninstallManager
    {
        private const string NsisForwardAutomationId = "1";
        private const string NsisCancelAutomationId = "2";
        private static readonly string[] GoodButtonNames = {"Uninstall", "OK", "Accept", "Apply", "Close"};
        private static readonly string[] ControlBoxButtonNames = {"Minimize", "Maximize", "Close"};

        /// <summary>
        ///     Automate uninstallation of an NSIS uninstaller.
        /// </summary>
        /// <param name="uninstallerCommand">Command line used to launch the NSIS uninstaller. (Usually path to uninstall.exe.)</param>
        public static void UninstallNsisQuietly(string uninstallerCommand)
        {
            Process pr = null;
            Application app = null;
            try
            {
                pr = Process.Start(uninstallerCommand);
                if (pr == null)
                    throw new IOException("Process failed to start");

                // NSIS uninstallers are first extracted by the executable to a temporary directory, and then ran from there.
                // Wait for the extracting exe to close and grab the child process that it started.
                pr.WaitForExit();

                // Attempt to get the extracted exe by looking up child processes, might not work in some cases
                var prs = ProcessTools.GetChildProcesses(pr.Id).FirstOrDefault();

                if (prs != 0)
                {
                    app = Application.Attach(prs);
                }
                else
                {
                    // Get all processes with name in format [A-Z]u_ (standard NSIS naming scheme, e.g. "Au_.exe") 
                    // and select the last one to launch. (Most likely to be ours)
                    var uninstallProcess = Process.GetProcesses()
                        .Where(x => x.ProcessName.Length == 3 && x.ProcessName.EndsWith("u_"))
                        .OrderByDescending(x => x.StartTime).First();
                    app = Application.Attach(uninstallProcess);
                }

                // Wait for the process to set up.
                app.WaitWhileBusyAndAlive();
                Thread.Sleep(100);

                while (!app.HasExited)
                {
                    // NSIS uninstallers always have only one window open (by default)
                    var windows = app.GetWindows();
                    var target = windows.Single();
                    
                    // Wait for the window to become ready for input.
                    target.WaitWhileBusy();
                    Thread.Sleep(100);

                    // target.IsClosed changes to true if window gets minimized?
                    while (!target.IsClosed)
                    {
                        var allButtons =
                            target.GetMultiple(SearchCriteria.ByControlType(typeof (Button), WindowsFramework.Win32))
                                .Cast<Button>();

                        // Filter out buttons that should not be pressed like "Cancel".
                        var buttons = allButtons.Where(x => x.Enabled)
                            .Where(NotControlBoxButton)
                            .Where(x => !x.Id.Equals(NsisCancelAutomationId, StringComparison.InvariantCulture))
                            .ToList();

                        if (buttons.Any())
                        {
                            var nextButton =
                                buttons.FirstOrDefault(
                                    x => x.Id.Equals(NsisForwardAutomationId, StringComparison.InvariantCulture));

                            if (nextButton == null)
                            {
                                nextButton = TryGetByName(buttons);

                                if (nextButton == null)
                                    continue;
                            }

                            // Finally press the button, doesn't require messing with the mouse.
                            nextButton.RaiseClickEvent();
                        }

                        target.WaitWhileBusy();
                        Thread.Sleep(100);

                        // Check for popups, they are opened as an extra window.
                        windows = app.GetWindows();
                        if (windows.Count > 1)
                            throw new InvalidOperationException("Unexpected popup window detected");
                    }

                    app.WaitWhileBusyAndAlive();
                    Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                throw new AutomatedUninstallException("Automatic uninstallation failed", e, 
                    uninstallerCommand, app?.Process ?? pr);
            }
        }

        /// <summary>
        ///     Return first button matching any of the known GoodButtonNames.
        /// </summary>
        private static Button TryGetByName(IEnumerable<Button> buttons)
        {
            return GoodButtonNames.Select(
                buttonName =>
                    buttons.FirstOrDefault(x => x.Name.Equals(buttonName, StringComparison.InvariantCultureIgnoreCase)))
                .FirstOrDefault(nextButton => nextButton != null);
        }

        /// <summary>
        ///     Tests if the button is not a part of the control box.
        /// </summary>
        /// <param name="x">Button to test</param>
        private static bool NotControlBoxButton(Button x)
        {
            var id = x.Id;
            return !ControlBoxButtonNames.Any(y => id.Equals(y));
        }

        public class AutomatedUninstallException : Exception
        {
            public AutomatedUninstallException(string message, Exception innerException, string uninstallerCommand,
                Process uninstallerProcess)
                : base(message, innerException)
            {
                UninstallerCommand = uninstallerCommand;
                UninstallerProcess = uninstallerProcess;
            }

            public string UninstallerCommand { get; }
            public Process UninstallerProcess { get; }
        }
    }
}