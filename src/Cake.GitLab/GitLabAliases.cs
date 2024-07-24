using System;
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
    private static IGitLabClient GetClient(ICakeContext context, string serverUrl, string accessToken)
    {
        if (String.IsNullOrWhiteSpace(serverUrl))
            throw new ArgumentException("Value must not be null or whitespace", nameof(serverUrl));

        if (String.IsNullOrWhiteSpace(accessToken))
            throw new ArgumentException("Value must not be null or whitespace", nameof(accessToken));

        context.Debug($"Creating GitLab client for server url '{serverUrl}'");
        if (context is IGitlabClientFactory clientFactory)
        {
            context.Debug($"Context of type '{context.GetType().FullName}' implements {nameof(IGitlabClientFactory)}. Delegating client creation to context");
            return clientFactory.GetClient(serverUrl, accessToken);
        }
        else
        {
            context.Debug($"Creating default GitLab client {typeof(GitLabClient).FullName}");
            return new GitLabClient(serverUrl, accessToken);
        }
    }
}
