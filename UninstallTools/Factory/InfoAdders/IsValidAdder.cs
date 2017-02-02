/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;
using System.Linq;
using System.Threading;
using Klocman.Extensions;
using UninstallTools.Uninstaller;

namespace UninstallTools.Factory.InfoAdders
{
    public class IsValidAdder : IMissingInfoAdder
    {
        public static bool GetIsValid(string uninstallString, string uninstallerFullFilename,
            UninstallerType uninstallerKind, Guid bundleProviderKey)
        {
            if (uninstallString.IsNotEmpty() && !ApplicationUninstallerFactory.PathPointsToMsiExec(uninstallString))
            {
                if (!Path.IsPathRooted(uninstallerFullFilename) || File.Exists(uninstallerFullFilename))
                    return true;
            }

            return uninstallerKind == UninstallerType.Msiexec && UninstallManager.WindowsInstallerValidGuids.Contains(bundleProviderKey);
        }

        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            target.IsValid = GetIsValid(target.UninstallString, target.UninstallerFullFilename, target.UninstallerKind,
                target.BundleProviderKey);
        }

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallString),
            nameof(ApplicationUninstallerEntry.UninstallerFullFilename),
            nameof(ApplicationUninstallerEntry.UninstallerKind),
            nameof(ApplicationUninstallerEntry.BundleProviderKey)
        };
        public string[] ProducedValueNames { get; } = { nameof(ApplicationUninstallerEntry.IsValid) };
        public bool RequiresAllValues { get; } = false;
        public ThreadPriority Priority { get; } = ThreadPriority.Lowest;
    }
}