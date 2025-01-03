using System;
using Cake.GitLab.Internal;

namespace Cake.GitLab;

/// <summary>
/// Provides both the identity and credentials for a GitLab project
/// </summary>
public record ProjectConnection : ProjectIdentity
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

    public new ServerConnection Server => new ServerConnection(base.Server, m_AccessToken);

    public ProjectConnection(ProjectIdentity projectIdentity, string accessToken) : base(projectIdentity)
    {
        m_AccessToken = Guard.NotNullOrWhitespace(accessToken);
    }

    public ProjectConnection(ServerIdentity server, string @namespace, string project, string accessToken) : base(server, @namespace, project)
    {
        m_AccessToken = Guard.NotNullOrWhitespace(accessToken);
    }

    public ProjectConnection(ServerIdentity server, string projectPath, string accessToken) : base(server, projectPath)
    {
        m_AccessToken = Guard.NotNullOrWhitespace(accessToken);
    }


    public ProjectConnection(ServerConnection server, string @namespace, string project) : base(server, @namespace, project)
    {
        m_AccessToken = server.AccessToken;
    }

    public ProjectConnection(ServerConnection server, string projectPath) : base(server, projectPath)
    {
        m_AccessToken = server.AccessToken;
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
    public virtual bool Equals(ProjectConnection? other)
    {
        return other is not null &&
               base.Equals(other) &&
               StringComparer.Ordinal.Equals(AccessToken, other.AccessToken);
    }
}
