using System.Collections.Generic;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Annotations;
using NGitLab.Models;

namespace Cake.GitLab;

public static partial class GitLabAliases
{
    /// <summary>
    /// Gets data about a GitLab CI pipeline
    /// </summary>
    /// <param name="context">The current Cake context</param>
    /// <param name="serverUrl">The url of the GitLab server</param>
    /// <param name="accessToken">The access token for authenticating to the GitLab server</param>
    /// <param name="project">The path (name and namespace) or id of the project to get the pipeline data for.</param>
    /// <param name="pipelineId">The id of the pipeline to load data for.</param>
    [CakeMethodAlias]
    [CakeAliasCategory("Pipelines")]
    public static async Task<Pipeline> GitLabGetPipelineAsync(this ICakeContext context, string serverUrl, string accessToken, ProjectId project, long pipelineId) =>
        await context.GetGitLabProvider().GetPipelineAsync(serverUrl, accessToken, project, pipelineId);

    /// <summary>
    /// Get the jobs for the specified GitLab CI pipeline.
    /// </summary>
    /// <param name="context">The current Cake context</param>
    /// <param name="serverUrl">The url of the GitLab server</param>
    /// <param name="accessToken">The access token for authenticating to the GitLab server</param>
    /// <param name="project">The path (name and namespace) or id of the project to get the pipeline data for.</param>
    /// <param name="pipelineId">The id of the pipeline to load data for.</param>
    /// <param name="options">Optional settings for getting pipeline jobs.</param>
    /// <seealso href="https://docs.gitlab.com/ee/api/jobs.html#list-pipeline-jobs">List pipeline jobs (GitLab Docs)</seealso>
    [CakeMethodAlias]
    [CakeAliasCategory("Pipelines")]
    public static async Task<IReadOnlyCollection<Job>> GitLabGetPipelineJobsAsync(this ICakeContext context, string serverUrl, string accessToken, ProjectId project, long pipelineId, GetPipelineJobsOptions? options = null) =>
        await context.GetGitLabProvider().GetPipelineJobsAsync(serverUrl, accessToken, project, pipelineId, options);

    /// <summary>
    /// Updates the name of the specified pipeline
    /// </summary>
    /// <param name="context">The current Cake context</param>
    /// <param name="serverUrl">The url of the GitLab server</param>
    /// <param name="accessToken">The access token for authenticating to the GitLab server</param>
    /// <param name="project">The path (name and namespace) or id of the project to get the pipeline data for.</param>
    /// <param name="pipelineId">The id of the pipeline to load data for.</param>
    /// <param name="name">The name to set the pipeline name to.</param>
    [CakeMethodAlias]
    [CakeAliasCategory("Pipelines")]
    public static async Task GitLabSetPipelineNameAsync(this ICakeContext context, string serverUrl, string accessToken, ProjectId project, long pipelineId, string name) =>
        await context.GetGitLabProvider().SetPipelineNameAsync(serverUrl, accessToken, project, pipelineId, name);
}
