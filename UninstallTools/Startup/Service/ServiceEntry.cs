using System;
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
            CommandFilePath = ProcessTools.SeparateArgsFromCommand(command).FileName;
            
            FillInformationFromFile(CommandFilePath);
        }

        public override string ParentShortName
        {
            get { return Localisation.Startup_ShortName_Service; }
            protected set {  }
        }

        public override string ParentLongName
        {
            get { return Localisation.Startup_ShortName_Service; }
            protected set {  }
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
            throw new NotImplementedException();
        }
    }
}
