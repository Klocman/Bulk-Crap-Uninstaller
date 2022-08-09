/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Windows.Forms;

namespace BulkCrapUninstaller.Controls
{
    public class TabControlWithoutHeader : TabControl
    {
        public TabControlWithoutHeader()
        {
            if (!DesignMode) Multiline = true;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x1328 && !DesignMode)
                m.Result = new IntPtr(1);
            else
                base.WndProc(ref m);
        }
    }
}