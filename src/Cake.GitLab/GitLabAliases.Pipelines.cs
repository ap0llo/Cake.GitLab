using System;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.GitLab.Internal;
using NGitLab.Models;

namespace Cake.GitLab;

public static partial class GitLabAliases
{
    /// <summary>
    /// Gets data about the specified pipeline
    /// </summary>
    /// <param name="context">The current Cake context</param>
    /// <param name="serverUrl">The url of the GitLab server</param>
    /// <param name="accessToken">The access token for authenticating to the GitLab server</param>
    /// <param name="project">The path (name and namespace) or id of the project to get the pipeline data for.</param>
    /// <param name="pipelineId">The id of the pipeline to load data for.</param>    
    [CakeMethodAlias]
    public static async Task<Pipeline> GitLabGetPipelineAsync(this ICakeContext context, string serverUrl, string accessToken, ProjectId project, int pipelineId)
    {
        var gitLabClient = GetClient(context, serverUrl, accessToken);
        var pipelinesClient = new PipelinesClient(context.Log, context.FileSystem, gitLabClient);

        return await pipelinesClient.GetPipelineAsync(project, pipelineId);
    }
}
