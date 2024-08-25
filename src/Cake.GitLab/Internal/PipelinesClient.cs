using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using NGitLab;
using NGitLab.Models;

namespace Cake.GitLab.Internal;

internal sealed class PipelinesClient(ICakeLog log, IFileSystem fileSystem, IGitLabClient gitLabClient) : ClientBase(log, fileSystem, gitLabClient)
{
    public async Task<Pipeline> GetPipelineAsync(ProjectId project, int pipelineId)
    {
        m_Log.Verbose($"Getting pipeline {pipelineId} from GitLab project {project}");

        var pipelinesClient = m_GitLabClient.GetPipelines(project);
        try
        {
            var pipeline = await pipelinesClient.GetByIdAsync(pipelineId);
            return pipeline;
        }
        catch (GitLabException ex)
        {
            throw new CakeException($"Error while getting pipeline {pipelineId} from GitLab project {project}: {ex.Message}", ex);
        }
    }
}
