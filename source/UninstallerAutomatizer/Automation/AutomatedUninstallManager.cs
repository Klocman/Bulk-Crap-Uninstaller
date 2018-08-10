/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using Klocman.Extensions;
using Klocman.Tools;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.WindowItems;
using TestStack.White.WindowsAPI;
using UninstallerAutomatizer.Extensions;
using UninstallerAutomatizer.Properties;
using Application = TestStack.White.Application;
using Button = TestStack.White.UIItems.Button;
using RadioButton = TestStack.White.UIItems.RadioButton;

namespace UninstallerAutomatizer
{
    public static class AutomatedUninstallManager
    {
        private const string NsisRebootNowRadioAutomationId = "1203";
        private const string NsisRebootLaterRadioAutomationId = "1204";
        private const string NsisForwardAutomationId = "1";
        private const string NsisCancelAutomationId = "2";
        private const string NsisYesAutomationId = "6";
        private const string NsisNoAutomationId = "7";

        private static readonly string[] GoodRadioIds = { NsisRebootLaterRadioAutomationId };
        private static readonly string[] BadRadioIds = { NsisRebootNowRadioAutomationId };
        private static readonly string[] BadButtonIds = { NsisNoAutomationId };
        private static readonly string[] CancelButtonIds = { NsisCancelAutomationId };
        private static readonly string[] GoodButtonIds = { NsisForwardAutomationId, NsisYesAutomationId };
        private static readonly string[] ControlBoxButtonIds = { "Minimize", "Maximize", "Close" };

        private static readonly string[] GoodButtonNames;
        private static readonly string[] CancelButtonNames;
        private static readonly string[] BadButtonNames;

        private static bool _hideAutomatizedWindows = true;

        static AutomatedUninstallManager()
        {
            var loc = Localization.Culture;

            var rm = new ResourceManager(typeof(Localization));
            var validCultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .Where(c => rm.GetResourceSet(c, true, false) != null)
                .ToList();
            
            string[] GetValuesFromAllLanguages(IEnumerable<CultureInfo> cultureInfos, Func<string> targetFieldSelector)
            {
                return cultureInfos
                    .Do(c => Localization.Culture = c)
                    .SelectMany(_ => targetFieldSelector().Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                    .Select(x => x.Trim().ToLowerInvariant())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .ToArray();
            }

            GoodButtonNames = GetValuesFromAllLanguages(validCultureInfos, () => Localization.Auto_GoodButtons);
            CancelButtonNames = GetValuesFromAllLanguages(validCultureInfos, () => Localization.Auto_CancelButtons);
            BadButtonNames = GetValuesFromAllLanguages(validCultureInfos, () => Localization.Auto_BadButtons);

            Localization.Culture = loc;
        }

        public static event EventHandler HideAutomatizedWindowsChanged;
        public static bool HideAutomatizedWindows
        {
            get => _hideAutomatizedWindows;
            set
            {
                if (_hideAutomatizedWindows == value) return;
                _hideAutomatizedWindows = value;
                HideAutomatizedWindowsChanged?.Invoke(null, EventArgs.Empty);
            }
        }

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

                pr = ProcessTools.SeparateArgsFromCommand(uninstallerCommand).ToProcessStartInfo().Start();
                if (pr == null)
                    throw new IOException(Localization.Message_Automation_ProcessFailedToStart);

                // NSIS uninstallers are first extracted by the executable to a temporary directory, and then ran from there.
                // Wait for the extracting exe to close and grab the child process that it started.
                statusCallback(Localization.Message_Automation_WaitingForNsisExtraction);
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
            }
            catch (Exception e)
            {
                throw new AutomatedUninstallException(Localization.Message_Automation_Failed, e,
                    uninstallerCommand, app?.Process ?? pr);
            }

            if (app != null)
                AutomatizeApplication(app, statusCallback);
        }

        public static void AutomatizeApplication(Application app, Action<string> statusCallback)
        {
            var windows = new List<Window>();

            void VisibleChangedHandler(object sender, EventArgs args) => SetWindowVisibility(windows, HideAutomatizedWindows);
            HideAutomatizedWindowsChanged += VisibleChangedHandler;

            try
            {
                statusCallback(string.Format(Localization.Message_Automation_AppAttached, app.Name));

                WaitForApplication(app);

                var seenWindows = new List<int>();

                while (!app.HasExited)
                {
                    statusCallback(Localization.Message_Automation_WindowSearching);
                    // NSIS uninstallers always have only one window open (by default)
                    windows.Clear();
                    windows.AddRange(app.GetWindows());

                    SetWindowVisibility(windows, HideAutomatizedWindows);

                    var target = windows.FirstOrDefault();
                    if (target == null)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    statusCallback(string.Format(Localization.Message_Automation_WindowFound, target.Title));
                    WaitForWindow(target);

                    while (!target.IsClosed)
                    {
                        TryClickNextNsisButton(target, statusCallback);
                        WaitForWindow(target);

                        ProcessNsisPopups(app, target, seenWindows, statusCallback);
                    }
                    statusCallback(Localization.Message_Automation_WindowClosed);

                    WaitForApplication(app);
                }
            }
            catch (Exception e)
            {
                throw new AutomatedUninstallException(Localization.Message_Automation_Failed, e, string.Empty,
                    app?.Process);
            }
            finally
            {
                HideAutomatizedWindowsChanged -= VisibleChangedHandler;
            }
        }

