using System.Threading.Tasks;
using Cake.Core;
using NGitLab;
using NGitLab.Mock.Config;
using Xunit;
using Xunit.Abstractions;

namespace Cake.GitLab.Test;

public partial class GitLabAliasesTest
{
    public class GitLabGetPipeline(ITestOutputHelper testOutputHelper)
    {
        private const string s_GroupName = "group1";
        private const string s_ProjectName = "project1";
        private const string s_ProjectPath = $"{s_GroupName}/{s_ProjectName}";
        private const int s_PipelineId = 23;

        private readonly GitLabConfig m_GitLabConfig =
            new GitLabConfig()
                .WithUser("user1", isDefault: true)
                .WithGroup(s_GroupName)
                .WithProjectOfFullPath(
                    s_ProjectPath,
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

        [Fact]
        public async Task Gets_expected_pipeline_data()
        {
            // ARRANGE            
            var server = m_GitLabConfig.BuildServer();

            var context = new FakeContext(testOutputHelper);
            context.AddServer(server);

            var connection = new GitLabConnection(server.Url.ToString(), "SomeAccessToken");

            // ACT
            var pipeline = await context.GitLabGetPipeline(connection, s_ProjectPath, s_PipelineId);

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

            var connection = new GitLabConnection(server.Url.ToString(), "SomeAccessToken");

            // ACT            
            var ex = await Record.ExceptionAsync(async () => await context.GitLabGetPipeline(connection, s_ProjectPath, s_PipelineId + 10));

            // ASSERT
            Assert.IsType<CakeException>(ex);
            Assert.StartsWith("Error while getting pipeline from GitLab:", ex.Message);
        }
    }
}
