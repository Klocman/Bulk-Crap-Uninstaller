/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.IO;
using System.IO.Compression;
using System.Text;

namespace Klocman.Tools
{
    public static class CompressionTools
    {
        public static byte[] ZipString(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    //msi.CopyTo(gs);
                    msi.CopyTo(gs);
                }

                return mso.ToArray();
            }
        }

        public static string UnzipString(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    //gs.CopyTo(mso);
                    gs.CopyTo(mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        public static byte[] BrotliDecompress(byte[] compressedData)
        {
            using (var msInput = new MemoryStream(compressedData))
            using (var bs = new BrotliStream(msInput, CompressionMode.Decompress))
            using (var msOutput = new MemoryStream())
            {
                bs.CopyTo(msOutput);
                msOutput.Seek(0, SeekOrigin.Begin);
                return msOutput.ToArray();
            }
        }

        public static byte[] BrotliCompress(string inputstr)
        {
            var bytes = Encoding.UTF8.GetBytes(inputstr);
            return BrotliCompress(bytes);
        }
        public static byte[] BrotliCompress(byte[] data)
        {
            using (var outputStream = new MemoryStream())
            {
                using (var gZipStream = new BrotliStream(outputStream, CompressionLevel.Optimal))
                    gZipStream.Write(data, 0, data.Length);

                var result = outputStream.ToArray();
                //Debug.WriteLine($"Compression result: {bytes.Length} -> {result.Length} ({(result.Length / (double)bytes.Length) * 100:F1}%)");
                return result;
            }
        }
    }
}