using System.Runtime.CompilerServices;
using Cake.Common.Build;
using Cake.Core;
using Cake.GitLab.Internal;
using NGitLab;

namespace Cake.GitLab;

public partial class DefaultGitLabProvider : IGitLabProvider
{
    private readonly ICakeContext m_Context;

    /// <summary>
    /// Determines whether the current build is running in GitLab CI
    /// </summary>
    private bool IsRunningOnGitLabCI => m_Context.GitLabCI().IsRunningOnGitLabCI;


    /// <summary>
    /// Initializes the default implementation of <see cref="IGitLabProvider"/> from a build context
    /// </summary>
    public DefaultGitLabProvider(ICakeContext context)
    {
        m_Context = Guard.NotNull(context);
    }


    protected virtual IGitLabClient GetClient(string serverUrl, string accessToken)
    {
        Guard.NotNullOrWhitespace(serverUrl);
        Guard.NotNullOrWhitespace(accessToken);
        return new GitLabClient(serverUrl, accessToken);
    }

    private DebugLog GetLogForCurrentOperation([CallerMemberName] string operationName = "") => new DebugLog(m_Context.Log, $"GitLab.{operationName}");
}
