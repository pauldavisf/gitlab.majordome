namespace GitLab.Majordome.Abstractions
{
    public class GetMergeRequestsOptionsBuilder
    {
        private GetMergeRequestsOptions options;

        public GetMergeRequestsOptionsBuilder()
        {
            options = new GetMergeRequestsOptions();
        }

        public GetMergeRequestsOptionsBuilder WithAuthor(string username)
        {
            options.AuthorUsername = username;
            return this;
        }

        public GetMergeRequestsOptionsBuilder NotUpvotedBy(string username)
        {
            options.NotUpvotedBy = username;
            return this;
        }

        public GetMergeRequestsOptionsBuilder NotAuthoredBy(string username)
        {
            options.NotAuthoredBy = username;
            return this;
        }

        public GetMergeRequestsOptionsBuilder OnlyOpened()
        {
            options.OnlyOpened = true;
            return this;
        }

        public GetMergeRequestsOptionsBuilder OnlyNotWorkInProgress()
        {
            options.OnlyNotInProgress = true;
            return this;
        }

        public GetMergeRequestsOptionsBuilder ExcludingProjects(string[] projectIds)
        {
            options.ExcludingProjectIds = projectIds;
            return this;
        }

        public GetMergeRequestsOptions Build()
        {
            return options;
        }
    }
}