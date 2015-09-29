using System;
using System.Drawing;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions;
using Klocman.Forms.Tools;

namespace BulkCrapUninstaller.Controls
{
    [WindowStyleController.ControlStyleAttribute(false)]
    public partial class ListLegend : UserControl
    {
        public ListLegend()
        {
            InitializeComponent();

            labelInvalid.BackColor = Constants.InvalidColor;
            labelOrphaned.BackColor = Constants.UnregisteredColor;
            labelUnverified.BackColor = Constants.UnverifiedColor;
            labelVerified.BackColor = Constants.VerifiedColor;
            labelWinFeature.BackColor = Constants.WindowsFeatureColor;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            Visible = false;
        }

        private void tableLayoutPanel1_EnabledChanged(object sender, EventArgs e)
        {
            BackColor = Enabled ? SystemColors.ControlLightLight : SystemColors.Control;
        }
    }
}