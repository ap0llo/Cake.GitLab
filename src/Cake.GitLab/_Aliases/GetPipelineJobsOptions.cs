using System.Collections.Generic;
using Cake.Core;
using NGitLab.Models;

namespace Cake.GitLab;

/// <summary>
/// Defines optional settings for the  <see cref="GitLabAliases.GitLabGetPipelineJobsAsync(ICakeContext,string,string,ProjectId,long,GetPipelineJobsOptions)"/> alias
/// </summary>
/// <seealso href="https://docs.gitlab.com/ee/api/jobs.html#list-pipeline-jobs">List pipeline jobs (GitLab Docs)</seealso>
public class GetPipelineJobsOptions
{
    /// <summary>
    /// Gets or sets whether to include retried jobs in the result.
    /// </summary>
    public bool? IncludeRetried { get; set; }

    /// <summary>
    /// Gets or sets the scopes of jobs to show.
    /// All jobs are returned if <see cref="Scopes"/> is <c>null</c> or empty (default).
    /// </summary>
    public IList<JobScope>? Scopes { get; set; } = null;
}
