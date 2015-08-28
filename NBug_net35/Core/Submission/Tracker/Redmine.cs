// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Redmine.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;
using NBug.Core.Reporting.Info;
using NBug.Core.Util.Logging;
using NBug.Core.Util.Serialization;

namespace NBug.Core.Submission.Tracker
{
    public class RedmineFactory : IProtocolFactory
    {
        public string SupportedType
        {
            get { return "Redmine"; }
        }

        public IProtocol FromConnectionString(string connectionString)
        {
            return new Redmine(connectionString);
        }
    }

    public class Redmine : ProtocolBase
    {
        public Redmine(string connectionString)
            : base(connectionString)
        {
        }

        public Redmine()
        {
        }

        public string ApiKey { get; set; }
        public string AssignedToId { get; set; }
        public string AuthorId { get; set; }
        // Connection string format (single line)
        // Warning: There should be no semicolon (;) or equals sign (=) used in any field except for password
        // Note: Url should be a full url with a trailing slash (/), like: http://....../

        /* Type=Redmine;
		 * Url=http://tracker.mydomain.com/;
		 * ProjectId=myproject;
		 * TrackerId=;
		 * PriorityId=;
		 * CategoryId=;
		 * CustomSubject=;
		 * FixedVersionId=;
		 * AssignedToId=;
		 * ParentId=;
		 * StatusId=;
		 * AuthorId=;
		 * ApiKey=myapikey;
		 */
        public string CategoryId { get; set; }
        public string CustomSubject { get; set; }
        public string FixedVersionId { get; set; }
        public string ParentId { get; set; }
        public string PriorityId { get; set; }
        public string ProjectId { get; set; }
        public string StatusId { get; set; }
        public string TrackerId { get; set; }
        public string Url { get; set; }

        public override bool Send(string fileName, Stream file, Report report, SerializableException exception)
        {
            HttpWebRequest request;

            if (string.IsNullOrEmpty(ApiKey))
            {
                request = (HttpWebRequest) WebRequest.Create(new Uri(Url + "issues.xml"));
            }
            else
            {
                request = (HttpWebRequest) WebRequest.Create(new Uri(Url + "issues.xml?key=" + ApiKey));
            }

            // Used POST as per http://www.redmine.org/projects/redmine/wiki/Rest_Issues#Creating-an-issue
            request.Method = "POST";
            request.ContentType = "application/xml";
            request.Accept = "application/xml";
            request.ServicePoint.Expect100Continue = false;
                // Patch #11593. Some servers seem to have problem accepting the Expect: 100-continue header

            // Redmine v1.1.1 REST XML templates (* indicate required fields)
            // Note: <*_id> fields always ask for numeric values

            /* <?xml version="1.0"?>
			 * <issue>
			 *   *<subject>Example</subject>
			 *   *<project_id>myproject</project_id>
			 *   <tracker_id></tracker_id> <!-- Bug=1/Issue=2/etc. -->
			 *   <priority_id></priority_id> <!-- Normal=4 -->
			 *   <category_id></category_id>
			 *   <description></description>
			 *   <fixed_version_id></fixed_version_id>
			 *   <assigned_to_id></assigned_to_id>
			 *   <parent_id></parent_id>
			 *   <status_id></status_id>
			 *   <author_id></author_id>
			 *   <start_date></start_date>
			 *   <due_date></due_date>
			 *   <done_ratio></done_ratio>
			 *   <estimated_hours></estimated_hours>
			 *   <created_on></created_on>
			 *   <updated_on></updated_on>
			 * </issue>
			 */
            var subject = "NBug: " + report.GeneralInfo.HostApplication + " (" +
                          report.GeneralInfo.HostApplicationVersion + "): "
                          + report.GeneralInfo.ExceptionType + " @ " + report.GeneralInfo.TargetSite;

            var description = "<pre>" + report + Environment.NewLine + Environment.NewLine + exception + "</pre>";

            var redmineRequestXml = new XElement("issue", new XElement("project_id", ProjectId));

            if (!string.IsNullOrEmpty(TrackerId))
            {
                redmineRequestXml.Add(new XElement("tracker_id", TrackerId));
            }

            if (!string.IsNullOrEmpty(PriorityId))
            {
                redmineRequestXml.Add(new XElement("priority_id", PriorityId));
            }

            if (!string.IsNullOrEmpty(CategoryId))
            {
                redmineRequestXml.Add(new XElement("category_id", CategoryId));
            }

            // Add the subject and make sure that it is less than 255 characters or Redmine trows an HTTP error code 422 : Unprocessable Entity
            if (!string.IsNullOrEmpty(CustomSubject))
            {
                var sbj = CustomSubject + " : " + subject;
                redmineRequestXml.Add(sbj.Length > 254
                    ? new XElement("subject", sbj.Substring(0, 254))
                    : new XElement("subject", sbj));
            }
            else
            {
                redmineRequestXml.Add(subject.Length > 254
                    ? new XElement("subject", subject.Substring(0, 254))
                    : new XElement("subject", subject));
            }

            if (!string.IsNullOrEmpty(description))
            {
                redmineRequestXml.Add(new XElement("description", description));
            }

            if (!string.IsNullOrEmpty(FixedVersionId))
            {
                redmineRequestXml.Add(new XElement("fixed_version_id", FixedVersionId));
            }

            if (!string.IsNullOrEmpty(AssignedToId))
            {
                redmineRequestXml.Add(new XElement("assigned_to_id", AssignedToId));
            }

            if (!string.IsNullOrEmpty(ParentId))
            {
                redmineRequestXml.Add(new XElement("parent_id", ParentId));
            }

            if (!string.IsNullOrEmpty(StatusId))
            {
                redmineRequestXml.Add(new XElement("status_id", StatusId));
            }

            if (!string.IsNullOrEmpty(AuthorId))
            {
                redmineRequestXml.Add(new XElement("author_id", AuthorId));
            }

            var bytes = Encoding.UTF8.GetBytes(redmineRequestXml.ToString());

            request.ContentLength = bytes.Length;

            using (var putStream = request.GetRequestStream())
            {
                putStream.Write(bytes, 0, bytes.Length);
            }

            // Log the response from Redmine RESTful service
            using (var response = (HttpWebResponse) request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                Logger.Info("Response from Redmine Issue Tracker: " + reader.ReadToEnd());
            }

            return true;
        }
    }
}