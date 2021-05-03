using System.Net;
using System.Net.Mail;
using System.Text;

namespace NBug.Helpers
{
    public class EmailDestinationBuilder
    {
        private readonly MailAddress[] _destinationAddresses;
        private readonly MailAddress _fromAddress;
        private readonly string _serverName;
        private MailAddress[] _blindCarbonCopyAddresses;
        private MailAddress[] _carbonCopyAddresses;
        private string _customBody;
        private string _fromName;
        private MailPriority? _mailPriority;
        private string _replyTo;
        private bool _sendAttachments;
        private int _smtpPort = 465;
        private string _smtpUserName;
        private string _smtpUserPassword;
        private string _subject;
        private bool _useSsl = true;

        public EmailDestinationBuilder(MailAddress fromAddress, MailAddress[] destinationAddresses, string serverName)
        {
            _serverName = serverName;
            _fromAddress = fromAddress;
            _destinationAddresses = destinationAddresses;

            _sendAttachments = false;
        }

        public EmailDestinationBuilder Bcc(MailAddress[] blindCarbonCopyAddresses)
        {
            _blindCarbonCopyAddresses = blindCarbonCopyAddresses;

            return this;
        }

        public EmailDestinationBuilder Body(string body)
        {
            _customBody = body;

            return this;
        }

        /* Type=Mail;
     * From=my_tracker@gmail.com;
     * FromName=NBug Error Reporter;
     * To=bugtracker@mycompany.com,someone@dummy.com,my_tracker@gmail.com;
     * Cc=;
     * Bcc=;
     * ReplyTo=;
     * UseAttachment=false;
     * CustomSubject=;
     * CustomBody=;
     * SmtpServer=smtp.gmail.com;
     * UseSSL=yes;
     * Port=465;
     * Priority=;
     * UseAuthentication=yes;
     * Username=my_tracker@gmail.com;
     * Password=mypassword;
     */

        public string Build()
        {
            var sb = new StringBuilder();
            sb.Append("Type=Mail;");
            sb.AppendFormat("From={0};", _fromAddress.Address);

            if (!string.IsNullOrEmpty(_fromName))
            {
                sb.AppendFormat("FromName={0};", _fromName);
            }

            sb.Append("To=");
            foreach (var address in _destinationAddresses)
            {
                sb.AppendFormat("{0},", address.Address);
            }

            sb.Length--;
            sb.Append(";");

            if (_carbonCopyAddresses != null && _carbonCopyAddresses.Length > 0)
            {
                sb.Append("Cc=");
                foreach (var address in _carbonCopyAddresses)
                {
                    sb.AppendFormat("{0},", address.Address);
                }

                sb.Length--;
                sb.Append(";");
            }

            if (_blindCarbonCopyAddresses != null && _blindCarbonCopyAddresses.Length > 0)
            {
                sb.Append("Bcc=");
                foreach (var address in _blindCarbonCopyAddresses)
                {
                    sb.AppendFormat("{0},", address.Address);
                }

                sb.Length--;
                sb.Append(";");
            }

            if (!string.IsNullOrEmpty(_replyTo))
            {
                sb.AppendFormat("ReplyTo={0};", _replyTo);
            }

            sb.AppendFormat("UseAttachment={0};", _sendAttachments ? "true" : "false");

            if (!string.IsNullOrEmpty(_subject))
            {
                sb.AppendFormat("CustomSubject={0};", _subject);
            }

            if (!string.IsNullOrEmpty(_customBody))
            {
                sb.AppendFormat("CustomBody={0};", _customBody);
            }

            sb.AppendFormat("SmtpServer={0};", _serverName);

            sb.Append("UseSSL=");
            sb.Append(_useSsl ? "yes" : "no");
            sb.Append(";");

            sb.AppendFormat("Port={0};", _smtpPort);

            if (_mailPriority.HasValue)
            {
                sb.AppendFormat("Priority={0};", _mailPriority);
            }

            if (string.IsNullOrEmpty(_smtpUserName))
            {
                sb.Append("UseAuthentication=no;");
            }
            else
            {
                sb.Append("UseAuthentication=yes;");
                sb.AppendFormat("Username={0};", _smtpUserName);
                sb.AppendFormat("Password={0};", _smtpUserPassword);
            }

            return sb.ToString();
        }

        public EmailDestinationBuilder Cc(MailAddress[] carbonCopyAddresses)
        {
            _carbonCopyAddresses = carbonCopyAddresses;
            return this;
        }

        public EmailDestinationBuilder FromName(string fromName)
        {
            _fromName = fromName;

            return this;
        }

        public EmailDestinationBuilder Priority(MailPriority mailPriority)
        {
            _mailPriority = mailPriority;

            return this;
        }

        public EmailDestinationBuilder ReplyTo(string replyTo)
        {
            _replyTo = replyTo;

            return this;
        }

        public EmailDestinationBuilder SendAttachments()
        {
            _sendAttachments = true;

            return this;
        }

        public EmailDestinationBuilder Subject(string subject)
        {
            _subject = subject;

            return this;
        }

        public EmailDestinationBuilder UseServer(bool useSecureSocketLayer, int port, NetworkCredential credential)
        {
            _useSsl = useSecureSocketLayer;
            _smtpPort = port;
            if (credential != null)
            {
                _smtpUserName = credential.UserName;
                _smtpUserPassword = credential.Password;
            }

            return this;
        }

        public EmailDestinationBuilder UseServer(bool useSecureSocketLayer, NetworkCredential credential)
        {
            return UseServer(useSecureSocketLayer, useSecureSocketLayer ? 465 : 25, credential);
        }
    }
}