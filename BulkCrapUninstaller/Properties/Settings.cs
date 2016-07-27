using Klocman.Binding.Settings;

namespace BulkCrapUninstaller.Properties
{
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    internal sealed partial class Settings
    {
        private SettingBinder<Settings> _settingManager;

        public SettingBinder<Settings> SettingBinder
            => _settingManager ?? (_settingManager = new SettingBinder<Settings>(this));
    }
}