using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using Klocman.Tools;
using Microsoft.VisualBasic.FileIO;
using UninstallTools.Properties;

namespace UninstallTools.Junk
{
    public class DriveJunkNode : JunkNode
    {
        public DriveJunkNode(string parentPath, string name, string uninstallerName)
            : base(parentPath, name, uninstallerName)
        {
        }

        public override string GroupName => Localisation.Junk_Drive_GroupName;

        public override void Delete()
        {
            if (Directory.Exists(FullName))
            {
                FileSystem.DeleteDirectory(FullName, UIOption.OnlyErrorDialogs,
                    RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
            }
            else
            {
                FileSystem.DeleteFile(FullName, UIOption.OnlyErrorDialogs,
                    RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
            }
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public override void Open()
        {
            if (Directory.Exists(FullName))
            {
                Process.Start(FullName);
            }
            else
            {
                WindowsTools.OpenExplorerFocusedOnObject(FullName);
            }
        }
    }
}