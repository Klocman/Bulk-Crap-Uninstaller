using System;
using System.Xml.Serialization;

namespace NBug.Core.Submission.Tracker.Mantis
{
    [Serializable,
     SoapType(Namespace = "http://futureware.biz/mantisconnect")]
    public class ProjectVersionData
    {
        private DateTime date_orderField;
        private bool date_orderFieldSpecified;
        private string descriptionField;
        private string idField;
        private string nameField;
        private bool obsoleteField;
        private bool obsoleteFieldSpecified;
        private string project_idField;
        private bool releasedField;
        private bool releasedFieldSpecified;

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

        [SoapElement(DataType = "integer")]
        public string project_id
        {
            get { return project_idField; }
            set { project_idField = value; }
        }

        public DateTime date_order
        {
            get { return date_orderField; }
            set { date_orderField = value; }
        }

        [SoapIgnore]
        public bool date_orderSpecified
        {
            get { return date_orderFieldSpecified; }
            set { date_orderFieldSpecified = value; }
        }

        public string description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        public bool released
        {
            get { return releasedField; }
            set { releasedField = value; }
        }

        [SoapIgnore]
        public bool releasedSpecified
        {
            get { return releasedFieldSpecified; }
            set { releasedFieldSpecified = value; }
        }

        public bool obsolete
        {
            get { return obsoleteField; }
            set { obsoleteField = value; }
        }

        [SoapIgnore]
        public bool obsoleteSpecified
        {
            get { return obsoleteFieldSpecified; }
            set { obsoleteFieldSpecified = value; }
        }
    }
}