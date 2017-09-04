using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Klocman.Forms;
using UniversalUninstaller.Properties;

namespace UniversalUninstaller
{
    public partial class UninstallSelection : Form
    {
        public UninstallSelection(DirectoryInfo target)
        {
            InitializeComponent();
            targetList1.Populate(target);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            var ex = LoadingDialog.ShowDialog(this, Localisation.UninstallSelection_DeleteProgress_Title, i =>
            {
                Program.DeleteItems(targetList1.GetItemsToDelete().ToList());
            });

            if (ex != null)
            {
                MessageBox.Show(ex.ToString(), Localisation.UninstallSelection_DeleteProgress_FailedTitle, 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(ex);

                DeleteFailed = true;
            }

            Close();
        }

        public bool WasCancelled { get; private set; }
        public bool DeleteFailed { get; private set; }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            WasCancelled = true;
            Close();
        }
    }
}
