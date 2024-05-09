// Disable nullable reference methods for the alias method, since
// the Cake.Scripting runner is currently not compatible with aliases that contains nullabilty annotations
// See https://github.com/cake-build/cake/issues/4197
#nullable disable

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
