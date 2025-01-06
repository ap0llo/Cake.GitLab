using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cake.GitLab.Test.TestHelpers;

public static class GitLabCollectionResponse
{
    private class EmptyGitLabCollectionResponse<T> : NGitLab.GitLabCollectionResponse<T>
    {
        public override async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
        {
            await Task.Yield();
            yield break;
        }

        public override IEnumerator<T> GetEnumerator()
        {
            yield break;
        }
    }

    public static NGitLab.GitLabCollectionResponse<T> Empty<T>() => new EmptyGitLabCollectionResponse<T>();
}
