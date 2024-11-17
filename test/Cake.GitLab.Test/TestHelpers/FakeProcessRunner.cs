using System.Collections.Generic;
using Cake.Core.IO;
using Cake.Testing;

namespace Cake.GitLab.Test;

public class FakeProcessRunner : IProcessRunner
{
    private readonly List<StartedProcess> m_StartedProcesses = new();


    public IReadOnlyList<StartedProcess> StartedProcesses => m_StartedProcesses;


    public IProcess Start(FilePath filePath, ProcessSettings settings)
    {
        m_StartedProcesses.Add(new StartedProcess(filePath, settings));
        return new FakeProcess();
    }
}
