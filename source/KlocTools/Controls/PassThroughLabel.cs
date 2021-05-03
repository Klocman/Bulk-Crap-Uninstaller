/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Windows.Forms;

namespace Klocman.Controls
{
    /// <summary>
    ///     Label that doesn't capture any mouse events
    /// </summary>
    public sealed class PassThroughLabel : Label
    {
        #region Methods

        //Pass through mouse events
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x0084;
            const int HTTRANSPARENT = (-1);

            if (m.Msg == WM_NCHITTEST)
            {
                m.Result = (IntPtr) HTTRANSPARENT;
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        #endregion Methods
    }
}