/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Klocman.Extensions;
using Klocman.IO;
using File = System.IO.File;

namespace UninstallTools.Factory.InfoAdders
{
    public class FastSizeGenerator : IMissingInfoAdder
    {
        private static readonly NativeMethods.IFileSystem3 _fileSystemObject;
        private static bool _everythingAvailable;

        static FastSizeGenerator()
        {
            try
            {
                _fileSystemObject = (NativeMethods.IFileSystem3)new NativeMethods.FileSystemObject();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(@"FastSizeGenerator: Scripting.FileSystemObjectClass is not available - " + ex.Message);
            }

            try
            {
                if (EvGetSize(UninstallToolsGlobalConfig.AssemblyLocation).GetKbSize() == 0)
                    throw new SystemException("Test failed to get valid BCU directory size");

                _everythingAvailable = true;
            }
            catch (Exception ex)
            {
                _everythingAvailable = false;
                Trace.WriteLine(@"FastSizeGenerator: Everything search engine is not available - " + ex.Message);
            }
        }

        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            if (!Directory.Exists(target.InstallLocation) || UninstallToolsGlobalConfig.IsSystemDirectory(target.InstallLocation))
                return;

            if (_everythingAvailable)
            {
                try
                {
                    target.EstimatedSize = EvGetSize(target.InstallLocation);
                    return;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                    _everythingAvailable = false;
                }
            }

            if (_fileSystemObject != null)
            {
                try
                {
                    var folder = _fileSystemObject.GetFolder(target.InstallLocation);
                    var size = new FileSize(Convert.ToInt64(folder.Size) / 1024);
                    target.EstimatedSize = size;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
            }
        }

        private static FileSize EvGetSize(string path)
        {
            path = Path.GetFullPath(path);
            var output = StartHelperAndReadOutput($"-size -a-d -size-leading-zero -no-digit-grouping -size-format 1 path:\"{path}\"").Result;
            var allResults = output.SplitNewlines(StringSplitOptions.RemoveEmptyEntries);

            long sum = 0;
            foreach (var result in allResults)
            {
                var split = result.Split(new[] { ' ' }, 2, StringSplitOptions.None);
                sum += long.Parse(split[0]);
            }
            return FileSize.FromBytes(sum);
        }

        private static async Task<string> StartHelperAndReadOutput(string args)
        {
            var esPath = Path.Combine(UninstallToolsGlobalConfig.AssemblyLocation, "es.exe");
            if (!File.Exists(esPath)) throw new FileNotFoundException();

            using (var process = Process.Start(new ProcessStartInfo(esPath, args)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8
            }))
            {
                if (process == null) throw new InvalidOperationException("Could not start a new process");

                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(40));
                var readOutputTask = process.StandardOutput.ReadToEndAsync();
                var readErrorTask = process.StandardError.ReadToEndAsync();

                await Task.WhenAny(readOutputTask, timeoutTask);

                if (!readOutputTask.IsCompleted)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch
                    {
                        // Ignore exceptions from killing the process
                    }
                    throw new TimeoutException("es.exe appears to have hung");
                }

                var output = await readOutputTask;
                var errorOutput = await readErrorTask;
                process.WaitForExit();

                if (process.ExitCode == 0) return output;

                var message = string.IsNullOrWhiteSpace(errorOutput)
                    ? "es.exe failed to connect to Everything"
                    : "es.exe failed to connect to Everything: " + errorOutput.Trim();
                throw new IOException(message, process.ExitCode);
            }
        }

        public string[] RequiredValueNames { get; } = { nameof(ApplicationUninstallerEntry.InstallLocation) };
        public bool RequiresAllValues { get; } = true;
        public bool AlwaysRun { get; } = false;
        public string[] CanProduceValueNames { get; } = { nameof(ApplicationUninstallerEntry.EstimatedSize) };
        public InfoAdderPriority Priority { get; } = InfoAdderPriority.RunLast;

        private static class NativeMethods
        {
            [ComImport]
            [Guid("0D43FE01-F093-11CF-8940-00A0C9054228")]
            internal class FileSystemObject { }

            [ComImport]
            [Guid("C7C3F5A4-88A3-11D0-ABCB-00A0C90FFFC0")]
            [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
            internal interface IFileSystem3
            {
                [DispId(0x00004e2c)]
                IFolder GetFolder(string folderPath);
            }

            [ComImport]
            [Guid("C7C3F5A2-88A3-11D0-ABCB-00A0C90FFFC0")]
            [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
            internal interface IFolder
            {
                [DispId(0x00004e24)]
                object Size { get; }
            }
        }
    }
}

