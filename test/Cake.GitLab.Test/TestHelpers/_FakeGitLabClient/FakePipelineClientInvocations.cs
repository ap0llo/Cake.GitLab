using System.Collections.Generic;
using NGitLab.Models;

namespace Cake.GitLab.Test.TestHelpers;

public class FakePipelineClientInvocations
{
    public List<PipelineJobQuery> GetJobsAsync { get; } = [];
}
