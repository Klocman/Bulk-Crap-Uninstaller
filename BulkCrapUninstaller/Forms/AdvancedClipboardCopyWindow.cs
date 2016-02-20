using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BulkCrapUninstaller.Controls;
using UninstallTools.Uninstaller;

namespace BulkCrapUninstaller.Forms
{
    public partial class AdvancedClipboardCopyWindow : Form
    {
        public static void ShowDialog(IWin32Window parent, IEnumerable<ApplicationUninstallerEntry> targets)
        {
            using (var window = new AdvancedClipboardCopyWindow())
            {
                window.advancedClipboardCopy1.Targets = targets;
                window.ShowDialog(parent);
            }
        }

        private AdvancedClipboardCopyWindow()
        {
            InitializeComponent();
        }

        private void AdvancedClipboardCopyWindow_Shown(object sender, EventArgs e)
        {
            advancedClipboardCopy1.PatternText = "{" + nameof(ApplicationUninstallerEntry.DisplayName) + "} - {" + nameof(ApplicationUninstallerEntry.UninstallString) + "}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(advancedClipboardCopy1.Result);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
