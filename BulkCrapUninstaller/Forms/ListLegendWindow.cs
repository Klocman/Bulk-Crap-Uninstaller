/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Drawing;
using System.Windows.Forms;
using BulkCrapUninstaller.Controls;

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
        }

        public void UpdatePosition(Control owner)
        {
            var local = new Point(owner.Width - Width - 30, owner.Height - Height - 30);
            var global = owner.PointToScreen(local);
            Location = global;
        }

        public ListLegend ListLegend => listLegend1;
    }
}
