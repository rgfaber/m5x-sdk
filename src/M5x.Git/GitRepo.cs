using System.Collections.Generic;
using LibGit2Sharp;

namespace M5x.Git
{
    internal class GitRepo : IGitRepo
    {
        private readonly IRepository _repo;

        public GitRepo(IRepository repo)
        {
            _repo = repo;
        }

        public void Dispose()
        {
            _repo.Dispose();
        }

        public void Checkout(Tree tree, IEnumerable<string> paths, CheckoutOptions opts)
        {
            _repo.Checkout(tree, paths, opts);
        }

        public void CheckoutPaths(string committishOrBranchSpec, IEnumerable<string> paths,
            CheckoutOptions checkoutOptions)
        {
            _repo.CheckoutPaths(committishOrBranchSpec, paths, checkoutOptions);
        }

        public GitObject Lookup(ObjectId id)
        {
            return _repo.Lookup(id);
        }

        public GitObject Lookup(string objectish)
        {
            return _repo.Lookup(objectish);
        }

        public GitObject Lookup(ObjectId id, ObjectType type)
        {
            return _repo.Lookup(id, type);
        }

        public GitObject Lookup(string objectish, ObjectType type)
        {
            return _repo.Lookup(objectish, type);
        }

        public Commit Commit(string message, Signature author, Signature committer, CommitOptions options)
        {
            return _repo.Commit(message, author, committer, options);
        }

        public void Reset(ResetMode resetMode, Commit commit)
        {
            _repo.Reset(resetMode, commit);
        }

        public void Reset(ResetMode resetMode, Commit commit, CheckoutOptions options)
        {
            _repo.Reset(resetMode, commit, options);
        }

        public void RemoveUntrackedFiles()
        {
            _repo.RemoveUntrackedFiles();
        }

        public RevertResult Revert(Commit commit, Signature reverter, RevertOptions options)
        {
            return _repo.Revert(commit, reverter, options);
        }

        public MergeResult Merge(Commit commit, Signature merger, MergeOptions options)
        {
            return _repo.Merge(commit, merger, options);
        }

        public MergeResult Merge(Branch branch, Signature merger, MergeOptions options)
        {
            return _repo.Merge(branch, merger, options);
        }

        public MergeResult Merge(string committish, Signature merger, MergeOptions options)
        {
            return _repo.Merge(committish, merger, options);
        }

        public MergeResult MergeFetchedRefs(Signature merger, MergeOptions options)
        {
            return _repo.MergeFetchedRefs(merger, options);
        }

        public CherryPickResult CherryPick(Commit commit, Signature committer, CherryPickOptions options)
        {
            return _repo.CherryPick(commit, committer, options);
        }

        public BlameHunkCollection Blame(string path, BlameOptions options)
        {
            return _repo.Blame(path, options);
        }

        public FileStatus RetrieveStatus(string filePath)
        {
            return _repo.RetrieveStatus(filePath);
        }

        public RepositoryStatus RetrieveStatus(StatusOptions options)
        {
            return _repo.RetrieveStatus(options);
        }

        public string Describe(Commit commit, DescribeOptions options)
        {
            return _repo.Describe(commit, options);
        }

        public void RevParse(string revision, out Reference reference, out GitObject obj)
        {
            _repo.RevParse(revision, out reference, out obj);
        }

        public Branch Head => _repo.Head;

        public Configuration Config => _repo.Config;

        public Index Index => _repo.Index;

        public ReferenceCollection Refs => _repo.Refs;

        public IQueryableCommitLog Commits => _repo.Commits;

        public BranchCollection Branches => _repo.Branches;

        public TagCollection Tags => _repo.Tags;

        public RepositoryInformation Info => _repo.Info;

        public Diff Diff => _repo.Diff;

        public ObjectDatabase ObjectDatabase => _repo.ObjectDatabase;

        public NoteCollection Notes => _repo.Notes;

        public SubmoduleCollection Submodules => _repo.Submodules;

        public WorktreeCollection Worktrees => _repo.Worktrees;

        public Rebase Rebase => _repo.Rebase;

        public Ignore Ignore => _repo.Ignore;

        public Network Network => _repo.Network;

        public StashCollection Stashes => _repo.Stashes;
    }


    public interface IGitRepo : IRepository
    {
    }
}