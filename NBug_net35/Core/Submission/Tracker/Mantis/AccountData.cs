using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace NBug.Core.Submission.Tracker.Mantis
{
    [Serializable, DebuggerStepThrough, SoapType(Namespace = "http://futureware.biz/mantisconnect")]
    public class AccountData
    {
        private string emailField;
        private string idField;
        private string nameField;
        private string real_nameField;

        [SoapElement(DataType = "integer")]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        public string real_name
        {
            get { return real_nameField; }
            set { real_nameField = value; }
        }

        public string email
        {
            get { return emailField; }
            set { emailField = value; }
        }
    }
}