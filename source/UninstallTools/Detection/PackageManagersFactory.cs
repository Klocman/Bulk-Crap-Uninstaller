/*
    OpenUninstall Pro - Open Source Professional Windows Uninstaller
    Package Managers Discovery Factory (WinGet)
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Klocman.Tools;
using UninstallTools.Core;
using UninstallTools.Factory;

namespace UninstallTools.Detection
{
    public sealed class PackageManagersFactory : IUninstallerFactory
    {
        public IList<ApplicationUninstallerEntry> GetUninstallerEntries(ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            var results = new List<ApplicationUninstallerEntry>();
            StructuredLogger.Info(LogCategory.Discovery, "Discovering packages from WinGet");

            try
            {
                ScanWinGet(results);
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Discovery, "Failed scanning WinGet packages", ex.Message);
            }

            return results;
        }

        private static void ScanWinGet(List<ApplicationUninstallerEntry> results)
        {
            var wingetPath = PathTools.GetFullPathOfExecutable("winget.exe");
            if (string.IsNullOrWhiteSpace(wingetPath) || !File.Exists(wingetPath))
            {
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var candidate = Path.Combine(localAppData, "Microsoft", "WindowsApps", "winget.exe");
                if (File.Exists(candidate))
                    wingetPath = candidate;
                else
                    return;
            }

            var output = RunProcessAndReadOutput(wingetPath, "list --accept-source-agreements");
            if (string.IsNullOrWhiteSpace(output)) return;

            var lines = output.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var headerIndex = -1;
            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("Name", StringComparison.OrdinalIgnoreCase) && lines[i].Contains("Id"))
                {
                    headerIndex = i;
                    break;
                }
            }

            if (headerIndex < 0 || headerIndex + 1 >= lines.Length) return;

            // Header gives column positions
            var headerLine = lines[headerIndex];
            var idCol = headerLine.IndexOf("Id", StringComparison.OrdinalIgnoreCase);
            var verCol = headerLine.IndexOf("Version", StringComparison.OrdinalIgnoreCase);
            var srcCol = headerLine.IndexOf("Source", StringComparison.OrdinalIgnoreCase);

            if (idCol < 0 || verCol < 0) return;

            for (var i = headerIndex + 2; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.Length <= idCol) continue;

                var name = line.Substring(0, idCol).Trim();
                var id = (verCol > idCol && line.Length > verCol)
                    ? line.Substring(idCol, verCol - idCol).Trim()
                    : line.Substring(idCol).Trim();

                var version = (srcCol > verCol && line.Length > srcCol)
                    ? line.Substring(verCol, srcCol - verCol).Trim()
                    : (line.Length > verCol ? line.Substring(verCol).Trim() : string.Empty);

                if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(id) && !id.Contains(" "))
                {
                    var entry = new ApplicationUninstallerEntry
                    {
                        DisplayName = name,
                        DisplayVersion = version,
                        RatingId = $"WinGet_{id}",
                        UninstallerKind = UninstallerType.WinGet,
                        UninstallString = $"winget.exe uninstall --id \"{id}\"",
                        QuietUninstallString = $"winget.exe uninstall --id \"{id}\" --silent",
                        UninstallPossible = true,
                        QuietUninstallPossible = true,
                        IsRegistered = true
                    };
                    results.Add(entry);
                }
            }
        }

        private static string RunProcessAndReadOutput(string executable, string args)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = executable,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                };

                using var proc = Process.Start(psi);
                if (proc == null) return null;

                var output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit(10000);
                return output;
            }
            catch
            {
                return null;
            }
        }
    }
}
