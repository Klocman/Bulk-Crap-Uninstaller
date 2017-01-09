using System.Net;
using System.Net.Mail;

namespace BulkCrapUninstaller.Functions.Tracking
{
    public sealed class MailSender
    {
        public MailSender()
        {
            Port = 25;
            Priority = MailPriority.Normal;
        }

        public string Bcc { get; set; }
        public string Cc { get; set; }
        //public string CustomBody { get; set; }
        //public string CustomSubject { get; set; }
        public string From { get; set; }
        public string FromName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public MailPriority Priority { get; set; }
        public string ReplyTo { get; set; }
        public string SmtpServer { get; set; }
        public string To { get; set; }
        //public bool UseAttachment { get; set; }
        public bool UseAuthentication { get; set; }
        public string Username { get; set; }
        public bool UseSsl { get; set; }

        public bool Send(string subject, string body)
        {
            if (string.IsNullOrEmpty(From) || string.IsNullOrEmpty(To))
            {
                return false;
            }

            if (string.IsNullOrEmpty(ReplyTo))
            {
                ReplyTo = From;
            }

            if (Port <= 0)
            {
                Port = UseSsl ? 465 : 25;
            }
            /*
            if (!this.UseAttachment)
            {
                this.UseAttachment = false;
            }*/

            // Make sure that we can use authentication even with emtpy username and password
            if (!string.IsNullOrEmpty(Username))
            {
                UseAuthentication = true;
            }

            //using (var smtpClient = new SmtpClient())
            using (var message = new MailMessage())
            {
                var smtpClient = new SmtpClient();
                if (!string.IsNullOrEmpty(SmtpServer))
                {
                    smtpClient.Host = SmtpServer;
                }

                smtpClient.Port = Port;

                if (UseAuthentication)
                {
                    smtpClient.Credentials = new NetworkCredential(Username, Password);
                }

                smtpClient.EnableSsl = UseSsl;

                if (!string.IsNullOrEmpty(Cc))
                {
                    message.CC.Add(Cc);
                }

                if (!string.IsNullOrEmpty(Bcc))
                {
                    message.Bcc.Add(Bcc);
                }

                message.Priority = Priority;

                message.To.Add(To);
                message.ReplyTo = new MailAddress(ReplyTo);
                message.From = !string.IsNullOrEmpty(FromName) ? new MailAddress(From, FromName) : new MailAddress(From);
                /*
                if (this.UseAttachment)
                {
                    // ToDo: Report file name should be attached to the report file object itself, file shouldn't be accessed directly!
                    file.Position = 0;
                    message.Attachments.Add(new Attachment(file, fileName));
                }*/

                if (!string.IsNullOrEmpty(subject))
                {
                    message.Subject = subject;
                }
                else
                {
                    return false;
                    //message.Subject = "NBug: " + report.GeneralInfo.HostApplication + " (" + report.GeneralInfo.HostApplicationVersion + "): "
                    //                  + report.GeneralInfo.ExceptionType + " @ " + report.GeneralInfo.TargetSite;
                }

                if (!string.IsNullOrEmpty(body))
                {
                    message.Body = body;
                }
                else
                {
                    return false;
                    //message.Body = report + Environment.NewLine + Environment.NewLine + exception;
                }

                smtpClient.Send(message);

                return true;
            }
        }
    }
}