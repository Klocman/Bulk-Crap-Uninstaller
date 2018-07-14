/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Management.Deployment;

namespace StoreAppHelper
{
    public static class AppManager
    {
        public static void UninstallApp(string fullName)
        {
            var packageManager = new PackageManager();

            var deploymentOperation = packageManager.RemovePackageAsync(fullName);

            var opCompletedEvent = new ManualResetEvent(false);
            deploymentOperation.Completed += (info, status) => opCompletedEvent.Set();

            Console.WriteLine($"Uninstalling \"{fullName}\"");

            opCompletedEvent.WaitOne();

            // Check the status of the operation
            switch (deploymentOperation.Status)
            {
                case AsyncStatus.Error:
                    var deploymentResult = deploymentOperation.GetResults();
                    Console.WriteLine(@"Error code: {0}", deploymentOperation.ErrorCode);
                    Console.WriteLine(@"Error text: {0}", deploymentResult.ErrorText);
                    throw new IOException(deploymentResult.ErrorText);

                case AsyncStatus.Canceled:
                    Console.WriteLine(@"Uninstallation was cancelled");
                    throw new OperationCanceledException();

                case AsyncStatus.Completed:
                    Console.WriteLine(@"Uninstallation completed successfully");
                    return;

                default:
                    Console.WriteLine(@"Invalid status: {0}", deploymentOperation.Status);
                    throw new IOException();
            }
        }

        public static IEnumerable<App> QueryApps()
        {
            var packageManager = new PackageManager();
            var userSecurityId = WindowsIdentity.GetCurrent().User?.Value;
            var packages = packageManager.FindPackagesForUserWithPackageTypes(userSecurityId, PackageTypes.Main);

            foreach (var package in packages)
            {
                if ( /*package.IsFramework || package.IsResourcePackage ||*/
                    package.Status.Disabled || package.Status.NotAvailable)
                    continue;
                
                var result = TryCreateAppFromPackage(package);
                if (result != null)
                    yield return result;
            }
        }

        private static App TryCreateAppFromPackage(Package package)
        {
            var manifestContents = TryGetAppManifest(package);
            if (manifestContents == null) return null;
            try
            {
                var start = manifestContents.IndexOf("<Properties>", StringComparison.Ordinal);
                var end = manifestContents.IndexOf("</Properties>", StringComparison.Ordinal);
                var rootXml = XElement.Parse(manifestContents.Substring(start, end - start + 13).Replace("uap:", string.Empty));
                var displayName = rootXml.Element("DisplayName")?.Value;
                var logoPath = rootXml.Element("Logo")?.Value;
                var publisherDisplayName = rootXml.Element("PublisherDisplayName")?.Value;
                var installPath = package.InstalledLocation.Path;
                var extractedDisplayName = ExtractDisplayName(installPath, package.Id.Name, displayName);

                return new App(package.Id.FullName, 
                    string.IsNullOrWhiteSpace(extractedDisplayName) ? package.Id.Name : extractedDisplayName, 
                    ExtractDisplayName(installPath, package.Id.Name, publisherDisplayName), 
                    ExtractDisplayIcon(installPath, logoPath), 
                    installPath,
                    package.SignatureKind == PackageSignatureKind.System);
            }
            catch (SystemException exception)
            {
                LogWriter.WriteExceptionToLog(exception);
                return null;
            }
        }

        private static string TryGetAppManifest(Package package)
        {
            try
            {
                var file = Path.Combine(package.InstalledLocation.Path, "AppxManifest.xml");
                if (!File.Exists(file)) return null;
                var manifestContents = File.ReadAllText(file);
                return string.IsNullOrWhiteSpace(manifestContents) ? null : manifestContents;
            }
            catch (SystemException exception)
            {
                LogWriter.WriteExceptionToLog(exception);
                return null;
            }
        }

        private static string ExtractDisplayIcon(string appDir, string iconDir)
        {
            var logo = Path.Combine(appDir, iconDir);
            if (File.Exists(logo))
                return logo;

            logo = Path.Combine(appDir, Path.ChangeExtension(logo, "scale-100.png"));
            if (File.Exists(logo))
                return logo;

            var localized = Path.Combine(Path.Combine(appDir, "en-us"), iconDir);
            localized = Path.Combine(appDir, Path.ChangeExtension(localized, "scale-100.png"));
            return File.Exists(localized) ? localized : null;
        }

        /// <summary>
        ///     Grabs display name from resources if necessary.
        /// </summary>
        /// <param name="appDir">package.InstalledLocation.Path</param>
        /// <param name="packageName">Package.Id.Name</param>
        /// <param name="displayName">Application.VisualElements.DisplayName</param>
        private static string ExtractDisplayName(string appDir, string packageName, string displayName)
        {
            Uri uri;
            if (!Uri.TryCreate(displayName, UriKind.Absolute, out uri))
                return displayName;

            var priPath = Path.Combine(appDir, "resources.pri");
            var resource = $"ms-resource://{packageName}/resources/{uri.Segments.Last()}";
            var name = NativeMethods.ExtractStringFromPriFile(priPath, resource);
            if (!string.IsNullOrEmpty(name.Trim()))
                return name;

            var res = string.Concat(uri.Segments.Skip(1));
            resource = $"ms-resource://{packageName}/{res}";
            return NativeMethods.ExtractStringFromPriFile(priPath, resource);
        }

        public sealed class App
        {
            public App(string fullName, string displayName, string publisherDisplayName, string logo,
                string installedLocation, bool isProtected)
            {
                FullName = fullName;
                DisplayName = displayName;
                PublisherDisplayName = publisherDisplayName;
                Logo = logo;
                InstalledLocation = installedLocation;
                IsProtected = isProtected;
            }

            public string FullName { get; }
            public string DisplayName { get; }
            public string PublisherDisplayName { get; }
            public string Logo { get; }
            public string InstalledLocation { get; }
            public bool IsProtected { get; }
        }

        private static class NativeMethods
        {
            [DllImport("shlwapi.dll", BestFitMapping = false, CharSet = CharSet.Unicode, ExactSpelling = true,
                SetLastError = false, ThrowOnUnmappableChar = true)]
            private static extern int SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, int cchOutBuf,
                IntPtr ppvReserved);

            internal static string ExtractStringFromPriFile(string pathToPri, string resourceKey)
            {
                var sWin8ManifestString = $"@{{{pathToPri}? {resourceKey}}}";
                var outBuff = new StringBuilder(1024);
                SHLoadIndirectString(sWin8ManifestString, outBuff, outBuff.Capacity, IntPtr.Zero);
                return outBuff.ToString();
            }
        }
    }
}