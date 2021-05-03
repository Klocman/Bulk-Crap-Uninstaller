/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Windows.UI.Popups;
using BulkCrapUninstaller.Forms;
using Klocman;
using Klocman.Forms;
using Klocman.Forms.Tools;

namespace BulkCrapUninstaller
{
    internal class EntryPoint
    {
        public static bool IsRestarting { get; internal set; }

        private const string MUTEX_NAME = @"Global\BCU-singleinstance";
        private static Mutex mutex;

        [STAThread]
        public static void Main(string[] args)
        {
            Application.SetCompatibleTextRenderingDefault(false);
            NBugConfigurator.SetupNBug();

            using (LogWriter.StartLogging())
            {
                try
                {
                    Directory.SetCurrentDirectory(Program.AssemblyLocation.FullName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    mutex = new Mutex(true, MUTEX_NAME, out var createdNew);
                    if (!createdNew)
                    {
                        mutex.Dispose();
                        HandleBeingSecondInstance();
                        return;
                    }

                    SetupDependancies();
                    Application.EnableVisualStyles();
                    Application.Run(new MainWindow());
                }
                finally
                {
                    ProcessShutdown();
                }
            }
        }

        private static void ProcessShutdown()
        {
            if (!IsRestarting && !mutex.SafeWaitHandle.IsClosed)
                mutex.ReleaseMutex();
            mutex.Dispose();
            // If running as portable, delete any leftovers from the system
            if (!IsRestarting && !Program.IsInstalled && !Program.EnableDebug)
                Program.StartLogCleaner();
        }

        public static void Restart()
        {
            try
            {
                IsRestarting = true;

                mutex.ReleaseMutex();
                Application.Restart();
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
                IsRestarting = false;
            }
        }



        private static void HandleBeingSecondInstance()
        {
            try
            {
                var location = Assembly.GetAssembly(typeof(EntryPoint)).Location;
                if (location.EndsWith(".dll")) location = location.Substring(0, location.Length - 3) + "exe";
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
                var mainWind = otherBcu?.MainWindowHandle;
                if (mainWind != null)
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
                    CustomMessageBox.ShowDialog(null, new CmbBasicSettings("BCUninstaller is already running", "BCUninstaller is already running", "You can start only one instance of BCUninstaller. Close previous instances and try again. If you don't see the BCUninstaller window or it's not responding, try closing it with Task Manager.", Icon.ExtractAssociatedIcon(location), "OK"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(int hwnd);

        private static void SetupDependancies()
        {
            // Order is semi-important, prepare settings should go first.
            Program.PrepareSettings();
            CultureConfigurator.SetupCulture();
            //try
            //{
            //    UpdateSystem.ProcessPendingUpdates();
            //}
            //catch (Exception ex)
            //{
            //    PremadeDialogs.GenericError(ex);
            //}
        }
    }
}