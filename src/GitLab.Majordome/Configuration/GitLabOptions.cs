using System;

namespace GitLab.Majordome.Configuration
{
    public class GitLabOptions
    {
        public int ProjectGroupId { get; set; } = default!;
        public string[] ExcludingProjects { get; set; } = Array.Empty<string>();
    }
}