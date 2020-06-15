namespace CoMutator.Common.Settings
{
    public class PublicGitRepoSettings : IRepoSettings
    {
        public string Url { get; set; }
        public string? Branch { get; set; }
        public string? CommitHash { get; set; }
    }
}