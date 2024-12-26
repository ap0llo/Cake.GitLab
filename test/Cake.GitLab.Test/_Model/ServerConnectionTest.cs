using System;
using System.Collections.Generic;
using Xunit;

namespace Cake.GitLab.Test;

/// <summary>
/// Tests for <see cref="ServerConnection"/>
/// </summary>
public class ServerConnectionTest : EqualityTest<ServerConnection, ServerConnectionTest>, IEqualityTestDataProvider<ServerConnection>
{
    public IEnumerable<(ServerConnection left, ServerConnection right)> GetEqualTestCases()
    {
        yield return (
            new ServerConnection("example.com", "someAccessToken"),
            new ServerConnection("example.com", "someAccessToken")
        );

        yield return (
            new ServerConnection("example.com", "someAccessToken"),
            new ServerConnection(new ServerIdentity("example.com"), "someAccessToken")
        );

        // Comparisons of host name must be case-insensitive
        yield return (
            new ServerConnection("example.com", "someAccessToken"),
            new ServerConnection("EXAMPLE.COM", "someAccessToken")
        );
    }

    public IEnumerable<(ServerConnection left, ServerConnection right)> GetUnequalTestCases()
    {
        yield return (
            new ServerConnection("example.com", "accessToken"),
            new ServerConnection("example.net", "accessToken")
        );

        yield return (
            new ServerConnection("example.com", "token1"),
            new ServerConnection("example.com", "token2")
        );

        yield return (
            new ServerConnection("example.com", "TOKEN"),
            new ServerConnection("example.com", "token")
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
        var ex = Record.Exception(() => new ServerConnection("example.com", accessToken!));

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
    }
}
