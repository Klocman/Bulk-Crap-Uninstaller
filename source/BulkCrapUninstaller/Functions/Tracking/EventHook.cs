/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Reflection;

namespace BulkCrapUninstaller.Functions.Tracking
{
    internal sealed class EventHook : IDisposable
    {
        private readonly List<SingleEventHook> _hooks = new();

        public EventHook(object target, FieldInfo targetField, IEnumerable<EventInfo> targetEvents)
        {
            Parent = target;
            Field = targetField;

            ParentName = Parent.GetType().Name;
            FieldName = Field.Name;

            SubscribeToEvents(targetEvents);
        }

        public FieldInfo Field { get; }
        public string FieldName { get; }
        public IEnumerable<SingleEventHook> Hooks => _hooks;
        public object Parent { get; }
        public string ParentName { get; }

        public void Dispose()
        {
            foreach (var hook in _hooks)
            {
                hook.Dispose();
            }
        }

        private void SubscribeToEvents(IEnumerable<EventInfo> targetEvents)
        {
            foreach (var targetEvent in targetEvents)
            {
                try
                {
                    _hooks.Add(new SingleEventHook(this, targetEvent));
                }
                catch (ArgumentException)
                {
                }
            }
        }
    }
}