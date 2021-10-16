using LibGit2Sharp;

namespace M5x.Git
{
    internal class GitRepoBuilder : IGitRepoBuilder
    {
        public IGitRepo Build(string localPath)
        {
            var res = new Repository(localPath);
            return new GitRepo(res);
        }
    }

    public interface IGitRepoBuilder
    {
        IGitRepo Build(string localPath);
    }
}