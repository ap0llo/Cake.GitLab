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
/// Tests for <see cref="GitLabServerIdentity"/>
/// </summary>
public class GitLabServerIdentityTest : EqualityTest<GitLabServerIdentity, GitLabServerIdentityTest>, IEqualityTestDataProvider<GitLabServerIdentity>
{
    public IEnumerable<(GitLabServerIdentity left, GitLabServerIdentity right)> GetEqualTestCases()
    {
        yield return (
            new GitLabServerIdentity("example.com"),
            new GitLabServerIdentity("example.com")
        );

        // Comparisons must be case-insensitive
        yield return (
            new GitLabServerIdentity("example.com"),
            new GitLabServerIdentity("EXAMPLE.COM")
        );
    }

    public IEnumerable<(GitLabServerIdentity left, GitLabServerIdentity right)> GetUnequalTestCases()
    {
        yield return (
            new GitLabServerIdentity("example.com"),
            new GitLabServerIdentity("example.net")
        );
    }


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("\t")]
    public void Host_must_not_be_null_or_whitespace(string? host)
    {
        // ARRANGE

        // ACT
        var ex = Record.Exception(() => new GitLabServerIdentity(host!));

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
    }
}
