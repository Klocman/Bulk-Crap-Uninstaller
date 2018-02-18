/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;
using Klocman.IO;
using Scripting;

namespace UninstallTools.Factory.InfoAdders
{
    public class FastSizeGenerator : IMissingInfoAdder
    {
        private static readonly FileSystemObjectClass FileSystemObject;

        static FastSizeGenerator()
        {
            try
            {
                FileSystemObject = new FileSystemObjectClass();
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"WARNING: Scripting.FileSystemObjectClass is not available - " + ex.Message);
            }
        }

        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            if (FileSystemObject == null || !Directory.Exists(target.InstallLocation) || 
                UninstallToolsGlobalConfig.IsSystemDirectory(target.InstallLocation))
                return;

            try
            {
                var folder = FileSystemObject.GetFolder(target.InstallLocation);
                var size = new FileSize(Convert.ToInt64(folder.Size) / 1024);
                target.EstimatedSize = size;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public string[] RequiredValueNames { get; } = { nameof(ApplicationUninstallerEntry.InstallLocation) };
        public bool RequiresAllValues { get; } = true;
        public bool AlwaysRun { get; } = false;
        public string[] CanProduceValueNames { get; } = { nameof(ApplicationUninstallerEntry.EstimatedSize) };
        public InfoAdderPriority Priority { get; } = InfoAdderPriority.RunLast;
    }
}