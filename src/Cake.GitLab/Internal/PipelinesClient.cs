using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using NGitLab;
using NGitLab.Models;

namespace Cake.GitLab.Internal;

internal sealed class PipelinesClient(ICakeLog log, IFileSystem fileSystem, IGitLabClient gitLabClient) : ClientBase(log, fileSystem, gitLabClient)
{
    //TODO: Wrap the data in a custom model or continue using the NGitLab types?
    public async Task<Pipeline> GetPipelineAsync(string project, int pipelineId)
    {
        m_Log.Debug($"Getting pipeline from GitLab. Project '{project}', Pipeline Id: '{pipelineId}'");

        var pipelinesClient = m_GitLabClient.GetPipelines(project);
        try
        {
            var pipeline = await pipelinesClient.GetByIdAsync(pipelineId);
            return pipeline;
        }
        catch (GitLabException ex)
        {
            throw new CakeException($"Error while getting pipeline from GitLab: {ex.Message}", ex);
        }
    }
}
