/*
    EBUninstaller Pro - Open Source Professional Windows Uninstaller
    Browser Extension Manager Subsystem
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UninstallTools.Core;
using UninstallTools.FileSystemEngine;

namespace UninstallTools.BrowserExtensions
{
    public static class BrowserExtensionManager
    {
        public static async Task<List<BrowserExtensionEntry>> GetInstalledExtensionsAsync(
            SupportedBrowser? filterBrowser = null,
            CancellationToken cancellationToken = default)
        {
            var results = new List<BrowserExtensionEntry>();
            StructuredLogger.Info(LogCategory.BrowserExtensions, "Scanning browser extensions");

            await Task.Run(() =>
            {
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                // Chrome
                if (!filterBrowser.HasValue || filterBrowser.Value == SupportedBrowser.GoogleChrome)
                {
                    ScanChromiumExtensions(SupportedBrowser.GoogleChrome,
                        Path.Combine(localAppData, "Google", "Chrome", "User Data"), results);
                }

                // Edge
                if (!filterBrowser.HasValue || filterBrowser.Value == SupportedBrowser.MicrosoftEdge)
                {
                    ScanChromiumExtensions(SupportedBrowser.MicrosoftEdge,
                        Path.Combine(localAppData, "Microsoft", "Edge", "User Data"), results);
                }

                // Brave
                if (!filterBrowser.HasValue || filterBrowser.Value == SupportedBrowser.BraveBrowser)
                {
                    ScanChromiumExtensions(SupportedBrowser.BraveBrowser,
                        Path.Combine(localAppData, "BraveSoftware", "Brave-Browser", "User Data"), results);
                }

                // Opera
                if (!filterBrowser.HasValue || filterBrowser.Value == SupportedBrowser.Opera)
                {
                    ScanChromiumExtensions(SupportedBrowser.Opera,
                        Path.Combine(appData, "Opera Software", "Opera Stable"), results);
                }

                // Vivaldi
                if (!filterBrowser.HasValue || filterBrowser.Value == SupportedBrowser.Vivaldi)
                {
                    ScanChromiumExtensions(SupportedBrowser.Vivaldi,
                        Path.Combine(localAppData, "Vivaldi", "User Data"), results);
                }

                // Firefox
                if (!filterBrowser.HasValue || filterBrowser.Value == SupportedBrowser.MozillaFirefox)
                {
                    ScanFirefoxExtensions(Path.Combine(appData, "Mozilla", "Firefox", "Profiles"), results);
                }
            }, cancellationToken).ConfigureAwait(false);

            StructuredLogger.Info(LogCategory.BrowserExtensions, $"Found {results.Count} browser extensions.");
            return results;
        }

        private static void ScanChromiumExtensions(SupportedBrowser browser, string userDataRoot, List<BrowserExtensionEntry> accumulator)
        {
            if (string.IsNullOrEmpty(userDataRoot) || !Directory.Exists(userDataRoot)) return;

            // Search Default profile and Profile 1..N
            var profileDirs = new List<string> { Path.Combine(userDataRoot, "Default") };
            try
            {
                foreach (var dir in Directory.GetDirectories(userDataRoot, "Profile *"))
                {
                    profileDirs.Add(dir);
                }
            }
            catch { }

            foreach (var pDir in profileDirs.Where(Directory.Exists))
            {
                var extRoot = Path.Combine(pDir, "Extensions");
                if (!Directory.Exists(extRoot)) continue;

                try
                {
                    foreach (var extIdDir in Directory.GetDirectories(extRoot))
                    {
                        var extId = Path.GetFileName(extIdDir);
                        // Each extension folder contains subfolders by version number (e.g. 1.0.0_0)
                        foreach (var verDir in Directory.GetDirectories(extIdDir))
                        {
                            var manifestPath = Path.Combine(verDir, "manifest.json");
                            if (File.Exists(manifestPath))
                            {
                                var entry = ParseChromiumManifest(browser, extId, verDir, manifestPath);
                                if (entry != null)
                                    accumulator.Add(entry);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Warning(LogCategory.BrowserExtensions, $"Error scanning Chromium extensions in {pDir}", ex.Message);
                }
            }
        }

        private static BrowserExtensionEntry ParseChromiumManifest(SupportedBrowser browser, string extId, string versionDir, string manifestPath)
        {
            try
            {
                var json = File.ReadAllText(manifestPath);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var name = root.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : extId;
                var version = root.TryGetProperty("version", out var verProp) ? verProp.GetString() : Path.GetFileName(versionDir);
                var description = root.TryGetProperty("description", out var descProp) ? descProp.GetString() : string.Empty;
                var author = root.TryGetProperty("author", out var authProp) ? authProp.GetString() : null;

                // Handle localized names (__MSG_appName__)
                if (name != null && name.StartsWith("__MSG_", StringComparison.OrdinalIgnoreCase))
                {
                    var msgKey = name.Replace("__MSG_", "").Replace("__", "").Trim();
                    name = ResolveLocaleMessage(versionDir, msgKey) ?? extId;
                }

                if (description != null && description.StartsWith("__MSG_", StringComparison.OrdinalIgnoreCase))
                {
                    var msgKey = description.Replace("__MSG_", "").Replace("__", "").Trim();
                    description = ResolveLocaleMessage(versionDir, msgKey) ?? description;
                }

                var permissions = new List<string>();
                if (root.TryGetProperty("permissions", out var permProp) && permProp.ValueKind == JsonValueKind.Array)
                {
                    foreach (var p in permProp.EnumerateArray())
                    {
                        var pStr = p.GetString();
                        if (!string.IsNullOrEmpty(pStr)) permissions.Add(pStr);
                    }
                }

                return new BrowserExtensionEntry
                {
                    ExtensionId = extId,
                    Browser = browser,
                    Name = name,
                    Version = version,
                    Publisher = author ?? "Third-party Developer",
                    Description = description,
                    InstallPath = versionDir,
                    ManifestPath = manifestPath,
                    IsEnabled = true,
                    Permissions = permissions
                };
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.BrowserExtensions, $"Failed to parse manifest {manifestPath}", ex.Message);
                return null;
            }
        }

        private static string ResolveLocaleMessage(string versionDir, string messageKey)
        {
            var localesDir = Path.Combine(versionDir, "_locales");
            if (!Directory.Exists(localesDir)) return null;

            // Try 'en', 'en_US', or first available folder
            var localeFolders = new[] { "en", "en_US", "en_GB" };
            foreach (var loc in localeFolders)
            {
                var msgFile = Path.Combine(localesDir, loc, "messages.json");
                if (File.Exists(msgFile))
                {
                    var val = ReadMessageFromJson(msgFile, messageKey);
                    if (!string.IsNullOrEmpty(val)) return val;
                }
            }

            try
            {
                foreach (var locDir in Directory.GetDirectories(localesDir))
                {
                    var msgFile = Path.Combine(locDir, "messages.json");
                    if (File.Exists(msgFile))
                    {
                        var val = ReadMessageFromJson(msgFile, messageKey);
                        if (!string.IsNullOrEmpty(val)) return val;
                    }
                }
            }
            catch { }

            return null;
        }

        private static string ReadMessageFromJson(string messagesJsonPath, string messageKey)
        {
            try
            {
                var json = File.ReadAllText(messagesJsonPath);
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty(messageKey, out var obj) &&
                    obj.TryGetProperty("message", out var msgProp))
                {
                    return msgProp.GetString();
                }
            }
            catch { }
            return null;
        }

        private static void ScanFirefoxExtensions(string profilesDir, List<BrowserExtensionEntry> accumulator)
        {
            if (string.IsNullOrEmpty(profilesDir) || !Directory.Exists(profilesDir)) return;

            try
            {
                foreach (var profile in Directory.GetDirectories(profilesDir))
                {
                    var extJsonPath = Path.Combine(profile, "extensions.json");
                    if (File.Exists(extJsonPath))
                    {
                        ParseFirefoxExtensionsJson(extJsonPath, accumulator);
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.BrowserExtensions, "Failed scanning Firefox extensions", ex.Message);
            }
        }

        private static void ParseFirefoxExtensionsJson(string jsonPath, List<BrowserExtensionEntry> accumulator)
        {
            try
            {
                var json = File.ReadAllText(jsonPath);
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("addons", out var addons) && addons.ValueKind == JsonValueKind.Array)
                {
                    foreach (var addon in addons.EnumerateArray())
                    {
                        var id = addon.TryGetProperty("id", out var idProp) ? idProp.GetString() : string.Empty;
                        var name = addon.TryGetProperty("defaultLocale", out var defLoc) &&
                                   defLoc.TryGetProperty("name", out var nProp)
                            ? nProp.GetString()
                            : id;
                        var version = addon.TryGetProperty("version", out var vProp) ? vProp.GetString() : string.Empty;
                        var active = addon.TryGetProperty("active", out var actProp) && actProp.GetBoolean();
                        var rootUri = addon.TryGetProperty("rootURI", out var uriProp) ? uriProp.GetString() : string.Empty;

                        accumulator.Add(new BrowserExtensionEntry
                        {
                            ExtensionId = id,
                            Browser = SupportedBrowser.MozillaFirefox,
                            Name = name,
                            Version = version,
                            Publisher = "Mozilla Add-on",
                            Description = "Firefox Extension",
                            InstallPath = rootUri,
                            IsEnabled = active
                        });
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Removes a browser extension folder safely.
        /// </summary>
        public static bool RemoveExtension(BrowserExtensionEntry entry)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.InstallPath))
                return false;

            StructuredLogger.Info(LogCategory.BrowserExtensions, $"Removing browser extension: {entry.Name} ({entry.InstallPath})");

            // For Chromium, the parent folder is the extension root
            var extParent = Path.GetDirectoryName(entry.InstallPath);
            if (Directory.Exists(extParent))
            {
                return SafeFileSystemEngine.DeleteDirectorySafe(extParent, DeletionMode.SendToRecycleBin);
            }

            if (Directory.Exists(entry.InstallPath))
            {
                return SafeFileSystemEngine.DeleteDirectorySafe(entry.InstallPath, DeletionMode.SendToRecycleBin);
            }

            return false;
        }
    }
}
