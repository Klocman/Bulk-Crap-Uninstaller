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
            labelStoreApp.BackColor = Constants.WindowsStoreAppColor;
        }
        
        public bool InvalidEnabled
        {
            get { return labelInvalid.Visible; }
            set { labelInvalid.Visible = value; }
        }

        public bool WinFeatureEnabled
        {
            get { return labelWinFeature.Visible; }
            set { labelWinFeature.Visible = value; }
        }

        public bool CertificatesEnabled
        {
            get { return labelVerified.Visible; }
            set { labelVerified.Visible = value; labelUnverified.Visible = value; }
        }

        public bool OrphanedEnabled
        {
            get { return labelOrphaned.Visible; }
            set { labelOrphaned.Visible = value; }
        }

        public bool StoreAppEnabled
        {
            get { return labelStoreApp.Visible; }
            set { labelStoreApp.Visible = value; }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            Visible = false;
        }

        private void ThisEnabledChanged(object sender, EventArgs e)
        {
            BackColor = Enabled ? SystemColors.ControlLightLight : SystemColors.Control;
        }
    }
}