/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using BulkCrapUninstaller.Controls;
using Klocman.Extensions;
using Klocman.Forms.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BulkCrapUninstaller.Properties;

namespace BulkCrapUninstaller.Forms
{
    public partial class TargetWindow : Form
    {
        public event EventHandler<DirectoriesSelectedEventArgs> DirectoriesSelected;

        public TargetWindow()
        {
            InitializeComponent();

            fileTargeter1.DirectoriesSelected += DirectoryTargeterDirectoriesSelected;
            windowTargeter1.WindowSelected += WindowTargeterWindowSelected;
        }

        private void DirectoryTargeterDirectoriesSelected(object sender, DirectoriesSelectedEventArgs e)
        {
            if (e.SelectedFiles.Count == 0)
            {
                MessageBox.Show(
                    Localisable.TargetWindow_NoFilesSelected_Message,
                    Localisable.TargetWindow_NoFilesSelected_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OnDirectoriesSelected(e);
        }

        protected virtual void OnDirectoriesSelected(DirectoriesSelectedEventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
            DirectoriesSelected?.Invoke(this, e);
        }

        private void WindowTargeterWindowSelected(object sender, Klocman.Subsystems.WindowHoverEventArgs e)
        {
            try
            {
                var parentDirectory = new FileInfo(e.TargetWindow.GetRunningProcess().MainModule.FileName).Directory;
                OnDirectoriesSelected(new DirectoriesSelectedEventArgs(parentDirectory.ToEnumerable().ToList()));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                PremadeDialogs.GenericError(exception);
            }
        }

        public new static ICollection<DirectoryInfo> ShowDialog(IWin32Window owner)
        {
            using (var window = new TargetWindow())
            {
                window.StartPosition = FormStartPosition.Manual;
                var targeterHalf = window.windowTargeter1.Height / 2;
                var offsetx = targeterHalf + 10;
                var offsety = targeterHalf + 30;
                window.Location = new Point(Cursor.Position.X - offsetx, Cursor.Position.Y - offsety);

                ICollection<DirectoryInfo> results = null;
                window.DirectoriesSelected += (sender, args) => results = args.SelectedFiles;

                if (window.ShowDialog() != DialogResult.OK)
                    return null;

                return results;
            }
        }
    }
}
