/*
    EBUninstaller Pro - Scripting File System COM Interop & Native Fast Sizer
    Provides lightweight COM dispatch / native fallback for fast folder sizing without MSBuild COMReference.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;

namespace Klocman.IO
{
    public static class ScriptingFileSystem
    {
        private static readonly dynamic? _fso;

        static ScriptingFileSystem()
        {
            try
            {
                var type = Type.GetTypeFromProgID("Scripting.FileSystemObject");
                if (type != null)
                {
                    _fso = Activator.CreateInstance(type);
                }
            }
            catch
            {
                _fso = null;
            }
        }

        public static bool IsAvailable => _fso != null;

        public static long? GetDirectorySizeBytes(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
                return null;

            if (_fso != null)
            {
                try
                {
                    var folder = _fso.GetFolder(directoryPath);
                    if (folder != null)
                    {
                        return Convert.ToInt64(folder.Size);
                    }
                }
                catch
                {
                    // Ignore COM retrieval failures
                }
            }

            return null;
        }
    }
}
