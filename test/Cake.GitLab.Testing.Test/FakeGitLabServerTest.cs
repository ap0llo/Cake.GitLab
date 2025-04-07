using System;
using NGitLab.Models;
using Xunit;

namespace Cake.GitLab.Testing.Test;

/// <summary>
/// Tests for <see cref="FakeGitLabServer"/>
/// </summary>
public class FakeGitLabServerTest
{
    [Theory]
    [InlineData("gitlab.example.com")]
    [InlineData("192.0.2.1")]
    [InlineData("2001:db8:3333:4444:5555:6666:7777:8888")]
    public void Ctor_succeeds_for_valid_host_name(string hostName)
    {
        // ARRANGE

        // ACT
        var sut = new FakeGitLabServer(hostName);

        // ASSERT
        Assert.Equal($"https://{hostName}", sut.Url);
        Assert.Empty(sut.Groups);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Ctor_fails_if_host_name_is_null_or_whitespace(string? hostName)
    {
        // ARRANGE

        // ACT
        var ex = Record.Exception(() => new FakeGitLabServer(hostName!));

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
        Assert.StartsWith("Value must not be null or whitespace", ex.Message);

    }

    [Theory]
    [InlineData("some host name")]
    public void Ctor_fails_if_host_name_is_invalid(string? hostName)
    {
        // ARRANGE

        // ACT
        var ex = Record.Exception(() => new FakeGitLabServer(hostName!));

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
        Assert.StartsWith("Value is not a valid host name", ex.Message);

    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void AddGroup_fails_if_name_is_null_or_whitespace(string? groupName)
    {
        // ARRANGE
        var sut = new FakeGitLabServer("gitlab.example.com");

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
    public void AddGroup_fails_if_group_already_exists(string existingGroupName, string newGroupName)
    {
        // ARRANGE
        var sut = new FakeGitLabServer("gitlab.example.com");
        sut.AddGroup(existingGroupName);

        // ACT
        var ex = Record.Exception(() => sut.AddGroup(newGroupName));

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
        Assert.StartsWith($"A group named '{newGroupName}' already exists", ex.Message);
    }

    [Fact]
    public void AddGroup_adds_a_new_group_to_the_server()
    {
        // ARRANGE
        var sut = new FakeGitLabServer("gitlab.example.com");

        // ACT
        var group = sut.AddGroup("group");

        // ASSERT
        Assert.NotNull(group);
        Assert.Equal("group", group.Name);
        Assert.Equal("group", group.Path);
        Assert.Empty(group.Projects);
        Assert.Empty(group.Groups);
        Assert.Same(sut, group.Server);
        Assert.Collection(
            sut.Groups,
            x => Assert.Same(group, x));
    }


    [Fact]
    public void BuildClient_fails_if_there_are_duplicate_project_ids()
    {
        // ARRANGE
        var sut = new FakeGitLabServer("example.gitlab.com");
        var group = sut.AddGroup("group1");
        var project1 = group.AddProject("project1");
        var project2 = group.AddProject("project2");

        project1.Id = 5;
        project2.Id = 5;

        // ACT
        var ex = Record.Exception(() => sut.BuildClient());

        // ASSERT
        Assert.IsType<NotSupportedException>(ex);
        Assert.Equal<object>("Duplicate project id", ex.Message);
    }
}
