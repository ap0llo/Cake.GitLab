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
        m_Log.Verbose($"Downloading {filePath} from GitLab project {project}");

        var repo = m_GitLabClient.GetRepository(project);
        FileData fileData;
        try
        {
            fileData = await repo.Files.GetAsync(filePath, @ref);
        }
        catch (GitLabException ex)
        {
            throw new CakeException($"Failed to download {filePath} at ref {@ref} from GitLab project {project}: {ex.ErrorMessage ?? ex.Message}", ex);
        }

        m_Log.Debug($"Received response from GitLab");

        // There is no case when GitLab returns content in an encoding other than base64 (or at least not documented)
        // To prevent wrong data from being written in case this changes in the future, abort if encoding is unexpected
        if (!StringComparer.Ordinal.Equals(fileData.Encoding, "base64"))
        {
            throw new InvalidOperationException($"Unexpected encoding of file content recevied from GitLab. Expected: base64, Actual: {fileData.Encoding}");
        }

        m_Log.Debug($"Saving file to '{destination}'");
        m_FileSystem.GetDirectory(destination.GetDirectory()).Create();
        using var outStream = m_FileSystem.GetFile(destination).OpenWrite();
        outStream.Write(Convert.FromBase64String(fileData.Content));
    }

    public IReadOnlyCollection<Branch> GetBranches(ProjectId project)
    {
        m_Log.Debug($"Getting branches from GitLab project {project}");

        var repo = m_GitLabClient.GetRepository(project);
        Branch[] branches;
        try
        {
            branches = repo.Branches.All.ToArray();
        }
        catch (GitLabException ex)
        {
            throw new CakeException($"Failed to get branches for from GitLab project {project}: {ex.ErrorMessage ?? ex.Message}", ex);
        }

        return branches;
    }

    public async Task<Tag> CreateTagAsync(ProjectId project, string @ref, string name)
    {
        m_Log.Debug($"Creating tag '{name}' for reference '{@ref}' in GitLab project {project}");

        // 'ref' might be a branch or tag name or abbreviated commit SHA
        // To make comparisons easier, get the commit for the reference (and thus let the GitLab server perform the matching of the ref to a commit)
        var commit = GetCommit(project, @ref);

        // Check if ´the tag already exists with the same target id
        var existingTag = await TryGetTagAsync(project, name);
        if (existingTag?.Commit?.Id == commit.Id)
        {
            m_Log.Debug($"Found existing tag '{name}' for target commit, skipping creation of tag");
            return existingTag;
        }

        Tag tag;
        try
        {
            // Create a new tag using the commit id as reference
            // By using the commit id, we can ensure there is no race-condition between the check if a tag already exists and the creation of the tag.
            // Otherwise, when 'ref' is e.g. a branch name, the commit the ref refers to might change between getting existing tags and creation of new tags
            tag = m_GitLabClient.GetRepository(project).Tags.Create(new TagCreate() { Ref = commit.Id.ToString(), Name = name });
            m_Log.Debug($"Created tag '{tag.Name}' for commit '{tag.Commit.Id}'");
        }

        catch (GitLabException ex)
        {
            throw new CakeException($"Failed to create tag '{name}' for reference '{@ref}' in GitLab project {project}: {ex.ErrorMessage ?? ex.Message}", ex);
        }

        return tag;
    }

    private Commit GetCommit(ProjectId project, string @ref)
    {
        m_Log.Debug($"Getting commit for reference '{@ref}' from GitLab project {project}");

        Commit commit;
        try
        {
            commit = m_GitLabClient.GetCommits(project).GetCommit(@ref);
            m_Log.Debug($"Commit for reference '{@ref}' is {commit.Id}");
        }
        catch (GitLabException ex)
        {
            throw new CakeException($"Failed to get commit for reference '{@ref}' from GitLab project {project}: {ex.ErrorMessage ?? ex.Message}", ex);
        }

        return commit;
    }

    private async Task<Tag?> TryGetTagAsync(ProjectId project, string name)
    {
        m_Log.Debug($"Trying to get tag '{name}' from GitLab project {project}");

        Tag tag;
        try
        {
            tag = await m_GitLabClient.GetRepository(project).Tags.GetByNameAsync(name);
            m_Log.Debug($"Found tag '{tag.Name}' for commit '{tag.Commit.Id}'");
        }
        catch (GitLabException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            m_Log.Debug($"Getting tag '{name}' failed with status code '{ex.StatusCode}' => Tag does not exist");
            return null;
        }
        catch (GitLabException ex)
        {
            throw new CakeException($"Failed to get tag '{name}' from GitLab project {project}: {ex.ErrorMessage ?? ex.Message}", ex);
        }

        return tag;
    }
}
