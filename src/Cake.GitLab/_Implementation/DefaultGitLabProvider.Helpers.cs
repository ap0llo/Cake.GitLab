using System;
using Cake.Common.Build;
using Cake.Core.Diagnostics;

namespace Cake.GitLab;

public partial class DefaultGitLabProvider
{
    // TODO: CI_SERVER_HOST is not available though Cake's IGitLabCIProvider => consider opening up a PR
    private const string CI_SERVER_HOST = "CI_SERVER_HOST";


    /// <inheritdoc />
    public GitLabServerIdentity? TryGetCurrentServerIdentity()
    {
        var log = GetLogForCurrentOperation();

        log.Debug("Attempting to determine the current GitLab server identity");

        if (!IsRunningOnGitLabCI)
        {
            log.Debug("Failed to determine current server identity: Current build is not running on GitLab CI");
            return null;
        }

        log.Debug("Current build is running on GitLab CI");

        var host = m_Context.Environment.GetEnvironmentVariable(CI_SERVER_HOST);
        if (String.IsNullOrWhiteSpace(host))
        {
            log.Debug($"Failed to determine current server identity: {CI_SERVER_HOST} is null or whitespace");
            return null;
        }

        log.Debug($"{CI_SERVER_HOST} is '{host}'");

        return new GitLabServerIdentity(host);
    }

    /// <inheritdoc />
    public GitLabProjectIdentity? TryGetCurrentProjectIdentity()
    {
        var log = GetLogForCurrentOperation();

        log.Debug("Attempting to determine the current GitLab project identity");

        if (!IsRunningOnGitLabCI)
        {
            log.Debug("Failed to determine current project identity: Current build is not running on GitLab CI");
            return null;
        }

        log.Debug("Current build is running on GitLab CI");

        var host = m_Context.Environment.GetEnvironmentVariable(CI_SERVER_HOST);
        if (String.IsNullOrWhiteSpace(host))
        {
            log.Debug($"Failed to determine current project identity: {CI_SERVER_HOST} is null or whitespace");
            return null;
        }

        log.Debug($"{CI_SERVER_HOST} is '{host}'");

        var projectPath = m_Context.GitLabCI().Environment.Project.Path;
        if (String.IsNullOrWhiteSpace(projectPath))
        {
            log.Debug($"Failed to determine current project identity: Project path is null or whitespace");
            return null;
        }

        log.Debug($"Project path is '{projectPath}'");


        if (GitLabProjectIdentity.TryGetFromHostAndProjectPath(host, projectPath, out var identity))
        {
            return identity;
        }

        log.Debug($"Failed to determine current project identity: Project path could not be parsed");
        return null;
    }
}
