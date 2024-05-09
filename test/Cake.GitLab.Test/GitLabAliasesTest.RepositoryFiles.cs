using System.Threading.Tasks;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;
using Cake.Testing;
using NGitLab.Mock.Config;
using Xunit;
using Xunit.Abstractions;

namespace Cake.GitLab.Test;
public partial class GitLabAliasesTest
{
    public class GitLabRepositoryDownloadFileAsync(ITestOutputHelper testOutputHelper)
    {
        private const string s_GroupName = "group1";
        private const string s_ProjectName = "project1";
        private const string s_ProjectPath = $"{s_GroupName}/{s_ProjectName}";
        private const string s_FileName = "file1.txt";
        private const string s_FileContent = "Hello World";
        private readonly GitLabConfig m_GitLabConfig =
            new GitLabConfig()
                .WithUser("user1", isDefault: true)
                .WithGroup(s_GroupName)
                .WithProjectOfFullPath(
                    s_ProjectPath,
                    configure: project => project.WithCommit(
                        configure: commit => commit.WithFile(s_FileName, s_FileContent)
                ));

        [Fact]
        public async Task Creates_expected_file()
        {
            // ARRANGE            
            var server = m_GitLabConfig.BuildServer();

            var context = new FakeContext(testOutputHelper);
            context.AddServer(server);

            var connection = new GitLabConnection(server.Url.ToString(), "SomeAccessToken");

            var outputPath = (FilePath)"file1.txt";

            // ACT
            await context.GitLabRepositoryDownloadFileAsync(connection, s_ProjectPath, s_FileName, "main", outputPath);

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

            var connection = new GitLabConnection(server.Url.ToString(), "SomeAccessToken");

            // ACT
            var outputDirectory = context.Environment.WorkingDirectory.Combine("new-directory");
            context.EnsureDirectoryDoesNotExist(outputDirectory);
            var outputPath = outputDirectory.CombineWithFilePath("outfile.txt");

            await context.GitLabRepositoryDownloadFileAsync(connection, s_ProjectPath, s_FileName, "main", outputPath);

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

            var connection = new GitLabConnection(server.Url.ToString(), "SomeAccessToken");

            // ACT            
            var ex = await Record.ExceptionAsync(async () => await context.GitLabRepositoryDownloadFileAsync(connection, s_ProjectPath, "does-not-exist", "main", "output.txt"));

            // ASSERT
            Assert.IsType<CakeException>(ex);
            Assert.Equal("Error while downloading file from GitLab: File not found", ex.Message);
        }
    }
}
