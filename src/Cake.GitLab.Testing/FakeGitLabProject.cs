using Cake.GitLab.Internal;
using NGitLab.Mock.Config;

namespace Cake.GitLab.Testing;

/// <summary>
/// Helper class to configure a Mock of a GitLab project for testing.
/// </summary>
public class FakeGitLabProject
{
    /// <summary>
    /// Gets the name of the project
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the path of the project group (the name of the project with the path of the parent group)
    /// </summary>
    public string Path => $"{Group.Path}/{Name}";

    public int Id { get; set; }

    /// <summary>
    /// Gets the <see cref="FakeGitLabGroup"/> this project belongs to
    /// </summary>
    public FakeGitLabGroup Group { get; }


    internal FakeGitLabProject(FakeGitLabGroup group, string name)
    {
        Group = Guard.NotNull(group);
        Name = Guard.NotNullOrWhitespace(name);
        Id = Group.Server.NextProjectId();
    }


    /// <summary>
    /// Applies the settings of this project to the specified <c>NGitLab.Mock</c> configuration
    /// </summary>
    internal void ApplyTo(GitLabConfig config)
    {
        config.WithProjectOfFullPath(Path, configure: project =>
        {
            project.Id = Id;
            project.Name = Name;
        });
    }
}
