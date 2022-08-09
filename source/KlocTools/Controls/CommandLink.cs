/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Klocman.Controls
{
    public sealed class CommandLink : Button
    {
        private const uint BCM_GETNOTE = 0x0000160A;
        private const uint BCM_GETNOTELENGTH = 0x0000160B;
        private const uint BCM_SETNOTE = 0x00001609;
        private const uint BCM_SETSHIELD = 0x0000160C;
        private const int BS_COMMANDLINK = 0x0000000E;
        private bool _shield;

        public CommandLink()
        {
            base.FlatStyle = FlatStyle.System;
        }

        // Prevent FlatStyle from changing
        public new FlatStyle FlatStyle => base.FlatStyle;
        protected override Size DefaultSize => new(180, 60);

        [Category("Command Link"),
         Description("Gets or sets the note text of the command link."),
         DefaultValue("")]
        public string Note
        {
            get { return GetNoteText(); }
            set { SetNoteText(value); }
        }

        [Category("Command Link"),
         Description("Gets or sets the shield icon visibility of the command link."),
         DefaultValue(false)]
        public bool Shield
        {
            get { return _shield; }
            set
            {
                _shield = value;
                SendMessage(new HandleRef(this, Handle), BCM_SETSHIELD, IntPtr.Zero,
                    _shield);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cParams = base.CreateParams;
                cParams.Style |= BS_COMMANDLINK;
                return cParams;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int SendMessage(HandleRef hWnd,
            uint Msg, ref int wParam, StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int SendMessage(HandleRef hWnd,
            uint Msg, IntPtr wParam, string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int SendMessage(HandleRef hWnd,
            uint Msg, IntPtr wParam, bool lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int SendMessage(HandleRef hWnd,
            uint Msg, IntPtr wParam, IntPtr lParam);

        private string GetNoteText()
        {
            var length = SendMessage(new HandleRef(this, Handle),
                BCM_GETNOTELENGTH,
                IntPtr.Zero, IntPtr.Zero) + 1;

            var sb = new StringBuilder(length);

            SendMessage(new HandleRef(this, Handle),
                BCM_GETNOTE,
                ref length, sb);

            return sb.ToString();
        }

        private void SetNoteText(string value)
        {
            SendMessage(new HandleRef(this, Handle),
                BCM_SETNOTE,
                IntPtr.Zero, value);
        }
    }
}