using System.Runtime.CompilerServices;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
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
    private static IGitLabClient GetClient(ICakeContext context, string serverUrl, string accessToken, [CallerMemberName] string aliasName = "")
    {
        Guard.NotNullOrWhitespace(serverUrl);
        Guard.NotNullOrWhitespace(accessToken);

        var log = GetLogForCurrentAlias(context, aliasName);

        log.Debug($"Creating GitLab client for server url '{serverUrl}'");

        IGitLabClientFactory clientFactory;
        if (context is IGitLabClientFactory)
        {
            log.Debug($"Context of type '{context.GetType().FullName}' implements {nameof(IGitLabClientFactory)}. Delegating client creation to context");
            clientFactory = (IGitLabClientFactory)context;
        }
        else
        {
            log.Debug($"Creating default GitLab client {typeof(GitLabClient).FullName}");
            clientFactory = GitLabClientFactory.Default;
        }

        return clientFactory.GetClient(serverUrl, accessToken);
    }


    private static DebugLog GetLogForCurrentAlias(ICakeContext context, [CallerMemberName] string aliasName = "") => new DebugLog(context.Log, aliasName);
}
