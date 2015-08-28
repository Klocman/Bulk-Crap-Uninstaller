// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Feedback.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Forms;
using NBug.Properties;

namespace NBug.Core.UI.WinForms
{
    internal partial class Feedback : Form
    {
        public Feedback()
        {
            InitializeComponent();
            Icon = Resources.NBug_icon_16;
        }
    }
}