/*
    Copyright (c) 2018 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Xml.Linq;
using Klocman;
using Klocman.Extensions;
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
            if (assemblyLocation.EndsWith(".exe") || assemblyLocation.EndsWith(".dll"))
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

            using (var powerShell = PowerShell.Create(RunspaceMode.NewRunspace))
            {
                // Allow running script files
                powerShell.AddScript("Set-ExecutionPolicy -Scope Process -ExecutionPolicy Unrestricted");
                powerShell.Invoke();

                var manifests = Directory.GetFiles(ScriptDir, "*.xml", SearchOption.AllDirectories);

                foreach (var manifest in manifests)
                {
                    var contents = TryGetContents(manifest);

                    if (contents == null || !contents.HasElements) continue;

                    var manifestDirectoryName = Path.GetDirectoryName(manifest);

                    var conditionScript = contents.Element("ConditionScript")?.Value;
                    if (!string.IsNullOrEmpty(conditionScript))
                    {
                        var arguments = contents.Element("ConditionScriptArgs")?.Value;
                        if (!TestCondition(manifestDirectoryName, conditionScript, arguments, powerShell))
                            continue;
                    }

                    var output = new Dictionary<string, string>();

                    // todo Handle registry and relative paths on all items, including conditions
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
                                        ?.GetStringSafe(Path.GetFileName(fullPath));
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
        }

        private static bool TestCondition(string directoryPath, string scriptPath, string arguments,
            PowerShell powerShell)
        {
            if (!Path.IsPathRooted(scriptPath))
                scriptPath = Path.GetFullPath(Path.Combine(directoryPath, scriptPath));

            try
            {
                // Add the script with arguments
                var myCommand = new Command(scriptPath);

                if (!string.IsNullOrEmpty(arguments))
                {
                    var splitArguments = arguments.Trim()
                        .Split(new[] {'-'}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Split(new[] {' '}, 2, StringSplitOptions.RemoveEmptyEntries));

                    foreach (var splitArgument in splitArguments)
                    {
                        var testParam = splitArgument.Length < 2
                            ? new CommandParameter(splitArgument[0])
                            : new CommandParameter(splitArgument[0], splitArgument[1]);
                        myCommand.Parameters.Add(testParam);
                    }
                }
                powerShell.Commands.Clear();
                powerShell.Commands.AddCommand(myCommand);

                // Needed to get the script's result
                powerShell.Commands.AddScript("$LASTEXITCODE");
                
                var results = powerShell.Invoke();

                var errorCodeString = results.LastOrDefault()?.ToString();
                if (!int.TryParse(errorCodeString, out var errorCode))
                    throw new Exception("Invalid exit value of condition script - " + errorCodeString);

                // 0 = Script successful and test passed
                return errorCode == 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteExceptionToLog(ex);
                return false;
            }
        }

        private static string MakePsCommand(string directoryName, string scriptName, string scriptArgs, bool hidden)
        {
            try
            {
                if (!Path.IsPathRooted(scriptName))
                    scriptName = Path.GetFullPath(Path.Combine(directoryName, scriptName));

                return $@"powershell.exe -NoLogo -ExecutionPolicy Bypass {(hidden ? "-NonInteractive -WindowStyle Hidden " : "")}-File ""{scriptName}"" {scriptArgs}";
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
                LogWriter.WriteExceptionToLog(new Exception(
                    $@"Invalid script manifest file ""{manifest}"" - {ex.Message}", ex));
                return null;
            }
        }
    }
}