/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Klocman.IO;

namespace UninstallTools.Factory.InfoAdders
{
    public static class CertificateGetter
    {
        internal static X509Certificate2 TryGetCertificate(ApplicationUninstallerEntry entry)
        {
            // Don't even try if the entry is invalid, it will be marked as bad anyways
            if (!entry.IsValid)
                return null;

            X509Certificate2 result = null;
            try
            {
                if (entry.UninstallerKind == UninstallerType.Msiexec)
                {
                    if (entry.IsInstallLocationValid())
                        result = TryExtractCertificateHelper(entry.GetMainExecutableCandidates());

                    // If no certs were found check the MSI store
                    if (result == null)
                        result = MsiTools.GetCertificate(entry.BundleProviderKey);
                }
                else
                {
                    // If no certs were found check the uninstaller
                    result = TryExtractCertificateHelper(entry.GetMainExecutableCandidates());
                    if (result == null && !string.IsNullOrEmpty(entry.UninstallerFullFilename))
                        result = new X509Certificate2(entry.UninstallerFullFilename);
                }
            }
            catch
            {
                // Default to no certificate
                return null;
            }
            return result;
        }

        /// <summary>
        ///     Check first few files from the install directory for certificates
        /// </summary>
        private static X509Certificate2 TryExtractCertificateHelper(string[] fileNames)
        {
            if (fileNames != null)
            {
                foreach (var candidate in fileNames.Take(2))
                {
                    try
                    {
                        return new X509Certificate2(candidate);
                    }
                    catch
                    {
                        // No cert was found
                    }
                }
            }

            return null;
        }
    }
}