/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Native;
using Klocman.Tools;

namespace UninstallTools.Factory
{
    public static class ApplicationUninstallerFactory
    {
        public static string GetUninstallerFilename(string uninstallString, UninstallerType type, Guid bundleKey)
        {
            if (!string.IsNullOrEmpty(uninstallString) && !PathPointsToMsiExec(uninstallString))
            {
                try
                {
                    var fileName = ProcessTools.SeparateArgsFromCommand(uninstallString).FileName;

                    Debug.Assert(!fileName.Contains(' ') || File.Exists(fileName));

                    return fileName;
                }
                catch (ArgumentException) { }
                catch (FormatException) { }
            }

            return type == UninstallerType.Msiexec ? MsiTools.MsiGetProductInfo(bundleKey, MsiWrapper.INSTALLPROPERTY.LOCALPACKAGE) : string.Empty;
        }

        /// <summary>
        ///     Check if path points to the windows installer program or to a .msi package
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool PathPointsToMsiExec(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            return path.ContainsAny(new[] { "msiexec ", "msiexec.exe" }, StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".msi", StringComparison.OrdinalIgnoreCase);
        }
    }
}