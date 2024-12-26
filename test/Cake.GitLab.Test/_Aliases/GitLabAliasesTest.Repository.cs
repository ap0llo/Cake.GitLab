using System;
using System.Linq;
using System.Threading.Tasks;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;
using Cake.GitLab.Test.TestHelpers;
using Cake.Testing;
using NGitLab.Mock.Config;
using NGitLab.Models;
using Xunit;

namespace Cake.GitLab.Test;

public static partial class GitLabAliasesTest
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
            var ex = await Record.ExceptionAsync(() => context.GitLabRepositoryDownloadFileAsync(server.Url.ToString(), "SomeAccessToken", s_ProjectPath, "does-not-exist", "main", "output.txt"));

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
        public async Task Returns_expected_branches(object projectIdOrPath)
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
            var branches = await context.GitLabRepositoryGetBranchesAsync(server.Url.ToString(), "SomeAccessToken", projectId);

            // ASSERT
            Assert.Collection(
                branches.OrderBy(x => x.Name, StringComparer.Ordinal),
                branch => Assert.Equal("main", branch.Name),
                branch => Assert.Equal("some-other-branch", branch.Name)
            );
        }
    }

    public class GitLabRepositoryCreateTagAsync
    {
        private readonly GitLabConfig m_GitLabConfig;
        private readonly ITestOutputHelper m_TestOutputHelper;
        private readonly FakeContext m_Context;
        private readonly NGitLab.Mock.Project m_Project;
        private readonly NGitLab.Mock.User m_User;

        public GitLabRepositoryCreateTagAsync(ITestOutputHelper testOutputHelper)
        {
            m_TestOutputHelper = testOutputHelper;

            m_GitLabConfig = new GitLabConfig() { Url = "https://example.gitlab.com" }
                .WithUser("user1", isDefault: true)
                .WithGroup(s_GroupName)
                .WithProjectOfFullPath(
                    fullPath: s_ProjectPath,
                    id: (int)s_ProjectId
                );

            m_GitLabConfig
                .Projects
                .Single()
                .Configure(project =>
                {
                    project.DefaultBranch = "main";
                    project.WithCommit("Intial commit");
                    project.WithUserPermission("user1", AccessLevel.Maintainer);
                });


            var server = m_GitLabConfig.BuildServer();
            m_Context = new FakeContext(m_TestOutputHelper);
            m_Context.AddServer(server);

            m_Project = server.AllProjects.Single();

            m_User = server.Users.Single();
        }

        [Theory]
        [InlineData(s_ProjectId)]
        [InlineData(s_ProjectPath)]
        public async Task Creates_expected_tag_from_commit_sha(object projectId)
        {
            // ARRANGE
            var commit = m_Project.Repository.Commit(m_User, "Some commit");

            // ACT
            var branches = await m_Context.GitLabRepositoryCreateTagAsync(m_Project.Server.Url.ToString(), "SomeAccessToken", projectId.AsProjectId(), commit.Sha, "tag-name");

            // ASSERT
            Assert.Collection(
                m_Project.Repository.GetTags(),
                tag =>
                {
                    Assert.Equal("tag-name", tag.FriendlyName);
                    Assert.Equal(commit.Sha, tag.Target.Sha);
                }
            );
        }

        [Theory]
        [InlineData(s_ProjectId)]
        [InlineData(s_ProjectPath)]
        public async Task Creates_expected_tag_from_branch_name(object projectId)
        {
            // ARRANGE
            var commit = m_Project.Repository.Commit(m_User, "Some commit");
            var branch = m_Project.Repository.CreateBranch("some-other-branch", commit.Sha);

            // ACT
            var branches = await m_Context.GitLabRepositoryCreateTagAsync(m_Project.Server.Url.ToString(), "SomeAccessToken", projectId.AsProjectId(), branch.FriendlyName, "tag-name");

            // ASSERT
            Assert.Collection(
                m_Project.Repository.GetTags(),
                tag =>
                {
                    Assert.Equal("tag-name", tag.FriendlyName);
                    Assert.Equal(commit.Sha, tag.Target.Sha);
                }
            );
        }

        [Theory]
        [InlineData(s_ProjectId)]
        [InlineData(s_ProjectPath)]
        public async Task Succeeds_if_tag_already_exists_for_the_target_commit(object projectId)
        {
            // ARRANGE
            var commit = m_Project.Repository.Commit(m_User, "Some commit");
            var branch = m_Project.Repository.CreateBranch("some-other-branch", commit.Sha);

            m_Project.Repository.CreateTag("tag-name", commit.Sha);

            // ACT
            var branches = await m_Context.GitLabRepositoryCreateTagAsync(m_Project.Server.Url.ToString(), "SomeAccessToken", projectId.AsProjectId(), branch.FriendlyName, "tag-name");

            // ASSERT
            Assert.Collection(
                m_Project.Repository.GetTags(),
                tag =>
                {
                    Assert.Equal("tag-name", tag.FriendlyName);
                    Assert.Equal(commit.Sha, tag.Target.Sha);
                }
            );
        }

        [Theory]
        [InlineData(s_ProjectId)]
        [InlineData(s_ProjectPath)]
        public async Task Fails_if_tag_already_exists_for_the_different_commit(object projectId)
        {
            // ARRANGE
            var commit1 = m_Project.Repository.Commit(m_User, "Commit 1");
            var commit2 = m_Project.Repository.Commit(m_User, "Commit 2");
            m_Project.Repository.CreateTag("tag-name", commit1.Sha);

            // ACT
            var ex = await Record.ExceptionAsync(async () => await m_Context.GitLabRepositoryCreateTagAsync(m_Project.Server.Url.ToString(), "SomeAccessToken", projectId.AsProjectId(), commit2.Sha, "tag-name"));

            // ASSERT
            Assert.NotNull(ex);
            Assert.Collection(
                m_Project.Repository.GetTags(),
                tag =>
                {
                    Assert.Equal("tag-name", tag.FriendlyName);
                    Assert.Equal(commit1.Sha, tag.Target.Sha);
                }
            );
        }
    }
}
