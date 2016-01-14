using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using BulkCrapUninstaller.Properties;
using UninstallTools.Lists;

namespace BulkCrapUninstaller.Controls
{
    public partial class AdvancedFilters : UserControl
    {
        public AdvancedFilters()
        {
            InitializeComponent();

            uninstallListEditor1.CurrentListChanged += OnCurrentListChanged;
            uninstallListEditor1.FiltersChanged += OnFiltersChanged;
        }

        private void OnFiltersChanged(object sender, EventArgs e)
        {
            FiltersChanged?.Invoke(sender, e);
        }

        private void OnCurrentListChanged(object sender, EventArgs e)
        {
            if (CurrentList == null)
                CurrentListFilename = string.Empty;
            CurrentListChanged?.Invoke(sender, e);
        }

        public event EventHandler CurrentListChanged;
        public event EventHandler CurrentListFilenameChanged;
        public event EventHandler FiltersChanged;

        public UninstallList CurrentList => uninstallListEditor1.CurrentList;

        public void LoadUninstallList(UninstallList list)
        {
            CurrentListFilename = string.Empty;
            uninstallListEditor1.CurrentList = list;
        }

        public void LoadUninstallList(string filename)
        {
            var result = UninstallList.ReadFromFile(filename);

            CurrentListFilename = filename;
            uninstallListEditor1.CurrentList = result;
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

        private void toolStripButtonToBasicFilters_Click(object sender, EventArgs e)
        {
            //TODO ask to save
            uninstallListEditor1.CurrentList = null;
        }

        private void toolStripButtonOpenUl_Click(object sender, EventArgs e)
        {
            //TODO ask to save
            if (!string.IsNullOrEmpty(CurrentListFilename))
            {
                try
                {
                    openUlDialog.InitialDirectory = Path.GetDirectoryName(CurrentListFilename);
                    openUlDialog.FileName = Path.GetFileName(CurrentListFilename);
                }
                catch (ArgumentException) { }
                catch (PathTooLongException) { }
            }
            openUlDialog.ShowDialog(this);
        }

        private void toolStripButtonSaveUl_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CurrentListFilename))
            {
                try
                {
                    saveUlDialog.InitialDirectory = Path.GetDirectoryName(CurrentListFilename);
                    saveUlDialog.FileName = Path.GetFileName(CurrentListFilename);
                }
                catch (ArgumentException) { }
                catch (PathTooLongException) { }
            }
            saveUlDialog.ShowDialog(this);
        }

        private static readonly string DefaultUninstallListPath =
            Path.Combine(Program.AssemblyLocation.FullName, Resources.DefaultUninstallListFilename);

        private string _currentListFilename;

        private void toolStripButtonSaveUlDef_Click(object sender, EventArgs e)
        {
            CurrentList.SaveToFile(DefaultUninstallListPath);
            CurrentListFilename = DefaultUninstallListPath;
        }

        private void openUlDialog_FileOk(object sender, CancelEventArgs e)
        {
            //todo error message
            LoadUninstallList(openUlDialog.FileName);
        }

        private void saveUlDialog_FileOk(object sender, CancelEventArgs e)
        {
            //todo error message
            CurrentList.SaveToFile(saveUlDialog.FileName);
            CurrentListFilename = saveUlDialog.FileName;
        }
    }
}
