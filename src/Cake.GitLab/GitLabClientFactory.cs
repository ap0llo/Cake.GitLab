using NGitLab;

namespace Cake.GitLab;

/// <summary>
/// Provides access to the default implementation of <see cref="IGitLabClientFactory"/> used by <c>Cake.GitLab</c>
/// </summary>
public static class GitLabClientFactory
{
    private class DefaultGitLabClientFactory : IGitLabClientFactory
    {
        public IGitLabClient GetClient(string serverUrl, string accessToken) => new GitLabClient(serverUrl, accessToken);
    }

    /// <summary>
    /// Gets the singleton instance of the default <see cref="IGitLabClientFactory"/>.
    /// </summary>
    /// <remarks>
    /// The default implementation of <see cref="IGitLabClientFactory"/> will create a new instance of <see cref="NGitLab.GitLabClient"/> every time an instance is requested.
    /// </remarks>
    public static readonly IGitLabClientFactory Default = new DefaultGitLabClientFactory();
}
