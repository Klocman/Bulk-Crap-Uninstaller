/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Drawing;
using System.Windows.Forms;

namespace Klocman.Controls
{
    public sealed partial class LineSeparator : UserControl
    {
        #region Constructors

        public LineSeparator()
        {
            InitializeComponent();

            Paint += LineSeparator_Paint;

            MaximumSize = new Size(2000, 2);

            MinimumSize = new Size(0, 2);

            Width = 350;
        }

        #endregion Constructors

        #region Methods

        private void LineSeparator_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            g.DrawLine(Pens.DarkGray, new Point(0, 0), new Point(Width, 0));

            g.DrawLine(Pens.White, new Point(0, 1), new Point(Width, 1));
        }

        #endregion Methods
    }
}