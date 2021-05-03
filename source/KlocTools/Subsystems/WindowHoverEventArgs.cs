/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;

namespace Klocman.Subsystems
{
    public sealed class WindowHoverEventArgs : EventArgs
    {
        public WindowHoverEventArgs(WindowHoverSearcher.WindowInfo targetWindow)
        {
            TargetWindow = targetWindow;
        }

        public WindowHoverSearcher.WindowInfo TargetWindow { get; }
    }
}