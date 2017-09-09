/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Windows.Forms;

namespace UninstallerAutomatizer
{
    /// <summary>
    /// UninstallerAutomatizer.exe UninstallerType [/K] UninstallCommand
    /// </summary>
    internal class Program
    {
        public static ReturnValue ReturnValue { get; set; } = ReturnValue.Ok;

        [STAThread]
        private static int Main()
        {

            // todo make a window ui, has a list with log, pause and cancel buttons
            // AutomatedUninstallManager.UninstallNsisQuietly runs in a separate thread
            // change it into instance class, add events for steps, errors and finishes. Could be a single event with switches for iserror, isended
            // hover around the original window? top left corner?
            // when hitting an error, show question with a time progress bar. Uninstall manually or kill and abort, aborts if time runs out

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new MainWindow());

            return (int)ReturnValue;
        }
    }
}