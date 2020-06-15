namespace CoMutator.Messages
{
    public class CloneThisRepoMessage
    {
        public string Url { get; }
        public string? Branch { get; }
        public string? CommitHash { get; }
        public CloneThisRepoMessage(string url, string? branch = null, string? commitHash = null)
        {
            Url = url;
            Branch = branch;
            CommitHash = commitHash;
        }
    }
}