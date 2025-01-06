using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NGitLab;
using NGitLab.Models;

namespace Cake.GitLab.Test.TestHelpers;

public class FakePipelineClient : IPipelineClient
{
    public FakePipelineClientInvocations Invocations { get; } = new();


    public IEnumerable<PipelineBasic> All => throw new NotImplementedException();

    public Pipeline this[long id] => throw new NotImplementedException();

    public IEnumerable<Job> AllJobs => throw new NotImplementedException();

    public Task<Pipeline> GetByIdAsync(long id, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public GitLabCollectionResponse<Job> GetAllJobsAsync()
    {
        throw new NotImplementedException();
    }

    [Obsolete]
    public IEnumerable<Job> GetJobsInProject(NGitLab.Models.JobScope scope)
    {
        throw new NotImplementedException();
    }

    public Job[] GetJobs(long pipelineId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Job> GetJobs(PipelineJobQuery query)
    {
        throw new NotImplementedException();
    }

    public GitLabCollectionResponse<Job> GetJobsAsync(PipelineJobQuery query)
    {
        Invocations.GetJobsAsync.Add(query);
        return GitLabCollectionResponse.Empty<Job>();
    }

    public Pipeline Create(string @ref)
    {
        throw new NotImplementedException();
    }

    public Pipeline Create(PipelineCreate createOptions)
    {
        throw new NotImplementedException();
    }

    public Task<Pipeline> CreateAsync(PipelineCreate createOptions, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Pipeline CreatePipelineWithTrigger(string token, string @ref, Dictionary<string, string> variables)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<PipelineBasic> Search(PipelineQuery query)
    {
        throw new NotImplementedException();
    }

    public GitLabCollectionResponse<PipelineBasic> SearchAsync(PipelineQuery query)
    {
        throw new NotImplementedException();
    }

    public void Delete(long pipelineId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<PipelineVariable> GetVariables(long pipelineId)
    {
        throw new NotImplementedException();
    }

    public GitLabCollectionResponse<PipelineVariable> GetVariablesAsync(long pipelineId)
    {
        throw new NotImplementedException();
    }

    public TestReport GetTestReports(long pipelineId)
    {
        throw new NotImplementedException();
    }

    public TestReportSummary GetTestReportsSummary(long pipelineId)
    {
        throw new NotImplementedException();
    }

    public GitLabCollectionResponse<Bridge> GetBridgesAsync(PipelineBridgeQuery query)
    {
        throw new NotImplementedException();
    }

    public Task<Pipeline> RetryAsync(long pipelineId, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task<Pipeline> UpdateMetadataAsync(long pipelineId, PipelineMetadataUpdate update, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
}
