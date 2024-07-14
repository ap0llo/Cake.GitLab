using Cake.Common.Diagnostics;
using Cake.Core;
using Cake.Core.Annotations;
using NGitLab;

namespace Cake.GitLab;

/// <summary>
/// Provides aliases for interacting with a GitLab Server
/// </summary>
[CakeAliasCategory("GitLab")]
[CakeNamespaceImport("Cake.GitLab")]
public static partial class GitLabAliases
{
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
