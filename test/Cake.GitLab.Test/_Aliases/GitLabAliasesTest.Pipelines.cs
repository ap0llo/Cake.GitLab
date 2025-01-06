using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cake.Core;
using NGitLab;
using NGitLab.Mock.Config;
using NGitLab.Models;
using Xunit;

namespace Cake.GitLab.Test;

public static partial class GitLabAliasesTest
{
    public class GitLabGetPipelineAsync(ITestOutputHelper testOutputHelper)
    {
        private const string s_GroupName = "group1";
        private const string s_ProjectName = "project1";
        private const string s_ProjectPath = $"{s_GroupName}/{s_ProjectName}";
        private const long s_ProjectId = 42;
        private const int s_PipelineId = 23;

        private readonly GitLabConfig m_GitLabConfig =
            new GitLabConfig()
                .WithUser("user1", isDefault: true)
                .WithGroup(s_GroupName)
                .WithProjectOfFullPath(
                    fullPath: s_ProjectPath,
                    id: (int)s_ProjectId,
                    configure: project =>
                        project
                            .WithCommit("Commit1", configure: commit => commit.Alias = "commit1")
                            .WithPipeline("commit1", configure: pipeline =>
                            {
                                pipeline.Id = s_PipelineId;
                                pipeline
                                    .WithJob("Job1", status: JobStatus.Success)
                                    .WithJob("Job2", status: JobStatus.Success);
                            }));

        [Theory]
        [InlineData(s_ProjectId)]
        [InlineData(s_ProjectPath)]
        public async Task Gets_expected_pipeline_data(object projectIdOrPath)
        {
            // ARRANGE
            ProjectId projectId = projectIdOrPath is string
                ? (string)projectIdOrPath
                : (long)projectIdOrPath;

            var server = m_GitLabConfig.BuildServer();


            var context = new NGitLabMockContext(testOutputHelper);
            context.GitLab.AddServer(server);

            // ACT
            var pipeline = await context.GitLabGetPipelineAsync(server.Url.ToString(), "SomeAccessToken", projectId, s_PipelineId);

            // ASSERT
            Assert.NotNull(pipeline);
            Assert.Equal(s_PipelineId, pipeline.Id);
        }

        [Fact]
        public async Task Fails_if_file_does_not_exist()
        {
            // ARRANGE
            var server = m_GitLabConfig.BuildServer();

            var context = new NGitLabMockContext(testOutputHelper);
            context.GitLab.AddServer(server);

            // ACT
            var ex = await Record.ExceptionAsync(async () => await context.GitLabGetPipelineAsync(server.Url.ToString(), "SomeAccessToken", s_ProjectPath, s_PipelineId + 10));

            // ASSERT
            Assert.IsType<CakeException>(ex);
            Assert.StartsWith($"Error while getting pipeline {s_PipelineId + 10} from GitLab project {s_ProjectPath}:", ex.Message);
        }
    }

    public class GitLabGetPipelineJobsAsync(ITestOutputHelper testOutputHelper)
    {
        public static TheoryData<string, long, GetPipelineJobsOptions?, Action<PipelineJobQuery>> TestCases()
        {
            var testCases = new TheoryData<string, long, GetPipelineJobsOptions?, Action<PipelineJobQuery>>()
            {
                {
                    "T01",
                    123, null,
                    query =>
                    {
                        Assert.Equal(123, query.PipelineId);
                        Assert.Null(query.IncludeRetried);
                        Assert.Null(query.Scope);
                    }
                },
                {
                    "T02",
                    123, new GetPipelineJobsOptions(),
                    query =>
                    {
                        Assert.Equal(123, query.PipelineId);
                        Assert.Null(query.IncludeRetried);
                        Assert.Null(query.Scope);
                    }
                },
                {
                    "T03",
                    123, new GetPipelineJobsOptions() { IncludeRetried = true },
                    query =>
                    {
                        Assert.Equal(123, query.PipelineId);
                        Assert.True(query.IncludeRetried);
                        Assert.Null(query.Scope);
                    }
                },
                {
                    "T04",
                    123, new GetPipelineJobsOptions() { IncludeRetried = false },
                    query =>
                    {
                        Assert.Equal(123, query.PipelineId);
                        Assert.False(query.IncludeRetried);
                        Assert.Null(query.Scope);
                    }
                },
                {
                    "T05",
                    456, new GetPipelineJobsOptions() { Scopes = [] },
                    query =>
                    {
                        Assert.Equal(456, query.PipelineId);
                        Assert.Null(query.IncludeRetried);
                        Assert.Null(query.Scope);
                    }
                },
            };

            // Add test cases to ensure JobScope values are mapped to the expected strings
            var scopeScenarios = new Dictionary<JobScope, string>()
            {
                { JobScope.Created , "created"},
                { JobScope.Pending , "pending"},
                { JobScope.Running , "running"},
                { JobScope.Failed , "failed"},
                { JobScope.Success , "success"},
                { JobScope.Canceled , "canceled"},
                { JobScope.Skipped , "skipped"},
                { JobScope.WaitingForResource , "waiting_for_resource"},
                { JobScope.Manual , "manual"},
            };

            foreach (var (scope, expectedScope) in scopeScenarios)
            {
                testCases.Add(
                    $"T06-{scope}",
                    123,
                    new GetPipelineJobsOptions() { Scopes = [scope] },
                    query =>
                    {
                        var actualScope = Assert.Single(query.Scope);
                        Assert.Equal(expectedScope, actualScope);
                    });
            }

            return testCases;
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public void Calls_GitLab_with_the_expected_pipeline_query(string id, long pipelineId, GetPipelineJobsOptions? options, Action<PipelineJobQuery> assertQuery)
        {
            // ARRANGE
            _ = id;
            var context = new FakeGitLabContext(testOutputHelper);

            // ACT
            _ = context.GitLabGetPipelineJobsAsync("https://gitlab.example.com", "ACCESSTOKEN", 123, pipelineId, options);

            // ASSERT
            var invocation = Assert.Single(context.GitLabClient.Pipelines[123].Invocations.GetJobsAsync);
            assertQuery(invocation);
        }

    }
}
