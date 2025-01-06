using System;
using System.Collections.Generic;
using NGitLab.Models;

namespace Cake.GitLab.Test.TestHelpers;

public class FakesCollection<T>(Func<ProjectId, T> getFake)
{
    private readonly Dictionary<ProjectId, T> m_Fakes = new Dictionary<ProjectId, T>();

    public T this[ProjectId id]
    {
        get
        {
            if (m_Fakes.TryGetValue(id, out var fake))
            {
                return fake;
            }
            else
            {
                fake = getFake(id);
                m_Fakes[id] = fake;
                return fake;
            }
        }
    }
}
