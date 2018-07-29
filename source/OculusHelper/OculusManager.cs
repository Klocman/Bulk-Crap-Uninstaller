using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klocman;
using Klocman.Tools;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace OculusHelper
{
    internal class OculusManager
    {
        private static IEnumerable<string> FindOculusLibraryLocations()
        {
            var libPaths = new List<string>();

            // Default library is in install dir and is not listed in the Libraries key.
            foreach (var softwareKey in new[] { @"SOFTWARE\Oculus VR, LLC\Oculus", @"SOFTWARE\WOW6432Node\Oculus VR, LLC\Oculus" })
            {
                try
                {
                    using (var key = Registry.LocalMachine.OpenSubKey(softwareKey))
                    {
                        if (key != null)
                        {
                            if (key.GetValue("Base", null, RegistryValueOptions.None) is string path)
                                libPaths.Add(path);
                        }
                    }
                }
                catch (SystemException ex)
                {
                    Console.WriteLine(ex);
                }
            }

            const string oculusLibPath = @"Software\Oculus VR, LLC\Oculus\Libraries";

            // Each user can have different libaries set up
            foreach (var userName in Registry.Users.GetSubKeyNames())
            {
                try
                {
                    using (var key = Registry.Users.OpenSubKey(Path.Combine(userName, oculusLibPath), false))
                    {
                        if (key == null) continue;

                        foreach (var libKeyName in key.GetSubKeyNames())
                        {
                            using (var libKey = key.OpenSubKey(libKeyName))
                            {
                                if (libKey != null)
                                {
                                    if (libKey.GetValue("Path", null, RegistryValueOptions.None) is string path)
                                        libPaths.Add(path);
                                }
                            }
                        }
                    }
                }
                catch (SystemException ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return libPaths.Select(x => x.Trim().ToLowerInvariant())
                .Select(PathTools.ResolveVolumeIdToPath)
                .Where(Directory.Exists)
                .Distinct();
        }

        public static IEnumerable<OculusApp> QueryOculusApps()
        {
            var libs = FindOculusLibraryLocations();

            var apps = new List<OculusApp>();

            foreach (var lib in libs)
            {
                var software = Path.Combine(lib, "Software");
                //var support = Path.Combine(lib, "Support");
                var manifests = Path.Combine(lib, "Manifests");
                if (!Directory.Exists(manifests)) continue;

                var jsonFiles = Directory.GetFiles(manifests)
                    .Where(x => x.EndsWith(".json", StringComparison.OrdinalIgnoreCase));
                foreach (var jsonFile in jsonFiles)
                {
                    if (!File.Exists(jsonFile)) continue;

                    try
                    {
                        var json = JsonConvert.DeserializeXNode(File.ReadAllText(jsonFile), "root")?.Root;
                        if (json == null) continue;

                        var name = json.Element("canonicalName")?.Value;
                        if (string.IsNullOrWhiteSpace(name)) continue;

                        var installLoc = Path.Combine(software, name);
                        if (!Directory.Exists(installLoc)) continue;

                        var launchFile = json.Element("launchFile")?.Value;
                        var executable = string.IsNullOrWhiteSpace(launchFile)
                            ? null
                            : Path.Combine(installLoc, launchFile);

                        apps.Add(new OculusApp(
                            name,
                            json.Element("version")?.Value,
                            "true".Equals(json.Element("isCore")?.Value, StringComparison.OrdinalIgnoreCase),
                            installLoc,
                            executable));
                    }
                    catch (SystemException ex)
                    {
                        LogWriter.WriteExceptionToLog(ex);
                    }
                }
            }

            return apps;
        }
    }
}