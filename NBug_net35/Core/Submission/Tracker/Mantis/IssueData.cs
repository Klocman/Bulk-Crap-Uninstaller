using System;
using System.Xml.Serialization;

namespace NBug.Core.Submission.Tracker.Mantis
{
    [Serializable, SoapType(Namespace = "http://futureware.biz/mantisconnect")]
    public class IssueData
    {
        private string additional_informationField;
        private AttachmentData[] attachmentsField;
        private string buildField;
        private string categoryField;
        private CustomFieldValueForIssueData[] custom_fieldsField;
        private DateTime date_submittedField;
        private bool date_submittedFieldSpecified;
        private string descriptionField;
        private DateTime due_dateField;
        private bool due_dateFieldSpecified;
        private ObjectRef etaField;
        private string fixed_in_versionField;
        private AccountData handlerField;
        private string idField;
        private DateTime last_updatedField;
        private bool last_updatedFieldSpecified;
        private AccountData[] monitorsField;
        private IssueNoteData[] notesField;
        private string os_buildField;
        private string osField;
        private string platformField;
        private ObjectRef priorityField;
        private ObjectRef projectField;
        private ObjectRef projectionField;
        private RelationshipData[] relationshipsField;
        private AccountData reporterField;
        private ObjectRef reproducibilityField;
        private ObjectRef resolutionField;
        private ObjectRef severityField;
        private string sponsorship_totalField;
        private ObjectRef statusField;
        private string steps_to_reproduceField;
        private bool stickyField;
        private bool stickyFieldSpecified;
        private string summaryField;
        private ObjectRef[] tagsField;
        private string target_versionField;
        private string versionField;
        private ObjectRef view_stateField;

        [SoapElement(DataType = "integer")]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        public ObjectRef view_state
        {
            get { return view_stateField; }
            set { view_stateField = value; }
        }

        public DateTime last_updated
        {
            get { return last_updatedField; }
            set { last_updatedField = value; }
        }

        [SoapIgnore]
        public bool last_updatedSpecified
        {
            get { return last_updatedFieldSpecified; }
            set { last_updatedFieldSpecified = value; }
        }

        public ObjectRef project
        {
            get { return projectField; }
            set { projectField = value; }
        }

        public string category
        {
            get { return categoryField; }
            set { categoryField = value; }
        }

        public ObjectRef priority
        {
            get { return priorityField; }
            set { priorityField = value; }
        }

        public ObjectRef severity
        {
            get { return severityField; }
            set { severityField = value; }
        }

        public ObjectRef status
        {
            get { return statusField; }
            set { statusField = value; }
        }

        public AccountData reporter
        {
            get { return reporterField; }
            set { reporterField = value; }
        }

        public string summary
        {
            get { return summaryField; }
            set { summaryField = value; }
        }

        public string version
        {
            get { return versionField; }
            set { versionField = value; }
        }

        public string build
        {
            get { return buildField; }
            set { buildField = value; }
        }

        public string platform
        {
            get { return platformField; }
            set { platformField = value; }
        }

        public string os
        {
            get { return osField; }
            set { osField = value; }
        }

        public string os_build
        {
            get { return os_buildField; }
            set { os_buildField = value; }
        }

        public ObjectRef reproducibility
        {
            get { return reproducibilityField; }
            set { reproducibilityField = value; }
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

        [SoapElement(DataType = "integer")]
        public string sponsorship_total
        {
            get { return sponsorship_totalField; }
            set { sponsorship_totalField = value; }
        }

        public AccountData handler
        {
            get { return handlerField; }
            set { handlerField = value; }
        }

        public ObjectRef projection
        {
            get { return projectionField; }
            set { projectionField = value; }
        }

        public ObjectRef eta
        {
            get { return etaField; }
            set { etaField = value; }
        }

        public ObjectRef resolution
        {
            get { return resolutionField; }
            set { resolutionField = value; }
        }

        public string fixed_in_version
        {
            get { return fixed_in_versionField; }
            set { fixed_in_versionField = value; }
        }

        public string target_version
        {
            get { return target_versionField; }
            set { target_versionField = value; }
        }

        public string description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        public string steps_to_reproduce
        {
            get { return steps_to_reproduceField; }
            set { steps_to_reproduceField = value; }
        }

        public string additional_information
        {
            get { return additional_informationField; }
            set { additional_informationField = value; }
        }

        public AttachmentData[] attachments
        {
            get { return attachmentsField; }
            set { attachmentsField = value; }
        }

        public RelationshipData[] relationships
        {
            get { return relationshipsField; }
            set { relationshipsField = value; }
        }

        public IssueNoteData[] notes
        {
            get { return notesField; }
            set { notesField = value; }
        }

        public CustomFieldValueForIssueData[] custom_fields
        {
            get { return custom_fieldsField; }
            set { custom_fieldsField = value; }
        }

        public DateTime due_date
        {
            get { return due_dateField; }
            set { due_dateField = value; }
        }

        [SoapIgnore]
        public bool due_dateSpecified
        {
            get { return due_dateFieldSpecified; }
            set { due_dateFieldSpecified = value; }
        }

        public AccountData[] monitors
        {
            get { return monitorsField; }
            set { monitorsField = value; }
        }

        public bool sticky
        {
            get { return stickyField; }
            set { stickyField = value; }
        }

        [SoapIgnore]
        public bool stickySpecified
        {
            get { return stickyFieldSpecified; }
            set { stickyFieldSpecified = value; }
        }

        public ObjectRef[] tags
        {
            get { return tagsField; }
            set { tagsField = value; }
        }
    }
}