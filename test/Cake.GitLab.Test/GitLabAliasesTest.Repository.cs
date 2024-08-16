using System;
using System.Linq;
using System.Threading.Tasks;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;
using Cake.Testing;
using Newtonsoft.Json.Serialization;
using NGitLab.Mock.Config;
using NGitLab.Models;
using Xunit;
using Xunit.Abstractions;

namespace Cake.GitLab.Test;
public partial class GitLabAliasesTest
{
    private const string s_GroupName = "group1";
    private const string s_ProjectName = "project1";
    private const string s_ProjectPath = $"{s_GroupName}/{s_ProjectName}";
    private const long s_ProjectId = 42;

    public class GitLabRepositoryDownloadFileAsync(ITestOutputHelper testOutputHelper)
    {
        private const string s_FileName = "file1.txt";
        private const string s_FileContent = "Hello World";
        private readonly GitLabConfig m_GitLabConfig =
            new GitLabConfig() { Url = "https://example.gitlab.com" }
                .WithUser("user1", isDefault: true)
                .WithGroup(s_GroupName)
                .WithProjectOfFullPath(
                    fullPath: s_ProjectPath,
                    id: (int)s_ProjectId,
                    configure: project => project.WithCommit(
                        configure: commit => commit.WithFile(s_FileName, s_FileContent)
                ));


        [Theory]
        [InlineData(s_ProjectId)]
        [InlineData(s_ProjectPath)]
        public async Task Creates_expected_file(object projectIdOrPath)
        {
            // ARRANGE
            ProjectId projectId = projectIdOrPath is string
                ? (string)projectIdOrPath
                : (long)projectIdOrPath;

            var server = m_GitLabConfig.BuildServer();

            var context = new FakeContext(testOutputHelper);
            context.AddServer(server);

            var outputPath = (FilePath)"file1.txt";

            // ACT
            await context.GitLabRepositoryDownloadFileAsync(server.Url.ToString(), "SomeAccessToken", projectId, s_FileName, "main", outputPath);

            // ASSERT
            var outFile = context.FileSystem.GetFile(outputPath);
            Assert.True(outFile.Exists);
            Assert.Equal(s_FileContent, outFile.GetTextContent());
        }

        [Fact]
        public async Task Creates_output_directory_if_it_does_not_exist()
        {
            // ARRANGE
            var server = m_GitLabConfig.BuildServer();

            var context = new FakeContext(testOutputHelper);
            context.AddServer(server);

            // ACT
            var outputDirectory = context.Environment.WorkingDirectory.Combine("new-directory");
            context.EnsureDirectoryDoesNotExist(outputDirectory);
            var outputPath = outputDirectory.CombineWithFilePath("outfile.txt");

            await context.GitLabRepositoryDownloadFileAsync(server.Url.ToString(), "SomeAccessToken", s_ProjectPath, s_FileName, "main", outputPath);

            // ASSERT
            var outFile = context.FileSystem.GetFile(outputPath);
            Assert.True(outFile.Exists);
        }

        [Fact]
        public async Task Fails_if_file_does_not_exist()
        {
            // ARRANGE
            var server = m_GitLabConfig.BuildServer();

            var context = new FakeContext(testOutputHelper);
            context.AddServer(server);

            // ACT            
            var ex = await Record.ExceptionAsync(async () => await context.GitLabRepositoryDownloadFileAsync(server.Url.ToString(), "SomeAccessToken", s_ProjectPath, "does-not-exist", "main", "output.txt"));

            // ASSERT
            Assert.IsType<CakeException>(ex);
            Assert.Equal("Failed to download does-not-exist at ref main from GitLab project group1/project1: File not found", ex.Message);
        }
    }


    public class GitLabRepositoryGetBranchesAsync(ITestOutputHelper testOutputHelper)
    {
        private readonly GitLabConfig m_GitLabConfig =
            new GitLabConfig() { Url = "https://example.gitlab.com" }
                .WithUser("user1", isDefault: true)
                .WithGroup(s_GroupName)
                .WithProjectOfFullPath(
                    fullPath: s_ProjectPath,
                    id: (int)s_ProjectId
                );


        [Theory]
        [InlineData(s_ProjectId)]
        [InlineData(s_ProjectPath)]
        public void Returns_expected_branches(object projectIdOrPath)
        {
            // ARRANGE
            ProjectId projectId = projectIdOrPath is string
                ? (string)projectIdOrPath
                : (long)projectIdOrPath;

            m_GitLabConfig.Projects.Single()
                .Configure(project =>
                {
                    project.DefaultBranch = "main";
                    project.WithCommit("Intial commit");
                });

            var server = m_GitLabConfig.BuildServer();

            server.AllProjects.Single().Repository.CreateBranch("some-other-branch");

            var context = new FakeContext(testOutputHelper);
            context.AddServer(server);

            // ACT
            var branches = context.GitLabRepositoryGetBranches(server.Url.ToString(), "SomeAccessToken", projectId);

            // ASSERT
            Assert.Collection(
                branches.OrderBy(x => x.Name, StringComparer.Ordinal),
                branch => Assert.Equal("main", branch.Name),
                branch => Assert.Equal("some-other-branch", branch.Name)
            );
        }
    }

    public class GitLabRepositoryCreateTag(ITestOutputHelper testOutputHelper)
    {
        private readonly GitLabConfig m_GitLabConfig =
            new GitLabConfig() { Url = "https://example.gitlab.com" }
                .WithUser("user1", isDefault: true)
                .WithGroup(s_GroupName)
                .WithProjectOfFullPath(
                    fullPath: s_ProjectPath,
                    id: (int)s_ProjectId
                );


        [Theory]
        [InlineData(s_ProjectId)]
        [InlineData(s_ProjectPath)]
        public void Creates_expected_tag(object projectIdOrPath)
        {
            // ARRANGE
            ProjectId projectId = projectIdOrPath is string
                ? (string)projectIdOrPath
                : (long)projectIdOrPath;



            m_GitLabConfig.Projects.Single()
                .Configure(project =>
                {
                    project.DefaultBranch = "main";
                    project.WithCommit("Intial commit");
                    project.WithUserPermission("user1", AccessLevel.Maintainer);
                });

            var server = m_GitLabConfig.BuildServer();
            var project = server.AllProjects.Single();
            var commit = project.Repository.CreateBranch("some-other-branch").Commits.Single();

            var context = new FakeContext(testOutputHelper);
            context.AddServer(server);

            // ACT
            var branches = context.GitLabRepositoryCreateTag(server.Url.ToString(), "SomeAccessToken", projectId, commit.Sha, "tag-name");

            // ASSERT
            Assert.Collection(
                project.Repository.GetTags(),
                tag => Assert.Equal("tag-name", tag.FriendlyName)
            );
        }
    }
}
