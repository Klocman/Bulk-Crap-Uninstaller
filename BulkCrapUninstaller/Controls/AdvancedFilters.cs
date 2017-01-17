using BulkCrapUninstaller.Functions;
using BulkCrapUninstaller.Properties;
using Klocman.Forms.Tools;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using UninstallTools.Lists;

namespace BulkCrapUninstaller.Controls
{
    public partial class AdvancedFilters : UserControl
    {
        public event EventHandler CurrentListChanged;
        public event EventHandler CurrentListFilenameChanged;
        public event EventHandler FiltersChanged;
        public event EventHandler UnsavedChangesChanged;

        private static readonly string DefaultUninstallListPath = Path.Combine(Program.AssemblyLocation.FullName, Resources.DefaultUninstallListFilename);
        private string _currentListFilename;
        private bool _unsavedChanges;

        public UninstallList CurrentList => uninstallListEditor1.CurrentList;

        public bool UnsavedChanges
        {
            get { return _unsavedChanges; }
            private set
            {
                if(_unsavedChanges != value)
                {
                    _unsavedChanges = value;
                    UnsavedChangesChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public string CurrentListFilename
        {
            get { return _currentListFilename; }
            private set
            {
                if (_currentListFilename != value)
                {
                    _currentListFilename = value;
                    CurrentListFilenameChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public AdvancedFilters()
        {
            InitializeComponent();

            uninstallListEditor1.CurrentListChanged += OnCurrentListChanged;
            uninstallListEditor1.FiltersChanged += OnFiltersChanged;
        }

        private bool AskToSaveUnsaved()
        {
            if (!UnsavedChanges || uninstallListEditor1.CurrentList == null)
                return true;

            switch (MessageBoxes.AskToSaveUninstallList())
            {
                case MessageBoxes.PressedButton.Cancel:
                    return false;
                case MessageBoxes.PressedButton.Yes:
                    return ShowSaveDialog();
                case MessageBoxes.PressedButton.No:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void LoadUninstallList(UninstallList list)
        {
            CurrentListFilename = string.Empty;
            uninstallListEditor1.CurrentList = list;
        }

        /// <summary>
        /// Load a list silently from filename
        /// </summary>
        /// <param name="filename">Filename of the list</param>
        public void LoadUninstallList(string filename)
        {
            try
            {
                var result = UninstallList.ReadFromFile(filename);

                CurrentListFilename = filename;
                uninstallListEditor1.CurrentList = result;
                UnsavedChanges = false;
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
            }
        }

        /// <summary>
        /// Show file select gui
        /// </summary>
        public void LoadUninstallList()
        {
            toolStripButtonOpenUl_Click(this, EventArgs.Empty);
        }

        private void OnCurrentListChanged(object sender, EventArgs e)
        {
            if (CurrentList == null)
                CurrentListFilename = string.Empty;

            UnsavedChanges = false;
            CurrentListChanged?.Invoke(sender, e);
        }

        private void OnFiltersChanged(object sender, EventArgs e)
        {
            UnsavedChanges = true;
            FiltersChanged?.Invoke(sender, e);
        }

        private void openUlDialog_FileOk(object sender, CancelEventArgs e)
        {
            LoadUninstallList(openUlDialog.FileName);
        }

        private void saveUlDialog_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                CurrentList.SaveToFile(saveUlDialog.FileName);
                CurrentListFilename = saveUlDialog.FileName;
                UnsavedChanges = false;
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
            }
        }

        private void ShowSaveDialog(object sender, EventArgs e)
        {
            ShowSaveDialog();
        }

        private bool ShowSaveDialog()
        {
            if (!string.IsNullOrEmpty(CurrentListFilename))
            {
                try
                {
                    saveUlDialog.InitialDirectory = Path.GetDirectoryName(CurrentListFilename);
                    saveUlDialog.FileName = Path.GetFileName(CurrentListFilename);
                }
                catch (ArgumentException)
                {
                }
                catch (PathTooLongException)
                {
                }
            }
            return saveUlDialog.ShowDialog(this) == DialogResult.OK;
        }

        private void toolStripButtonOpenUl_Click(object sender, EventArgs e)
        {
            if (!AskToSaveUnsaved()) return;

            if (!string.IsNullOrEmpty(CurrentListFilename))
            {
                try
                {
                    openUlDialog.InitialDirectory = Path.GetDirectoryName(CurrentListFilename);
                    openUlDialog.FileName = Path.GetFileName(CurrentListFilename);
                }
                catch (ArgumentException)
                {
                }
                catch (PathTooLongException)
                {
                }
            }
            openUlDialog.ShowDialog(this);
        }

        private void toolStripButtonSaveUlDef_Click(object sender, EventArgs e)
        {
            CurrentList.SaveToFile(DefaultUninstallListPath);
            CurrentListFilename = DefaultUninstallListPath;
        }

        private void toolStripButtonToBasicFilters_Click(object sender, EventArgs e)
        {
            if (!AskToSaveUnsaved()) return;

            uninstallListEditor1.CurrentList = null;
        }
    }
}
