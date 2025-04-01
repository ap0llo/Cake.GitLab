using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.GitLab.Internal;
using NGitLab;
using NGitLab.Models;

namespace Cake.GitLab;

public partial class DefaultGitLabProvider
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> RepositoryGetFilesAsync(string serverUrl, string accessToken, ProjectId project, string @ref)
    {
        var log = GetLogForCurrentOperation();
        log.Verbose($"Getting all files for reference '{@ref}' in GitLab project {project}");
        return await GetFilesInternal(log, serverUrl, accessToken, project, @ref, null);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> RepositoryGetFilesAsync(string serverUrl, string accessToken, ProjectId project, string @ref, string path)
    {
        var log = GetLogForCurrentOperation();
        log.Verbose($"Getting all files in directory '{path}' for reference '{@ref}' in GitLab project {project}");
        return await GetFilesInternal(log, serverUrl, accessToken, project, @ref, path);
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

        // Check if the tag already exists with the same target id
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

    private async Task<IReadOnlyList<string>> GetFilesInternal(ICakeLog log, string serverUrl, string accessToken, ProjectId project, string @ref, string? path)
    {
        var client = GetClient(serverUrl, accessToken);
        var repo = client.GetRepository(project);

        var options = new RepositoryGetTreeOptions()
        {
            Ref = @ref,
            Recursive = true,
        };

        if (path is not null)
        {
            options.Path = path;
        }

        var files = new List<string>();

        try
        {
            log.Debug($"Getting tree with options {nameof(options.Ref)} = {options.Ref}, {nameof(options.Recursive)} = {options.Recursive}{(path is null ? "" : $", {nameof(options.Path)} = {options.Path}")}");
            await foreach (var tree in repo.GetTreeAsync(options))
            {
                if (tree.Type == ObjectType.blob)
                {
                    files.Add(tree.Path);
                }
            }
        }
        catch (GitLabException ex)
        {
            throw new CakeException($"Failed to get list of files at ref {@ref} from GitLab project {project}: {ex.ErrorMessage ?? ex.Message}", ex);
        }

        log.Debug($"Found {files.Count} files");
        return files;
    }
}
