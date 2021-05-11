using System;
using System.Xml.Serialization;

namespace NBug.Core.Submission.Tracker.Mantis
{
    [Serializable, SoapType(Namespace = "http://futureware.biz/mantisconnect")]
    public class AttachmentData
    {
        private string content_typeField;
        private DateTime date_submittedField;
        private bool date_submittedFieldSpecified;
        private string download_urlField;
        private string filenameField;
        private string idField;
        private string sizeField;
        private string user_idField;

        [SoapElement(DataType = "integer")]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        public string filename
        {
            get { return filenameField; }
            set { filenameField = value; }
        }

        [SoapElement(DataType = "integer")]
        public string size
        {
            get { return sizeField; }
            set { sizeField = value; }
        }

        public string content_type
        {
            get { return content_typeField; }
            set { content_typeField = value; }
        }

        public DateTime date_submitted
        {
            get { return date_submittedField; }
            set { date_submittedField = value; }
        }

        [SoapIgnore]
        public bool date_submittedSpecified
        {
            get { return date_submittedFieldSpecified; }
            set { date_submittedFieldSpecified = value; }
        }

        [SoapElement(DataType = "anyURI")]
        public string download_url
        {
            get { return download_urlField; }
            set { download_urlField = value; }
        }

        [SoapElement(DataType = "integer")]
        public string user_id
        {
            get { return user_idField; }
            set { user_idField = value; }
        }
    }
}