/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace Klocman.IO
{
    public static class WmiQueries
    {
        /// <summary>
        /// Get information about enabled and disabled windows features. Works on Windows 7 and newer.
        /// </summary>
        public static ICollection<WindowsFeatureInfo> GetWindowsFeatures()
        {
            var features = new List<WindowsFeatureInfo>();

            var searcher = new ManagementObjectSearcher(new ManagementScope(), 
                new ObjectQuery("select * from Win32_OptionalFeature"), 
                new EnumerationOptions(null, TimeSpan.FromSeconds(35), 100, false, false, false, false, false, false, false));
            using (var moc = searcher.Get())
            {
                var items = moc.Cast<ManagementObject>().ToList();
                foreach (var managementObject in items)
                {
                    var featureInfo = new WindowsFeatureInfo();
                    foreach (var property in managementObject.Properties)
                    {
                        if (property.Name == "Caption")
                        {
                            featureInfo.DisplayName = property.Value.ToString();
                        }
                        else if (property.Name == "InstallState")
                        {
                            var status = (uint)property.Value;
                            if (status == 2)
                                featureInfo.Enabled = false;
                            else if (status == 1)
                                featureInfo.Enabled = true;
                            else
                            {
                                featureInfo.FeatureName = null;
                                break;
                            }
                        }
                        else if (property.Name == "Name")
                        {
                            featureInfo.FeatureName = property.Value.ToString();
                        }
                    }

                    if (string.IsNullOrEmpty(featureInfo.FeatureName)) continue;

                    features.Add(featureInfo);
                }
            }

            return features;
        }
    }
}
