using Cake.Core;
using Cake.Core.Annotations;

namespace Cake.GitLab;

public static partial class GitLabAliases
{
    /// <summary>
    /// Attempts to determine the <see cref="ServerIdentity"/> from environment variables.
    /// </summary>
    /// <remarks>
    /// This alias attempts to determine the current <see cref="ServerIdentity"/> from the environment variables set by GitLab CI.
    /// Thus, this alias will only work if the build is running inside a GitLab CI pipeline.
    /// </remarks>
    /// <param name="context">The current context</param>
    /// <returns>Returns a <see cref="ServerIdentity"/> if it could be determined or <c>null</c> if the server identity could not be determined.</returns>
    /// <seealso href="https://docs.gitlab.com/ee/ci/variables/predefined_variables.html">Predefined CI/CD variables reference (GitLab Documentation)</seealso>
    [CakeMethodAlias]
    [CakeAliasCategory("Helpers")]
    public static ServerIdentity? GitLabTryGetCurrentServerIdentity(this ICakeContext context) =>
        context.GetGitLabProvider().TryGetCurrentServerIdentity();

    /// <summary>
    /// Attempts to determine the <see cref="ProjectIdentity"/> from environment variables.
    /// </summary>
    /// <remarks>
    /// This alias attempts to determine the current <see cref="ProjectIdentity"/> from the environment variables set by GitLab CI.
    /// Thus, this alias will only work if the build is running inside a GitLab CI pipeline.
    /// </remarks>
    /// <param name="context">The current context</param>
    /// <returns>Returns a <see cref="ProjectIdentity"/> if it could be determined or <c>null</c> if the identity could not be determined.</returns>
    /// <seealso href="https://docs.gitlab.com/ee/ci/variables/predefined_variables.html">Predefined CI/CD variables reference (GitLab Documentation)</seealso>
    [CakeMethodAlias]
    [CakeAliasCategory("Helpers")]
    public static ProjectIdentity? GitLabTryGetCurrentProjectIdentity(this ICakeContext context) =>
        context.GetGitLabProvider().TryGetCurrentProjectIdentity();
}
