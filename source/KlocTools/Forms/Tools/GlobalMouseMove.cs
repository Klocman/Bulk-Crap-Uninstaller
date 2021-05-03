/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Windows.Forms;

namespace Klocman.Forms.Tools
{
    public class GlobalMouseMove : IMessageFilter
    {
        private const int WM_MOUSEMOVE = 0x0200;

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_MOUSEMOVE)
            {
                MouseMove?.Invoke(this, EventArgs.Empty);
            }
            // Always allow message to continue to the next filter control
            return false;
        }

        public event MouseMovedEvent MouseMove;

        public void RegisterHandler()
        {
            Application.AddMessageFilter(this);
        }

        public void UnregisterEvent()
        {
            Application.RemoveMessageFilter(this);
        }
    }
}