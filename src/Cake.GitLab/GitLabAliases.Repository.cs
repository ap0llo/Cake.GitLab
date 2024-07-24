using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.GitLab.Internal;
using NGitLab.Models;

namespace Cake.GitLab;

public static partial class GitLabAliases
{
    /// <summary>
    /// Downloads a file from a GitLab-hosted repository
    /// </summary>
    /// <param name="context">The current Cake context.</param>
    /// <param name="serverUrl">The url of the GitLab server</param>
    /// <param name="accessToken">The access token for authenticating to the GitLab server</param>
    /// <param name="project">The path (name and namespace) or id of the project to get the file from.</param>
    /// <param name="filePath">The path of the file to download (as relative path within the repository).</param>
    /// <param name="ref">The name of the branch, a git tag or commit specifying the version of the file to get.</param>
    /// <param name="destination">The path to save the file's content to.</param>
    /// <example language="cs"><![CDATA[
    /// [TaskName("Download-File")]   
    /// public class DownloadRepositoryFileTask : AsyncFrostingTask
    /// {
    ///    public override async Task RunAsync(ICakeContext context)
    ///    {
    ///      var connection = new GitLabConnection();
    ///      await context.GitLabRepositoryDownloadFileAsync(
    ///          "https://gitlab.com",
    ///          "ACCESSTOKEN"
    ///          "owner/repository",
    ///          "README.md",
    ///          "main",
    ///          "downloaded/README.md"
    ///       );
    ///    }
    /// }
    /// ]]>
    /// </example>
    [CakeMethodAlias]
    public static async Task GitLabRepositoryDownloadFileAsync(this ICakeContext context, string serverUrl, string accessToken, ProjectId project, string filePath, string @ref, FilePath destination)
    {
        if (String.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Value must not be null or whitespace", nameof(filePath));

        if (String.IsNullOrWhiteSpace(@ref))
            throw new ArgumentException("Value must not be null or whitespace", nameof(@ref));

        if (destination is null)
            throw new ArgumentNullException(nameof(destination));

        var gitLabClient = GetClient(context, serverUrl, accessToken);
        var repositoryClient = new RepositoryClient(context.Log, context.FileSystem, gitLabClient);

        await repositoryClient.DownloadFileAsync(project, filePath, @ref, destination);
    }

    [CakeMethodAlias]
    public static async Task GitLabRepositoryDownloadFileAsync(this ICakeContext context, GitLabConnection connection, ProjectId project, string filePath, string @ref, FilePath destination)
    {
        if (connection is null)
            throw new ArgumentNullException(nameof(connection));

        await context.GitLabRepositoryDownloadFileAsync(connection.ServerUrl, connection.AccessToken, project, filePath, @ref, destination);
    }

    /// <summary>
    /// Get the branches that exist in a GitLab-hosted repository
    /// </summary>
    /// <param name="context">The current Cake context.</param>
    /// <param name="serverUrl">The url of the GitLab server</param>
    /// <param name="accessToken">The access token for authenticating to the GitLab server</param>
    /// <param name="project">The path (name and namespace) or id of the project to get the branches for.</param>
    /// <example language="cs"><![CDATA[
    /// [TaskName("Get-Branches")]   
    /// public class GetBranchesTask : FrostingTask
    /// {
    ///    public override void Run(ICakeContext context)
    ///    {
    ///      var connection = new GitLabConnection("https://gitlab.com", "ACCESSTOKEN");
    ///      context.GitLabRepositoryGetBranches(
    ///          connection,
    ///          "owner/repository"
    ///      );
    ///    }
    /// }
    /// ]]>
    /// </example>
    [CakeMethodAlias]
    public static IReadOnlyCollection<Branch> GitLabRepositoryGetBranches(this ICakeContext context, string serverUrl, string accessToken, ProjectId project)
    {
        var gitLabClient = GetClient(context, serverUrl, accessToken);
        var repositoryClient = new RepositoryClient(context.Log, context.FileSystem, gitLabClient);

        return repositoryClient.GetBranches(project);
    }

    public static IReadOnlyCollection<Branch> GitLabRepositoryGetBranches(this ICakeContext context, GitLabConnection connection, ProjectId project)
    {
        if (connection is null)
            throw new ArgumentNullException(nameof(connection));

        return context.GitLabRepositoryGetBranches(connection.ServerUrl, connection.AccessToken, project);
    }
}
