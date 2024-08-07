using System;
using System.Collections.Generic;
using Xunit;

namespace Cake.GitLab.Test;

/// <summary>
/// Tests for <see cref="GitLabProjectConnection"/>
/// </summary>
public class GitLabProjectConnectionTest : EqualityTest<GitLabProjectConnection, GitLabProjectConnectionTest>, IEqualityTestDataProvider<GitLabProjectConnection>
{
    public IEnumerable<(GitLabProjectConnection left, GitLabProjectConnection right)> GetEqualTestCases()
    {
        yield return (
            new GitLabProjectConnection("example.com", "user", "repo", "ACCESSTOKEN"),
            new GitLabProjectConnection("example.com", "user", "repo", "ACCESSTOKEN")
        );
        yield return (
            new GitLabProjectConnection("example.com", "user", "repo", "ACCESSTOKEN"),
            new GitLabProjectConnection("example.com", "user/repo", "ACCESSTOKEN")
        );
        yield return (
            new GitLabProjectConnection("example.com", "user", "repo", "ACCESSTOKEN"),
            new GitLabProjectConnection(new GitLabProjectIdentity("example.com", "user", "repo"), "ACCESSTOKEN")
        );

        yield return (
            new GitLabProjectConnection("example.com", "group/subgroup", "repo", "ACCESSTOKEN"),
            new GitLabProjectConnection("example.com", "group/subgroup", "repo", "ACCESSTOKEN")
        );
        yield return (
            new GitLabProjectConnection("example.com", "group/subgroup", "repo", "ACCESSTOKEN"),
            new GitLabProjectConnection("example.com", "group/subgroup/repo", "ACCESSTOKEN")
        );

        yield return (
            new GitLabProjectConnection("example.com", "group/subgroup", "repo", "ACCESSTOKEN"),
            new GitLabProjectConnection(new GitLabProjectIdentity("example.com", "group/subgroup", "repo"), "ACCESSTOKEN")
        );

        // Comparisons must be case-insensitive (except for the access token)
        yield return (
            new GitLabProjectConnection("example.com", "user", "repo", "ACCESSTOKEN"),
            new GitLabProjectConnection("EXAMPLE.COM", "user", "repo", "ACCESSTOKEN")
        );
        yield return (
            new GitLabProjectConnection("example.com", "group/subgroup", "repo", "ACCESSTOKEN"),
            new GitLabProjectConnection("EXAMPLE.COM", "group/subgroup", "repo", "ACCESSTOKEN")
        );

        yield return (
            new GitLabProjectConnection("example.com", "user", "repo", "ACCESSTOKEN"),
            new GitLabProjectConnection("example.com", "USER", "repo", "ACCESSTOKEN")
        );
        yield return (
            new GitLabProjectConnection("example.com", "group/subgroup", "repo", "ACCESSTOKEN"),
            new GitLabProjectConnection("example.com", "GROUP/SUBGROUP", "repo", "ACCESSTOKEN")
        );
        yield return (
            new GitLabProjectConnection("example.com", "user", "repo", "ACCESSTOKEN"),
            new GitLabProjectConnection("example.com", "user", "REPO", "ACCESSTOKEN")
        );
        yield return (
            new GitLabProjectConnection("example.com", "group/subgroup", "repo", "ACCESSTOKEN"),
            new GitLabProjectConnection("example.com", "group/subgroup", "REPO", "ACCESSTOKEN")
        );
        yield return (
            new GitLabProjectConnection("example.com", "group/subgroup", "repo", "ACCESSTOKEN"),
            new GitLabProjectConnection("example.com", "group/SUBGROUP", "repo", "ACCESSTOKEN")
        );
    }

    public IEnumerable<(GitLabProjectConnection left, GitLabProjectConnection right)> GetUnequalTestCases()
    {
        yield return (
            new GitLabProjectConnection("example.com", "user", "repo", "ACCESSTOKEN"),
            new GitLabProjectConnection("example.net", "user", "repo", "ACCESSTOKEN")
        );
        yield return (
            new GitLabProjectConnection("example.com", "group/subgroup", "repo", "ACCESSTOKEN"),
            new GitLabProjectConnection("example.net", "group/subgroup", "repo", "ACCESSTOKEN")
        );
        yield return (
            new GitLabProjectConnection("example.com", "user1", "repo", "ACCESSTOKEN"),
            new GitLabProjectConnection("example.com", "user2", "repo", "ACCESSTOKEN")
        );
        yield return (
            new GitLabProjectConnection("example.com", "group1/subgroup", "repo", "ACCESSTOKEN"),
            new GitLabProjectConnection("example.com", "group2/subgroup", "repo", "ACCESSTOKEN")
        );
        yield return (
            new GitLabProjectConnection("example.com", "user", "repo1", "ACCESSTOKEN"),
            new GitLabProjectConnection("example.com", "user", "repo2", "ACCESSTOKEN")
        );
        yield return (
            new GitLabProjectConnection("example.com", "group/subgroup", "repo1", "ACCESSTOKEN"),
            new GitLabProjectConnection("example.com", "group/subgroup", "repo2", "ACCESSTOKEN")
        );
        yield return (
            new GitLabProjectConnection("example.com", "user", "repo", "ACCESSTOKEN_1"),
            new GitLabProjectConnection("example.com", "user", "repo", "ACCESSTOKEN_2")
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("\t")]
    public void AccessToken_must_not_be_null_or_whitespace(string? accessToken)
    {
        // ARRANGE

        // ACT
        var ex = Record.Exception(() => new GitLabProjectConnection("example.com", "group", "project", accessToken!));

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
    }
}
