using System.Collections.Generic;
using System.Threading.Tasks;

namespace GitLab.Majordome.Abstractions
{
    public interface IMergeRequestsProvider
    {
        Task<IReadOnlyList<MergeRequestInfo>> GetOpenedMergeRequestAsync(int projectGroupId,
            GetMergeRequestsOptions? options = null);
    }
}