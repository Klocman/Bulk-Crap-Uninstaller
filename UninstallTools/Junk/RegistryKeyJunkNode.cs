using System.IO;
using System.Security.Permissions;
using Klocman.Tools;
using UninstallTools.Properties;

namespace UninstallTools.Junk
{
    public class RegistryKeyJunkNode : RegistryJunkNode
    {
        public RegistryKeyJunkNode(string parentPath, string name, string uninstallerName)
            : base(parentPath, name, uninstallerName)
        {
        }

        public override string GroupName => Localisation.Junk_Registry_GroupName;

        public override void Backup(string backupDirectory)
        {
            var fileName = PathTools.SanitizeFileName(FullName.TrimStart('\\')) + ".reg";
            var path = Path.Combine(CreateBackupDirectory(backupDirectory), fileName);
            RegistryTools.ExportRegistry(path, new[] { FullName });
        }

        public override void Delete()
        {
            using (var key = RegistryTools.OpenRegistryKey(ParentPath, true))
            {
                key.DeleteSubKeyTree(Name);
            }
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public override void Open()
        {
            RegistryTools.OpenRegKeyInRegedit(FullName);
        }
    }
}