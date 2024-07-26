using System;
using Cake.GitLab.Internal;

namespace Cake.GitLab;

/// <summary>
/// Encapsualtes the identity of GitLab server
/// </summary>
public record GitLabServerIdentity : IEquatable<GitLabServerIdentity>
{
    private readonly string m_Host;

    /// <summary>
    /// Gets or sets the host name of the GitLab server.
    /// </summary>
    public string Host
    {
        get => m_Host;
        init => m_Host = Guard.NotNullOrWhitespace(value);
    }

    /// <summary>
    /// Gets the GitLab server's https URL
    /// </summary>
    public string Url => $"https://{Host}/";


    /// <summary>
    /// Initializes a new instance of <see cref="GitLabProjectIdentity"/>
    /// </summary>
    /// <param name="host">The host name of the GitLab server.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="host"/> is null or whitespace</exception>
    public GitLabServerIdentity(string host)
    {
        m_Host = Guard.NotNullOrWhitespace(host);
    }

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Host);

    /// <inheritdoc />
    public virtual bool Equals(GitLabServerIdentity? other)
    {
        return other is not null &&
            StringComparer.OrdinalIgnoreCase.Equals(Host, other.Host);
    }
}
