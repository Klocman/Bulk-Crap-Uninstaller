/*
    Copyright (c) 2018 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

namespace OculusHelper
{
    internal class OculusApp
    {
        public OculusApp(string canonicalName, string version, bool isCore, string installLocation, string launchFile)
        {
            CanonicalName = canonicalName;
            Version = version;
            IsCore = isCore;
            InstallLocation = installLocation;
            LaunchFile = launchFile;
        }

        public string CanonicalName { get; }
        public string Version { get; }
        public bool IsCore { get; }
        public string InstallLocation { get; }
        public string LaunchFile { get; }
    }
}