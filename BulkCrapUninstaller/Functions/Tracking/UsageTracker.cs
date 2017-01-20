/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Klocman.Forms.Tools;

namespace BulkCrapUninstaller.Functions.Tracking
{
    public sealed class UsageTracker : ReferencedComponent
    {
        private static readonly IEnumerable<string> EventFilters = new[] {"Click", "DoubleClick"};
        private static readonly List<Type> TypeBlacklist = new List<Type>();
        // new[] { typeof(Forms.CustomMessageBox) });

        private readonly List<EventHook> _hooks = new List<EventHook>();
        internal IEnumerable<EventHook> Hooks => _hooks;

        public static void AddBlacklistType(Type t)
        {
            TypeBlacklist.Add(t);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UsageManager.UsageTrackerDestructionCallback(this);
            }
            base.Dispose(disposing);
        }

        private void HookToForm()
        {
            var type = ContainerControl.GetType();

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                var fieldType = field.FieldType;

                if (typeof (Form).IsAssignableFrom(fieldType) || TypeBlacklist.Contains(fieldType))
                    continue;

                var targetEvents = fieldType.GetEvents(BindingFlags.Instance | BindingFlags.Public)
                    .Where(x => EventFilters.Contains(x.Name)).ToList();
                if (targetEvents.Count > 0)
                    _hooks.Add(new EventHook(ContainerControl, field, targetEvents));
            }
        }

        protected override void OnContainerInitialized(ContainerControl containerControl, EventArgs args)
        {
            HookToForm();
            UsageManager.Trackers.Add(this);
        }
    }
}