using System;
using System.IO;
using Klocman.Tools;

namespace UninstallTools.Startup.Normal
{
    /// <summary>
    ///     Starup entries stored in Startup folders and Run/RunOnce registry keys
    /// </summary>
    public sealed class StartupEntry : StartupEntryBase
    {
        internal bool AllUsersStore;
        internal bool DisabledStore;

        internal StartupEntry(StartupPointData dataPoint, string fileName, string targetString)
        {
            AllUsersStore = dataPoint.AllUsers;
            IsRegKey = dataPoint.IsRegKey;
            IsRunOnce = dataPoint.IsRunOnce;
            EntryLongName = fileName;
            ParentShortName = dataPoint.Name;
            ParentLongName = dataPoint.Path.TrimEnd('\\');

            Command = targetString;

            if (!string.IsNullOrEmpty(EntryLongName))
                ProgramName = IsRegKey ? EntryLongName : Path.GetFileNameWithoutExtension(EntryLongName);

            ProcessCommandString(Command);

            if (ProgramName == null)
                throw new ArgumentException("Failed to extract program name from supplied information");

            var result = StringTools.StripStringFromVersionNumber(ProgramName);
            ProgramNameTrimmed = result.Length < 3 ? ProgramName : result;
        }

        /// <summary>
        ///     True if the entry is not processed during startup.
        ///     It is stored in the backup reg key and optionally backup directory if it's a link file.
        /// </summary>
        public override bool Disabled
        {
            get { return DisabledStore; }
            set
            {
                if (value != DisabledStore)
                {
                    if (value)
                    {
                        StartupEntryManager.Disable(this);
                    }
                    else
                    {
                        StartupEntryManager.Enable(this);
                    }
                }
            }
        }

        /// <summary>
        ///     True if this entry is executed during logon of all users, false if it is only for the current user.
        /// </summary>
        public bool AllUsers
        {
            get { return AllUsersStore; }
            set
            {
                if (AllUsersStore != value)
                {
                    StartupEntryManager.SetAllUsers(this, value);
                }
            }
        }

        /// <summary>
        ///     True if entry is a registry value, false if it's a link file
        /// </summary>
        public bool IsRegKey { get; internal set; }

        /// <summary>
        ///     True if the entry will be removed after running
        /// </summary>
        public bool IsRunOnce { get; internal set; }

        /// <summary>
        ///     Filename of the link (with extension), or name of the registry value.
        /// </summary>
        public override string EntryLongName { get; protected set; }

        /// <summary>
        ///     Full path to the link file backup
        /// </summary>
        internal string BackupPath { get; set; }

        private void ProcessCommandString(string command)
        {
            if (string.IsNullOrEmpty(command))
                return;

            ProcessStartCommand temp;
            if (ProcessStartCommand.TryParse(command, out temp))
                CommandFilePath = temp.FileName;

            var info = StartupManager.GetInfoFromFileAttributes(CommandFilePath);
            if (!string.IsNullOrEmpty(info.ProgramName))
                ProgramName = info.ProgramName;
            Company = info.Company;
        }

        /// <summary>
        ///     Delete this startup entry from the system
        /// </summary>
        public override void Delete()
        {
            StartupEntryManager.Delete(this);
        }

        /// <summary>
        ///     Check if this entry still exists in the system
        /// </summary>
        public override bool StillExists()
        {
            return StartupEntryManager.StillExists(this);
        }

        //TODO temporary hack
        internal void SetParentFancyName(string newValue)
        {
            ParentShortName = newValue;
        }

        //TODO temporary hack
        internal void SetParentLongName(string newValue)
        {
            ParentLongName = newValue;
        }

        /// <summary>
        ///     $"{ProgramName} | {Company} | {ParentLongName} | {Command}"
        /// </summary>
        public override string ToLongString()
        {
            return $"{ProgramName} | {Company} | {ParentLongName} | {Command}";
        }

        public override void CreateBackup(string backupPath)
        {
            StartupEntryManager.CreateBackup(this, backupPath);
        }
    }
}