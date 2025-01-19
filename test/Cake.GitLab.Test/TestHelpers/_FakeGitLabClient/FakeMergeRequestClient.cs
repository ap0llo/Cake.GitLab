using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NGitLab;
using NGitLab.Models;

namespace Cake.GitLab.Test.TestHelpers;

public class FakeMergeRequestClient : IMergeRequestClient
{
    public FakeMergeRequestClientInvocations Invocations { get; } = new();


    public IEnumerable<MergeRequest> All => throw new System.NotImplementedException();

    public MergeRequest this[long iid] => throw new System.NotImplementedException();


    public IEnumerable<MergeRequest> AllInState(NGitLab.Models.MergeRequestState state)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable<MergeRequest> Get(MergeRequestQuery query)
    {
        Invocations.Get.Add(query);
        return [];
    }

    public Task<MergeRequest> GetByIidAsync(long iid, SingleMergeRequestQuery options, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new System.NotImplementedException();
    }

    public MergeRequest Create(MergeRequestCreate mergeRequest)
    {
        throw new System.NotImplementedException();
    }

    public MergeRequest Update(long mergeRequestIid, MergeRequestUpdate mergeRequest)
    {
        throw new System.NotImplementedException();
    }

    public MergeRequest Close(long mergeRequestIid)
    {
        throw new System.NotImplementedException();
    }

    public MergeRequest Reopen(long mergeRequestIid)
    {
        throw new System.NotImplementedException();
    }

    public void Delete(long mergeRequestIid)
    {
        throw new System.NotImplementedException();
    }

    public MergeRequest CancelMergeWhenPipelineSucceeds(long mergeRequestIid)
    {
        throw new System.NotImplementedException();
    }

    [Obsolete]
    public MergeRequest Accept(long mergeRequestIid, MergeRequestAccept message)
    {
        throw new System.NotImplementedException();
    }

    public MergeRequest Accept(long mergeRequestIid, MergeRequestMerge message)
    {
        throw new System.NotImplementedException();
    }

    public MergeRequest Approve(long mergeRequestIid, MergeRequestApprove message)
    {
        throw new System.NotImplementedException();
    }

    public RebaseResult Rebase(long mergeRequestIid)
    {
        throw new System.NotImplementedException();
    }

    public Task<RebaseResult> RebaseAsync(long mergeRequestIid, MergeRequestRebase options, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable<PipelineBasic> GetPipelines(long mergeRequestIid)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable<Author> GetParticipants(long mergeRequestIid)
    {
        throw new System.NotImplementedException();
    }

    public GitLabCollectionResponse<MergeRequestVersion> GetVersionsAsync(long mergeRequestIid)
    {
        throw new System.NotImplementedException();
    }

    public IMergeRequestCommentClient Comments(long mergeRequestIid)
    {
        throw new System.NotImplementedException();
    }

    public IMergeRequestDiscussionClient Discussions(long mergeRequestIid)
    {
        throw new System.NotImplementedException();
    }

    public IMergeRequestCommitClient Commits(long mergeRequestIid)
    {
        throw new System.NotImplementedException();
    }

    public IMergeRequestChangeClient Changes(long mergeRequestIid)
    {
        throw new System.NotImplementedException();
    }

    public IMergeRequestApprovalClient ApprovalClient(long mergeRequestIid)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable<Issue> ClosesIssues(long mergeRequestIid)
    {
        throw new System.NotImplementedException();
    }

    public Task<TimeStats> TimeStatsAsync(long mergeRequestIid, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new System.NotImplementedException();
    }

    public GitLabCollectionResponse<ResourceLabelEvent> ResourceLabelEventsAsync(long projectId, long mergeRequestIid)
    {
        throw new System.NotImplementedException();
    }

    public GitLabCollectionResponse<ResourceMilestoneEvent> ResourceMilestoneEventsAsync(long projectId, long mergeRequestIid)
    {
        throw new System.NotImplementedException();
    }

    public GitLabCollectionResponse<ResourceStateEvent> ResourceStateEventsAsync(long projectId, long mergeRequestIid)
    {
        throw new System.NotImplementedException();
    }

}
