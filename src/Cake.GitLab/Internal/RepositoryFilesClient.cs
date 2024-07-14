using System;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using NGitLab;
using NGitLab.Models;

namespace Cake.GitLab.Internal;

internal sealed class RepositoryFilesClient(ICakeLog log, IFileSystem fileSystem, IGitLabClient gitLabClient) : ClientBase(log, fileSystem, gitLabClient)
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
}
