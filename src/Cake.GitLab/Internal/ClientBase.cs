using Cake.Core.Diagnostics;
using Cake.Core.IO;
using NGitLab;

namespace Cake.GitLab.Internal;

internal class ClientBase
{
    protected readonly ICakeLog m_Log;
    protected readonly IFileSystem m_FileSystem;
    protected readonly IGitLabClient m_GitLabClient;

    public ClientBase(ICakeLog log, IFileSystem fileSystem, IGitLabClient gitLabClient)
    {
        m_Log = Guard.NotNull(log);
        m_FileSystem = Guard.NotNull(fileSystem);
        m_GitLabClient = Guard.NotNull(gitLabClient);
    }
}
