/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

namespace UninstallTools.Factory.InfoAdders
{
    public class InnoSetupQuietUninstallStringGenerator : IMissingInfoAdder
    {
        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            if (target.UninstallerKind != UninstallerType.InnoSetup || string.IsNullOrEmpty(target.UninstallerFullFilename))
                return;

            if (!UninstallToolsGlobalConfig.QuietAutomatization)
                return;

            /* Use UninstallerFullFilename to get rid of quotes and arguments (InnoSetup doesn't need any arguments)
             * 
             * Unlike '/SILENT', when '/VERYSILENT' is specified, the uninstallation progress window is not displayed.
             * -> But if a restart is necessary and the '/NORESTART' command isn't used, the uninstaller will reboot without asking.
             * 
             * /SUPPRESSMSGBOXES Instructs the uninstaller to suppress message boxes. Doesn't affect boxes from spawned processes.
             */
            target.QuietUninstallString = $"\"{target.UninstallerFullFilename}\" /VERYSILENT /SUPPRESSMSGBOXES /NORESTART";
        }

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallerFullFilename),
            nameof(ApplicationUninstallerEntry.UninstallerKind)
        };

        public bool RequiresAllValues { get; } = true;
        public bool AlwaysRun { get; } = true;

        public string[] CanProduceValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.QuietUninstallString)
        };

        public InfoAdderPriority Priority { get; } = InfoAdderPriority.Normal;
    }
}