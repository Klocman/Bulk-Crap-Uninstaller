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

        public BulkUninstallConfiguration(bool ignoreProtection, bool preferQuiet, bool intelligentSort,
            bool simulate)
        {
            IgnoreProtection = ignoreProtection;
            PreferQuiet = preferQuiet;
            IntelligentSort = intelligentSort;
            Simulate = simulate;
        }

        public bool IgnoreProtection { get; set; }
        public bool IntelligentSort { get; set; }
        public bool PreferQuiet { get; set; }
        public bool QuietAreSorted => PreferQuiet && IntelligentSort;
        public bool Simulate { get; set; }
    }
}