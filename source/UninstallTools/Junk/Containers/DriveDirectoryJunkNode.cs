/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using Microsoft.VisualBasic.FileIO;

namespace UninstallTools.Junk
{
    public class DriveDirectoryJunkNode : DriveJunkNode
    {
        public DirectoryInfo DirectoryPath { get; }

        public DriveDirectoryJunkNode(DirectoryInfo directory, ApplicationUninstallerEntry app, IJunkCreator source)
            : base(app, source)
        {
            DirectoryPath = directory;
            Debug.Assert(directory.Exists);
        }

        public override void Delete()
        {
            FileSystem.DeleteDirectory(FullName, UIOption.OnlyErrorDialogs,
                RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
        }

        public override string GetDisplayName()
        {
            throw new System.NotImplementedException();
        }

        public override FileSystemInfo Path => DirectoryPath;
    }
}