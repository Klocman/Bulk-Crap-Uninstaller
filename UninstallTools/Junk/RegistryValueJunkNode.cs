/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using Klocman.Tools;
namespace UninstallTools.Junk
{
    public class RegistryValueJunkNode : RegistryJunkNode
    {
        public RegistryValueJunkNode(string keyPath, string valueName, string uninstallerName)
            : base(keyPath, valueName, uninstallerName)
        {
        }

        public override void Backup(string backupDirectory)
        {
            using (var key = RegistryTools.OpenRegistryKey(ParentPath))
            {
                var target = key.GetValue(Name);

                var targetValue = target as string;
                if(targetValue != null)
                {
                    var dir = CreateBackupDirectory(backupDirectory);
                    var fileName = PathTools.SanitizeFileName(FullName.TrimStart('\\').Replace('.', '_')) + ".reg";
                    RegistryTools.ExportRegistryStringValues(Path.Combine(dir, fileName), ParentPath,
                        new KeyValuePair<string, string>(Name, targetValue));
                }
                else
                {
                    Debug.Fail("Unsupported type " + target.GetType().FullName);
                }
            }
        }

        public override void Delete()
        {
            using (var key = RegistryTools.OpenRegistryKey(ParentPath, true))
            {
                key?.DeleteValue(Name);
            }
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public override void Open()
        {
            RegistryTools.OpenRegKeyInRegedit(ParentPath);
        }
    }
}
