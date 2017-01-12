using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using BulkCrapUninstaller.Functions;
using BulkCrapUninstaller.Properties;
using Klocman.Extensions;
using Klocman.Forms.Tools;
using Klocman.Localising;
using Klocman.Resources;
using Klocman.Tools;
using UninstallTools.Junk;

namespace BulkCrapUninstaller.Forms
{
    public partial class JunkRemoveWindow : Form
    {
        private static readonly string SelectionBoxText = Localisable.JunkRemove_SelectionBoxText;

        private readonly string _defaultRegBackupFilename;

        private bool _confirmLowConfidenceMessageShown;
        private TypedObjectListView<JunkNode> _listViewWrapper;

        public JunkRemoveWindow(IEnumerable<JunkNode> junk)
        {
            InitializeComponent();

            Icon = Resources.Icon_Logo;

            var junkNodes = junk as IList<JunkNode> ?? junk.ToList();

            SetupListView(junkNodes);

            if (junkNodes.All(x => x.Confidence.GetRawConfidence() < 0))
            {
                _confirmLowConfidenceMessageShown = true;
                checkBoxHideLowConfidence.Checked = true;
                checkBoxHideLowConfidence.Enabled = false;
            }
            else if(junkNodes.All(x=> x.Confidence.GetRawConfidence() >= 0))
                checkBoxHideLowConfidence.Enabled = false;

            new[] {Confidence.VeryGood, Confidence.Good, Confidence.Questionable, Confidence.Bad}
                .ForEach(x => comboBoxChecker.Items.Add(new LocalisedEnumWrapper(x)));
            comboBoxChecker_DropDownClosed(this, EventArgs.Empty);

            _defaultRegBackupFilename = saveFileDialogBackupRegistry.FileName;
        }

        public IEnumerable<JunkNode> SelectedJunk => _listViewWrapper.CheckedObjects;

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            if (SelectedJunk.Any(x => x is RegistryJunkNode))
            {
                switch (MessageBoxes.BackupRegistryQuestion())
                {
                    case MessageBoxes.PressedButton.Yes:
                        var enus = new CultureInfo("en-US", false).DateTimeFormat;
                        saveFileDialogBackupRegistry.FileName = _defaultRegBackupFilename + "_" +
                                                                DateTime.Now.ToString(
                                                                    enus.SortableDateTimePattern.Replace(':', '-')
                                                                        .Replace('T', '_'));
                        if (saveFileDialogBackupRegistry.ShowDialog() != DialogResult.OK)
                            return;
                        break;

                    case MessageBoxes.PressedButton.No:
                        break;

                    default:
                        return;
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            exportDialog.ShowDialog();
        }

        private void checkBoxHideLowConfidence_CheckedChanged(object sender, EventArgs e)
        {
            objectListViewMain.UpdateColumnFiltering();
        }

        private void checkBoxHideLowConfidence_Click(object sender, EventArgs e)
        {
            if (!checkBoxHideLowConfidence.Checked)
            {
                if (_confirmLowConfidenceMessageShown || MessageBoxes.ConfirmLowConfidenceQuestion())
                    _confirmLowConfidenceMessageShown = true;
                else
                    checkBoxHideLowConfidence.Checked = true;
            }
        }

        private void comboBoxChecker_DropDown(object sender, EventArgs e)
        {
            comboBoxChecker.Items.Remove(SelectionBoxText);
            //comboBoxChecker.ForeColor = SystemColors.WindowText;
        }

        private void comboBoxChecker_DropDownClosed(object sender, EventArgs e)
        {
            var localisedEnumWrapper = comboBoxChecker.SelectedItem as LocalisedEnumWrapper;
            if (localisedEnumWrapper != null)
            {
                var selectedConfidence = (Confidence) localisedEnumWrapper.TargetEnum;

                if ((selectedConfidence != Confidence.Bad && selectedConfidence != Confidence.Questionable)
                    || MessageBoxes.ConfirmLowConfidenceQuestion()) //Ask if selected low confidence
                {
                    SelectUpTo(selectedConfidence);
                }
            }

            comboBoxChecker.Items.Add(SelectionBoxText);
            comboBoxChecker.SelectedItem = SelectionBoxText;
            //comboBoxChecker.ForeColor = SystemColors.Control;
        }

        private void comboBoxChecker_SelectedIndexChanged(object sender, EventArgs e)
        {
            objectListViewMain.BuildList(true);
        }

        private void exportDialog_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                File.WriteAllLines(exportDialog.FileName,
                    objectListViewMain.FilteredObjects.Cast<JunkNode>().Select(x => x.ToLongString()).ToArray());
            }
            catch (Exception ex)
            {
                MessageBoxes.ExportFailed(ex.Message);
            }
        }

