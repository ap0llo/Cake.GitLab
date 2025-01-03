//
// Derived from
//  - https://github.com/ap0llo/changelog/blob/97d0d3111510dae9f4222704a50d88938be8debe/src/ChangeLog.Test/Integrations/GitLab/GitLabProjectInfoTest.cs
//  - https://github.com/ap0llo/changelog/blob/97d0d3111510dae9f4222704a50d88938be8debe/src/ChangeLog.Test/Integrations/GitLab/GitLabUrlParserTest.cs
//
// Licensed under the MIT License
//

using System;
using System.Collections.Generic;
using Xunit;

namespace Cake.GitLab.Test;

/// <summary>
/// Tests for <see cref="ProjectIdentity"/>
/// </summary>
public class ProjectIdentityTest : EqualityTest<ProjectIdentity, ProjectIdentityTest>, IEqualityTestDataProvider<ProjectIdentity>
{
    public IEnumerable<(ProjectIdentity left, ProjectIdentity right)> GetEqualTestCases()
    {
        yield return (
            new ProjectIdentity(new ServerIdentity("example.com"), "user", "repo"),
            new ProjectIdentity(new ServerIdentity("example.com"), "user", "repo")
        );

        yield return (
            new ProjectIdentity(new ServerIdentity("example.com"), "user", "repo"),
            new ProjectIdentity(new ServerIdentity("example.com"), "user/repo")
        );

        yield return (
            new ProjectIdentity(new ServerIdentity("example.com"), "group/subgroup", "repo"),
            new ProjectIdentity(new ServerIdentity("example.com"), "group/subgroup", "repo")
        );

        yield return (
            new ProjectIdentity(new ServerIdentity("example.com"), "group/subgroup", "repo"),
            new ProjectIdentity(new ServerIdentity("example.com"), "group/subgroup/repo")
        );


        // Comparisons must be case-insensitive
        yield return (
            new ProjectIdentity(new ServerIdentity("example.com"), "user", "repo"),
            new ProjectIdentity(new ServerIdentity("EXAMPLE.COM"), "user", "repo")
        );
        yield return (
            new ProjectIdentity(new ServerIdentity("example.com"), "group/subgroup", "repo"),
            new ProjectIdentity(new ServerIdentity("EXAMPLE.COM"), "group/subgroup", "repo")
        );

        yield return (
            new ProjectIdentity(new ServerIdentity("example.com"), "user", "repo"),
            new ProjectIdentity(new ServerIdentity("example.com"), "USER", "repo")
        );
        yield return (
            new ProjectIdentity(new ServerIdentity("example.com"), "group/subgroup", "repo"),
            new ProjectIdentity(new ServerIdentity("example.com"), "GROUP/SUBGROUP", "repo")
        );
        yield return (
            new ProjectIdentity(new ServerIdentity("example.com"), "user", "repo"),
            new ProjectIdentity(new ServerIdentity("example.com"), "user", "REPO")
        );
        yield return (
            new ProjectIdentity(new ServerIdentity("example.com"), "group/subgroup", "repo"),
            new ProjectIdentity(new ServerIdentity("example.com"), "group/subgroup", "REPO")
        );
        yield return (
            new ProjectIdentity(new ServerIdentity("example.com"), "group/subgroup", "repo"),
            new ProjectIdentity(new ServerIdentity("example.com"), "group/SUBGROUP", "repo")
        );
    }

    public IEnumerable<(ProjectIdentity left, ProjectIdentity right)> GetUnequalTestCases()
    {
        yield return (
            new ProjectIdentity(new ServerIdentity("example.com"), "user", "repo"),
            new ProjectIdentity(new ServerIdentity("example.net"), "user", "repo")
        );
        yield return (
            new ProjectIdentity(new ServerIdentity("example.com"), "group/subgroup", "repo"),
            new ProjectIdentity(new ServerIdentity("example.net"), "group/subgroup", "repo")
        );
        yield return (
            new ProjectIdentity(new ServerIdentity("example.com"), "user1", "repo"),
            new ProjectIdentity(new ServerIdentity("example.com"), "user2", "repo")
        );
        yield return (
            new ProjectIdentity(new ServerIdentity("example.com"), "group1/subgroup", "repo"),
            new ProjectIdentity(new ServerIdentity("example.com"), "group2/subgroup", "repo")
        );
        yield return (
            new ProjectIdentity(new ServerIdentity("example.com"), "user", "repo1"),
            new ProjectIdentity(new ServerIdentity("example.com"), "user", "repo2")
        );
        yield return (
            new ProjectIdentity(new ServerIdentity("example.com"), "group/subgroup", "repo1"),
            new ProjectIdentity(new ServerIdentity("example.com"), "group/subgroup", "repo2")
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("\t")]
    public void Namespace_must_not_be_null_or_whitespace(string? @namespace)
    {
        // ARRANGE

        // ACT
        var ex = Record.Exception(() => new ProjectIdentity(new ServerIdentity("example.com"), @namespace!, "repo"));

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("\t")]
    public void Project_must_not_be_null_or_whitespace(string? project)
    {
        // ARRANGE

        // ACT
        var ex = Record.Exception(() => new ProjectIdentity(new ServerIdentity("example.com"), "user", project!));

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
    }


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("\t")]
    public void ProjectPath_must_not_be_null_or_whitespace(string? projectPath)
    {
        // ARRANGE

        // ACT
        var ex = Record.Exception(() => new ProjectIdentity(new ServerIdentity("example.com"), projectPath!));

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
    }

