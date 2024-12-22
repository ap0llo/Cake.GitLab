using System.Globalization;
using Cake.Core.Diagnostics;
using Xunit;

namespace Cake.GitLab.Test;

internal class XunitCakeLog(ITestOutputHelper testOutputHelper) : ICakeLog
{
    public Verbosity Verbosity { get; set; }

    public void Write(Verbosity verbosity, LogLevel level, string format, params object[] args)
    {
        testOutputHelper.WriteLine($"{level.ToString().ToUpper(),12} | {string.Format(CultureInfo.InvariantCulture, format, args)}");
    }
}
