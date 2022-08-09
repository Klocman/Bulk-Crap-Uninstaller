/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Klocman.Extensions;
using Klocman.Tools;
using Microsoft.Win32;

namespace UninstallTools.Junk.Containers
{
    public class RegistryValueJunk : RegistryKeyJunk
    {
        public RegistryValueJunk(string containingKeyPath, string valueName, ApplicationUninstallerEntry application,
            IJunkCreator source) : base(containingKeyPath, application, source)
        {
            ValueName = valueName;
        }

        public string ValueName { get; }

        /// <summary>
        /// If not null, overrides ValueName in GetDisplayName
        /// </summary>
        public string DisplayValueName { get; set; }

        public override void Backup(string backupDirectory)
        {
            using var key = OpenRegKey();

            var valueKind = key.GetValueKind(ValueName);
            switch (valueKind)
            {
                case RegistryValueKind.ExpandString:
                case RegistryValueKind.String:
                    var targetValue = key.GetStringSafe(ValueName);
                    var dir = CreateBackupDirectory(backupDirectory);
                    var fileName = PathTools.SanitizeFileName(string.Concat(FullRegKeyPath, " - ", ValueName)
                                                                    .TrimStart('\\').Replace('.', '_')) + ".reg";
                    RegistryTools.ExportRegistryStringValues(Path.Combine(dir, fileName), FullRegKeyPath,
                                                             new KeyValuePair<string, string>(ValueName, targetValue));
                    break;
                case RegistryValueKind.MultiString:
                case RegistryValueKind.Binary:
                case RegistryValueKind.DWord:
                case RegistryValueKind.QWord:
                case RegistryValueKind.Unknown:
                default:
                    Debug.Fail($"Unsupported type {valueKind} of value {ValueName}");
                    break;
                case RegistryValueKind.None:
                    break;
            }
        }

        public override string GetDisplayName()
        {
            return base.GetDisplayName() + " => " + (DisplayValueName ?? ValueName);
        }

        public override void Delete()
        {
            using (var key = RegistryTools.OpenRegistryKey(FullRegKeyPath, true))
            {
                key?.DeleteValue(ValueName);
            }
        }
    }
}