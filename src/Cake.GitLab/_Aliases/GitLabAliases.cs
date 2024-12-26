using Cake.Core;
using Cake.Core.Annotations;
using Cake.GitLab.Testing;

namespace Cake.GitLab;

/// <summary>
/// Provides aliases for interacting with a GitLab Server
/// </summary>
[CakeAliasCategory("GitLab")]
[CakeNamespaceImport("Cake.GitLab")]
public static partial class GitLabAliases
{
    /// <summary>
    /// Gets the <see cref="IGitLabProvider"/> to use
    /// </summary>
    /// <returns>
    /// Returns the <see cref="IGitLabProvider"/> provided by <paramref name="context"/> if it implements <see cref="IGitLabCakeContext"/>,
    /// otherwise returns the default implementation.
    /// </returns>
    private static IGitLabProvider GetGitLabProvider(this ICakeContext context)
    {
        return context is IGitLabCakeContext { GitLab: { } provider }
            ? provider
            : new DefaultGitLabProvider(context);
    }
}
