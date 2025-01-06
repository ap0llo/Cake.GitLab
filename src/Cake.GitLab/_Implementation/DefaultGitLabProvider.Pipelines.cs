using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.GitLab.Internal;
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
    public async Task<IReadOnlyCollection<Job>> GetPipelineJobsAsync(string serverUrl, string accessToken, ProjectId project, long pipelineId, GetPipelineJobsOptions? options)
    {
        var log = GetLogForCurrentOperation();
        log.Verbose($"Getting jobs for pipeline {pipelineId} from GitLab project {project}");

        var jobQuery = new PipelineJobQuery()
        {
            PipelineId = pipelineId,
            IncludeRetried = options?.IncludeRetried
        };

        if (options?.Scopes is { Count: > 0 } scopes)
        {
            jobQuery.Scope = scopes.Select(EnumHelper.ConvertToString).ToArray();
        }

        var client = GetClient(serverUrl, accessToken);
        try
        {
            var jobs = new List<Job>();
            await foreach (var job in client.GetPipelines(project).GetJobsAsync(jobQuery))
            {
                jobs.Add(job);
            }
            return jobs;
        }
        catch (GitLabException ex)
        {
            throw new CakeException($"Error while getting jobs for pipeline {pipelineId} from GitLab project {project}: {ex.Message}", ex);
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
