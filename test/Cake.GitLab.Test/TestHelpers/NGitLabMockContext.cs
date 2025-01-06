using Cake.GitLab.Testing;
using Xunit;

namespace Cake.GitLab.Test;

public class NGitLabMockContext : FakeContext, IGitLabCakeContext
{
    public NGitLabMockGitLabProvider GitLab { get; }

    /// <inheritdoc />
    IGitLabProvider IGitLabCakeContext.GitLab => GitLab;


    public NGitLabMockContext(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        GitLab = new NGitLabMockGitLabProvider(this);
    }
}
