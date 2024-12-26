using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cake.Common.Build;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.GitLab.Internal;
using NGitLab;
using NGitLab.Models;

namespace Cake.GitLab;

public class DefaultGitLabProvider : IGitLabProvider
{
    // TODO: CI_SERVER_HOST is not available though Cake's IGitLabCIProvider => consider opening up a PR
    private const string CI_SERVER_HOST = "CI_SERVER_HOST";

    private readonly ICakeContext m_Context;

    /// <summary>
    /// Determines whether the current build is running in GitLab CI
    /// </summary>
    private bool IsRunningOnGitLabCI => m_Context.GitLabCI().IsRunningOnGitLabCI;

    /// <summary>
    /// Initializes the default implementation of <see cref="IGitLabProvider"/> from a build context
    /// </summary>
    public DefaultGitLabProvider(ICakeContext context)
    {
        m_Context = Guard.NotNull(context);
    }



    public GitLabServerIdentity? TryGetCurrentServerIdentity()
    {
        var log = GetLogForCurrentAlias();

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


    public GitLabProjectIdentity? TryGetCurrentProjectIdentity()
    {
        var log = GetLogForCurrentAlias();

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


    public async Task<Pipeline> GetPipelineAsync(string serverUrl, string accessToken, ProjectId project, int pipelineId)
    {
        var pipelinesClient = GetPipelinesClient(m_Context, serverUrl, accessToken);
        return await pipelinesClient.GetPipelineAsync(project, pipelineId);
    }

    public async Task SetPipelineNameAsync(string serverUrl, string accessToken, ProjectId project, int pipelineId, string name)
    {
        var pipelinesClient = GetPipelinesClient(m_Context, serverUrl, accessToken);
        await pipelinesClient.SetPipelineNameAsync(project, pipelineId, name);
    }

    public async Task RepositoryDownloadFileAsync(string serverUrl, string accessToken, ProjectId project, string filePath, string @ref, FilePath destination)
    {
        Guard.NotNullOrWhitespace(filePath);
        Guard.NotNullOrWhitespace(@ref);
        Guard.NotNull(destination);

        var repositoryClient = GetRepositoryClient(m_Context, serverUrl, accessToken);
        await repositoryClient.DownloadFileAsync(project, filePath, @ref, destination);
    }

    public async Task<IReadOnlyCollection<Branch>> RepositoryGetBranchesAsync(string serverUrl, string accessToken, ProjectId project)
    {
        var repositoryClient = GetRepositoryClient(m_Context, serverUrl, accessToken);
        return await repositoryClient.GetBranchesAsync(project);
    }

    public async Task<Tag> RepositoryCreateTagAsync(string serverUrl, string accessToken, ProjectId project, string @ref, string name)
    {
        Guard.NotNullOrWhitespace(@ref);
        Guard.NotNullOrWhitespace(name);

        var repositoryClient = GetRepositoryClient(m_Context, serverUrl, accessToken);
        return await repositoryClient.CreateTagAsync(project, @ref, name);
    }

    private DebugLog GetLogForCurrentAlias([CallerMemberName] string methodName = "") => new DebugLog(m_Context.Log, $"GitLab.{methodName}");


    //TODO: Merge PipelinesClient and Provider classey
    private static PipelinesClient GetPipelinesClient(ICakeContext context, string serverUrl, string accessToken, [CallerMemberName] string aliasName = "")
    {
        var gitLabClient = GetClient(context, serverUrl, accessToken);
        var log = GetLogForCurrentAlias(context, aliasName);
        var pipelinesClient = new PipelinesClient(log, context.FileSystem, gitLabClient);
        return pipelinesClient;
    }

    private static IGitLabClient GetClient(ICakeContext context, string serverUrl, string accessToken, [CallerMemberName] string aliasName = "")
    {
        Guard.NotNullOrWhitespace(serverUrl);
        Guard.NotNullOrWhitespace(accessToken);

        var log = GetLogForCurrentAlias(context, aliasName);

        log.Debug($"Creating GitLab client for server url '{serverUrl}'");

        IGitLabClientFactory clientFactory;
        if (context is IGitLabClientFactory)
        {
            log.Debug($"Context of type '{context.GetType().FullName}' implements {nameof(IGitLabClientFactory)}. Delegating client creation to context");
            clientFactory = (IGitLabClientFactory)context;
        }
        else
        {
            log.Debug($"Creating default GitLab client {typeof(GitLabClient).FullName}");
            clientFactory = GitLabClientFactory.Default;
        }

        return clientFactory.GetClient(serverUrl, accessToken);
    }

    private static DebugLog GetLogForCurrentAlias(ICakeContext context, [CallerMemberName] string aliasName = "") => new DebugLog(context.Log, aliasName);


    private static RepositoryClient GetRepositoryClient(ICakeContext context, string serverUrl, string accessToken, [CallerMemberName] string aliasName = "")
    {
        var gitLabClient = GetClient(context, serverUrl, accessToken);
        var log = GetLogForCurrentAlias(context, aliasName);
        var repositoryClient = new RepositoryClient(log, context.FileSystem, gitLabClient);
        return repositoryClient;
    }

}
