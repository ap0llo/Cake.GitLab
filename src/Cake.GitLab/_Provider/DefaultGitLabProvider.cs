using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

    /// <inheritdoc />
    public async Task<Pipeline> GetPipelineAsync(string serverUrl, string accessToken, ProjectId project, int pipelineId)
    {
        var log = GetLogForCurrentOperation();
        log.Verbose($"Getting pipeline {pipelineId} from GitLab project {project}");

        var client = GetClient(serverUrl, accessToken);

        try
        {
            var pipeline = await client.GetPipelines(project).GetByIdAsync(pipelineId);
            return pipeline;
        }
        catch (GitLabException ex)
        {
            throw new CakeException($"Error while getting pipeline {pipelineId} from GitLab project {project}: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task SetPipelineNameAsync(string serverUrl, string accessToken, ProjectId project, int pipelineId, string name)
    {
        var log = GetLogForCurrentOperation();
        log.Verbose($"Setting name of pipeline {pipelineId} in GitLab project {project} to {name}");

        var client = GetClient(serverUrl, accessToken);
        var pipelinesClient = client.GetPipelines(project);
        try
        {
            await pipelinesClient.UpdateMetadataAsync(pipelineId, new PipelineMetadataUpdate() { Name = name });
        }
        catch (GitLabException ex)
        {
            throw new CakeException($"Error while setting name of pipeline {pipelineId} in GitLab project {project}: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task RepositoryDownloadFileAsync(string serverUrl, string accessToken, ProjectId project, string filePath, string @ref, FilePath destination)
    {
        Guard.NotNullOrWhitespace(filePath);
        Guard.NotNullOrWhitespace(@ref);
        Guard.NotNull(destination);

        var log = GetLogForCurrentOperation();
        log.Verbose($"Downloading {filePath} from GitLab project {project} at reference '{@ref}' to '{destination}'");

        var client = GetClient(serverUrl, accessToken);
        var repo = client.GetRepository(project);
        FileData fileData;
        try
        {
            log.Debug("Getting file from GitLab");
            fileData = await repo.Files.GetAsync(filePath, @ref);
            log.Debug("Received response from GitLab");
        }
        catch (GitLabException ex)
        {
            throw new CakeException($"Failed to download {filePath} at ref {@ref} from GitLab project {project}: {ex.ErrorMessage ?? ex.Message}", ex);
        }

        // There is no case when GitLab returns content in an encoding other than base64 (or at least not documented)
        // To prevent wrong data from being written in case this changes in the future, abort if encoding is unexpected
        if (!StringComparer.Ordinal.Equals(fileData.Encoding, "base64"))
        {
            throw new InvalidOperationException($"Unexpected encoding of file content recevied from GitLab. Expected: base64, Actual: {fileData.Encoding}");
        }

        var destinationDirectory = destination.GetDirectory();
        log.Debug($"Creating destination directory '{destination}'");
        m_Context.FileSystem.GetDirectory(destinationDirectory).Create();

        log.Debug($"Writing contents to destination file '{destination}'");
        using var outStream = m_Context.FileSystem.GetFile(destination).OpenWrite();
        await outStream.WriteAsync(Convert.FromBase64String(fileData.Content));

        log.Verbose("File downloaded successfully");
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<Branch>> RepositoryGetBranchesAsync(string serverUrl, string accessToken, ProjectId project)
    {
        var log = GetLogForCurrentOperation();
        log.Verbose($"Getting branches from GitLab project {project}");

        var client = GetClient(serverUrl, accessToken);
        var repo = client.GetRepository(project);
        Branch[] branches;
        try
        {
            branches = repo.Branches.All.ToArray();
        }
        catch (GitLabException ex)
        {
            throw new CakeException($"Failed to get branches from GitLab project {project}: {ex.ErrorMessage ?? ex.Message}", ex);
        }

        log.Verbose($"Loaded data for {branches.Length} branches");
        return Task.FromResult<IReadOnlyCollection<Branch>>(branches);
    }

    /// <inheritdoc />
    public async Task<Tag> RepositoryCreateTagAsync(string serverUrl, string accessToken, ProjectId project, string @ref, string name)
    {
        Guard.NotNullOrWhitespace(@ref);
        Guard.NotNullOrWhitespace(name);


        var log = GetLogForCurrentOperation();
        log.Verbose($"Creating tag '{name}' for reference '{@ref}' in GitLab project {project}");

        var client = GetClient(serverUrl, accessToken);

        // 'ref' might be a branch or tag name or abbreviated commit SHA
        // To make comparisons easier, get the commit for the reference (and thus let the GitLab server perform the matching of the ref to a commit)
        var commit = GetCommitInternal(log, client, project, @ref);

        // Check if ´the tag already exists with the same target id
        var existingTag = await TryGetTagInternalAsync(log, client, project, name);
        if (existingTag?.Commit?.Id == commit.Id)
        {
            log.Verbose($"Tag '{existingTag.Name}' for commit {existingTag.Commit.Id} already exists");
            return existingTag;
        }

        Tag tag;
        try
        {
            // Create a new tag using the commit id as reference
            // By using the commit id, we can ensure there is no race-condition between the check if a tag already exists and the creation of the tag.
            // Otherwise, when 'ref' is e.g. a branch name, the commit the ref refers to might change between getting existing tags and creation of new tags
            var tagCreate = new TagCreate() { Ref = commit.Id.ToString(), Name = name };
            log.Debug($"Creating tag '{tagCreate.Name}' for reference '{tagCreate.Ref}'");
            tag = client.GetRepository(project).Tags.Create(tagCreate);
        }
        catch (GitLabException ex)
        {
            throw new CakeException($"Failed to create tag '{name}' for reference '{@ref}' in GitLab project {project}: {ex.ErrorMessage ?? ex.Message}", ex);
        }

        log.Verbose($"Tag '{tag.Name}' for commit {tag.Commit.Id} created successfully");
        return tag;
    }

    protected virtual IGitLabClient GetClient(string serverUrl, string accessToken, [CallerMemberName] string operationName = "")
    {
        Guard.NotNullOrWhitespace(serverUrl);
        Guard.NotNullOrWhitespace(accessToken);

        var log = GetLogForCurrentOperation(operationName);

        log.Debug($"Creating GitLab client for server url '{serverUrl}'");

        IGitLabClientFactory clientFactory;
        if (m_Context is IGitLabClientFactory)
        {
            log.Debug($"Context of type '{m_Context.GetType().FullName}' implements {nameof(IGitLabClientFactory)}. Delegating client creation to context");
            clientFactory = (IGitLabClientFactory)m_Context;
        }
        else
        {
            log.Debug($"Creating default GitLab client {typeof(GitLabClient).FullName}");
            clientFactory = GitLabClientFactory.Default;
        }

        return clientFactory.GetClient(serverUrl, accessToken);
    }

    private DebugLog GetLogForCurrentOperation([CallerMemberName] string operationName = "") => new DebugLog(m_Context.Log, $"GitLab.{operationName}");

    private Commit GetCommitInternal(ICakeLog log, IGitLabClient client, ProjectId project, string @ref)
    {
        Commit commit;
        try
        {
            log.Debug($"Getting commit for reference '{@ref}'");
            commit = client.GetCommits(project).GetCommit(@ref);
            log.Debug($"Commit for reference '{@ref}' is {commit.Id}");
        }
        catch (GitLabException ex)
        {
            throw new CakeException($"Failed to get commit for reference '{@ref}' from GitLab project {project}: {ex.ErrorMessage ?? ex.Message}", ex);
        }

        return commit;
    }

    private async Task<Tag?> TryGetTagInternalAsync(ICakeLog log, IGitLabClient client, ProjectId project, string name)
    {
        Tag tag;
        try
        {
            log.Debug($"Attempting to get tag '{name}'");
            tag = await client.GetRepository(project).Tags.GetByNameAsync(name);
            log.Debug($"Found tag '{tag.Name}' for commit '{tag.Commit.Id}'");
        }
        catch (GitLabException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            log.Debug($"Tag '{name}' does not exist. Getting tag failed with status code '{ex.StatusCode}'");
            return null;
        }
        catch (GitLabException ex)
        {
            throw new CakeException($"Failed to get tag '{name}' from GitLab project {project}: {ex.ErrorMessage ?? ex.Message}", ex);
        }

        return tag;
    }
}
