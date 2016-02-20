using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Klocman.Extensions;
using UninstallTools.Uninstaller;

namespace BulkCrapUninstaller.Controls
{
    public partial class AdvancedClipboardCopy : UserControl
    {
        public IEnumerable<ApplicationUninstallerEntry> Targets { get; set; }

        public AdvancedClipboardCopy()
        {
            InitializeComponent();

            comboBoxInsert.Items.AddRange(ClipboardCopyItem.Items.Cast<object>().ToArray());
            comboBoxInsert.SelectedIndex = 0;
        }

        public string PatternText
        {
            get { return textBoxPatternInput.Text; }
            set { textBoxPatternInput.Text = value; }
        }

        public string Result => textBoxResults.Text;

        private void RefreshResult(object sender, EventArgs e)
        {
            try
            {
                var pattern = checkBoxUnescape.Checked ? Regex.Unescape(textBoxPatternInput.Text) : textBoxPatternInput.Text;
                textBoxResults.Text = string.Join(Environment.NewLine,
                    Targets.Select(x => ClipboardCopyItem.GetStringFromPattern(pattern, x))
                        .ToArray());
            }
            catch (Exception ex)
            {
                textBoxResults.Text = ex.Message;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxInsert.SelectedIndex == 0) return;
            var x = comboBoxInsert.SelectedItem as ClipboardCopyItem;
            if (x != null)
                textBoxPatternInput.SelectedText = x.Name;
            comboBoxInsert.SelectedIndex = 0;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            RefreshResult(sender, e);
        }
    }
}
