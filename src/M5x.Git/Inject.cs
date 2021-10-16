using Microsoft.Extensions.DependencyInjection;

namespace M5x.Git
{
    public static class Inject
    {
        public static IServiceCollection AddGitRepo(this IServiceCollection services)
        {
            return services?
                .AddSingleton<IGitRepoBuilder, GitRepoBuilder>();
        }
    }
}