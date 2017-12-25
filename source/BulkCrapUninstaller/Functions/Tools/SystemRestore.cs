/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.IO;
using BulkCrapUninstaller.Properties;
using Klocman.Forms;
using Klocman.IO;

namespace BulkCrapUninstaller.Functions.Tools
{
    internal static class SystemRestore
    {
        // Currently running system restore number
        private static long _currentRestoreId;

        /// <summary>
        ///     Ask the user to begin system restore and do so if he accepts. Returns false if user decides to cancel the
        ///     operation.
        /// </summary>
        /// <param name="count">How many items are being uninstalled</param>
        /// <returns></returns>
        public static bool BeginSysRestore(int count)
        {
            if (SysRestore.SysRestoreAvailable())
            {
                switch (MessageBoxes.SysRestoreBeginQuestion())
                {
                    case MessageBoxes.PressedButton.Yes:
                        var error = LoadingDialog.ShowDialog(null, Localisable.LoadingDialogTitleCreatingRestorePoint, x =>
                        {
                            //if (_currentRestoreId > 0)
                            EndSysRestore();

                            var result = SysRestore.StartRestore(MessageBoxes.GetSystemRestoreDescription(count),
                                SysRestore.RestoreType.ApplicationUninstall, out _currentRestoreId);
                            if (result < 0)
                                throw new IOException(Localisable.SysRestoreGenericError);
                        });

                        return error == null ||
                               MessageBoxes.SysRestoreContinueAfterError(error.Message) ==
                               MessageBoxes.PressedButton.Yes;

                    case MessageBoxes.PressedButton.Cancel:
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        ///     Cancel running restore if any
        /// </summary>
        public static void CancelSysRestore()
        {
            if (_currentRestoreId > 0)
            {
                SysRestore.CancelRestore(_currentRestoreId);
                _currentRestoreId = 0;
            }
        }

        public static void EndSysRestore()
        {
            if (_currentRestoreId > 0)
            {
                SysRestore.EndRestore(_currentRestoreId);
                _currentRestoreId = 0;
            }
        }
    }
}