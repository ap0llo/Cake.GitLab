using Cake.Testing;
using Moq;
using NGitLab.Mock.Config;
using Xunit;


public class UnitTestExample
{
    //begin-snippet: Example-Testing

    [Fact]
    public void GitLab_mocking_example()
    {
        //
        // ARRANGE
        //

        // Create a mock GitLab server (using NGitLab.Mock library)
        var gitLabConfig =
            new GitLabConfig() { Url = "https://example.com" }
                .WithUser("user1", isDefault: true)
                .WithGroup("example-group")
                .WithProjectOfFullPath(
                    fullPath: "example-project",
                    id: 23,
                    configure: project =>
                    {
                        project.DefaultBranch = "example-branch";
                        project.WithCommit("Intial commit");
                    }
                );

        // Create a mock ICakeContext
        var contextMock = new Mock<ICakeContext>();
        {
            // Add implementation of IGitlabClientFactory to the context
            contextMock
                .As<IGitLabClientFactory>()
                .Setup(x => x.GetClient(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(gitLabConfig.BuildClient());

            var log = new FakeLog();
            contextMock.Setup(x => x.Log).Returns(log);

            var fileSystem = new FakeFileSystem(FakeEnvironment.CreateUnixEnvironment());
            contextMock.Setup(x => x.FileSystem).Returns(fileSystem);
        }

        //
        // ACT
        //
        var branches = contextMock.Object.GitLabRepositoryGetBranches("https://example.com", "ACCESSTOKEN", 23);

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
