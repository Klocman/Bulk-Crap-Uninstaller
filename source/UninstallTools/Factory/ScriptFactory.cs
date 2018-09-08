using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        static ScriptFactory()
        {
            ScriptDir = Path.Combine(UninstallToolsGlobalConfig.AssemblyLocation, @"Resources\Scripts");

            EntryProps = typeof(ApplicationUninstallerEntry)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanWrite && p.PropertyType == typeof(string))
                .ToArray();
        }

        public IEnumerable<ApplicationUninstallerEntry> GetUninstallerEntries(ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            if (!Directory.Exists(ScriptDir)) yield break;

            var manifests = Directory.GetFiles(ScriptDir, "*.xml", SearchOption.AllDirectories);

            foreach (var manifest in manifests)
            {
                var contents = TryGetContents(manifest);

                if (contents == null || !contents.HasElements) continue;

                var conditionScript = contents.Element("ConditionScript")?.Value;
                if (!string.IsNullOrEmpty(conditionScript))
                {
                    var psc = MakePsCommand(Path.GetDirectoryName(manifest), conditionScript, contents.Element("ConditionScriptArgs")?.Value);
                    if (!string.IsNullOrEmpty(psc))
                    {
                        if(!CheckCondition(psc))
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
                    var psc = MakePsCommand(Path.GetDirectoryName(manifest), script, contents.Element("ScriptArgs")?.Value);
                    if (!string.IsNullOrEmpty(psc))
                    {
                        entry.UninstallString = psc;
                        entry.QuietUninstallString = psc;
                        entry.UninstallerKind = UninstallerType.PowerShell;
                    }
                }

                if (entry.UninstallPossible || entry.QuietUninstallPossible)
                {
                    if (string.IsNullOrEmpty(entry.DisplayName))
                        entry.DisplayName = Path.GetFileNameWithoutExtension(manifest);
                    if (string.IsNullOrEmpty(entry.Publisher))
                        entry.Publisher = "Script";

                    yield return entry;
                }
            }
        }

        private static bool CheckCondition(string psc)
        {
            try
            {
                var p = Process.Start(ProcessTools.SeparateArgsFromCommand(psc).ToProcessStartInfo());
                return p != null && p.WaitForExit(1000) && p.ExitCode == 0;
            }
            catch (SystemException ex)
            {
                Console.WriteLine($@"Failed to test script condition ""{psc}"" - {ex.Message}");
                return false;
            }
        }

        private static string MakePsCommand(string directoryName, string scriptName, string scriptArgs)
        {
            try
            {
                if (!Path.IsPathRooted(scriptName))
                    scriptName = Path.GetFullPath(Path.Combine(directoryName, scriptName));

                const string psPrefix = "powershell.exe -NoLogo -ExecutionPolicy Bypass -File ";

                return $@"{psPrefix}""{scriptName}"" {scriptArgs}";
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