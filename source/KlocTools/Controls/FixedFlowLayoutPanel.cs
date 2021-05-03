/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Linq;
using System.Windows.Forms;

namespace Klocman.Controls
{
    public sealed class FixedFlowLayoutPanel : FlowLayoutPanel
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (AutoSize && Dock != DockStyle.None)
            {
                var newHeight = Controls.Cast<Control>().Sum(c => c.Height) + Padding.Top + Padding.Bottom;
                if (Height != newHeight)
                {
                    Height = newHeight;
                }
            }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            
            if (AutoSize && Dock != DockStyle.None)
            {
                var newHeight = Controls.Cast<Control>().Sum(c => c.Height) + Padding.Top + Padding.Bottom;
                if (Height != newHeight)
                {
                    Height = newHeight;
                }
            }
        }

        private bool _autoSize;

        public override bool AutoSize
        {
            get { return _autoSize; }
            set { _autoSize = value; }
        }
    }
}
