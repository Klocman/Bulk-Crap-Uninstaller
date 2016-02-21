using System;
using System.Drawing;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions.Ratings;

namespace BulkCrapUninstaller.Forms
{
    public partial class RatingPopup : Form
    {
        private UninstallerRating _result = UninstallerRating.Unknown;

        private RatingPopup()
        {
            InitializeComponent();
        }

        public static UninstallerRating ShowRateDialog(Form owner, string applicationName)
        {
            return ShowRateDialog(owner, applicationName, Point.Empty);
        }

        public static UninstallerRating ShowRateDialog(Form owner, string applicationName, Point mouseLocation)
        {
            using (var window = new RatingPopup())
            {
                window.Text += " " + applicationName;
                window.Icon = owner.Icon;

                if (mouseLocation.IsEmpty)
                    window.StartPosition = FormStartPosition.CenterParent;
                else
                    window.Location = new Point(mouseLocation.X - window.Size.Width/2,
                        mouseLocation.Y - window.Size.Height/2);

                window.ShowDialog(owner);

                return window._result;
            }
        }

        private void buttonGood_Click(object sender, EventArgs e)
        {
            _result = UninstallerRating.Good;
            Close();
        }

        private void buttonNormal_Click(object sender, EventArgs e)
        {
            _result = UninstallerRating.Neutral;
            Close();
        }

        private void buttonBad_Click(object sender, EventArgs e)
        {
            _result = UninstallerRating.Bad;
            Close();
        }
    }
}