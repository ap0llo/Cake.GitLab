using Xunit;

namespace Cake.GitLab.Test;

public static partial class GitLabAliasesTest
{
    public class GitLabTryGetCurrentServerIdentity(ITestOutputHelper testOutputHelper)
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("no")]
        [InlineData("some value")]
        public void Returns_null_if_the_build_is_not_running_in_GitLab_CI(string? ciServerVariable)
        {
            // ARRANGE
            var context = new FakeContext(testOutputHelper);
            context.Environment.SetEnvironmentVariable("CI_SERVER", ciServerVariable);

            // ACT
            var actual = context.GitLabTryGetCurrentServerIdentity();

            // ASSERT
            Assert.Null(actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void Returns_null_if_the_host_is_null_or_empty(string? ciServerHostVariable)
        {
            // ARRANGE
            var context = new FakeContext(testOutputHelper);
            context.Environment.SetEnvironmentVariable("CI_SERVER", "yes");
            context.Environment.SetEnvironmentVariable("CI_SERVER_HOST", ciServerHostVariable);

            // ACT
            var actual = context.GitLabTryGetCurrentServerIdentity();

            // ASSERT
            Assert.Null(actual);
        }

        [Theory]
        [InlineData("yes")]
        [InlineData("YES")]
        public void Returns_expected_server_identity(string? ciServerVariable)
        {
            // ARRANGE
            var context = new FakeContext(testOutputHelper);
            context.Environment.SetEnvironmentVariable("CI_SERVER", ciServerVariable);
            context.Environment.SetEnvironmentVariable("CI_SERVER_URL", "https://example.com");

            var expected = new ServerIdentity("example.com");

            // ACT
            var actual = context.GitLabTryGetCurrentServerIdentity();

            // ASSERT
            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }
    }

    public class GitLabTryGetCurrentProjectIdentity(ITestOutputHelper testOutputHelper)
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("no")]
        [InlineData("some value")]
        public void Returns_null_if_the_build_is_not_running_in_GitLab_CI(string? ciServerVariable)
        {
            // ARRANGE
            var context = new FakeContext(testOutputHelper);
            context.Environment.SetEnvironmentVariable("CI_SERVER", ciServerVariable);

            // ACT
            var actual = context.GitLabTryGetCurrentProjectIdentity();

            // ASSERT
            Assert.Null(actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void Returns_null_if_the_host_is_null_or_empty(string? ciServerHostVariable)
        {
            // ARRANGE
            var context = new FakeContext(testOutputHelper);
            context.Environment.SetEnvironmentVariable("CI_SERVER", "yes");
            context.Environment.SetEnvironmentVariable("CI_SERVER_HOST", ciServerHostVariable);

            // ACT
            var actual = context.GitLabTryGetCurrentProjectIdentity();

            // ASSERT
            Assert.Null(actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("not-a-project-path")]
        public void Returns_null_if_the_project_path_is_invalid(string? ciProjectPathVariable)
        {
            // ARRANGE
            var context = new FakeContext(testOutputHelper);
            context.Environment.SetEnvironmentVariable("CI_SERVER", "yes");
            context.Environment.SetEnvironmentVariable("CI_SERVER_HOST", "example.com");
            context.Environment.SetEnvironmentVariable("CI_PROJECT_PATH", ciProjectPathVariable);

            // ACT
            var actual = context.GitLabTryGetCurrentProjectIdentity();

            // ASSERT
            Assert.Null(actual);
        }

        [Theory]
        [InlineData("yes", "https://example.com", "user/project")]
        [InlineData("YES", "https://gitlab.example.com", "group/subgroup/project")]
        public void Returns_expected_project_identity(string? ciServerVariable, string serverUrl, string projectPath)
        {
            // ARRANGE
            var context = new FakeContext(testOutputHelper);
            context.Environment.SetEnvironmentVariable("CI_SERVER", ciServerVariable);
            context.Environment.SetEnvironmentVariable("CI_SERVER_URL", serverUrl);
            context.Environment.SetEnvironmentVariable("CI_PROJECT_PATH", projectPath);

            var expected = new ProjectIdentity(ServerIdentity.FromUrl(serverUrl), projectPath);

            // ACT
            var actual = context.GitLabTryGetCurrentProjectIdentity();

            // ASSERT
            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }
    }
}
