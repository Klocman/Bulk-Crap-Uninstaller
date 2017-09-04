using System;
using System.IO;

namespace UniversalUninstaller
{
    public class TreeEntry
    {
        public TreeEntry(FileSystemInfo fileSystemInfo)
        {
            if (fileSystemInfo == null) throw new ArgumentNullException(nameof(fileSystemInfo));
            FileSystemInfo = fileSystemInfo;
            IsDirectory = fileSystemInfo is DirectoryInfo;
            Checked = true;
        }

        public bool IsDirectory { get; }
        public FileSystemInfo FileSystemInfo { get; }
        public bool Checked { get; set; }
    }
}