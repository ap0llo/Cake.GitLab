using System;
using NGitLab;
using NGitLab.Models;

namespace Cake.GitLab.Test.TestHelpers;

public class FakeGitLabClient : IGitLabClient
{
    public FakesCollection<FakePipelineClient> PipelinesClient { get; } = new FakesCollection<FakePipelineClient>(id => new FakePipelineClient());

    public FakesCollection<FakeMergeRequestClient> MergeRequestClient { get; } = new FakesCollection<FakeMergeRequestClient>(id => new FakeMergeRequestClient());

    IPipelineClient IGitLabClient.GetPipelines(ProjectId projectId) => PipelinesClient[projectId];

    IMergeRequestClient IGitLabClient.GetMergeRequest(ProjectId projectId) => MergeRequestClient[projectId];

    public IEventClient GetEvents()
    {
        throw new System.NotImplementedException();
    }

    public IEventClient GetUserEvents(long userId)
    {
        throw new System.NotImplementedException();
    }

    public IEventClient GetProjectEvents(ProjectId projectId)
    {
        throw new System.NotImplementedException();
    }

    public IRepositoryClient GetRepository(ProjectId projectId)
    {
        throw new System.NotImplementedException();
    }

    public ICommitClient GetCommits(ProjectId projectId)
    {
        throw new System.NotImplementedException();
    }

    public ICommitStatusClient GetCommitStatus(ProjectId projectId)
    {
        throw new System.NotImplementedException();
    }

    public IPipelineScheduleClient GetPipelineSchedules(ProjectId projectId)
    {
        throw new System.NotImplementedException();
    }

    public ITriggerClient GetTriggers(ProjectId projectId)
    {
        throw new System.NotImplementedException();
    }

    public IJobClient GetJobs(ProjectId projectId)
    {
        throw new System.NotImplementedException();
    }

    public IMergeRequestClient GetGroupMergeRequest(GroupId groupId)
    {
        throw new System.NotImplementedException();
    }

    public IMilestoneClient GetMilestone(ProjectId projectId)
    {
        throw new System.NotImplementedException();
    }

    public IMilestoneClient GetGroupMilestone(GroupId groupId)
    {
        throw new System.NotImplementedException();
    }

    public IReleaseClient GetReleases(ProjectId projectId)
    {
        throw new System.NotImplementedException();
    }

    public IProjectIssueNoteClient GetProjectIssueNoteClient(ProjectId projectId)
    {
        throw new System.NotImplementedException();
    }

    public IEnvironmentClient GetEnvironmentClient(ProjectId projectId)
    {
        throw new System.NotImplementedException();
    }

    public IClusterClient GetClusterClient(ProjectId projectId)
    {
        throw new System.NotImplementedException();
    }

    public IWikiClient GetWikiClient(ProjectId projectId)
    {
        throw new System.NotImplementedException();
    }

    public IProjectBadgeClient GetProjectBadgeClient(ProjectId projectId)
    {
        throw new System.NotImplementedException();
    }

    public IGroupBadgeClient GetGroupBadgeClient(GroupId groupId)
    {
        throw new System.NotImplementedException();
    }

    public IProjectVariableClient GetProjectVariableClient(ProjectId projectId)
    {
        throw new System.NotImplementedException();
    }

    public IGroupVariableClient GetGroupVariableClient(GroupId groupId)
    {
        throw new System.NotImplementedException();
    }

    public IProjectLevelApprovalRulesClient GetProjectLevelApprovalRulesClient(ProjectId projectId)
    {
        throw new System.NotImplementedException();
    }

    public IProtectedBranchClient GetProtectedBranchClient(ProjectId projectId)
    {
        throw new System.NotImplementedException();
    }

    public IProtectedTagClient GetProtectedTagClient(ProjectId projectId)
    {
        throw new System.NotImplementedException();
    }

    public ISearchClient GetGroupSearchClient(GroupId groupId)
    {
        throw new System.NotImplementedException();
    }

    public ISearchClient GetProjectSearchClient(ProjectId projectId)
    {
        throw new System.NotImplementedException();
    }

    public IGroupHooksClient GetGroupHooksClient(GroupId groupId)
    {
        throw new System.NotImplementedException();
    }

    public IUserClient Users => throw new NotImplementedException();

    public IProjectClient Projects => throw new NotImplementedException();

    public IIssueClient Issues => throw new NotImplementedException();

    public IGroupsClient Groups => throw new NotImplementedException();

    public IGlobalJobClient Jobs => throw new NotImplementedException();

    public ILabelClient Labels => throw new NotImplementedException();

    public IRunnerClient Runners => throw new NotImplementedException();

    public IMergeRequestClient MergeRequests => throw new NotImplementedException();

    public ILintClient Lint => throw new NotImplementedException();

    public IMembersClient Members => throw new NotImplementedException();

    public IVersionClient Version => throw new NotImplementedException();

    public INamespacesClient Namespaces => throw new NotImplementedException();

    public ISnippetClient Snippets => throw new NotImplementedException();

    public ISystemHookClient SystemHooks => throw new NotImplementedException();

    public IDeploymentClient Deployments => throw new NotImplementedException();

    public IEpicClient Epics => throw new NotImplementedException();

    public IGraphQLClient GraphQL => throw new NotImplementedException();

    public ISearchClient AdvancedSearch => throw new NotImplementedException();
}
