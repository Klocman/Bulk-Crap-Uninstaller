/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

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
using TestStack.White.WindowsAPI;
using Debug = System.Diagnostics.Debug;

namespace UninstallerAutomatizer
{
    public static class AutomatedUninstallManager
    {
        private const string NsisForwardAutomationId = "1";
        private const string NsisCancelAutomationId = "2";
        private const string NsisYesAutomationId = "6";
        private const string NsisNoAutomationId = "7";
        private static readonly string[] GoodButtonNames = { "Uninstall", "OK", "Accept", "Apply", "Close", "Yes" };
        private static readonly string[] ControlBoxButtonNames = { "Minimize", "Maximize", "Close" };

        /// <summary>
        ///     Automate uninstallation of an NSIS uninstaller.
        /// </summary>
        /// <param name="uninstallerCommand">Command line used to launch the NSIS uninstaller. (Usually path to uninstall.exe.)</param>
        /// <param name="statusCallback">Information about the process is relayed here</param>
        public static void UninstallNsisQuietly(string uninstallerCommand, Action<string> statusCallback)
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
                statusCallback("Waiting for NSIS uninstaller to extract...");
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
                        .Where(x => x.ProcessName.Length == 3 && x.ProcessName.EndsWith("u_", StringComparison.Ordinal))
                        .OrderByDescending(x => x.StartTime).First();
                    app = Application.Attach(uninstallProcess);
                }
                statusCallback("Attached to " + app.Name);

                WaitForApplication(app);

                // Use UI item counts to identify TODO Check using something better
                var seenWindows = new List<int>();

                while (!app.HasExited)
                {
                    statusCallback("Looking for windows...");
                    // NSIS uninstallers always have only one window open (by default)
                    var windows = app.GetWindows();
                    var target = windows.First();

                    statusCallback($"Found window \"{target.Title}\"");
                    WaitForWindow(target);

                    // BUG target.IsClosed changes to true if window gets minimized?
                    while (!target.IsClosed)
                    {
                        TryClickNextNsisButton(target, statusCallback);
                        WaitForWindow(target);

                        ProcessNsisPopups(app, target, seenWindows, statusCallback);
                    }
                    statusCallback("Window closed");

                    WaitForApplication(app);
                }
            }
            catch (Exception e)
            {
                throw new AutomatedUninstallException("Automatic uninstallation failed", e,
                    uninstallerCommand, app?.Process ?? pr);
            }
        }

        /// <summary>
        ///     Wait for the application to become ready for input.
        /// </summary>
        private static void WaitForApplication(Application app)
        {
            app.WaitWhileBusyAndAlive();
            Thread.Sleep(100);
        }

        /// <summary>
        ///     Wait for the window to become ready for input.
        /// </summary>
        private static void WaitForWindow(Window target)
        {
            target.WaitWhileBusy();
            Thread.Sleep(100);
        }

        private static void ProcessNsisPopups(Application app, Window mainWindow, ICollection<int> seenWindows, Action<string> statusCallback)
        {
            // Check for popups, they are opened as an extra window.
            var currentWindows = app.GetWindows();
            var popupWindow = currentWindows.SingleOrDefault(x => !x.Equals(mainWindow));

            if (popupWindow == null) return;

            if (seenWindows.Contains(popupWindow.Items.Count))
                throw new InvalidOperationException("Reoccuring popup window detected");
            seenWindows.Add(popupWindow.Items.Count);
            statusCallback($"Found a pop-up window - \"{popupWindow.Title}\"");

            while (!popupWindow.IsClosed)
            {
                TryClickNextNsisButton(popupWindow, statusCallback);

                popupWindow.WaitWhileBusy();
                Thread.Sleep(100);
            }
            statusCallback("Pop-up window closed");
        }
        
        private static void TryClickNextNsisButton(IUIItemContainer target, Action<string> statusCallback)
        {
            statusCallback("Looking for buttons to press...");
            var badButtons = new[] { NsisCancelAutomationId, NsisNoAutomationId };
            var goodButtons = new[] { NsisForwardAutomationId, NsisYesAutomationId };

            var allButtons =
                target.GetMultiple(SearchCriteria.ByControlType(typeof(Button), WindowsFramework.Win32))
                    .Cast<Button>();

            // Filter out buttons that should not be pressed like "Cancel".
            var filteredButtons = allButtons.Where(x => x.Enabled).Where(NotControlBoxButton).ToList();

            var buttons = filteredButtons.Count > 1
                ? filteredButtons
                    .Where(x => !badButtons.Any(y => x.Id.Equals(y, StringComparison.InvariantCulture)))
                    .ToList()
                : filteredButtons;

            if (buttons.Any())
            {
                var nextButton = buttons.FirstOrDefault(x =>
                             goodButtons.Any(
                                 y => x.Id.Equals(y, StringComparison.InvariantCulture)));

                if (nextButton == null)
                {
                    nextButton = TryGetByName(buttons);

                    if (nextButton == null)
                    {
                        Debug.Fail("Nothing to press!");
                        return;
                    }
                }

                // Finally press the button, doesn't require messing with the mouse.
                //nextButton.RaiseClickEvent();

                statusCallback("Clicking on the \"" + nextButton.Text + "\" button.");
                nextButton.Focus();
                nextButton.KeyIn(KeyboardInput.SpecialKeys.RETURN);
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