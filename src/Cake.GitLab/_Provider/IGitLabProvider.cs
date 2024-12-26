using System.Collections.Generic;
using System.Threading.Tasks;
using Cake.Core.IO;
using NGitLab.Models;

namespace Cake.GitLab;

public interface IGitLabProvider
{
    GitLabServerIdentity? TryGetCurrentServerIdentity();

    GitLabProjectIdentity? TryGetCurrentProjectIdentity();

    Task<Pipeline> GetPipelineAsync(string serverUrl, string accessToken, ProjectId project, int pipelineId);

    Task SetPipelineNameAsync(string serverUrl, string accessToken, ProjectId project, int pipelineId, string name);

    Task RepositoryDownloadFileAsync(string serverUrl, string accessToken, ProjectId project, string filePath, string @ref, FilePath destination);

    Task<IReadOnlyCollection<Branch>> RepositoryGetBranchesAsync(string serverUrl, string accessToken, ProjectId project);

    Task<Tag> RepositoryCreateTagAsync(string serverUrl, string accessToken, ProjectId project, string @ref, string name);
}
