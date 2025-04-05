using System;

namespace Cake.GitLab.Testing;

public class FakeGitLabProviderException : Exception
{
    public FakeGitLabProviderException(string? message) : base(message)
    {
    }
}
