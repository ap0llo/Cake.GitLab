using System;
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
        m_Log = log ?? throw new ArgumentNullException(nameof(log));
        m_FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        m_GitLabClient = gitLabClient ?? throw new ArgumentNullException(nameof(gitLabClient));
    }
}
