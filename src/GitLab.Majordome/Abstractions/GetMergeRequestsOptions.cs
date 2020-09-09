namespace GitLab.Majordome.Abstractions
{
    public class GetMergeRequestsOptions
    {
        public string? AuthorUsername { get; set; }
        public string? NotUpvotedBy { get; set; }
        public string? NotAuthoredBy { get; set; }
        public bool? OnlyOpened { get; set; }
        public bool OnlyNotInProgress { get; set; }
        public string[]? ExcludingProjectIds { get; set; }
    }
}