using Klocman.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UninstallTools.Properties;

namespace UninstallTools.Factory
{
    public sealed class ChocolateyFactory : IIndependantUninstallerFactory
    {
        private static bool? _chocoIsAvailable;
        private static string _chocoLocation;

        private static string ChocoFullFilename
        {
            get
            {
                if (_chocoLocation == null)
                    GetChocoInfo();
                return _chocoLocation;
            }
        }

        private static bool ChocoIsAvailable
        {
            get
            {
                if (!_chocoIsAvailable.HasValue)
                {
                    _chocoIsAvailable = false;
                    GetChocoInfo();
                }
                return _chocoIsAvailable.Value;
            }
        }

        private static void GetChocoInfo()
        {
            try
            {
                var chocoPath = PathTools.GetFullPathOfExecutable("choco.exe");
                if (string.IsNullOrEmpty(chocoPath)) return;

                var result = StartProcessAndReadOutput(chocoPath, string.Empty);
                if (result.StartsWith("Chocolatey", StringComparison.Ordinal))
                {
                    _chocoLocation = chocoPath;
                    _chocoIsAvailable = true;
                }
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static readonly string[] NewlineSeparators = StringTools.NewLineChars.ToArray();

        public IEnumerable<ApplicationUninstallerEntry> GetUninstallerEntries(ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            if (!ChocoIsAvailable) yield break;

            var result = StartProcessAndReadOutput(ChocoFullFilename, @"list -l -nocolor -y -r");

            if (string.IsNullOrEmpty(result)) yield break;

            var appEntries = result.Split(NewlineSeparators, StringSplitOptions.RemoveEmptyEntries);
            var appNames = appEntries.Select(x =>
            {
                var i = x.IndexOf('|');
                if (i <= 0) return null;
                return new { name = x.Substring(0, i), version = x.Substring(i + 1) };
            }).Where(x => x != null);

            foreach (var appName in appNames)
            {
                var info = StartProcessAndReadOutput(ChocoFullFilename, "info -l -nocolor -y -v " + appName.name);
                var kvps = ExtractPackageInformation(info);
                if (kvps.Count == 0) continue;

                var entry = new ApplicationUninstallerEntry();

                AddInfo(entry, kvps, "Title", (e, s) => e.RawDisplayName = s);

                entry.DisplayVersion = appName.version;
                entry.RatingId = "Choco " + appName.name;
                entry.UninstallerKind = UninstallerType.Chocolatey;

                AddInfo(entry, kvps, "Summary", (e, s) => e.Comment = s);
                if (string.IsNullOrEmpty(entry.Comment))
                {
                    AddInfo(entry, kvps, "Description", (e, s) => e.Comment = s);
                    if (string.IsNullOrEmpty(entry.Comment))
                        AddInfo(entry, kvps, "Tags", (e, s) => e.Comment = s);
                }

                AddInfo(entry, kvps, "Documentation", (e, s) => e.AboutUrl = s);
                if (string.IsNullOrEmpty(entry.AboutUrl))
                {
                    AddInfo(entry, kvps, "Software Site", (e, s) => e.AboutUrl = s);
                    if (string.IsNullOrEmpty(entry.AboutUrl))
                        AddInfo(entry, kvps, "Chocolatey Package Source", (e, s) => e.AboutUrl = s);
                }

                var psc = new ProcessStartCommand(ChocoFullFilename, $"uninstall {appName.name} -y -r");

                entry.UninstallString = psc.ToString();

                if (entry.RawDisplayName == "Chocolatey")
                    entry.InstallLocation = GetChocoInstallLocation();

                // Prevent chocolatey from trying to run the original uninstaller (it's deleted by now), only remove the package
                psc.Arguments += " -n --skipautouninstaller";
                var junk = new Junk.Containers.RunProcessJunk(entry, null, psc, Localisation.ChocolateyFactory_UninstallInChocolateyJunkName);
                junk.Confidence.Add(Junk.Confidence.ConfidenceRecords.ExplicitConnection);
                entry.AdditionalJunk.Add(junk);

                yield return entry;
            }
        }

        private static string GetChocoInstallLocation()
        {
            // The path is C:\ProgramData\chocolatey\bin\choco.exe OR C:\ProgramData\chocolatey\choco.exe
            var chocoLocation = Path.GetDirectoryName(ChocoFullFilename);
            if (chocoLocation != null && chocoLocation.EndsWith(@"\bin", StringComparison.OrdinalIgnoreCase))
                return chocoLocation.Substring(0, chocoLocation.Length - 4);
            return chocoLocation;
        }

        private static void AddInfo(ApplicationUninstallerEntry target, Dictionary<string, string> source,
            string key, Action<ApplicationUninstallerEntry, string> setter)
        {
            if (source.TryGetValue(key, out var val))
            {
                try
                {
                    setter(target, val);
                }
                catch (SystemException ex)
                {
                    Console.WriteLine(@"Exception while extracting info from choco: " + ex.Message);
                }
            }
        }

        private static Dictionary<string, string> ExtractPackageInformation(string result)
        {
            // Parse the console output into lines, then into key-value pairs
            var lines = result.Split(NewlineSeparators, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.Length > 2 && x[0] == ' ' && x[1] != ' ' && x.Contains(": "))
                .Select(x => x.TrimStart())
                .SelectMany(x => x.Split(new[] { " | " }, StringSplitOptions.None));

            var kvps = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                var i = line.IndexOf(": ", StringComparison.Ordinal);
                if (i <= 0) continue;

                var key = line.Substring(0, i);
                var val = line.Substring(i + 2);
                if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(val)) continue;

                kvps.Add(key, val);
            }

            return kvps;
        }

        public bool IsEnabled() => UninstallToolsGlobalConfig.ScanChocolatey;
        public string DisplayName => Localisation.Progress_AppStores_Chocolatey;

        private static string StartProcessAndReadOutput(string filename, string args)
        {
            using (var process = Process.Start(new ProcessStartInfo(filename, args)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.Default
            })) return process?.StandardOutput.ReadToEnd();
        }
    }
}