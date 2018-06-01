using Klocman.Tools;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace UninstallTools.Factory
{
    public sealed class ChocolateyFactory : IUninstallerFactory
    {
        private static bool? _chocoIsAvailable;
        private static string _chocoLocation;

        internal static string ChocoLocation
        {
            get
            {
                if (_chocoLocation == null)
                    GetChocoInfo();
                return _chocoLocation;
            }
            private set { _chocoLocation = value; }
        }

        internal static bool ChocoIsAvailable
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

        private static string StartProcessAndReadOutput(string filename, string args)
        {
            using (var process = Process.Start(new ProcessStartInfo(filename, args)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = false,
                CreateNoWindow = true,
            })) return process?.StandardOutput.ReadToEnd();
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

            var result = StartProcessAndReadOutput(ChocoLocation, @"list -l -nocolor -y -r");

            if (string.IsNullOrEmpty(result)) yield break;

            var appEntries = result.Split(NewlineSeparators, StringSplitOptions.RemoveEmptyEntries);
            var appNames = appEntries.Select(x =>
            {
                var i = x.LastIndexOf('|');
                if (i <= 0) return null;
                return new { name = x.Substring(0, i), version = x.Substring(i + 1) };
            });

            foreach (var appName in appNames)
            {
                var info = StartProcessAndReadOutput(ChocoLocation, "info -l -nocolor -y -v " + appName.name);
                var kvps = ExtractPackageInformation(info);
                if (kvps.Count == 0) continue;

                var entry = new ApplicationUninstallerEntry();

                AddInfo(entry, kvps, "Title", (e, s) => e.RawDisplayName = s);

                entry.DisplayVersion = appName.version;
                entry.RatingId = "Choco " + appName.name;
                entry.UninstallerKind = UninstallerType.Chocolatey;

                AddInfo(entry, kvps, "Tags", (e, s) => e.Comment = s);

                AddInfo(entry, kvps, "Documentation", (e, s) => e.AboutUrl = s);
                if (string.IsNullOrEmpty(entry.AboutUrl))
                    AddInfo(entry, kvps, "Software Site", (e, s) => e.AboutUrl = s);

                entry.UninstallString = $"\"{ChocoLocation}\" uninstall {appName.name} -y -r";

                yield return entry;
            }

        }

        private static void AddInfo(ApplicationUninstallerEntry target, Dictionary<string, string> source, string key, Action<ApplicationUninstallerEntry, string> setter)
        {
            if (source.TryGetValue(key, out var val))
            {
                try
                {
                    setter(target, val);
                }
                catch (SystemException ex)
                {
                    Console.WriteLine("Exception while extracting info from choco: " + ex.Message);
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
    }
}