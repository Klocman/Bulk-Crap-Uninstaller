/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using Klocman.Forms.Tools;
using Klocman.Tools;

namespace UninstallTools.Junk.Containers
{
    public class RunProcessJunk : JunkResultBase
    {
        public ProcessStartCommand ProcessToStart { get; }

        private readonly string _junkName;

        public RunProcessJunk(ApplicationUninstallerEntry application, IJunkCreator source, ProcessStartCommand processToStart, string junkName) : base(application, source)
        {
            _junkName = junkName;
            ProcessToStart = processToStart;
        }

        public override void Backup(string backupDirectory)
        {

        }

        public override void Delete()
        {
            try
            {
                var info = ProcessToStart.ToProcessStartInfo();
                info.WindowStyle = ProcessWindowStyle.Minimized;
                info.UseShellExecute = true;
                Process.Start(info)?.WaitForExit();
            }
            catch (SystemException ex)
            {
                Trace.WriteLine($"Failed to delete junk {GetDisplayName()} - {ex}");
            }
        }

        public override string GetDisplayName()
        {
            return $"{_junkName} ({ProcessToStart})";
        }

        public override void Open()
        {
            try
            {
                WindowsTools.OpenExplorerFocusedOnObject(ProcessToStart.FileName);
            }
            catch (SystemException ex)
            {
                PremadeDialogs.GenericError(ex);
            }
        }
    }
}