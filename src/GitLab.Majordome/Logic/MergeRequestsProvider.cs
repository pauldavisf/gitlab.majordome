using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitLab.Majordome.Abstractions;
using GitLabApiClient;
using GitLabApiClient.Models.AwardEmojis.Responses;
using GitLabApiClient.Models.MergeRequests.Responses;
using Newtonsoft.Json;

namespace GitLab.Majordome.Logic
{
    public class MergeRequestsProvider : IMergeRequestsProvider
    {
        private readonly IGitLabClient gitLabClient;

        public MergeRequestsProvider(IGitLabClient gitLabClient)
        {
            this.gitLabClient = gitLabClient;
        }

        public async Task<IList<MergeRequestInfo>> GetOpenedMergeRequestAsync(
            int projectGroupId,
            GetMergeRequestsOptions? options = null)
        {
            options ??= new GetMergeRequestsOptions();

            var projectGroup = await gitLabClient.Groups.GetAsync(projectGroupId);

            var getMergeRequestsTasks = projectGroup.Projects.Select(async project =>
                    await GetMergeRequests(project.Id, options));

            var allMergeRequests = await Task.WhenAll(getMergeRequestsTasks);
            return allMergeRequests.SelectMany(x => x).ToList();
        }

        private async Task<IList<MergeRequestInfo>> GetMergeRequests(int projectId, GetMergeRequestsOptions options)
        {
            var result = new List<MergeRequestInfo>();

            IList<MergeRequest> mergeRequests = new List<MergeRequest>();
            try
            {
                mergeRequests = await gitLabClient.MergeRequests.GetAsync(projectId);
            }
            catch (JsonSerializationException)
            {
                return result;
            }

            foreach (var mergeRequest in mergeRequests)
            {
                var emojis = await GetAwardEmojisAsync(projectId, mergeRequest.Iid);
                var mergeRequestInfo = mergeRequest.ToMergeRequestInfo(emojis);

                if (mergeRequestInfo.MatchesOptions(options))
                {
                    result.Add(mergeRequestInfo);
                }
            }

            return result;
        }

        private async Task<IList<AwardEmoji>> GetAwardEmojisAsync(int projectId, int mergeRequestId)
        {
            IList<AwardEmoji> result = new List<AwardEmoji>();
            try
            {
                result = await gitLabClient.MergeRequests.GetAwardEmojisAsync(projectId, mergeRequestId);
            }
            catch (GitLabException)
            {
                Console.WriteLine();
            }

            return result;
        }
    }
}