using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using NGitLab;
using NGitLab.Models;

namespace Cake.GitLab.Internal;

internal sealed class RepositoryClient(ICakeLog log, IFileSystem fileSystem, IGitLabClient gitLabClient) : ClientBase(log, fileSystem, gitLabClient)
{
    public async Task DownloadFileAsync(ProjectId project, string filePath, string @ref, FilePath destination)
    {
        m_Log.Verbose($"Downloading {filePath} from GitLab project {project} at reference '{@ref}' to '{destination}'");

        var repo = m_GitLabClient.GetRepository(project);
        FileData fileData;
        try
        {
            m_Log.Debug("Getting file from GitLab");
            fileData = await repo.Files.GetAsync(filePath, @ref);
            m_Log.Debug("Received response from GitLab");
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
        m_Log.Debug($"Creating destination directory '{destination}'");
        m_FileSystem.GetDirectory(destinationDirectory).Create();

        m_Log.Debug($"Writing contents to destination file '{destination}'");
        using var outStream = m_FileSystem.GetFile(destination).OpenWrite();
        await outStream.WriteAsync(Convert.FromBase64String(fileData.Content));

        m_Log.Verbose("File downloaded sucessfully");
    }

    public ValueTask<IReadOnlyCollection<Branch>> GetBranchesAsync(ProjectId project)
    {
        m_Log.Verbose($"Getting branches from GitLab project {project}");

        var repo = m_GitLabClient.GetRepository(project);
        Branch[] branches;
        try
        {
            branches = repo.Branches.All.ToArray();
        }
        catch (GitLabException ex)
        {
            throw new CakeException($"Failed to get branches from GitLab project {project}: {ex.ErrorMessage ?? ex.Message}", ex);
        }

        m_Log.Verbose($"Loaded data for {branches.Length} branches");
        return new ValueTask<IReadOnlyCollection<Branch>>(branches);
    }

    public async Task<Tag> CreateTagAsync(ProjectId project, string @ref, string name)
    {
        m_Log.Verbose($"Creating tag '{name}' for reference '{@ref}' in GitLab project {project}");

        // 'ref' might be a branch or tag name or abbreviated commit SHA
        // To make comparisons easier, get the commit for the reference (and thus let the GitLab server perform the matching of the ref to a commit)
        var commit = GetCommitInternal(project, @ref);

        // Check if ´the tag already exists with the same target id
        var existingTag = await TryGetTagInternalAsync(project, name);
        if (existingTag?.Commit?.Id == commit.Id)
        {
            m_Log.Verbose($"Tag '{existingTag.Name}' for commit {existingTag.Commit.Id} already exists");
            return existingTag;
        }

        Tag tag;
        try
        {
            // Create a new tag using the commit id as reference
            // By using the commit id, we can ensure there is no race-condition between the check if a tag already exists and the creation of the tag.
            // Otherwise, when 'ref' is e.g. a branch name, the commit the ref refers to might change between getting existing tags and creation of new tags
            var tagCreate = new TagCreate() { Ref = commit.Id.ToString(), Name = name };
            m_Log.Debug($"Creating tag '{tagCreate.Name}' for reference '{tagCreate.Ref}'");
            tag = m_GitLabClient.GetRepository(project).Tags.Create(tagCreate);
        }

        catch (GitLabException ex)
        {
            throw new CakeException($"Failed to create tag '{name}' for reference '{@ref}' in GitLab project {project}: {ex.ErrorMessage ?? ex.Message}", ex);
        }

        m_Log.Verbose($"Tag '{tag.Name}' for commit {tag.Commit.Id} created successfully");
        return tag;
    }

    private Commit GetCommitInternal(ProjectId project, string @ref)
    {
        Commit commit;
        try
        {
            m_Log.Debug($"Getting commit for reference '{@ref}'");
            commit = m_GitLabClient.GetCommits(project).GetCommit(@ref);
            m_Log.Debug($"Commit for reference '{@ref}' is {commit.Id}");
        }
        catch (GitLabException ex)
        {
            throw new CakeException($"Failed to get commit for reference '{@ref}' from GitLab project {project}: {ex.ErrorMessage ?? ex.Message}", ex);
        }

        return commit;
    }

    private async Task<Tag?> TryGetTagInternalAsync(ProjectId project, string name)
    {
        Tag tag;
        try
        {
            m_Log.Debug($"Attempting to get tag '{name}'");
            tag = await m_GitLabClient.GetRepository(project).Tags.GetByNameAsync(name);
            m_Log.Debug($"Found tag '{tag.Name}' for commit '{tag.Commit.Id}'");
        }
        catch (GitLabException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            m_Log.Debug($"Tag '{name}' does not exist. Getting tag failed with status code '{ex.StatusCode}'");
            return null;
        }
        catch (GitLabException ex)
        {
            throw new CakeException($"Failed to get tag '{name}' from GitLab project {project}: {ex.ErrorMessage ?? ex.Message}", ex);
        }

        return tag;
    }
}
