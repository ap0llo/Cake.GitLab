using Cake.Core.Diagnostics;

namespace Cake.GitLab.Test;

internal class CompositeCakeLog(params ICakeLog[] logs) : ICakeLog
{
    public Verbosity Verbosity { get; set; }

    public void Write(Verbosity verbosity, LogLevel level, string format, params object[] args)
    {
        foreach (var log in logs)
        {
            log.Write(verbosity, level, format, args);
        }
    }
}
