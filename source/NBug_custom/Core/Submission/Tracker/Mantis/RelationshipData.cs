using System;
using System.Xml.Serialization;

namespace NBug.Core.Submission.Tracker.Mantis
{
    [Serializable, SoapType(Namespace = "http://futureware.biz/mantisconnect")]
    public class RelationshipData
    {
        private string idField;
        private string target_idField;
        private ObjectRef typeField;

        [SoapElement(DataType = "integer")]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        public ObjectRef type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        [SoapElement(DataType = "integer")]
        public string target_id
        {
            get { return target_idField; }
            set { target_idField = value; }
        }
    }
}