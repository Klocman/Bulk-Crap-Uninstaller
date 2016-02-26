using Klocman.Localising;
using UninstallTools.Properties;

namespace UninstallTools.Uninstaller
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
        Dism, 
        [LocalisedName(typeof(Localisation), "UninstallerType_StoreApp")]
        StoreApp,
        [LocalisedName(typeof(Localisation), "UninstallerType_SimpleDelete")]
        SimpleDelete
    }
}