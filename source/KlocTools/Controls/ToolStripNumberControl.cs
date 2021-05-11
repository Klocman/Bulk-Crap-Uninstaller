/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Klocman.Controls
{
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip)]
    public sealed class ToolStripNumberControl : ToolStripControlHost
    {
        #region Constructors

        public ToolStripNumberControl()
            : base(new NumericUpDown())
        {
        }

        #endregion Constructors

        #region Properties

        public NumericUpDown NumericUpDownControl
        {
            get { return Control as NumericUpDown; }
        }

        #endregion Properties
    }
}