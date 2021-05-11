namespace Klocman.Binding.Settings
{
    internal interface ISettingChangedHandlerEntry
    {
        object Tag { get; set; }
        void SendEvent(object value);
    }
}