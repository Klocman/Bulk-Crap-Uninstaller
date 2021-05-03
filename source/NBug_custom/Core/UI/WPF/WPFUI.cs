// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WPFUI.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using NBug.Core.Reporting.Info;
using NBug.Core.UI.WinForms;
using NBug.Core.Util.Exceptions;
using NBug.Core.Util.Serialization;
using NBug.Enums;

namespace NBug.Core.UI.WPF
{
    /// <summary>
    ///     This class is used to prevent statically referencing any WPF dlls from the UISelector.cs thus prevents
    ///     any unnecessary assembly from getting loaded into the memory.
    /// </summary>
    internal static class WPFUI
    {
        internal static UIDialogResult ShowDialog(UIMode uiMode, SerializableException exception, Report report)
        {
            if (uiMode == UIMode.Minimal)
            {
                return new Minimal().ShowDialog(report);
            }
            if (uiMode == UIMode.Normal)
            {
                using (var ui = new Normal())
                {
                    return ui.ShowDialog(report);
                }
            }
            if (uiMode == UIMode.Full)
            {
                using (var ui = new Full())
                {
                    return ui.ShowDialog(exception, report);
                }
            }
            throw NBugConfigurationException.Create(() => Settings.UIMode,
                "Parameter supplied for settings property is invalid.");
        }
    }
}