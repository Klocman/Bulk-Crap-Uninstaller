/*
    EBUninstaller Pro - Open Source Professional Windows Uninstaller
    Cryptographic Hashing Subsystem
*/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace UninstallTools.Core
{
    public static class CryptoHasher
    {
        private const int BufferSize = 64 * 1024; // 64 KB

        /// <summary>
        /// Computes SHA-256 hash of a file using streamed reading.
        /// </summary>
        public static string ComputeSha256(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));
            if (!File.Exists(filePath)) throw new FileNotFoundException("Target file not found for hashing", filePath);

            using var sha256 = SHA256.Create();
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, BufferSize);
            var hashBytes = sha256.ComputeHash(stream);
            return ToHexString(hashBytes);
        }

        /// <summary>
        /// Computes SHA-256 hash of byte array.
        /// </summary>
        public static string ComputeSha256(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(data);
            return ToHexString(hashBytes);
        }

        /// <summary>
        /// Computes SHA-256 hash of string content with UTF-8 encoding.
        /// </summary>
        public static string ComputeSha256(string content, Encoding encoding = null)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            encoding ??= Encoding.UTF8;
            return ComputeSha256(encoding.GetBytes(content));
        }

        /// <summary>
        /// Computes SHA-1 hash of a file for legacy compatibility checks.
        /// </summary>
        public static string ComputeSha1(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));
            if (!File.Exists(filePath)) throw new FileNotFoundException("Target file not found for hashing", filePath);

            using var sha1 = SHA1.Create();
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, BufferSize);
            var hashBytes = sha1.ComputeHash(stream);
            return ToHexString(hashBytes);
        }

        /// <summary>
        /// Constant-time verification between computed file hash and expected hash.
        /// </summary>
        public static bool VerifyFileHash(string filePath, string expectedHash)
        {
            if (string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(expectedHash))
                return false;

            try
            {
                var actualHash = ComputeSha256(filePath);
                return FixedTimeEquals(actualHash.ToLowerInvariant(), expectedHash.Trim().ToLowerInvariant());
            }
            catch
            {
                return false;
            }
        }

        private static string ToHexString(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        private static bool FixedTimeEquals(string a, string b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            var result = 0;
            for (var i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }
            return result == 0;
        }
    }
}
