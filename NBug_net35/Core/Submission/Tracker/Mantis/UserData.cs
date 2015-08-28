using System;
using System.Xml.Serialization;

namespace NBug.Core.Submission.Tracker.Mantis
{
    [Serializable, SoapType(Namespace = "http://futureware.biz/mantisconnect")]
    public class UserData
    {
        private string access_levelField;
        private AccountData account_dataField;
        private string timezoneField;

        public AccountData account_data
        {
            get { return account_dataField; }
            set { account_dataField = value; }
        }

        [SoapElement(DataType = "integer")]
        public string access_level
        {
            get { return access_levelField; }
            set { access_levelField = value; }
        }

        public string timezone
        {
            get { return timezoneField; }
            set { timezoneField = value; }
        }
    }
}