using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using NBug.Core.Reporting.Info;
using NBug.Core.Util.Logging;
using NBug.Core.Util.Serialization;

namespace NBug.Core.Submission.Tracker.Mantis
{
    public class MantisFactory : IProtocolFactory
    {
        public string SupportedType
        {
            get { return "Mantis"; }
        }

        public IProtocol FromConnectionString(string connectionString)
        {
            return new Mantis(connectionString);
        }
    }

    public class Mantis : ProtocolBase
    {
        private int _severity = 70; //default Severity is Crash
        private int _status = 10; //default Status is Open

        public Mantis(string connectionString)
            : base(connectionString)
        {
        }

        public Mantis()
        {
        }

        /// <summary>
        ///     Mandatory Field
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     Mandatory Field
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     URL to your Mantis API
        ///     IE. http://www.yourdomain.com/mantis/api/soap/mantisconnect.php
        ///     Mandatory Field
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///     ID of the project in Mantis bug tracker
        ///     Mandatory Field
        /// </summary>
        /// <remarks>Use IMantisConnectService.mc_project_get_id_from_name if you need to get ProjectId</remarks>
        public int ProjectId { get; set; }

        /// <summary>
        ///     Existing project category in Mantis bug tracker
        ///     Mandatory Field
        /// </summary>
        /// <remarks>Use IMantisConnectService.mc_project_get_categories if you need to get list of categories</remarks>
        public string Category { get; set; }

        /// <summary>
        ///     Title seen in bug overview page.
        ///     If not specified defaults to ExceptionMessage @ TargetSite
        ///     Mandatory Field
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        ///     If not specified uses Exception string
        ///     Mandatory Field
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     If not specified uses Report string
        /// </summary>
        public string AdditionalInformation { get; set; }

        /// <summary>
        ///     Information to help developers to reproduce problems
        /// </summary>
        public string StepsToReproduce { get; set; }

        /// <summary>
        ///     Default Severity is Crash (70)
        /// </summary>
        public int Severity
        {
            get { return _severity; }
            set
            {
                if (value < 10 || value > 90)
                    throw new ArgumentException("Severity");
                _severity = value;
            }
        }

        /// <summary>
        ///     Default Status is Open (10)
        /// </summary>
        public int Status
        {
            get { return _status; }
            set
            {
                if (value < 10 || value > 90)
                    throw new ArgumentException("Status");
                _status = value;
            }
        }

        /// <summary>
        ///     Should exception zip data be included with original bug in Mantis bug tracker
        /// </summary>
        public bool SendAttachment { get; set; }

        /// <summary>
        ///     Should action be treated as success if attachment upload fails
        /// </summary>
        /// <remarks>Works only if SendAttachment is true</remarks>
        public bool SuccessIfAttachmentFails { get; set; }

        /// <summary>
        ///     If current assembly version is not foud on Mantis bug tracker add it
        /// </summary>
        public bool AddVersionIfNotExists { get; set; }

