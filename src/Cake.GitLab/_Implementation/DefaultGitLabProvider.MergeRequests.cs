using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using NGitLab;
using NGitLab.Models;

namespace Cake.GitLab;

public partial class DefaultGitLabProvider
{
    /// <inheritdoc />
    public Task<IReadOnlyCollection<MergeRequest>> GetMergeRequestsAsync(string serverUrl, string accessToken, ProjectId project, GetMergeRequestsOptions? options)
    {
        var log = GetLogForCurrentOperation();
        log.Verbose($"Getting merge requests for GitLab project {project}");

        var client = GetClient(serverUrl, accessToken);

        var query = new MergeRequestQuery();

        if (options is not null)
        {
            query.State = options.State?.ToNGitLabMergeRequestState();
            query.SourceBranch = options.SourceBranch;
            query.TargetBranch = options.TargetBranch;
        }

        try
        {
            var mergeRequests = client.GetMergeRequest(project).Get(query).ToList();
            return Task.FromResult<IReadOnlyCollection<MergeRequest>>(mergeRequests);
        }
        catch (GitLabException ex)
        {
            throw new CakeException($"Error while getting merge requests for GitLab project {project}: {ex.Message}", ex);
        }
    }
}
