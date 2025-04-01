using System.Collections.Generic;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using NGitLab.Models;

namespace Cake.GitLab;

public static partial class GitLabAliases
{
    /// <summary>
    /// Gets a list of all files that exist in a GitLab-hosted repository for a directory
    /// </summary>
    /// <returns>
    /// Returns the list of all files (as relative paths) that exist in the repository inside the specified directory at the specified ref.
    /// Listing of files is recursive.
    /// </returns>
    /// <param name="context">The current Cake context.</param>
    /// <param name="serverUrl">The url of the GitLab server</param>
    /// <param name="accessToken">The access token for authenticating to the GitLab server</param>
    /// <param name="project">The path (name and namespace) or id of the project to get the file from.</param>
    /// <param name="ref">The name of the branch, a git tag or commit specifying the version of the file to get.</param>
    /// <param name="path">The relative path of the directory which's files to list (or <c>null</c> to get all files).</param>
    /// <example language="cs"><![CDATA[
    /// [TaskName("List-File")]
    /// public class ListRepositoryFilesTask : AsyncFrostingTask
    /// {
    ///    public override async Task RunAsync(ICakeContext context)
    ///    {
    ///      var files = await context.GitLabRepositoryGetFilesAsync(
    ///          "https://gitlab.com",
    ///          "ACCESSTOKEN"
    ///          "owner/repository",
    ///          "main",
    ///          "some/directory"
    ///       );
    ///    }
    /// }
    /// ]]>
    /// </example>
    [CakeMethodAlias]
    [CakeAliasCategory("Repository")]
    public static async Task<IReadOnlyList<string>> GitLabRepositoryGetFilesAsync(this ICakeContext context, string serverUrl, string accessToken, ProjectId project, string @ref, string? path = null) =>
        await context.GetGitLabProvider().RepositoryGetFilesAsync(serverUrl, accessToken, project, @ref, path);


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
    [CakeAliasCategory("Repository")]
    public static async Task GitLabRepositoryDownloadFileAsync(this ICakeContext context, string serverUrl, string accessToken, ProjectId project, string filePath, string @ref, FilePath destination) =>
        await context.GetGitLabProvider().RepositoryDownloadFileAsync(serverUrl, accessToken, project, filePath, @ref, destination);

    /// <summary>
    /// Lists all of a project's branches
    /// </summary>
    /// <param name="context">The current Cake context.</param>
    /// <param name="serverUrl">The url of the GitLab server</param>
    /// <param name="accessToken">The access token for authenticating to the GitLab server</param>
    /// <param name="project">The path (name and namespace) or id of the project to get the branches for.</param>
    /// <example language="cs"><![CDATA[
    /// [TaskName("Get-Branches")]
    /// public class GetBranchesTask : AsyncFrostingTask
    /// {
    ///    public override Task RunAsync(ICakeContext context)
    ///    {
    ///      await context.GitLabRepositoryGetBranchesAsync(
    ///          "https://gitlab.com",
    ///          "ACCESSTOKEN"
    ///          "owner/repository"
    ///      );
    ///    }
    /// }
    /// ]]>
    /// </example>
    [CakeMethodAlias]
    [CakeAliasCategory("Repository")]
    public static async Task<IReadOnlyCollection<Branch>> GitLabRepositoryGetBranchesAsync(this ICakeContext context, string serverUrl, string accessToken, ProjectId project) =>
        await context.GetGitLabProvider().RepositoryGetBranchesAsync(serverUrl, accessToken, project);

    /// <summary>
    /// Creates a new tag in the project repository
    /// </summary>
    /// <remarks>
    /// Creates a tag with the specified name for the specified target reference (commit SHA, branch name or tag name).
    /// If the specified tag already exists for the target commit, creation of the tag is skipped and no error is thrown.
    /// </remarks>
    /// <param name="context">The current Cake context.</param>
    /// <param name="serverUrl">The url of the GitLab server</param>
    /// <param name="accessToken">The access token for authenticating to the GitLab server</param>
    /// <param name="project">The path (name and namespace) or id of the project to create a tag in.</param>
    /// <param name="ref">The ref to create the tag from. Can be a commit SHA, branch name or existing tag name</param>
    /// <param name="name"></param>
    /// <example language="cs"><![CDATA[
    /// [TaskName("Create-Tag")]
    /// public class CreateTagTask : FrostingTask
    /// {
    ///    public override void Run(ICakeContext context)
    ///    {
    ///      context.GitLabRepositoryCreateTagAsync(
    ///          "https://gitlab.com",
    ///          "ACCESSTOKEN"
    ///          "owner/repository",
    ///          "b5d7135",
    ///          "tagName"
    ///      );
    ///    }
    /// }
    /// ]]>
    /// </example>
    /// <seealso href="https://docs.gitlab.com/ee/api/tags.html#create-a-new-tag">Create a new tag (GitLab REST API documentation)</seealso>
    [CakeMethodAlias]
    [CakeAliasCategory("Repository")]
    public static async Task<Tag> GitLabRepositoryCreateTagAsync(this ICakeContext context, string serverUrl, string accessToken, ProjectId project, string @ref, string name) =>
        await context.GetGitLabProvider().RepositoryCreateTagAsync(serverUrl, accessToken, project, @ref, name);
}
