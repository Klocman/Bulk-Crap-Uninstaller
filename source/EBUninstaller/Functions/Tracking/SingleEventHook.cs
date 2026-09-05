/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.Reflection;

namespace BulkCrapUninstaller.Functions.Tracking
{
    internal sealed class SingleEventHook : IDisposable
    {
        private static readonly MethodInfo SimpleHandlerInfo = typeof (SingleEventHook).GetMethod("SimpleHandler",
            BindingFlags.Instance | BindingFlags.NonPublic);

        public SingleEventHook(EventHook parent, EventInfo targetEvent)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (targetEvent == null) throw new ArgumentNullException(nameof(targetEvent));

            EventName = targetEvent.Name;

            Parent = parent;
            TargetProperty = parent.Field.GetValue(parent.Parent);

            if (TargetProperty == null)
                throw new ArgumentException("EventHook has an invalid Field or Parent property.");

            TargetEvent = targetEvent;

            Handler = Delegate.CreateDelegate(targetEvent.EventHandlerType ?? throw new ArgumentException("EventHandlerType is null"), this, SimpleHandlerInfo);

            TargetEvent.AddEventHandler(TargetProperty, Handler);
        }

        public string EventName { get; }
        public Delegate Handler { get; }
        public int HitCount { get; private set; }
        public string Name => Parent.ParentName + " -> " + Parent.FieldName + " -> " + EventName;
        public EventHook Parent { get; }
        public EventInfo TargetEvent { get; }
        public object TargetProperty { get; }

        public void Dispose()
        {
            TargetEvent.RemoveEventHandler(TargetProperty, Handler);
        }

        public override string ToString()
        {
            return Name;
        }

        // Accessed via reflection by SimpleHandlerInfo
        private void SimpleHandler(object sender, EventArgs args)
        {
            HitCount++;
            Debug.WriteLine(Name + " | Times hit: " + HitCount);
        }
    }
}