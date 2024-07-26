//
// Derived from:
//  - https://github.com/ap0llo/changelog/blob/97d0d3111510dae9f4222704a50d88938be8debe/src/ChangeLog/Integrations/GitLab/GitLabProjectIdentity.cs
//  - https://github.com/ap0llo/changelog/blob/97d0d3111510dae9f4222704a50d88938be8debe/src/ChangeLog/Integrations/GitLab/GitLabUrlParser.cs
//
// Licensed under the MIT License
//

using System;
using System.Diagnostics.CodeAnalysis;
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
        init
        {
            if (!TryParseProjectPath(value, out var @namespace, out var project, out var error))
            {
                throw new ArgumentException(error, nameof(value));
            }
            (m_Namespace, m_Project) = (@namespace, project);
        }
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


    /// <summary>
    /// Determines the GitLab server and project path based on a git repository's remote url
    /// </summary>
    /// <param name="remoteUrl">A git remote url. Supports both HTTP and SSH urls (including urls in the SCP format, e.g. <c>git@github.com:ap0llo/Cake.GitLab.git</c>)</param>
    /// <returns>Returns a <see cref="GitLabProjectIdentity"/> with the information extracted from the url or <c>null</c> if not project data could be read from the remote url.</returns>
    public static GitLabProjectIdentity FromGitRemoteUrl(string remoteUrl)
    {
        Guard.NotNullOrWhitespace(remoteUrl);

        if (TryParseRemoteUrl(remoteUrl, out var projectInfo, out var errorMessage))
        {
            return projectInfo;
        }
        else
        {
            throw new ArgumentException(errorMessage, nameof(remoteUrl));
        }
    }

    /// <summary>
    /// Attempts to determine the GitLab server and project path based on a git repository's remote url
    /// </summary>    
    public static bool TryGetFromGitRemoteUrl(string remoteUrl, [NotNullWhen(true)] out GitLabProjectIdentity? projectIdentity) =>
        TryParseRemoteUrl(remoteUrl, out projectIdentity, out var _);


    private static bool TryParseProjectPath(string projectPath, [NotNullWhen(true)] out string? @namespace, [NotNullWhen(true)] out string? project, [NotNullWhen(false)] out string? errorMessage)
    {
        @namespace = null;
        project = null;

        if (String.IsNullOrWhiteSpace(projectPath))
        {
            errorMessage = "Value must not be null or whitespace";
            return false;
        }

        if (!projectPath.Contains('/'))
        {
            errorMessage = $"Cannot parse '{projectPath}' as GitLab project path. Expected the path to contain at least one '/' character";
            return false;
        }

        if (projectPath.StartsWith('/') || projectPath.EndsWith("/"))
        {
            errorMessage = $"Cannot parse '{projectPath}' as GitLab project path. Path must not start or end with a '/' character";
            return false;
        }

        var splitIndex = projectPath.LastIndexOf('/');
        @namespace = projectPath.Substring(0, splitIndex);
        project = projectPath.Substring(splitIndex + 1);

        if (String.IsNullOrWhiteSpace(@namespace))
        {
            errorMessage = $"Cannot parse '{projectPath}' as GitLab project path. Project namespace is empty";
        }

        if (String.IsNullOrWhiteSpace(project))
        {
            errorMessage = $"Cannot parse '{projectPath}' as GitLab project path. Project name is empty";
        }

        errorMessage = null;
        return true;
    }

    private static bool TryParseRemoteUrl(string url, [NotNullWhen(true)] out GitLabProjectIdentity? projectInfo, [NotNullWhen(false)] out string? errorMessage)
    {
        projectInfo = null;
        errorMessage = null;

        if (String.IsNullOrWhiteSpace(url))
        {
            errorMessage = "Value must not be null or empty";
            return false;
        }

        if (!GitUrl.TryGetUri(url, out var uri))
        {
            errorMessage = $"Value '{url}' is not a valid uri";
            return false;
        }

        switch (uri.Scheme.ToLower())
        {
            case "http":
            case "https":
            case "ssh":
                var projectPath = uri.AbsolutePath.Trim('/');

                if (projectPath.EndsWith(".git"))
                {
                    projectPath = projectPath.Substring(0, projectPath.Length - ".git".Length);
                }


                if (!TryParseProjectPath(projectPath, out var @namespace, out var project, out errorMessage))
                {
                    return false;
                }
                if (String.IsNullOrWhiteSpace(projectPath))
                {
                    errorMessage = $"Cannot parse '{url}' as GitLab url: Project path is empty";
                    return false;
                }

                projectInfo = new GitLabProjectIdentity(uri.Host, @namespace, project);
                return true;

            default:
                errorMessage = $"Cannot parse '{url}' as GitLab url: Unsupported scheme '{uri.Scheme}'";
                return false;
        }
    }
}