        private static void SetWindowVisibility(IEnumerable<Window> windows, bool hide)
        {
            var primaryScreenBounds = Screen.PrimaryScreen.Bounds;
            foreach (var window in windows)
            {
                try
                {
                    if (hide)
                    {
                        // Move window mostly off-screen. A part of it has to be on some screen for teststack to work properly.
                        window.Move(primaryScreenBounds.Right - 1, primaryScreenBounds.Bottom - 1);
                    }
                    else
                    {
                        var b = window.Bounds;
                        // Move window to main screen center
                        window.Move((int)(primaryScreenBounds.X + primaryScreenBounds.Width / 2 - b.Width / 2),
                            (int)(primaryScreenBounds.Y + primaryScreenBounds.Height / 2 - b.Height / 2));
                    }
                }
                catch (SystemException e)
                {
                    Console.WriteLine(e);
                }
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

            var handle = popupWindow.GetHandle();
            if (seenWindows.Contains(handle))
                throw new InvalidOperationException(Localization.Message_Automation_PopupRecurringFound);
            seenWindows.Add(handle);
            statusCallback(string.Format(Localization.Message_Automation_PopupFound, popupWindow.Title));

            while (!popupWindow.IsClosed)
            {
                TryClickNextNsisButton(popupWindow, statusCallback);

                popupWindow.WaitWhileBusy();
                Thread.Sleep(100);
            }
            statusCallback(Localization.Message_Automation_PopupClosed);
        }

        private static void TryClickNextNsisButton(IUIItemContainer target, Action<string> statusCallback)
        {
            statusCallback("Looking for buttons to press...");

            var allButtons = target
                .GetMultiple(SearchCriteria.ByControlType(typeof(Button), WindowsFramework.Win32))
                .Cast<Button>()
                .ToList();

            var filteredButtons = allButtons
                .Where(x => x.Enabled)
                .Where(NotControlBoxButton)
                .ToList();

            var cancelButtons = FilterButtonsByName(filteredButtons, CancelButtonNames)
                .Concat(FilterButtonsById(filteredButtons, CancelButtonIds))
                .ToList();

            filteredButtons.RemoveAll(cancelButtons);

            var badButtons = FilterButtonsById(filteredButtons, BadButtonIds)
                .Concat(FilterButtonsByName(filteredButtons, BadButtonNames)).ToList();

            var applicableButtons = filteredButtons.Count == 1 ? filteredButtons : filteredButtons.Except(badButtons).ToList();

            if (applicableButtons.Any())
            {
                var nextButton = FilterButtonsById(applicableButtons, GoodButtonIds)
                    .Concat(FilterButtonsByName(applicableButtons, GoodButtonNames))
                    .FirstOrDefault();

                if (nextButton == null)
                {
                    if (applicableButtons.Count == 1 && cancelButtons.Any() && allButtons.All(x=>x.Enabled))
                    {
                        // If there is only one valid button, and the rest are cancel buttons,
                        // assume the valid button is the correct choice.
                        nextButton = applicableButtons.Single();
                    }
                    else
                    {
                        //Debug.Fail("Nothing to press!");
                        return;
                    }
                }

                ProcessRadioButtons(target, statusCallback);

                statusCallback(string.Format(Localization.Message_Automation_ClickingButton, nextButton.Text));
                // Finally press the button, doesn't require messing with the mouse.
                //nextButton.RaiseClickEvent();
                nextButton.Focus();
                nextButton.KeyIn(KeyboardInput.SpecialKeys.RETURN);
            }
        }

        private static void ProcessRadioButtons(IUIItemContainer target, Action<string> statusCallback)
        {
            var allRadios = target.GetMultiple(SearchCriteria.ByControlType(typeof(RadioButton), WindowsFramework.Win32))
                                .Cast<RadioButton>().ToList();
            if (allRadios.Any())
            {
                statusCallback(string.Format(Localization.Message_Automation_FoundButtons, allRadios.Count));

                // Select all known good radio buttons first
                var goodRadios = allRadios.Where(x => GoodRadioIds.Any(
                    y => y.Equals(x.Id, StringComparison.OrdinalIgnoreCase)));

                foreach (var radioButton in goodRadios)
                {
                    if (radioButton.Enabled)
                    {
                        statusCallback(string.Format(Localization.Message_Automation_SelectingGoodButton, radioButton.Name));
                        radioButton.IsSelected = true;
                    }
                }

                // Check if any known bad radio buttons are still enabled. If yes, select other, non-bad buttons.
                var badRadios = allRadios.Where(x => BadRadioIds.Any(
                    y => y.Equals(x.Id, StringComparison.OrdinalIgnoreCase))).ToList();

                if (badRadios.Any(x => x.Enabled && x.IsSelected))
                {
                    foreach (var notBadRadio in allRadios.Except(badRadios))
                    {
                        if (notBadRadio.Enabled)
                        {
                            statusCallback(string.Format(Localization.Message_Automation_SelectingNotBadButton, notBadRadio.Name));
                            notBadRadio.IsSelected = true;
                        }
                    }
                }
            }
        }

        private static IEnumerable<Button> FilterButtonsByName(IEnumerable<Button> buttons, IEnumerable<string> buttonNames)
        {
            return buttonNames
                .Select(buttonName => buttons.FirstOrDefault(
                    x => x.Name.Equals(buttonName, StringComparison.InvariantCultureIgnoreCase)))
                .Where(button => button != null);
        }

        private static IEnumerable<Button> FilterButtonsById(IEnumerable<Button> buttons, IEnumerable<string> buttonIds)
        {
            return buttonIds
                .Select(buttonName => buttons.FirstOrDefault(
                    x => x.Id.Equals(buttonName, StringComparison.InvariantCultureIgnoreCase)))
                .Where(button => button != null);
        }

        /// <summary>
        ///     Tests if the button is not a part of the control box.
        /// </summary>
        /// <param name="x">Button to test</param>
        private static bool NotControlBoxButton(Button x)
        {
            var id = x.Id;
            return !ControlBoxButtonIds.Any(y => id.Equals(y));
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