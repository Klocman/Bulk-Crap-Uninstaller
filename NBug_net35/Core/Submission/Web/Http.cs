// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Http.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;
using NBug.Core.Reporting.Info;
using NBug.Core.Util.Logging;
using NBug.Core.Util.Serialization;
using NBug.Core.Util.Web;

namespace NBug.Core.Submission.Web
{
    public class HttpFactory : IProtocolFactory
    {
        public string SupportedType
        {
            get { return "Http"; }
        }

        public IProtocol FromConnectionString(string connectionString)
        {
            return new Http(connectionString);
        }
    }

    public class Http : ProtocolBase
    {
        public Http(string connectionString)
            : base(connectionString)
        {
        }

        public Http()
        {
        }

        // Connection string format (single line)
        // Warning: There should be no semicolon (;) or equals sign (=) used in any field.
        // Note: Url should be a full url with a trailing slash (/) or file extension (i.e. .php), like: http://....../ -or- http://....../upload.php

        /* Type=Http;
		 * Url=http://tracker.mydomain.com/myproject/upload.php;
		 */
        public string Url { get; set; }

        public override bool Send(string fileName, Stream file, Report report, SerializableException exception)
        {
            // Advanced method with ability to post variables along with file (do not forget to urlencode the query parameters)
            // http://www.codeproject.com/KB/cs/uploadfileex.aspx
            // http://stackoverflow.com/questions/566462/upload-files-with-httpwebrequest-multipart-form-data
            // http://stackoverflow.com/questions/767790/how-do-i-upload-an-image-file-using-a-post-request-in-c
            // http://netomatix.com/HttpPostData.aspx

            /* upload.php file my look like the one below (note that uploaded files are not statically named in this case script may need modification)
			 *
			 * <?php
			 * $uploadDir = 'Upload/';
			 * $uploadFile = $uploadDir . basename($_FILES['file']['name']);
			 * if (is_uploaded_file($_FILES['file']['tmp_name']))
			 * {
			 *     echo "File ". $_FILES['file']['name'] ." is successfully uploaded!\r\n";
			 *     if (move_uploaded_file($_FILES['file']['tmp_name'], $uploadFile))
			 *     {
			 *         echo "File is successfully stored! ";
			 *     }
			 *     else print_r($_FILES);
			 * }
			 * else
			 * {
			 *     echo "Upload Failed!";
			 *     print_r($_FILES);
			 * }
			 * ?>
			 */
            file.Position = 0;

            var response = StreamUpload.Create().Add(file, "file", fileName, "application/zip").Upload(Url).Response();

            Logger.Info("Response from HTTP server: " + response);
            file.Position = 0;

            return true;
        }
    }
}