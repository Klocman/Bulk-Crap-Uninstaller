namespace UninstallTools.Factory.InfoAdders
{
    public class SimpleDeleteUninstallStringGenerator : IMissingInfoAdder
    {
        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            if (target.UninstallerKind != UninstallerType.SimpleDelete)
                return;

            if (target.UninstallString == null)
                target.UninstallString = $"cmd.exe /C del /S \"{target.InstallLocation}\\\" && pause";

            if (target.QuietUninstallString == null)
                target.QuietUninstallString =
                    $"cmd.exe /C del /F /S /Q \"{target.InstallLocation}\\\"";
        }

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallerKind),
            nameof(ApplicationUninstallerEntry.InstallLocation)
        };
        public bool RequiresAllValues { get; } = true;

        public string[] CanProduceValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallString),
            nameof(ApplicationUninstallerEntry.QuietUninstallString)
        };

        public InfoAdderPriority Priority { get; } = InfoAdderPriority.RunLast;
    }
}