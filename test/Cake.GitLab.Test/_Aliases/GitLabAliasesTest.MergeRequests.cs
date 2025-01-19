using System;
using NGitLab.Models;
using Xunit;
using NGitLabMergeRequestState = NGitLab.Models.MergeRequestState;

namespace Cake.GitLab.Test;

public static partial class GitLabAliasesTest
{
    public class GitLabGetMergeRequestsAsync(ITestOutputHelper testOutputHelper)
    {
        public static TheoryData<string, GetMergeRequestsOptions?, Action<MergeRequestQuery>> TestCases()
        {
            var testCases = new TheoryData<string, GetMergeRequestsOptions?, Action<MergeRequestQuery>>()
            {
                {
                    "T01",
                    null,
                    query =>
                    {
                        Assert.Null(query.State);
                        Assert.Null(query.SourceBranch);
                        Assert.Null(query.TargetBranch);
                    }
                },
                {
                    "T02",
                    new GetMergeRequestsOptions(),
                    query =>
                    {
                        Assert.Null(query.State);
                        Assert.Null(query.SourceBranch);
                        Assert.Null(query.TargetBranch);
                    }
                },
                {
                    "T03",
                    new GetMergeRequestsOptions() { State = MergeRequestState.Opened },
                    query =>
                    {
                        Assert.Equal(NGitLabMergeRequestState.opened, query.State);
                        Assert.Null(query.SourceBranch);
                        Assert.Null(query.TargetBranch);
                    }
                },
                {
                    "T04",
                    new GetMergeRequestsOptions() { State = MergeRequestState.Closed },
                    query =>
                    {
                        Assert.Equal(NGitLabMergeRequestState.closed, query.State);
                        Assert.Null(query.SourceBranch);
                        Assert.Null(query.TargetBranch);
                    }
                },
                {
                    "T05",
                    new GetMergeRequestsOptions() { State = MergeRequestState.Merged },
                    query =>
                    {
                        Assert.Equal(NGitLabMergeRequestState.merged, query.State);
                        Assert.Null(query.SourceBranch);
                        Assert.Null(query.TargetBranch);
                    }
                },
                {
                    "T06",
                    new GetMergeRequestsOptions() { State = MergeRequestState.Locked },
                    query =>
                    {
                        Assert.Equal(NGitLabMergeRequestState.locked, query.State);
                        Assert.Null(query.SourceBranch);
                        Assert.Null(query.TargetBranch);
                    }
                },
                {
                    "T07",
                    new GetMergeRequestsOptions() { SourceBranch = "some-branch"},
                    query =>
                    {
                        Assert.Null( query.State);
                        Assert.Equal("some-branch", query.SourceBranch);
                        Assert.Null(query.TargetBranch);
                    }
                },
                {
                    "T08",
                    new GetMergeRequestsOptions() { TargetBranch = "some-branch"},
                    query =>
                    {
                        Assert.Null(query.State);
                        Assert.Null(query.SourceBranch);
                        Assert.Equal("some-branch", query.TargetBranch);
                    }
                },
            };

            return testCases;
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public void Calls_GitLab_with_the_expected_merge_request_query(string id, GetMergeRequestsOptions? options, Action<MergeRequestQuery> assertQuery)
        {
            // ARRANGE
            _ = id;
            var context = new FakeGitLabContext(testOutputHelper);

            // ACT
            _ = context.GitLabGetMergeRequestsAsync("https://gitlab.example.com", "ACCESSTOKEN", 123, options);

            // ASSERT
            var invocation = Assert.Single(context.GitLabClient.MergeRequestClient[123].Invocations.Get);
            assertQuery(invocation);
        }
    }
}
