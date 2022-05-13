using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;

namespace ContextMenuExtension
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".lnk")]
    public class UninstallExtension : SharpContextMenu
    {
        private readonly string _consolePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "BCUninstaller\\win-x64\\BCU-Console.exe");
        private readonly List<string> _executables = new List<string>();

        protected override bool CanShowMenu()
        {
            _executables.Clear();
            _executables.AddRange(from item in SelectedItemPaths
                                  where IsExecutable(item)
                                  select GetShortcutTarget(item));
            return _executables.Count > 0;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();

            var itemUninstall = new ToolStripMenuItem
            {
                Text = "Uninstall with BCUninstaller"
            };

            itemUninstall.Click += (sender, args) => RunBcUninstaller();

            menu.Items.Add(itemUninstall);

            return menu;
        }

        private void RunBcUninstaller()
        {
            var result = MessageBox.Show("Uninstallation will start in the background.", "BCUninstaller", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result != DialogResult.OK)
            {
                return;
            }

            var argument = $"uninstall {string.Join(" ", _executables.Select(exe => "\"" + exe + "\"").ToArray())} /P /U";
#if DEBUG
            MessageBox.Show($"Arguments: {argument}", "BCUninstaller", MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _consolePath,
                    Arguments = argument,
                    RedirectStandardError = false,
                    RedirectStandardOutput = false,
                    UseShellExecute = true,
                    ErrorDialog = true,
#if DEBUG
                    WindowStyle = ProcessWindowStyle.Hidden
#endif
                },
                EnableRaisingEvents = true
            };
            process.Exited += (sender, obj) => {
                _ = process.ExitCode != 0
                    ? MessageBox.Show("An error occurred during installation. Please use BCUninstaller GUI.", "BCUninstaller", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    : MessageBox.Show("Operation completed successfully.", "BCUninstaller", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            process.Start();

        }

        private static bool IsExecutable(string item)
        {
            var target = GetShortcutTarget(item);
            return !string.IsNullOrEmpty(target) && Path.GetExtension(target).Equals(".exe", StringComparison.Ordinal);
        }

        /// <summary>
        ///     The method reads the binary file to find target path ignoring the arguments.
        ///     Please check the reference: <see href="https://blez.wordpress.com/2013/02/18/get-file-shortcuts-target-with-c/"/>
        /// </summary>
        /// <param name="filePath">LNK File full path</param>
        /// <returns>Target path or empty string.</returns>
        private static string GetShortcutTarget(string filePath)
        {
            try
            {
                if (!StringComparer.OrdinalIgnoreCase.Equals(Path.GetExtension(filePath), ".lnk"))
                {
                    return string.Empty;
                }

                var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                using (var fileReader = new BinaryReader(fileStream))
                {
                    fileStream.Seek(0x0, SeekOrigin.Begin);
                    // The first 4 bytes of the file form a long integer that is always set to 4Ch this it the ASCII value for the uppercase letter L.
                    // This is used to identify a valid shell link file.

                    var l = fileReader.ReadInt32();
                    if (l != 76)
                    {
                        return string.Empty;
                    }

                    fileStream.Seek(0x14, SeekOrigin.Begin);     // Seek to flags

                    //var flags = fileReader.ReadBytes(4);        // Read flags
                    //var flag = new DataFlag(flags);

                    var flags = fileReader.ReadUInt32();        // Read flags
                    if ((flags & 1) == 1)
                    {                      // Bit 1 set means we have to
                                           // skip the shell item ID list
                        fileStream.Seek(0x4c, SeekOrigin.Begin); // Seek to the end of the header
                        uint offset = fileReader.ReadUInt16();   // Read the length of the Shell item ID list
                        fileStream.Seek(offset, SeekOrigin.Current); // Seek past it (to the file locator info)
                    }

                    var fileInfoStartsAt = fileStream.Position; // Store the offset where the file info
                                                                // structure begins
                    var totalStructLength = fileReader.ReadUInt32(); // read the length of the whole struct
                    fileStream.Seek(0xc, SeekOrigin.Current); // seek to offset to base pathname
                    var fileOffset = fileReader.ReadUInt32(); // read offset to base pathname
                                                              // the offset is from the beginning of the file info struct (fileInfoStartsAt)
                    fileStream.Seek((fileInfoStartsAt + fileOffset), SeekOrigin.Begin); // Seek to beginning of
                                                                                        // base pathname (target)
                    var pathLength = (totalStructLength + fileInfoStartsAt) - fileStream.Position - 2; // read
                                                                                                       // the base pathname. I don't need the 2 terminating nulls.


                    var link = Encoding.Default.GetString(fileReader.ReadBytes((int)pathLength));

                    var begin = link.IndexOf("\0\0");
                    if (begin > -1)
                    {
                        var end = link.IndexOf("\\\\", begin + 2) + 2;
                        end = link.IndexOf('\0', end) + 1;

                        var firstPart = link.Substring(0, begin);
                        var secondPart = link.Substring(end);

                        return firstPart + secondPart;
                    }
                    else
                    {
                        return link;
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}