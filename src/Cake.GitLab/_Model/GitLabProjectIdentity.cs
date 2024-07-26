// Derived from https://github.com/ap0llo/changelog/blob/97d0d3111510dae9f4222704a50d88938be8debe/src/ChangeLog/Integrations/GitLab/GitLabProjectInfo.cs
// Licensed under the MIT License

using System;
using Cake.GitLab.Internal;

namespace Cake.GitLab;

/// <summary>
/// Encapsualtes the identity of a project on a GitLab server
/// </summary>
public sealed record GitLabProjectIdentity : IEquatable<GitLabProjectIdentity>
{
    private readonly string m_Host;
    private readonly string m_Namespace;
    private readonly string m_Project;

    /// <summary>
    /// Gets of sets the host name of the GitLab server.
    /// </summary>
    public string Host
    {
        get => m_Host;
        init => m_Host = Guard.NotNullOrWhitespace(value);
    }

    /// <summary>
    /// Gets or sets the project namespace (the user or group (incl. subgroups) the project belongs to).
    /// </summary>
    /// <remarks>
    /// Setting the namespace will update the <see cref="ProjectPath"/> property.
    /// </remarks>
    public string Namespace
    {
        get => m_Namespace;
        init => m_Namespace = Guard.NotNullOrWhitespace(value);
    }

    /// <summary>
    /// Gets or sets the project name.
    /// </summary>
    /// <remarks>
    /// Setting the project name will update the <see cref="ProjectPath"/> property.
    /// </remarks>
    public string Project
    {
        get => m_Project;
        init => m_Project = Guard.NotNullOrWhitespace(value);
    }

    /// <summary>
    /// Gets or sets the GitLab project path (namespace and project name).
    /// </summary>
    /// <remarks>
    /// Updating the project path will update both <see cref="Namespace"/> and <see cref="Project"/> properties.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when the value is not a valid project namespace.</exception>
    public string ProjectPath
    {
        get => $"{Namespace}/{Project}";
        init => (m_Namespace, m_Project) = ParseProjectPath(value);
    }


    /// <summary>
    /// Initializes a new instance of <see cref="GitLabProjectIdentity"/>
    /// </summary>
    /// <param name="host">The host name of the GitLab server.</param>
    /// <param name="namespace">The GitLab project's namespace (user name or group and subgroup)</param>
    /// <param name="project">The GitLab project's name</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="host"/>, <paramref name="namespace"/> or <paramref name="project"/> is null or whitespace</exception>
    public GitLabProjectIdentity(string host, string @namespace, string project)
    {
        m_Host = Guard.NotNullOrWhitespace(host);
        m_Namespace = Guard.NotNullOrWhitespace(@namespace);
        m_Project = Guard.NotNullOrWhitespace(project);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hash = StringComparer.OrdinalIgnoreCase.GetHashCode(Host) * 397;
            hash ^= StringComparer.OrdinalIgnoreCase.GetHashCode(Namespace);
            hash ^= StringComparer.OrdinalIgnoreCase.GetHashCode(Project);
            return hash;
        }
    }

    /// <inheritdoc />
    public bool Equals(GitLabProjectIdentity? other)
    {
        return other is not null &&
            StringComparer.OrdinalIgnoreCase.Equals(Host, other.Host) &&
            StringComparer.OrdinalIgnoreCase.Equals(Namespace, other.Namespace) &&
            StringComparer.OrdinalIgnoreCase.Equals(Project, other.Project);
    }


    private (string @namespace, string project) ParseProjectPath(string projectPath)
    {
        projectPath = Guard.NotNullOrWhitespace(projectPath);

        if (!projectPath.Contains('/'))
        {
            throw new ArgumentException($"Cannot parse '{projectPath}' as GitLab project path. Expected the path to contain at least one '/' character", nameof(projectPath));
        }

        if (projectPath.StartsWith('/') || projectPath.EndsWith("/"))
        {
            throw new ArgumentException($"Cannot parse '{projectPath}' as GitLab project path. Path must not start or end with a '/' character", nameof(projectPath));
        }

        var splitIndex = projectPath.LastIndexOf('/');
        var @namespace = projectPath.Substring(0, splitIndex);
        var project = projectPath.Substring(splitIndex + 1);

        if (String.IsNullOrWhiteSpace(@namespace))
        {
            throw new ArgumentException($"Cannot parse '{projectPath}' as GitLab project path. Project namespace is empty", nameof(projectPath));
        }

        if (String.IsNullOrWhiteSpace(project))
        {
            throw new ArgumentException($"Cannot parse '{projectPath}' as GitLab project path. Project name is empty", nameof(projectPath));
        }

        return (@namespace, project);
    }
}
