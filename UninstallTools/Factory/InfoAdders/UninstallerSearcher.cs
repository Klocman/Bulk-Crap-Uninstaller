/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UninstallTools.Factory.InfoAdders
{
    public class UninstallerSearcher : IMissingInfoAdder
    {
        private static readonly string[] UninstallerFilters = { "unins0", "uninstall", "uninst", "uninstaller" };

        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            if (!string.IsNullOrEmpty(target.UninstallString) || target.SortedExecutables == null)
                return;

            // Attempt to find an uninstaller application
            foreach (var file in target.SortedExecutables.Concat(FindExtraExecutables(target.InstallLocation)))
            {
                string name;
                try
                {
                    name = Path.GetFileNameWithoutExtension(file);
                    if (string.IsNullOrEmpty(name)) continue;
                }
                catch (ArgumentException)
                {
                    continue;
                }

                if (UninstallerFilters.Any(filter =>
                    name.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase) ||
                    name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase)))
                {
                    target.UninstallString = file;
                    return;
                }
            }
        }

        private static IEnumerable<string> FindExtraExecutables(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                try
                {
                    return Directory.GetFiles(directoryPath, "*.bat", SearchOption.TopDirectoryOnly);
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
            return Enumerable.Empty<string>();
        }

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.SortedExecutables)
        };
        public bool RequiresAllValues { get; } = true;

        public string[] CanProduceValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallString)
        };
        public InfoAdderPriority Priority { get; } = InfoAdderPriority.RunLast;
    }
}