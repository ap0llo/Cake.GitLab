using Cake.Core;

namespace Cake.GitLab;

/// <summary>
/// Extended <see cref="ICakeContext"/> that provides a <see cref="GitLabServerConnection"/>.
/// </summary>
/// <remarks>
/// Implement this interface in your context class to avoid having to pass a <see cref="GitLabServerConnection"/> to each GitLab alias call.
/// </remarks>
public interface IGitLabServerConnectionCakeContext : ICakeContext
{
    /// <summary>
    /// Gets the connection parameters for accessing GitLab
    /// </summary>
    public GitLabServerConnection Connection { get; }
}
