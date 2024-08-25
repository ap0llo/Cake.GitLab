using System;
using NGitLab.Models;

namespace Cake.GitLab.Test.TestHelpers;

internal static class Extensions
{
    public static ProjectId AsProjectId(this object projectIdOrPath)
    {
        return projectIdOrPath switch
        {
            string str => str,
            long longValue => longValue,
            _ => throw new InvalidOperationException()
        };
    }

}
