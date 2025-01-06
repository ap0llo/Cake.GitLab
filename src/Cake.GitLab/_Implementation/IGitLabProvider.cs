using System.Collections.Generic;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.IO;
using NGitLab.Models;

namespace Cake.GitLab;

/// <summary>
/// Provides the implementation of the Cake aliases defined by <see cref="GitLabAliases"/>
/// </summary>
public interface IGitLabProvider
{
    /// <summary>
    /// Implements the functionality of <see cref="GitLabAliases.GitLabTryGetCurrentServerIdentity"/>
    /// </summary>
    ServerIdentity? TryGetCurrentServerIdentity();

    /// <summary>
    /// Implements the functionality of <see cref="GitLabAliases.GitLabTryGetCurrentProjectIdentity"/>
    /// </summary>
    ProjectIdentity? TryGetCurrentProjectIdentity();

    /// <summary>
    /// Implements the functionality of <see cref="GitLabAliases.GitLabGetPipelineAsync(ICakeContext,string,string,ProjectId,long)"/>
    /// </summary>
    Task<Pipeline> GetPipelineAsync(string serverUrl, string accessToken, ProjectId project, long pipelineId);

    /// <summary>
    /// Implements the functionality of <see cref="GitLabAliases.GitLabGetPipelineJobsAsync(ICakeContext,string,string,ProjectId,long,GetPipelineJobsOptions)"/>
    /// </summary>
    Task<IReadOnlyCollection<Job>> GetPipelineJobsAsync(string serverUrl, string accessToken, ProjectId project, long pipelineId, GetPipelineJobsOptions? options);

    /// <summary>
    /// Implements the functionality of <see cref="GitLabAliases.GitLabSetPipelineNameAsync(ICakeContext,string,string,ProjectId,long,string)"/>
    /// </summary>
    Task SetPipelineNameAsync(string serverUrl, string accessToken, ProjectId project, long pipelineId, string name);

    /// <summary>
    /// Implements the functionality of <see cref="GitLabAliases.GitLabRepositoryDownloadFileAsync(ICakeContext,string,string,ProjectId,string,string,FilePath)"/>
    /// </summary>
    Task RepositoryDownloadFileAsync(string serverUrl, string accessToken, ProjectId project, string filePath, string @ref, FilePath destination);

    /// <summary>
    /// Implements the functionality of <see cref="GitLabAliases.GitLabRepositoryGetBranchesAsync(ICakeContext,string,string,ProjectId)"/>
    /// </summary>
    Task<IReadOnlyCollection<Branch>> RepositoryGetBranchesAsync(string serverUrl, string accessToken, ProjectId project);

    /// <summary>
    /// Implements the functionality of <see cref="GitLabAliases.GitLabRepositoryCreateTagAsync(ICakeContext,string,string,ProjectId,string,string)"/>
    /// </summary>
    Task<Tag> RepositoryCreateTagAsync(string serverUrl, string accessToken, ProjectId project, string @ref, string name);
}
