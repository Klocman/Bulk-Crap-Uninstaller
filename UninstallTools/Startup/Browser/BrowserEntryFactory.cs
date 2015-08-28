using System.Collections.Generic;
using Klocman.Tools;
using Microsoft.Win32;

namespace UninstallTools.Startup.Browser
{
    public static class BrowserEntryFactory
    {
        internal const string AutorunsDisabledKeyName = "AutorunsDisabled";

        private static readonly string[] RegistryStartupPoints =
        {
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Browser Helper Objects",
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\explorer\Browser Helper Objects"
        };

        private static readonly string ClsidPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\CLSID";

        public static IEnumerable<BrowserHelperEntry> GetBrowserHelpers()
        {
            using (var clsidKey = RegistryTools.OpenRegistryKey(ClsidPath))
            {
                foreach (var registryStartupPoint in RegistryStartupPoints)
                {
                    using (var mainKey = RegistryTools.CreateSubKeyRecursively(registryStartupPoint))
                    {
                        foreach (var browserHelperEntry in
                            GatherBrowserHelpersFromKey(mainKey, clsidKey, registryStartupPoint, false))
                            yield return browserHelperEntry;

                        using (var disabledKey = mainKey.OpenSubKey(AutorunsDisabledKeyName))
                        {
                            if (disabledKey == null) continue;

                            foreach (var browserHelperEntry in
                                GatherBrowserHelpersFromKey(disabledKey, clsidKey, registryStartupPoint, true))
                                yield return browserHelperEntry;
                        }
                    }
                }
            }
        }

        private static IEnumerable<BrowserHelperEntry> GatherBrowserHelpersFromKey(RegistryKey workingKey,
            RegistryKey clsidKey,
            string registryStartupPoint, bool disabled)
        {
            foreach (var registryKey in workingKey.GetSubKeyNames())
            {
                using (var classKey = clsidKey.OpenSubKey(registryKey))
                {
                    var name = classKey?.GetValue(null) as string;
                    if (string.IsNullOrEmpty(name))
                        continue;

                    string command;
                    using (var runKey = classKey.OpenSubKey("InProcServer32") ?? classKey.OpenSubKey("InProcServer"))
                    {
                        command = runKey?.GetValue(null) as string;
                    }

                    ProcessStartCommand startCommand = null;
                    if (!string.IsNullOrEmpty(command))
                    {
                        ProcessStartCommand.TryParse(command, out startCommand);
                    }

                    yield return new BrowserHelperEntry(name, command, startCommand?.FileName ?? string.Empty,
                        registryStartupPoint, registryKey, disabled, workingKey.Name.Contains("Wow6432Node"));
                }
            }
        }
    }
}