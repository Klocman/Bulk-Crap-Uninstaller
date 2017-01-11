using System;
using System.IO;
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

            ParentLongName = Localisation.Startup_ShortName_Task + task.Path;
            EntryLongName = task.Name;

            FillInformationFromFile(commandFilename);
        }

        private Microsoft.Win32.TaskScheduler.Task SourceTask { get; }

        public override bool Disabled
        {
            get
            {
                try { return !SourceTask.Enabled; }
                catch (FileNotFoundException) { }
                catch (InvalidCastException) { }
                catch (System.Runtime.InteropServices.COMException) { }
                //HACK: If it's impossible to check disabled state, assume not disabled
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