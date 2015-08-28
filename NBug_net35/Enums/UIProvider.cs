// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIProvider.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NBug.Enums
{
    public enum UIProvider
    {
        /// <summary>
        ///     Automatic provider selection is the default setting.
        /// </summary>
        Auto,

        /// <summary>
        ///     Only the console is used to display the interface and interact with the user if necessary.
        /// </summary>
        Console,

        /// <summary>
        ///     Windows Forms interface is used for all UI displayed to the user.
        /// </summary>
        WinForms,

        /// <summary>
        ///     Windows Presentation Foundation interface is used for all UI displayed to the user.
        /// </summary>
        WPF,

        /// <summary>
        ///     Custom UIProvider.
        /// </summary>
        Custom
    }
}