using System.ServiceModel;
using System.ServiceModel.Channels;

namespace NBug.Core.Submission.Tracker.Mantis
{
    public class MantisConnectService : ClientBase<IMantisConnectService>, IMantisConnectService
    {
        public MantisConnectService(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
        {
        }

        public string mc_version()
        {
            return Channel.mc_version();
        }

        public UserData mc_login(string username, string password)
        {
            return Channel.mc_login(username, password);
        }

        public string mc_issue_add(string username, string password, IssueData issue)
        {
            return Channel.mc_issue_add(username, password, issue);
        }

        public string mc_project_get_id_from_name(string username, string password, string project_name)
        {
            return Channel.mc_project_get_id_from_name(username, password, project_name);
        }

        public string[] mc_project_get_categories(string username, string password, string project_id)
        {
            return Channel.mc_project_get_categories(username, password, project_id);
        }

        public string mc_issue_attachment_add(string username, string password, string issue_id, string name,
            string file_type, byte[] content)
        {
            return Channel.mc_issue_attachment_add(username, password, issue_id, name, file_type, content);
        }

        public ProjectVersionData[] mc_project_get_versions(string username, string password, string project_id)
        {
            return Channel.mc_project_get_versions(username, password, project_id);
        }

        public string mc_project_version_add(string username, string password, ProjectVersionData version)
        {
            return Channel.mc_project_version_add(username, password, version);
        }
    }
}