using System;

namespace UninstallerAutomatizer
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