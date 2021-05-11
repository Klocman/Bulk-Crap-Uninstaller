using System;

namespace Klocman.Binding.Settings
{
    /// <summary>
    ///     EventArgs used by the SettingBinder to announce setting changes
    /// </summary>
    /// <typeparam name="T">Type of the setting that was changed</typeparam>
    public class SettingChangedEventArgs<T> : EventArgs
    {
        internal SettingChangedEventArgs(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            NewValue = value;
        }

        /// <summary>
        ///     New value of the changed setting
        /// </summary>
        public T NewValue { get; }
    }
}