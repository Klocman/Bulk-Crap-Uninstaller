/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

namespace UninstallTools.Uninstaller
{
    public sealed class BulkUninstallConfiguration
    {
        /*public BulkUninstallConfiguration()
        {
            IgnoreProtection = false;
            PreferQuiet = false;
            QuietMsi = false;
            IntelligentSort = false;
            Simulate = false;
        }*/

        public BulkUninstallConfiguration(bool ignoreProtection, bool preferQuiet,
            bool simulate, bool autoKillStuckQuiet, bool retryFailedQuiet)
        {
            IgnoreProtection = ignoreProtection;
            PreferQuiet = preferQuiet;
            Simulate = simulate;
            AutoKillStuckQuiet = autoKillStuckQuiet;
            RetryFailedQuiet = retryFailedQuiet;
        }

        public bool AutoKillStuckQuiet { get; set; }
        public bool RetryFailedQuiet { get; set; }
        public bool IgnoreProtection { get; set; }
        public bool PreferQuiet { get; set; }
        public bool Simulate { get; set; }
    }
}