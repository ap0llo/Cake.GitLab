// Derived from https://github.com/ap0llo/changelog/blob/97d0d3111510dae9f4222704a50d88938be8debe/src/ChangeLog.Test/Integrations/GitLab/GitLabUrlParserTest.cs
// Licensed under the MIT License

using Cake.GitLab.Model;
using Xunit;
using Xunit.Abstractions;

namespace Cake.GitLab.Test;

public partial class GitLabAliasesTest
{
    public class GitLabGetProjectFromRemoteUrl(ITestOutputHelper testOutputHelper)
    {
        [Theory]
        [InlineData("not-a-url")]
        [InlineData("ftp://gitlab.com/owner/repo.git")] // unsupported scheme
        [InlineData("http://gitlab.com")]               // missing project path
        [InlineData("http://gitlab.com/user")]          // missing project name
        public void TryParseRemoteUrl_returns_false_for_invalid_input(string url)
        {
            // ARRANGE
            var context = new FakeContext(testOutputHelper);

            // ACT
            var projectInfo = context.GitLabGetProjectFromRemoteUrl(url);

            // ASSERT
            Assert.Null(projectInfo);
        }

        [Theory]
        [InlineData("https://gitlab.com/user/repoName.git", "gitlab.com", "user", "reponame")]
        [InlineData("https://gitlab.com/group/subgroup/repoName.git", "gitlab.com", "group/subgroup", "reponame")]
        [InlineData("https://example.com/user/repoName.git", "example.com", "user", "reponame")]
        [InlineData("git@gitlab.com:user/repoName.git", "gitlab.com", "user", "repoName")]
        [InlineData("git@gitlab.com:group/subgroup/repoName.git", "gitlab.com", "group/subgroup", "repoName")]
        [InlineData("git@example.com:user/repoName.git", "example.com", "user", "repoName")]
        [InlineData("git@example.com:group/subgroup/repoName.git", "example.com", "group/subgroup", "repoName")]
        public void TryParseRemoteUrl_returns_the_expected_GitHubProjectInfo(string url, string host, string @namespace, string projectName)
        {
            // ARRANGE
            var context = new FakeContext(testOutputHelper);

            var expected = new GitLabProjectInfo(host, @namespace, projectName);

            // ACT 
            var projectInfo = context.GitLabGetProjectFromRemoteUrl(url);

            // ASSERT
            Assert.NotNull(projectInfo);
            Assert.Equal(expected, projectInfo);
        }
    }
}
