/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using BulkCrapUninstaller.Controls;
using Klocman.Extensions;

namespace BulkCrapUninstaller.Forms
{
    public partial class ListLegendWindow : Form
    {
        public ListLegendWindow()
        {
            InitializeComponent();

            listLegend1.CloseRequested += (sender, args) =>
            {
                Visible = false;
                Owner.Focus();
            };
            
            foreach (var control in this.GetAllChildren())
            {
                control.MouseLeave += ControlOnMouseEvent;
                control.MouseEnter += ControlOnMouseEvent;
            }
        }

        public void UpdatePosition(Control owner)
        {
            var local = new Point(owner.Width - Width - 30, owner.Height - Height - 30);
            var global = owner.PointToScreen(local);
            Location = global;
        }

        public ListLegend ListLegend => listLegend1;

        private void ListLegendWindow_VisibleChanged(object sender, System.EventArgs e)
        {
            if(Opacity < .9)
                opacityResetTimer.Enabled = true;
        }

        private void ListLegendWindow_EnabledChanged(object sender, System.EventArgs e)
        {
            if (Opacity < .9)
                opacityResetTimer.Enabled = true;
        }

        private void opacityResetTimer_Tick(object sender, EventArgs e)
        {
            if (!CheckMouseHover())
            {
                opacityResetTimer.Stop();
                Opacity = 1;
            }
        }

        private void ControlOnMouseEvent(object sender, EventArgs eventArgs)
        {
            if (CheckMouseHover())
            {
                Opacity = .3;
                opacityResetTimer.Enabled = true;
            }
            else
            {
                Opacity = 1;
                opacityResetTimer.Enabled = false;
            }
        }

        private bool CheckMouseHover()
        {
            var pt = PointToClient(Cursor.Position);
            return ClientRectangle.Contains(pt);
        }
    }
}
