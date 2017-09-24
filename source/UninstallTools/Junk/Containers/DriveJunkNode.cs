/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using Klocman.Tools;
using UninstallTools.Properties;

namespace UninstallTools.Junk
{
    public abstract class DriveJunkNode : JunkResultBase
    {
        public override void Backup(string backupDirectory)
        {
            // Items are deleted to the recycle bin
        }

        public DriveJunkNode(ApplicationUninstallerEntry application, IJunkCreator source) : base(application, source)
        {
        }

        public abstract FileSystemInfo Path { get; }
        
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public override void Open()
        {
            if (Path.Exists)
                WindowsTools.OpenExplorerFocusedOnObject(Path.FullName);
            else
                throw new FileNotFoundException(null, Path.FullName);
        }
    }
}