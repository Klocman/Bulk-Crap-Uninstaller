/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

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