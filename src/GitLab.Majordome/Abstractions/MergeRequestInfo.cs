using System.Collections.Generic;

namespace GitLab.Majordome.Abstractions
{
    public class MergeRequestInfo
    {
        public int Id { get; set; }
        public string ProjectId { get; set; } = default!;
        public string Title { get; set; } = default!;
        public bool IsOpened { get; set; }
        public bool IsWorkInProgress { get; set; }
        public string WebUrl { get; set; } = default!;
        public string AuthorUsername { get; set; } = default!;
        public IList<string> UpvotedBy { get; set; } = new List<string>();
    }
}