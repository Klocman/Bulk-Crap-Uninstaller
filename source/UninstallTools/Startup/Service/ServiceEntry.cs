/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.IO;
using System.Management;
using Klocman.Tools;
using UninstallTools.Properties;

namespace UninstallTools.Startup.Service
{
    public sealed class ServiceEntry : StartupEntryBase
    {
        public ServiceEntry(string serviceName, string displayName, string command)
        {
            ProgramName = serviceName;
            EntryLongName = displayName;

            Command = command;

            if (ProcessStartCommand.TryParse(command, out var pc))
                CommandFilePath = pc.FileName;
            else if (File.Exists(command))
                CommandFilePath = command;

            FillInformationFromFile(CommandFilePath);
        }

        public override string ParentShortName
        {
            get { return Localisation.Startup_ShortName_Service; }
            protected set { }
        }

        public override string ParentLongName
        {
            get { return Localisation.Startup_ShortName_Service; }
            protected set { }
        }

        public override bool Disabled
        {
            get
            {
                try
                {
                    return ServiceEntryFactory.CheckServiceEnabled(ProgramName);
                }
                catch (ManagementException)
                {
                    return false;
                }
            }
            set { ServiceEntryFactory.EnableService(ProgramName, !value); }
        }

        public override void Delete()
        {
            ServiceEntryFactory.DeleteService(ProgramName);
        }

        public override bool StillExists()
        {
            try
            {
                ServiceEntryFactory.CheckServiceEnabled(ProgramName);
                return true;
            }
            catch (ManagementException)
            {
                return false;
            }
        }

        public override void CreateBackup(string backupPath)
        {
            var path = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\" + ProgramName;

            using (var key = RegistryTools.OpenRegistryKey(path))
            {
                if (key == null)
                    throw new IOException();
            }

            var filename = PathTools.SanitizeFileName(FullLongName) + ".reg";
            RegistryTools.ExportRegistry(Path.Combine(backupPath, filename), new[] { path });
        }
    }
}
