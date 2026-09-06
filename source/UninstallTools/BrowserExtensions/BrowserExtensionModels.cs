/*
    EBUninstaller Pro - Open Source Professional Windows Uninstaller
    Browser Extension Models
*/

using System;
using System.Collections.Generic;

namespace UninstallTools.BrowserExtensions
{
    public enum SupportedBrowser
    {
        GoogleChrome,
        MicrosoftEdge,
        MozillaFirefox,
        BraveBrowser,
        Opera,
        Vivaldi
    }

    public sealed class BrowserExtensionEntry
    {
        public string ExtensionId { get; set; }
        public SupportedBrowser Browser { get; set; }
        public string BrowserName => Browser.ToString();
        public string Name { get; set; }
        public string Version { get; set; }
        public string Publisher { get; set; }
        public string Description { get; set; }
        public string InstallPath { get; set; }
        public string ManifestPath { get; set; }
        public bool IsEnabled { get; set; } = true;
        public List<string> Permissions { get; set; } = new();

        public override string ToString() => $"[{Browser}] {Name} v{Version} (ID: {ExtensionId})";
    }
}
