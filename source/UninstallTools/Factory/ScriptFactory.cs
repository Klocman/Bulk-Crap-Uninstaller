using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Klocman.Tools;

namespace UninstallTools.Factory
{
    public class ScriptFactory : IUninstallerFactory
    {
        private static readonly string ScriptDir;
        private static readonly PropertyInfo[] EntryProps;
        private static readonly PropertyInfo[] SystemIconProps;

        private static bool PowershellExists { get; }

        static ScriptFactory()
        {
            ScriptDir = Path.Combine(UninstallToolsGlobalConfig.AssemblyLocation, @"Resources\Scripts");

            EntryProps = typeof(ApplicationUninstallerEntry)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanWrite && p.PropertyType == typeof(string))
                .ToArray();

            SystemIconProps = typeof(SystemIcons)
                .GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Where(p => p.CanRead)
                .ToArray();

            try
            {
                PowershellExists = File.Exists(PathTools.GetFullPathOfExecutable("powershell.exe"));
            }
            catch (SystemException)
            {
                PowershellExists = false;
            }
        }

        public IEnumerable<ApplicationUninstallerEntry> GetUninstallerEntries(ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            if (!Directory.Exists(ScriptDir)) yield break;

            var manifests = Directory.GetFiles(ScriptDir, "*.xml", SearchOption.AllDirectories);

            foreach (var manifest in manifests)
            {
                var contents = TryGetContents(manifest);

                if (contents == null || !contents.HasElements) continue;

                var manifestDirectoryName = Path.GetDirectoryName(manifest);

                var conditionScript = contents.Element("ConditionScript")?.Value;
                if (!string.IsNullOrEmpty(conditionScript))
                {
                    var psc = MakePsCommand(manifestDirectoryName, conditionScript, contents.Element("ConditionScriptArgs")?.Value, true);
                    if (!string.IsNullOrEmpty(psc))
                    {
                        if (!PowershellExists || !CheckCondition(psc))
                            continue;
                    }
                }

                var entry = new ApplicationUninstallerEntry();

                // Automatically fill in any supplied static properties
                foreach (var entryProp in EntryProps)
                {
                    var item = contents.Element(entryProp.Name)?.Value;
                    if (item != null)
                    {
                        try
                        {
                            const string registryHeader = "Registry::";
                            if (item.StartsWith(registryHeader, StringComparison.OrdinalIgnoreCase))
                            {
                                var fullPath = item.Substring(registryHeader.Length);
                                item = RegistryTools.OpenRegistryKey(Path.GetDirectoryName(fullPath))
                                    ?.GetValue(Path.GetFileName(fullPath))
                                    ?.ToString();
                            }

                            entryProp.SetValue(entry, item, null);
                        }
                        catch (SystemException ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }

                // Override any static uninstall strings if valid script is supplied
                var script = contents.Element("Script")?.Value;
                if (!string.IsNullOrEmpty(script))
                {
                    var scriptArgs = contents.Element("ScriptArgs")?.Value;
                    var psc = MakePsCommand(manifestDirectoryName, script, scriptArgs, false);
                    if (!string.IsNullOrEmpty(psc))
                    {
                        if (!PowershellExists)
                            continue;

                        entry.UninstallString = psc;
                        entry.QuietUninstallString = MakePsCommand(manifestDirectoryName, script, scriptArgs, true);
                        entry.UninstallerKind = UninstallerType.PowerShell;
                    }
                }

                if (entry.UninstallPossible || entry.QuietUninstallPossible)
                {
                    if (string.IsNullOrEmpty(entry.DisplayName))
                        entry.DisplayName = Path.GetFileNameWithoutExtension(manifest);
                    if (string.IsNullOrEmpty(entry.Publisher))
                        entry.Publisher = "Script";

                    var icon = contents.Element("SystemIcon")?.Value;
                    if (!string.IsNullOrEmpty(icon))
                    {
                        var iconObj = SystemIconProps.FirstOrDefault(p => p.Name.Equals(icon, StringComparison.OrdinalIgnoreCase))
                            ?.GetValue(null, null) as Icon;
                        entry.IconBitmap = iconObj;
                    }
                    
                    if (string.IsNullOrEmpty(entry.RatingId))
                        entry.RatingId = manifest.Remove(0, ScriptDir.Length).Trim(' ', '\\', '/').ToLowerInvariant().Replace(".xml", "", StringComparison.Ordinal);

                    yield return entry;
                }
            }
        }

        private static bool CheckCondition(string psc)
        {
            try
            {
                var startInfo = ProcessTools.SeparateArgsFromCommand(psc).ToProcessStartInfo();
                startInfo.CreateNoWindow = true;
                startInfo.ErrorDialog = false;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                var p = Process.Start(startInfo);
                if (p == null) return false;

                if (!p.WaitForExit(5000))
                {
                    Console.WriteLine(@"Script's ConditionScript command timed out - " + psc);
                    p.Kill();
                    return false;
                }

                return p.ExitCode == 0;
            }
            catch (SystemException ex)
            {
                Console.WriteLine($@"Failed to test script condition ""{psc}"" - {ex.Message}");
                return false;
            }
        }

        private static string MakePsCommand(string directoryName, string scriptName, string scriptArgs, bool hidden)
        {
            try
            {
                if (!Path.IsPathRooted(scriptName))
                    scriptName = Path.GetFullPath(Path.Combine(directoryName, scriptName));

                return $@"powershell.exe -File ""{scriptName}"" {scriptArgs} -NoLogo -ExecutionPolicy Bypass {(hidden ? "-NonInteractive -WindowStyle Hidden" : "")}";
            }
            catch (SystemException)
            {
                return null;
            }
        }

        private static XElement TryGetContents(string manifest)
        {
            try
            {
                return XDocument.Parse(File.ReadAllText(manifest)).Root;
            }
            catch (SystemException ex)
            {
                Console.WriteLine($@"Invalid script manifest file ""{manifest}"" - {ex.Message}");
                return null;
            }
        }
    }
}