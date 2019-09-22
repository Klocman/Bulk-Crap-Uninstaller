/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;
using BulkCrapUninstaller.Properties;
using Klocman.Forms;
using Klocman.IO;

namespace BulkCrapUninstaller.Functions.Tools
{
    internal static class SystemRestore
    {
        /// <summary>
        /// Currently running system restore number
        /// </summary>
        private static long _currentRestoreId;

        /// <summary>
        ///     Ask the user to begin system restore and do so if he accepts. Returns false if user decides to cancel the
        ///     operation.
        /// </summary>
        /// <param name="count">How many items are being uninstalled</param>
        /// <param name="displayMessage">If user should be asked to create the restore point. If false, always create</param>
        public static bool BeginSysRestore(int count, bool displayMessage = true)
        {
            if (SysRestore.SysRestoreAvailable())
            {
                switch (displayMessage ? MessageBoxes.SysRestoreBeginQuestion() : MessageBoxes.PressedButton.Yes)
                {
                    case MessageBoxes.PressedButton.Yes:
                        var error = LoadingDialog.ShowDialog(null, Localisable.LoadingDialogTitleCreatingRestorePoint, x =>
                        {
                            //if (_currentRestoreId > 0)
                            EndSysRestore();

                            var result = SysRestore.StartRestore(MessageBoxes.GetSystemRestoreDescription(count),
                                SysRestore.RestoreType.ApplicationUninstall, out _currentRestoreId, 3);
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
            try { SysRestore.CancelRestore(_currentRestoreId); }
            catch (Exception ex) { Console.WriteLine(ex); }
        }

        public static void EndSysRestore()
        {
            try { SysRestore.EndRestore(_currentRestoreId); }
            catch (Exception ex) { Console.WriteLine(ex); }
        }
    }
}