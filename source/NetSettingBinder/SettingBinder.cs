using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq.Expressions;
using System.Reflection;

namespace Klocman.Binding.Settings
{
    /// <summary>
    ///     Binding helper used for binding controls, variables and typed event handlers to custom Settings classes.
    ///     It is realized using generics and does not require any boxing or string literals.
    /// </summary>
    /// <typeparam name="TSettingClass">Type of your custom Settings class, it must inherit from ApplicationSettingsBase</typeparam>
    public partial class SettingBinder<TSettingClass> where TSettingClass : ApplicationSettingsBase
    {
        private readonly LockedList<KeyValuePair<string, ISettingChangedHandlerEntry>> _eventEntries;

        /// <summary>
        ///     Create a new SettingBinder and hook into the specified class
        /// </summary>
        /// <param name="settingSet">Custom Settings class this binder is hooked into</param>
        public SettingBinder(TSettingClass settingSet)
        {
            _eventEntries = new LockedList<KeyValuePair<string, ISettingChangedHandlerEntry>>();
            Settings = settingSet ?? throw new ArgumentNullException(nameof(settingSet));
            Settings.PropertyChanged += PropertyChangedCallback;
        }

        /// <summary>
        ///     Custom Settings class this manager is hooked into
        /// </summary>
        public TSettingClass Settings { get; }

