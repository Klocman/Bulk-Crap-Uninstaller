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

            try
            {
                X509Certificate2 result = null;
                if (entry.SortedExecutables != null)
                    result = TryExtractCertificateHelper(entry.SortedExecutables);

                // Check executables before this because signatures in MSI store are modified and won't verify
                if (result == null && entry.UninstallerKind == UninstallerType.Msiexec)
                    result = MsiTools.GetCertificate(entry.BundleProviderKey);

                // If no certs were found finally check the uninstaller
                if (result == null && !string.IsNullOrEmpty(entry.UninstallerFullFilename))
                    result = TryExtractCertificateHelper(entry.UninstallerFullFilename);
                return result;
            }
            catch
            {
                // Default to no certificate
                return null;
            }
        }

        /// <summary>
        ///     Check first few files from the install directory for certificates
        /// </summary>
        private static X509Certificate2 TryExtractCertificateHelper(params string[] fileNames)
        {
            foreach (var candidate in fileNames.Take(2))
            {
                try
                {
                    return new X509Certificate2(candidate);
                }
                catch
                {
                    // No cert was found, try next
                }
            }

            return null;
        }
    }
}