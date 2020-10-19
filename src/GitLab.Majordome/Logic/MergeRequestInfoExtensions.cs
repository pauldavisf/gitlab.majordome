using System.Linq;
using GitLab.Majordome.Abstractions;

namespace GitLab.Majordome.Logic
{
    internal static class MergeRequestInfoExtensions
    {
        public static bool MatchesOptions(this MergeRequestInfo mergeRequest, GetMergeRequestsOptions mergeRequestsOptions)
        {
            if (mergeRequestsOptions.ExcludingProjectIds != null)
            {
                if (mergeRequestsOptions.ExcludingProjectIds.Contains(mergeRequest.ProjectId))
                {
                    return false;
                }
            }

            if (mergeRequestsOptions.AuthorUsername != null)
            {
                if (mergeRequest.AuthorUsername != mergeRequestsOptions.AuthorUsername)
                {
                    return false;
                }
            }

            if (mergeRequestsOptions.NotAuthoredBy != null)
            {
                if (mergeRequest.AuthorUsername == mergeRequestsOptions.NotAuthoredBy)
                {
                    return false;
                }
            }

            if (mergeRequestsOptions.OnlyNotInProgress)
            {
                if (mergeRequest.IsWorkInProgress)
                {
                    return false;
                }
            }

            if (mergeRequestsOptions.OnlyOpened == true && !mergeRequest.IsOpened)
            {
                return false;
            }

            if (mergeRequestsOptions.NotUpvotedBy != null)
            {
                if (mergeRequest.UpvotedBy.Contains(mergeRequestsOptions.NotUpvotedBy))
                {
                    return false;
                }
            }

            return true;
        }
    }
}