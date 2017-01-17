using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using Microsoft.VisualBasic.FileIO;

namespace UninstallTools.Junk
{
    public class DriveDirectoryJunkNode : DriveJunkNode
    {
        public DriveDirectoryJunkNode(string parentPath, string name, string uninstallerName)
            : base(parentPath, name, uninstallerName)
        {
            Debug.Assert(!File.Exists(FullName));
        }

        public override void Delete()
        {
            FileSystem.DeleteDirectory(FullName, UIOption.OnlyErrorDialogs,
                RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
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
                throw new FileNotFoundException(null, FullName);
            }
        }
    }
}