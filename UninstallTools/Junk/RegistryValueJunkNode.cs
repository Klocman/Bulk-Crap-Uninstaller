using System;
using System.Diagnostics;
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
        
        public override void Delete()
        {
            try
            {
                using (var key = RegistryTools.OpenRegistryKey(ParentPath, true))
                {
                    key.DeleteValue(Name);
                }
            }
            catch (Exception ex)
            {
                // Failed to remove the key
                Debug.WriteLine("RegistryValueJunkNode\\Delete -> " + ex.Message);
            }
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public override void Open()
        {
            RegistryTools.OpenRegKeyInRegedit(ParentPath);
        }
    }
}
