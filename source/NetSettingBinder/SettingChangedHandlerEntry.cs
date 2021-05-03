using System;

namespace Klocman.Binding.Settings
{
    /// <summary>
    ///     This class contains the information required to send the event back to the subscriber
    /// </summary>
    internal sealed class SettingChangedHandlerEntry<T> : ISettingChangedHandlerEntry
    {
        internal SettingChangedHandlerEntry(SettingChangedEventHandler<T> handler, object tag)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));

            Handler = handler;
            Tag = tag;
        }

        private SettingChangedEventHandler<T> Handler { get; }
        public object Tag { get; set; }

        /// <summary>
        ///     Implemented explicitly to hide it from outside access
        /// </summary>
        void ISettingChangedHandlerEntry.SendEvent(object value)
        {
            Handler(this, new SettingChangedEventArgs<T>((T) value));
        }
    }
}