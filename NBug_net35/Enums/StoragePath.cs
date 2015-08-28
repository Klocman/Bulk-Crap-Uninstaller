// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoragePath.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NBug.Enums
{
    public enum StoragePath
    {
        /// <summary>
        ///     %Temp% directory is the default storage. Usually set to 'C:\Documents and Settings\Username\Local Settings\Temp'.
        /// </summary>
        WindowsTemp,

        /// <summary>
        ///     Initial working directory, i.e. where the executing assembly (MyProduct.exe) is located.
        /// </summary>
        CurrentDirectory,

        /// <summary>
        ///     User's isolated storage store (application scope).
        /// </summary>
        IsolatedStorage,

        /// <summary>
        ///     Custom path should be a full path like 'C:\Documents and Settings\MyUser\Local Settings\Temp'.
        /// </summary>
        /// <remarks>Path should not have a trailing slash. If the directory doesn't exist, it is created first.</remarks>
        Custom
    }
}