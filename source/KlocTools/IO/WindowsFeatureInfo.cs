/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;

namespace Klocman.IO
{
    public class WindowsFeatureInfo
    {
        public WindowsFeatureInfo()
        {
            CustomProperties = new List<KeyValuePair<string, string>>();
        }

        public string FeatureName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string RestartRequired { get; set; }
        public bool Enabled { get; set; }
        public IList<KeyValuePair<string, string>> CustomProperties { get; private set; }
    }
}