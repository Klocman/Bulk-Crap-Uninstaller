/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BulkCrapUninstaller.Forms;
using Klocman;
using Klocman.Forms.Tools;
using Klocman.UpdateSystem;
using Microsoft.VisualBasic.ApplicationServices;

namespace BulkCrapUninstaller
{
    internal class EntryPoint
    {
        public static bool IsRestarting { get; internal set; }

        [STAThread]
        public static void Main(string[] args)
        {
            Application.SetCompatibleTextRenderingDefault(false);
            NBugConfigurator.SetupNBug();

            using (LogWriter.StartLogging())
            {
                try
                {
                    var instance = new SingleInstanceWrapper();
                    instance.Run(args);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex);
                    SafeRun();
                }
                catch (CantStartSingleInstanceException ex)
                {
                    Console.WriteLine(ex);
                    SafeRun();
                }
            }
        }

        private static void ProcessShutdown()
        {
            // If running as portable, delete any leftovers from the system
            if (!IsRestarting && !Program.IsInstalled && !Program.EnableDebug)
                Program.StartLogCleaner();
        }

        public static void Restart()
        {
            try
            {
                IsRestarting = true;
                UpdateSystem.RestartApplication();
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
                IsRestarting = false;
            }
        }

        /// <summary>
        /// Alternative startup routine in case WindowsFormsApplicationBase fails.
        /// Uses Process.GetProcesses to check for other instances.
        /// </summary>
        private static void SafeRun()
        {
            var location = Assembly.GetAssembly(typeof (EntryPoint)).Location;
            var otherBcu = Process.GetProcesses().FirstOrDefault(x =>
            {
                try
                {
                    return string.Equals(x.MainModule.FileName, location, StringComparison.OrdinalIgnoreCase);
                }
                catch
                {
                    return false;
                }
            });

            if (otherBcu != null)
            {
                try
                {
                    SetForegroundWindow(otherBcu.MainWindowHandle.ToInt32());
                }
                catch (Exception ex)
                {
                    PremadeDialogs.GenericError(ex);
                }
            }
            else
            {
                SetupDependancies();
                Application.ApplicationExit += (sender, eventArgs) => ProcessShutdown();
                Application.EnableVisualStyles();
                Application.Run(new MainWindow());
            }
        }

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(int hwnd);

        private static void SetupDependancies()
        {
            // Order is semi-important, prepare settings should go first.
            Program.PrepareSettings();
            CultureConfigurator.SetupCulture();
            try
            {
                UpdateSystem.ProcessPendingUpdates();
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
            }
        }

        private class SingleInstanceWrapper : WindowsFormsApplicationBase
        {
            public SingleInstanceWrapper()
            {
                EnableVisualStyles = true;
                IsSingleInstance = true;
            }

            protected override void OnShutdown()
            {
                ProcessShutdown();

                base.OnShutdown();
            }

            protected override bool OnStartup(StartupEventArgs eventArgs)
            {
                SetupDependancies();

                // Necessary to put form constructor here for objectlistbox. It flips out if
                // the main form is created inside of the EntryPoint constructor.
                MainForm = new MainWindow();
                return true;
            }

            protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
            {
                try
                {
                    MainForm?.Activate();
                }
                catch (Exception ex)
                {
                    PremadeDialogs.GenericError(ex);
                }
            }
        }
    }
}