/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions;
using BulkCrapUninstaller.Properties;
using Klocman.Extensions;
using Klocman.Forms.Tools;
using Klocman.Tools;
using UninstallTools;

namespace BulkCrapUninstaller.Controls
{
    public partial class FileTargeter : UserControl
    {
        public FileTargeter()
        {
            InitializeComponent();
        }

        private void ProcessFiles(ICollection<string> files)
        {
            if (files == null || files.Count < 1)
                return;

            var folders = new List<string>();

            foreach (var file in files)
            {
                var fname = file;

                if (string.IsNullOrEmpty(fname))
                    continue;

                RewindDropLoop:

                if (Directory.Exists(fname))
                {
                    folders.Add(fname);
                    continue;
                }

                if (!File.Exists(fname))
                {
                    try
                    {
                        fname = ProcessTools.SeparateArgsFromCommand(file).FileName;

                        if (Directory.Exists(fname))
                        {
                            folders.Add(fname);
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

                if (fname.TrimEnd().EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var result = WindowsTools.ResolveShortcut(fname);
                        if (result != null)
                        {
                            fname = result;
                            goto RewindDropLoop;
                        }
                    }
                    catch (Exception ex)
                    {
                        PremadeDialogs.GenericError(ex);
                    }
                }
                else
                {
                    try
                    {
                        var dirName = Path.GetDirectoryName(fname);
                        folders.Add(dirName);
                    }
                    catch (Exception ex)
                    {
                        PremadeDialogs.GenericError(ex);
                    }
                }
            }

            var distinctFolders = folders.Where(x => x != null)
                .Select(x => x.SafeNormalize().ToLowerInvariant().Trim().Trim('\'', '"').Trim())
                .Distinct();

            var folderInfos = distinctFolders.Select(x =>
            {
                try
                {
                    return new DirectoryInfo(x);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
            }).Where(x => x != null);

            var results = folderInfos.Where(x => !UninstallToolsGlobalConfig.IsSystemDirectory(x)).ToList();

            DirectoriesSelected?.Invoke(this, new DirectoriesSelectedEventArgs(results));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AllowDrop = true;
        }

        public event EventHandler<DirectoriesSelectedEventArgs> DirectoriesSelected;

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK
                && openFileDialog1.FileNames.Any())
                ProcessFiles(openFileDialog1.FileNames);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ParentForm != null) ParentForm.Enabled = false;
            else Enabled = false;

            var path = MessageBoxes.SelectFolder(Localisable.FileTargeter_SelectDirectoryWithAppsToRemove);
            if (!string.IsNullOrEmpty(path))
                ProcessFiles(new[] { path });
            
            if (ParentForm != null) ParentForm.Enabled = true;
            else Enabled = true;
        }
    }

    public sealed class DirectoriesSelectedEventArgs : EventArgs
    {
        public DirectoriesSelectedEventArgs(ICollection<DirectoryInfo> selectedFiles)
        {
            SelectedFiles = selectedFiles;
        }

        public ICollection<DirectoryInfo> SelectedFiles { get; }
    }
}
