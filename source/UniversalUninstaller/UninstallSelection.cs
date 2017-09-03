using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Klocman.Forms;

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
            var ex = LoadingDialog.ShowDialog(this, "Deleting items", i =>
            {
                Program.DeleteItems(targetList1.GetItemsToDelete().ToList());
            });

            MessageBox.Show(ex.ToString(), "Failed to remove some items", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
