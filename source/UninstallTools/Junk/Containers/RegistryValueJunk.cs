/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Klocman.Tools;

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

        public override void Backup(string backupDirectory)
        {
            using (var key = OpenRegKey())
            {
                var target = key.GetValue(ValueName);

                var targetValue = target as string;
                if (targetValue != null)
                {
                    var dir = CreateBackupDirectory(backupDirectory);
                    var fileName = PathTools.SanitizeFileName(string.Concat(FullRegKeyPath, " - ", ValueName)
                        .TrimStart('\\').Replace('.', '_')) + ".reg";
                    RegistryTools.ExportRegistryStringValues(Path.Combine(dir, fileName), FullRegKeyPath,
                        new KeyValuePair<string, string>(ValueName, targetValue));
                }
                else
                {
                    Debug.Fail("Unsupported type " + target.GetType().FullName);
                }
            }
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