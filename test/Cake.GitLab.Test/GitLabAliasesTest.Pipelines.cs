using System.Threading.Tasks;
using Cake.Core;
using NGitLab;
using NGitLab.Mock.Config;
using NGitLab.Models;
using Xunit;
using Xunit.Abstractions;

namespace Cake.GitLab.Test;

public partial class GitLabAliasesTest
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

            var context = new FakeContext(testOutputHelper);
            context.AddServer(server);

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

            var context = new FakeContext(testOutputHelper);
            context.AddServer(server);

            // ACT            
            var ex = await Record.ExceptionAsync(async () => await context.GitLabGetPipelineAsync(server.Url.ToString(), "SomeAccessToken", s_ProjectPath, s_PipelineId + 10));

            // ASSERT
            Assert.IsType<CakeException>(ex);
            Assert.StartsWith($"Error while getting pipeline {s_PipelineId + 10} from GitLab project {s_ProjectPath}:", ex.Message);
        }
    }
}
