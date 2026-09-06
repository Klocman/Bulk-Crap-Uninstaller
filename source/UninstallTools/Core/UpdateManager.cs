/*
    EBUninstaller Pro - Application Update & Version Engine
    Offline-First, Cryptographically Verified Software Updater
*/

using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace UninstallTools.Core
{
    public enum UpdateChannel
    {
        Stable,
        Beta,
        Nightly
    }

    public sealed class UpdateCheckResult
    {
        public bool IsUpdateAvailable { get; set; }
        public string CurrentVersion { get; set; }
        public string LatestVersion { get; set; }
        public string ReleaseNotes { get; set; }
        public string DownloadUrl { get; set; }
        public string Sha256Checksum { get; set; }
        public DateTime ReleaseDate { get; set; }
        public UpdateChannel Channel { get; set; }
        public string ErrorMessage { get; set; }
    }

    public static class UpdateManager
    {
        public static readonly Version CurrentAssemblyVersion = new(7, 0, 0);
        public static UpdateChannel SelectedChannel { get; set; } = UpdateChannel.Stable;

        private const string ReleaseApiUrl = "https://api.github.com/repos/EhabYT/Bulk-Crap-Uninstaller/releases/latest";

        public static async Task<UpdateCheckResult> CheckForUpdatesAsync(bool isManualCheck = false)
        {
            var result = new UpdateCheckResult
            {
                CurrentVersion = CurrentAssemblyVersion.ToString(3),
                Channel = SelectedChannel
            };

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", $"EBUninstaller-Pro/{CurrentAssemblyVersion}");
                client.Timeout = TimeSpan.FromSeconds(10);

                var response = await client.GetStringAsync(ReleaseApiUrl).ConfigureAwait(false);
                using var doc = JsonDocument.Parse(response);
                var root = doc.RootElement;

                if (root.TryGetProperty("tag_name", out var tagElem))
                {
                    var tag = tagElem.GetString()?.TrimStart('v', 'V');
                    result.LatestVersion = tag;

                    if (Version.TryParse(tag, out var latestVer))
                    {
                        result.IsUpdateAvailable = latestVer > CurrentAssemblyVersion;
                    }
                }

                if (root.TryGetProperty("body", out var bodyElem))
                {
                    result.ReleaseNotes = bodyElem.GetString();
                }

                if (root.TryGetProperty("published_at", out var dateElem) && dateElem.TryGetDateTime(out var pubDate))
                {
                    result.ReleaseDate = pubDate;
                }

                if (root.TryGetProperty("html_url", out var urlElem))
                {
                    result.DownloadUrl = urlElem.GetString();
                }

                StructuredLogger.Info(LogCategory.General, $"Update check completed. Available: {result.IsUpdateAvailable}, Latest: {result.LatestVersion}");
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                StructuredLogger.Warning(LogCategory.General, "Update check failed (offline or unreachable)", ex.Message);
            }

            return result;
        }

        public static bool ValidateDownloadChecksum(string filePath, string expectedSha256)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(expectedSha256)) return false;
            var actualHash = CryptoHasher.ComputeFileSha256(filePath);
            return string.Equals(actualHash, expectedSha256, StringComparison.OrdinalIgnoreCase);
        }
    }
}
