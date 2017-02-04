/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using Klocman.Tools;

namespace UninstallTools.Factory.InfoAdders
{
    public class Is64BitGetter : IMissingInfoAdder
    {
        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            if (target.SortedExecutables == null || target.SortedExecutables.Length == 0 ||
                target.Is64Bit != MachineType.Unknown)
                return;

            try
            {
                target.Is64Bit = FilesystemTools.CheckExecutableMachineType(target.SortedExecutables[0]);
            }
            catch
            {
                target.Is64Bit = MachineType.Unknown;
            }
        }

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.SortedExecutables)
        };

        public bool RequiresAllValues { get; } = true;

        public string[] CanProduceValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.Is64Bit)
        };

        public InfoAdderPriority Priority { get; } = InfoAdderPriority.Normal;
    }
}