using System;
using Cake.GitLab.Internal;

namespace Cake.GitLab;

/// <summary>
/// Provides both the identity and credentials for a GitLab server
/// </summary>
public record GitLabServerConnection : GitLabServerIdentity
{
    private readonly string m_AccessToken = "";

    /// <summary>
    /// Gets or sets the access token to use for communicating with the GitLab server
    /// </summary>
    public string AccessToken
    {
        get => m_AccessToken;
        init => m_AccessToken = Guard.NotNullOrWhitespace(value);
    }

    public GitLabServerConnection(string host, string accessToken) : base(host)
    {
        m_AccessToken = Guard.NotNullOrWhitespace(accessToken);
    }

    public GitLabServerConnection(GitLabServerIdentity identity, string accessToken) : base(identity)
    {
        m_AccessToken = Guard.NotNullOrWhitespace(accessToken);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hash = base.GetHashCode() * 397;
            hash ^= StringComparer.Ordinal.GetHashCode(AccessToken);
            return hash;
        }
    }

    /// <inheritdoc />
    public virtual bool Equals(GitLabServerConnection? other)
    {
        return other is not null &&
            base.Equals(other) &&
            StringComparer.Ordinal.Equals(AccessToken, other.AccessToken);
    }
}
