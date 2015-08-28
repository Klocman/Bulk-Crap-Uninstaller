using System.ServiceModel;
using System.Xml.Serialization;

namespace NBug.Core.Submission.Tracker.Mantis
{
    [ServiceContract(Namespace = "http://futureware.biz/mantisconnect", ConfigurationName = "MantisConnectPortType")]
    public interface IMantisConnectService
    {
        [OperationContract(Action = "*", ReplyAction = "*"),
         XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        [return: MessageParameter(Name = "return")]
        string mc_version();

        [OperationContract(Action = "*", ReplyAction = "*"),
         XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        [return: MessageParameter(Name = "return")]
        UserData mc_login(string username, string password);

        [OperationContract(Action = "*", ReplyAction = "*"),
         XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        [return: MessageParameter(Name = "return"), SoapElement(DataType = "integer")]
        string mc_issue_add(string username, string password, IssueData issue);

        [OperationContract(Action = "*", ReplyAction = "*"),
         XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        [return: MessageParameter(Name = "return"), SoapElement(DataType = "integer")]
        string mc_project_get_id_from_name(string username, string password, string project_name);

        [OperationContract(Action = "*", ReplyAction = "*"),
         XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        [return: MessageParameter(Name = "return")]
        string[] mc_project_get_categories(
            string username,
            string password,
            [SoapElement(DataType = "integer")] string project_id
            );

        [OperationContract(Action = "*", ReplyAction = "*"),
         XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        [return: MessageParameter(Name = "return"), SoapElement(DataType = "integer")]
        string mc_issue_attachment_add(
            string username,
            string password,
            [SoapElement(DataType = "integer")] string issue_id,
            string name,
            string file_type,
            [SoapElement(DataType = "base64Binary")] byte[] content
            );

        [OperationContract(Action = "*", ReplyAction = "*"),
         XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        [return: MessageParameter(Name = "return")]
        ProjectVersionData[] mc_project_get_versions(
            string username,
            string password,
            [SoapElement(DataType = "integer")] string project_id);

        [OperationContract(Action = "*", ReplyAction = "*"),
         XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        [return: MessageParameter(Name = "return"), SoapElement(DataType = "integer")]
        string mc_project_version_add(string username, string password, ProjectVersionData version);
    }
}