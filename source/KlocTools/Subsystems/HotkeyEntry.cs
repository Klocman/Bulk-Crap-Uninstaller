/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Windows.Forms;
using Klocman.Extensions;

namespace Klocman.Subsystems
{
    /// <summary>
    ///     Represents a single hotkey. It gets disposed when removed from main hotkey collection, so don't use it outside
    /// </summary>
    public sealed class HotkeyEntry : IDisposable
    {
        private readonly Func<bool> _isEnabled;

        public HotkeyEntry(Keys key, Action<object, EventArgs> eventHandlerDelegate, ToolStripMenuItem masterControl)
        {
            Shift = false;
            Ctrl = false;
            Alt = false;
            KeyCode = key;
            EventHandler = eventHandlerDelegate;
            Master = masterControl;
        }

        public HotkeyEntry(Keys key, ToolStripMenuItem masterControl)
            : this(key, (x, y) => masterControl.PerformClick(), masterControl)
        {
        }

        public HotkeyEntry(Keys key, ToolStripMenuItem masterControl, Func<bool> isEnabledDelegate)
            : this(key, masterControl)
        {
            _isEnabled = isEnabledDelegate;
        }

        public HotkeyEntry(Keys key, Action<object, EventArgs> eventHandlerDelegate, ToolStripMenuItem masterControl,
            Func<bool> isEnabledDelegate)
            : this(key, eventHandlerDelegate, masterControl)
        {
            _isEnabled = isEnabledDelegate;
        }

        public HotkeyEntry(Keys key, bool altPressed, bool ctrlPressed, bool shiftPressed,
            ToolStripMenuItem masterControl)
            : this(key, masterControl)
        {
            Alt = altPressed;
            Ctrl = ctrlPressed;
            Shift = shiftPressed;
        }

        public HotkeyEntry(Keys key, bool altPressed, bool ctrlPressed, bool shiftPressed,
            Action<object, EventArgs> eventHandlerDelegate, ToolStripMenuItem masterControl)
            : this(key, eventHandlerDelegate, masterControl)
        {
            Alt = altPressed;
            Ctrl = ctrlPressed;
            Shift = shiftPressed;
        }

        public HotkeyEntry(Keys key, bool altPressed, bool ctrlPressed, bool shiftPressed,
            ToolStripMenuItem masterControl,
            Func<bool> isEnabledDelegate)
            : this(key, altPressed, ctrlPressed, shiftPressed, masterControl)
        {
            _isEnabled = isEnabledDelegate;
        }

        public HotkeyEntry(Keys key, bool altPressed, bool ctrlPressed, bool shiftPressed,
            Action<object, EventArgs> eventHandlerDelegate, ToolStripMenuItem masterControl,
            Func<bool> isEnabledDelegate)
            : this(key, altPressed, ctrlPressed, shiftPressed, eventHandlerDelegate, masterControl)
        {
            _isEnabled = isEnabledDelegate;
        }

        public bool Alt { get; }
        public bool Ctrl { get; }
        public string Description { get; set; }
        //Action<object, EventArgs> _eventHandler;
        public Action<object, EventArgs> EventHandler { get; set; }

        public bool IsEnabled
        {
            get
            {
                if (_isEnabled != null)
                    return _isEnabled();
                return true;
            }
        }

        public Keys KeyCode { get; }
        public ToolStripMenuItem Master { get; private set; }
        public bool Shift { get; }

        public void Dispose()
        {
            if (Master != null)
            {
                Master.ShortcutKeyDisplayString = string.Empty;
                Master = null;
            }
            EventHandler = null;
        }

        public override string ToString()
        {
            return (Ctrl ? "Ctrl+" : string.Empty).AppendIf(Shift, "Shift+").AppendIf(Alt, "Alt+") + KeyCode;
        }
    }
}