        /// <summary>
        ///     Bind specified property inside of targetClass to the specified setting.
        ///     Use event handler specified by eventHandlerName to get notifications of property changes.
        /// </summary>
        /// <typeparam name="TProperty">Type of the property</typeparam>
        /// <typeparam name="TPropertyClass">Type of the class containing specified property</typeparam>
        /// <param name="targetProperty">Lambda of style 'x => x.Property' or 'x => class.Property'</param>
        /// <param name="eventHandlerName">Name of the event handler</param>
        /// <param name="targetClass">Instance of the target class</param>
        /// <param name="selectedSetting">Lambda of style 'x => x.Property' or 'x => class.Property'</param>
        /// <param name="tag">Tag used for grouping</param>
        public void BindProperty<TProperty, TPropertyClass>(TPropertyClass targetClass,
            Expression<Func<TPropertyClass, TProperty>> targetProperty,
            string eventHandlerName, Expression<Func<TSettingClass, TProperty>> selectedSetting, object tag)
            where TPropertyClass : class
        {
            var propertyInfo = ReflectionTools.GetPropertyInfo(targetProperty);
            var eventInfo = typeof(TPropertyClass).GetEvent(eventHandlerName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            if (eventInfo == null)
                throw new ArgumentException(@"Event name is invalid or the event is not public",
                    nameof(eventHandlerName));

            Bind(x => propertyInfo.SetValue(targetClass, x, null),
                () => (TProperty)propertyInfo.GetValue(targetClass, null),
                handler => eventInfo.AddEventHandler(targetClass, handler),
                handler => eventInfo.RemoveEventHandler(targetClass, handler), selectedSetting, tag);
        }

        /// <summary>
        ///     Bind specified property inside of targetClass to the specified setting.
        ///     Use INotifyPropertyChanged.PropertyChanged event to get notifications of property changes.
        /// </summary>
        /// <typeparam name="TProperty">Type of the property</typeparam>
        /// <typeparam name="TPropertyClass">Type of the class containing specified property</typeparam>
        /// <param name="targetProperty">Lambda of style 'x => x.Property' or 'x => class.Property'</param>
        /// <param name="targetClass">Instance of the target class implementing INotifyPropertyChanged</param>
        /// <param name="selectedSetting">Lambda of style 'x => x.Property' or 'x => class.Property'</param>
        /// <param name="tag">Tag used for grouping</param>
        public void BindProperty<TProperty, TPropertyClass>(TPropertyClass targetClass,
            Expression<Func<TPropertyClass, TProperty>> targetProperty,
            Expression<Func<TSettingClass, TProperty>> selectedSetting, object tag)
            where TPropertyClass : class, INotifyPropertyChanged
        {
            var propertyInfo = ReflectionTools.GetPropertyInfo(targetProperty);
            var propertyName = propertyInfo.Name;
            var eventInterface = (INotifyPropertyChanged)targetClass;

            EventHandler handlerCallback = null;

            void PropertyChangedHandler(object sender, PropertyChangedEventArgs args)
            {
                if (propertyName.Equals(args.PropertyName))
                    handlerCallback?.Invoke(sender, args);
            }

            Bind(x => propertyInfo.SetValue(targetClass, x, null),
                () => (TProperty)propertyInfo.GetValue(targetClass, null),
                handler =>
                {
                    handlerCallback = handler;
                    eventInterface.PropertyChanged += PropertyChangedHandler;
                },
                handler =>
                {
                    eventInterface.PropertyChanged -= PropertyChangedHandler;
                },
                selectedSetting,
                tag);
        }

        /// <summary>
        ///     Manually bind to a setting
        /// </summary>
        /// <typeparam name="T">Bound value type</typeparam>
        /// <param name="setter">Delegate used to set value of the external property</param>
        /// <param name="getter">Delegate used to get value of the external property</param>
        /// <param name="registerEvent">Delegate used to register to the notifying event of the external property</param>
        /// <param name="unregisterEvent">Delegate used to unregister from the notifying event of the external property</param>
        /// <param name="targetSetting">Lambda of style 'x => x.Property' or 'x => class.Property'</param>
        /// <param name="tag">Tag used for grouping</param>
        public void Bind<T>(Action<T> setter, Func<T> getter,
            Action<EventHandler> registerEvent, Action<EventHandler> unregisterEvent,
            Expression<Func<TSettingClass, T>> targetSetting, object tag)
        {
            var property = ReflectionTools.GetPropertyInfo(targetSetting);

            void CheckedChanged(object x, EventArgs y)
            {
                property.SetValue(Settings, getter(), null);
            }

            registerEvent(CheckedChanged);

            void SettingChanged(object x, SettingChangedEventArgs<T> y)
            {
                var remoteValue = getter();
                if ((remoteValue != null && !remoteValue.Equals(y.NewValue)) || (remoteValue == null && y.NewValue != null))
                {
                    unregisterEvent(CheckedChanged);
                    setter(y.NewValue);
                    registerEvent(CheckedChanged);
                }
            }

            Subscribe(SettingChanged, targetSetting, tag);
        }

        /// <summary>
        ///     Create event handler that will automatically update target property in target class when the specified setting
        ///     changes.
        /// </summary>
        /// <typeparam name="TProperty">Type of the property</typeparam>
        /// <typeparam name="TPropertyClass">Type of the class containing specified property</typeparam>
        /// <param name="targetProperty">
        ///     The property to be updated when setting changes. Lambda of style 'x => x.Property' or 'x
        ///     => class.Property'
        /// </param>
        /// <param name="targetClass">Instance of the class with the property to be updated</param>
        /// <param name="selectedSetting">Lambda of style 'x => x.Property' or 'x => class.Property'</param>
        /// <param name="tag">Tag used for grouping</param>
        public void Subscribe<TProperty, TPropertyClass>(TPropertyClass targetClass,
            Expression<Func<TPropertyClass, TProperty>> targetProperty,
            Expression<Func<TSettingClass, TProperty>> selectedSetting, object tag)
        {
            var property = ReflectionTools.GetPropertyInfo(targetProperty);

            Subscribe((sender, args) => property.SetValue(targetClass, args.NewValue, null), selectedSetting, tag);
        }

        /// <summary>
        ///     Register event handler for the chosen property and tag it.
        /// </summary>
        /// <typeparam name="TProperty">Type of the property</typeparam>
        /// <param name="handler">Handler to register</param>
        /// <param name="selectedSetting">Lambda of style 'x => x.Property' or 'x => class.Property'</param>
        /// <param name="tag">Tag used for grouping</param>
        public void Subscribe<TProperty>(SettingChangedEventHandler<TProperty> handler,
            Expression<Func<TSettingClass, TProperty>> selectedSetting, object tag)
        {
            var name = ReflectionTools.GetPropertyName(selectedSetting);
            _eventEntries.Add(new KeyValuePair<string, ISettingChangedHandlerEntry>(name,
                new SettingChangedHandlerEntry<TProperty>(handler, tag)));
        }

        /// <summary>
        ///     Remove all handlers with the specified tag
        /// </summary>
        /// <param name="groupTag">Tag used by group to remove</param>
        public void RemoveHandlers(object groupTag)
        {
            _eventEntries.RemoveAll(pair => Equals(pair.Value.Tag, groupTag));
        }

        /// <summary>
        ///     Send property changed events to all registered handlers
        ///     Warning: Do not rely on this firing xyzChanged events as controls and such 
        ///     might not fire them if the value doesn't actually change.
        ///     change.
        /// </summary>
        public void SendUpdates()
        {
            foreach (var entry in _eventEntries)
            {
                entry.Value.SendEvent(Settings[entry.Key]);
            }
        }

        /// <summary>
        ///     Send property changed events to the whole group.
        ///     Warning: Do not rely on this firing xyzChanged events as controls and such 
        ///     might not fire them if the value doesn't actually change.
        /// </summary>
        /// <param name="groupTag">Tag used by group to update</param>
        public void SendUpdates(object groupTag)
        {
            foreach (var entry in _eventEntries)
            {
                if (Equals(entry.Value.Tag, groupTag))
                {
                    entry.Value.SendEvent(Settings[entry.Key]);
                }
            }
        }

        /// <summary>
        ///     Event handler registered to the custom Settings class
        /// </summary>
        private void PropertyChangedCallback(object sender, PropertyChangedEventArgs e)
        {
            foreach (var entry in _eventEntries)
            {
                if (entry.Key.Equals(e.PropertyName))
                {
                    entry.Value.SendEvent(Settings[e.PropertyName]);
                }
            }
        }
    }
}