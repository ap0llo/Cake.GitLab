using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Cake.Core;
using Cake.Core.Configuration;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Tooling;
using Cake.GitLab.Testing;
using Cake.Testing;
using NGitLab;
using NGitLab.Mock;
using Xunit;

namespace Cake.GitLab.Test;

public class FakeContext : IGitLabCakeContext
{
    private class MockGitLabProvider(ICakeContext context) : DefaultGitLabProvider(context)
    {
        private readonly Dictionary<string, GitLabServer> m_GitLabServers = new(StringComparer.OrdinalIgnoreCase);

        protected override IGitLabClient GetClient(string serverUrl, string accessToken)
        {
            if (m_GitLabServers.TryGetValue(serverUrl, out var server))
            {
                return server.CreateClient(server.Users.First());
            }
            else
            {
                throw new ArgumentException($"No server for connection {serverUrl} configured");
            }
        }

        public void AddServer(GitLabServer server) => m_GitLabServers.Add(server.Url.ToString(), server);
    }


    private readonly CompositeCakeLog m_LogWithXunitOutput;
    private readonly MockGitLabProvider m_GitLabProvider;

    public FakeFileSystem FileSystem { get; }

    /// <inheritdoc />
    IFileSystem ICakeContext.FileSystem => FileSystem;

    public FakeEnvironment Environment { get; }

    /// <inheritdoc />
    ICakeEnvironment ICakeContext.Environment => Environment;

    IGlobber ICakeContext.Globber => throw new NotImplementedException();

    public FakeLog Log { get; }

    /// <inheritdoc />
    ICakeLog ICakeContext.Log => m_LogWithXunitOutput;

    ICakeArguments ICakeContext.Arguments => throw new NotImplementedException();

    public FakeProcessRunner ProcessRunner { get; } = new();

    /// <inheritdoc />
    IProcessRunner ICakeContext.ProcessRunner => ProcessRunner;

    IRegistry ICakeContext.Registry => throw new NotImplementedException();

    IToolLocator ICakeContext.Tools => throw new NotImplementedException();

    ICakeDataResolver ICakeContext.Data => throw new NotImplementedException();

    ICakeConfiguration ICakeContext.Configuration => throw new NotImplementedException();

    /// <inheritdoc />
    public IGitLabProvider GitLab => m_GitLabProvider;


    public FakeContext(ITestOutputHelper testOutputHelper)
    {
        Log = new FakeLog();
        m_LogWithXunitOutput = new CompositeCakeLog(Log, new XunitCakeLog(testOutputHelper));

        Environment = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? FakeEnvironment.CreateWindowsEnvironment()
            : FakeEnvironment.CreateUnixEnvironment();
        FileSystem = new FakeFileSystem(Environment);

        Environment.WorkingDirectory = FileSystem.GetDirectory("/").Path.Combine("work");
        FileSystem.CreateDirectory(Environment.WorkingDirectory);

        m_GitLabProvider = new MockGitLabProvider(this);
    }



    public void AddServer(GitLabServer server) => m_GitLabProvider.AddServer(server);

}
