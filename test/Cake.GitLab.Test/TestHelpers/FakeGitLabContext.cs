using Cake.Core;
using Cake.GitLab.Test.TestHelpers;
using Cake.GitLab.Testing;
using NGitLab;
using Xunit;

namespace Cake.GitLab.Test;

public class FakeGitLabContext(ITestOutputHelper testOutputHelper) : FakeContext(testOutputHelper), IGitLabCakeContext
{
    private class GitLabProvider(ICakeContext context, IGitLabClient gitLabClient) : DefaultGitLabProvider(context)
    {
        protected override IGitLabClient GetClient(string serverUrl, string accessToken) => gitLabClient;
    }

    public FakeGitLabClient GitLabClient { get; } = new();

    /// <inheritdoc />
    IGitLabProvider IGitLabCakeContext.GitLab => new GitLabProvider(this, GitLabClient);
}
