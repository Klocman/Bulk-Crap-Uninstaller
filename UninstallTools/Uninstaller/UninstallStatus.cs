using Klocman.Localising;
using UninstallTools.Properties;

namespace UninstallTools.Uninstaller
{
    public enum UninstallStatus
    {
        [LocalisedName(typeof (Localisation), "UninstallStatus_Waiting")] Waiting,
        [LocalisedName(typeof (Localisation), "UninstallStatus_Uninstalling")] Uninstalling,
        [LocalisedName(typeof (Localisation), "UninstallStatus_Completed")] Completed,
        [LocalisedName(typeof (Localisation), "UninstallStatus_Failed")] Failed,
        [LocalisedName(typeof (Localisation), "UninstallStatus_Skipped")] Skipped,
        [LocalisedName(typeof (Localisation), "UninstallStatus_Protected")] Protected,
        [LocalisedName(typeof (Localisation), "UninstallStatus_Invalid")] Invalid
    }
}