        public override bool Send(string fileName, Stream file, Report report, SerializableException exception)
        {
            if (string.IsNullOrEmpty(Username) || Username.Trim().Length == 0)
                throw new ArgumentNullException("Username");

            if (string.IsNullOrEmpty(Password) || Password.Trim().Length == 0)
                throw new ArgumentNullException("Password");

            if (string.IsNullOrEmpty(Url) || Url.Trim().Length == 0)
                throw new ArgumentNullException("Url");

            if (ProjectId <= 0)
                throw new ArgumentNullException("ProjectId");

            if (string.IsNullOrEmpty(Category) || Category.Trim().Length == 0)
                throw new ArgumentNullException("Category");

            if (string.IsNullOrEmpty(Description) || Description.Trim().Length == 0)
            {
                Description = exception.ToString();
            }

            if (string.IsNullOrEmpty(Summary) || Summary.Trim().Length == 0)
            {
                Summary = report.GeneralInfo.ExceptionMessage + " @ " + report.GeneralInfo.TargetSite;
            }

            if (string.IsNullOrEmpty(AdditionalInformation) || AdditionalInformation.Trim().Length == 0)
            {
                AdditionalInformation = report.ToString();
            }

            var service = new MantisConnectService(new BasicHttpBinding(), new EndpointAddress(Url));
            var user = service.mc_login(Username, Password);
            Logger.Info(string.Format("Successfully logged in to Mantis bug tracker as {0}", user.account_data.real_name));
            var issue = new IssueData();
            issue.date_submitted = DateTime.Now;
            issue.date_submittedSpecified = true;
            issue.version = report.GeneralInfo.HostApplicationVersion;
            issue.os = Environment.OSVersion.ToString();
            issue.os_build = Environment.OSVersion.Version.ToString();
            issue.platform = Environment.OSVersion.Platform.ToString();
            issue.reporter = user.account_data;
            issue.project = new ObjectRef {id = ProjectId.ToString(CultureInfo.InvariantCulture), name = string.Empty};
            issue.category = Category;
            issue.summary = Summary;
            issue.description = Description;
            issue.additional_information = AdditionalInformation;
            issue.steps_to_reproduce = StepsToReproduce;
            issue.severity = new ObjectRef
            {
                id = Severity.ToString(CultureInfo.InvariantCulture),
                name = Severity.ToString(CultureInfo.InvariantCulture)
            };
            issue.status = new ObjectRef
            {
                id = Status.ToString(CultureInfo.InvariantCulture),
                name = Status.ToString(CultureInfo.InvariantCulture)
            };
            var success = false;

            if (AddVersionIfNotExists)
            {
                var versions = service.mc_project_get_versions(Username, Password,
                    ProjectId.ToString(CultureInfo.InvariantCulture));
                if (versions.Count() == 0 ||
                    !versions.Any(
                        x => x.name == report.GeneralInfo.HostApplicationVersion.ToString(CultureInfo.InvariantCulture)))
                {
                    var version = new ProjectVersionData
                    {
                        name = report.GeneralInfo.HostApplicationVersion.ToString(CultureInfo.InvariantCulture),
                        project_id = ProjectId.ToString(CultureInfo.InvariantCulture),
                        released = true,
                        releasedSpecified = true,
                        date_order = DateTime.Now,
                        date_orderSpecified = true,
                        description = "Added by NBug"
                    };
                    var versionId = service.mc_project_version_add(Username, Password, version);
                    Logger.Info(string.Format("Successfully added new version id {0} to Mantis bug tracker", versionId));
                }
            }

            int bugId;
            int.TryParse(service.mc_issue_add(Username, Password, issue), out bugId);

            if (bugId > 0)
            {
                Logger.Info(string.Format("Successfully added new issue id {0} to Mantis bug tracker", bugId));
                success = true;
                if (SendAttachment)
                {
                    if (file != null && file.Length > 0)
                    {
                        if (!SuccessIfAttachmentFails)
                            success = false;
                        try
                        {
                            var attachment = new byte[file.Length];
                            file.Position = 0;
                            file.Read(attachment, 0, Convert.ToInt32(file.Length));
                            var attachmentId = service.mc_issue_attachment_add(Username, Password,
                                bugId.ToString(CultureInfo.InvariantCulture), fileName, "application/zip", attachment);
                            Logger.Info(
                                string.Format(
                                    "Successfully added attachment id {0} for isssue id {1} to Mantis bug tracker",
                                    attachmentId, bugId));
                            success = true;
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(
                                string.Format(
                                    "Failed to upload attachment with issue id {0} to Mantis bug tracker{1}{2}", bugId,
                                    Environment.NewLine, ex.Message));
                            if (!SuccessIfAttachmentFails)
                                throw;
                        }
                    }
                }
            }

            return success;
        }
    }
}