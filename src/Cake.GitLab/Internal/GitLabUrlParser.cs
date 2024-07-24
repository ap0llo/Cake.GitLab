// Derived from https://github.com/ap0llo/changelog/blob/97d0d3111510dae9f4222704a50d88938be8debe/src/ChangeLog/Integrations/GitLab/GitLabUrlParser.cs
// Licensed under the MIT License

using System;
using Cake.Core.Diagnostics;

namespace Cake.GitLab.Internal;

internal class GitLabUrlParser
{
    private readonly ICakeLog m_Log;

    public GitLabUrlParser(ICakeLog log)
    {
        m_Log = log;
    }

    public GitLabProjectInfo? GetProjectFromRemoteUrl(string remoteUrl)
    {
        if (!GitUrl.TryGetUri(remoteUrl, out var uri))
        {
            m_Log.Debug($"Value '{remoteUrl}' is not a valid uri");
            return null;
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

                if (String.IsNullOrWhiteSpace(projectPath))
                {
                    m_Log.Debug($"Cannot parse '{remoteUrl}' as GitLab url: Project path is empty");
                    return null;
                }

                if (!projectPath.Contains('/'))
                {
                    m_Log.Debug($"Cannot parse '{remoteUrl}' as GitLab url: Invalid project path '{projectPath}'");
                    return null;
                }

                var splitIndex = projectPath.LastIndexOf('/');
                var @namespace = projectPath.Substring(0, splitIndex);
                var projectName = projectPath.Substring(splitIndex);

                if (String.IsNullOrWhiteSpace(@namespace))
                {
                    m_Log.Debug($"Cannot parse '{remoteUrl}' as GitLab url: Project namespace is empty");
                    return null;
                }

                if (String.IsNullOrWhiteSpace(projectName))
                {
                    m_Log.Debug($"Cannot parse '{remoteUrl}' as GitLab url: Project name is empty");
                    return null; ;
                }

                return new GitLabProjectInfo(uri.Host, @namespace, projectName);

            default:
                m_Log.Debug($"Cannot parse '{remoteUrl}' as GitLab url: Unsupported scheme '{uri.Scheme}'");
                return null;
        }
    }
}
