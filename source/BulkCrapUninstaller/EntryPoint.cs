/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;
using System.Windows.Forms;
using BulkCrapUninstaller.Forms;
using Klocman.Forms.Tools;
using Klocman.UpdateSystem;
using Microsoft.VisualBasic.ApplicationServices;

namespace BulkCrapUninstaller
{
    internal class EntryPoint : WindowsFormsApplicationBase
    {
        private static EntryPoint _instance;
        private static bool _isRestarting;

        public EntryPoint()
        {
            EnableVisualStyles = true;
            IsSingleInstance = true;
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Application.SetCompatibleTextRenderingDefault(false);
            NBugConfigurator.SetupNBug();

            using (LogWriter.StartLogging(Path.Combine(Program.AssemblyLocation.FullName, "BCUninstaller.log")))
            {
                _instance = new EntryPoint();
                _instance.Run(args);
            }
        }

        public static void Restart()
        {
            try
            {
                _isRestarting = true;
                UpdateSystem.RestartApplication();
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
                _isRestarting = false;
            }
        }

        protected override bool OnStartup(StartupEventArgs eventArgs)
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

            // Necessary to put form constructor here for objectlistbox. It flips out if
            // the main form is created inside of the EntryPoint constructor.
            MainForm = new MainWindow();
            return true;
        }

        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
        {
            try
            {
                _instance.MainForm?.Activate();
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
            }
        }

        protected override void OnShutdown()
        {
            // If running as portable, delete any leftovers from the system
            if (!_isRestarting && !Program.IsInstalled && !Program.EnableDebug)
                Program.StartLogCleaner();

            base.OnShutdown();
        }
    }
}