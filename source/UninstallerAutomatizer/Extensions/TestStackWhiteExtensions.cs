/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;

namespace UninstallerAutomatizer.Extensions
{
    public static class TestStackWhiteExtensions
    {
        /// <summary>
        /// Same as WaitWhileBusy, but doesn't throw when the application exits; it returns instead.
        /// </summary>
        public static void WaitWhileBusyAndAlive(this TestStack.White.Application app)
        {
            try
            {
                app.WaitWhileBusy();

            }
            catch (InvalidOperationException)
            {
                if (!app.HasExited)
                    throw;
            }
        }
    }
}