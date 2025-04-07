using System;
using System.Linq;
using NGitLab.Mock.Config;
using Xunit;

namespace Cake.GitLab.Testing.Test;

/// <summary>
/// Tests for <see cref="FakeGitLabGroup"/>
/// </summary>
public class FakeGitLabGroupTest
{
    [Theory]
    [InlineData("some-group-name")]
    public void Ctor_for_server_level_group_succeeds_for_valid_group_name(string groupName)
    {
        // ARRANGE
        var server = new FakeGitLabServer("gitlab.example.com");

        // ACT
        var sut = new FakeGitLabGroup(server, groupName);

        // ASSERT
        Assert.Equal(server, sut.Server);
        Assert.Null(sut.Group);
        Assert.Equal(groupName, sut.Name);
        Assert.Equal(groupName, sut.Path);
        Assert.Empty(sut.Groups);
        Assert.Empty(sut.Projects);
    }

    [Fact]
    public void Ctor_for_subgroup_succeeds_for_valid_group_name()
    {
        // ARRANGE
        var server = new FakeGitLabServer("gitlab.example.com");
        var parentGroup = new FakeGitLabGroup(server, "parent");

        // ACT
        var sut = new FakeGitLabGroup(parentGroup, "subgroup");

        // ASSERT
        Assert.Equal(server, sut.Server);
        Assert.Same(parentGroup, sut.Group);
        Assert.Equal("subgroup", sut.Name);
        Assert.Equal("parent/subgroup", sut.Path);
        Assert.Empty(sut.Groups);
        Assert.Empty(sut.Projects);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Ctor_fails_if_name_is_null_or_whitespace(string? groupName)
    {
        // ARRANGE
        var server = new FakeGitLabServer("gitlab.example.com");

        // ACT
        var ex = Record.Exception(() => new FakeGitLabGroup(server, groupName!));

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
        Assert.StartsWith("Value must not be null or whitespace", ex.Message);

    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void AddGroup_fails_if_name_is_null_or_whitespace(string? groupName)
    {
        // ARRANGE
        var server = new FakeGitLabServer("gitlab.example.com");
        var sut = new FakeGitLabGroup(server, "group");

        // ACT
        var ex = Record.Exception(() => sut.AddGroup(groupName!));

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
        Assert.StartsWith("Value must not be null or whitespace", ex.Message);
    }

    [Theory]
    [InlineData("group", "group")]
    // Comparison should be case-insensitive
    [InlineData("group", "GROUP")]
    public void AddGroup_fails_if_subgroup_already_exists(string existingGroupName, string newGroupName)
    {
        // ARRANGE
        var server = new FakeGitLabServer("gitlab.example.com");
        var sut = new FakeGitLabGroup(server, "group");
        sut.AddGroup(existingGroupName);

        // ACT
        var ex = Record.Exception(() => sut.AddGroup(newGroupName));

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
        Assert.StartsWith($"A group named '{newGroupName}' already exists", ex.Message);
    }

    [Theory]
    [InlineData("name", "name")]
    // Comparison should be case-insensitive
    [InlineData("name", "NAME")]
    public void AddGroup_fails_if_a_project_with_the_same_name_already_exists(string projectName, string groupName)
    {
        // ARRANGE
        var server = new FakeGitLabServer("gitlab.example.com");
        var sut = new FakeGitLabGroup(server, "group");
        sut.AddProject(projectName);

        // ACT
        var ex = Record.Exception(() => sut.AddGroup(groupName));

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
        Assert.StartsWith($"Cannot add group '{groupName}' since there is a project with that name", ex.Message);
    }

    [Fact]
    public void AddGroup_adds_a_new_subgroup_to_the_group()
    {
        // ARRANGE
        var server = new FakeGitLabServer("gitlab.example.com");
        var sut = new FakeGitLabGroup(server, "group");

        // ACT
        var project = sut.AddProject("project");

        // ASSERT
        Assert.NotNull(project);
        Assert.Equal("project", project.Name);
        Assert.Equal("group/project", project.Path);
        Assert.Same(sut, project.Group);
        Assert.Collection(
            sut.Projects,
            x => Assert.Same(project, x));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void AddProject_fails_if_name_is_null_or_whitespace(string? projectName)
    {
        // ARRANGE
        var server = new FakeGitLabServer("gitlab.example.com");
        var sut = new FakeGitLabGroup(server, "group");

        // ACT
        var ex = Record.Exception(() => sut.AddProject(projectName!));

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
        Assert.StartsWith("Value must not be null or whitespace", ex.Message);
    }

    [Theory]
    [InlineData("group", "group")]
    // Comparison should be case-insensitive
    [InlineData("group", "GROUP")]
    public void AddProject_fails_if_project_already_exists(string existingProjectName, string newProjectName)
    {
        // ARRANGE
        var server = new FakeGitLabServer("gitlab.example.com");
        var sut = new FakeGitLabGroup(server, "group");
        sut.AddProject(existingProjectName);

        // ACT
        var ex = Record.Exception(() => sut.AddProject(newProjectName));

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
        Assert.StartsWith($"A project named '{newProjectName}' already exists", ex.Message);
    }

    [Fact]
    public void AddProject_adds_a_new_project_to_the_group()
    {
        // ARRANGE
        var server = new FakeGitLabServer("gitlab.example.com");
        var sut = new FakeGitLabGroup(server, "group");

        // ACT
        var project = sut.AddProject("project");

        // ASSERT
        Assert.NotNull(project);
        Assert.Equal("project", project.Name);
        Assert.Equal("group/project", project.Path);
        Assert.Same(sut, project.Group);
        Assert.Collection(
            sut.Projects,
            x => Assert.Same(project, x));
    }

    [Theory]
    [InlineData("name", "name")]
    // Comparison should be case-insensitive
    [InlineData("name", "NAME")]
    public void AddProject_fails_if_a_group_with_the_same_name_already_exists(string groupName, string projectName)
    {
        // ARRANGE
        var server = new FakeGitLabServer("gitlab.example.com");
        var sut = new FakeGitLabGroup(server, "group");
        sut.AddGroup(groupName);

        // ACT
        var ex = Record.Exception(() => sut.AddProject(projectName));

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
        Assert.StartsWith($"Cannot add project '{projectName}' since there is a group with that name", ex.Message);
    }

    [Fact]
    public void ApplyTo_adds_the_group_to_the_NGitLab_Mock_config()
    {
        // ARRANGE
        var server = new FakeGitLabServer("gitlab.example.com");
        var sut = new FakeGitLabGroup(server, "group");
        sut.AddGroup("subgroup");
        sut.AddProject("project1");

        var config = new GitLabConfig();

        // ACT
        sut.ApplyTo(config);

        // ASSERT
        var mockServer = config.BuildServer();
        Assert.Collection(
            mockServer.Groups,
            group =>
            {
                Assert.Equal("group", group.Name);
                Assert.Equal("group", group.Path);

                Assert.Collection(
                    group.Groups,
                    subgroup =>
                    {
                        Assert.Equal("subgroup", subgroup.Name);
                        Assert.Equal("group/subgroup", subgroup.PathWithNameSpace);
                    });
            }
        );

        Assert.Collection(
            mockServer.AllProjects.OrderBy(x => x.Path.Length),
            x =>
            {
                Assert.Equal("project1", x.Name);
                Assert.Equal("group/project1", x.PathWithNamespace);
            }
        );
    }
}
