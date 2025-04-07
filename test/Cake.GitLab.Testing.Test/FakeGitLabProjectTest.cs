using System;
using System.Linq;
using NGitLab.Mock.Config;
using Xunit;

namespace Cake.GitLab.Testing.Test;

/// <summary>
/// Tests for <see cref="FakeGitLabProject"/>
/// </summary>
public class FakeGitLabProjectTest
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Ctor_fails_if_name_is_null_or_whitespace(string? projectName)
    {
        // ARRANGE
        var group = new FakeGitLabServer("gitlab.example.com").AddGroup("group");

        // ACT
        var ex = Record.Exception(() => new FakeGitLabProject(group, projectName!));

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
        Assert.StartsWith("Value must not be null or whitespace", ex.Message);
    }

    [Fact]
    public void Ctor_for_succeeds_for_valid_project_name()
    {
        // ARRANGE
        var group = new FakeGitLabServer("gitlab.example.com").AddGroup("group");

        // ACT
        var sut = new FakeGitLabProject(group, "project");

        // ASSERT
        Assert.Same(group, sut.Group);
        Assert.Equal("project", sut.Name);
        Assert.Equal("group/project", sut.Path);
    }


    [Fact]
    public void Ctor_assigns_a_unique_id_for_the_project()
    {
        // ARRANGE
        var server = new FakeGitLabServer("gitlab.example.com");
        var group1 = server.AddGroup("group1");
        var subGroup = group1.AddGroup("subgroup");
        var group2 = server.AddGroup("group2");

        // ACT
        var project1 = new FakeGitLabProject(group1, "project");
        var project2 = new FakeGitLabProject(subGroup, "project");
        var project3 = new FakeGitLabProject(group2, "project");

        // ASSERT
        Assert.Equal(1, project1.Id);
        Assert.Equal(2, project2.Id);
        Assert.Equal(3, project3.Id);
    }

    [Fact]
    public void ApplyTo_adds_the_project_to_the_NGitLab_Mock_config()
    {
        // ARRANGE
        var group = new FakeGitLabServer("gitlab.example.com").AddGroup("group");
        var sut = new FakeGitLabProject(group, "project")
        {
            Id = 23
        };

        var config = new GitLabConfig();

        // ACT
        sut.ApplyTo(config);

        // ASSERT
        Assert.Collection(
            config.BuildServer().AllProjects,
            x =>
            {
                Assert.Equal("project", x.Name);
                Assert.Equal("group/project", x.PathWithNamespace);
                Assert.Equal(23, x.Id);
            }
        );
    }
}
