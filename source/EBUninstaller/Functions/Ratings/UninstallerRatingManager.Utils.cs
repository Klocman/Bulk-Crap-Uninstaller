using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace BulkCrapUninstaller.Functions.Ratings
{
    public partial class UninstallerRatingManager
    {
        internal static class Utils
        {
            public static T DecompressAndDeserialize<T>(byte[] bytes, JsonSerializerOptions jsonSerializerOptions = null)
            {
                using (var msInput = new MemoryStream(bytes))
                using (var bs = new BrotliStream(msInput, CompressionMode.Decompress))
                using (var msOutput = new MemoryStream())
                {
                    bs.CopyTo(msOutput);
                    msOutput.Seek(0, SeekOrigin.Begin);
                    var output = msOutput.ToArray();
                    return JsonSerializer.Deserialize<T>(output, jsonSerializerOptions);
                }
            }

            public static byte[] SerializeAndCompress(object objToJsonserialize)
            {
                var inputStr = JsonSerializer.Serialize(objToJsonserialize);

                var bytes = Encoding.UTF8.GetBytes(inputStr);

                using (var outputStream = new MemoryStream())
                {
                    using (var gZipStream = new BrotliStream(outputStream, CompressionLevel.Optimal))
                        gZipStream.Write(bytes, 0, bytes.Length);

                    var result = outputStream.ToArray();
                    //Debug.WriteLine($"Compression result: {bytes.Length} -> {result.Length} ({(result.Length / (double)bytes.Length) * 100:F1}%)");
                    return result;
                }
            }


            [ThreadStatic] private static System.Security.Cryptography.MD5 _md5;
            private static readonly ConcurrentDictionary<string, ulong> _hashCache = new();

            public static ulong StableHash(string str)
            {
                if (str == null) return 0;
                if (_hashCache.TryGetValue(str, out var hash))
                {
                    return hash;
                }
                else
                {
                    if (_md5 == null) _md5 = System.Security.Cryptography.MD5.Create();
                    byte[] inputBytes = Encoding.UTF8.GetBytes(str);
                    var hashBytes = _md5.ComputeHash(inputBytes);
                    var stableHash = BitConverter.ToUInt64(hashBytes, 0) ^ BitConverter.ToUInt64(hashBytes, 8);
                    _hashCache.TryAdd(str, stableHash);
                    return stableHash;
                }
            }

            // Needed for deserializing incoming data from the server
            public class AverageRatingEntry
            {
                public int AverageRating { get; set; }
                public ulong AppId { get; set; }
            }
            // Needed for deserializing incoming data from the server
            public class UserRatingEntry
            {
                public ulong UserId { get; set; }
                public int Rating { get; set; }
                public ulong AppId { get; set; }
            }
        }
    }
}
