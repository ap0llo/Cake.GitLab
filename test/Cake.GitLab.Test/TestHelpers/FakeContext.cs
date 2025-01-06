using System;
using System.Runtime.InteropServices;
using Cake.Core;
using Cake.Core.Configuration;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Tooling;
using Cake.Testing;
using Xunit;

namespace Cake.GitLab.Test;

public class FakeContext : ICakeContext
{
    private readonly CompositeCakeLog m_LogWithXunitOutput;

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
    }
}
