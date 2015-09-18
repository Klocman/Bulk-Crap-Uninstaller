using System.IO;
using Klocman.Tools;
using Microsoft.Win32.TaskScheduler;
using UninstallTools.Properties;

namespace UninstallTools.Startup.Task
{
    public sealed class TaskEntry : StartupEntryBase
    {
        internal TaskEntry(string name, string command, string commandFilename, Microsoft.Win32.TaskScheduler.Task task)
        {
            ProgramName = name;
            Command = command;
            CommandFilePath = commandFilename;
            SourceTask = task;

            ParentLongName = task.Path;
            EntryLongName = task.Name;

            var info = StartupManager.GetInfoFromFileAttributes(CommandFilePath);
            Company = info.Company;

            if (string.IsNullOrEmpty(info.ProgramName))
            {
                ProgramNameTrimmed = StringTools.StripStringFromVersionNumber(ProgramName);
            }
            else
            {
                var result = StringTools.StripStringFromVersionNumber(info.ProgramName);
                ProgramNameTrimmed = result.Length < 3 ? info.ProgramName : result;
            }
        }

        private Microsoft.Win32.TaskScheduler.Task SourceTask { get; }

        public override bool Disabled
        {
            //HACK: For now if it's impossible to check disabled state, assume not disabled
            get
            {
                try { return !SourceTask.Enabled; }
                catch (FileNotFoundException) { }
                catch (System.Runtime.InteropServices.COMException) { }
                return false;
            }
            //TODO: Give some sort of message instead of crashing if not supported, maybe disable disable buttons
            set { SourceTask.Enabled = !value; } 
        }

        public override string ParentShortName
        {
            get { return Localisation.Startup_ShortName_Task; }
            protected set { }
        }

        public override void Delete()
        {
            SourceTask.Folder.DeleteTask(SourceTask.Name, false);
        }

        public override bool StillExists()
        {
            return TaskService.Instance.FindTask(SourceTask.Name) != null;
        }

        public override void CreateBackup(string backupPath)
        {
            File.WriteAllText(
                Path.Combine(backupPath, Localisation.Startup_ShortName_Task + " - " + EntryLongName + ".xml"),
                SourceTask.Xml);
        }
    }
}