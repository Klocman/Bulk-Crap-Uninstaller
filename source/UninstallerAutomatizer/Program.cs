/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Klocman;

namespace UninstallerAutomatizer
{
    /// <summary>
    /// UninstallerAutomatizer.exe UninstallerType [/K] UninstallCommand
    /// </summary>
    internal class Program
    {
        public static readonly string AutomatizerProcessName = Process.GetCurrentProcess().ProcessName;

        public static ReturnValue ReturnValue { get; set; } = ReturnValue.OkCode;

        [STAThread]
        private static int Main()
        {
            using (LogWriter.StartLogging())
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Application.Run(new MainWindow());

                return (int)ReturnValue;
            }
        }
    }
}