// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomUI.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using NBug.Core.Reporting.Info;
using NBug.Core.Util.Exceptions;
using NBug.Core.Util.Serialization;
using NBug.Enums;
using NBug.Events;

namespace NBug.Core.UI.Custom
{
    /// <summary>
    ///     This class is used to prevent statically referencing any WPF dlls from the UISelector.cs thus prevents
    ///     any unnecessary assembly from getting loaded into the memory.
    /// </summary>
    internal static class CustomUI
    {
        internal static UIDialogResult ShowDialog(UIMode uiMode, SerializableException exception, Report report)
        {
            if (Settings.CustomUIHandle != null)
            {
                var e = new CustomUIEventArgs(uiMode, exception, report);
                Settings.CustomUIHandle.DynamicInvoke(null, e);
                return e.Result;
            }
            throw NBugConfigurationException.Create(() => Settings.UIMode,
                "Parameter supplied for settings property is invalid.");
        }
    }
}