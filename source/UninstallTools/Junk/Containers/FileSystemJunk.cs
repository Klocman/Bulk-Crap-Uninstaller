/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;
using Klocman.Tools;
using Microsoft.VisualBasic.FileIO;

namespace UninstallTools.Junk.Containers
{
    public class FileSystemJunk : JunkResultBase
    {
        public FileSystemJunk(FileSystemInfo path, ApplicationUninstallerEntry application, IJunkCreator source) : base(application, source)
        {
            Path = path;
        }

        public FileSystemInfo Path { get; }

        public override void Backup(string backupDirectory)
        {
            // Items are deleted to the recycle bin
        }

        public override void Delete()
        {
            if (Path is DirectoryInfo)
                FileSystem.DeleteDirectory(Path.FullName, UIOption.OnlyErrorDialogs,
                    RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
            else if (Path is FileInfo)
                FileSystem.DeleteFile(Path.FullName, UIOption.OnlyErrorDialogs,
                    RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
            else
                throw new NotImplementedException("Unknown FileSystemInfo implementation");
        }

        public override string GetDisplayName()
        {
            return Path.FullName;
        }

        public override void Open()
        {
            if (Path.Exists)
                WindowsTools.OpenExplorerFocusedOnObject(Path.FullName);
            else
                throw new FileNotFoundException(null, Path.FullName);
        }
    }
}