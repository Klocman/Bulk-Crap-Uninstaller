using Klocman.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UninstallTools.Properties;

namespace UninstallTools.Factory
{
    public sealed class ChocolateyFactory : IIndependantUninstallerFactory
    {
        private static readonly TimeSpan ChocolateyCommandTimeout = TimeSpan.FromSeconds(45);

        private static bool GetChocoInfo(out string chocoLocation)
        {
            chocoLocation = null;
            try
            {
                var chocoPath = PathTools.GetFullPathOfExecutable("choco.exe");
                if (string.IsNullOrEmpty(chocoPath)) return false;

                var result = StartProcessAndReadOutput(chocoPath, string.Empty);
                if (result.StartsWith("Chocolatey", StringComparison.Ordinal))
                {
                    chocoLocation = chocoPath;
                    return true;
                }
            }
            catch (SystemException ex)
            {
                Trace.WriteLine("Failed to get Choco info: " + ex);
            }
            return false;
        }

        private static readonly string[] NewlineSeparators = StringTools.NewLineChars.ToArray();

        public IList<ApplicationUninstallerEntry> GetUninstallerEntries(ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            var results = new List<ApplicationUninstallerEntry>();

            // Check on every reload in case Chocolatey was uninstalled since last reload
            if (!GetChocoInfo(out var chocoFullFilename)) return results;
            var versionResult = StartProcessAndReadOutput(chocoFullFilename, "--version");
            if (string.IsNullOrEmpty(versionResult)) return results;

            var versionString = versionResult.Trim();
            string result;
            if (Version.TryParse(versionString, out var version))
            {
                if (version >= new Version(2, 0, 0))
                {
                    result = StartProcessAndReadOutput(chocoFullFilename, @"list -nocolor --detail");
                }
                else
                {
                    result = StartProcessAndReadOutput(chocoFullFilename, @"list -lo -nocolor --detail");
                }
            }
            else
            {
                Trace.WriteLine("Failed to parse Chocolatey version: " + versionString);
                return results;
            }
            
            if (string.IsNullOrEmpty(result)) return results;

            var re = new System.Text.RegularExpressions.Regex(@"\n\w.+\r\n Title:");
            var match = re.Match(result);
            if (!match.Success) return results;
            var begin = match.Index + 1;

            while (true)
            {
                match = match.NextMatch();
                if (!match.Success) break;
                var end = match.Index + 1;
                var info = result.Substring(begin, end - begin);
                int i = info.IndexOf(' '), j = info.IndexOf("\r\n", StringComparison.Ordinal);
                if (i <= 0 || j <= i + 1)
                {
                    Trace.WriteLine("Failed to parse Chocolatey entry block: " + info);
                    begin = end;
                    continue;
                }
                var appName = new { name = info.Substring(0, i), version = info.Substring(i + 1, j - i - 1) };

                var kvps = ExtractPackageInformation(info);
                if (kvps.Count == 0) continue;

                var entry = new ApplicationUninstallerEntry();

                AddInfo(entry, kvps, "Title", (e, s) => e.RawDisplayName = s);

                entry.DisplayVersion = ApplicationEntryTools.CleanupDisplayVersion(appName.version);
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

                var psc = new ProcessStartCommand(chocoFullFilename, $"uninstall {appName.name} -y -r");

                entry.UninstallString = psc.ToString();

                if (entry.RawDisplayName == "Chocolatey")
                    entry.InstallLocation = GetChocoInstallLocation(chocoFullFilename);

                // Prevent chocolatey from trying to run the original uninstaller (it's deleted by now), only remove the package
                psc.Arguments += " -n --skipautouninstaller";
                var junk = new Junk.Containers.RunProcessJunk(entry, null, psc, Localisation.ChocolateyFactory_UninstallInChocolateyJunkName);
                junk.Confidence.Add(Junk.Confidence.ConfidenceRecords.ExplicitConnection);
                junk.Confidence.Add(4);
                entry.AdditionalJunk.Add(junk);

                results.Add(entry);
                begin = end;
            }

            return results;
        }

        private static string GetChocoInstallLocation(string chocoFullFilename)
        {
            // The path is C:\ProgramData\chocolatey\bin\choco.exe OR C:\ProgramData\chocolatey\choco.exe
            var chocoLocation = Path.GetDirectoryName(chocoFullFilename);
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
                    Trace.WriteLine(@"Exception while extracting info from choco: " + ex.Message);
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

                kvps[key] = val;
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
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.Default
            }))
            {
                if (process == null)
                {
                    Trace.WriteLine($"[Factory] Failed to start command {filename} {args}");
                    return null;
                }

                var sw = Stopwatch.StartNew();
                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();

                if (!process.WaitForExit((int)ChocolateyCommandTimeout.TotalMilliseconds))
                {
                    try
                    {
                        process.Kill(true);
                    }
                    catch
                    {
                        // Best effort only.
                    }

                    Trace.WriteLine($"[Factory] Chocolatey command timed out after {ChocolateyCommandTimeout.TotalSeconds:F0}s: {filename} {args}");
                    return null;
                }

                try
                {
                    Task.WaitAll(new[] { outputTask, errorTask }, ChocolateyCommandTimeout);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"[Factory] Failed to read Chocolatey output for {filename} {args} - {ex}");
                    return null;
                }

                var output = outputTask.Result;
                var error = errorTask.Result;
                Trace.WriteLine($"[Performance] Running command {filename} {args} took {sw.ElapsedMilliseconds}ms");

                if (process.ExitCode != 0)
                {
                    Trace.WriteLine($"[Factory] Chocolatey command exited with code {process.ExitCode}. stderr={error}");
                    return null;
                }

                if (!string.IsNullOrWhiteSpace(error))
                    Trace.WriteLine($"[Factory] Chocolatey command produced stderr output: {error}");

                return output;
            }
        }
    }
}
