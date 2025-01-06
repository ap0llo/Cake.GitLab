using Moq;
using NGitLab;
using NGitLab.Models;

namespace Cake.GitLab.Test;

public class MoqGitLabClientMock
{
    public class PipelineClientMock
    {
        public Mock<IPipelineClient> Mock { get; } = new Mock<IPipelineClient>(MockBehavior.Strict);

        public IPipelineClient Object => Mock.Object;
    }

    public Mock<IGitLabClient> Mock { get; }

    public IGitLabClient Object => Mock.Object;

    public PipelineClientMock Pipelines { get; }


    public MoqGitLabClientMock()
    {
        Pipelines = new PipelineClientMock();

        Mock = new Mock<IGitLabClient>(MockBehavior.Strict);
        Mock
            .Setup(x => x.GetPipelines(It.IsAny<ProjectId>()))
            .Returns(Pipelines.Object);
    }
}
