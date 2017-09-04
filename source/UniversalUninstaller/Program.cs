using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UniversalUninstaller.Properties;

namespace UniversalUninstaller
{
    internal static class Program
    {
        private static bool _quietMode;

        /// <summary>
        /// The main entry point for the application.
        /// args:
        /// Exe.exe [/q] DirPath
        /// /q - quiet
        /// return codes:
        /// 0 - ok
        /// 1 - Installation aborted by user (cancel button)
        /// 11 - invalid arguments
        /// 161 - failed to delete
        /// </summary>
        [STAThread]
        private static int Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var args = Environment.GetCommandLineArgs().Skip(1).ToList();

            if (args.Any(x => x.Equals("/q", StringComparison.OrdinalIgnoreCase)))
                _quietMode = true;

            if (args.Count > 2 || args.Count < 1)
            {
                ShowInvalidArgsBox();
                return 11;
            }

            var strings = args.Where(x => !x.StartsWith("/", StringComparison.Ordinal)).ToList();
            if (strings.Count != 1)
            {
                ShowInvalidArgsBox();
                return 11;
            }

            DirectoryInfo dir;
            try
            {
                dir = new DirectoryInfo(strings.Single());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ShowInvalidArgsBox();
                return 11;
            }

            if (!_quietMode)
            {
                var uninstallWindow = new UninstallSelection(dir);
                Application.Run(uninstallWindow);
                if (uninstallWindow.WasCancelled)
                    return 1;
                if (uninstallWindow.DeleteFailed)
                    return 161;
            }
            else
            {
                try
                {
                    DeleteItems(new[] {dir});
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return 161;
                }
            }

            return 0;
        }

        public static void DeleteItems(IEnumerable<FileSystemInfo> it)
        {
            foreach (var fileSystemInfo in it)
            {
                var di = fileSystemInfo as DirectoryInfo;
                if (di != null)
                    di.Delete(true);
                else
                    fileSystemInfo.Delete();
            }
        }

        private static void ShowInvalidArgsBox()
        {
            if (_quietMode) return;

            MessageBox.Show(Localisation.Program_ShowInvalidArgsBox_Message,
                Localisation.Program_ShowInvalidArgsBox_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}