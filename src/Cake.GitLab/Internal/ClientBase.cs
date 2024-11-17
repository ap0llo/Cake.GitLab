using Cake.Core.Diagnostics;
using Cake.Core.IO;
using NGitLab;

namespace Cake.GitLab.Internal;

internal class ClientBase(ICakeLog log, IFileSystem fileSystem, IGitLabClient gitLabClient)
{
    protected readonly ICakeLog m_Log = Guard.NotNull(log);
    protected readonly IFileSystem m_FileSystem = Guard.NotNull(fileSystem);
    protected readonly IGitLabClient m_GitLabClient = Guard.NotNull(gitLabClient);
}
