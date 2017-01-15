using System;
using System.Collections.Generic;
using System.Management;
using Klocman.Extensions;
using Klocman.Native;
using Klocman.Tools;

namespace UninstallTools.Startup.Service
{
    internal class ServiceEntryFactory
    {
        internal enum StartMode
        {
            Auto,
            Manual,
            Disabled
        }

        /* ServiceType
Kernel Driver 
File System Driver 
Adapter 
Recognizer Driver 
Own Process 
Share Process 
Interactive Process 
*/

        public static IEnumerable<ServiceEntry> GetServiceEntries()
        {
            var searcher =
                new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT * FROM Win32_Service");

            var results = new List<ServiceEntry>();
            foreach (ManagementObject queryObj in searcher.Get())
            {
                // Skip drivers and adapters
                if (!((string)queryObj["ServiceType"]).Contains("Process"))
                    continue;

                var filename = queryObj["PathName"] as string;
                // Don't show system services
                if (filename.Contains(WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_WINDOWS),
                    StringComparison.InvariantCultureIgnoreCase))
                    continue;

                var e = new ServiceEntry((string)queryObj["Name"],
                    queryObj["DisplayName"] as string, filename);

                //queryObj["Caption"]);
                //queryObj["Description"]);

                // for killing before junk delete (not necessarily uninstall?) queryObj["ProcessId"]

                results.Add(e);
            }

            return results.ToArray();
        }

        private static bool GetEnabledState(ManagementBaseObject queryObj)
        {
            return queryObj["StartMode"] as string != nameof(StartMode.Auto);
        }

        public static void EnableService(string serviceName, bool newState)
        {
            var classInstance = GetServiceObject(serviceName);

            // Obtain in-parameters for the method
            var inParams = classInstance.GetMethodParameters("ChangeStartMode");

            // Add the input parameters.
            inParams["StartMode"] = newState ? "Automatic" : "Disabled";

            // Execute the method and obtain the return values.
            var outParams = classInstance.InvokeMethod("ChangeStartMode", inParams, null);

            if (((UInt32)outParams["ReturnValue"]) != 0)
                throw new ManagementException("ChangeStartMode returned " + outParams["ReturnValue"]);
        }

        public static bool CheckServiceEnabled(string serviceName)
        {
            var classInstance = GetServiceObject(serviceName);

            return GetEnabledState(classInstance);
        }

        public static void DeleteService(string serviceName)
        {
            try { EnableService(serviceName, false); }
            catch(ManagementException) { }

            var classInstance = GetServiceObject(serviceName);
            
            // Execute the method and obtain the return values.
            var outParams = classInstance.InvokeMethod("Delete", null, null);

            if (((UInt32)outParams["ReturnValue"]) != 0)
                throw new ManagementException("ChangeStartMode returned " + outParams["ReturnValue"]);
        }

        private static ManagementObject GetServiceObject(string serviceName)
        {
            return new ManagementObject("root\\CIMV2", $"Win32_Service.Name='{serviceName}'", null);
        }
    }
}