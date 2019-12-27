namespace CrossroadsDemo.ViewModels
{
    /// <summary>
    /// Git Commits View Model
    /// </summary>
    public class GitCommitsViewModel
    {
        public string Committer { get; set; }
        public string CommittedAt { get; set; }
        public string Message { get; set; }
        public string HtmlUrl { get; set; }
    }
}
