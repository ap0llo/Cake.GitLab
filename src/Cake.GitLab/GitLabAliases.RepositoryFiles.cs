// Disable nullable reference methods for the alias method, since
// the Cake.Scripting runner is currently not compatible with aliases that contains nullabilty annotations
// See https://github.com/cake-build/cake/issues/4197
#nullable disable

using System;
using System.Threading.Tasks;
using Cake.Common.Diagnostics;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.GitLab.Internal;
using NGitLab;

namespace Cake.GitLab;

[CakeAliasCategory("GitLab")]
[CakeNamespaceImport("Cake.GitLab")]
public static partial class GitLabAliases
{
    /// <summary>
    /// Downloads a file from a GitLab-hosted repository
    /// </summary>
    /// <param name="context">The current Cake context</param>
    /// <param name="connection">The connection specifing the GitLab server to connect to</param>
    /// <param name="project">The path (name and namespace) of the project to get the file from</param>
    /// <param name="filePath">The path of the file to download (as relative path within the repository)</param>
    /// <param name="ref">The name of the branch, a git tag or commit specifying the version of the file to get</param>
    /// <param name="destination">The path to save the file's content to.</param>
    [CakeMethodAlias]
    //TODO: allow both project id and path
    public static async Task GitLabRepositoryDownloadFileAsync(this ICakeContext context, GitLabConnection connection, string project, string filePath, string @ref, FilePath destination)
    {
        if (connection is null)
            throw new ArgumentNullException(nameof(connection));

        if (String.IsNullOrWhiteSpace(project))
            throw new ArgumentException("Value must not be null or whitespace", nameof(project));

        if (String.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Value must not be null or whitespace", nameof(filePath));

        if (String.IsNullOrWhiteSpace(@ref))
            throw new ArgumentException("Value must not be null or whitespace", nameof(@ref));

        if (destination is null)
            throw new ArgumentNullException(nameof(destination));

        var gitLabClient = GetClient(context, connection);
        var filesClient = new RepositoryFilesClient(context.Log, context.FileSystem, gitLabClient);

        await filesClient.DownloadFileAsync(project, filePath, @ref, destination);
    }


    private static IGitLabClient GetClient(ICakeContext context, GitLabConnection connection)
    {
        context.Debug($"Creating GitLab client for server url '{connection.ServerUrl}'");
        if (context is IGitlabClientFactory clientFactory)
        {
            context.Debug($"Context of type '{context.GetType().FullName}' implements {nameof(IGitlabClientFactory)}. Delegating client creation to context");
            return clientFactory.GetClient(connection);
        }
        else
        {
            context.Debug($"Creating default GitLab client {typeof(GitLabClient).FullName}");
            return new GitLabClient(connection.ServerUrl, connection.AccessToken);
        }
    }
}
