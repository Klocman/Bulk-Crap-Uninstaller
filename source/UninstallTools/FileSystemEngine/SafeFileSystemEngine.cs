/*
    EBUninstaller Pro - Open Source Professional Windows Uninstaller
    Safe File System Subsystem with Secure Deletion
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.VisualBasic.FileIO;
using UninstallTools.Core;

namespace UninstallTools.FileSystemEngine
{
    public enum DeletionMode
    {
        SendToRecycleBin,
        PermanentNormal,
        SecureZeroFill,
        SecureMultiPassDod
    }

    public sealed class FileSystemOperationResult
    {
        public int DeletedFilesCount { get; set; }
        public int DeletedDirectoriesCount { get; set; }
        public int BlockedCount { get; set; }
        public int FailedCount { get; set; }
        public long BytesFreed { get; set; }
        public List<string> Errors { get; } = new();
        public bool Success => FailedCount == 0;
    }

    public static class SafeFileSystemEngine
    {
        /// <summary>
        /// Deletes a file safely according to specified deletion mode.
        /// </summary>
        public static bool DeleteFileSafe(string filePath, DeletionMode mode = DeletionMode.SendToRecycleBin)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return false;

            var normalized = SecurityGuard.NormalizePath(filePath);
            if (SecurityGuard.IsPathProtected(normalized))
            {
                StructuredLogger.Warning(LogCategory.Security, $"Blocked file deletion on protected path: {normalized}");
                return false;
            }

            if (!File.Exists(normalized))
                return true; // Already deleted or doesn't exist

            try
            {
                // Remove read-only / hidden attributes if needed
                var attrs = File.GetAttributes(normalized);
                if ((attrs & (FileAttributes.ReadOnly | FileAttributes.Hidden)) != 0)
                {
                    File.SetAttributes(normalized, FileAttributes.Normal);
                }

                switch (mode)
                {
                    case DeletionMode.SendToRecycleBin:
                        FileSystem.DeleteFile(normalized, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
                        break;

                    case DeletionMode.PermanentNormal:
                        File.Delete(normalized);
                        break;

                    case DeletionMode.SecureZeroFill:
                        SecureWipeFile(normalized, 1);
                        File.Delete(normalized);
                        break;

                    case DeletionMode.SecureMultiPassDod:
                        SecureWipeFile(normalized, 3);
                        File.Delete(normalized);
                        break;
                }

                StructuredLogger.Info(LogCategory.FileSystem, $"Deleted file [{mode}]: {normalized}");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.FileSystem, $"Failed to delete file: {normalized}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Deletes a directory safely without traversing into symlinks / reparse points.
        /// </summary>
        public static bool DeleteDirectorySafe(string dirPath, DeletionMode mode = DeletionMode.SendToRecycleBin)
        {
            if (string.IsNullOrWhiteSpace(dirPath)) return false;

            var normalized = SecurityGuard.NormalizePath(dirPath);
            if (SecurityGuard.IsPathProtected(normalized))
            {
                StructuredLogger.Warning(LogCategory.Security, $"Blocked directory deletion on protected path: {normalized}");
                return false;
            }

            if (!Directory.Exists(normalized))
                return true; // Already gone

            try
            {
                // If it's a junction/symlink, unmount/delete junction itself without recursing into target!
                if (SecurityGuard.IsReparsePointOrSymlink(normalized))
                {
                    Directory.Delete(normalized, false);
                    StructuredLogger.Info(LogCategory.FileSystem, $"Removed reparse point/junction: {normalized}");
                    return true;
                }

                if (mode == DeletionMode.SendToRecycleBin)
                {
                    FileSystem.DeleteDirectory(normalized, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
                }
                else
                {
                    // Recursively clean files securely if requested
                    CleanDirectoryRecursive(normalized, mode);
                    Directory.Delete(normalized, true);
                }

                StructuredLogger.Info(LogCategory.FileSystem, $"Deleted directory [{mode}]: {normalized}");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.FileSystem, $"Failed to delete directory: {normalized}", ex.Message);
                return false;
            }
        }

        private static void CleanDirectoryRecursive(string dirPath, DeletionMode mode)
        {
            var di = new DirectoryInfo(dirPath);
            if (!di.Exists) return;

            // Check if subdirectory itself is a reparse point
            if ((di.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
            {
                Directory.Delete(dirPath, false);
                return;
            }

            // Clean files
            foreach (var fi in di.GetFiles())
            {
                if (SecurityGuard.IsPathProtected(fi.FullName)) continue;

                if (fi.IsReadOnly)
                    fi.Attributes = FileAttributes.Normal;

                if (mode == DeletionMode.SecureZeroFill || mode == DeletionMode.SecureMultiPassDod)
                {
                    SecureWipeFile(fi.FullName, mode == DeletionMode.SecureZeroFill ? 1 : 3);
                }
                fi.Delete();
            }

            // Recurse child directories
            foreach (var subDir in di.GetDirectories())
            {
                CleanDirectoryRecursive(subDir.FullName, mode);
            }
        }

        /// <summary>
        /// Overwrites file contents before deletion.
        /// Transparent disclaimer: On modern solid-state drives (SSDs) with wear-leveling, TRIM, and
        /// copy-on-write filesystems, hardware block reallocation means raw flash sectors might retain
        /// residual data until trimmed by the controller.
        /// </summary>
        public static void SecureWipeFile(string filePath, int passes = 1)
        {
            if (!File.Exists(filePath)) return;

            var fileLength = new FileInfo(filePath).Length;
            if (fileLength == 0) return;

            var buffer = new byte[Math.Min(64 * 1024, fileLength)];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Write, FileShare.None))
            {
                for (var pass = 0; pass < passes; pass++)
                {
                    stream.Position = 0;
                    long written = 0;

                    if (pass % 2 == 0)
                    {
                        Array.Clear(buffer, 0, buffer.Length); // Zero fill
                    }
                    else
                    {
                        RandomNumberGenerator.Fill(buffer); // Cryptographic random fill
                    }

                    while (written < fileLength)
                    {
                        var bytesToWrite = (int)Math.Min(buffer.Length, fileLength - written);
                        stream.Write(buffer, 0, bytesToWrite);
                        written += bytesToWrite;
                    }

                    stream.Flush(true);
                }
            }
        }

        /// <summary>
        /// Safely computes the recursive size of a directory.
        /// </summary>
        public static long GetDirectorySize(string dirPath, out int fileCount, out int dirCount)
        {
            fileCount = 0;
            dirCount = 0;
            if (string.IsNullOrWhiteSpace(dirPath) || !Directory.Exists(dirPath))
                return 0;

            long totalSize = 0;
            var queue = new Queue<string>();
            queue.Enqueue(dirPath);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                try
                {
                    var di = new DirectoryInfo(current);
                    if ((di.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                        continue; // Do not traverse symlinks

                    foreach (var fi in di.GetFiles())
                    {
                        try
                        {
                            totalSize += fi.Length;
                            fileCount++;
                        }
                        catch { }
                    }

                    foreach (var sub in di.GetDirectories())
                    {
                        try
                        {
                            if ((sub.Attributes & FileAttributes.ReparsePoint) == 0)
                            {
                                queue.Enqueue(sub.FullName);
                                dirCount++;
                            }
                        }
                        catch { }
                    }
                }
                catch
                {
                    // Skip inaccessible folders
                }
            }

            return totalSize;
        }
    }
}
