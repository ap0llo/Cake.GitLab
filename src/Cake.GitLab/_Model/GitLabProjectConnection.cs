using System;
using Cake.GitLab.Internal;

namespace Cake.GitLab;

/// <summary>
/// Provides both the identity and credentials for a GitLab project
/// </summary>
public record GitLabProjectConnection : GitLabProjectIdentity
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

    public GitLabProjectConnection(string host, string @namespace, string project, string accessToken) : base(host, @namespace, project)
    {
        m_AccessToken = Guard.NotNullOrWhitespace(accessToken);
    }

    public GitLabProjectConnection(string host, string projectPath, string accessToken) : base(host, projectPath)
    {
        m_AccessToken = Guard.NotNullOrWhitespace(accessToken);
    }

    public GitLabProjectConnection(GitLabProjectIdentity projectIdentity, string accessToken) : base(projectIdentity)
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
    public virtual bool Equals(GitLabProjectConnection? other)
    {
        return other is not null &&
            base.Equals(other) &&
            StringComparer.Ordinal.Equals(AccessToken, other.AccessToken);
    }
}
