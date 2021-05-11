// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIMode.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NBug.Enums
{
    public enum UIMode
    {
        /// <summary>
        ///     Automatic mode selection is the default setting. Mode and provider is automatically selected for different
        ///     application types.
        /// </summary>
        Auto,

        /// <summary>
        ///     No user interface is displayed at all. All the exception handling and bug reporting process is silent. In this
        ///     mode, termination of
        ///     of the host application can be skipped altogether via <see cref="Settings.ExitApplicationImmediately" />
        /// </summary>
        None,

        /// <summary>
        ///     Minimal user interface is displayed. This consists of a simple message box for WinForms and WPF, and a single line
        ///     of information
        ///     message for console applications.
        /// </summary>
        Minimal,

        /// <summary>
        ///     Normal user interface is displayed to the user, which strikes a balance between the level of details shown about
        ///     the exception and
        ///     being still user friendly. This closely replicates the original interface displayed by CLR in case of unhandled
        ///     exceptions.
        /// </summary>
        Normal,

        /// <summary>
        ///     Full blown user interface is displayed to the user. This interface contains as much detail about the exception and
        ///     the application
        ///     as possible. This is very useful for power users.
        /// </summary>
        Full
    }
}