using System.Collections.Generic;
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
                var dir = CreateBackupDirectory(backupDirectory);
                var fileName = PathTools.SanitizeFileName(FullName.TrimStart('\\')) + ".reg";
                RegistryTools.ExportRegistryStringValues(Path.Combine(dir, fileName), ParentPath,
                    new KeyValuePair<string, string>(Name, key.GetValue(Name)?.ToString()));
            }
        }

        public override void Delete()
        {
            using (var key = RegistryTools.OpenRegistryKey(ParentPath, true))
            {
                key.DeleteValue(Name);
            }
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public override void Open()
        {
            RegistryTools.OpenRegKeyInRegedit(ParentPath);
        }
    }
}
