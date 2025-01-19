using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using NGitLab;
using NGitLab.Models;

namespace Cake.GitLab;

public partial class DefaultGitLabProvider
{
    /// <inheritdoc />
    public async Task<Project> GetProjectAsync(string serverUrl, string accessToken, ProjectId project)
    {
        var log = GetLogForCurrentOperation();
        log.Verbose($"Getting GitLab project {project}");

        var client = GetClient(serverUrl, accessToken);

        try
        {
            var projectData = await client.Projects.GetAsync(project);
            return projectData;
        }
        catch (GitLabException ex)
        {
            throw new CakeException($"Error while getting GitLab project {project}: {ex.Message}", ex);
        }
    }
}
