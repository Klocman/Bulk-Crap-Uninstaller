using System;
using System.ComponentModel;
using System.Windows.Forms;
using Klocman.Forms.Tools;
using UninstallTools.Lists;

namespace UninstallTools.Dialogs
{
    public partial class UninstallListOpenDialog : Form
    {
        private UninstallListOpenDialog()//IEnumerable<string> previewItems)
        {
            InitializeComponent();

            UninstallList tempList;
            while (true)
            {
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    DialogResult = DialogResult.Abort;
                    return;
                }

                try
                {
                    tempList = UninstallList.ReadFromFile(openFileDialog.FileName);
                    break;
                }
                catch (Exception ex)
                {
                    PremadeDialogs.GenericError(ex);
                }
            }

            uninstallListEditor1.CurrentList = tempList;
            //uninstallListEditor1.TestItems = previewItems;
        }

        public UninstallList Result => uninstallListEditor1.CurrentList;

        public static UninstallList ShowOpenDialog()//IEnumerable<string> previewItems)
        {
            using (var dialog = new UninstallListOpenDialog())
            {
                if ((dialog.DialogResult != DialogResult.Abort) && (dialog.ShowDialog() == DialogResult.OK))
                {
                    return dialog.Result;
                }
            }
            return null;
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            saveFileDialog.ShowDialog();
        }

        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                uninstallListEditor1.CurrentList.SaveToFile(saveFileDialog.FileName);
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
                e.Cancel = true;
            }
        }
    }
}