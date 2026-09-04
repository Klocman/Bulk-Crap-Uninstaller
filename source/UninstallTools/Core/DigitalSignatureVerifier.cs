/*
    OpenUninstall Pro - Open Source Professional Windows Uninstaller
    Digital Signature and Certificate Verification Subsystem
*/

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace UninstallTools.Core
{
    public enum SignatureStatus
    {
        Unsigned,
        Valid,
        Invalid,
        Expired,
        UntrustedRoot,
        Revoked,
        Error
    }

    public sealed class DigitalSignatureInfo
    {
        public bool IsSigned { get; set; }
        public bool IsValid { get; set; }
        public SignatureStatus Status { get; set; } = SignatureStatus.Unsigned;
        public string SignerName { get; set; }
        public string IssuerName { get; set; }
        public string Subject { get; set; }
        public string SerialNumber { get; set; }
        public string Thumbprint { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string StatusMessage { get; set; }

        public override string ToString()
        {
            if (!IsSigned) return "Unsigned";
            return $"{(IsValid ? "Valid" : "Invalid")} - Signer: {SignerName ?? Subject} (Issuer: {IssuerName})";
        }
    }

    public static class DigitalSignatureVerifier
    {
        #region Native Win32 WinTrust P/Invoke
        private const string WINTRUST_ACTION_GENERIC_VERIFY_V2 = "{00AAC56B-CD44-11d0-8CC2-00C04FC295EE}";
        private const uint WTD_CHOICE_FILE = 1;
        private const uint WTD_UI_NONE = 2;
        private const uint WTD_REVOKE_WHOLECHAIN = 1;
        private const uint WTD_STATEACTION_IGNORE = 0;
        private const uint WTD_SAFER_FLAG = 0x00000100;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WINTRUST_FILE_INFO
        {
            public uint cbStruct;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pcwszFilePath;
            public IntPtr hFile;
            public IntPtr pgKnownSubject;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WINTRUST_DATA
        {
            public uint cbStruct;
            public IntPtr pPolicyCallbackData;
            public IntPtr pSIPClientData;
            public uint dwUIChoice;
            public uint fdwRevocationChecks;
            public uint dwUnionChoice;
            public IntPtr pFile;
            public uint dwStateAction;
            public IntPtr hWVTStateData;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pwszURLReference;
            public uint dwProvFlags;
            public uint dwUIContext;
        }

        [DllImport("wintrust.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int WinVerifyTrust(
            [In] IntPtr hwnd,
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid pgActionID,
            [In] IntPtr pWVTData);
        #endregion

        /// <summary>
        /// Inspects and verifies the digital signature of a PE executable or library.
        /// </summary>
        public static DigitalSignatureInfo VerifySignature(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return new DigitalSignatureInfo
                {
                    IsSigned = false,
                    IsValid = false,
                    Status = SignatureStatus.Error,
                    StatusMessage = "File not found or invalid path"
                };
            }

            var result = new DigitalSignatureInfo();

            // First attempt native WinTrust verification on Windows
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (isWindows)
            {
                VerifyWithWinTrust(filePath, result);
            }

            // Extract detailed X509 certificate details
            ExtractX509Details(filePath, result);

            return result;
        }

        private static void VerifyWithWinTrust(string filePath, DigitalSignatureInfo info)
        {
            try
            {
                var actionGuid = new Guid(WINTRUST_ACTION_GENERIC_VERIFY_V2);
                var fileInfo = new WINTRUST_FILE_INFO
                {
                    cbStruct = (uint)Marshal.SizeOf<WINTRUST_FILE_INFO>(),
                    pcwszFilePath = filePath,
                    hFile = IntPtr.Zero,
                    pgKnownSubject = IntPtr.Zero
                };

                var fileInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf<WINTRUST_FILE_INFO>());
                Marshal.StructureToPtr(fileInfo, fileInfoPtr, false);

                var trustData = new WINTRUST_DATA
                {
                    cbStruct = (uint)Marshal.SizeOf<WINTRUST_DATA>(),
                    pPolicyCallbackData = IntPtr.Zero,
                    pSIPClientData = IntPtr.Zero,
                    dwUIChoice = WTD_UI_NONE,
                    fdwRevocationChecks = WTD_REVOKE_WHOLECHAIN,
                    dwUnionChoice = WTD_CHOICE_FILE,
                    pFile = fileInfoPtr,
                    dwStateAction = WTD_STATEACTION_IGNORE,
                    hWVTStateData = IntPtr.Zero,
                    pwszURLReference = null,
                    dwProvFlags = WTD_SAFER_FLAG,
                    dwUIContext = 0
                };

                var trustDataPtr = Marshal.AllocHGlobal(Marshal.SizeOf<WINTRUST_DATA>());
                Marshal.StructureToPtr(trustData, trustDataPtr, false);

                try
                {
                    var verifyResult = WinVerifyTrust(new IntPtr(-1), actionGuid, trustDataPtr);

                    switch ((uint)verifyResult)
                    {
                        case 0:
                            info.IsSigned = true;
                            info.IsValid = true;
                            info.Status = SignatureStatus.Valid;
                            info.StatusMessage = "Signature is valid and trusted.";
                            break;

                        case 0x800B0100: // TRUST_E_NOSIGNATURE
                            info.IsSigned = false;
                            info.IsValid = false;
                            info.Status = SignatureStatus.Unsigned;
                            info.StatusMessage = "No signature found.";
                            break;

                        case 0x800B0101: // CERT_E_EXPIRED
                            info.IsSigned = true;
                            info.IsValid = false;
                            info.Status = SignatureStatus.Expired;
                            info.StatusMessage = "Certificate has expired.";
                            break;

                        case 0x800B0109: // CERT_E_UNTRUSTEDROOT
                            info.IsSigned = true;
                            info.IsValid = false;
                            info.Status = SignatureStatus.UntrustedRoot;
                            info.StatusMessage = "Untrusted root certificate.";
                            break;

                        case 0x80092010: // CRYPT_E_REVOKED
                            info.IsSigned = true;
                            info.IsValid = false;
                            info.Status = SignatureStatus.Revoked;
                            info.StatusMessage = "Certificate has been revoked.";
                            break;

                        default:
                            info.IsSigned = true;
                            info.IsValid = false;
                            info.Status = SignatureStatus.Invalid;
                            info.StatusMessage = $"Untrusted or invalid signature (HRESULT: 0x{verifyResult:X8}).";
                            break;
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(fileInfoPtr);
                    Marshal.FreeHGlobal(trustDataPtr);
                }
            }
            catch (Exception ex)
            {
                info.StatusMessage = $"WinTrust error: {ex.Message}";
            }
        }

        private static void ExtractX509Details(string filePath, DigitalSignatureInfo info)
        {
            try
            {
#pragma warning disable SYSLIB0057 // Type or member is obsolete
                using var cert = new X509Certificate2(X509Certificate.CreateFromSignedFile(filePath));
#pragma warning restore SYSLIB0057
                if (cert != null)
                {
                    info.IsSigned = true;
                    info.Subject = cert.Subject;
                    info.IssuerName = cert.Issuer;
                    info.SignerName = ExtractCommonName(cert.Subject);
                    info.SerialNumber = cert.SerialNumber;
                    info.Thumbprint = cert.Thumbprint;
                    info.ValidFrom = cert.NotBefore;
                    info.ValidTo = cert.NotAfter;

                    var now = DateTime.UtcNow;
                    if (now < cert.NotBefore || now > cert.NotAfter)
                    {
                        if (info.Status == SignatureStatus.Valid)
                        {
                            info.Status = SignatureStatus.Expired;
                            info.IsValid = false;
                            info.StatusMessage = "Certificate is outside its validity dates.";
                        }
                    }

                    if (info.Status == SignatureStatus.Unsigned)
                    {
                        info.Status = SignatureStatus.Valid;
                        info.IsValid = true;
                        info.StatusMessage = "Certificate loaded successfully.";
                    }
                }
            }
            catch
            {
                // File does not contain an embedded X509 certificate or isn't a signed PE
                if (!info.IsSigned)
                {
                    info.IsSigned = false;
                    info.IsValid = false;
                    info.Status = SignatureStatus.Unsigned;
                    info.StatusMessage = "No embedded digital certificate found.";
                }
            }
        }

        private static string ExtractCommonName(string subject)
        {
            if (string.IsNullOrWhiteSpace(subject)) return subject;
            var parts = subject.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                var trimmed = part.Trim();
                if (trimmed.StartsWith("CN=", StringComparison.OrdinalIgnoreCase))
                {
                    return trimmed.Substring(3).Trim('"', '\'');
                }
            }
            return subject;
        }
    }
}
