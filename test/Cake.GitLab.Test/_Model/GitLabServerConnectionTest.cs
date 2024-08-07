using System;
using System.Collections.Generic;
using Xunit;

namespace Cake.GitLab.Test;

/// <summary>
/// Tests for <see cref="GitLabServerConnection"/>
/// </summary>
public class GitLabServerConnectionTest : EqualityTest<GitLabServerConnection, GitLabServerConnectionTest>, IEqualityTestDataProvider<GitLabServerConnection>
{
    public IEnumerable<(GitLabServerConnection left, GitLabServerConnection right)> GetEqualTestCases()
    {
        yield return (
            new GitLabServerConnection("example.com", "someAccessToken"),
            new GitLabServerConnection("example.com", "someAccessToken")
        );

        yield return (
            new GitLabServerConnection("example.com", "someAccessToken"),
            new GitLabServerConnection(new GitLabServerIdentity("example.com"), "someAccessToken")
        );

        // Comparisons of host name must be case-insensitive
        yield return (
            new GitLabServerConnection("example.com", "someAccessToken"),
            new GitLabServerConnection("EXAMPLE.COM", "someAccessToken")
        );
    }

    public IEnumerable<(GitLabServerConnection left, GitLabServerConnection right)> GetUnequalTestCases()
    {
        yield return (
            new GitLabServerConnection("example.com", "accessToken"),
            new GitLabServerConnection("example.net", "accessToken")
        );

        yield return (
            new GitLabServerConnection("example.com", "token1"),
            new GitLabServerConnection("example.com", "token2")
        );

        yield return (
            new GitLabServerConnection("example.com", "TOKEN"),
            new GitLabServerConnection("example.com", "token")
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
        var ex = Record.Exception(() => new GitLabServerConnection("example.com", accessToken!));

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
    }
}
