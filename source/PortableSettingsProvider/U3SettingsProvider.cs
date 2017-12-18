using System;

namespace PortableSettingsProvider
{
    public class U3SettingsProvider : PortableSettingsProvider
    {
        private const string U3APPDATAPATH = "U3_APP_DATA_PATH";

        public override string GetAppSettingsPath()
        {
            // Get the environment variable set by the U3 application for a pointer to its data
            try
            {
                return Environment.GetEnvironmentVariable(U3APPDATAPATH);
            }
            catch (Exception)
            {
                // Not running in a U3 environment, just return the application path

                return base.GetAppSettingsPath();
            }
        }
    }
}
