/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.IO;
using Klocman.Extensions;

namespace UninstallTools.Factory
{
    public static class InstallDateAdder
    {
        public static void NewMethod(ApplicationUninstallerEntry applicationUninstaller)
        {
            if (applicationUninstaller.InstallDate.IsDefault() &&
                Directory.Exists(applicationUninstaller.InstallLocation))
            {
                applicationUninstaller.InstallDate =
                    Directory.GetCreationTime(applicationUninstaller.InstallLocation);
            }
        }
    }
}