/*
    OpenUninstall Pro - Open Source Professional Windows Uninstaller
    Uninstaller Types Definition
*/

using Klocman.Localising;
using UninstallTools.Properties;

namespace UninstallTools
{
    public enum UninstallerType
    {
        [LocalisedName(typeof(Localisation), "UninstallerType_Unknown")]
        Unknown = 0,
        [LocalisedName(typeof(Localisation), "UninstallerType_Msiexec")]
        Msiexec,
        [LocalisedName(typeof(Localisation), "UninstallerType_InnoSetup")]
        InnoSetup,
        [LocalisedName(typeof(Localisation), "UninstallerType_Steam")]
        Steam,
        [LocalisedName(typeof(Localisation), "UninstallerType_NSIS")]
        Nsis,
        [LocalisedName(typeof(Localisation), "UninstallerType_InstallShield")]
        InstallShield,
        [LocalisedName(typeof(Localisation), "UninstallerType_SdbInst")]
        SdbInst,
        [LocalisedName(typeof(Localisation), "UninstallerType_WindowsFeature")]
        WindowsFeature,
        [LocalisedName(typeof(Localisation), "UninstallerType_WindowsUpdate")]
        WindowsUpdate,
        [LocalisedName(typeof(Localisation), "UninstallerType_StoreApp")]
        StoreApp,
        [LocalisedName(typeof(Localisation), "UninstallerType_SimpleDelete")]
        SimpleDelete,
        [LocalisedName(typeof(Localisation), "UninstallerType_Chocolatey")]
        Chocolatey,
        [LocalisedName(typeof(Localisation), "UninstallerType_Oculus")]
        Oculus,
        [LocalisedName(typeof(Localisation), nameof(Localisation.UninstallerType_PowerShell))]
        PowerShell,
        EpicGames,
        Uplay,
        Origin,
        WinGet,
        Scoop
    }
}
