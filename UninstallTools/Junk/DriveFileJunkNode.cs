/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using Klocman.Tools;
using Microsoft.VisualBasic.FileIO;

namespace UninstallTools.Junk
{
    public class DriveFileJunkNode : DriveJunkNode
    {
        public DriveFileJunkNode(string parentPath, string name, string uninstallerName)
            : base(parentPath, name, uninstallerName)
        {
            Debug.Assert(!Directory.Exists(FullName));
        }

        public override void Delete()
        {
            FileSystem.DeleteFile(FullName, UIOption.OnlyErrorDialogs,
                RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public override void Open()
        {
            if (File.Exists(FullName))
                WindowsTools.OpenExplorerFocusedOnObject(FullName);
            else
                throw new FileNotFoundException(null, FullName);
        }
    }
}