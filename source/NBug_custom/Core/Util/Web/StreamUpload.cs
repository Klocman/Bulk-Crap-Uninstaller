// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamUpload.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// <remarks>
//   Originates from: http://www.andrescottwilson.com/posting-content-from-memory-stream-using-httpwebrequest-c/
//   Changed for NBug by Michal Turecki
// </remarks>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace NBug.Core.Util.Web
{
    public class StreamUpload
    {
        private readonly string boundary = string.Empty;
        private readonly MemoryStream outputStream = new();
        private ICredentials credentials;
        private WebResponse response;

        public StreamUpload()
        {
            boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);
        }

        public static StreamUpload Create()
        {
            return new StreamUpload();
        }

        public StreamUpload Add(string name, string value)
        {
            WriteBoundaryToStream(outputStream, Environment.NewLine);
            WriteToStream(
                outputStream, Encoding.UTF8,
                string.Format("Content-Disposition: form-data; name=\"{0}\"{1}{1}", name, Environment.NewLine));
            WriteToStream(outputStream, Encoding.UTF8, value + Environment.NewLine);
            return this;
        }

        public StreamUpload Add(Stream inputStream)
        {
            return Add(inputStream, "form", "file", "application/octet-stream");
        }

        public StreamUpload Add(Stream inputStream, string formName, string fileName, string contentType)
        {
            // write content boundary start
            WriteBoundaryToStream(outputStream, Environment.NewLine);

            WriteToStream(
                outputStream,
                Encoding.UTF8,
                string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}", formName, fileName,
                    Environment.NewLine));
            WriteToStream(outputStream, Encoding.UTF8,
                string.Format("Content-Type: {0}{1}{1}", contentType, Environment.NewLine));

            var buffer = new byte[inputStream.Length];
            var bytesRead = 0;

            while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                outputStream.Write(buffer, 0, bytesRead);
            }

            // must include a new line before writing the end boundary
            WriteToStream(outputStream, Encoding.ASCII, Environment.NewLine);

            // make sure we end boundary now as the content is finished
            WriteEndBoundaryToStream(outputStream);

            return this;
        }

        public StreamUpload AddNameValues(NameValueCollection nameValues)
        {
            foreach (string name in nameValues.Keys)
            {
                Add(name, nameValues[name]);
            }

            return this;
        }

        public StreamUpload Clear()
        {
            outputStream.Close();
            response = null;
            return this;
        }

        public string Response()
        {
            return (response == null)
                ? string.Empty
                : new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException("GetResponseStream null"), Encoding.UTF8).ReadToEnd();
        }

        public StreamUpload Upload(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
            request.Method = "POST";

            if (credentials != null)
            {
                var user = credentials.GetCredential(request.RequestUri, "Basic");
                var auth = string.Format("{0}:{1}", user.UserName, user.Password);
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(auth)));
            }

            WriteToHttpStream(request, outputStream);
            response = request.GetResponse();
            return this;
        }

        public StreamUpload WithCredentials(ICredentials credentials)
        {
            this.credentials = credentials;
            return this;
        }

        private void WriteBoundaryToStream(MemoryStream stream, string endDeliminator)
        {
            WriteToStream(stream, Encoding.ASCII, string.Format("--{0}{1}", boundary, endDeliminator));
        }

        private void WriteEndBoundaryToStream(MemoryStream stream)
        {
            WriteBoundaryToStream(stream, "--");
        }

        private void WriteNameValuesToStream(MemoryStream stream, NameValueCollection nameValues)
        {
            foreach (string name in nameValues.Keys)
            {
                WriteBoundaryToStream(stream, Environment.NewLine);

                WriteToStream(stream, Encoding.UTF8,
                    string.Format("Content-Disposition: form-data; name=\"{0}\"{1}{1}", name, Environment.NewLine));
                WriteToStream(stream, Encoding.UTF8, nameValues[name] + Environment.NewLine);
            }
        }

        private void WriteToHttpStream(HttpWebRequest request, MemoryStream outputStream)
        {
            request.ContentLength = outputStream.Length;

            using (var requestStream = request.GetRequestStream())
            {
                outputStream.Position = 0;

                var tempBuffer = new byte[outputStream.Length];
                outputStream.Read(tempBuffer, 0, tempBuffer.Length);
                outputStream.Close();

                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                requestStream.Close();
            }
        }

        private void WriteToStream(MemoryStream stream, Encoding encoding, string output)
        {
            var headerbytes = encoding.GetBytes(output);
            stream.Write(headerbytes, 0, headerbytes.Length);
        }
    }
}