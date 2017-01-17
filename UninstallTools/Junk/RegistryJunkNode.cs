using UninstallTools.Properties;

namespace UninstallTools.Junk
{
    public abstract class RegistryJunkNode : JunkNode
    {
        protected RegistryJunkNode(string parentPath, string name, string uninstallerName)
            : base(parentPath, name, uninstallerName)
        {
        }

        public override string GroupName => Localisation.Junk_Registry_GroupName;
    }
}