namespace CoMutator.Leader
{
    public class TestThisRepoMessage
    {
        public string Url { get; }
        public string? Branch { get; }
        public string? CommitHash { get; }
        public TestThisRepoMessage(string url, string? branch = null, string? commitHash = null)
        {
            Url = url;
            Branch = branch;
            CommitHash = commitHash;
        }
    }
}
