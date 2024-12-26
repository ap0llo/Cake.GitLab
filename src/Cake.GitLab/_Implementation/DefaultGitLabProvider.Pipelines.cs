using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using NGitLab;
using NGitLab.Models;

namespace Cake.GitLab;

public partial class DefaultGitLabProvider
{
    /// <inheritdoc />
    public async Task<Pipeline> GetPipelineAsync(string serverUrl, string accessToken, ProjectId project, long pipelineId)
    {
        var log = GetLogForCurrentOperation();
        log.Verbose($"Getting pipeline {pipelineId} from GitLab project {project}");

        var client = GetClient(serverUrl, accessToken);

        try
        {
            var pipeline = await client.GetPipelines(project).GetByIdAsync(pipelineId);
            return pipeline;
        }
        catch (GitLabException ex)
        {
            throw new CakeException($"Error while getting pipeline {pipelineId} from GitLab project {project}: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task SetPipelineNameAsync(string serverUrl, string accessToken, ProjectId project, long pipelineId, string name)
    {
        var log = GetLogForCurrentOperation();
        log.Verbose($"Setting name of pipeline {pipelineId} in GitLab project {project} to {name}");

        var client = GetClient(serverUrl, accessToken);
        var pipelinesClient = client.GetPipelines(project);
        try
        {
            await pipelinesClient.UpdateMetadataAsync(pipelineId, new PipelineMetadataUpdate() { Name = name });
        }
        catch (GitLabException ex)
        {
            throw new CakeException($"Error while setting name of pipeline {pipelineId} in GitLab project {project}: {ex.Message}", ex);
        }
    }
}
