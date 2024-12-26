using Cake.Core;
using Cake.Core.Annotations;

namespace Cake.GitLab;

public static partial class GitLabAliases
{
    // TODO: CI_SERVER_HOST is not available though Cake's IGitLabCIProvider => consider opening up a PR
    private const string CI_SERVER_HOST = "CI_SERVER_HOST";

    /// <summary>
    /// Attempts to determine the <see cref="GitLabServerIdentity"/> from environment variables.
    /// </summary>
    /// <remarks>
    /// This alias attempts to determine the current <see cref="GitLabServerIdentity"/> from the environment variables set by GitLab CI.
    /// Thus, this alias will only work if the build is running inside a GitLab CI pipeline.
    /// </remarks>
    /// <param name="context">The current context</param>
    /// <returns>Returns a <see cref="GitLabServerIdentity"/> if it could be determined or <c>null</c> if the server identity could not be determined.</returns>
    /// <seealso href="https://docs.gitlab.com/ee/ci/variables/predefined_variables.html">Predefined CI/CD variables reference (GitLab Documentation)</seealso>
    [CakeMethodAlias]
    [CakeAliasCategory("Helpers")]
    public static GitLabServerIdentity? GitLabTryGetCurrentServerIdentity(this ICakeContext context) =>
        context.GetGitLabProvider().TryGetCurrentServerIdentity();

    /// <summary>
    /// Attempts to determine the <see cref="GitLabProjectIdentity"/> from environment variables.
    /// </summary>
    /// <remarks>
    /// This alias attempts to determine the current <see cref="GitLabProjectIdentity"/> from the environment variables set by GitLab CI.
    /// Thus, this alias will only work if the build is running inside a GitLab CI pipeline.
    /// </remarks>
    /// <param name="context">The current context</param>
    /// <returns>Returns a <see cref="GitLabProjectIdentity"/> if it could be determined or <c>null</c> if the identity could not be determined.</returns>
    /// <seealso href="https://docs.gitlab.com/ee/ci/variables/predefined_variables.html">Predefined CI/CD variables reference (GitLab Documentation)</seealso>
    [CakeMethodAlias]
    [CakeAliasCategory("Helpers")]
    public static GitLabProjectIdentity? GitLabTryGetCurrentProjectIdentity(this ICakeContext context) =>
        context.GetGitLabProvider().TryGetCurrentProjectIdentity();

    private static IGitLabProvider GetGitLabProvider(this ICakeContext context)
    {
        return new DefaultGitLabProvider(context);
    }

}
