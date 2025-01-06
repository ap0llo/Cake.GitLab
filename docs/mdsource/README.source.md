# Cake.GitLab Documentation

Cake.GitLab currently provides the following Cake aliases.
For each alias, multiple overloads are available, see [Overloads](./overloads.md) for details.

| Category   | Alias                                  | Description                                                                                                                                        |
|------------|----------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------|
| Helpers    | `GitLabTryGetCurrentServerIdentity()`  | Attempts to determine the server identity from environment variables (see [Identity and Connection objects](identites-and-connection-objects.md))  | 
|            | `GitLabTryGetCurrentProjectIdentity()` | Attempts to determine the project identity from environment variables (see [Identity and Connection objects](identites-and-connection-objects.md)) | 
| Pipelines  | `GitLabGetPipelineAsync()`             | Gets data about a GitLab CI pipeline                                                                                                               |
|            | `GitLabSetPipelineNameAsync()`         | Updates the name of a pipeline                                                                                                                     |
|            | `GitLabGetPipelineJobsAsync()`         | Get the jobs for a pipeline.                                                                                                                       |
| Repository | `GitLabRepositoryDownloadFileAsync()`  | Downloads a file from a GitLab-hosted repository                                                                                                   |
|            | `GitLabRepositoryGetBranchesAsync()`   | Lists all of a project's branches                                                                                                                  |
|            | `GitLabRepositoryCreateTagAsync()`     | Creates a new tag in the project repository                                                                                                        |

## Additional Information

- [Overloads](./overloads.md)
- [Identity and Connection objects](./identites-and-connection-objects.md)
- [Testing](./testing.md)
