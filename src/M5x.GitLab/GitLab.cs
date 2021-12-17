using System.Threading.Tasks;
using GitLabApiClient;
using GitLabApiClient.Models.Oauth.Responses;

namespace M5x.GitLab;

public class GitLab : IGitLab
{
    private readonly IGitLabClient _client;

    public GitLab(IGitLabClient client)
    {
        _client = client;
    }

    public async Task<AccessTokenResponse> LoginAsync(string username, string password, string scope = "api")
    {
        return await _client.LoginAsync(username, password, scope);
    }

    public IIssuesClient Issues => _client.Issues;

    public IUploadsClient Uploads => _client.Uploads;

    public IMergeRequestsClient MergeRequests => _client.MergeRequests;

    public IProjectsClient Projects => _client.Projects;

    public IUsersClient Users => _client.Users;

    public IGroupsClient Groups => _client.Groups;

    public IBranchClient Branches => _client.Branches;

    public IReleaseClient Releases => _client.Releases;

    public ITagClient Tags => _client.Tags;

    public IWebhookClient Webhooks => _client.Webhooks;

    public ICommitsClient Commits => _client.Commits;

    public ITreesClient Trees => _client.Trees;

    public IFilesClient Files => _client.Files;

    public IMarkdownClient Markdown => _client.Markdown;

    public IPipelineClient Pipelines => _client.Pipelines;

    public IRunnersClient Runners => _client.Runners;

    public IToDoListClient ToDoList => _client.ToDoList;

    public string HostUrl => _client.HostUrl;
}

public interface IGitLab : IGitLabClient
{
}