using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.GitLab.Internal;
using Cake.GitLab.Model;

namespace Cake.GitLab;

public static partial class GitLabAliases
{
    /// <summary>
    /// Attempts to determine the GitLab server and project path based on a git repository's remote url
    /// </summary>
    /// <param name="context">The current Cake context.</param>
    /// <param name="remoteUrl">A git remote url. Supports both HTTP and SSH urls (including urls in the SCP format, e.g. <c>git@github.com:ap0llo/Cake.GitLab.git</c>)</param>
    /// <returns>Returns a <see cref="GitLabProjectInfo"/> with the information extracted from the url or <c>null</c> if not project data could be read from the remote url.</returns>
    [CakeMethodAlias]
    public static GitLabProjectInfo? GitLabGetProjectFromRemoteUrl(this ICakeContext context, string remoteUrl)
    {
        if (String.IsNullOrWhiteSpace(remoteUrl))
            throw new ArgumentException("Value must not be null or whitespace", nameof(remoteUrl));

        var urlParser = new GitLabUrlParser(context.Log);
        return urlParser.GetProjectFromRemoteUrl(remoteUrl);
    }
}
