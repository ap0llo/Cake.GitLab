using System;
using System.Collections.Generic;
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
    /// <param name="connection">The connection specifing the GitLab server to connect to</param>
    /// <param name="project">The path (name and namespace) or id of the project to get the pipeline data for.</param>
    /// <param name="pipelineId">The id of the pipeline to load data for.</param>    
    [CakeMethodAlias]
    public static async Task<Pipeline> GitLabGetPipelineAsync(this ICakeContext context, GitLabConnection connection, ProjectId project, int pipelineId)
    {
        if (connection is null)
            throw new ArgumentNullException(nameof(connection));

        var gitLabClient = GetClient(context, connection);
        var pipelinesClient = new PipelinesClient(context.Log, context.FileSystem, gitLabClient);

        return await pipelinesClient.GetPipelineAsync(project, pipelineId);
    }
}
