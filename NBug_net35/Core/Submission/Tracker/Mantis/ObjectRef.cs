using System;
using System.Xml.Serialization;

namespace NBug.Core.Submission.Tracker.Mantis
{
    [Serializable, SoapType(Namespace = "http://futureware.biz/mantisconnect")]
    public class ObjectRef
    {
        private string idField;
        private string nameField;

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
    }
}