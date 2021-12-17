using System;
using GitLabApiClient;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.GitLab;

public static class Inject
{
    public static IServiceCollection AddGitLab(this IServiceCollection serrvices)
    {
        return serrvices?
            .AddSingleton<IGitLab>(new GitLab(new GitLabClient(GitLabConfig.GitLabUrl, GitLabConfig.Token)));
    }
}

public static class GitLabConfig
{
    public static string Token = Environment.GetEnvironmentVariable(EnVars.GITLAB_TOKEN) ?? string.Empty;

    public static string GitLabUrl =>
        Environment.GetEnvironmentVariable(EnVars.GITLAB_URL) ?? "https://git.macula.io";
}

public static class EnVars
{
    public const string GITLAB_TOKEN = "GITLAB_TOKEN";
    public const string GITLAB_URL = "GITLAB_URL";
}