using UninstallTools.Properties;

namespace UninstallTools.Junk
{
    public abstract class DriveJunkNode : JunkNode
    {
        protected DriveJunkNode(string parentPath, string name, string uninstallerName)
            : base(parentPath, name, uninstallerName)
        {
            
        }

        public override string GroupName => Localisation.Junk_Drive_GroupName;

        public override void Backup(string backupDirectory)
        {
            // Items are deleted to the recycle bin
        }
    }
}