    [Fact]
    public void Can_be_initialized_from_project_path()
    {
        var fromNamespaceAndProjectName = new ProjectIdentity(new ServerIdentity("example.com"), "some-group", "some-project");
        var fromProjectPath = new ProjectIdentity(new ServerIdentity("example.com"), "some-group/some-project");

        Assert.Equal(fromNamespaceAndProjectName, fromProjectPath);
    }

    [Fact]
    public void Setting_Project_updates_project_path()
    {
        // ARRANGE
        var initial = new ProjectIdentity(new ServerIdentity("example.com"), "group/subgroup", "project");

        // ACT
        var updated = initial with { Project = "another-project" };

        // ASSERT
        Assert.Equal("group/subgroup/another-project", updated.ProjectPath);
    }


    [Fact]
    public void Setting_Namespace_updates_project_path()
    {
        // ARRANGE
        var initial = new ProjectIdentity(new ServerIdentity("example.com"), "group/subgroup", "project");

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
        var initial = new ProjectIdentity(new ServerIdentity("example.com"), "initalNamespace", "initialProject");

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
        var initial = new ProjectIdentity(new ServerIdentity("example.com"), "initalNamespace", "initialProject");

        // ACT
        var ex = Record.Exception(() => initial with { ProjectPath = projectPath });

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("\t")]
    [InlineData("  ")]
    [InlineData("not-a-url")]
    [InlineData("ftp://gitlab.com/owner/repo.git")] // unsupported scheme
    [InlineData("http://gitlab.com")]               // missing project path
    [InlineData("http://gitlab.com/user")]          // missing project name
    public void FromGitRemoteUrl_throws_ArgumentException_for_invalid_input(string? url)
    {
        // ARRANGE

        // ACT
        var ex = Record.Exception(() => ProjectIdentity.FromGitRemoteUrl(url!));

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
    }

    [Theory]
    [InlineData("https://gitlab.com/user/repoName.git", "gitlab.com", "user", "reponame")]
    [InlineData("https://gitlab.com/group/subgroup/repoName.git", "gitlab.com", "group/subgroup", "reponame")]
    [InlineData("https://example.com/user/repoName.git", "example.com", "user", "reponame")]
    [InlineData("git@gitlab.com:user/repoName.git", "gitlab.com", "user", "repoName")]
    [InlineData("git@gitlab.com:group/subgroup/repoName.git", "gitlab.com", "group/subgroup", "repoName")]
    [InlineData("git@example.com:user/repoName.git", "example.com", "user", "repoName")]
    [InlineData("git@example.com:group/subgroup/repoName.git", "example.com", "group/subgroup", "repoName")]
    public void FromGitRemoteUrl_returns_the_expected_ProjectIdentity(string remoteUrl, string host, string @namespace, string projectName)
    {
        // ARRANGE
        var expected = new ProjectIdentity(new ServerIdentity(host), @namespace, projectName);

        // ACT
        var actual = ProjectIdentity.FromGitRemoteUrl(remoteUrl);

        // ASSERT
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("\t")]
    [InlineData("  ")]
    [InlineData("not-a-url")]
    [InlineData("ftp://gitlab.com/owner/repo.git")] // unsupported scheme
    [InlineData("http://gitlab.com")]               // missing project path
    [InlineData("http://gitlab.com/user")]          // missing project name
    public void TryGetFromGitRemoteUrl_returns_false_for_invalid_input(string? url)
    {
        // ARRANGE

        // ACT
        var success = ProjectIdentity.TryGetFromGitRemoteUrl(url!, out var parsed);

        // ASSERT
        Assert.False(success);
        Assert.Null(parsed);
    }

    [Theory]
    [InlineData("https://gitlab.com/user/repoName.git", "gitlab.com", "user", "reponame")]
    [InlineData("https://gitlab.com/group/subgroup/repoName.git", "gitlab.com", "group/subgroup", "reponame")]
    [InlineData("https://example.com/user/repoName.git", "example.com", "user", "reponame")]
    [InlineData("git@gitlab.com:user/repoName.git", "gitlab.com", "user", "repoName")]
    [InlineData("git@gitlab.com:group/subgroup/repoName.git", "gitlab.com", "group/subgroup", "repoName")]
    [InlineData("git@example.com:user/repoName.git", "example.com", "user", "repoName")]
    [InlineData("git@example.com:group/subgroup/repoName.git", "example.com", "group/subgroup", "repoName")]
    public void TryParseRemoteUrl_returns_the_expected_GitHubProjectInfo(string url, string host, string @namespace, string projectName)
    {
        // ARRANGE
        var expected = new ProjectIdentity(new ServerIdentity(host), @namespace, projectName);

        // ACT
        var success = ProjectIdentity.TryGetFromGitRemoteUrl(url, out var parsed);

        // ASSERT
        Assert.True(success);
        Assert.NotNull(parsed);
        Assert.Equal(expected, parsed);
    }
}
