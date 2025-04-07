using System;
using System.Collections.Generic;
using Cake.GitLab.Internal;
using NGitLab.Mock.Config;

namespace Cake.GitLab.Testing;

/// <summary>
/// Helper class to configure a Mock of a GitLab group for testing.
/// </summary>
public class FakeGitLabGroup
{
    private readonly Dictionary<string, FakeGitLabGroup> m_Groups = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, FakeGitLabProject> m_Projects = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the <see cref="FakeGitLabServer"/> this group belongs to
    /// </summary>
    public FakeGitLabServer Server { get; }

    /// <summary>
    /// Gets the parent group for the current group or <c>null</c> if this group is not a subgroup.
    /// </summary>
    public FakeGitLabGroup? Group { get; }

    /// <summary>
    /// Gets the name of the group
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the path of the current group (the name of the group including the name of the parent groups)
    /// </summary>
    public string Path => Group is null ? Name : $"{Group.Path}/{Name}";

    /// <summary>
    /// Gets the subgroups for the current group
    /// </summary>
    public IEnumerable<FakeGitLabGroup> Groups => m_Groups.Values;

    /// <summary>
    /// Gets the projects in the current group
    /// </summary>
    public IEnumerable<FakeGitLabProject> Projects => m_Projects.Values;


    internal FakeGitLabGroup(FakeGitLabServer server, string name)
    {
        Server = Guard.NotNull(server);
        Group = null;
        Name = Guard.NotNullOrWhitespace(name);
    }

    internal FakeGitLabGroup(FakeGitLabGroup group, string name) : this(Guard.NotNull(group).Server, name)
    {
        Group = group;
    }


    /// <summary>
    /// Adds a new group to the current group
    /// </summary>
    /// <remarks>
    /// Note that this method checks <paramref name="name"/> for null or whitespace and will ensure the group name is unique within this instance of <see cref="FakeGitLabGroup"/>.
    /// A real GitLab server will impose additional restrictions on group names.
    /// These rules are not enforced here.
    /// </remarks>
    /// <param name="name">The name of the group</param>
    /// <returns>Returns the newly created group</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace.</exception>
    public FakeGitLabGroup AddGroup(string name)
    {
        name = Guard.NotNullOrWhitespace(name);

        if (m_Groups.ContainsKey(name))
        {
            throw new ArgumentException($"A group named '{name}' already exists");
        }

        if (m_Projects.ContainsKey(name))
        {
            throw new ArgumentException($"Cannot add group '{name}' since there is a project with that name");
        }


        var group = new FakeGitLabGroup(this, name);
        m_Groups.Add(name, group);
        return group;
    }

    /// <summary>
    /// Adds a new project to the current group
    /// </summary>
    /// <remarks>
    /// Note that this method checks <paramref name="name"/> for null or whitespace and will ensure the project name is unique within this instance of <see cref="FakeGitLabGroup"/>.
    /// A real GitLab server will impose additional restrictions on project names.
    /// These rules are not enforced here.
    /// </remarks>
    /// <param name="name">The name of the group</param>
    /// <returns>Returns the newly created group</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace.</exception>
    public FakeGitLabProject AddProject(string name)
    {
        name = Guard.NotNullOrWhitespace(name);

        if (m_Projects.ContainsKey(name))
        {
            throw new ArgumentException($"A project named '{name}' already exists");
        }

        if (m_Groups.ContainsKey(name))
        {
            throw new ArgumentException($"Cannot add project '{name}' since there is a group with that name");
        }

        var project = new FakeGitLabProject(this, name);
        m_Projects.Add(name, project);
        return project;
    }

    /// <summary>
    /// Applies the settings of this group to the specified <c>NGitLab.Mock</c> configuration
    /// </summary>
    internal void ApplyTo(GitLabConfig config)
    {
        config.WithGroupOfFullPath(fullPath: Path, configure: group =>
        {
            group.Name = Name;
        });

        foreach (var project in m_Projects.Values)
        {
            project.ApplyTo(config);
        }

        foreach (var group in m_Groups.Values)
        {
            group.ApplyTo(config);
        }
    }
}
