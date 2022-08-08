/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading;
using BulkCrapUninstaller.Properties;
using Klocman.Forms;
using Klocman.Tools;

namespace BulkCrapUninstaller.Functions.Tools
{
    internal static class UpdateGrabber
    {
        /// <summary>
        ///     Look for updates while displaying a progress bar. At the end display a message box with the result.
        /// </summary>
        public static void LookForUpdates()
        {
            bool? result = null;
            Version latestVersion = null;
            var error = LoadingDialog.ShowDialog(null, Localisable.LoadingDialogTitleSearchingForUpdates,
                _ => { result = IsUpdateAvailable(Assembly.GetExecutingAssembly().GetName().Version, out latestVersion); });

            if (error == null)
            {
                switch (result)
                {
                    case null:
                        MessageBoxes.UpdateFailed("Unknown error");
                        break;

                    case true:
                        AskAndBeginUpdate(latestVersion);
                        break;

                    case false:
                        MessageBoxes.UpdateUptodate();
                        break;
                }
            }
            else
            {
                MessageBoxes.UpdateFailed(error.Message);
            }
        }

        public static void AskAndBeginUpdate(Version latestVersion)
        {
            if (MessageBoxes.UpdateAskToDownload(latestVersion))
            {
                try
                {
                    // Prevent log cleaner from running in portable builds
                    //EntryPoint.IsRestarting = true;

                    Process.Start(new ProcessStartInfo(LatestReleaseUrl) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    EntryPoint.IsRestarting = false;

                    Console.WriteLine(ex);
                    MessageBoxes.UpdateFailed(ex.Message);
                }
            }
        }

        /// <summary>
        ///     Setup update system and automatically search for them if auto update is enabled.
        ///     Doesn't block, use delegates to interface.
        /// </summary>
        public static void Setup()
        {
            //UpdateSystem.UpdateFeedUri = Program.EnableDebug ? DebugUpdateFeedUri : UpdateFeedUri;
            //UpdateSystem.CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version;
        }

        /// <summary>
        ///     Automatically search for updates if auto update is enabled.
        ///     Doesn't block, use delegates to interface.
        /// </summary>
        /// <param name="canDisplayMessage">updateFoundCallback will be called after this returns true</param>
        /// <param name="updateFoundCallback">Launched only if a new update was found. It's launched from a background thread.</param>
        public static void AutoUpdate(Func<bool> canDisplayMessage, Action<Version> updateFoundCallback)
        {
            if (Settings.Default.MiscCheckForUpdates && WindowsTools.IsNetworkAvailable())
            {
                new Thread(() =>
                {
                    if (IsUpdateAvailable(Assembly.GetExecutingAssembly().GetName().Version, out var updateVersion) == true)
                    {
                        while (!canDisplayMessage())
                            Thread.Sleep(100);

                        try
                        {
                            updateFoundCallback(updateVersion);
                        }
                        catch
                        {
                            // Ignore background error, not necessary
                        }
                    }
                })
                { Name = "UpdateCheck_Thread", IsBackground = true }.Start();
            }
        }
        
        public static string LatestReleaseUrl = "https://github.com/Klocman/Bulk-Crap-Uninstaller/releases/latest";

        public static Version CheckLatestVersion()
        {
            // Should result in something like "https://github.com/Klocman/Bulk-Crap-Uninstaller/releases/tag/v4.1"
            var url = GetFinalRedirect(LatestReleaseUrl);
            if (url != null)
            {
                var i = url.LastIndexOf('/');
                var tag = url.Substring(i).TrimStart('/', 'v');
                return new Version(tag);
            }

            return null;
        }

        /// <summary>
        /// Returns null if failed to look for updates, else returns if there is a newer version available
        /// </summary>
        public static bool? IsUpdateAvailable(Version currentVersion, out Version updateVersion)
        {
            updateVersion = null;
            try
            {
                var latestVersion = CheckLatestVersion();
                if (latestVersion == null)
                {
                    throw new WebException("Failed to get version from URL");
                }
                else if (latestVersion > currentVersion)
                {
                    updateVersion = latestVersion;
                    Console.WriteLine("A new version is available: " + latestVersion);
                    return true;
                }
                else
                {
                    Console.WriteLine("The current version is the latest");
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to check for new versions: " + e.Message);
                return null;
            }
        }

        // https://stackoverflow.com/a/28424940
        private static string GetFinalRedirect(string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            if (ServicePointManager.SecurityProtocol < SecurityProtocolType.Tls12)
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            int maxRedirCount = 8; // prevent infinite loops
            string newUrl = url;
            do
            {
                HttpWebResponse resp = null;
                try
                {
                    var req = WebRequest.CreateHttp(new Uri(url));
                    req.Method = "HEAD";
                    req.AllowAutoRedirect = false;
                    resp = (HttpWebResponse)req.GetResponse();
                    switch (resp.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            return newUrl;
                        case HttpStatusCode.Redirect:
                        case HttpStatusCode.MovedPermanently:
                        case HttpStatusCode.RedirectKeepVerb:
                        case HttpStatusCode.RedirectMethod:
                            newUrl = resp.Headers["Location"];
                            if (newUrl == null)
                                return url;

                            if (!newUrl.Contains("://"))
                            {
                                // Doesn't have a URL Schema, meaning it's a relative or absolute URL
                                Uri u = new Uri(new Uri(url), newUrl);
                                newUrl = u.ToString();
                            }

                            break;
                        default:
                            return newUrl;
                    }
                    url = newUrl;
                }
                catch (WebException)
                {
                    // Return the last known good URL
                    return newUrl;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
                finally
                {
                    if (resp != null)
                        resp.Close();
                }
            } while (maxRedirCount-- > 0);

            return newUrl;
        }
    }
}