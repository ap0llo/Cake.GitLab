using Cake.Common.Diagnostics;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.GitLab.Internal;
using NGitLab;

namespace Cake.GitLab;

/// <summary>
/// Provides aliases for interacting with a GitLab Server
/// </summary>
[CakeAliasCategory("GitLab")]
[CakeNamespaceImport("Cake.GitLab")]
public static partial class GitLabAliases
{
    private static IGitLabClient GetClient(ICakeContext context, string serverUrl, string accessToken)
    {
        Guard.NotNullOrWhitespace(serverUrl);
        Guard.NotNullOrWhitespace(accessToken);

        context.Debug($"Creating GitLab client for server url '{serverUrl}'");

        IGitLabClientFactory clientFactory;
        if (context is IGitLabClientFactory)
        {
            context.Debug($"Context of type '{context.GetType().FullName}' implements {nameof(IGitLabClientFactory)}. Delegating client creation to context");
            clientFactory = (IGitLabClientFactory)context;
        }
        else
        {
            context.Debug($"Creating default GitLab client {typeof(GitLabClient).FullName}");
            clientFactory = GitLabClientFactory.Default;
        }

        return clientFactory.GetClient(serverUrl, accessToken);
    }
}
