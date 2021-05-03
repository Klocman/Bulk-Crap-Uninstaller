using System;
using System.IO;

namespace UniversalUninstaller
{
    public class TreeEntry
    {
        public TreeEntry(FileSystemInfo fileSystemInfo)
        {
            FileSystemInfo = fileSystemInfo ?? throw new ArgumentNullException(nameof(fileSystemInfo));
            IsDirectory = fileSystemInfo is DirectoryInfo;
            Checked = true;
        }

        public bool IsDirectory { get; }
        public FileSystemInfo FileSystemInfo { get; }
        public bool Checked { get; set; }
    }
}