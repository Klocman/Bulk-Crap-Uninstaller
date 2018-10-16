/*
    Copyright (c) 2018 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Klocman;
using Klocman.Tools;

namespace ScriptHelper
{
    internal static class ScriptManager
    {
        private static readonly string ScriptDir;
        private static readonly bool PowershellExists;

        static ScriptManager()
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            if (assemblyLocation.EndsWith(".exe"))
                assemblyLocation = Path.GetDirectoryName(assemblyLocation);
            ScriptDir = Path.Combine(assemblyLocation ?? string.Empty, @"Resources\Scripts");

            try
            {
                PowershellExists = File.Exists(PathTools.GetFullPathOfExecutable("powershell.exe"));
            }
            catch (SystemException)
            {
                PowershellExists = false;
            }
        }

        public static IEnumerable<Dictionary<string, string>> GetScripts()
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
                    var psc = MakePsCommand(manifestDirectoryName, conditionScript,
                        contents.Element("ConditionScriptArgs")?.Value, true);
                    if (!string.IsNullOrEmpty(psc))
                        if (!PowershellExists || !CheckCondition(psc))
                            continue;
                }

                var output = new Dictionary<string, string>();

                // Automatically fill in any supplied static properties
                // todo Handle registry and relative paths on all items
                foreach (var entryProp in contents.Elements())
                {
                    var item = contents.Element(entryProp.Name)?.Value;
                    if (item != null)
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

                            output[entryProp.Name.ToString()] = item;
                        }
                        catch (SystemException ex)
                        {
                            LogWriter.WriteExceptionToLog(ex);
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

                        output["UninstallString"] = psc;
                        output["QuietUninstallString"] = MakePsCommand(manifestDirectoryName, script, scriptArgs, true);
                    }
                }

                string TryGetValue(string key)
                {
                    output.TryGetValue(key, out var o);
                    return o;
                }

                if (output.ContainsKey("UninstallString") || output.ContainsKey("QuietUninstallString"))
                {
                    if (string.IsNullOrEmpty(TryGetValue("DisplayName")))
                        output["DisplayName"] = Path.GetFileNameWithoutExtension(manifest);
                    if (string.IsNullOrEmpty(TryGetValue("Publisher")))
                        output["Publisher"] = "Script";


                    if (string.IsNullOrEmpty(TryGetValue("RatingId")))
                        output["RatingId"] = manifest.Remove(0, ScriptDir.Length).Trim(' ', '\\', '/')
                            .ToLowerInvariant().Replace(".xml", "", StringComparison.Ordinal);

                    yield return output;
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

                return $@"powershell.exe -File ""{scriptName}"" {scriptArgs} -NoLogo -ExecutionPolicy Bypass {
                        (hidden ? "-NonInteractive -WindowStyle Hidden" : "")
                    }";
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