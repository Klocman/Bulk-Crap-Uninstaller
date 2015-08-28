using System;
using System.Diagnostics;
using System.Security.Permissions;
using Microsoft.VisualBasic.FileIO;
using UninstallTools.Properties;

namespace UninstallTools.Junk
{
    public class DriveJunkNode : JunkNode
    {
        public DriveJunkNode(string parentPath, string name, string parentName)
            : base(parentPath, name, parentName)
        {
        }

        public override string GroupName => Localisation.Junk_Drive_GroupName;

        public override void Delete()
        {
            try
            {
                FileSystem.DeleteDirectory(FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin,
                    UICancelOption.DoNothing);
            }
            catch (Exception ex)
            {
                // Failed to delete the file
                Debug.WriteLine("RegistryJunkNode\\Delete -> " + ex.Message);
            }
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public override void Open()
        {
            Process.Start(FullName);
        }
    }
}