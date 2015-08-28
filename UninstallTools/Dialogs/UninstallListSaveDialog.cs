using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Klocman.Forms.Tools;
using UninstallTools.Lists;

namespace UninstallTools.Dialogs
{
    public partial class UninstallListSaveDialog : Form
    {
        private UninstallListSaveDialog(IEnumerable<string> initialItems, IEnumerable<string> testItems)
        {
            InitializeComponent();

            var tempList = new UninstallList();
            tempList.AddItems(initialItems);
            uninstallListEditor1.CurrentList = tempList;

            uninstallListEditor1.TestItems = testItems;
        }

        public static bool Show(IEnumerable<string> initialItems, IEnumerable<string> testItems)
        {
            using (var dialog = new UninstallListSaveDialog(initialItems, testItems))
                return dialog.ShowDialog() == DialogResult.OK;
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
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}