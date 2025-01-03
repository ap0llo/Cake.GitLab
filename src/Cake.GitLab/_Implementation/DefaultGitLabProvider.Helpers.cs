using System;
using Cake.Common.Build;
using Cake.Core.Diagnostics;
using Cake.GitLab.Internal;

namespace Cake.GitLab;

public partial class DefaultGitLabProvider
{
    // CI_SERVER_URL is not available though Cake's IGitLabCIProvider, see https://github.com/cake-build/cake/issues/4418
    private const string CI_SERVER_URL = "CI_SERVER_URL";

    /// <inheritdoc />
    public ServerIdentity? TryGetCurrentServerIdentity()
    {
        var log = GetLogForCurrentOperation();
        return TryGetCurrentServerIdentity(log);
    }

    /// <inheritdoc />
    public ProjectIdentity? TryGetCurrentProjectIdentity()
    {
        var log = GetLogForCurrentOperation();

        log.Debug("Attempting to determine the current GitLab project identity");

        if (!IsRunningOnGitLabCI)
        {
            log.Debug("Failed to determine current project identity: Current build is not running on GitLab CI");
            return null;
        }

        log.Debug("Current build is running on GitLab CI");

        if (TryGetCurrentServerIdentity(log) is not { } server)
        {
            return null;
        }

        var projectPath = m_Context.GitLabCI().Environment.Project.Path;
        if (String.IsNullOrWhiteSpace(projectPath))
        {
            log.Debug("Failed to determine current project identity: Project path is null or whitespace");
            return null;
        }

        log.Debug($"Project path is '{projectPath}'");

        if (ProjectIdentity.TryGetFromServerAndProjectPath(server, projectPath, out var identity))
        {
            return identity;
        }

        log.Debug($"Failed to determine current project identity: Project path could not be parsed");
        return null;
    }

    private ServerIdentity? TryGetCurrentServerIdentity(DebugLog log)
    {
        log.Debug("Attempting to determine the current GitLab server identity");

        if (!IsRunningOnGitLabCI)
        {
            log.Debug("Failed to determine current server identity: Current build is not running on GitLab CI");
            return null;
        }

        log.Debug("Current build is running on GitLab CI");

        var serverUrl = m_Context.Environment.GetEnvironmentVariable(CI_SERVER_URL);
        if (String.IsNullOrWhiteSpace(serverUrl))
        {
            log.Debug($"Failed to determine current server identity: {CI_SERVER_URL} is null or whitespace");
            return null;
        }

        log.Debug($"{CI_SERVER_URL} is '{serverUrl}'");

        return ServerIdentity.FromUrl(serverUrl);
    }
}
