using System;
using Cake.Common.Build;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;

namespace Cake.GitLab;

public static partial class GitLabAliases
{
    // TODO: CI_SERVER_HOST is not available though Cake's IGitLabCIProvider => consider opening up a PR
    private const string CI_SERVER_HOST = "CI_SERVER_HOST";

    /// <summary>
    /// Attempts to determine the <see cref="GitLabServerIdentity"/> from environment variables.
    /// </summary>
    /// <remarks>
    /// This alias attempts to determine the current <see cref="GitLabServerIdentity"/> from the environment variables set by GitLab CI.
    /// Thus, this alias will only work if the build is running inside a GitLab CI pipeline.
    /// </remarks>
    /// <param name="context">The current context</param>
    /// <returns>Returns a <see cref="GitLabServerIdentity"/> if it could be determined or <c>null</c> if the server identity could not be determined.</returns>
    /// <seealso href="https://docs.gitlab.com/ee/ci/variables/predefined_variables.html">Predefined CI/CD variables reference (GitLab Documentation)</seealso>
    [CakeMethodAlias]
    [CakeAliasCategory("Helpers")]
    public static GitLabServerIdentity? GitLabTryGetCurrentServerIdentity(this ICakeContext context)
    {
        var log = CreateLog(context);

        log.Debug("Attempting to determine the current GitLab server identity");

        var gitlabCi = context.GitLabCI();
        if (!gitlabCi.IsRunningOnGitLabCI)
        {
            log.Debug("Failed to determine current server identity: Current build is not running on GitLab CI");
            return null;
        }
        log.Debug("Current build is running on GitLab CI");

        var host = context.Environment.GetEnvironmentVariable(CI_SERVER_HOST);
        if (String.IsNullOrWhiteSpace(host))
        {
            log.Debug($"Failed to determine current server identity: {CI_SERVER_HOST} is null or whitespace");
            return null;
        }
        log.Debug($"{CI_SERVER_HOST} is '{host}'");

        return new GitLabServerIdentity(host);
    }

    /// <summary>
    /// Attempts to determine the <see cref="GitLabProjectIdentity"/> from environment variables.
    /// </summary>
    /// <remarks>
    /// This alias attempts to determine the current <see cref="GitLabProjectIdentity"/> from the environment variables set by GitLab CI.
    /// Thus, this alias will only work if the build is running inside a GitLab CI pipeline.
    /// </remarks>
    /// <param name="context">The current context</param>
    /// <returns>Returns a <see cref="GitLabProjectIdentity"/> if it could be determined or <c>null</c> if the identity could not be determined.</returns>
    /// <seealso href="https://docs.gitlab.com/ee/ci/variables/predefined_variables.html">Predefined CI/CD variables reference (GitLab Documentation)</seealso>
    [CakeMethodAlias]
    [CakeAliasCategory("Helpers")]
    public static GitLabProjectIdentity? GitLabTryGetCurrentProjectIdentity(this ICakeContext context)
    {
        var log = CreateLog(context);

        log.Debug("Attempting to determine the current GitLab project identity");

        var gitlabCi = context.GitLabCI();
        if (!gitlabCi.IsRunningOnGitLabCI)
        {
            log.Debug("Failed to determine current project identity: Current build is not running on GitLab CI");
            return null;
        }
        log.Debug("Current build is running on GitLab CI");

        var host = context.Environment.GetEnvironmentVariable(CI_SERVER_HOST);
        if (String.IsNullOrWhiteSpace(host))
        {
            log.Debug($"Failed to determine current project identity: {CI_SERVER_HOST} is null or whitespace");
            return null;
        }
        log.Debug($"{CI_SERVER_HOST} is '{host}'");

        var projectPath = gitlabCi.Environment.Project.Path;
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
        else
        {
            log.Debug($"Failed to determine current project identity: Project path could not be parsed");
            return null;
        }
    }
}
