using System;
using System.Xml.Serialization;

namespace NBug.Core.Submission.Tracker.Mantis
{
    [Serializable, SoapType(Namespace = "http://futureware.biz/mantisconnect")]
    public class IssueNoteData
    {
        private DateTime date_submittedField;
        private bool date_submittedFieldSpecified;
        private string idField;
        private DateTime last_modifiedField;
        private bool last_modifiedFieldSpecified;
        private string note_attrField;
        private string note_typeField;
        private AccountData reporterField;
        private string textField;
        private string time_trackingField;
        private ObjectRef view_stateField;

        [SoapElement(DataType = "integer")]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        public AccountData reporter
        {
            get { return reporterField; }
            set { reporterField = value; }
        }

        public string text
        {
            get { return textField; }
            set { textField = value; }
        }

        public ObjectRef view_state
        {
            get { return view_stateField; }
            set { view_stateField = value; }
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

        public DateTime last_modified
        {
            get { return last_modifiedField; }
            set { last_modifiedField = value; }
        }

        [SoapIgnore]
        public bool last_modifiedSpecified
        {
            get { return last_modifiedFieldSpecified; }
            set { last_modifiedFieldSpecified = value; }
        }

        [SoapElement(DataType = "integer")]
        public string time_tracking
        {
            get { return time_trackingField; }
            set { time_trackingField = value; }
        }

        [SoapElement(DataType = "integer")]
        public string note_type
        {
            get { return note_typeField; }
            set { note_typeField = value; }
        }

        public string note_attr
        {
            get { return note_attrField; }
            set { note_attrField = value; }
        }
    }
}