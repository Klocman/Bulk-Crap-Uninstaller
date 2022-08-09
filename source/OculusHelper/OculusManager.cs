/*
    Copyright (c) 2018 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using Klocman;
using Klocman.Tools;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace OculusHelper
{
    internal static class OculusManager
    {
        private static IEnumerable<string> _oculusLibraryLocations;

        private static IEnumerable<string> OculusLibraryLocations =>
            _oculusLibraryLocations ?? (_oculusLibraryLocations = FindOculusLibraryLocations());

        private static IEnumerable<string> FindOculusLibraryLocations()
        {
            var libPaths = new List<string>();

            // Default library is in install dir and is not listed in the Libraries key.
            foreach (var softwareKey in new[]
                {@"SOFTWARE\Oculus VR, LLC\Oculus", @"SOFTWARE\WOW6432Node\Oculus VR, LLC\Oculus"})
                try
                {
                    using (var key = Registry.LocalMachine.OpenSubKey(softwareKey))
                    {
                        if (key != null)
                            if (key.GetValue("Base", null, RegistryValueOptions.None) is string path)
                                libPaths.Add(path);
                    }
                }
                catch (SystemException ex)
                {
                    Console.WriteLine(ex);
                }

            const string oculusLibPath = @"Software\Oculus VR, LLC\Oculus\Libraries";

            // Each user can have different libaries set up
            foreach (var userName in Registry.Users.GetSubKeyNames())
                try
                {
                    using (var key = Registry.Users.OpenSubKey(Path.Combine(userName, oculusLibPath), false))
                    {
                        if (key == null) continue;

                        foreach (var libKeyName in key.GetSubKeyNames())
                            using (var libKey = key.OpenSubKey(libKeyName))
                            {
                                if (libKey != null)
                                    if (libKey.GetValue("Path", null, RegistryValueOptions.None) is string path)
                                        libPaths.Add(path);
                            }
                    }
                }
                catch (SystemException ex)
                {
                    Console.WriteLine(ex);
                }

            return libPaths.Select(x => x.Trim().ToLowerInvariant())
                .Select(PathTools.ResolveVolumeIdToPath)
                .Where(Directory.Exists)
                .Distinct();
        }

        public static IEnumerable<OculusApp> QueryOculusApps()
        {
            var apps = new List<OculusApp>();

            foreach (var lib in OculusLibraryLocations)
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

        public static void RemoveApp(string canonicalName)
        {
            Console.WriteLine("Looking for apps with canonical name: " + canonicalName);

            var apps = QueryOculusApps()
                .Where(x => canonicalName.Equals(x.CanonicalName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (apps.Count == 0)
            {
                Console.WriteLine("Invalid app name or app can't be uninstalled");
                throw new Exception($"{canonicalName} is not a recognized or allowed app name");
            }

            foreach (var app in apps)
                RemoveApp(app);
        }

        public static void RemoveApp(OculusApp app)
        {
            CloseOculusClient();

            Console.WriteLine("Removing Oculus app: " + app.CanonicalName);
            Debug.Assert(app.CanonicalName.Length > 10);

            foreach (var libraryLocation in OculusLibraryLocations)
            {
                // Collect paths first to avoid crashing halfway
                var dirs = Directory.GetDirectories(libraryLocation,
                    $"*{app.CanonicalName}*", SearchOption.AllDirectories);
                var files = Directory.GetFiles(libraryLocation,
                    $"*{app.CanonicalName}*", SearchOption.AllDirectories);

                foreach (var path in dirs)
                {
                    Console.WriteLine("Deleting " + path);
                    Directory.Delete(path, true);
                }

                foreach (var path in files)
                {
                    Console.WriteLine("Deleting " + path);
                    File.Delete(path);
                }
            }

            RestartOculusService();

            Console.WriteLine("Finished");
        }

        /// <summary>
        /// After removing an app the oculus library service has to be restarted to update the state.
        /// </summary>
        private static void RestartOculusService()
        {
            try
            {
                // Library service can't be restarted by itself, have to do it through OVRService
                var serviceController = new ServiceController(@"OVRService");

                if(serviceController.Status.Equals(ServiceControllerStatus.Stopped)) return;

                if (serviceController.Status.Equals(ServiceControllerStatus.Running) || 
                    serviceController.Status.Equals(ServiceControllerStatus.StartPending))
                {
                    Console.WriteLine("Stopping Oculus VR Services...");
                    serviceController.Stop();
                }
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(20));
                Console.WriteLine("Restarting Oculus VR Services...");
                serviceController.Start();
                serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(20));
            }
            catch (SystemException ex)
            {
                Console.WriteLine("Restarting Oculus VR Service failed - " + ex.Message);
            }
        }

        private static void CloseOculusClient()
        {
            var processes = Process.GetProcessesByName("OculusClient");

            if (processes.Length == 0) return;

            Console.WriteLine("Closing Oculus Client...");

            foreach (var process in processes)
            {
                try
                {
                    if (!process.HasExited)
                        process.Kill(true);
                }
                catch (SystemException ex)
                {
                    Console.WriteLine($"Failed to kill process {process.ProcessName} - {ex.Message}");
                    Console.WriteLine("Please close the Oculus Client manually to continue.");
                }
            }

            Console.WriteLine("Waiting for Oculus Client to close...");

            while (processes.Any(x=>!x.HasExited))
            {
                Thread.Sleep(250);
            }
        }
    }
}