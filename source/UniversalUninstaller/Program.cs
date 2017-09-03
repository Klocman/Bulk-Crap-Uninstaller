using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Klocman.Forms;

namespace UniversalUninstaller
{
    static class Program
    {
        static bool quietMode;
        /// <summary>
        /// The main entry point for the application.
        /// 
        /// args:
        /// Exe.exe [/q] DirPath
        /// /q - quiet
        /// </summary>
        [STAThread]
        static int Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var args = Environment.GetCommandLineArgs().Skip(1).ToList();

            if (args.Any(x => x.Equals("/q", StringComparison.OrdinalIgnoreCase)))
                quietMode = true;

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

            if (!quietMode)
                Application.Run(new UninstallSelection(dir));
            else
            {
                try
                {
                    DeleteItems(new[] { dir });
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
            if (quietMode) return;

            MessageBox.Show(
                "Invalid number of arguments. Pass full path of the directory to remove as an argument, and optionally /q for quiet mode.",
                "Universal Uninstaller", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
