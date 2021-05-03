namespace Klocman.Binding.Settings
{
    public delegate void SettingChangedEventHandler<TProperty>(
        object sender, SettingChangedEventArgs<TProperty> args);
}