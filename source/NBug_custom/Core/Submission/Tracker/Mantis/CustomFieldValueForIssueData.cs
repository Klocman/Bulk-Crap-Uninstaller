using System;
using System.Xml.Serialization;

namespace NBug.Core.Submission.Tracker.Mantis
{
    [Serializable, SoapType(Namespace = "http://futureware.biz/mantisconnect")]
    public class CustomFieldValueForIssueData
    {
        private ObjectRef fieldField;
        private string valueField;

        public ObjectRef field
        {
            get { return fieldField; }
            set { fieldField = value; }
        }

        public string value
        {
            get { return valueField; }
            set { valueField = value; }
        }
    }
}