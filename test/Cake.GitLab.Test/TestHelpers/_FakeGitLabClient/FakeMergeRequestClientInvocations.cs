using System.Collections.Generic;
using NGitLab.Models;

namespace Cake.GitLab.Test.TestHelpers;

public class FakeMergeRequestClientInvocations
{
    public List<MergeRequestQuery> Get { get; } = [];
}
