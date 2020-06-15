namespace CoMutator.Messages
{
    public class StartWorkingMessage
    {
        public string Url { get; }
        public string? CurrentRepoBranch { get; }
        public string? CurrentRepoCommitHash { get; }
        public string File { get; }


        public StartWorkingMessage(string url, string? currentRepoBranch, string? currentRepoCommitHash, string file)
        {
            Url = url;
            CurrentRepoBranch = currentRepoBranch;
            CurrentRepoCommitHash = currentRepoCommitHash;
            File = file;
        }
    }
}