/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Reflection;
using System.Windows.Forms;

namespace Klocman.Extensions
{
    public static class CheckBoxExtension
    {
        private static readonly FieldInfo CheckStateInfo = typeof (CheckBox).GetField("checkState",
            BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        ///     Set the CheckState value, optionally not raising the CheckStateChanged event.
        /// </summary>
        /// <param name="check">New state</param>
        /// <param name="raiseEvent">If true the CheckStateChanged event will not be raised. The control will still be invalidated.</param>
        /// <param name="chBox">Checkbox to access</param>
        public static void SetCheckState(this CheckBox chBox, CheckState check, bool raiseEvent)
        {
            if (raiseEvent)
            {
                chBox.CheckState = check;
            }
            else
            {
                CheckStateInfo.SetValue(chBox, check);
                chBox.Invalidate();
            }
        }
    }
}