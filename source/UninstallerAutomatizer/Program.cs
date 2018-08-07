/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace UninstallerAutomatizer
{
    /// <summary>
    /// UninstallerAutomatizer.exe UninstallerType [/K] UninstallCommand
    /// </summary>
    internal class Program
    {
        public static readonly string AutomatizerProcessName = Process.GetCurrentProcess().ProcessName;

        public static ReturnValue ReturnValue { get; set; } = ReturnValue.Ok;

        [STAThread]
        private static int Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            using (LogWriter.StartLogging(Path.Combine(dir ?? string.Empty, "UninstallerAutomatizer.log")))
            {
                Application.Run(new MainWindow());
            }

            return (int)ReturnValue;
        }
    }
}