/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using Klocman.Extensions;
using Klocman.Tools;
using Microsoft.Win32;

namespace UninstallTools.Factory.InfoAdders
{
    public class UninstallerTypeAdder : IMissingInfoAdder
    {
        private static readonly Regex InnoSetupFilenameRegex = new Regex(@"unins\d\d\d", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            if (target.UninstallerKind != UninstallerType.Unknown)
                return;

            if (target.IsRegistered)
            {
                using (var key = target.OpenRegKey())
                {
                    target.UninstallerKind = GetUninstallerType(key);
                }
            }
            else
            {
                var s = target.UninstallString ?? target.QuietUninstallString;
                if(!string.IsNullOrEmpty(s))
                    target.UninstallerKind = GetUninstallerType(s);
            }
        }
        
        public static UninstallerType GetUninstallerType(RegistryKey uninstallerKey)
        {
            // Detect MSI installer based on registry entry (the proper way)
            if ((int)uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameWindowsInstaller, 0) != 0)
            {
                return UninstallerType.Msiexec;
            }

            // Detect InnoSetup
            if (uninstallerKey.GetValueNames().Any(x => x.Contains("Inno Setup:")))
            {
                return UninstallerType.InnoSetup;
            }

            // Detect Steam
            if (uninstallerKey.GetKeyName().StartsWith("Steam App ", StringComparison.Ordinal))
            {
                return UninstallerType.Steam;
            }

            var uninstallString =
                uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameUninstallString) as string;

            return string.IsNullOrEmpty(uninstallString)
                ? UninstallerType.Unknown
                : UninstallerTypeAdder.GetUninstallerType(uninstallString);
        }

        public static UninstallerType GetUninstallerType(string uninstallString)
        {
            // Detect MSI installer based on the uninstall string
            //"C:\ProgramData\Package Cache\{33d1fd90-4274-48a1-9bc1-97e33d9c2d6f}\vcredist_x86.exe"  /uninstall
            if (ApplicationUninstallerFactory.PathPointsToMsiExec(uninstallString) || uninstallString.ContainsAll(
                new[] { @"\Package Cache\{", @"}\", ".exe" }, StringComparison.OrdinalIgnoreCase))
                return UninstallerType.Msiexec;

            // Detect Sdbinst
            if (uninstallString.Contains("sdbinst", StringComparison.OrdinalIgnoreCase)
                && uninstallString.Contains(".sdb", StringComparison.OrdinalIgnoreCase))
                return UninstallerType.SdbInst;

            if (uninstallString.Contains(@"InstallShield Installation Information\{", StringComparison.OrdinalIgnoreCase))
                return UninstallerType.InstallShield;

            ProcessStartCommand ps;
            if (ProcessStartCommand.TryParse(uninstallString, out ps) && Path.IsPathRooted(ps.FileName) &&
                File.Exists(ps.FileName))
            {
                try
                {
                    var fileName = Path.GetFileNameWithoutExtension(ps.FileName);
                    // Detect Inno Setup
                    if (fileName != null && InnoSetupFilenameRegex.IsMatch(fileName))
                    {
                        // Check if Inno Setup Uninstall Log exists
                        if (File.Exists(ps.FileName.Substring(0, ps.FileName.Length - 3) + "dat"))
                            return UninstallerType.InnoSetup;
                    }

                    var result = File.ReadAllText(ps.FileName, Encoding.ASCII);

                    // Detect NSIS Nullsoft.NSIS (the most common)
                    if (result.Contains("Nullsoft"))
                        return UninstallerType.Nsis;

                    /* Unused/unnecessary
                    if (result.Contains("InstallShield"))
                        return UninstallerType.InstallShield;
                    if (result.Contains("Inno.Setup") || result.Contains("Inno Setup"))
                        return UninstallerType.InnoSetup;
                    if(result.Contains(@"<description>Adobe Systems Incorporated Setup</description>"))
                        return UninstallerType.AdobeSetup;
                    */
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
                catch (SecurityException) { }
            }
            return UninstallerType.Unknown;
        }
    }
}