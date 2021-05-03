/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Drawing;
using System.Windows.Forms;

namespace Klocman.Forms.Tools
{
    //Add this to the bottom of your form. (After your class ends)
    public class StandardSystemColorTable : ProfessionalColorTable
    {
        public override Color ToolStripGradientBegin => SystemColors.Control;
        public override Color ToolStripGradientMiddle => SystemColors.Control;
        public override Color ToolStripGradientEnd => SystemColors.Control;
        public override Color MenuStripGradientBegin => SystemColors.MenuBar;
        public override Color MenuStripGradientEnd => SystemColors.MenuBar;
        public override Color StatusStripGradientBegin => SystemColors.MenuBar;
        public override Color StatusStripGradientEnd => SystemColors.MenuBar;
    }

    //Add this to the bottom of your form. (After your class ends)
}