// Derived from https://github.com/ap0llo/changelog/blob/97d0d3111510dae9f4222704a50d88938be8debe/src/ChangeLog.Test/Integrations/GitLab/GitLabProjectInfoTest.cs
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using Xunit;

namespace Cake.GitLab.Test;

/// <summary>
/// Tests for <see cref="GitLabProjectIdentity"/>
/// </summary>
public class GitLabProjectIdentityTest : EqualityTest<GitLabProjectIdentity, GitLabProjectIdentityTest>, IEqualityTestDataProvider<GitLabProjectIdentity>
{
    public IEnumerable<(GitLabProjectIdentity left, GitLabProjectIdentity right)> GetEqualTestCases()
    {
        yield return (
            new GitLabProjectIdentity("example.com", "user", "repo"),
            new GitLabProjectIdentity("example.com", "user", "repo")
        );
        yield return (
            new GitLabProjectIdentity("example.com", "group/subgroup", "repo"),
            new GitLabProjectIdentity("example.com", "group/subgroup", "repo")
        );

        // Comparisons must be case-insensitive
        yield return (
            new GitLabProjectIdentity("example.com", "user", "repo"),
            new GitLabProjectIdentity("EXAMPLE.COM", "user", "repo")
        );
        yield return (
            new GitLabProjectIdentity("example.com", "group/subgroup", "repo"),
            new GitLabProjectIdentity("EXAMPLE.COM", "group/subgroup", "repo")
        );

        yield return (
            new GitLabProjectIdentity("example.com", "user", "repo"),
            new GitLabProjectIdentity("example.com", "USER", "repo")
        );
        yield return (
            new GitLabProjectIdentity("example.com", "group/subgroup", "repo"),
            new GitLabProjectIdentity("example.com", "GROUP/SUBGROUP", "repo")
        );
        yield return (
            new GitLabProjectIdentity("example.com", "user", "repo"),
            new GitLabProjectIdentity("example.com", "user", "REPO")
        );
        yield return (
            new GitLabProjectIdentity("example.com", "group/subgroup", "repo"),
            new GitLabProjectIdentity("example.com", "group/subgroup", "REPO")
        );
        yield return (
            new GitLabProjectIdentity("example.com", "group/subgroup", "repo"),
            new GitLabProjectIdentity("example.com", "group/SUBGROUP", "repo")
        );
    }

    public IEnumerable<(GitLabProjectIdentity left, GitLabProjectIdentity right)> GetUnequalTestCases()
    {
        yield return (
            new GitLabProjectIdentity("example.com", "user", "repo"),
            new GitLabProjectIdentity("example.net", "user", "repo")
        );
        yield return (
            new GitLabProjectIdentity("example.com", "group/subgroup", "repo"),
            new GitLabProjectIdentity("example.net", "group/subgroup", "repo")
        );
        yield return (
            new GitLabProjectIdentity("example.com", "user1", "repo"),
            new GitLabProjectIdentity("example.com", "user2", "repo")
        );
        yield return (
            new GitLabProjectIdentity("example.com", "group1/subgroup", "repo"),
            new GitLabProjectIdentity("example.com", "group2/subgroup", "repo")
        );
        yield return (
            new GitLabProjectIdentity("example.com", "user", "repo1"),
            new GitLabProjectIdentity("example.com", "user", "repo2")
        );
        yield return (
            new GitLabProjectIdentity("example.com", "group/subgroup", "repo1"),
            new GitLabProjectIdentity("example.com", "group/subgroup", "repo2")
        );
    }


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("\t")]
    public void Host_must_not_be_null_or_whitespace(string? host)
    {
        Assert.Throws<ArgumentException>(() => new GitLabProjectIdentity(host!, "user", "repo"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("\t")]
    public void Namespace_must_not_be_null_or_whitespace(string? @namespace)
    {
        Assert.Throws<ArgumentException>(() => new GitLabProjectIdentity("example.com", @namespace!, "repo"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("\t")]
    public void Project_must_not_be_null_or_whitespace(string? project)
    {
        Assert.Throws<ArgumentException>(() => new GitLabProjectIdentity("example.com", "user", project!));
    }


    [Fact]
    public void Setting_Project_updates_project_path()
    {
        // ARRANGE
        var initial = new GitLabProjectIdentity("example.com", "group/subgroup", "project");

        // ACT 
        var updated = initial with { Project = "another-project" };

        // ASSERT
        Assert.Equal("group/subgroup/another-project", updated.ProjectPath);
    }


    [Fact]
    public void Setting_Namespace_updates_project_path()
    {
        // ARRANGE
        var initial = new GitLabProjectIdentity("example.com", "group/subgroup", "project");

        // ACT 
        var updated = initial with { Namespace = "someUser" };

        // ASSERT
        Assert.Equal("someUser/project", updated.ProjectPath);
    }



    [Theory]
    [InlineData("user/project", "user", "project")]
    [InlineData("group/subgroup/project", "group/subgroup", "project")]
    public void Setting_ProjectPath_updates_namespace_and_project(string projectPath, string expectedNamespace, string expetedProject)
    {
        // ARRANGE
        var initial = new GitLabProjectIdentity("example.com", "initalNamespace", "initialProject");

        // ACT 
        var updated = initial with { ProjectPath = projectPath };

        // ASSERT
        Assert.Equal(projectPath, updated.ProjectPath);
        Assert.Equal(expectedNamespace, updated.Namespace);
        Assert.Equal(expetedProject, updated.Project);
    }

    [Theory]
    [InlineData("user")]
    [InlineData("user/")]
    [InlineData("/user")]
    [InlineData("group/subgroup/")]
    [InlineData("/group/subgroup/")]
    [InlineData("/group/subgroup/project")]
    public void Setting_ProjectPath_throws_ArgumentException_if_path_is_invalid(string projectPath)
    {
        // ARRANGE
        var initial = new GitLabProjectIdentity("example.com", "initalNamespace", "initialProject");

        // ACT 
        var ex = Record.Exception(() => initial with { ProjectPath = projectPath });

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
    }
}
