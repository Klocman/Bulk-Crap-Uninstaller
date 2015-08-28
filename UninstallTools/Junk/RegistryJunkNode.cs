using System;
using System.Diagnostics;
using System.Security.Permissions;
using Klocman.Tools;
using UninstallTools.Properties;

namespace UninstallTools.Junk
{
    public class RegistryJunkNode : JunkNode
    {
        public RegistryJunkNode(string parentPath, string name, string parentName)
            : base(parentPath, name, parentName)
        {
        }

        public override string GroupName => Localisation.Junk_Registry_GroupName;

        public override void Delete()
        {
            try
            {
                using (var key = RegistryTools.OpenRegistryKey(ParentPath, true))
                {
                    key.DeleteSubKeyTree(Name);
                }
            }
            catch (Exception ex)
            {
                // Failed to remove the key
                Debug.WriteLine("RegistryJunkNode\\Delete -> " + ex.Message);
            }
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public override void Open()
        {
            WindowsTools.OpenRegKeyInRegedit(FullName);
        }
    }
}