using Cake.Core;
using NGitLab.Models;

namespace Cake.GitLab;

/// <summary>
/// Defines optional settings for the  <see cref="GitLabAliases.GitLabGetMergeRequestsAsync(ICakeContext,string,string,ProjectId,GetMergeRequestsOptions)"/> alias
/// </summary>
/// <seealso href="https://docs.gitlab.com/ee/api/merge_requests.html#list-project-merge-requests">List project merge requests (GitLab Docs)</seealso>
public class GetMergeRequestsOptions
{
    /// <summary>
    /// When set, returns only merge requests with the specified source branch
    /// </summary>
    public string? SourceBranch { get; set; }

    /// <summary>
    /// When set, returns only merge requests with the specified target branch
    /// </summary>
    public string? TargetBranch { get; set; }

    /// <summary>
    /// When set, returns only merge requests in the specified state
    /// </summary>
    public MergeRequestState? State { get; set; }
}
