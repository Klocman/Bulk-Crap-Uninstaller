/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;

namespace Klocman.Extensions
{
    public static class WebExtensions
    {
        /// <summary>
        /// http://stackoverflow.com/a/30049848/4309247
        /// </summary>
        public static Encoding GetEncodingFrom(NameValueCollection responseHeaders, Encoding defaultEncoding = null)
        {
            if (responseHeaders == null)
                throw new ArgumentNullException("responseHeaders");

            //Note that key lookup is case-insensitive
            var contentType = responseHeaders["Content-Type"];
            if (contentType == null)
                return defaultEncoding;

            var contentTypeParts = contentType.Split(';');
            if (contentTypeParts.Length <= 1)
                return defaultEncoding;

            var charsetPart =
                contentTypeParts.Skip(1).FirstOrDefault(
                    p => p.TrimStart().StartsWith("charset", StringComparison.InvariantCultureIgnoreCase));
            if (charsetPart == null)
                return defaultEncoding;

            var charsetPartParts = charsetPart.Split('=');
            if (charsetPartParts.Length != 2)
                return defaultEncoding;

            var charsetName = charsetPartParts[1].Trim();
            if (charsetName == "")
                return defaultEncoding;

            try
            {
                return Encoding.GetEncoding(charsetName);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException("The server returned data in an unknown encoding: " + charsetName, ex);
            }
        }

        public static string DownloadStringAwareOfEncoding(this WebClient webClient, Uri uri)
        {
            var rawData = webClient.DownloadData(uri);
            var encoding = GetEncodingFrom(webClient.ResponseHeaders, Encoding.UTF8);
            return encoding.GetString(rawData);
        }
    }
}