        private bool JunkListFilter(object obj)
        {
            var item = obj as JunkNode;
            if (item == null)
                return false;

            if (checkBoxHideLowConfidence.Checked && item.Confidence.GetRawConfidence() < 0)
                return false;

            return true;
        }

        private void JunkRemoveWindow_Shown(object sender, EventArgs e)
        {
            SelectUpTo(Confidence.Good);
        }

        private void objectListViewMain_CellEditStarting(object sender, CellEditEventArgs e)
        {
            e.Cancel = true;
            var item = e.RowObject as JunkNode;
            if (item == null) return;
            OpenJunkNodePreview(item);
        }

        private static void OpenJunkNodePreview(JunkNode item)
        {
            try
            {
                item.Open();
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
            }
        }

        private static void DisplayDetails(JunkNode item)
        {
            var groups = item.Confidence.ConfidenceParts.GroupBy(part => part.Change > 0).ToList();

            var positives = Localisable.Empty;
            if (groups.Any(x => x.Key))
            {
                var items = groups.First(x => x.Key)
                    .Where(x => x.Reason.IsNotEmpty())
                    .Select(x => x.Reason)
                    .ToArray();
                if (items.Any())
                    positives = string.Join("\n", items);
            }

            var negatives = Localisable.Empty;
            if (groups.Any(x => !x.Key))
            {
                var items = groups.First(x => !x.Key)
                    .Where(x => x.Reason.IsNotEmpty())
                    .Select(x => x.Reason)
                    .ToArray();
                if (items.Any())
                    negatives = string.Join("\n", items);
            }

            MessageBox.Show(string.Format(Localisable.JunkRemove_Details_Message,
                item.Confidence.GetRawConfidence(), positives, negatives), Localisable.JunkRemove_Details_Title,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void objectListViewMain_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            buttonAccept.Enabled = SelectedJunk.Any();
        }

        private void saveFileDialogBackupRegistry_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                var regPaths = SelectedJunk.OfType<RegistryJunkNode>()
                    // If the thingy is a value, export the entire key
                    .Select(x => x is RegistryValueJunkNode ? x.ParentPath : x.FullName)
                    .Distinct();

                RegistryTools.ExportRegistry(saveFileDialogBackupRegistry.FileName, regPaths);
            }
            catch (Exception ex)
            {
                if (MessageBoxes.BackupFailedQuestion(ex.Message) == MessageBoxes.PressedButton.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void SelectUpTo(Confidence selectedConfidence)
        {
            objectListViewMain.DeselectAll();
            objectListViewMain.UncheckAll();

            objectListViewMain.CheckObjects(objectListViewMain.FilteredObjects.Cast<JunkNode>()
                .Where(x => x.Confidence.GetConfidence() >= selectedConfidence).ToList());
        }

        private void SetupListView(IEnumerable<JunkNode> junk)
        {
            _listViewWrapper = new TypedObjectListView<JunkNode>(objectListViewMain);

            olvColumnSafety.AspectGetter = x => (x as JunkNode)?.Confidence.GetConfidence().GetLocalisedName();
            olvColumnPath.GroupKeyGetter = x => (x as JunkNode)?.GroupName ?? CommonStrings.Unknown;

            objectListViewMain.UseFiltering = true;
            objectListViewMain.AdditionalFilter = new ModelFilter(JunkListFilter);

            objectListViewMain.SetObjects(junk);
            objectListViewMain.Sort(olvColumnUninstallerName, SortOrder.Ascending);
        }

        private void objectListViewMain_CellRightClick(object sender, CellRightClickEventArgs e)
        {
            if (e.Model == null)
                return;

            if (objectListViewMain.CheckBoxes && !objectListViewMain.IsChecked(e.Model))
            {
                objectListViewMain.UncheckAll();
                objectListViewMain.CheckObject(e.Model);
            }

            e.MenuStrip = listViewContextMenuStrip;
        }

        private void detailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = objectListViewMain.SelectedObject as JunkNode;
            if (item == null) return;
            DisplayDetails(item);
        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var items = objectListViewMain.SelectedObjects.Cast<JunkNode>().Select(x => x.ToLongString()).ToArray();

                if (items.Any())
                {
                    Clipboard.SetText(string.Join(Environment.NewLine, items));
                }
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = objectListViewMain.SelectedObject as JunkNode;
            if (item == null) return;
            OpenJunkNodePreview(item);
        }
    }
}