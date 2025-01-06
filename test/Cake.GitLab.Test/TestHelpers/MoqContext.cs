using Cake.Core;
using Cake.GitLab.Testing;
using Moq;
using NGitLab;
using Xunit;

namespace Cake.GitLab.Test;

public class MoqContext : FakeContext, IGitLabCakeContext
{
    private class MoqGitLabProvider(ICakeContext context, MoqGitLabClientMock gitLabClientMock) : DefaultGitLabProvider(context)
    {
        protected override IGitLabClient GetClient(string serverUrl, string accessToken) => gitLabClientMock.Object;
    }


    public MoqGitLabClientMock GitLabClient { get; }

    /// <inheritdoc />
    public IGitLabProvider GitLab { get; }


    public MoqContext(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        GitLabClient = new MoqGitLabClientMock();
        GitLab = new MoqGitLabProvider(this, GitLabClient);
    }
}
