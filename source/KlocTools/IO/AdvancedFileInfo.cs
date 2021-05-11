/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.IO;
using Klocman.Localising;
using Klocman.Properties;

namespace Klocman.IO
{
    /// <summary>
    ///     Advanced file information
    /// </summary>
    public class AdvancedFileInfo
    {
        private readonly FileInfo _fileInfo;
        private readonly FileVersionInfo _versionInfo;

        private AdvancedFileInfo(FileInfo fileInfo, FileVersionInfo versionInfo)
        {
            _fileInfo = fileInfo;
            _versionInfo = versionInfo;
        }

        [LocalisedName(typeof (Localisation), nameof(Localisation.FileInfo_CreationTime))]
        public DateTime CreationTime
        {
            get { return _fileInfo.CreationTime; }
            set { _fileInfo.CreationTime = value; }
        }

        [LocalisedName(typeof (Localisation), nameof(Localisation.FileInfo_LastAccessTime))]
        public DateTime LastAccessTime
        {
            get { return _fileInfo.LastAccessTime; }
            set { _fileInfo.LastAccessTime = value; }
        }

        [LocalisedName(typeof (Localisation), nameof(Localisation.FileInfo_LastWriteTime))]
        public DateTime LastWriteTime
        {
            get { return _fileInfo.LastWriteTime; }
            set { _fileInfo.LastWriteTime = value; }
        }

        [LocalisedName(typeof (Localisation), nameof(Localisation.FileInfo_Attributes))]
        public FileAttributes Attributes
        {
            get { return _fileInfo.Attributes; }
            set { _fileInfo.Attributes = value; }
        }

        [LocalisedName(typeof (Localisation), nameof(Localisation.FileInfo_IsReadOnly))]
        public bool IsReadOnly
        {
            get { return _fileInfo.IsReadOnly; }
            set { _fileInfo.IsReadOnly = value; }
        }

        [LocalisedName(typeof (Localisation), nameof(Localisation.FileInfo_FullName))]
        public string FullName => _fileInfo.FullName;

        [LocalisedName(typeof (Localisation), nameof(Localisation.FileInfo_FileName))]
        public string FileName => _fileInfo.Name;

        [LocalisedName(typeof (Localisation), nameof(Localisation.FileInfo_Size))]
        public FileSize Size => FileSize.FromBytes(_fileInfo.Length);

        [LocalisedName(typeof (Localisation), nameof(Localisation.FileInfo_Directory))]
        public DirectoryInfo Directory => _fileInfo.Directory;

        [LocalisedName(typeof (Localisation), nameof(Localisation.FileInfo_Exists))]
        public bool Exists => _fileInfo.Exists;

        [LocalisedName(typeof (Localisation), nameof(Localisation.FileInfo_Comments))]
        public string Comments => _versionInfo?.Comments;

        [LocalisedName(typeof (Localisation), nameof(Localisation.FileInfo_CompanyName))]
        public string CompanyName => _versionInfo?.CompanyName;

        [LocalisedName(typeof (Localisation), nameof(Localisation.FileInfo_FileDescription))]
        public string FileDescription => _versionInfo?.FileDescription;

        [LocalisedName(typeof (Localisation), nameof(Localisation.FileInfo_FileVersion))]
        public Version FileVersion => _versionInfo == null
            ? null
            : new Version(_versionInfo.FileMajorPart, _versionInfo.FileMinorPart, _versionInfo.FileBuildPart,
                _versionInfo.FilePrivatePart);

        [LocalisedName(typeof (Localisation), nameof(Localisation.FileInfo_InternalName))]
        public string InternalName => _versionInfo?.InternalName;

        [LocalisedName(typeof (Localisation), nameof(Localisation.FileInfo_Language))]
        public string Language => _versionInfo?.Language;

        [LocalisedName(typeof (Localisation), nameof(Localisation.FileInfo_LegalCopyright))]
        public string LegalCopyright => _versionInfo?.LegalCopyright;

        [LocalisedName(typeof (Localisation), nameof(Localisation.FileInfo_LegalTrademarks))]
        public string LegalTrademarks => _versionInfo?.LegalTrademarks;

        [LocalisedName(typeof (Localisation), nameof(Localisation.FileInfo_OriginalFilename))]
        public string OriginalFilename => _versionInfo?.OriginalFilename;

        [LocalisedName(typeof (Localisation), nameof(Localisation.FileInfo_ProductName))]
        public string ProductName => _versionInfo?.ProductName;

        [LocalisedName(typeof (Localisation), nameof(Localisation.FileInfo_ProductVersion))]
        public Version ProductVersion => _versionInfo == null
            ? null
            : new Version(_versionInfo.ProductMajorPart, _versionInfo.ProductMinorPart,
                _versionInfo.ProductBuildPart, _versionInfo.ProductPrivatePart);

        public static AdvancedFileInfo FromPath(string filePath)
        {
            FileVersionInfo fileVersionInfo;

            try
            {
                fileVersionInfo = FileVersionInfo.GetVersionInfo(filePath);
            }
            catch
            {
                fileVersionInfo = null;
            }

            return new AdvancedFileInfo(new FileInfo(filePath), fileVersionInfo);
        }
    }
}