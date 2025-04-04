﻿using System;
using System.Collections.Generic;
using Xunit;

namespace Cake.GitLab.Test;

/// <summary>
/// Tests for <see cref="ProjectConnection"/>
/// </summary>
public class ProjectConnectionTest : EqualityTest<ProjectConnection, ProjectConnectionTest>, IEqualityTestDataProvider<ProjectConnection>
{
    public IEnumerable<(ProjectConnection left, ProjectConnection right)> GetEqualTestCases()
    {
        yield return (
            new ProjectConnection(new ServerIdentity("example.com"), "user", "repo", "ACCESSTOKEN"),
            new ProjectConnection(new ServerIdentity("example.com"), "user", "repo", "ACCESSTOKEN")
        );
        yield return (
            new ProjectConnection(new ServerIdentity("example.com"), "user", "repo", "ACCESSTOKEN"),
            new ProjectConnection(new ServerIdentity("example.com"), "user/repo", "ACCESSTOKEN")
        );
        yield return (
            new ProjectConnection(new ServerIdentity("example.com"), "user", "repo", "ACCESSTOKEN"),
            new ProjectConnection(new ProjectIdentity(new ServerIdentity("example.com"), "user", "repo"), "ACCESSTOKEN")
        );

        yield return (
            new ProjectConnection(new ServerIdentity("example.com"), "group/subgroup", "repo", "ACCESSTOKEN"),
            new ProjectConnection(new ServerIdentity("example.com"), "group/subgroup", "repo", "ACCESSTOKEN")
        );
        yield return (
            new ProjectConnection(new ServerIdentity("example.com"), "group/subgroup", "repo", "ACCESSTOKEN"),
            new ProjectConnection(new ServerIdentity("example.com"), "group/subgroup/repo", "ACCESSTOKEN")
        );

        yield return (
            new ProjectConnection(new ServerIdentity("example.com"), "group/subgroup", "repo", "ACCESSTOKEN"),
            new ProjectConnection(new ProjectIdentity(new ServerIdentity("example.com"), "group/subgroup", "repo"), "ACCESSTOKEN")
        );

        // Comparisons must be case-insensitive (except for the access token)
        yield return (
            new ProjectConnection(new ServerIdentity("example.com"), "user", "repo", "ACCESSTOKEN"),
            new ProjectConnection(new ServerIdentity("EXAMPLE.COM"), "user", "repo", "ACCESSTOKEN")
        );
        yield return (
            new ProjectConnection(new ServerIdentity("example.com"), "group/subgroup", "repo", "ACCESSTOKEN"),
            new ProjectConnection(new ServerIdentity("EXAMPLE.COM"), "group/subgroup", "repo", "ACCESSTOKEN")
        );

        yield return (
            new ProjectConnection(new ServerIdentity("example.com"), "user", "repo", "ACCESSTOKEN"),
            new ProjectConnection(new ServerIdentity("example.com"), "USER", "repo", "ACCESSTOKEN")
        );
        yield return (
            new ProjectConnection(new ServerIdentity("example.com"), "group/subgroup", "repo", "ACCESSTOKEN"),
            new ProjectConnection(new ServerIdentity("example.com"), "GROUP/SUBGROUP", "repo", "ACCESSTOKEN")
        );
        yield return (
            new ProjectConnection(new ServerIdentity("example.com"), "user", "repo", "ACCESSTOKEN"),
            new ProjectConnection(new ServerIdentity("example.com"), "user", "REPO", "ACCESSTOKEN")
        );
        yield return (
            new ProjectConnection(new ServerIdentity("example.com"), "group/subgroup", "repo", "ACCESSTOKEN"),
            new ProjectConnection(new ServerIdentity("example.com"), "group/subgroup", "REPO", "ACCESSTOKEN")
        );
        yield return (
            new ProjectConnection(new ServerIdentity("example.com"), "group/subgroup", "repo", "ACCESSTOKEN"),
            new ProjectConnection(new ServerIdentity("example.com"), "group/SUBGROUP", "repo", "ACCESSTOKEN")
        );
    }

    public IEnumerable<(ProjectConnection left, ProjectConnection right)> GetUnequalTestCases()
    {
        yield return (
            new ProjectConnection(new ServerIdentity("example.com"), "user", "repo", "ACCESSTOKEN"),
            new ProjectConnection(new ServerIdentity("example.net"), "user", "repo", "ACCESSTOKEN")
        );
        yield return (
            new ProjectConnection(new ServerIdentity("example.com"), "group/subgroup", "repo", "ACCESSTOKEN"),
            new ProjectConnection(new ServerIdentity("example.net"), "group/subgroup", "repo", "ACCESSTOKEN")
        );
        yield return (
            new ProjectConnection(new ServerIdentity("example.com"), "user1", "repo", "ACCESSTOKEN"),
            new ProjectConnection(new ServerIdentity("example.com"), "user2", "repo", "ACCESSTOKEN")
        );
        yield return (
            new ProjectConnection(new ServerIdentity("example.com"), "group1/subgroup", "repo", "ACCESSTOKEN"),
            new ProjectConnection(new ServerIdentity("example.com"), "group2/subgroup", "repo", "ACCESSTOKEN")
        );
        yield return (
            new ProjectConnection(new ServerIdentity("example.com"), "user", "repo1", "ACCESSTOKEN"),
            new ProjectConnection(new ServerIdentity("example.com"), "user", "repo2", "ACCESSTOKEN")
        );
        yield return (
            new ProjectConnection(new ServerIdentity("example.com"), "group/subgroup", "repo1", "ACCESSTOKEN"),
            new ProjectConnection(new ServerIdentity("example.com"), "group/subgroup", "repo2", "ACCESSTOKEN")
        );
        yield return (
            new ProjectConnection(new ServerIdentity("example.com"), "user", "repo", "ACCESSTOKEN_1"),
            new ProjectConnection(new ServerIdentity("example.com"), "user", "repo", "ACCESSTOKEN_2")
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
        var ex = Record.Exception(() => new ProjectConnection(new ServerIdentity("example.com"), "group", "project", accessToken!));

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
    }
}
