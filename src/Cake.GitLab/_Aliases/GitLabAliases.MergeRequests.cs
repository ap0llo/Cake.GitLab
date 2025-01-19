using System.Collections.Generic;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Annotations;
using NGitLab.Models;

namespace Cake.GitLab;

public static partial class GitLabAliases
{
    /// <summary>
    /// Gets the Merge Requests for a project
    /// </summary>
    /// <param name="context">The current Cake context</param>
    /// <param name="serverUrl">The url of the GitLab server</param>
    /// <param name="accessToken">The access token for authenticating to the GitLab server</param>
    /// <param name="project">The path (name and namespace) or id of the project to get the merge requests for.</param>
    /// <param name="options">Optional settings for getting merge requests.</param>
    /// <seealso href="https://docs.gitlab.com/ee/api/merge_requests.html#list-project-merge-requests">List project merge requests (GitLab Docs)</seealso>
    [CakeMethodAlias]
    [CakeAliasCategory("MergeRequests")]
    public static async Task<IReadOnlyCollection<MergeRequest>> GitLabGetMergeRequestsAsync(this ICakeContext context, string serverUrl, string accessToken, ProjectId project, GetMergeRequestsOptions? options = null) =>
        await context.GetGitLabProvider().GetMergeRequestsAsync(serverUrl, accessToken, project, options);
}
