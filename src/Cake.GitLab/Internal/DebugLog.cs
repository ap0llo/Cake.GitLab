using System;
using Cake.Core.Diagnostics;

namespace Cake.GitLab.Internal;

/// <summary>
/// Specialized implementation of <see cref="ICakeLog"/> that adds a prefix to the log message when the log level is <c>Debug</c>
/// </summary>
internal class DebugLog(ICakeLog innerLog, string prefix) : ICakeLog
{
    public Verbosity Verbosity
    {
        get => innerLog.Verbosity;
        set => innerLog.Verbosity = value;
    }

    public void Write(Verbosity verbosity, LogLevel level, string format, params object[] args)
    {
        if (level <= LogLevel.Debug)
        {
            format = String.Concat(prefix, ": ", format);
        }

        innerLog.Write(verbosity, level, format, args);
    }
}
