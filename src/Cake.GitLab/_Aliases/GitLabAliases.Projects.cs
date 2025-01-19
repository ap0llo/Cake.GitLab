using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Annotations;
using NGitLab.Models;

namespace Cake.GitLab;

public static partial class GitLabAliases
{
    /// <summary>
    /// Gets information about a specific project
    /// </summary>
    /// <param name="context">The current Cake context</param>
    /// <param name="serverUrl">The url of the GitLab server</param>
    /// <param name="accessToken">The access token for authenticating to the GitLab server</param>
    /// <param name="project">The path (name and namespace) or id of the project to get</param>
    /// <seealso href="https://docs.gitlab.com/ee/api/projects.html#get-a-single-project">Get a single project (GitLab Docs)</seealso>
    [CakeMethodAlias]
    [CakeAliasCategory("Projects")]
    public static async Task<Project> GitLabGetProjectAsync(this ICakeContext context, string serverUrl, string accessToken, ProjectId project) =>
        await context.GetGitLabProvider().GetProjectAsync(serverUrl, accessToken, project);
}
