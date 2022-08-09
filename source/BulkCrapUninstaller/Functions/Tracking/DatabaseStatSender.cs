/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using Klocman.Tools;

namespace BulkCrapUninstaller.Functions.Tracking
{
    public class DatabaseStatSender
    {
        private readonly ulong userId;

        public DatabaseStatSender(ulong userId)
        {
            this.userId = userId;
        }

        public bool SendData(string value)
        {
            try
            {
                var compressed = CompressionTools.BrotliCompress(value);

                using var s = Program.GetHttpClient();
                var response = s.PostAsync(new Uri($"SendStats?userId={userId}&data={Convert.ToBase64String(compressed)}", UriKind.Relative), null!).Result;
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to send stats: " + e);
                return false;
            }
            return true;
        }
    }
}