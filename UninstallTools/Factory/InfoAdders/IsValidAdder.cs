/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;
using System.Linq;
using Klocman.Extensions;

namespace UninstallTools.Factory
{
    public static class IsValidAdder
    {
        public static bool GetIsValid(string uninstallString, string uninstallerFullFilename,
            UninstallerType uninstallerKind, Guid bundleProviderKey)
        {
            if (uninstallString.IsNotEmpty() && !ApplicationUninstallerFactory.PathPointsToMsiExec(uninstallString))
            {
                if (!Path.IsPathRooted(uninstallerFullFilename) || File.Exists(uninstallerFullFilename))
                    return true;
            }

            return uninstallerKind == UninstallerType.Msiexec && ApplicationUninstallerManager.WindowsInstallerValidGuids.Contains(bundleProviderKey);
        }
    }
}