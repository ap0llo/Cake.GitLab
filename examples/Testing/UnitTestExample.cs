﻿using Cake.GitLab.Testing;
using Cake.Testing;
using Moq;
using NGitLab.Models;
using Xunit;

public class UnitTestExample
{
    //begin-snippet: Example-Testing

    [Fact]
    public async Task GitLab_mocking_example()
    {
        //
        // ARRANGE
        //

        // Create a mock of IGitLabProvider
        var gitlabProviderMock = new Mock<IGitLabProvider>();
        {
            gitlabProviderMock
                .Setup(x => x.RepositoryGetBranchesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ProjectId>()))
                .ReturnsAsync(
                    [
                        new Branch() { Name = "example-branch" }
                    ]);
        }

        // Create a mock IGitLabCakeContext
        var contextMock = new Mock<IGitLabCakeContext>();
        {
            contextMock.Setup(x => x.GitLab).Returns(gitlabProviderMock.Object);

            contextMock.Setup(x => x.Log).Returns(new FakeLog());

            var fileSystem = new FakeFileSystem(FakeEnvironment.CreateUnixEnvironment());
            contextMock.Setup(x => x.FileSystem).Returns(fileSystem);
        }

        //
        // ACT
        //
        var branches = await contextMock.Object.GitLabRepositoryGetBranchesAsync("https://example.com", "ACCESSTOKEN", 23);

        //
        // ASSERT
        //
        Assert.Collection(
            branches,
            branch => Assert.Equal("example-branch", branch.Name)
        );
    }

    //end-snippet
}
