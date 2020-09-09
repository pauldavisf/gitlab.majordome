using System.Collections.Generic;
using System.Linq;
using GitLab.Majordome.Abstractions;
using GitLabApiClient.Models.AwardEmojis.Responses;
using GitLabApiClient.Models.MergeRequests.Responses;

namespace GitLab.Majordome.Logic
{
    internal static class MergeRequestMapper
    {
        public static MergeRequestInfo ToMergeRequestInfo(this MergeRequest mergeRequest, IList<AwardEmoji> awardEmojis)
        {
            return new MergeRequestInfo
            {
                Id = mergeRequest.Id,
                Title = mergeRequest.Title,
                AuthorUsername = mergeRequest.Author.Username,
                UpvotedBy = awardEmojis.Where(x => x.Name == "thumbsup").Select(x => x.User.Username).ToList(),
                WebUrl = mergeRequest.WebUrl,
                IsOpened = mergeRequest.State == MergeRequestState.Opened,
                IsWorkInProgress = mergeRequest.WorkInProgress
            };
        }
    }
}